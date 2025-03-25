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

        public async Task FilesDeleted(List<DeleteFileRequest> deleteFileRequests)
        {
            var filesDeletedEvent = new FilesDeletedEvent { DeleteFileRequests = deleteFileRequests };
            var message = JsonSerializer.Serialize(filesDeletedEvent);

            await _rabbitMqService.PublishMessageAsync(
                ExchangeEnum.WeightExchange.Name,
                RoutingKeyEnum.FilesDeleted.Name,
                message);
        }
    }
}
