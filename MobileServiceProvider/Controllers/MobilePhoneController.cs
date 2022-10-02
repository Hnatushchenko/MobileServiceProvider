using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Controllers
{
    public class MobilePhoneController : Controller
    {
        [HttpGet]
        public IActionResult Charge()
        {
            return View();
        }

        [HttpPost] // MobilePhone/Charge
        public IActionResult ChargeResult([FromServices] ApplicationContext dbContext)
        {
            string phoneNumber = Request.Form["phoneNumber"];
            string sumAsString = Request.Form["sum"];
            double sum;
            
            try
            {
                sum = Convert.ToDouble(sumAsString);
            }
            catch (FormatException)
            {
                string message = $"Неможливо перетворити \"{sumAsString}\" у число.";
                return View(new ChargeResultViewModel { Success = false, Details = message });
            }

            BaseConsumer? consumer = dbContext.OrdinarConsumers.SingleOrDefault(consumer => consumer.PhoneNumber == phoneNumber);
            if (consumer == null)
            {
                consumer = dbContext.VIPConsumers.SingleOrDefault(consumer => consumer.PhoneNumbers.Contains(phoneNumber));
            }
            if (consumer == null)
            {
                return View(new ChargeResultViewModel { Success = false, Details = $"Клієнта з мобільним номером \"{phoneNumber}\" не знайдено." });
            }
            consumer.TotalMoney += sum;
            dbContext.SaveChanges();

            return View(new ChargeResultViewModel { Success = true, Details = $"Мобільний номер {phoneNumber} було поповнено на {sum}." });
        }
    }
}
