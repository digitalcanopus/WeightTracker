using OneOf;
using OneOf.Types;
using WeightMicroservice.Services.Weights.Requests;
using File = WeightTracker.Entities.File;

namespace WeightMicroservice.Services.Files
{
    public interface IFileService
    {
        public Task<List<File>> GetFilesByIdsAsync(
            string[] fileIds,
            CancellationToken cancellationToken = default);

        public Task<OneOf<List<string>, NotFound>> AddFilesAsync(
            List<AddFileRequest> addFileRequests,
            CancellationToken cancellationToken = default);

        public Task<List<string>> DeleteFilesAsync(
            string[] fileIds,
            CancellationToken cancellationToken = default);
    }
}
