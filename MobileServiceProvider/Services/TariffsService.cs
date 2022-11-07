using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Services
{
    public class TariffsService : ITariffsService
    {
        private readonly ApplicationContext _dbContext;

        public TariffsService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Tariff> GetAllTariffs()
        {
            return _dbContext.Tariffs.AsNoTracking();
        }

        public IEnumerable<string?> GetTariffNames()
        {
            return _dbContext.Tariffs.AsNoTracking().Select(t => t.Name);
        }
    }
}
