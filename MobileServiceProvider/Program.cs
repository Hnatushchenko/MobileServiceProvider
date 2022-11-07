using Microsoft.EntityFrameworkCore;
using MobileServiceProvider.Repository;
using MobileServiceProvider.Services;

namespace MobileServiceProvider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.Add(ServiceDescriptor.Transient(typeof(IConsumersFromFileLoader<>), typeof(ConsumersFromFileLoader<>)));
            builder.Services.AddTransient<IRandomPhoneCallsGenerator, RandomPhoneCallsGenerator>();
            builder.Services.AddTransient<IRandomDateGenerator, RandomDateGenerator>();
            builder.Services.AddTransient<IRandomPhoneNumberGenerator, RandomPhoneNumberGenerator>();
            builder.Services.AddTransient<IConsumerValidator, ConsumerValidator>();
            builder.Services.AddTransient<IConsumerToDisplayModelConverter, ConsumerToDisplayModelConverter>();
            builder.Services.AddSingleton<IDisplayModelSorter, DisplayModelSorter>();
            builder.Services.AddTransient<IConsumersService, ConsumersService>();
            builder.Services.AddTransient<IMobilePhoneService, MobilePhoneService>();
            builder.Services.AddTransient<ITariffsService, TariffsService>();
            builder.Services.AddTransient<IPhoneCallsService, PhoneCallsService>();
            builder.Services.AddTransient<ApplicationContext>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "PhoneCalls",
                pattern: "PhoneCalls",
                defaults: new { controller = "PhoneCalls", action = "Display" }
                );
            app.MapControllerRoute( 
                name: "default",
                pattern: "{controller=Consumer}/{action=Display}");

            app.Run();
        }
    }
}