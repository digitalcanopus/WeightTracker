namespace UserMicroservice.Services.Users.Models
{
    public class UserDetailsDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
