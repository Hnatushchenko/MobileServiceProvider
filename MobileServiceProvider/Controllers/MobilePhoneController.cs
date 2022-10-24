﻿using Microsoft.AspNetCore.Mvc;
using MobileServiceProvider.Models;
using MobileServiceProvider.Enums;
using MobileServiceProvider.Repository;

namespace MobileServiceProvider.Controllers
{
    public class MobilePhoneController : Controller
    {
        private readonly ApplicationContext _dbContext;

        public MobilePhoneController(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Charge()
        {
            return View();
        }

        [HttpPost] 
        public IActionResult ChargeResult()
        {
            string phoneNumber = Request.Form["phoneNumber"];
            string sumAsString = Request.Form["sum"];
            double sum;
            
            try
            {
                sum = Convert.ToDouble(sumAsString);
            }
            catch (FormatException)
            {
                return View(new ResultViewModel
                { 
                    Type = ResultType.Error, 
                    Title = "Помилка поповнення",
                    Details = $"Неможливо перетворити \"{sumAsString}\" у число."
                });
            }

            BaseConsumer? consumer = _dbContext.OrdinarConsumers.SingleOrDefault(consumer => consumer.PhoneNumber == phoneNumber);
            if (consumer == null)
            {
                consumer = _dbContext.VIPConsumers.SingleOrDefault(consumer => consumer.PhoneNumbers.Contains(phoneNumber));
            }
            if (consumer == null)
            {
                return View(new ResultViewModel
                {
                    Type = ResultType.Error,
                    Title = "Помилка поповнення",
                    Details = $"Абонента з мобільним номером {phoneNumber} не знайдено." });
            }
            consumer.TotalMoney += sum;
            _dbContext.SaveChanges();

            return View(new ResultViewModel 
            { 
                Type = ResultType.Success, 
                Title = "Поповнення пройшло успішно", 
                Details = $"Мобільний номер {phoneNumber} було поповнено на {sum}."
            });
        }
    }
}
