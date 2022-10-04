using MobileServiceProvider.Repository;
using MobileServiceProvider.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MobileServiceProvider.Services;

public class ConsumerValidator : IConsumerValidator
{
    private readonly ApplicationContext _dbContext;
    public ConsumerValidator(ApplicationContext context)
    {
        _dbContext = context;
    }

    public ValidationResult? Validate(BaseConsumer consumerToValidate)
    {
        if (string.IsNullOrWhiteSpace(consumerToValidate.Name))
        {
            return new ValidationResult("Ім'я не має бути порожнім.");
        }
        if (string.IsNullOrWhiteSpace(consumerToValidate.Surname))
        {
            return new ValidationResult("Прізвище не має бути порожнім.");
        }
        if (string.IsNullOrWhiteSpace(consumerToValidate.Patronymic))
        {
            return new ValidationResult("Ім'я по батькові не має бути порожнім.");
        }
        if (string.IsNullOrWhiteSpace(consumerToValidate.Address))
        {
            return new ValidationResult("Адреса не має бути порожнью.");
        }
        if (consumerToValidate.TotalMoney < 0)
        {
            return new ValidationResult("Початковий баланс має бути більше 0");
        }
        if (consumerToValidate is OrdinarConsumer ordinarConsumer)
        {
            foreach (var consumer in _dbContext.OrdinarConsumers)
            {
                if (consumer.PhoneNumber == ordinarConsumer.PhoneNumber)
                {
                    return new ValidationResult($"Абонент з мобільним номером {ordinarConsumer.PhoneNumber} вже існує.");
                }
            }
            foreach (var consumer in _dbContext.VIPConsumers)
            {
                if (consumer.PhoneNumbers.Contains(ordinarConsumer.PhoneNumber))
                {
                    return new ValidationResult($"Абонент з мобільним номером {ordinarConsumer.PhoneNumber} вже існує.");
                }
            }
            if (!Regex.IsMatch(ordinarConsumer.PhoneNumber, @"^\+380[0-9]{9}$"))
            {
                return new ValidationResult("Мобільний номер не відповідає формату +380XXXXXXXXX");
            }
        }
        else if (consumerToValidate is VIPConsumer VIPconsumer)
        {
            foreach (string phoneNumber in VIPconsumer.PhoneNumbers.Split(", "))
            {
                foreach (var consumer in _dbContext.OrdinarConsumers)
                {
                    if (consumer.PhoneNumber == phoneNumber)
                    {
                        return new ValidationResult($"Абонент з мобільним номером {phoneNumber} вже існує.");
                    }
                }
                foreach (var consumer in _dbContext.VIPConsumers)
                {
                    if (consumer.PhoneNumbers.Contains(phoneNumber))
                    {
                        return new ValidationResult($"Абонент з мобільним номером {phoneNumber} вже існує.");
                    }
                }
                if (!Regex.IsMatch(phoneNumber, @"^\+380[0-9]{9}$"))
                {
                    return new ValidationResult("Мобільний номер не відповідає формату +380XXXXXXXXX");
                }
            }
        }
        return ValidationResult.Success;
    }
}
