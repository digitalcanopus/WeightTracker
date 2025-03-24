using WeightTracker.Services.Weights.Models;
using OneOf;
using OneOf.Types;
using WeightTracker.Services.Weights.Requests;
using WeightMicroservice.Services.Weights.Requests;

namespace WeightTracker.Services.Weights
{
    public interface IWeightService
    {
        public Task<List<WeightDetailsDto>> GetWeightsAsync(
            string userId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<WeightDetailsDto, NotFound>> GetWeightByIdAsync(
            string userId,
            string weightId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> AddWeightAsync(
            string userId,
            AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> EditWeightAsync(
            string userId,
            string weightId,
            EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> AddFileToWeightAsync(
            string userId,
            string weightId,
            AddFileRequest addFileRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> DeleteFileFromWeightAsync(
            string userId,
            string weightId,
            string fileId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<string, NotFound>> DeleteWeightAsync(
            string userId,
            string weightId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<List<string>, NotFound>> DeleteWeightsAsync(
            string userId,
            CancellationToken cancellationToken = default);
    }
}
