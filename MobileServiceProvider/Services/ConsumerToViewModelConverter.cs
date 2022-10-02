using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Services
{
    public class ConsumerToViewModelConverter : IConsumerToViewModelConverter
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ApplicationContext dbContext;

        public ConsumerToViewModelConverter(ApplicationContext context, IDateTimeProvider dateTimeProvider)
        {
            dbContext = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public ViewAllModel Convert(BaseConsumer consumer)
        {
            ViewAllModel model = new ViewAllModel()
            {
                Name = consumer.Name,
                Surname = consumer.Surname,
                Patronymic = consumer.Patronymic,
                Address = consumer.Address,
            };

            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                model.PhoneNumbers.Add(ordinarConsumer.PhoneNumber);
                model.MonthlyFee = dbContext.Tariffs.Where(tariff =>
                    tariff.Name == consumer.TariffName).Single().MonthlyFeeForOrdinarConsumer;
            }
            else if (consumer is VIPConsumer VIPConsumer)
            {
                model.PhoneNumbers = VIPConsumer.PhoneNumbers.Split(", ").ToList();
                model.MonthlyFee = dbContext.Tariffs.Where(tariff =>
                    tariff.Name == consumer.TariffName).Single().MonthlyFeeForVIPConsumer;
                model.MonthlyFee -= model.MonthlyFee * 0.1 * (model.PhoneNumbers.Count / 5);
                model.MonthlyFee = Math.Round(model.MonthlyFee, 2);
            }

            int HowManyMonthsIsActive = (_dateTimeProvider.Now.Year - consumer.RegistrationDate.Year) * 12 + _dateTimeProvider.Now.Month - consumer.RegistrationDate.Month;
            model.Balance = Math.Round(consumer.TotalMoney - HowManyMonthsIsActive * model.MonthlyFee, 2);

            model.Status = model.Balance < 0 ? "Відключено" : "Підключено";
            return model;
        }
    }
}
