using RabbitMQ.Client;

namespace WeightMicroservice.Services.RabbitMQ
{
    public interface IRabbitMqService
    {
        public Task<IChannel> GetChannelAsync();
        public Task PublishMessageAsync(string exchange, string routingKey, string message);
    }
}
