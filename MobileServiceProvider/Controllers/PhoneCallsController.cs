using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Services;

namespace MobileServiceProvider.Controllers
{
    public class PhoneCallsController : Controller
    {
        IPhoneCallsService _phoneCallsService;

        public PhoneCallsController(IPhoneCallsService phoneCallsService)
        {
            _phoneCallsService = phoneCallsService;
        }

        [HttpGet]
        public IActionResult Display([FromQuery] Guid? consumerId)
        {
            var phoneCalls = _phoneCallsService.GetPhoneCalls(consumerId);

            if (phoneCalls.Count() == 0)
            {
                return View(viewName: "NoData", new ResultViewModel()
                {
                    Title = "Телефонні дзвінки відсутні"
                });
            }

            return View(phoneCalls);
        }
    }
}
