using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// Logging Service interface which should be implemented in an appropriate manor.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Debug a message!
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="e">Exception</param>
        void Trace(string message, Exception e = null);

        /// <summary>
        /// Debug a message!
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="e">Exception</param>
        void Debug(string message, Exception e = null);

        /// <summary>
        /// Info a message!
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="e">Exception</param>
        void Info(string message, Exception e = null);

        /// <summary>
        /// Warn a message!
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="e">Exception</param>
        void Warn(string message, Exception e = null);

        /// <summary>
        /// Error a message!
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="e">Exception</param>
        void Error(string message, Exception e = null);

        /// <summary>
        /// Error a message!
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="e">Exception</param>
        void Fatal(string message, Exception e = null);
    }
}
