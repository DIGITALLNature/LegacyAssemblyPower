using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using D365.Extension.Core.Logging.Contract;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class LoggingService : ILoggingService
    {
        private const int Timeout = 15000;

        //https://docs.microsoft.com/de-de/powerapps/developer/common-data-service/best-practices/business-logic/set-keepalive-false-interacting-external-hosts-plugin
        //by default, Lazy objects are thread-safe.
        private static readonly Lazy<HttpClient> Lazy = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(Timeout)
            };
            client.DefaultRequestHeaders.ConnectionClose = true;
            return client;
        });

        private static HttpClient HttpClient => Lazy.Value;

        private readonly Executor _executor;
        private SeverityLevel? _activeSeverityLevel;

        public LoggingService(Executor executor)
        {
            _executor = executor;
            _activeSeverityLevel = null;
        }

        private void Track(string json)
        {
            if ("true".Equals(_executor.ConfigService.GetConfig(_executor.LoggingServiceDebug, defaultValue: "false")))
            {
                _executor.Delegate.TracingService.Trace($"App Insights JSON: {json}");
                var response = HttpClient.SendAsync(GetRequest(json)).ConfigureAwait(false).GetAwaiter().GetResult();
                _executor.Delegate.TracingService.Trace($"HTTP Status: {response.StatusCode}");
                _executor.Delegate.TracingService.Trace($"HTTP Content: {response.Content?.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
            else
            {
                HttpClient.SendAsync(GetRequest(json));
            }
        }

        private static HttpRequestMessage GetRequest(string json)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://dc.services.visualstudio.com/v2/track")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Properties["RequestTimeout"] = TimeSpan.FromMilliseconds(Timeout);
            return request;
        }

        private TraceTelemetry GetTraceTelemetry(SeverityLevel severityLevel, string message)
        {
            return new TraceTelemetry
            {
                Name = "MessageData",
                InstrumentationKey = _executor.ConfigService.GetConfig(_executor.LoggingServiceInstrumentationKey),
                Data = new Data<TraceData>
                {
                    BaseType = "MessageData",
                    BaseData = new TraceData
                    {
                        Message = message,
                        SeverityLevel = severityLevel,
                        Properties = new Dictionary<string, string> { { "correlationId", _executor.CorrelationId.ToString("D") }, { "system", "d365.extension" }, { "process", _executor.ProcessName }, { "caller", $"{_executor.CallerId:D}" }, { "businessunit", $"{_executor.BusinessUnitId:D}" }, { "entityname", _executor.Delegate.ExecutionContext.PrimaryEntityName }, { "entityid", $"{_executor.Delegate.ExecutionContext.PrimaryEntityId:D}" } },
                    }
                }
            };
        }

        private void Log(SeverityLevel severityLevel, string message, Exception e = null)
        {
            if (_activeSeverityLevel == null)
            {
                _activeSeverityLevel = Enum.TryParse(_executor.ConfigService.GetConfig(_executor.LoggingServiceLogLevel, defaultValue: $"{SeverityLevel.Off}"), out SeverityLevel activeSeverityLevel) ? activeSeverityLevel : SeverityLevel.Off;
            }
            if ((int)severityLevel < (int)_activeSeverityLevel) return;
            var trace = string.Format("{0}{1}", message, e != null ? $"; {e.RootException().GetType().Name}:{e.RootMessage()}" : "");
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}", $"{severityLevel}", DateTime.UtcNow, trace);
            Track(_executor.SerializerService.JsonSerialize<TraceTelemetry>(GetTraceTelemetry(severityLevel, trace)));
        }

        public void Trace(string message, Exception e = null)
        {
            _executor.Delegate.TracingService.Trace("{0} {1:s}; {2}{3}", "Trace", DateTime.UtcNow, message, e != null ? $"; {e.RootException().GetType().Name}:{ e.RootMessage() }" : "");
        }

        public void Debug(string message, Exception e = null)
        {
            Log(SeverityLevel.Verbose, message, e);
        }

        public void Info(string message, Exception e = null)
        {
            Log(SeverityLevel.Information, message, e);
        }

        public void Warn(string message, Exception e = null)
        {
            Log(SeverityLevel.Warning, message, e);
        }

        public void Error(string message, Exception e = null)
        {
            Log(SeverityLevel.Error, message, e);
        }

        public void Fatal(string message, Exception e = null)
        {
            Log(SeverityLevel.Critical, message, e);
        }
    }
}
