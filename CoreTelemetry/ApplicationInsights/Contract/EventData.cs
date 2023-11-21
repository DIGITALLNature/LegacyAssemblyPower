using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("Instances of Event represent structured event records that can be grouped and searched by their properties. Event data item also creates a metric of event count by name.")]
    public class EventData
    {
        [DataMember(Name = "ver", IsRequired = true)]
        [Required]
        [Description("Schema version")]
        public int SchemaVersion = 2;

        [DataMember(Name = "name", IsRequired = true)]
        [Required]
        [Description("Event name. Keep it low cardinality to allow proper grouping and useful metrics.")]
        [StringLength(512, ErrorMessage = "String length should be max. 512")]
        public string Name;

        [DataMember(Name = "properties", IsRequired = false, EmitDefaultValue = false)]
        [Description("Collection of custom properties.")]
        //[MaxKeyLength("150")]
        //[MaxValueLength("8192")]
        public Dictionary<string, string> Properties;

        [DataMember(Name = "measurements", IsRequired = false, EmitDefaultValue = false)]
        [Description("Collection of custom measurements.")]
        //[MaxKeyLength("150")]
        public Dictionary<string, double> Measurements;
    }
}