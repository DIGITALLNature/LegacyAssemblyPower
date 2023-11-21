﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Logging.Contract
{
    [DataContract]
    public class TraceData
    {
        [DataMember(Name = "ver", IsRequired = true)]
        [Required]
        [Description("Schema version")]
        public int SchemaVersion = 2;

        [DataMember(Name = "message", IsRequired = true)]
        [Required]
        [Description("Trace message.")]
        public string Message;

        [DataMember(Name = "severityLevel", IsRequired = false)]
        [Description("Severity level. Mostly used to indicate exception severity level when it is reported by logging library.")]
        public SeverityLevel SeverityLevel;

        [DataMember(Name = "properties", IsRequired = false, EmitDefaultValue = false)]
        [Description("Collection of custom properties.")]
        //[MaxKeyLength("150")]
        //[MaxValueLength("8192")]
        public Dictionary<string, string> Properties;
    }
}
