using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Controllers
{
    public class MobilePhoneController : Controller
    {
        [HttpGet] // MobilePhone/Calls?consumerId=
        public IActionResult Calls([FromServices] ApplicationContext dbContext, [FromQuery] Guid? consumerId)
        {
            List<PhoneCall> phoneCalls = dbContext.PhoneCalls.Where(x => x.ConsumerId == consumerId).ToList();
            return View(phoneCalls);
        }
        [HttpGet]
        public IActionResult Charge()
        {
            return View();
        }

        [HttpPost] 
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
                return View(new ResultViewModel
                { 
                    Success = false, 
                    Title = "Помилка поповнення",
                    Details = $"Неможливо перетворити \"{sumAsString}\" у число."
                });
            }

            BaseConsumer? consumer = dbContext.OrdinarConsumers.SingleOrDefault(consumer => consumer.PhoneNumber == phoneNumber);
            if (consumer == null)
            {
                consumer = dbContext.VIPConsumers.SingleOrDefault(consumer => consumer.PhoneNumbers.Contains(phoneNumber));
            }
            if (consumer == null)
            {
                return View(new ResultViewModel
                {
                    Success = false,
                    Title = "Помилка поповнення",
                    Details = $"Абонента з мобільним номером {phoneNumber} не знайдено." });
            }
            consumer.TotalMoney += sum;
            dbContext.SaveChanges();

            return View(new ResultViewModel 
            { 
                Success = true, 
                Title = "Поповнення пройшло успішно", 
                Details = $"Мобільний номер {phoneNumber} було поповнено на {sum}."
            });
        }
    }
}
