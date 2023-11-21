using Microsoft.Crm.Sdk.Messages;
using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class TimeZoneExtension
    {
        /// <summary>
        /// Converts the given localTime to UTC time for the user identified by the userId
        /// </summary>
        /// <param name="executor" cref="Executor">extension cref</param>
        /// <param name="userId"></param>
        /// <param name="localTime"></param>
        /// <param name="defaultTimeZoneCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DateTime UtcTimeFromLocalTime(this Executor executor, Guid userId, DateTime localTime, int? defaultTimeZoneCode = null)
        {
            using (var serviceContext = executor.DataContext(executor.ElevatedOrganizationService))
            {
                var result = (from us in serviceContext.UserSettingsSet
                              where us.SystemUserId == userId
                              select us.TimeZoneCode).SingleOrDefault();

                if (result != null)
                {
                    var timezoneCode = (int)result;

                    var utcTimeRequest = new UtcTimeFromLocalTimeRequest { TimeZoneCode = timezoneCode, LocalTime = localTime };
                    var utcTimeResponse = (UtcTimeFromLocalTimeResponse)executor.ElevatedOrganizationService.Execute(utcTimeRequest);

                    return utcTimeResponse.UtcTime;
                }

                if (defaultTimeZoneCode.HasValue)
                {
                    var utcTimeRequest = new UtcTimeFromLocalTimeRequest { TimeZoneCode = defaultTimeZoneCode.Value, LocalTime = localTime };
                    var utcTimeResponse = (UtcTimeFromLocalTimeResponse)executor.ElevatedOrganizationService.Execute(utcTimeRequest);

                    return utcTimeResponse.UtcTime;
                }

                throw new InvalidPluginExecutionException("User does not have Timezone Code");
            }
        }

        /// <summary>
        /// Converts the given utcTime to local time for the user identified by the userId
        /// </summary>
        /// <param name="executor" cref="Executor">extension cref</param>
        /// <param name="userId"></param>
        /// <param name="utcTime"></param>
        /// <param name="defaultTimeZoneCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DateTime LocalTimeFromUtcTime(this Executor executor, Guid userId, DateTime utcTime, int? defaultTimeZoneCode = null)
        {
            using (var serviceContext = executor.DataContext(executor.ElevatedOrganizationService))
            {
                var result = (from us in serviceContext.UserSettingsSet
                              where us.SystemUserId == userId
                              select us.TimeZoneCode).SingleOrDefault();

                if (result != null)
                {
                    var timezoneCode = (int)result;

                    var localTimeRequest = new LocalTimeFromUtcTimeRequest { TimeZoneCode = timezoneCode, UtcTime = utcTime };
                    var localTimeResponse = (LocalTimeFromUtcTimeResponse)executor.ElevatedOrganizationService.Execute(localTimeRequest);

                    return localTimeResponse.LocalTime;
                }

                if (defaultTimeZoneCode.HasValue)
                {
                    var localTimeRequest = new LocalTimeFromUtcTimeRequest { TimeZoneCode = defaultTimeZoneCode.Value, UtcTime = utcTime };
                    var localTimeResponse = (LocalTimeFromUtcTimeResponse)executor.ElevatedOrganizationService.Execute(localTimeRequest);

                    return localTimeResponse.LocalTime;
                }

                throw new InvalidPluginExecutionException("User does not have Timezone Code");
            }
        }
    }
}
