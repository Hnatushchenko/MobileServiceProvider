namespace MobileServiceProvider.Services
{
    public interface IMobilePhoneService
    {
        void Charge(string phoneNumber, string sumAsString);
    }
}