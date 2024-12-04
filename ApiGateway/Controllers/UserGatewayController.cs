using ApiGateway.Models.Users;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserGatewayController
    {
        private readonly IHttpService _httpService;
        private readonly EndpointsService _microserviceEndpoints;

        public UserGatewayController(IHttpService httpService, EndpointsService microserviceEndpoints)
        {
            _httpService = httpService;
            _microserviceEndpoints = microserviceEndpoints;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var userServiceUrl = $"{_microserviceEndpoints.UserMicroservice}/api/users/login";
            return await _httpService.ForwardPostRequest(userServiceUrl, loginRequest);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var userServiceUrl = $"{_microserviceEndpoints.UserMicroservice}/api/users/register";
            return await _httpService.ForwardPostRequest(userServiceUrl, registerRequest);
        }
    }
}