using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Controllers
{
    public class TariffsController : Controller
    {
        ApplicationContext _dbContext;
        public TariffsController(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Display()
        {
            var tariffs = await _dbContext.Tariffs.AsNoTracking().ToListAsync();
            return View(tariffs);
        }
    }
}
