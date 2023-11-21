using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("Metric data single measurement.")]
    public class DataPoint
    {
        [DataMember(Name = "ns", IsRequired = false, EmitDefaultValue = false)]
        [Description("Namespace of the metric.")]
        [StringLength(256, ErrorMessage = "String length should be max. 256")]
        public string Namespace;

        [DataMember(Name = "name", IsRequired = true)]
        [Required]
        [Description("Name of the metric.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string Name;

        [DataMember(Name = "kind", IsRequired = true)]
        [Required]
        [Description("Metric type. Single measurement or the aggregated value.")]
        public DataPointType Kind = DataPointType.Measurement;

        [DataMember(Name = "value", IsRequired = true)]
        [Required]
        [Description("Single value for measurement. Sum of individual measurements for the aggregation.")]
        public double Value;

        [DataMember(Name = "count", IsRequired = false, EmitDefaultValue = false)]
        [Description("Metric weight of the aggregated metric. Should not be set for a measurement.")]
        public int? Count;

        [DataMember(Name = "min", IsRequired = false, EmitDefaultValue = false)]
        [Description("Minimum value of the aggregated metric. Should not be set for a measurement.")]
        public double? Minimum;

        [DataMember(Name = "max", IsRequired = false, EmitDefaultValue = false)]
        [Description("Maximum value of the aggregated metric. Should not be set for a measurement.")]
        public double? Maximum;

        [DataMember(Name = "stdDev", IsRequired = false, EmitDefaultValue = false)]
        [Description("Standard deviation of the aggregated metric. Should not be set for a measurement.")]
        public double? StandardDeviation;
    }
}
