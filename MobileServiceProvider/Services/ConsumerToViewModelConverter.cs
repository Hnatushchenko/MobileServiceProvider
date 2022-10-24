using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;
using System.Globalization;

namespace MobileServiceProvider.Services
{
    public class ConsumerToViewModelConverter : IConsumerToViewModelConverter
    {
        private readonly ApplicationContext _dbContext;

        public ConsumerToViewModelConverter(ApplicationContext context)
        {
            _dbContext = context;
        }

        public ViewAllModel Convert(BaseConsumer consumer, string dateAsString)
        {
            DateTime date = DateTime.ParseExact(dateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            ViewAllModel model = new ViewAllModel()
            {
                Id = consumer.Id,
                Name = consumer.Name,
                Surname = consumer.Surname,
                Patronymic = consumer.Patronymic,
                Address = consumer.Address,
            };

            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                model.PhoneNumbers.Add(ordinarConsumer.PhoneNumber);
                model.MonthlyFee = _dbContext.Tariffs.Where(tariff =>
                    tariff.Name == consumer.TariffName).Single().MonthlyFeeForOrdinarConsumer;
            }
            else if (consumer is VIPConsumer VIPConsumer)
            {
                model.PhoneNumbers = VIPConsumer.PhoneNumbers.Split(",").ToList();
                model.MonthlyFee = _dbContext.Tariffs.Where(tariff =>
                    tariff.Name == consumer.TariffName).Single().MonthlyFeeForVIPConsumer;
                model.MonthlyFee -= model.MonthlyFee * 0.1 * (model.PhoneNumbers.Count / 5);
                model.MonthlyFee = Math.Round(model.MonthlyFee, 2);
            }

            int HowManyMonthsIsActive = (date.Year - consumer.RegistrationDate.Year) * 12 + date.Month - consumer.RegistrationDate.Month;
            model.Balance = Math.Round(consumer.TotalMoney - HowManyMonthsIsActive * model.MonthlyFee, 2);
            if (model.Balance > consumer.TotalMoney)
            {
                model.Balance = consumer.TotalMoney;
            }

            model.Status = model.Balance < 0 ? "Відключено" : "Підключено";
            return model;
        }

        public IEnumerable<ViewAllModel> ConvertMany(IEnumerable<BaseConsumer> consumers, string dateAsString)
        {
            List<ViewAllModel> models = new List<ViewAllModel>(consumers.Count());
            foreach (var consumer in consumers)
            {
                var model = Convert(consumer, dateAsString);
                models.Add(model);
            }
            return models;
        }
    }
}
