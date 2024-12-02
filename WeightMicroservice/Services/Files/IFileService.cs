namespace WeightMicroservice.Services.Files
{
    public interface IFileService
    {
        public Task SaveFileAsync(
            IFormFile file,
            string fileName,
            string fileFolderName,
            CancellationToken cancellationToken = default);

        public Task DeleteFileAsync(
            string fileName,
            string fileExtension,
            string fileFolderName,
            CancellationToken cancellationToken = default);
    }
}
