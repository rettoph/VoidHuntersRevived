using Guppy;

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    public class Logger : Service
    {
        #region Events
        public delegate void OnLogDelegate(String message, LogLevel level);
        public event OnLogDelegate OnLog;
        public LogLevel LogLevel { get; set; }
        #endregion

        #region Public Methods
        public void Log(String message, LogLevel level)
        {
            if(level >= this.LogLevel)
                this.OnLog?.Invoke(message, level);
        }

        public void Log(Func<String> message, LogLevel level)
        {
            if (level >= this.LogLevel)
                this.OnLog?.Invoke(message(), level);
        }

        public void LogTrace(String message)
            => this.Log(message, LogLevel.Trace);

        public void LogDebug(String message)
            => this.Log(message, LogLevel.Debug);

        public void LogInformation(String message)
            => this.Log(message, LogLevel.Information);

        public void LogWarning(String message)
            => this.Log(message, LogLevel.Warning);

        public void LogError(String message)
            => this.Log(message, LogLevel.Error);

        public void LogCritical(String message)
            => this.Log(message, LogLevel.Critical);

        public void LogTrace(Func<String> message)
            => this.Log(message, LogLevel.Trace);

        public void LogDebug(Func<String> message)
            => this.Log(message, LogLevel.Debug);

        public void LogInformation(Func<String> message)
            => this.Log(message, LogLevel.Information);

        public void LogWarning(Func<String> message)
            => this.Log(message, LogLevel.Warning);

        public void LogError(Func<String> message)
            => this.Log(message, LogLevel.Error);

        public void LogCritical(Func<String> message)
            => this.Log(message, LogLevel.Critical);
        #endregion
    }

    /// <summary>
    /// Defines logging severity levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logs that contain the most detailed messages. These messages may contain sensitive application data.
        /// These messages are disabled by default and should never be enabled in a production environment.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Logs that are used for interactive investigation during development.  These logs should primarily contain
        /// information useful for debugging and have no long-term value.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Logs that track the general flow of the application. These logs should have long-term value.
        /// </summary>
        Information = 2,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the
        /// application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a
        /// failure in the current activity, not an application-wide failure.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires
        /// immediate attention.
        /// </summary>
        Critical = 5,

        /// <summary>
        /// Not used for writing log messages. Specifies that a logging category should not write any messages.
        /// </summary>
        None = 6,
    }
}
