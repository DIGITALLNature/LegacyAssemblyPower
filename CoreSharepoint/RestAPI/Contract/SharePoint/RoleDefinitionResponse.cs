using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class RoleDefinitionResponse : ISharepointPayload
    {
        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Order")]
        public int Order { get; set; }

        [DataMember(Name = "RoleTypeKind")]
        public int RoleTypeKind { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "Hidden")]
        public string Hidden { get; set; }
    }
}
