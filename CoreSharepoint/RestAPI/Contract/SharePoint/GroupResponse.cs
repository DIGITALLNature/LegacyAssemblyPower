using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class GroupResponse : ISharepointPayload
    {
        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "IsHiddenInUI")]
        public bool IsHiddenInUi { get; set; }

        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        [DataMember(Name = "Title")]
        public string Title { get; set; }

        [DataMember(Name = "PrincipalType")]
        public int PrincipalType { get; set; }

        [DataMember(Name = "AllowMembersEditMembership")]
        public bool AllowMembersEditMembership { get; set; }

        [DataMember(Name = "AllowRequestToJoinLeave")]
        public bool AllowRequestToJoinLeave { get; set; }

        [DataMember(Name = "AutoAcceptRequestToJoinLeave")]
        public bool AutoAcceptRequestToJoinLeave { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "OnlyAllowMembersViewMembership")]
        public bool OnlyAllowMembersViewMembership { get; set; }

        [DataMember(Name = "OwnerTitle")]
        public string OwnerTitle { get; set; }
    }
}
