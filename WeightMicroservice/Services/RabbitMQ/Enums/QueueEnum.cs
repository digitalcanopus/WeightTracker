using Ardalis.SmartEnum;

namespace WeightMicroservice.Services.RabbitMQ.Enums
{
    public class QueueEnum(string name, int value) : SmartEnum<QueueEnum>(name, value)
    {
        public static readonly QueueEnum UserQueue = new("UserQueue", 1);
        public static readonly QueueEnum WeightQueue = new("WeightQueue", 2);
    }
}
