using System.IO;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class BinaryDataResponse : ISharepointPayload
    {
        [DataMember(Name = "Stream")]
        public MemoryStream Stream { get; set; }
    }
}
