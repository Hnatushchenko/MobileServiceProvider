using MobileServiceProvider.Enums;

namespace MobileServiceProvider.Models
{
    public class ResultViewModel
    {
        public ResultType Type { get; set; } = ResultType.Unknown;
        public string Title { get; set; } = "";
        public string Details { get; set; } = "";
    }
}
