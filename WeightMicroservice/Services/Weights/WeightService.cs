using MongoDB.Driver;
using OneOf;
using OneOf.Types;
using WeightTracker.Entities;
using WeightTracker.Services.Weights.Models;
using Mapster;
using WeightTracker.Services.Weights.Requests;
using WeightMicroservice.Services.Files;
using WeightMicroservice.Services.Files.Models;
using WeightMicroservice.Services.Weights.Requests;

namespace WeightTracker.Services.Weights
{
    public class WeightService : IWeightService
    {
        private readonly IMongoCollection<Weight> _weights;
        private readonly IFileService _fileService;

        public WeightService(IMongoDatabase database, IFileService fileService)
        {
            _weights = database.GetCollection<Weight>("Weights");
            _fileService = fileService;
        }

        public async Task<List<WeightDetailsDto>> GetWeightsAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var weights = await _weights
                .Find(w => w.UserId == userId)
                .ToListAsync(cancellationToken);

            if (weights.Count == 0)
                return [];

            var fileIds = weights.SelectMany(w => w.Files ?? []).ToArray();

            var files = await _fileService.GetFilesByIdsAsync(fileIds, cancellationToken);

            var fileDictionary = files.ToDictionary(f => f.Id);

            var result = weights.Select(weight => new WeightDetailsDto {
                Id = weight.Id,
                WeightValue = weight.WeightValue,
                Date = weight.Date,
                Files = weight.Files?.Select(f => fileDictionary[f].Adapt<FileDetailsDto>()).ToList(),
                UserId = weight.UserId
            }).ToList();

            return result;
        }

        public async Task<OneOf<WeightDetailsDto, NotFound>> GetWeightByIdAsync(
            string userId,
            string weightId,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId && w.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var fileIds = weight.Files ?? [];

            var files = await _fileService.GetFilesByIdsAsync(fileIds, cancellationToken);

            var fileDictionary = files.ToDictionary(f => f.Id);

            var result = new WeightDetailsDto
            {
                Id = weight.Id,
                WeightValue = weight.WeightValue,
                Date = weight.Date,
                Files = weight.Files?.Select(f => fileDictionary[f].Adapt<FileDetailsDto>()).ToList(),
                UserId = weight.UserId
            };

            return result.Adapt<WeightDetailsDto>();
        }

        public async Task<OneOf<string, NotFound>> AddWeightAsync(
            string userId,
            AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var weightToAdd = addWeightRequest.Adapt<Weight>();
            weightToAdd.UserId = userId;
            weightToAdd.Files = [];

            await _weights
                .InsertOneAsync(weightToAdd, cancellationToken: cancellationToken);

            if (addWeightRequest.Files.Count != 0)
            {
                List<AddFileRequest> fileRequests = addWeightRequest.Files
                    .Select(file => new AddFileRequest(file))
                    .ToList();

                var fileIdsResult = await _fileService.AddFilesAsync(fileRequests, cancellationToken);

                if (fileIdsResult.IsT0)
                {
                    weightToAdd.Files = [.. fileIdsResult.AsT0];
                    var update = Builders<Weight>.Update.Set(w => w.Files, weightToAdd.Files);

                    await _weights.UpdateOneAsync(
                        w => w.Id == weightToAdd.Id,
                        update,
                        cancellationToken: cancellationToken);
                }
            }

            return weightToAdd.Id;
        }

        public async Task<OneOf<string, NotFound>> EditWeightAsync(
            string userId,
            string weightId,
            EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId && w.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var update = Builders<Weight>.Update
                .Set(w => w.Date, editWeightRequest.Date)
                .Set(w => w.WeightValue, editWeightRequest.WeightValue);

            await _weights
                .UpdateOneAsync(w => w.Id == weightId && w.UserId == userId, update, cancellationToken: cancellationToken);

            return weight.Id;
        }

        public async Task<OneOf<string, NotFound>> AddFileToWeightAsync(
            string userId,
            string weightId,
            AddFileRequest addFileRequest,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId && w.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var addFileResult = await _fileService.
                AddFilesAsync([addFileRequest], cancellationToken);

            if (addFileResult.IsT1)
                return new NotFound();

            var fileId = addFileResult.AsT0.First();

            var update = Builders<Weight>.Update
                .Push(w => w.Files, fileId);

            await _weights.UpdateOneAsync(
                w => w.Id == weightId && w.UserId == userId,
                update,
                cancellationToken: cancellationToken);

            return fileId;
        }

        public async Task<OneOf<string, NotFound>> DeleteFileFromWeightAsync(
            string userId,
            string weightId,
            string fileId,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId
                    && w.UserId == userId
                    && w.Files != null
                    && w.Files.Contains(fileId))
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var update = Builders<Weight>.Update
                .Pull(w => w.Files, fileId);

            await _weights
                .UpdateOneAsync(w => w.Id == weightId && w.UserId == userId, update, cancellationToken: cancellationToken);

            var deletedFiles = await _fileService.DeleteFilesAsync([fileId], cancellationToken);

            return deletedFiles.Count > 0 ? fileId : new NotFound();
        }

        public async Task<OneOf<string, NotFound>> DeleteWeightAsync(
            string userId,
            string weightId,
            CancellationToken cancellationToken = default)
        {
            var weightToDelete = await _weights
                .Find(w => w.Id == weightId && w.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weightToDelete == null)
                return new NotFound();

            await _weights
                .DeleteOneAsync(w => w.Id == weightId && w.UserId == userId, cancellationToken);

            if (weightToDelete.Files.Length > 0)
            {
                await _fileService.DeleteFilesAsync(weightToDelete.Files, cancellationToken);
            }

            return weightToDelete.Id;
        }

        public async Task<OneOf<List<string>, NotFound>> DeleteWeightsAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var weightsToDelete = await _weights
                .Find(w => w.UserId == userId)
                .ToListAsync(cancellationToken);

            if (weightsToDelete.Count == 0)
                return new NotFound();

            var weightIdsToDelete = weightsToDelete.Select(w => w.Id).ToList();

            var deleteResult = await _weights.DeleteManyAsync(w => w.UserId == userId, cancellationToken);

            var fileIds = weightsToDelete
                .Where(w => w.Files != null && w.Files.Length > 0)
                .SelectMany(w => w.Files)
                .ToArray();

            if (fileIds.Length > 0)
            {
                await _fileService.DeleteFilesAsync(fileIds, cancellationToken);
            }

            return weightIdsToDelete;
        }
    }
}