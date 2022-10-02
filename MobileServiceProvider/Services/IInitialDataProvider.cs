using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IInitialDataProvider
    {
        Tariff[] GetTariffs();
        OrdinarConsumer[] GetOrdinarConsumers();
        VIPConsumer[] GetVIPConsumers();
    }
}