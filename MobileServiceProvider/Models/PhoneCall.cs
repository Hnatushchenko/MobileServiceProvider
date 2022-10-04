namespace MobileServiceProvider.Models
{
    public class PhoneCall
    {
        public Guid Id { get; set; }
        public string? FromNumber { get; set; }
        public string? ToNumber { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Guid ConsumerId { get; set; }
        public BaseConsumer? Consumer { get; set; }
    }
}
