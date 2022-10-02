using MobileServiceProvider.Models;
using System.Text.Json;

namespace MobileServiceProvider.Services
{
    class InitialDataProvider : IInitialDataProvider
    {
        public const string PathToTarrifs = @"InitialData\tariffs.json";
        public const string PathToOrdinarConsumers = @"InitialData\ordinarConsumers.json";
        public const string PathToVIPConsumers = @"InitialData\VIPConsumers.json";

        private readonly JsonSerializerOptions options;
        public InitialDataProvider()
        {
            options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new CustomDateTimeConverter("dd.MM.yyyy"));
        }

        public Tariff[] GetTariffs()
        {
            string tarrifsJSON = File.ReadAllText(PathToTarrifs);
            Tariff[]? tariffs = JsonSerializer.Deserialize<Tariff[]>(tarrifsJSON, options);
            if (tariffs is null)
            {
                throw new ArgumentNullException("Cannot deserialize tarrifs");
            }
            foreach (Tariff tariff in tariffs)
            {
                tariff.Id = Guid.NewGuid();
            }
            return tariffs;

        }
        public OrdinarConsumer[] GetOrdinarConsumers()
        {
            string ordinarConsumersJSON = File.ReadAllText(PathToOrdinarConsumers);
            OrdinarConsumer[]? ordinarConsumers = JsonSerializer.Deserialize<OrdinarConsumer[]>(ordinarConsumersJSON, options);
            if (ordinarConsumers is null)
            {
                throw new ArgumentNullException("Cannot deserialize ordinar consumers");
            }
            foreach (OrdinarConsumer consumer in ordinarConsumers)
            {
                consumer.Id = Guid.NewGuid();
            }
            return ordinarConsumers;
        }
        public VIPConsumer[] GetVIPConsumers()
        {
            string VIPConsumersJSON = File.ReadAllText(PathToVIPConsumers);
            VIPConsumer[]? VIPConsumers = JsonSerializer.Deserialize<VIPConsumer[]>(VIPConsumersJSON, options);
            if (VIPConsumers is null)
            {
                throw new ArgumentNullException("Cannot deserialize VIP Consumers");
            }
            foreach (VIPConsumer consumer in VIPConsumers)
            {
                consumer.Id = Guid.NewGuid();
            }
            return VIPConsumers;
        }
    }
}





