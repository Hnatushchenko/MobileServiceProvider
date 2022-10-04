namespace MobileServiceProvider.Services
{
    public interface IRandomDateGenerator
    {
        DateTimeOffset GenerateFrom(DateTimeOffset start);
    }
}