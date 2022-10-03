using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;
using MobileServiceProvider.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;


namespace MobileServiceProvider.Controllers
{
    public class ConsumerController : Controller
    {
        private readonly IViewAllModelSorter _sorter;
        private readonly IConsumerToViewModelConverter _converter;

        public ConsumerController(IViewAllModelSorter viewAllModelSorter, IConsumerToViewModelConverter converter)
        {
            _converter = converter;
            _sorter = viewAllModelSorter;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAll([FromServices] ApplicationContext dbContext)
        {
            List<BaseConsumer> consumers = new List<BaseConsumer>();
            await dbContext.OrdinarConsumers.ForEachAsync(consumers.Add);
            await dbContext.VIPConsumers.ForEachAsync(consumers.Add);

            List<ViewAllModel> models = new List<ViewAllModel>(consumers.Count());

            string? date = Request.Query["date"];
            date ??= DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["date"] = date;

            string? orderBy = Request.Query["orderby"];
            orderBy ??= "id";
            ViewData["orderby"] = orderBy;

            string? order = Request.Query["order"];
            order ??= "ascending";
            ViewData["order"] = order;

            foreach (var consumer in consumers)
            {
                ViewAllModel model = _converter.Convert(consumer, date);
                models.Add(model);
            }

            var sortedModels = _sorter.Sort(models, orderBy, order);
            dbContext.Dispose();
            return View(sortedModels);
        }

        [HttpGet]
        public async Task<IActionResult> Load([FromServices] ApplicationContext dbContext, [FromServices] IInitialDataProvider dataProvider)
        {
            //Tariff[] tariffs = dataProvider.GetTariffs();
            OrdinarConsumer[] ordinarConsumers = dataProvider.GetOrdinarConsumers();
            VIPConsumer[] VIPConsumers = dataProvider.GetVIPConsumers();

            //await dbContext.Tariffs.AddRangeAsync(tariffs);
            await dbContext.OrdinarConsumers.AddRangeAsync(ordinarConsumers);
            await dbContext.VIPConsumers.AddRangeAsync(VIPConsumers);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Content("Loaded");
        }
        [HttpGet]
        public async Task<IActionResult> Remove([FromServices] ApplicationContext dbContext, [FromQuery] Guid id)
        {
            BaseConsumer? consumer = await dbContext.OrdinarConsumers.SingleOrDefaultAsync(consumer => consumer.Id == id);
            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                dbContext.OrdinarConsumers.Remove(ordinarConsumer);
            }
            else
            {
                consumer = await dbContext.VIPConsumers.SingleOrDefaultAsync(consumer => consumer.Id == id);
                if (consumer is VIPConsumer VIPconsumer)
                {
                    dbContext.VIPConsumers.Remove(VIPconsumer);
                }
                else
                {
                    await dbContext.SaveChangesAsync();
                    await dbContext.DisposeAsync();
                    return NotFound("Not found");
                }
            }
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return LocalRedirect("~/Consumer/ViewAll");
        }
    }
}
