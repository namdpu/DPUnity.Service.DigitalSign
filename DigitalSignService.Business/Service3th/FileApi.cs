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
                using var response = await HeadAsync(url, token, customHeaders, false);
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

                // Nếu HEAD request bị chặn (403) hoặc không thành công, thử dùng Range request
                if (response?.StatusCode == System.Net.HttpStatusCode.Forbidden || !response?.IsSuccessStatusCode == true)
                {
                    return await GetFileSizeUsingRangeAsync(url, token, customHeaders);
                }

                _logger.LogWarning($"Could not get file size from HEAD request to {url}. Status: {response?.StatusCode}");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when getting file size from {url}. Trying Range request as fallback...");
                // Fallback to Range request nếu HEAD request throw exception
                try
                {
                    return await GetFileSizeUsingRangeAsync(url, token, customHeaders);
                }
                catch (Exception rangeEx)
                {
                    _logger.LogError(rangeEx, $"Range request also failed for {url}");
                    return -1;
                }
            }
        }

        /// <summary>
        /// Lấy dung lượng file từ URL bằng Range request (bytes=0-0)
        /// Phương thức này hữu ích khi HEAD request bị chặn (403 Forbidden)
        /// </summary>
        /// <param name="url">URL của file</param>
        /// <param name="token">Access token (optional)</param>
        /// <param name="customHeaders">Custom headers (optional)</param>
        /// <returns>Dung lượng file theo bytes, hoặc -1 nếu không lấy được</returns>
        public async Task<long> GetFileSizeUsingRangeAsync(string url, string? token = null, Dictionary<string, string>? customHeaders = null)
        {
            try
            {
                var client = CreateHttpClient(token, customHeaders, false);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                // Thêm Range header để chỉ lấy byte đầu tiên
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 0);

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                {
                    // Với Range request, server thường trả về Content-Range header
                    // Format: "bytes 0-0/12345" trong đó 12345 là tổng kích thước file
                    if (response.Content.Headers.ContentRange != null)
                    {
                        var totalLength = response.Content.Headers.ContentRange.Length;
                        if (totalLength.HasValue)
                        {
                            return totalLength.Value;
                        }
                    }
                }

                _logger.LogWarning($"Could not get file size from Range request to {url}. Status: {response.StatusCode}");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when getting file size using Range request from {url}");
                return -1;
            }
        }
    }
}