using Microsoft.Extensions.Options;
using MobileServiceProvider.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;

namespace MobileServiceProvider.Services
{
    public class ConsumersFromFileLoader<TConsumer> : IConsumersFromFileLoader<TConsumer> where TConsumer : BaseConsumer
    {
        IConsumerValidator _validator;
        private readonly JsonSerializerOptions _options;

        public ConsumersFromFileLoader(IConsumerValidator validator)
        {
            _options = new JsonSerializerOptions() { WriteIndented = true };
            _options.Converters.Add(new CustomDateTimeConverter("dd.MM.yyyy"));
            _validator = validator;
        }

        public async Task<TConsumer[]> LoadAsync(IFormFile file)
        {
            string consumersAsJson;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                consumersAsJson = await reader.ReadToEndAsync();
            }

            TConsumer[]? consumers = JsonSerializer.Deserialize<TConsumer[]>(
                                                  consumersAsJson,
                                                  _options);
            if (consumers is null)
            {
                throw new JsonException("Cannot deserialize the file with ordinar consumers");
            }

            foreach (TConsumer consumer in consumers)
            {
                ValidationResult? result = _validator.Validate(consumer);
                if (result == ValidationResult.Success)
                {
                    consumer.Id = Guid.NewGuid();
                }
                else
                {
                    throw new ValidationException(result!.ErrorMessage);
                }
            }
            return consumers;
        }
    }
}