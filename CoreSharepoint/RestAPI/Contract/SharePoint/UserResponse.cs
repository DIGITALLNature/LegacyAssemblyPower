using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class UserResponse : ISharepointPayload
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

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "IsEmailAuthenticationGuestUser")]
        public bool IsEmailAuthenticationGuestUser { get; set; }

        [DataMember(Name = "IsShareByEmailGuestUser")]
        public bool IsShareByEmailGuestUser { get; set; }

        [DataMember(Name = "IsSiteAdmin")]
        public bool IsSiteAdmin { get; set; }
    }
}
