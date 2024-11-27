using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserGatewayController : ControllerBase
    {
        private readonly HttpService _httpService;
        private readonly EndpointsService _microserviceEndpoints;

        public UserGatewayController(HttpService httpService, IOptions<EndpointsService> microserviceEndpoints)
        {
            _httpService = httpService;
            _microserviceEndpoints = microserviceEndpoints.Value;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userServiceUrl = $"{_microserviceEndpoints.UserMicroservice}/api/users/{id}";
            return await _httpService.ForwardRequest(userServiceUrl);
        }
    }
}