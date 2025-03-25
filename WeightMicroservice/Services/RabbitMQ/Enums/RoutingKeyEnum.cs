using Ardalis.SmartEnum;

namespace WeightMicroservice.Services.RabbitMQ.Enums
{
    public class RoutingKeyEnum(string name, int value) : SmartEnum<RoutingKeyEnum>(name, value)
    {
        public static readonly RoutingKeyEnum FilesDeleted = new("Files.Deleted", 1);
    }
}
