using WeightTracker.Services.Weights.Models;
using OneOf;
using OneOf.Types;
using WeightTracker.Services.Weights.Requests;
using WeightMicroservice.Services.Weights.Requests;

namespace WeightTracker.Services.Weights
{
    public interface IWeightService
    {
        public Task<OneOf<List<WeightDetailsDto>, NotFound>> GetWeightsAsync(
            CancellationToken cancellationToken = default);

        public Task<OneOf<WeightDetailsDto, NotFound>> GetWeightByIdAsync(
            string weightId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> AddWeightAsync(
            AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> EditWeightAsync(
            string weightId,
            EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> AddFileToWeightAsync(
            string weightId,
            AddFileRequest addFileRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> DeleteFileFromWeightAsync(
            string weightId,
            string fileId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> DeleteWeightAsync(
            string weightId,
            CancellationToken cancellationToken = default);
    }
}
