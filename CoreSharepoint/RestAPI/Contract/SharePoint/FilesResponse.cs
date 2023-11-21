using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract.SharePoint
{
    [DataContract]
    public class FilesResponse : ISharepointPayload
    {
        [DataMember(Name = "value")]
        public List<File> Files { get; set; }
    }

    [DataContract]
    public class File
    {
        [DataMember(Name = "ETag")]
        public string ETag { get; set; }

        [DataMember(Name = "Length")]
        public string Length { get; set; }

        [DataMember(Name = "MajorVersion")]
        public int MajorVersion { get; set; }

        [DataMember(Name = "MinorVersion")]
        public string MinorVersion { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "ServerRelativeUrl")]
        public string ServerRelativeUrl { get; set; }

        [DataMember(Name = "TimeCreated")]
        public DateTime TimeCreated { get; set; }

        [DataMember(Name = "TimeLastModified")]
        public DateTime TimeLastModified { get; set; }

        [DataMember(Name = "Title")]
        public string Title { get; set; }

        [DataMember(Name = "UniqueId")]
        public string UniqueId { get; set; }
    }
}
