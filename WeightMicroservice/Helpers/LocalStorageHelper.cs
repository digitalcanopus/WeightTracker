using WeightMicroservice.Services.Files.Requests;

namespace WeightMicroservice.Helpers
{
    public class LocalStorageHelper : IStorageHelper
    {
        private readonly string _fileFolderName;

        public LocalStorageHelper(string fileFolderName)
        {
            _fileFolderName = fileFolderName;
        }

        public async Task SaveFilesAsync(
            List<SaveFileRequest> filesToSave,
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(_fileFolderName);

            foreach (var file in filesToSave)
            {
                var filePath = Path.Combine(_fileFolderName, file.FileName + Path.GetExtension(file.File.FileName));

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.File.CopyToAsync(stream, cancellationToken);
            }
        }

        public async Task DeleteFilesAsync(
            List<DeleteFileRequest> filesToDelete,
            CancellationToken cancellationToken = default)
        {
            var tasks = filesToDelete.Select(async file =>
            {
                var filePath = Path.Combine(_fileFolderName, file.FileName + file.FileExtension);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File {file.FileName} not found in {_fileFolderName}.");
                }

                try
                {
                    await Task.Run(() => File.Delete(filePath), cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error deleting file {file.FileName}. ", ex);
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
