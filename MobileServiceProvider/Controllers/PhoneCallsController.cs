using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Controllers
{
    public class PhoneCallsController : Controller
    {
        private readonly ApplicationContext _dbContext;
        public PhoneCallsController(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Display([FromQuery] Guid? consumerId)
        {
            IEnumerable<PhoneCallInfo> phoneCalls = _dbContext.PhoneCalls
                                                 .OrderBy(d => d.StartDate);

            if (consumerId is not null)
            {
                phoneCalls = phoneCalls.Where(call => call.ConsumerId == consumerId);
            }

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
