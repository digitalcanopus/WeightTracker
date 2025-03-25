using WeightMicroservice.Helpers;
using WeightMicroservice.Services.RabbitMQ.Models;

namespace WeightMicroservice.Services.RabbitMQ.Handlers
{
    public class WeightDeletedEventHandler : IMessageHandler<WeightDeletedEvent>
    {
        private readonly IStorageHelper _localStorageHelper;

        public WeightDeletedEventHandler(IStorageHelper localStorageHelper)
        {
            _localStorageHelper = localStorageHelper;
        }

        public async Task HandleAsync(WeightDeletedEvent message)
        {
            Console.WriteLine($"Weight deleted. Start deleting files");

            await _localStorageHelper.DeleteFilesAsync(message.DeleteFileRequests);
        }
    }
}
