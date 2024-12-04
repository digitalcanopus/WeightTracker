namespace UserMicroservice.Services.Users.Requests
{
    public record RegisterRequest
    (
        string Username,
        string Password
    );
}
