using System.ComponentModel;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("Defines the level of severity for the event.")]
    public enum SeverityLevel
    {
        [EnumMember]
        Verbose = 0,
        [EnumMember]
        Information = 1,
        [EnumMember]
        Warning = 2,
        [EnumMember]
        Error = 3,
        [EnumMember]
        Critical = 4
    }
}
