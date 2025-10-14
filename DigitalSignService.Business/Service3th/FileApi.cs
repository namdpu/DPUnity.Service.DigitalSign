using DigitalSignService.Business.Service3th;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DigitalSignService.Business.Service3th
{
    public class FileApi : HttpService
    {
        public FileApi(IHttpContextAccessor httpContextAccessor, ILogger<FileApi> logger) 
            : base(httpContextAccessor, logger, string.Empty)
        {
        }

        /// <summary>
        /// Lấy dung lượng file từ URL bằng HTTP HEAD request
        /// </summary>
        /// <param name="url">URL của file</param>
        /// <param name="token">Access token (optional)</param>
        /// <param name="customHeaders">Custom headers (optional)</param>
        /// <returns>Dung lượng file theo bytes, hoặc -1 nếu không lấy được</returns>
        public async Task<long> GetFileSizeAsync(string url, string? token = null, Dictionary<string, string>? customHeaders = null)
        {
            try
            {
                using var response = await HeadAsync(url, token, customHeaders);
                if (response?.IsSuccessStatusCode == true)
                {
                    // Thử lấy Content-Length header
                    if (response.Content.Headers.ContentLength.HasValue)
                    {
                        return response.Content.Headers.ContentLength.Value;
                    }

                    // Nếu không có Content-Length, thử lấy từ custom headers
                    if (response.Headers.Contains("Content-Length"))
                    {
                        var contentLengthStr = response.Headers.GetValues("Content-Length").FirstOrDefault();
                        if (long.TryParse(contentLengthStr, out var contentLength))
                        {
                            return contentLength;
                        }
                    }
                }

                _logger.LogWarning($"Could not get file size from HEAD request to {url}. Status: {response?.StatusCode}");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when getting file size from {url}");
                return -1;
            }
        }
    }
}