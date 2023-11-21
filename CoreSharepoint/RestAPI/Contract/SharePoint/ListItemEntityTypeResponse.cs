using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class ListItemEntityTypeResponse : ISharepointPayload
    {
        [DataMember(Name = "ListItemEntityTypeFullName")]
        public string ListItemEntityTypeFullName { get; set; }
    }
}
