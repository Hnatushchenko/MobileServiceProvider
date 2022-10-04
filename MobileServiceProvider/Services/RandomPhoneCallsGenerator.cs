using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Services
{
    public class RandomPhoneCallsGenerator
    {
        const int maxNumberOfCalls = 7;
        private static readonly TimeSpan maxLengthOfCall = TimeSpan.FromHours(1);

        private readonly Random _random = new Random();
        private readonly ApplicationContext _dbContext;
        IRandomPhoneNumberGenerator _randomPhoneNumberGenerator;
        IRandomDateGenerator _randomDateGenerator;

        public RandomPhoneCallsGenerator(ApplicationContext dbContext, 
            IRandomPhoneNumberGenerator randomPhoneNumberGenerator,
            IRandomDateGenerator randomDateGenerator)
        {
            _dbContext = dbContext;
            _randomPhoneNumberGenerator = randomPhoneNumberGenerator;
            _randomDateGenerator = randomDateGenerator;
        }

        public async Task GenerateFor(BaseConsumer consumer, DateTimeOffset maxDate)
        {
            int numberOfCalls = _random.Next(maxNumberOfCalls + 1);
            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                for (int i = 0; i < numberOfCalls; i++)
                {
                    var startDate = _randomDateGenerator.GenerateFrom(consumer.RegistrationDate);
                    DateTimeOffset endDate = startDate.AddSeconds(_random.Next((int)maxLengthOfCall.TotalSeconds));
                    if (endDate > DateTimeOffset.UtcNow)
                    {
                        endDate = DateTimeOffset.UtcNow;
                    }
                    PhoneCall phoneCall = new PhoneCall
                    {
                        Id = Guid.NewGuid(),
                        FromNumber = ordinarConsumer.PhoneNumber,
                        ToNumber = _randomPhoneNumberGenerator.GenerateUkrainianPhoneNumber(),
                        StartDate = startDate,
                        EndDate = endDate,
                        ConsumerId = consumer.Id,
                        Consumer = consumer
                    };
                    _dbContext.
                    ordinarConsumer.PhoneCalls
                }
            }
        }
    }
}
