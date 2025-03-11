using RabbitMQ.Client;
using System.Text;
using UserMicroservice.Services.RabbitMQ.Enums;
using UserMicroservice.Settings;

namespace UserMicroservice.Services.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly ConnectionFactory _factory;
        private readonly List<BrokerSettings> _brokerSettings;

        private IConnection? _connection;
        private IChannel? _channel;
        private SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

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

        public async Task PublishMessageAsync(string exchange, string routingKey, string message)
        {
            await EnsureConnectionAsync();

            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties();

            await _channel!.BasicPublishAsync(
                exchange,
                routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);
        }

        private async Task EnsureConnectionAsync()
        {
            if (_connection != null && _channel != null)
                return;

            await _connectionLock.WaitAsync();
            try
            {
                if (_connection == null)
                {
                    _connection = await _factory.CreateConnectionAsync();
                }

                if (_channel == null)
                {
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
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        //private async Task CreateConnection()
        //{
        //    _connection = await _factory.CreateConnectionAsync();
        //}

        //private async Task CreateChannel()
        //{
        //    _channel = await _connection!.CreateChannelAsync();
        //    await _channel.BasicQosAsync(0, 1, false);

        //    foreach (var exchange in ExchangeEnum.List)
        //    {
        //        await _channel.ExchangeDeclareAsync(exchange.Name, ExchangeType.Topic, durable: true);
        //    }

        //    foreach (var queue in QueueEnum.List)
        //    {
        //        await _channel.QueueDeclareAsync(queue.Name, durable: true, exclusive: false, autoDelete: false);
        //    }

        //    foreach (var brokerSetting in _brokerSettings)
        //    {
        //        await _channel.QueueBindAsync(
        //            brokerSetting.QueueName, brokerSetting.ExchangeName, brokerSetting.RoutingKey);
        //    }
        //}
    }
}
