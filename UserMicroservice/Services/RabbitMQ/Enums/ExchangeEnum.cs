using Ardalis.SmartEnum;

namespace UserMicroservice.Services.RabbitMQ.Enums
{
    public class ExchangeEnum(string name, int value) : SmartEnum<ExchangeEnum>(name, value)
    {
        public static readonly ExchangeEnum UserExchange = new("UserExchange", 1);
        public static readonly ExchangeEnum WeightExchange = new("WeightExchange", 2);
    }
}
