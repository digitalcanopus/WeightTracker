using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/weights")]
    public class WeightGatewayController
    {
        private readonly HttpService _httpService;
        private readonly EndpointsService _microserviceEndpoints;

        public WeightGatewayController(HttpService httpService, EndpointsService microserviceEndpoints)
        {
            _httpService = httpService;
            _microserviceEndpoints = microserviceEndpoints;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeightById(string id)
        {
            var weightServiceUrl = $"{_microserviceEndpoints.WeightMicroservice}/api/weights/{id}";
            Console.WriteLine(_microserviceEndpoints.WeightMicroservice);
            return await _httpService.ForwardRequest(weightServiceUrl);
        }
    }
}
