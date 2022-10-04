namespace MobileServiceProvider.Services
{
    public class RandomDateGenerator : IRandomDateGenerator
    {
        private readonly Random _random = new Random();

        public DateTimeOffset GenerateFrom(DateTimeOffset start)
        {
            long range = (long)(DateTimeOffset.UtcNow - start).TotalSeconds;
            var result = start.AddSeconds(_random.NextInt64(range));
            return result;
        }
    }
}
