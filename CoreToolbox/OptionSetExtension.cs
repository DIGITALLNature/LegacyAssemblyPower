using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public static class OptionSetExtension
    {
        /// <summary>
        /// Returns Value of OptionSet in select language or UserLanguage of Repository
        /// </summary>
        /// <param name="executor" cref="Executor">extension cref</param>
        /// <param name="optionSetName">OptionSet</param>
        /// <param name="optionSetLabel">Label of OptionSet</param>
        /// <param name="lcid">Language - leave empty for UserLanguage</param>
        /// <returns>Label</returns>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public static int? GetOptionSetValue(this Executor executor, string optionSetName, string optionSetLabel, int lcid = 1033)
        {
            var label = optionSetLabel.Trim();

            var cacheKey = $"OptionSetExtension-{optionSetName}-{label}-{lcid}";
            if (executor.CacheService.TryGet(cacheKey, out object value))
            {
                return (int?)value;
            }

            // Retrieve OptionSet
            var retrieveOptionSetRequest = new RetrieveOptionSetRequest
            {
                Name = optionSetName
            };
            var retrieveOptionSetResponse = (RetrieveOptionSetResponse)executor.ElevatedOrganizationService.Execute(retrieveOptionSetRequest);
            var retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

            //TODO: check lcid, currently ignored

            // Read MetaData
            var optionList = retrievedOptionSetMetadata.Options.ToArray();
            foreach (var optionMetadata in optionList)
            {
                if (optionMetadata.Label.UserLocalizedLabel.Label.Trim() != label) continue;
                executor.CacheService.SetAbsolute(cacheKey, optionMetadata.Value, 121);
                return optionMetadata.Value;
            }
            throw new InvalidPluginExecutionException($"OptionSet {label} not found in {optionSetName}!");
        }

        /// <summary>
        /// Returns Label of OptionSet in select language or UserLanguage of Repository
        /// </summary>
        /// <param name="executor" cref="Executor">extension cref</param>
        /// <param name="optionSetName">OptionSet</param>
        /// <param name="optionSetValue">Value of OptionSet</param>
        /// <param name="lcid">Language - leave empty for UserLanguage</param>
        /// <returns>Label</returns>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public static string GetOptionSetLabel(this Executor executor, string optionSetName, int optionSetValue, int lcid = 1033)
        {
            var cacheKey = $"OptionSetExtension-{optionSetName}-{optionSetValue}-{lcid}";
            if (executor.CacheService.TryGet(cacheKey, out object label))
            {
                return (string)label;
            }

            // Retrieve OptionSet
            var retrieveOptionSetRequest = new RetrieveOptionSetRequest
            {
                Name = optionSetName
            };
            var retrieveOptionSetResponse = (RetrieveOptionSetResponse)executor.ElevatedOrganizationService.Execute(retrieveOptionSetRequest);
            var retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

            //TODO: check lcid, currently ignored

            // Read MetaData
            var optionList = retrievedOptionSetMetadata.Options.ToArray();
            foreach (var optionMetadata in optionList)
            {
                if (optionMetadata.Value != optionSetValue) continue;
                executor.CacheService.SetAbsolute(cacheKey, optionMetadata.Label.UserLocalizedLabel.Label.Trim(), 121);
                return optionMetadata.Label.UserLocalizedLabel.Label.Trim();
            }
            throw new InvalidPluginExecutionException($"OptionSet {optionSetValue} not found in {optionSetName}!");
        }
    }
}
