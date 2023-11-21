using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("An instance of the Metric item is a list of measurements (single data points) and/or aggregations.")]
    public class MetricData
    {
        [DataMember(Name = "ver", IsRequired = true)]
        [Required]
        [Description("Schema version")]
        public int SchemaVersion = 2;

        [DataMember(Name = "metrics", IsRequired = true)]
        [Required]
        [Description("List of metrics. Only one metric in the list is currently supported by Application Insights storage. If multiple data points were sent only the first one will be used.")]
        public List<DataPoint> Metrics;

        [DataMember(Name = "properties", IsRequired = false, EmitDefaultValue = false)]
        [Description("Collection of custom properties.")]
        //[MaxKeyLength("150")]
        //[MaxValueLength("8192")]
        public Dictionary<string, string> Properties;
    }
}
