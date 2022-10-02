using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
	public interface IConsumerToViewModelConverter
	{
		ViewAllModel Convert(BaseConsumer consumer);
	}
}