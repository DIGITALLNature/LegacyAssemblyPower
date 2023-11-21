using D365.Extension.Core;
using Microsoft.Xrm.Sdk.Messages;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
  internal static class AttributeExtension
  {
    internal static string RetrieveAttributeDisplayName(this Executor executor, string entityLogicalName, string attributeLogicalName)
    {
      var request = new RetrieveAttributeRequest
      {
        EntityLogicalName = entityLogicalName,
        LogicalName = attributeLogicalName,
        RetrieveAsIfPublished = true
      };

      var response = (RetrieveAttributeResponse)executor.SecuredOrganizationService.Execute(request);

      return response.AttributeMetadata.DisplayName?.UserLocalizedLabel?.Label ?? attributeLogicalName;
    }
  }
}