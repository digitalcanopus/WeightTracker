using ApiGateway.Models.Weights;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> ForwardGetRequest(string url, string? userId = null)
        {
            return await ForwardRequest(HttpMethod.Get, url, userId);
        }

        public async Task<IActionResult> ForwardPostRequest<T>(string url, T body, string? userId = null)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await ForwardRequest(HttpMethod.Post, url, userId, content);
        }

        public async Task<IActionResult> ForwardPutRequest<T>(string url, T body, string? userId = null)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await ForwardRequest(HttpMethod.Put, url, userId, content);
        }

        public async Task<IActionResult> ForwardDeleteRequest(string url, string? userId = null)
        {
            return await ForwardRequest(HttpMethod.Delete, url, userId);
        }

        public async Task<IActionResult> ForwardMultipartRequest<T>(string url, T body, string? userId = null)
        {
            var multipartContent = new MultipartFormDataContent();

            if (body is AddFileRequest fileRequest && fileRequest.File != null)
            {
                var fileStreamContent = new StreamContent(fileRequest.File.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(fileRequest.File.ContentType);
                multipartContent.Add(fileStreamContent, "File", fileRequest.File.FileName);
            }
            else if (body is AddWeightRequest weightRequest)
            {
                multipartContent.Add(new StringContent(weightRequest.WeightValue.ToString()), "WeightValue");
                multipartContent.Add(new StringContent(weightRequest.Date.ToString("o")), "Date");

                if (weightRequest.Files != null)
                {
                    foreach (var file in weightRequest.Files)
                    {
                        var fileStreamContent = new StreamContent(file.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        multipartContent.Add(fileStreamContent, "Files", file.FileName);
                    }
                }
            }

            return await ForwardRequest(HttpMethod.Post, url, userId, multipartContent);
        }

        private async Task<IActionResult> ForwardRequest(HttpMethod method, string url, string? userId = null, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (userId != null)
                request.Headers.Add("X-User-Id", userId);

            if (content != null)
                request.Content = content;

            var response = await _httpClient.SendAsync(request);

            return await CreateActionResultFromResponse(response);
        }

        private static async Task<IActionResult> CreateActionResultFromResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new StatusCodeResult((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
