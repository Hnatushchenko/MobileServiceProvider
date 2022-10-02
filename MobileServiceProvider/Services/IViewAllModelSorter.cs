using MobileServiceProvider.Models;

namespace MobileServiceProvider.Services
{
    public interface IViewAllModelSorter
    {
        IEnumerable<ViewAllModel> Sort(List<ViewAllModel> models, string? propertyName, string order);
    }
}