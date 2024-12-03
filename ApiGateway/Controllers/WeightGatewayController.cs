using ApiGateway.Models.Weights;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/weights")]
    public class WeightGatewayController
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
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights";
            return await _httpService.ForwardGetRequest(weightServiceUrl);
        }

        [HttpGet("{weightId}")]
        public async Task<IActionResult> GetWeightById([FromRoute] string weightId)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}";
            return await _httpService.ForwardGetRequest(weightServiceUrl);
        }

        [HttpPost]
        public async Task<IActionResult> AddWeight([FromBody] AddWeightRequest addWeightRequest)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights";
            return await _httpService.ForwardPostRequest(weightServiceUrl, addWeightRequest);
        }

        [HttpPut("{weightId}")]
        public async Task<IActionResult> EditWeight([FromRoute] string weightId,
            [FromBody] EditWeightRequest editWeightRequest)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}";
            return await _httpService.ForwardPutRequest(weightServiceUrl, editWeightRequest);
        }

        [HttpPost("{weightId}/files")]
        public async Task<IActionResult> AddFileToWeight([FromRoute] string weightId,
            [FromForm] AddFileRequest addFileRequest)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}/files";
            return await _httpService.ForwardMultipartRequest(weightServiceUrl, addFileRequest);
        }

        [HttpDelete("{weightId}/files/{fileId}")]
        public async Task<IActionResult> DeleteFileFromWeight([FromRoute] string weightId,
            [FromRoute] string fileId)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}/files/{fileId}";
            return await _httpService.ForwardDeleteRequest(weightServiceUrl);
        }

        [HttpDelete("{weightId}")]
        public async Task<IActionResult> DeleteWeight([FromRoute] string weightId)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{weightId}";
            return await _httpService.ForwardDeleteRequest(weightServiceUrl);
        }
    }
}