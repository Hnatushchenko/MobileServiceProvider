using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IRandomPhoneCallsGenerator
    {
        Task GenerateForAsync(BaseConsumer consumer, DateTimeOffset maxDate);
        Task GenerateForAsync(OrdinarConsumer consumer, DateTimeOffset maxDate);
        Task GenerateForAsync(VIPConsumer consumer, DateTimeOffset maxDate);
    }
}