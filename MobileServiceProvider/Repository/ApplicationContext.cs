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

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=MobileSeviceProviderDatabase;Trusted_Connection=True;");
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    if (!initialDataLoaded)
        //    {
        //        Tariff[] tariffs = _initialDataProvider.GetTariffs();
        //        OrdinarConsumer[] ordinarConsumers = _initialDataProvider.GetOrdinarConsumers();
        //        VIPConsumer[] VIPConsumers = _initialDataProvider.GetVIPConsumers();

        //        modelBuilder.Entity<Tariff>().HasData(tariffs);
        //        modelBuilder.Entity<OrdinarConsumer>().HasData(ordinarConsumers);
        //        modelBuilder.Entity<VIPConsumer>().HasData(VIPConsumers);
        //        initialDataLoaded = true;
        //    }
        //}
    }
}
