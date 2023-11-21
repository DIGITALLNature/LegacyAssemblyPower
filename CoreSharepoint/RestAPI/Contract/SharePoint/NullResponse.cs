using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class NullResponse : ISharepointPayload
    {
        [DataMember(Name = "odata.null")]
        public bool ODataNull { get; set; }
    }
}
