using MobileServiceProvider.Repository;
using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public class PhoneCallsService : IPhoneCallsService
    {
        private readonly ApplicationContext _dbContext;

        public PhoneCallsService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<PhoneCallInfo> GetPhoneCalls(Guid? consumerId)
        {
            IEnumerable<PhoneCallInfo> phoneCalls = _dbContext.PhoneCalls
                                                 .OrderBy(d => d.StartDate);

            if (consumerId is not null)
            {
                phoneCalls = phoneCalls.Where(call => call.ConsumerId == consumerId);
            }

            return phoneCalls;
        }
    }
}
