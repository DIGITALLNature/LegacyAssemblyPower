using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Logging.Contract
{
    [DataContract]
    [Description("Data struct to contain both B and C sections.")]
    public class Data<T> : Base
    {
        [DataMember(Name = "baseData", IsRequired = true)]
        [Required]
        [Description("Container for data item (B section).")]
        public T BaseData;
    }
}
