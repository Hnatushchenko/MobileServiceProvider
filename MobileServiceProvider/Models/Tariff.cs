namespace MobileServiceProvider.Models
{
    public class Tariff
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int MonthlyFeeForOrdinarConsumer { get; set; }
        public int MonthlyFeeForVIPConsumer { get; set; }
    }
}
