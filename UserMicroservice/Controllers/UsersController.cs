using Microsoft.AspNetCore.Mvc;
using UserMicroservice.Services.Tokens;
using UserMicroservice.Services.Users;
using UserMicroservice.Services.Users.Requests;

namespace UserMicroservice.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UsersController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("~/api/users/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _userService.LoginAsync(loginRequest, cancellationToken);

            return result.Match<IActionResult>(
                user =>
                {
                    var token = _tokenService.GenerateToken(user);
                    return Ok(new { token });
                },
                _ => Unauthorized("Invalid username or password."));
        }

        [HttpPost("~/api/users/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _userService.RegisterAsync(registerRequest, cancellationToken);

            return result.Match<IActionResult>(
                userId => Ok(new { userId }),
                _ => Conflict("User already exists."));
        }
    }
}
