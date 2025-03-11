using System.Text.Json;
using UserMicroservice.Services.RabbitMQ;
using UserMicroservice.Services.RabbitMQ.Enums;
using UserMicroservice.Services.RabbitMQ.Models;

namespace UserMicroservice.Services.Users
{
    public class UserEventPublisher : IUserEventPublisher
    {
        private readonly IRabbitMqService _rabbitMqService;

        public UserEventPublisher(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public async Task UserDeleted(string userId)
        {
            var userDeletedEvent = new UserDeletedEvent { UserId = userId };
            var message = JsonSerializer.Serialize(userDeletedEvent);

            await _rabbitMqService.PublishMessageAsync(
                ExchangeEnum.UserExchange.Name,
                RoutingKeyEnum.UserDeleted.Name,
                message);
        }
    }
}
