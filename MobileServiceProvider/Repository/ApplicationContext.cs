using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Models;
using MobileServiceProvider.Services;

namespace MobileServiceProvider.Repository
{
	public class ApplicationContext : DbContext
	{
        public DbSet<OrdinarConsumer> OrdinarConsumers { get; set; } = null!;
        public DbSet<VIPConsumer> VIPConsumers { get; set; } = null!;
		public DbSet<Tariff> Tariffs { get; set; } = null!;
        public DbSet<PhoneCallInfo> PhoneCalls { get; set; } = null!;

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=MobileSeviceProviderDatabase;Trusted_Connection=True;");
        }
    }
}
