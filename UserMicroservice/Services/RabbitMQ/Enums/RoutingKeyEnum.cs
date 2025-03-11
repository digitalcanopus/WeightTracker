using Ardalis.SmartEnum;

namespace UserMicroservice.Services.RabbitMQ.Enums
{
    public class RoutingKeyEnum(string name, int value) : SmartEnum<RoutingKeyEnum>(name, value)
    {
        public static readonly RoutingKeyEnum UserDeleted = new("User.Deleted", 1);
    }
}
