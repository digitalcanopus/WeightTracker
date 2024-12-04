using MongoDB.Driver;
using OneOf;
using OneOf.Types;
using WeightTracker.Entities;
using WeightTracker.Services.Weights.Models;
using Mapster;
using WeightTracker.Services.Weights.Requests;
using File = WeightTracker.Entities.File;
using WeightMicroservice.Services.Files;
using WeightMicroservice.Services.Weights.Requests;
using WeightMicroservice.Services.Files.Models;

namespace WeightTracker.Services.Weights
{
    public class WeightService : IWeightService
    {
        private readonly IMongoCollection<Weight> _weights;
        private readonly IMongoCollection<File> _files;
        private readonly IFileService _fileService;
        private readonly string _fileFolderName;

        public WeightService(IMongoDatabase database, string fileFolderName, IFileService fileService)
        {
            _weights = database.GetCollection<Weight>("Weights");
            _files = database.GetCollection<File>("Files");
            _fileFolderName = fileFolderName;
            _fileService = fileService;
        }

        public async Task<OneOf<List<WeightDetailsDto>, NotFound>> GetWeightsAsync(
            CancellationToken cancellationToken = default)
        {
            var weights = await _weights
                .Find(_ => true)
                .ToListAsync(cancellationToken);

            if (weights.Count == 0)
                return new NotFound();

            var fileIds = weights.SelectMany(w => w.Files ?? []);

            var files = await _files
                .Find(f => fileIds.Contains(f.Id))
                .ToListAsync(cancellationToken);

            var fileDictionary = files.ToDictionary(f => f.Id);

            var result = weights.Select(weight => new WeightDetailsDto {
                Id = weight.Id,
                WeightValue = weight.WeightValue,
                Date = weight.Date,
                Files = weight.Files?.Select(f => fileDictionary[f].Adapt<FileDetailsDto>()).ToList(),
                UserId = weight.UserId
            }).ToList();

            return OneOf<List<WeightDetailsDto>, NotFound>.FromT0(result);
        }

        public async Task<OneOf<WeightDetailsDto, NotFound>> GetWeightByIdAsync(
            string weightId,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var fileIds = weight.Files ?? [];

            var files = await _files
                .Find(f => fileIds.Contains(f.Id))
                .ToListAsync(cancellationToken);

            var fileDictionary = files.ToDictionary(f => f.Id);

            var result = new WeightDetailsDto
            {
                Id = weight.Id,
                WeightValue = weight.WeightValue,
                Date = weight.Date,
                Files = weight.Files?.Select(f => fileDictionary[f].Adapt<FileDetailsDto>()).ToList(),
                UserId = weight.UserId
            };

            return OneOf<WeightDetailsDto, NotFound>.FromT0(result.Adapt<WeightDetailsDto>());
        }

        public async Task<OneOf<string, NotFound>> AddWeightAsync(
            AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var weightToAdd = addWeightRequest.Adapt<Weight>();
            weightToAdd.Files = [];

            await _weights
                .InsertOneAsync(weightToAdd, cancellationToken: cancellationToken);

            return weightToAdd.Id;
        }

        public async Task<OneOf<string, NotFound>> EditWeightAsync(
            string weightId,
            EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var update = Builders<Weight>.Update
                .Set(w => w.Date, editWeightRequest.Date)
                .Set(w => w.WeightValue, editWeightRequest.WeightValue);

            await _weights
                .UpdateOneAsync(w => w.Id == weightId, update, cancellationToken: cancellationToken);

            return weight.Id;
        }

        public async Task<OneOf<string, NotFound>> AddFileToWeightAsync(
            string weightId,
            AddFileRequest addFileRequest,
            CancellationToken cancellationToken = default)
        {
            var weight = await _weights
                .Find(w => w.Id == weightId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weight == null)
                return new NotFound();

            var fileToInsert = new File
            {
                Extension = Path.GetExtension(addFileRequest.File.FileName),
                OriginalName = addFileRequest.File.FileName
            };

            await _files
                .InsertOneAsync(fileToInsert, cancellationToken: cancellationToken);

            await _fileService.SaveFileAsync(
                addFileRequest.File, fileToInsert.Id, _fileFolderName, cancellationToken);

            var update = Builders<Weight>.Update
                .Push(w => w.Files, fileToInsert.Id);

            await _weights
                .UpdateOneAsync(w => w.Id == weightId, update, cancellationToken: cancellationToken);

            return weight.Id;
        }

        public async Task<OneOf<string, NotFound>> DeleteFileFromWeightAsync(
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

            var fileToDelete = await _files
                .Find(f => f.Id == fileId)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileToDelete == null)
                return new NotFound();

            var update = Builders<Weight>.Update
                .Pull(w => w.Files, fileId);

            await _weights
                .UpdateOneAsync(w => w.Id == weightId, update, cancellationToken: cancellationToken);

            await _files
                .DeleteOneAsync(f => f.Id == fileId, cancellationToken: cancellationToken);

            try
            {
                await _fileService.DeleteFileAsync(
                    fileToDelete.Id, fileToDelete.Extension, _fileFolderName, cancellationToken);
            }
            catch
            {
                return new NotFound();
            }

            return fileToDelete.Id;
        }

        public async Task<OneOf<string, NotFound>> DeleteWeightAsync(
            string weightId,
            CancellationToken cancellationToken = default)
        {
            var weightToDelete = await _weights
                .Find(w => w.Id == weightId)
                .FirstOrDefaultAsync(cancellationToken);

            if (weightToDelete == null)
                return new NotFound();

            await _weights
                .DeleteOneAsync(w => w.Id == weightId, cancellationToken);

            if (weightToDelete.Files == null || weightToDelete.Files.Length == 0)
                return weightToDelete.Id;

            var files = await _files
                .Find(f => weightToDelete.Files.Contains(f.Id))
                .ToListAsync(cancellationToken);

            await _files
                .DeleteManyAsync(f => weightToDelete.Files.Contains(f.Id), cancellationToken);

            foreach (var file in files)
            {
                try
                {
                    await _fileService.DeleteFileAsync(
                        file.Id, file.Extension, _fileFolderName, cancellationToken);
                }
                catch
                {
                    continue;
                }
            }

            return weightToDelete.Id;
        }
    }
}