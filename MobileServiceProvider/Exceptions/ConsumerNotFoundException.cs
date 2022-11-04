namespace MobileServiceProvider.Exceptions
{
    public class ConsumerNotFoundException : Exception
    {
        public ConsumerNotFoundException()
        {
        }

        public ConsumerNotFoundException(string message)
            : base(message)
        {
        }

        public ConsumerNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
