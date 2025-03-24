using ApiGateway.Models.Weights;
using ApiGateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/weights")]
    public class WeightGatewayController : ControllerBase
    {
        private readonly IHttpService _httpService;
        private readonly EndpointsService _microserviceEndpoints;

        public WeightGatewayController(IHttpService httpService, EndpointsService microserviceEndpoints)
        {
            _httpService = httpService;
            _microserviceEndpoints = microserviceEndpoints;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeights()
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights";
            return await _httpService.ForwardGetRequest(weightServiceUrl, userId);
        }

        [HttpGet("{weightId}")]
        public async Task<IActionResult> GetWeightById([FromRoute] string weightId)
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}";
            return await _httpService.ForwardGetRequest(weightServiceUrl, userId);
        }

        [HttpPost]
        public async Task<IActionResult> AddWeight([FromForm] AddWeightRequest addWeightRequest)
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights";
            return await _httpService.ForwardMultipartRequest(weightServiceUrl, addWeightRequest, userId);
        }

        [HttpPut("{weightId}")]
        public async Task<IActionResult> EditWeight([FromRoute] string weightId,
            [FromBody] EditWeightRequest editWeightRequest)
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}";
            return await _httpService.ForwardPutRequest(weightServiceUrl, editWeightRequest, userId);
        }

        [HttpPost("{weightId}/files")]
        public async Task<IActionResult> AddFileToWeight([FromRoute] string weightId,
            [FromForm] AddFileRequest addFileRequest)
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}/files";
            return await _httpService.ForwardMultipartRequest(weightServiceUrl, addFileRequest, userId);
        }

        [HttpDelete("{weightId}/files/{fileId}")]
        public async Task<IActionResult> DeleteFileFromWeight([FromRoute] string weightId,
            [FromRoute] string fileId)
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}/files/{fileId}";
            return await _httpService.ForwardDeleteRequest(weightServiceUrl, userId);
        }

        [HttpDelete("{weightId}")]
        public async Task<IActionResult> DeleteWeight([FromRoute] string weightId)
        {
            var userId = User.FindFirstValue("userId");

            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}";
            return await _httpService.ForwardDeleteRequest(weightServiceUrl, userId);
        }
    }
}