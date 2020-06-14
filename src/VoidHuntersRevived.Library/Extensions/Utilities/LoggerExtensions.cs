using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Extensions.Utilities
{
    public static class LoggerExtensions
    {
        #region Console Logging
        private static Dictionary<LogLevel, ConsoleColor> ConsoleColors;

        public static void ConfigureConsoleLogging(this Logger logger)
        {
            LoggerExtensions.ConsoleColors = new Dictionary<LogLevel, ConsoleColor>();
            LoggerExtensions.ConsoleColors[LogLevel.Trace] = ConsoleColor.Magenta;
            LoggerExtensions.ConsoleColors[LogLevel.Debug] = ConsoleColor.Cyan;
            LoggerExtensions.ConsoleColors[LogLevel.Information] = ConsoleColor.White;
            LoggerExtensions.ConsoleColors[LogLevel.Warning] = ConsoleColor.Yellow;
            LoggerExtensions.ConsoleColors[LogLevel.Error] = ConsoleColor.Red;

            logger.OnLog += LoggerExtensions.HandleOnWriteLine;
            logger.OnDisposed += LoggerExtensions.HandleDisposed;
        }

        private static void HandleOnWriteLine(string message, LogLevel level)
        {
            Console.ForegroundColor = LoggerExtensions.ConsoleColors[level];
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
        }

        private static void HandleDisposed(IService sender)
        {
            (sender as Logger).OnLog -= LoggerExtensions.HandleOnWriteLine;
        }
        #endregion
    }
}
