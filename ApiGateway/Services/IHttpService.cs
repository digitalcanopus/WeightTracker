using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Services
{
    public interface IHttpService
    {
        public Task<IActionResult> ForwardGetRequest(string url);
        public Task<IActionResult> ForwardPostRequest<T>(string url, T body);
        public Task<IActionResult> ForwardPutRequest<T>(string url, T body);
        public Task<IActionResult> ForwardDeleteRequest(string url);
    }
}
