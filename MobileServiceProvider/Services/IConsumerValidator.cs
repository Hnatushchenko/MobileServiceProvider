using MobileServiceProvider.Models;
using System.ComponentModel.DataAnnotations;

namespace MobileServiceProvider.Services
{
    public interface IConsumerValidator
    {
        ValidationResult? Validate(BaseConsumer consumerToValidate);
    }
}