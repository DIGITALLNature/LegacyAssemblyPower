using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class BadRequestResponse : ISharepointPayload
    {
        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "error_description")]
        public string ErrorDescription { get; set; }

        [DataMember(Name = "error_codes")]
        public List<int> ErrorCodes { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }

        [DataMember(Name = "trace_id")]
        public string TraceId { get; set; }

        [DataMember(Name = "correlation_id")]
        public string CorrelationId { get; set; }
    }
}
