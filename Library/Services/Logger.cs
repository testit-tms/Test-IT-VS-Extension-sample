namespace TestIT.Linker.Interfaces
{
    public class Logger : ILogger
    {
        private readonly ILogger logger;

        public Logger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Write(string message)
        {
            logger.Write(message);
        }
    }
}
