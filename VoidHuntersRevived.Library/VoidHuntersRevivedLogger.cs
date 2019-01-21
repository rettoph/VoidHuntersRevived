using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersRevivedLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.ForegroundColor = this.GetColor(logLevel);

            Console.WriteLine($"{logLevel}: {state}");
        }

        private ConsoleColor GetColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return ConsoleColor.Magenta;
                case LogLevel.Debug:
                    return ConsoleColor.Cyan;
                case LogLevel.Information:
                    return ConsoleColor.White;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Critical:
                    return ConsoleColor.DarkRed;
                case LogLevel.None:
                    return ConsoleColor.Gray;
                default:
                    return ConsoleColor.DarkGray;
            }
        }
    }
}