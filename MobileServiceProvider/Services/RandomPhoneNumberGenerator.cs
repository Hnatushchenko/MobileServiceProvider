using System.Text;

namespace MobileServiceProvider.Services
{
    public class RandomPhoneNumberGenerator : IRandomPhoneNumberGenerator
    {
        private readonly Random _random = new Random();
        public string GenerateUkrainianPhoneNumber()
        {
            StringBuilder phoneNumber = new StringBuilder("+380");
            for (int i = 0; i < 9; i++)
            {
                phoneNumber.Append(_random.Next(10));
            }
            return phoneNumber.ToString();
        }
    }
}
