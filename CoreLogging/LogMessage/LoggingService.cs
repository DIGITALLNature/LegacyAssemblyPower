using System;
using D365.Extension.Core.Contract;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class LoggingService : ILoggingService
    {
        private readonly Executor _executor;
        private LogLevel? _activeLogLevel;

        public LoggingService(Executor executor)
        {
            _executor = executor;
            _activeLogLevel = null;
        }

        private void Log(LogLevel logLevel, string message, Exception e = null)
        {
            var now = DateTime.UtcNow;
            var logEntry = new Ec4uLogMessage
            {
                Ec4uLogProcessName = _executor.ProcessName,
                Ec4uLogLogLevelSet = new OptionSetValue((int)logLevel),
                Ec4uLogMessageLine = string.IsNullOrEmpty(message) ? null : (message.Length > 4000 ? message.Substring(0, 4000) : message),
                Ec4uLogCorrelationid = $"{_executor.CorrelationId:D}",
                Ec4uLogEntityname = _executor.Delegate.ExecutionContext.PrimaryEntityName,
                Ec4uLogEntityid = $"{_executor.Delegate.ExecutionContext.PrimaryEntityId:D}",
                Ec4uLogStackTrace = e == null ? null : (e.StackTrace.Length > 20000 ? e.StackTrace.Substring(0, 20000) : e.StackTrace),
                Ec4uLogExceptionType = e?.RootException().GetType().Name,
                Ec4uLogDateTimeTs = now,
                Ec4uLogMillisecondsNo = _executor.DateTimeService.MillisecondsOfDay(now)
            };
            //TODO: make caller id a mandatory property or even better use delegation to create the entity
            if (logEntry.GetType().GetProperty("Ec4uLogCallerid") != null)
            {
                logEntry["ec4u_log_callerid"] = $"{_executor.CallerId:D}";
            }
            Log(logLevel, logEntry);
        }

        public void Log(LogLevel logLevel, Ec4uLogMessage logMessage)
        {
            if (_activeLogLevel == null)
            {
                _activeLogLevel = Enum.TryParse(_executor.ConfigService.GetConfig("LoggingService.LogLevel", defaultValue: $"{LogLevel.Off}"), out LogLevel activeLogLevel) ? activeLogLevel : LogLevel.Off;
            }
            if ((int)logLevel < (int)_activeLogLevel) return;

            //new trx needed to skip the rollback
            var executeMultipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };
            executeMultipleRequest.Requests.Add(new CreateRequest
            {
                Target = logMessage
            });
            try
            {
                _executor.ElevatedOrganizationService.Execute(executeMultipleRequest);
            }
            catch (Exception ex)
            {
                _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", $"{logLevel}", DateTime.UtcNow, logMessage.Ec4uLogMessageLine, logMessage.Ec4uLogStackTrace != null ? $"; { logMessage.Ec4uLogStackTrace }" : "");
                var cause = ex.RootException();
                _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}; {3}; {4}", "Error", DateTime.UtcNow, cause.GetType().Name, cause.Message, cause.StackTrace);
            }
        }

        public void Trace(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Trace", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Debug(string message, Exception e = null)
        {
            Log(LogLevel.Debug, message, e);
        }

        public void Info(string message, Exception e = null)
        {
            Log(LogLevel.Info, message, e);
        }

        public void Warn(string message, Exception e = null)
        {
            Log(LogLevel.Warn, message, e);
        }

        public void Error(string message, Exception e = null)
        {
            Log(LogLevel.Error, message, e);
        }

        public void Fatal(string message, Exception e = null)
        {
            Log(LogLevel.Fatal, message, e);
        }
    }
}
