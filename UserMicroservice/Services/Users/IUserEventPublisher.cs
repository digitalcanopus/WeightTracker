namespace UserMicroservice.Services.Users
{
    public interface IUserEventPublisher
    {
        Task UserDeleted(string userId);
    }
}
