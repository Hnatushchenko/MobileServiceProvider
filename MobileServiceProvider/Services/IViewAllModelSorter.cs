using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IViewAllModelSorter
    {
        IEnumerable<ViewAllModel> Sort(IEnumerable<ViewAllModel> models, string propertyName, string order);
    }
}