using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Services
{
    public interface IHttpService
    {
        public Task<IActionResult> ForwardGetRequest(string url, string? userId = null);
        public Task<IActionResult> ForwardPostRequest<T>(string url, T body, string? userId = null);
        public Task<IActionResult> ForwardPutRequest<T>(string url, T body, string? userId = null);
        public Task<IActionResult> ForwardDeleteRequest(string url, string? userId = null);
        public Task<IActionResult> ForwardMultipartRequest<T>(string url, T body, string? userId = null);
    }
}
