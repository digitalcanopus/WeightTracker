using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserGatewayController : ControllerBase
    {
        private readonly HttpService _httpService;
        private readonly EndpointsService _microserviceEndpoints;

        public UserGatewayController(HttpService httpService, EndpointsService microserviceEndpoints)
        {
            _httpService = httpService;
            _microserviceEndpoints = microserviceEndpoints;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userServiceUrl = $"{_microserviceEndpoints.UserMicroservice}/api/users/{id}";
            return await _httpService.ForwardGetRequest(userServiceUrl);
        }
    }
}