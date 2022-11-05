using MobileServiceProvider.Enums;
using MobileServiceProvider.Exceptions;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Services
{
    public class MobilePhoneService : IMobilePhoneService
    {
        private readonly ApplicationContext _dbContext;

        public MobilePhoneService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Charge(string phoneNumber, string sumAsString)
        {
            double sum = Convert.ToDouble(sumAsString);

            BaseConsumer? consumer = _dbContext.OrdinarConsumers.SingleOrDefault(consumer => consumer.PhoneNumber == phoneNumber);
            if (consumer == null)
            {
                consumer = _dbContext.VIPConsumers.SingleOrDefault(consumer => consumer.PhoneNumbers.Contains(phoneNumber));
            }
            if (consumer == null)
            {
                throw new ConsumerNotFoundException();
            }
            consumer.TotalMoney += sum;
            _dbContext.SaveChanges();
        }
    }
}
