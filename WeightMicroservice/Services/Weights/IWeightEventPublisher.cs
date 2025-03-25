using WeightMicroservice.Services.Files.Requests;

namespace WeightMicroservice.Services.Weights
{
    public interface IWeightEventPublisher
    {
        Task WeightDeleted(List<DeleteFileRequest> deleteFileRequests);
    }
}
