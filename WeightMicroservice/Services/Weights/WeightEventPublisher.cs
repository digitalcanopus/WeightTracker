using System.Text.Json;
using WeightMicroservice.Services.Files.Requests;
using WeightMicroservice.Services.RabbitMQ;
using WeightMicroservice.Services.RabbitMQ.Enums;
using WeightMicroservice.Services.RabbitMQ.Models;

namespace WeightMicroservice.Services.Weights
{
    public class WeightEventPublisher : IWeightEventPublisher
    {
        private readonly IRabbitMqService _rabbitMqService;

        public WeightEventPublisher(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public async Task WeightDeleted(List<DeleteFileRequest> deleteFileRequests)
        {
            var weightDeletedEvent = new WeightDeletedEvent { DeleteFileRequests = deleteFileRequests };
            var message = JsonSerializer.Serialize(weightDeletedEvent);

            await _rabbitMqService.PublishMessageAsync(
                ExchangeEnum.WeightExchange.Name,
                RoutingKeyEnum.WeightDeleted.Name,
                message);
        }
    }
}
