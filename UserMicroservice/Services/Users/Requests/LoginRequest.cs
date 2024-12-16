namespace UserMicroservice.Services.Users.Requests
{
    public record LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
