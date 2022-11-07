using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Enums;
using MobileServiceProvider.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MobileServiceProvider.Exceptions;

namespace MobileServiceProvider.Controllers
{
    public class ConsumerController : Controller
    {
        private readonly IConsumersService _consumersService;
        private readonly ITariffsService _tariffsService;

        public ConsumerController(IConsumersService consumersService, ITariffsService tariffsService)
        {
            _consumersService = consumersService;
            _tariffsService = tariffsService;
        }

        [HttpGet]
        public IActionResult Display()
        {
            string? date = Request.Query["date"];
            date ??= DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["date"] = date;

            string? orderBy = Request.Query["orderby"];
            orderBy ??= "id";
            ViewData["orderby"] = orderBy;

            string? order = Request.Query["order"];
            order ??= "ascending";
            ViewData["order"] = order;

            var models = _consumersService.GetDisplayModels(date, order, orderBy);
            
            if (models.Count() == 0)
            {
                return View(viewName: "NoConsumers", new ResultViewModel()
                {
                    Title = "Абоненти відсутні"
                });
            }

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
            ResultViewModel resultViewModel;
            try
            {
                IFormFile file = Consumers.Single();
                await _consumersService.UploadFromFileAsync(file, consumerType);
            }
            catch (InvalidOperationException)
            {
                resultViewModel = new ResultViewModel
                {
                    Title = "Помилка завантеження",
                    Details = "Жоден файл не було вибрано.",
                    Type = ResultType.Error,
                };
                return View("Result", resultViewModel);
            }
            catch (JsonException)
            {
                resultViewModel = new ResultViewModel
                {
                    Title = "Помилка завантеження",
                    Details = "Дані зберігаються у непральвиному форматі. Файл повинен містити розширення .json.",
                    Type = ResultType.Error,
                };
                return View("Result", resultViewModel);
            }
            catch (ValidationException validationException)
            {
                resultViewModel = new ResultViewModel
                {
                    Type = ResultType.Error,
                    Title = "Помилка при додаванні абонента",
                    Details = validationException.Message
                };
                return View(viewName: "Result", resultViewModel);
            }
            resultViewModel = new ResultViewModel
            {
                Title = "Абоненти успішно додані",
                Type = ResultType.Success,
            };
            return View("Result", resultViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            try
            {
                await _consumersService.RemoveAsync(id);
            }
            catch (ConsumerNotFoundException)
            {
                return NotFound("Not found");
            }
            
            return LocalRedirect("~/Consumer/Display"); 
        }
        [HttpGet]
        public IActionResult Add()
        {
            AddConsumerViewModel model = new AddConsumerViewModel();
            model.TariffNames = _tariffsService.GetTariffNames().ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromServices] IConsumerValidator validator, [FromForm] ConsumerType consumerType)
        {
            var form = Request.Form;
            try
            {
                await _consumersService.AddAsync(consumerType, form["name"], form["surname"], form["patronymic"], form["address"], form["tariff"],
                form["registrationDate"], form["phoneNumber"]);
            }
            catch (ValidationException validationException)
            {
                return View(viewName: "Result", new ResultViewModel
                {
                    Type = ResultType.Error,
                    Title = "Помилка при додаванні абонента",
                    Details = validationException.Message
                });
            }
            return View(viewName: "Result", new ResultViewModel
            {
                Type = ResultType.Success,
                Title = "Абонент успішно доданий",
                Details = $"Абонент {form["surname"]} {form["name"]} {form["patronymic"]} успішно доданий"
            });
        }
    }
}
