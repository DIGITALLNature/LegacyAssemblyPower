using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Xrm.Sdk.Metadata;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Plugin.Test
{
  public static class EntityMetadataLoader
  {
    public static IEnumerable<EntityMetadata> Load(string dir, params string[] logicalNames)
    {
      var directory = new DirectoryInfo(dir);
      if (!directory.Exists) throw new DirectoryNotFoundException($"Metadata directory '{dir}' does not exist!");

      var serializer = new DataContractSerializer(typeof(EntityMetadata));

      var files = directory.GetFiles("*.xml").AsEnumerable();
      if (logicalNames.Any()) files = files.Where(x => logicalNames.Contains(Path.GetFileNameWithoutExtension(x.Name)));

      var metadata = files.Select(x => XmlReader.Create(x.OpenRead())).Select(x => (EntityMetadata)serializer.ReadObject(x, true));

      return metadata;
    }
  }
}