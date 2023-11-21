using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// 
        /// </summary>
        int MillisecondsOfDay(DateTime dateTime = default(DateTime));
    }
}
