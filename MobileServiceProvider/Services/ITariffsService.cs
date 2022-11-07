using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface ITariffsService
    {
        IEnumerable<Tariff> GetAllTariffs();
        IEnumerable<string?> GetTariffNames();
    }
}