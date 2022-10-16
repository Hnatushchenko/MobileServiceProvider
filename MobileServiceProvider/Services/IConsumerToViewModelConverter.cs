using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
	public interface IConsumerToViewModelConverter
	{
		IEnumerable<ViewAllModel> ConvertMany(IEnumerable<BaseConsumer> consumers, string dateAsString);
		ViewAllModel Convert(BaseConsumer consumer, string dateAsString);
	}
}