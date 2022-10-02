namespace MobileServiceProvider.Models
{
    public abstract class BaseConsumer
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public double TotalMoney { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? TariffName { get; set; }
    }
}
