using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Services
{
    public class RandomPhoneCallsGenerator : IRandomPhoneCallsGenerator
    {
        const int maxNumberOfCallsForEachPhoneNumber = 7;
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

        public async Task GenerateFor(VIPConsumer consumer, DateTimeOffset maxDate)
        {
            string[] phoneNumbers = consumer.PhoneNumbers.Split(",");
            foreach (string phoneNumber in phoneNumbers)
            {
                int numberOfCalls = _random.Next(maxNumberOfCallsForEachPhoneNumber + 1);
                for (int i = 0; i < numberOfCalls; i++)
                {
                    var startDate = _randomDateGenerator.GenerateFrom(consumer.RegistrationDate);
                    DateTimeOffset endDate = startDate.AddSeconds(_random.Next((int)maxLengthOfCall.TotalSeconds));
                    if (endDate > maxDate)
                    {
                        endDate = maxDate;
                    }
                    PhoneCallInfo phoneCall = new PhoneCallInfo
                    {
                        Id = Guid.NewGuid(),
                        FromNumber = phoneNumber,
                        ToNumber = _randomPhoneNumberGenerator.GenerateUkrainianPhoneNumber(),
                        StartDate = startDate,
                        EndDate = endDate,
                        ConsumerId = consumer.Id
                    };
                    await _dbContext.PhoneCalls.AddAsync(phoneCall);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task GenerateFor(OrdinarConsumer consumer, DateTimeOffset maxDate)
        {
            int numberOfCalls = _random.Next(maxNumberOfCallsForEachPhoneNumber + 1);
            for (int i = 0; i < numberOfCalls; i++)
            {
                var startDate = _randomDateGenerator.GenerateFrom(consumer.RegistrationDate);
                DateTimeOffset endDate = startDate.AddSeconds(_random.Next((int)maxLengthOfCall.TotalSeconds));
                if (endDate > maxDate)
                {
                    endDate = maxDate;
                }
                PhoneCallInfo phoneCall = new PhoneCallInfo
                {
                    Id = Guid.NewGuid(),
                    FromNumber = consumer.PhoneNumber,
                    ToNumber = _randomPhoneNumberGenerator.GenerateUkrainianPhoneNumber(),
                    StartDate = startDate,
                    EndDate = endDate,
                    ConsumerId = consumer.Id
                };
                await _dbContext.PhoneCalls.AddAsync(phoneCall);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
