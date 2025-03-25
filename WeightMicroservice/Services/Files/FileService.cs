using MongoDB.Driver;
using File = WeightTracker.Entities.File;
using OneOf;
using OneOf.Types;
using WeightMicroservice.Services.Weights.Requests;
using WeightMicroservice.Services.Files.Requests;
using WeightMicroservice.Services.Weights;
using WeightMicroservice.Helpers;

namespace WeightMicroservice.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IMongoCollection<File> _files;
        private readonly IWeightEventPublisher _weightEventPublisher;

        private readonly IStorageHelper _localStorageHelper;

        public FileService(IMongoDatabase database, IWeightEventPublisher weightEventPublisher, IStorageHelper localStorageHelper)
        {
            _files = database.GetCollection<File>("Files");
            _weightEventPublisher = weightEventPublisher;
            _localStorageHelper = localStorageHelper;
        }

        public async Task<List<File>> GetFilesByIdsAsync(
            string[] fileIds,
            CancellationToken cancellationToken = default)
        {
            if (fileIds.Length == 0)
                return [];

            return await _files
                .Find(f => fileIds.Contains(f.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<OneOf<List<string>, NotFound>> AddFilesAsync(
            List<AddFileRequest> addFileRequests,
            CancellationToken cancellationToken = default)
        {
            var fileEntities = addFileRequests.Select(f => new File
            {
                Extension = Path.GetExtension(f.File.FileName),
                OriginalName = f.File.FileName
            }).ToList();

            await _files
                .InsertManyAsync(fileEntities, cancellationToken: cancellationToken);

            var saveFileRequests = fileEntities.Select(f => new SaveFileRequest(
                File: addFileRequests.First(file => file.File.FileName == f.OriginalName).File,
                FileName: $"{f.Id}")
            ).ToList();

            await _localStorageHelper.SaveFilesAsync(saveFileRequests, cancellationToken);

            return fileEntities.Select(f => f.Id).ToList();
        }

        public async Task<List<string>> DeleteFilesAsync(
            string[] fileIds,
            CancellationToken cancellationToken = default)
        {
            var filesToDelete = await _files
                .Find(f => fileIds.Contains(f.Id))
                .ToListAsync(cancellationToken);

            if (filesToDelete.Count == 0)
                return [];

            await _files
                .DeleteManyAsync(f => fileIds.Contains(f.Id), cancellationToken);

            var deletedFileIds = filesToDelete.Select(f => f.Id).ToList();

            var deleteRequests = filesToDelete
                .Select(f => new DeleteFileRequest(f.Id, f.Extension))
                .ToList();

            await _weightEventPublisher.FilesDeleted(deleteRequests);

            return deletedFileIds;
        }
    }
}
