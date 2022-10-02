namespace MobileServiceProvider.Services
{
	public class RealDateTimeProvider : IDateTimeProvider
	{
		public DateTime Now
		{
			get { return DateTime.Now; }
		}
	}
}
