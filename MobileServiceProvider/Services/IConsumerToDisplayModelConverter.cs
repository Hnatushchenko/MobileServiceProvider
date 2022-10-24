using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
	public interface IConsumerToDisplayModelConverter
	{
		IEnumerable<DisplayModel> ConvertMany(IEnumerable<BaseConsumer> consumers, string dateAsString);
		DisplayModel Convert(BaseConsumer consumer, string dateAsString);
	}
}