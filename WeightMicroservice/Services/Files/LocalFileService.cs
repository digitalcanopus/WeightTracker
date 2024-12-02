namespace WeightMicroservice.Services.Files
{
    public class LocalFileService : IFileService
    {
        public async Task SaveFileAsync(
            IFormFile file,
            string fileName,
            string fileFolderName,
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(fileFolderName);
            var filePath = Path.Combine(fileFolderName, fileName + Path.GetExtension(file.FileName));

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);
        }

        public async Task DeleteFileAsync(
            string fileName,
            string fileExtension,
            string fileFolderName,
            CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(fileFolderName, fileName + fileExtension);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File {fileName} not found in {fileFolderName}.");

            try
            {
                await Task.Run(() => File.Delete(filePath), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting file {fileName}.", ex);
            }
        }
    }
}
