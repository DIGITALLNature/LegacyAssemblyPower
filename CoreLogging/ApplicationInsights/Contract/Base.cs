using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Logging.Contract
{
    [DataContract]
    [Description("Data struct to contain only C section with custom fields.")]
    public class Base
    {
        [DataMember(Name = "baseType", IsRequired = true)]
        [Required]
        [Description("Name of item (B section) if any. If telemetry data is derived straight from this, this should be null.")]
        public string BaseType;
    }
}
