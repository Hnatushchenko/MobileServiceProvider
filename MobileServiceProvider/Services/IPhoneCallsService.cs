using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IPhoneCallsService
    {
        IEnumerable<PhoneCallInfo> GetPhoneCalls(Guid? consumerId);
    }
}