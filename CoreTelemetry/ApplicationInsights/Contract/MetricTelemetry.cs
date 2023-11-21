using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    public class MetricTelemetry : Envelope<MetricData>
    {
    }
}
