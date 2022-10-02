namespace MobileServiceProvider.Models
{
	public class ViewAllModel
	{
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Address { get; set; }
        public List<string> PhoneNumbers { get; set; } = new();
        public double MonthlyFee { get; set; }
        public double Balance { get; set; }
        public string? Status { get; set; }
    }
}
