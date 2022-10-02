namespace MobileServiceProvider.Models
{
    public class PhoneCall
    {
        public string? FromNumber { get; set; }
        public string? ToNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
