using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class ErrorResponse : ISharepointPayload
    {
        [DataMember(Name = "odata.error")]
        public ErrorDetails ODataError { get; set; }
    }

    [DataContract]
    public class ErrorDetails
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "message")]
        public ErrorMessage Message { get; set; }
    }

    [DataContract]
    public class ErrorMessage
    {
        [DataMember(Name = "lang")]
        public string Lang { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
