using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using MobileServiceProvider.Models;
using MobileServiceProvider.Enums;
using MobileServiceProvider.Repository;
using MobileServiceProvider.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

namespace MobileServiceProvider.Controllers
{
    public class ConsumerController : Controller
    {
        private readonly IRandomPhoneCallsGenerator _randomPhoneCallsGenerator;
        private readonly IViewAllModelSorter _sorter;
        private readonly IConsumerToViewModelConverter _converter;
        private readonly ApplicationContext _dbContext;

        public ConsumerController(IViewAllModelSorter viewAllModelSorter, IConsumerToViewModelConverter converter, ApplicationContext dbContext, IRandomPhoneCallsGenerator randomPhoneCallsGenerator)
        {
            _converter = converter;
            _sorter = viewAllModelSorter;
            _dbContext = dbContext;
            _randomPhoneCallsGenerator = randomPhoneCallsGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAll()
        {
            List<BaseConsumer> consumers = new List<BaseConsumer>();
            _dbContext.OrdinarConsumers.ToList().ForEach(consumers.Add);
            _dbContext.VIPConsumers.ToList().ForEach(consumers.Add);

            if (consumers.Count() == 0)
            {
                return View(viewName: "NoData", new ResultViewModel()
                {
                    Title = "Абоненти відсутні"
                });
            }

            string? date = Request.Query["date"];
            date ??= DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["date"] = date;

            string? orderBy = Request.Query["orderby"];
            orderBy ??= "id";
            ViewData["orderby"] = orderBy;

            string? order = Request.Query["order"];
            order ??= "ascending";
            ViewData["order"] = order;

            var models = _converter.ConvertMany(consumers, date);
            models = _sorter.Sort(models, orderBy, order);
            return View(models);
        }

        [HttpGet]
        public IActionResult UploadFromFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFromFile([FromForm] IFormFileCollection Consumers, [FromForm] ConsumerType consumerType)
        {
            try
            {
                IFormFile file = Consumers.Single();
                if (consumerType == ConsumerType.OrdinarConsumer)
                {
                    IConsumersFromFileLoader<OrdinarConsumer> loader = HttpContext.RequestServices.GetRequiredService<IConsumersFromFileLoader<OrdinarConsumer>>();
                    OrdinarConsumer[] consumers = await loader.LoadAsync(file);
                    await _dbContext.OrdinarConsumers.AddRangeAsync(consumers);
                    await _dbContext.SaveChangesAsync();
                    foreach (var consumer in consumers)
                    {
                        await _randomPhoneCallsGenerator.GenerateForAsync(consumer, DateTimeOffset.Now);
                    }
                }
                else if (consumerType == ConsumerType.VIPConsumer)
                {
                    IConsumersFromFileLoader<VIPConsumer> loader = HttpContext.RequestServices.GetRequiredService<IConsumersFromFileLoader<VIPConsumer>>();
                    VIPConsumer[] consumers = await loader.LoadAsync(file);
                    await _dbContext.VIPConsumers.AddRangeAsync(consumers);
                    await _dbContext.SaveChangesAsync();
                    foreach (var consumer in consumers)
                    {
                        await _randomPhoneCallsGenerator.GenerateForAsync(consumer, DateTimeOffset.Now);
                    }
                }
                ResultViewModel resultViewModel = new ResultViewModel
                {
                    Title = "Абоненти успішно додані",
                    Type = ResultType.Success,
                };
                return View("Result", resultViewModel);
            }
            catch (JsonException)
            {
                ResultViewModel resultViewModel = new ResultViewModel
                {
                    Title = "Помилка завантеження",
                    Details = "Дані зберігаються у непральвиному форматі",
                    Type = ResultType.Error,
                };
                return View("Result", resultViewModel);
            }
            catch (ValidationException validationException)
            {
                var resultViewModel = new ResultViewModel
                {
                    Type = ResultType.Error,
                    Title = "Помилка при додаванні абонента",
                    Details = validationException.Message
                };
                return View(viewName: "Result", resultViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            BaseConsumer? consumer = await _dbContext.OrdinarConsumers.SingleOrDefaultAsync(consumer => consumer.Id == id);
            if (consumer is OrdinarConsumer ordinarConsumer)
            {
                _dbContext.OrdinarConsumers.Remove(ordinarConsumer);
            }
            else
            {
                consumer = await _dbContext.VIPConsumers.SingleOrDefaultAsync(consumer => consumer.Id == id);
                if (consumer is VIPConsumer VIPconsumer)
                {
                    _dbContext.VIPConsumers.Remove(VIPconsumer);
                }
                else
                {
                    await _dbContext.SaveChangesAsync();
                    return NotFound("Not found");
                }
            }
            await _dbContext.SaveChangesAsync();
            return LocalRedirect("~/Consumer/ViewAll");
        }
        [HttpGet]
        public IActionResult Add()
        {
            AddConsumerViewModel model = new AddConsumerViewModel();
            model.TariffNames = _dbContext.Tariffs.Select(t => t.Name).ToList() ?? new List<string?>();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromServices] IConsumerValidator validator, [FromServices] IRandomPhoneCallsGenerator randomPhoneCallsGenerator, [FromForm] ConsumerType consumerType)
        {
            var form = Request.Form;
            BaseConsumer consumer;

            consumer = consumerType switch
            {
                ConsumerType.OrdinarConsumer => new OrdinarConsumer(),
                ConsumerType.VIPConsumer => new VIPConsumer(),
                _ => throw new ArgumentException($"Unknown type of consumer: {form["type"]}")
            };

            consumer.Id = Guid.NewGuid();
            consumer.Name = form["name"];
            consumer.Surname = form["surname"];
            consumer.Patronymic = form["patronymic"];
            consumer.Address = form["address"];
            consumer.TariffName = form["tariff"];
            consumer.RegistrationDate = DateTime.ParseExact(form["registrationDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);


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
                    await _dbContext.OrdinarConsumers.AddAsync(ordinarConsumer);
                }
                else if (consumer is VIPConsumer VIPconsumer)
                {
                    await _dbContext.VIPConsumers.AddAsync(VIPconsumer);
                }

                await _dbContext.SaveChangesAsync();
                await randomPhoneCallsGenerator.GenerateForAsync(consumer, DateTimeOffset.Now);

                return View(viewName: "Result", new ResultViewModel
                {
                    Type = ResultType.Success,
                    Title = "Абонент успішно доданий",
                    Details = $"Абонент {form["surname"]} {form["name"]} {form["patronymic"]} успішно доданий"
                });
            }
            return View(viewName: "Result", new ResultViewModel
            {
                Type = ResultType.Error,
                Title = "Помилка при додаванні абонента",
                Details = validationrResult!.ErrorMessage ?? ""
            });
        }
    }
}
