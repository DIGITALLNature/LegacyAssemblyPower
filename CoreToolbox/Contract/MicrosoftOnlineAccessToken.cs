using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract
{
    [DataContract]
    public class MicrosoftOnlineAccessToken
    {
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "not_before")]
        public int NotBefore { get; set; }

        [DataMember(Name = "expires_on")]
        public int ExpiresOn { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }
}
