using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using MobileServiceProvider.Models;
using MobileServiceProvider.Repository;
using MobileServiceProvider.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;


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
            OrdinarConsumer[] ordinarConsumers = dataProvider.GetOrdinarConsumers();
            VIPConsumer[] VIPConsumers = dataProvider.GetVIPConsumers();

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
        [HttpGet]
        public async Task<IActionResult> Add([FromServices] ApplicationContext dbContext)
        {
            AddConsumerViewModel model = new AddConsumerViewModel();
            model.TariffNames = dbContext.Tariffs.Select(t => t.Name).ToList() ?? new List<string?>();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromServices] ApplicationContext dbContext, [FromServices] IConsumerValidator validator, [FromForm] string name)
        {
            var form = Request.Form;
            BaseConsumer consumer;

            if (form["type"] == "ordinar")
            {
                consumer = new OrdinarConsumer();
            }
            else if (form["type"] == "VIP")
            {
                consumer = new VIPConsumer();
            }
            else
            {
                throw new ArgumentException($"Unknown type of consumer: {form["type"]}");
            }

            consumer.Id = Guid.NewGuid();
            consumer.Name = name;
            consumer.Surname = form["surname"];
            consumer.Patronymic = form["patronymic"];
            consumer.Address = form["address"];
            consumer.TariffName = form["tariff"];
            consumer.RegistrationDate = DateTime.Now;

            {
                if (consumer is OrdinarConsumer ordinarConsumer)
                {
                    ordinarConsumer!.PhoneNumber = form["phoneNumber"];
                }
                else if (consumer is VIPConsumer VIPconsumer)
                {
                    VIPconsumer!.PhoneNumbers = form["phoneNumber"];
                }
            }

            ValidationResult? validationrResult = validator.Validate(consumer);

            if (validationrResult == ValidationResult.Success)
            {
                if (consumer is OrdinarConsumer ordinarConsumer)
                {
                    await dbContext.OrdinarConsumers.AddAsync(ordinarConsumer);
                }
                else if (consumer is VIPConsumer VIPconsumer)
                {
                    await dbContext.VIPConsumers.AddAsync(VIPconsumer);
                }
                await dbContext.SaveChangesAsync();
                return View(viewName: "Result", new ResultViewModel
                {
                    Success = true,
                    Title = "Абонент успішно доданий",
                    Details = $"Абонент {form["surname"]} {name} {form["patronymic"]} успішно доданий"
                });
            }
            else
            {
                return View(viewName: "Result", new ResultViewModel
                {
                    Success = false,
                    Title = "Помилка при додаванні абонента",
                    Details = validationrResult!.ErrorMessage ?? ""
                });
            }
        }
    }
}
