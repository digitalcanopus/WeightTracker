namespace UserMicroservice.Services.Users.Models
{
    public class UserDetailsDto
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
