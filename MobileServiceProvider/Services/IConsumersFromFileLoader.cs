using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IConsumersFromFileLoader<TConsumer> where TConsumer : BaseConsumer
    {
        Task<TConsumer[]> LoadAsync(IFormFile file);
    }
}