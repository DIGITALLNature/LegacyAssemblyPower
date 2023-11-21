using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using D365.Extension.Core.Telemetry.Contract;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class TelemetryService : ITelemetryService
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

        public TelemetryService(Executor executor)
        {
            _executor = executor;
        }

        public void Exception(Exception exception)
        {
            if (!"true".Equals(_executor.ConfigService.GetConfig(_executor.TelemetryServiceEnabled, defaultValue: "false"))) return;
            Track(_executor.SerializerService.JsonSerialize<ExceptionTelemetry>(GetExceptionTelemetry(exception)));
        }

        public void Metric(double duration, string name = null)
        {
            if (!"true".Equals(_executor.ConfigService.GetConfig(_executor.TelemetryServiceEnabled, defaultValue: "false"))) return;
            if (name == null)
            {
                name = _executor.ProcessName;
            }
            Track(_executor.SerializerService.JsonSerialize<MetricTelemetry>(GetMetricTelemetry(duration, name)));
        }

        public void Event(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (!"true".Equals(_executor.ConfigService.GetConfig(_executor.TelemetryServiceEnabled, defaultValue: "false"))) return;
            Track(_executor.SerializerService.JsonSerialize<EventTelemetry>(GetEventTelemetry(name)));
        }

        private void Track(string json)
        {
            if ("true".Equals(_executor.ConfigService.GetConfig(_executor.TelemetryServiceDebug, defaultValue: "false")))
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

        private MetricTelemetry GetMetricTelemetry(double duration, string name)
        {
            return new MetricTelemetry
            {
                Name = typeof(MetricData).Name,
                InstrumentationKey = _executor.ConfigService.GetConfig(_executor.TelemetryServiceInstrumentationKey),
                Data = new Data<MetricData>
                {
                    BaseType = typeof(MetricData).Name,
                    BaseData = new MetricData
                    {
                        Metrics = new List<DataPoint>
                        {
                            new DataPoint
                            {
                                Name = name,
                                Kind = DataPointType.Measurement,
                                Value = duration
                            }
                        },
                        Properties = new Dictionary<string, string> { { "correlationId", _executor.CorrelationId.ToString("D") }, { "system", "d365.extension" }, { "process", _executor.ProcessName }, { "caller", $"{_executor.CallerId:D}" }, { "businessunit", $"{_executor.BusinessUnitId:D}" }, { "entityname", _executor.Delegate.ExecutionContext.PrimaryEntityName }, { "entityid", $"{_executor.Delegate.ExecutionContext.PrimaryEntityId:D}" } },
                    }
                }
            };
        }

        private ExceptionTelemetry GetExceptionTelemetry(Exception exception)
        {
            return new ExceptionTelemetry
            {
                Name = typeof(ExceptionData).Name,
                InstrumentationKey = _executor.ConfigService.GetConfig(_executor.TelemetryServiceInstrumentationKey),
                Data = new Data<ExceptionData>
                {
                    BaseType = typeof(ExceptionData).Name,
                    BaseData = new ExceptionData
                    {
                        SeverityLevel = SeverityLevel.Error,
                        Exceptions = new List<ExceptionDetails>
                        {
                            new ExceptionDetails
                            {
                                HasFullStack = true,
                                Id = 1,
                                Message = exception.Message,
                                Stack = exception.StackTrace,
                                TypeName = $"CRM.{exception.GetType().Name}"
                            }
                        },
                        Properties = new Dictionary<string, string> { { "correlationId", _executor.CorrelationId.ToString("D") }, { "system", "d365.extension" }, { "process", _executor.ProcessName }, { "caller", $"{_executor.CallerId:D}" }, { "businessunit", $"{_executor.BusinessUnitId:D}" }, { "entityname", _executor.Delegate.ExecutionContext.PrimaryEntityName }, { "entityid", $"{_executor.Delegate.ExecutionContext.PrimaryEntityId:D}" } },
                        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                        Measurements = { }
                    }
                }
            };
        }

        private EventTelemetry GetEventTelemetry(string name)
        {
            return new EventTelemetry
            {
                Name = typeof(EventData).Name,
                InstrumentationKey = _executor.ConfigService.GetConfig(_executor.TelemetryServiceInstrumentationKey),
                Data = new Data<EventData>
                {
                    BaseType = typeof(EventData).Name,
                    BaseData = new EventData
                    {
                        Name = name,
                        Properties = new Dictionary<string, string> { { "correlationId", _executor.CorrelationId.ToString("D") }, { "system", "d365.extension" }, { "process", _executor.ProcessName }, { "caller", $"{_executor.CallerId:D}" }, { "businessunit", $"{_executor.BusinessUnitId:D}" }, { "entityname", _executor.Delegate.ExecutionContext.PrimaryEntityName }, { "entityid", $"{_executor.Delegate.ExecutionContext.PrimaryEntityId:D}" } },
                        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                        Measurements = { }
                    }
                }
            };
        }

    }
}
