using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IDisplayModelSorter
    {
        IEnumerable<DisplayModel> Sort(IEnumerable<DisplayModel> models, string propertyName, string order);
    }
}