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

        public async Task<IActionResult> ForwardGetRequest(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await CreateActionResultFromResponse(response);
        }

        public async Task<IActionResult> ForwardPostRequest<T>(string url, T body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return await CreateActionResultFromResponse(response);
        }

        public async Task<IActionResult> ForwardPutRequest<T>(string url, T body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            return await CreateActionResultFromResponse(response);
        }

        public async Task<IActionResult> ForwardDeleteRequest(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return await CreateActionResultFromResponse(response);
        }

        public async Task<IActionResult> ForwardMultipartRequest<T>(string url, T body)
        {
            var multipartContent = new MultipartFormDataContent();

            if (body is AddFileRequest fileRequest && fileRequest.File != null)
            {
                var fileStreamContent = new StreamContent(fileRequest.File.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(fileRequest.File.ContentType);
                multipartContent.Add(fileStreamContent, "File", fileRequest.File.FileName);
            }

            var response = await _httpClient.PostAsync(url, multipartContent);
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
