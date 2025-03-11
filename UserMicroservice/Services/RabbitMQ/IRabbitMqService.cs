namespace UserMicroservice.Services.RabbitMQ
{
    public interface IRabbitMqService
    {
        public Task PublishMessageAsync(string exchange, string routingKey, string message);
    }
}
