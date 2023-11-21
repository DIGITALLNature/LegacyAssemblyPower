using System.ComponentModel;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Logging.Contract
{
    [DataContract]
    public class TraceTelemetry : Envelope<TraceData>
    {
    }
}
