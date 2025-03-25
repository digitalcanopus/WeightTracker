using WeightMicroservice.Services.Files.Requests;

namespace WeightMicroservice.Services.RabbitMQ.Models
{
    public record WeightDeletedEvent
    {
        public List<DeleteFileRequest> DeleteFileRequests { get; set; } = [];
    }
}
