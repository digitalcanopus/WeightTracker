namespace WeightMicroservice.Settings
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
