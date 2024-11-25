using MongoDB.Driver;
using OneOf;
using OneOf.Types;
using WeightTracker.Entities;
using WeightTracker.Services.Weights.Models;
using Mapster;
using WeightTracker.Services.Weights.Requests;
using File = WeightTracker.Entities.File;

namespace WeightTracker.Services.Weights
{
    public class WeightService : IWeightService
    {
        private readonly IMongoCollection<Weight> _weights;
        private readonly IMongoCollection<File> _files;

        public WeightService(IMongoDatabase database)
        {
            _weights = database.GetCollection<Weight>("Weights");
            _files = database.GetCollection<File>("Files");
        }

        public async Task<OneOf<List<WeightDetailsDto>, NotFound>> GetWeightsAsync(
            CancellationToken cancellationToken = default)
        {
            var weights = await _weights
                .Find(_ => true)
                .ToListAsync(cancellationToken);

            return OneOf<List<WeightDetailsDto>, NotFound>.FromT0(weights.Adapt<List<WeightDetailsDto>>());
        }

        public async Task<OneOf<WeightDetailsDto, NotFound>> GetWeightByIdAsync(
            string weightId,
            CancellationToken cancellationToken = default)
        {
            var result = await _weights
                .Find(w => w.Id == weightId)
                .FirstOrDefaultAsync(cancellationToken);

            return OneOf<WeightDetailsDto, NotFound>.FromT0(result.Adapt<WeightDetailsDto>());
        }

        public async Task<OneOf<WeightDetailsDto, NotFound>> AddWeightAsync(
            AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var weightToAdd = addWeightRequest.Adapt<Weight>();

            await _weights
                .InsertOneAsync(weightToAdd, cancellationToken: cancellationToken);

            return weightToAdd.Adapt<WeightDetailsDto>();
        }

        public async Task<OneOf<WeightDetailsDto, NotFound>> EditWeightAsync(
            string weightId,
            EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var existingWeight = await _weights
                .Find(weightId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingWeight == null)
                return new NotFound();

            if (editWeightRequest.Files != null && editWeightRequest.Files.Length > 0)
            {
                
            }

            var update = Builders<Weight>.Update
                .Set(w => w.Date, editWeightRequest.Date)
                .Set(w => w.WeightValue, editWeightRequest.WeightValue)
                .Set(w => w.Files, existingWeight.Files);

            await _weights
                .UpdateOneAsync(weightId, update, cancellationToken: cancellationToken);

            var updatedWeight = await _weights
                .Find(weightId)
                .FirstOrDefaultAsync(cancellationToken);

            return updatedWeight.Adapt<WeightDetailsDto>();
        }

        public async Task<OneOf<Success, NotFound>> DeleteFileFromWeightAsync(
            string weightId,
            string fileId,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId
                    && w.Files != null
                    && w.Files.Contains(fileId))
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var fileExists = await _files
                .Find(fileId)
                .AnyAsync(cancellationToken);

            if (!fileExists)
                return new NotFound();

            var updatedFiles = weight.Files?.Where(f => f != fileId).ToArray();
            var update = Builders<Weight>.Update.Set(w => w.Files, updatedFiles);

            await _weights
                .UpdateOneAsync(weightId, update, cancellationToken: cancellationToken);

            return new Success();
        }
    }
}