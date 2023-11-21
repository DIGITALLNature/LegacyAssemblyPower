using System;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class UserExtension
    {
        /// <summary>
        /// Retrieves User Culture according his UI language
        /// </summary>
        /// <param name="executor" cref="Executor">extension cref</param>
        /// <param name="userId">ID of Systemuser</param>
        /// <param name="defaultCulture">Culture if User Culture is indeterminated</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static CultureInfo UserCulture(this Executor executor, Guid userId, CultureInfo defaultCulture = null)
        {
            var cacheKey = $"UserExtension-{userId:N}";

            if (executor.CacheService.TryGet(cacheKey, out object value))
            {
                return (CultureInfo)value;
            }

            using (var serviceContext = executor.DataContext(executor.ElevatedOrganizationService))
            {
                var result = (from us in serviceContext.UserSettingsSet
                              where us.SystemUserId == userId
                              select us).SingleOrDefault();

                if (result != null && result.UILanguageId > 0)
                {
                    var cultureInfo = CultureInfo.GetCultureInfo((int)result.UILanguageId);

                    executor.CacheService.SetSliding(cacheKey, cultureInfo, 300);
                    return cultureInfo;
                }
            }

            if (defaultCulture != null)
            {
                return defaultCulture;
            }
            throw new InvalidPluginExecutionException("User does not have UI language");
        }
    }
}