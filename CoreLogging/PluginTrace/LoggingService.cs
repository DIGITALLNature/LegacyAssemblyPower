using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class LoggingService : ILoggingService
    {
        private readonly Executor _executor;

        public LoggingService(Executor executor)
        {
            _executor = executor;
        }

        public void Trace(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Trace", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Debug(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Debug", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Info(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Info", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Warn(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Warn", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Error(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Error", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Fatal(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Fatal", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }
    }
}
