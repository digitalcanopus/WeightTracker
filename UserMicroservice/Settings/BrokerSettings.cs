namespace UserMicroservice.Settings
{
    public class BrokerSettings
    {
        public string ExchangeName { get; set; } = null!;
        public string QueueName { get; set; } = null!;
        public string RoutingKey { get; set; } = null!;
    }
}
