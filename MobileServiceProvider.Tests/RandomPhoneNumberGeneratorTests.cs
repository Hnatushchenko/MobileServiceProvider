using Microsoft.AspNetCore.Authorization;
using MobileServiceProvider.Services;

namespace MobileServiceProvider.Tests
{
    public class RandomPhoneNumberGeneratorTests
    {
        private readonly RandomPhoneNumberGenerator _sut;

        public RandomPhoneNumberGeneratorTests()
        {
            _sut = new RandomPhoneNumberGenerator();
        }

        [Fact]
        public void GenerateUkrainianPhoneNumberShouldGenerateAppropriateNumber()
        {
            // Act
            var phoneNumber = _sut.GenerateUkrainianPhoneNumber();
            // Assert
            Assert.StartsWith("+380", phoneNumber);
            Assert.Equal(13, phoneNumber.Length);
            Assert.All(phoneNumber[1..], ch => char.IsDigit(ch));
        }
    }
}