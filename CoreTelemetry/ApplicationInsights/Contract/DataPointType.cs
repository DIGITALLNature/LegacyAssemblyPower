using System.ComponentModel;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("Type of the metric data measurement.")]
    public enum DataPointType
    {
        [EnumMember]
        Measurement = 0,
        [EnumMember]
        Aggregation = 1
    }
}
