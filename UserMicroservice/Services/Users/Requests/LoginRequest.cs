namespace UserMicroservice.Services.Users.Requests
{
    public record LoginRequest
    (
        string Username,
        string Password
    );
}
