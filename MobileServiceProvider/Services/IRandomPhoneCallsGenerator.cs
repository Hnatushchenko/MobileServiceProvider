using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IRandomPhoneCallsGenerator
    {
        Task GenerateFor(OrdinarConsumer consumer, DateTimeOffset maxDate);
        Task GenerateFor(VIPConsumer consumer, DateTimeOffset maxDate);
    }
}