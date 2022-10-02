﻿using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public class ViewAllModelSorter : IViewAllModelSorter
    {
        public IEnumerable<ViewAllModel> Sort(List<ViewAllModel> models, string? propertyName, string? order)
        {
            if (propertyName is null || order is null)
            {
                return models;
            }

            if (propertyName == "phoneNumber")
            {
                if (order == "ascending")
                {
                    foreach (var model in models)
                    {
                        model.PhoneNumbers.Sort();
                    }
                }
                else if (order == "descending")
                {
                    foreach (var model in models)
                    {
                        model.PhoneNumbers.Sort((phoneNumber1, phoneNumber2) => phoneNumber2.CompareTo(phoneNumber1));
                    }
                }
            }

            switch (order)
            {
                case "ascending":
                    return propertyName switch
                    {
                        "name" => models.OrderBy(model => model.Name),
                        "surname" => models.OrderBy(model => model.Surname),
                        "patronymic" => models.OrderBy(model => model.Patronymic),
                        "address" => models.OrderBy(model => model.Address),
                        "phoneNumber" => models.OrderBy(model => model.PhoneNumbers.First()),
                        "monthlyFee" => models.OrderBy(model => model.MonthlyFee),
                        "balance" => models.OrderBy(model => model.Balance),
                        _ => throw new ArgumentException($"ViewAllModel doesn't contain a \"{propertyName}\" property.")
                    };
                case "descending":
                    return propertyName switch
                    {
                        "name" => models.OrderByDescending(model => model.Name),
                        "surname" => models.OrderByDescending(model => model.Surname),
                        "patronymic" => models.OrderByDescending(model => model.Patronymic),
                        "address" => models.OrderByDescending(model => model.Address),
                        "phoneNumber" => models.OrderByDescending(model => model.PhoneNumbers.First()),
                        "monthlyFee" => models.OrderByDescending(model => model.MonthlyFee),
                        "balance" => models.OrderByDescending(model => model.Balance),
                        _ => throw new ArgumentException($"ViewAllModel doesn't contain a \"{propertyName}\" property.")

                    };
                default:
                    throw new ArgumentException($"Order cannot be \"{order}\" in ViewAllModelSorter");
            }
        }
    }
}
