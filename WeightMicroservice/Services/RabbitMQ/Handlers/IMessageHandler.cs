namespace WeightMicroservice.Services.RabbitMQ.Handlers
{
    public interface IMessageHandler<TMessage>
    {
        public Task HandleAsync(TMessage message);
    }
}
