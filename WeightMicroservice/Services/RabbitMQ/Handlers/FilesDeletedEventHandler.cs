using WeightMicroservice.Helpers;
using WeightMicroservice.Services.RabbitMQ.Models;

namespace WeightMicroservice.Services.RabbitMQ.Handlers
{
    public class FilesDeletedEventHandler : IMessageHandler<FilesDeletedEvent>
    {
        private readonly IStorageHelper _localStorageHelper;

        public FilesDeletedEventHandler(IStorageHelper localStorageHelper)
        {
            _localStorageHelper = localStorageHelper;
        }

        public async Task HandleAsync(FilesDeletedEvent message)
        {
            Console.WriteLine($"Files deleted. Start deleting files from the server");

            await _localStorageHelper.DeleteFilesAsync(message.DeleteFileRequests);
        }
    }
}
