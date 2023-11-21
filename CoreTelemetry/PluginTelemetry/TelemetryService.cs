using System;
using Microsoft.Xrm.Sdk.PluginTelemetry;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class TelemetryService : ITelemetryService
    {
        private readonly Executor _executor;

        public TelemetryService(Executor executor)
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

        public void Exception(Exception exception)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Critical))
            {
                _executor.Delegate.PluginTelemetry.LogCritical(exception, exception.Message);
            }
        }

        public void Metric(double duration, string name = null)
        {
            if (_executor.Delegate.PluginTelemetry.IsEnabled(LogLevel.Critical))
            {
                if (name == null)
                {
                    name = _executor.ProcessName;
                }
                _executor.Delegate.PluginTelemetry.LogMetric(name, Convert.ToInt64(duration));
            }
        }

        public void Event(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            _executor.Delegate.TracingService.Trace($"Microsoft.Xrm.Sdk.PluginTelemetry does not implement application insights events: {name}");
        }
    }
}
