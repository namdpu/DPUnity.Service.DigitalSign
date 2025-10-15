using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace DigitalSignService.Business.Service3th
{
    public class HttpService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;
        protected readonly string _baseEndpoint;

        public HttpService(IHttpContextAccessor httpContextAccessor, ILogger<HttpService> logger, string baseEndpoint)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _baseEndpoint = baseEndpoint;
        }

        protected HttpClient CreateHttpClient(string? token = null, Dictionary<string, string>? customHeaders = null, bool addToken = true)
        {
            var client = new HttpClient();
            var accessToken = token ?? _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(accessToken) && addToken)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            return client;
        }

        protected async Task<T?> PostAsync<T>(string url, object data, string? token = null, Dictionary<string, string>? customHeaders = null)
        {
            try
            {
                using var client = CreateHttpClient(token, customHeaders);
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseEndpoint}/{url}", content);
                if (typeof(T).IsAssignableFrom(response.GetType()))
                    return (T)(object)response;

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                    return JsonConvert.DeserializeObject<T>(responseContent);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when calling POST {url}");
                return default;
            }
        }

        protected async Task<T?> GetAsync<T>(string url, string? token = null, Dictionary<string, string>? customHeaders = null)
        {
            try
            {
                using var client = CreateHttpClient(token, customHeaders);
                var response = await client.GetAsync($"{_baseEndpoint}/{url}");
                if (typeof(T).IsAssignableFrom(response.GetType()))
                    return (T)(object)response;

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                    return JsonConvert.DeserializeObject<T>(responseContent);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when calling GET {url}");
                return default;
            }
        }

        protected async Task<HttpResponseMessage?> HeadAsync(string url, string? token = null, Dictionary<string, string>? customHeaders = null, bool addToken = true)
        {
            try
            {
                using var client = CreateHttpClient(token, customHeaders, addToken);
                var request = new HttpRequestMessage(HttpMethod.Head, $"{(String.IsNullOrEmpty(_baseEndpoint) ? url : $"{_baseEndpoint}/{url}")}");
                var response = await client.SendAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when calling HEAD {url}");
                return null;
            }
        }
    }
}
