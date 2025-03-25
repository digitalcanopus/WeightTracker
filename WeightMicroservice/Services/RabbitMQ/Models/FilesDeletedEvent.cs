using WeightMicroservice.Services.Files.Requests;

namespace WeightMicroservice.Services.RabbitMQ.Models
{
    public record FilesDeletedEvent
    {
        public List<DeleteFileRequest> DeleteFileRequests { get; set; } = [];
    }
}
