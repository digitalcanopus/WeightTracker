namespace ApiGateway.Models.Users
{
    public record RegisterRequest
    (
        string Username,
        string Password
    );
}
