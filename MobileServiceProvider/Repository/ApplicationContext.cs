﻿using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Models;
using MobileServiceProvider.Services;
using System.Xml.Linq;

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
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=MobileSeviceProviderDatabase;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tariff>().HasData(
                new Tariff
                {
                    Id = Guid.NewGuid(),
                    Name = "Lite",
                    Description = "5 ГБ мобільний інтернет. Безлім дзвінки. 100 хв дзвінки за кордон.",
                    MonthlyFeeForOrdinarConsumer = 100,
                    MonthlyFeeForVIPConsumer = 80
                },
                new Tariff
                {
                    Id = Guid.NewGuid(),
                    Name = "LOVE UA",
                    Description = "15 ГБ мобільний інтернет. Безлім дзвінки. 200 хв дзвінки за кордон.",
                    MonthlyFeeForOrdinarConsumer = 200,
                    MonthlyFeeForVIPConsumer = 180
                },
                new Tariff
                {
                    Id = Guid.NewGuid(),
                    Name = "SuperNet",
                    Description = "Безлім мобільний інтернет. Безлім дзвінки. 250 хв дзвінки за кордон.",
                    MonthlyFeeForOrdinarConsumer = 350,
                    MonthlyFeeForVIPConsumer = 300
                }
            );
        }
    }
}
