using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class FolderIdResponse : ISharepointPayload
    {
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }
}
