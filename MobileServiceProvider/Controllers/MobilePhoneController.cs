using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Enums;
using MobileServiceProvider.Services;
using MobileServiceProvider.Exceptions;

namespace MobileServiceProvider.Controllers
{
    public class MobilePhoneController : Controller
    {
        private readonly IMobilePhoneService _mobilePhoneService;

        public MobilePhoneController(IMobilePhoneService mobilePhoneService)
        {
            _mobilePhoneService = mobilePhoneService;
        }

        [HttpGet]
        public IActionResult Charge()
        {
            return View();
        }

        [HttpPost] 
        public IActionResult ChargeResult()
        {
            string phoneNumber = Request.Form["phoneNumber"];
            string sum = Request.Form["sum"];

            try
            {
                _mobilePhoneService.Charge(phoneNumber, sum);
            }
            catch (FormatException)
            {
                return View(new ResultViewModel
                {
                    Type = ResultType.Error,
                    Title = "Помилка поповнення",
                    Details = $"Неможливо перетворити \"{sum}\" у число."
                });
            }
            catch (ConsumerNotFoundException)
            {
                return View(new ResultViewModel
                {
                    Type = ResultType.Error,
                    Title = "Помилка поповнення",
                    Details = $"Абонента з мобільним номером {phoneNumber} не знайдено."
                });
            }
            return View(new ResultViewModel 
            { 
                Type = ResultType.Success, 
                Title = "Поповнення пройшло успішно", 
                Details = $"Мобільний номер {phoneNumber} було поповнено на {sum}."
            });
        }
    }
}
