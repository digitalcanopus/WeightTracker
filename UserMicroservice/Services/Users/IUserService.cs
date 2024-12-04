using UserMicroservice.Services.Users.Requests;
using OneOf;
using OneOf.Types;
using UserMicroservice.Services.Users.Models;

namespace UserMicroservice.Services.Users
{
    public interface IUserService
    {
        public Task<OneOf<UserDetailsDto, NotFound>> GetUserAsync(
            LoginRequest loginRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, Error>> RegisterAsync(
            RegisterRequest registerRequest,
            CancellationToken cancellationToken = default);
    }
}
