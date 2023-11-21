using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    public class UnknownResponse : ISharepointPayload
    {
        public Dictionary<string, object> Content { get; set; }
    }
}
