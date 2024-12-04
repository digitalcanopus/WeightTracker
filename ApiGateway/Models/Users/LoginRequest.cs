namespace ApiGateway.Models.Users
{
    public record LoginRequest
    (
        string Username,
        string Password
    );
}
