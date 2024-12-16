namespace ApiGateway.Models.Users
{
    public record RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
