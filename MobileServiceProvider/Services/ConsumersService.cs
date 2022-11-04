using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Enums;
using MobileServiceProvider.Exceptions;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

namespace MobileServiceProvider.Services
{
    public class ConsumersService : IConsumersService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationContext _dbContext;
        private readonly IDisplayModelSorter _sorter;
        private readonly IConsumerToDisplayModelConverter _converter;
        private readonly IRandomPhoneCallsGenerator _randomPhoneCallsGenerator;
        private readonly IConsumerValidator _validator;

        public ConsumersService(ApplicationContext dbContext, IDisplayModelSorter sorter, IConsumerToDisplayModelConverter converter, IServiceProvider serviceProvider, IRandomPhoneCallsGenerator randomPhoneCallsGenerator, IConsumerValidator validator)
        {
            _dbContext = dbContext;
            _sorter = sorter;
            _converter = converter;
            _serviceProvider = serviceProvider;
            _randomPhoneCallsGenerator = randomPhoneCallsGenerator;
            _validator = validator;
        }

        public async Task AddAsync(ConsumerType consumerType, string name, string surname, string patronymic, string address,
            string tariff, string registrationDate, string phoneNumber)
        {
            BaseConsumer consumer;

            consumer = consumerType switch
            {
                ConsumerType.OrdinarConsumer => new OrdinarConsumer(),
                ConsumerType.VIPConsumer => new VIPConsumer(),
                _ => throw new ArgumentException($"Unknown type of consumer: {consumerType}")
            };

            consumer.Id = Guid.NewGuid();
            consumer.Name = name;
            consumer.Surname = surname;
            consumer.Patronymic = patronymic;
            consumer.Address = address;
            consumer.TariffName = tariff;
            consumer.RegistrationDate = DateTime.ParseExact(registrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                ordinarConsumer.PhoneNumber = phoneNumber;

            }
            else if (consumer is VIPConsumer VIPconsumer)
            {
                VIPconsumer.PhoneNumbers = phoneNumber;
            }

            ValidationResult? validationrResult = _validator.Validate(consumer);

            if (validationrResult == ValidationResult.Success)
            {
                if (consumerType == ConsumerType.OrdinarConsumer)
                {
                    await _dbContext.OrdinarConsumers.AddAsync((OrdinarConsumer)consumer);
                }
                else if (consumerType == ConsumerType.VIPConsumer)
                {
                    await _dbContext.VIPConsumers.AddAsync((VIPConsumer)consumer);
                }

                await _dbContext.SaveChangesAsync();
                await _randomPhoneCallsGenerator.GenerateForAsync(consumer, DateTimeOffset.Now);

                return;   
            }

            throw new ValidationException(validationrResult!.ErrorMessage ?? string.Empty);
        }

        public async Task RemoveAsync(Guid id)
        {
            BaseConsumer? consumer = await _dbContext.OrdinarConsumers.SingleOrDefaultAsync(consumer => consumer.Id == id);
            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                _dbContext.OrdinarConsumers.Remove(ordinarConsumer);
            }
            else
            {
                consumer = await _dbContext.VIPConsumers.SingleOrDefaultAsync(consumer => consumer.Id == id);
                if (consumer is VIPConsumer VIPconsumer)
                {
                    _dbContext.VIPConsumers.Remove(VIPconsumer);
                }
                else
                {
                    throw new ConsumerNotFoundException();
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<DisplayModel> GetDisplayModels(string date, string order, string orderBy)
        {
            List<BaseConsumer> consumers = new List<BaseConsumer>();
            _dbContext.OrdinarConsumers.ToList().ForEach(consumers.Add);
            _dbContext.VIPConsumers.ToList().ForEach(consumers.Add);

            var models = _converter.ConvertMany(consumers, date);
            models = _sorter.Sort(models, orderBy, order);

            return models;
        }

        public async Task UploadFromFileAsync(IFormFile file, ConsumerType consumerType)
        {
            if (consumerType == ConsumerType.OrdinarConsumer)
            {
                IConsumersFromFileLoader<OrdinarConsumer> loader = _serviceProvider.GetRequiredService<IConsumersFromFileLoader<OrdinarConsumer>>();
                OrdinarConsumer[] consumers = await loader.LoadAsync(file);
                await _dbContext.OrdinarConsumers.AddRangeAsync(consumers);
                await _dbContext.SaveChangesAsync();
                foreach (var consumer in consumers)
                {
                    await _randomPhoneCallsGenerator.GenerateForAsync(consumer, DateTimeOffset.Now);
                }
                return;
            }

            if (consumerType == ConsumerType.VIPConsumer)
            {
                IConsumersFromFileLoader<VIPConsumer> loader = _serviceProvider.GetRequiredService<IConsumersFromFileLoader<VIPConsumer>>();
                VIPConsumer[] consumers = await loader.LoadAsync(file);
                await _dbContext.VIPConsumers.AddRangeAsync(consumers);
                await _dbContext.SaveChangesAsync();
                foreach (var consumer in consumers)
                {
                    await _randomPhoneCallsGenerator.GenerateForAsync(consumer, DateTimeOffset.Now);
                }
            }
        }
    }
}
