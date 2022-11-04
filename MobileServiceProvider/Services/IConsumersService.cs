using MobileServiceProvider.Enums;
using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IConsumersService
    {
        Task AddAsync(ConsumerType consumerType, string name, string surname, string patronymic, string address,
            string tariff, string registrationDate, string phoneNumber);
        Task RemoveAsync(Guid id);
        IEnumerable<DisplayModel> GetDisplayModels(string date, string order, string orderBy);
        Task UploadFromFileAsync(IFormFile file, ConsumerType consumerType);
    }
}