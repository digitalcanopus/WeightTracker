using WeightMicroservice.Services.RabbitMQ.Models;
using WeightTracker.Services.Weights;

namespace WeightMicroservice.Services.RabbitMQ.Handlers
{
    public class UserDeletedEventHandler : IMessageHandler<UserDeletedEvent>
    {
        private readonly IWeightService _weightService;

        public UserDeletedEventHandler(IWeightService weightService)
        {
            _weightService = weightService;
        }

        public async Task HandleAsync(UserDeletedEvent message)
        {
            Console.WriteLine($"User deleted: {message.UserId}");
            
            await _weightService.DeleteWeightsAsync(message.UserId);
        }
    }
}
