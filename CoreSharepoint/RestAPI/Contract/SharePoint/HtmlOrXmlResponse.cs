using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    public class HtmlOrXmlResponse : ISharepointPayload
    {
        public string HtmlOrXml { get; set; }
    }
}
