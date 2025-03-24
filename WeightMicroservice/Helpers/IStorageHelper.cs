using WeightMicroservice.Services.Files.Requests;

namespace WeightMicroservice.Helpers
{
    public interface IStorageHelper
    {
        public Task SaveFilesAsync(
            List<SaveFileRequest> filesToSave,
            CancellationToken cancellationToken = default);

        public Task DeleteFilesAsync(
            List<DeleteFileRequest> filesToDelete,
            CancellationToken cancellationToken = default);
    }
}
