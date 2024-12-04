using UserMicroservice.Services.Users.Models;

namespace UserMicroservice.Services.Tokens
{
    public interface ITokenService
    {
        public string GenerateToken(UserDetailsDto userDetails);
    }
}
