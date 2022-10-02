﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ViewAll([FromServices] ApplicationContext dbContext)
        {
            List<BaseConsumer> consumers = new List<BaseConsumer>();
            dbContext.OrdinarConsumers.ToList().ForEach(consumers.Add);
            dbContext.VIPConsumers.ToList().ForEach(consumers.Add);

            List<ViewAllModel> models = new List<ViewAllModel>(consumers.Count());

            foreach (var consumer in consumers)
            {
                ViewAllModel model = _converter.Convert(consumer);
                models.Add(model);
            }

            var sortedModels = _sorter.Sort(models, Request.Query["orderby"], Request.Query["order"]);
            dbContext.Dispose();
            return View(sortedModels);
        }

        [HttpGet]
        public async Task<IActionResult> Load([FromServices] ApplicationContext dbContext, [FromServices] IInitialDataProvider dataProvider)
        {
            Tariff[] tariffs = dataProvider.GetTariffs();
            OrdinarConsumer[] ordinarConsumers = dataProvider.GetOrdinarConsumers();
            VIPConsumer[] VIPConsumers = dataProvider.GetVIPConsumers();

            await dbContext.Tariffs.AddRangeAsync(tariffs);
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
