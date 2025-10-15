using DigitalSignService.Business.IServices.Sign;
using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.Models;
using DPUStorageService.APIs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace DigitalSignService.Business.Services.Sign
{
    public abstract class BaseSigningProvider : ISigningProvider
    {
        protected readonly ILogger _logger;
        protected readonly DigitalSignSettings _defaultSettings;
        protected readonly CachingService _cachingService;
        protected readonly AppSetting _appSetting;
        private static readonly object _customHeaderLock = new object();
        protected readonly IApiStorage _apiStorage;

        public abstract string Name { get; }
        protected BaseSigningProvider(ILogger logger, IOptions<DigitalSignSettings> options, CachingService cachingService, IApiStorage apiStorage, IOptions<AppSetting> options1)
        {
            _logger = logger;
            _defaultSettings = options.Value;
            _cachingService = cachingService;
            _apiStorage = apiStorage;
            _appSetting = options1.Value;
            if (string.IsNullOrEmpty(_apiStorage.Configuration.CustomHeader.GetValueOrDefault(nameof(_appSetting.InternalKey))))
                lock (_customHeaderLock)
                {
                    if (string.IsNullOrEmpty(_apiStorage.Configuration.CustomHeader.GetValueOrDefault(nameof(_appSetting.InternalKey))))
                    {
                        _apiStorage.Configuration.CustomHeader.Add(nameof(_appSetting.InternalKey), _appSetting.InternalKey);
                    }
                }
        }

        /// <summary>
        /// Downloads file from URL with support for unknown file size
        /// </summary>
        /// <param name="url">File URL to download</param>
        /// <param name="maxConcurrentDownloads">Maximum concurrent download parts (default: 5)</param>
        /// <param name="partSizeBytes">Size of each download part in bytes (default: 1MB)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream containing downloaded file content</returns>
        private async Task<Stream?> DownloadFile(string url, int maxConcurrentDownloads = 5,
            long partSizeBytes = 5 * 1024 * 1024, CancellationToken cancellationToken = default)
        {
            try
            {
                // Configure handler to bypass SSL validation (use cautiously)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        // Optional: Add specific certificate validation logic
                        // Return true to accept all certificates (not recommended for production)
                        return true;
                    },
                };

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromMinutes(15); // 15 minutes timeout

                // First, try to get file size using HEAD request
                var fileSize = await GetFileSizeAsync(client, url, cancellationToken);

                if (fileSize.HasValue && fileSize.Value > 0)
                {
                    _logger.LogInformation($"Downloading file with known size: {fileSize.Value} bytes");
                    return await DownloadFileWithKnownSize(client, url, fileSize.Value,
                        maxConcurrentDownloads, partSizeBytes, cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Downloading file with unknown size using streaming approach");
                    return await DownloadFileWithUnknownSize(client, url, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file from URL: {url}");
                throw;
            }
        }

        /// <summary>
        /// Gets file size using HEAD request
        /// </summary>
        private async Task<long?> GetFileSizeAsync(HttpClient client, string url, CancellationToken cancellationToken)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                using var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.Headers.ContentLength;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not determine file size using HEAD request");
            }

            return null;
        }

        /// <summary>
        /// Downloads file when size is known using parallel chunks
        /// </summary>
        private async Task<Stream> DownloadFileWithKnownSize(HttpClient client, string url, long fileSize,
            int maxConcurrentDownloads, long partSizeBytes, CancellationToken cancellationToken)
        {
            var bytesDownloaded = 0L;
            var downloadTasks = new List<Task<(byte[] data, int index)>>();
            var partIndex = 0;

            using var semaphore = new SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);

            // Create download tasks for each part
            while (bytesDownloaded < fileSize)
            {
                var startByte = bytesDownloaded;
                var endByte = Math.Min(bytesDownloaded + partSizeBytes - 1, fileSize - 1);
                var currentPartIndex = partIndex;

                downloadTasks.Add(DownloadPartWithRetry(client, url, startByte, endByte,
                    currentPartIndex, semaphore, cancellationToken));

                bytesDownloaded += (endByte - startByte + 1);
                partIndex++;
            }

            // Wait for all parts to complete
            var completedParts = await Task.WhenAll(downloadTasks);

            // Sort parts by index and combine
            var sortedParts = completedParts.OrderBy(p => p.index).Select(p => p.data).ToArray();
            return CombineChunksIntoStream(sortedParts);
        }

        /// <summary>
        /// Downloads file when size is unknown using streaming approach
        /// </summary>
        private async Task<Stream> DownloadFileWithUnknownSize(HttpClient client, string url, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            var memoryStream = new MemoryStream();
            using var contentStream = await response.Content.ReadAsStreamAsync();

            var buffer = new byte[8192]; // 8KB buffer
            int bytesRead;
            long totalBytesRead = 0;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await memoryStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;

                // Log progress every 5MB
                if (totalBytesRead % (5 * 1024 * 1024) == 0)
                {
                    _logger.LogInformation($"Downloaded {totalBytesRead / (5 * 1024 * 1024)} MB so far...");
                }
            }

            _logger.LogInformation($"Download completed. Total size: {totalBytesRead} bytes");
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Downloads a specific part of file with retry logic
        /// </summary>
        private async Task<(byte[] data, int index)> DownloadPartWithRetry(HttpClient client, string url,
            long startByte, long endByte, int partIndex, SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                const int maxRetries = 3;
                Exception lastException = null;

                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var partData = await DownloadPart(client, url, startByte, endByte, cancellationToken);

                        _logger.LogDebug($"Downloaded part {partIndex}: {startByte}-{endByte} ({partData.Length} bytes)");
                        return (partData, partIndex);
                    }
                    catch (Exception ex) when (retry < maxRetries - 1)
                    {
                        lastException = ex;
                        _logger.LogWarning($"Retry {retry + 1} for part {partIndex} ({startByte}-{endByte}): {ex.Message}");

                        // Wait before retry with exponential backoff
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retry)), cancellationToken);
                    }
                }

                throw new Exception($"Failed to download part {partIndex} after {maxRetries} retries", lastException);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Downloads a specific byte range from URL
        /// </summary>
        private async Task<byte[]> DownloadPart(HttpClient client, string url, long startByte, long endByte, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startByte, endByte);

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var partBytes = await response.Content.ReadAsByteArrayAsync();
            return partBytes;
        }

        /// <summary>
        /// Combines byte arrays into a single stream
        /// </summary>
        private Stream CombineChunksIntoStream(byte[][] chunks)
        {
            try
            {
                var memoryStream = new MemoryStream();

                foreach (var chunk in chunks)
                {
                    if (chunk != null && chunk.Length > 0)
                    {
                        memoryStream.Write(chunk, 0, chunk.Length);
                    }
                }

                memoryStream.Position = 0; // Reset position for reading
                _logger.LogInformation($"Combined {chunks.Length} chunks into stream of {memoryStream.Length} bytes");

                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error combining chunks into stream");
                throw;
            }
        }

        /// <summary>
        /// Public method to download file - can be used by derived classes
        /// </summary>
        /// <param name="url">File URL to download</param>
        /// <param name="maxConcurrentDownloads">Maximum concurrent downloads (default: 5)</param>
        /// <param name="partSizeBytes">Part size in bytes (default: 1MB)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream containing file content</returns>
        protected async Task<byte[]?> DownloadFileAsync(string url, int maxConcurrentDownloads = 5,
            long partSizeBytes = 5 * 1024 * 1024, CancellationToken cancellationToken = default)
        {
            var stream = await DownloadFile(url, maxConcurrentDownloads, partSizeBytes, cancellationToken);
            if (stream == null) return null;
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            return memoryStream.ToArray();
        }

        public virtual Task<string> SignCAPDF(SignReq req, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<SignStatusDTO> GetSignStatus(SignReq req, string transactionId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<SignStatusDTO> HandleWebhook(string dataString, string documentName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected byte[] RotateImage(byte[] imageBytes, int rotate)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // ---- WINDOWS VERSION (System.Drawing) ----
                var rotateType = System.Drawing.RotateFlipType.RotateNoneFlipNone;
                switch (rotate)
                {
                    case 90:
                        rotateType = System.Drawing.RotateFlipType.Rotate90FlipNone;
                        break;
                    case 180:
                        rotateType = System.Drawing.RotateFlipType.Rotate180FlipNone;
                        break;
                    case 270:
                        rotateType = System.Drawing.RotateFlipType.Rotate270FlipNone;
                        break;
                }

                using (var ms = new MemoryStream(imageBytes))
                using (var image = System.Drawing.Image.FromStream(ms))
                {
                    image.RotateFlip(rotateType);

                    using (var output = new MemoryStream())
                    {
                        image.Save(output, System.Drawing.Imaging.ImageFormat.Png);
                        return output.ToArray();
                    }
                }
            }
            else
            {
                // ---- NON-WINDOWS VERSION (ImageSharp) ----
                using var ms = new MemoryStream(imageBytes);
                using var image = SixLabors.ImageSharp.Image.Load(ms);

                image.Mutate(x => x.Rotate((float)rotate));

                using var output = new MemoryStream();
                image.Save(output, new PngEncoder());
                return output.ToArray();
            }
        }
    }
}
