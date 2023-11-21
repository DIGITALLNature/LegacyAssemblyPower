using System;
using Microsoft.Xrm.Sdk.PluginTelemetry;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class LoggingService : ILoggingService
    {
        private readonly Executor _executor;

        public LoggingService(Executor executor)
        {
            _executor = executor;

            _executor.Delegate.PluginTelemetry.AddCustomProperty("correlationId", executor.CorrelationId.ToString("D"));
            _executor.Delegate.PluginTelemetry.AddCustomProperty("system", "d365.extension");
            _executor.Delegate.PluginTelemetry.AddCustomProperty("process", executor.ProcessName);
            _executor.Delegate.PluginTelemetry.AddCustomProperty("caller", $"{executor.CallerId:D}");
            _executor.Delegate.PluginTelemetry.AddCustomProperty("businessunit", $"{executor.BusinessUnitId:D}");
            _executor.Delegate.PluginTelemetry.AddCustomProperty("entityname", executor.Delegate.ExecutionContext.PrimaryEntityName);
            _executor.Delegate.PluginTelemetry.AddCustomProperty("entityid", $"{executor.Delegate.ExecutionContext.PrimaryEntityId:D}");
        }

        public void Trace(string message, Exception e = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Trace))
            {
                if (e == null)
                {
                    _executor.Delegate.PluginTelemetry.LogTrace(message);
                }
                else
                {
                    _executor.Delegate.PluginTelemetry.LogTrace(e, message);
                }
            }
        }

        public void Debug(string message, Exception e = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Debug))
            {
                if (e == null)
                {
                    _executor.Delegate.PluginTelemetry.LogDebug(message);
                }
                else
                {
                    _executor.Delegate.PluginTelemetry.LogDebug(e, message);
                }
            }
        }

        public void Info(string message, Exception e = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Information))
            {
                if (e == null)
                {
                    _executor.Delegate.PluginTelemetry.LogInformation(message);
                }
                else
                {
                    _executor.Delegate.PluginTelemetry.LogInformation(e, message);
                }
            }
        }

        public void Warn(string message, Exception e = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Warning))
            {
                if (e == null)
                {
                    _executor.Delegate.PluginTelemetry.LogWarning(message);
                }
                else
                {
                    _executor.Delegate.PluginTelemetry.LogWarning(e, message);
                }
            }
        }

        public void Error(string message, Exception e = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Error))
            {
                if (e == null)
                {
                    _executor.Delegate.PluginTelemetry.LogError(message);
                }
                else
                {
                    _executor.Delegate.PluginTelemetry.LogError(e, message);
                }
            }
        }

        public void Fatal(string message, Exception e = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Critical))
            {
                if (e == null)
                {
                    _executor.Delegate.PluginTelemetry.LogCritical(message);
                }
                else
                {
                    _executor.Delegate.PluginTelemetry.LogCritical(e, message);
                }
            }
        }
    }
}
