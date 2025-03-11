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

        [HttpDelete("~/api/users")]
        public async Task<IActionResult> DeleteUser(CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _userService.DeleteUserAsync(userId!, cancellationToken);

            return result.Match<IActionResult>(
                deletedUserId => Ok(new { userId = deletedUserId }),
                _ => NotFound("User not found."));
        }
    }
}
