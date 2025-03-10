using WeightMicroservice.Services.RabbitMQ.Handlers;

namespace WeightMicroservice.Services.RabbitMQ
{
    public class MessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TMessage>(TMessage message)
        {
            var handler = _serviceProvider.GetService<IMessageHandler<TMessage>>();

            if (handler != null)
            {
                await handler.HandleAsync(message);
            }
            else
            {
                Console.WriteLine($"No handler found for message type {typeof(TMessage).Name}");
            }
        }
    }
}
