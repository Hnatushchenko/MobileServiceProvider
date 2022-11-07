using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Services;

namespace MobileServiceProvider.Controllers
{
    public class TariffsController : Controller
    {
        ITariffsService _tariffsService;

        public TariffsController(ITariffsService tariffsService)
        {
            _tariffsService = tariffsService;
        }

        public IActionResult Display()
        {
            var tariffs = _tariffsService.GetAllTariffs();
            return View(tariffs);
        }
    }
}
