using RabbitMQ.Client;
using WeightMicroservice.Services.RabbitMQ.Enums;
using WeightMicroservice.Settings;

namespace WeightMicroservice.Services.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly ConnectionFactory _factory;
        private readonly List<BrokerSettings> _brokerSettings;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqService(RabbitMqSettings rabbitMqSettings, List<BrokerSettings> brokerSettings)
        {
            _factory = new ConnectionFactory
            {
                HostName = rabbitMqSettings.HostName,
                Port = rabbitMqSettings.Port,
                UserName = rabbitMqSettings.Username,
                Password = rabbitMqSettings.Password
            };

            _brokerSettings = brokerSettings;
        }

        public async Task<IChannel> GetChannelAsync()
        {
            _connection ??= await _factory.CreateConnectionAsync();

            if (_channel != null)
                return _channel;

            _channel = await _connection.CreateChannelAsync();
            await _channel.BasicQosAsync(0, 1, false);

            foreach (var exchange in ExchangeEnum.List)
            {
                await _channel.ExchangeDeclareAsync(exchange.Name, ExchangeType.Topic, durable: true);
            }

            foreach (var queue in QueueEnum.List)
            {
                await _channel.QueueDeclareAsync(queue.Name, durable: true, exclusive: false, autoDelete: false);
            }

            foreach (var brokerSetting in _brokerSettings)
            {
                await _channel.QueueBindAsync(
                    brokerSetting.QueueName, brokerSetting.ExchangeName, brokerSetting.RoutingKey);
            }

            return _channel;
        }
    }
}
