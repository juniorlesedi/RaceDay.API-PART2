
using System.Net.Http.Json;

namespace RaceDay.MVC.Services
{
    public class RaceDayApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        private const string ApiCookieKey = "RaceDayApiCookie";

        public RaceDayApiService(
            HttpClient httpClient,
            IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(
            string endpoint, T data)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, endpoint);

            request.Content = JsonContent.Create(data);

            return await SendAsync(request);
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, endpoint);

            using var response = await SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<HttpResponseMessage> PutAsync<T>(
            string endpoint, T data)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put, endpoint);

            request.Content = JsonContent.Create(data);

            return await SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(
            string endpoint)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete, endpoint);

            return await SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request)
        {
            var session = _contextAccessor.HttpContext?.Session;

            var savedCookie = session?.GetString(ApiCookieKey);

            if (!string.IsNullOrEmpty(savedCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie", savedCookie);
            }

            var response = await _httpClient.SendAsync(request);

            if (response.Headers.TryGetValues(
                "Set-Cookie", out var cookies))
            {
                var sessionCookie = cookies
                    .Select(cookie => cookie.Split(';')[0])
                    .FirstOrDefault(cookie =>
                        cookie.StartsWith(
                            ".AspNetCore.Session=",
                            StringComparison.OrdinalIgnoreCase));

                if (sessionCookie != null)
                {
                    session?.SetString(
                        ApiCookieKey, sessionCookie);
                }
            }

            return response;
        }
    }
}
