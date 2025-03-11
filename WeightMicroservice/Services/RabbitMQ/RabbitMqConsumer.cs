using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using WeightMicroservice.Services.RabbitMQ.Utils;

namespace WeightMicroservice.Services.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly MessageDispatcher _dispatcher;

        public RabbitMqConsumer(IRabbitMqService rabbitMqService, MessageDispatcher dispatcher)
        {
            _rabbitMqService = rabbitMqService;
            _dispatcher = dispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = await _rabbitMqService.GetChannelAsync();

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                Console.WriteLine($"Received message with routing key: {routingKey}");

                try
                {
                    var eventType = EventTypeResolver.Resolve(routingKey);

                    if (eventType == null)
                    {
                        Console.WriteLine($"Unknown event type for routing key: {routingKey}");
                        return;
                    }

                    var eventObj = JsonSerializer.Deserialize(message, eventType);
                    if (eventObj != null)
                    {
                        await _dispatcher.DispatchAsync((dynamic)eventObj);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message processing failed: {ex.Message}");
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync("WeightQueue", false, consumer, cancellationToken: stoppingToken);

            Console.WriteLine("Listening for messages...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Consumer stopped.");
            }
        }
    }
}
