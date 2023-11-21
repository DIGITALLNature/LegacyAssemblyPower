using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("An instance of Exception represents a handled or unhandled exception that occurred during execution of the monitored application.")]
    public class ExceptionData
    {
        [DataMember(Name = "ver", IsRequired = true)]
        [Required]
        [Description("Schema version")]
        public int SchemaVersion = 2;

        [DataMember(Name = "exceptions", IsRequired = true)]
        [Required]
        [Description("Exception chain - list of inner exceptions.")]
        public List<ExceptionDetails> Exceptions;

        [DataMember(Name = "severityLevel", IsRequired = false)]
        [Description("Severity level. Mostly used to indicate exception severity level when it is reported by logging library.")]
        public SeverityLevel SeverityLevel;

        [DataMember(Name = "problemId", IsRequired = false, EmitDefaultValue = false)]
        [Description("Identifier of where the exception was thrown in code. Used for exceptions grouping. Typically a combination of exception type and a function from the call stack.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string ProblemId;

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
