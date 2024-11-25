using WeightTracker.Services.Weights.Models;
using OneOf;
using OneOf.Types;
using WeightTracker.Services.Weights.Requests;

namespace WeightTracker.Services.Weights
{
    public interface IWeightService
    {
        public Task<OneOf<List<WeightDetailsDto>, NotFound>> GetWeightsAsync(
            CancellationToken cancellationToken = default);

        public Task<OneOf<WeightDetailsDto, NotFound>> GetWeightByIdAsync(
            string weightId,
            CancellationToken cancellationToken = default);

        public Task<OneOf<WeightDetailsDto, NotFound>> AddWeightAsync(
            AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<WeightDetailsDto, NotFound>> EditWeightAsync(
            string weightId,
            EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default);

        public Task<OneOf<Success, NotFound>> DeleteFileFromWeightAsync(
            string weightId,
            string fileId,
            CancellationToken cancellationToken = default);

        //public Task<OneOf<Success, NotFound>> DeleteWeightAsync();
    }
}
