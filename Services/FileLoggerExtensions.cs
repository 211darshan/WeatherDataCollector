using Microsoft.Extensions.Logging;

namespace Services
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddSimpleFileLogger(this ILoggingBuilder builder, string filePath)
        {
            builder.AddProvider(new FileLoggerProvider(filePath));
            return builder;
        }
    }
}