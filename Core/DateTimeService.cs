using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class DateTimeService : IDateTimeService
    {
        public int MillisecondsOfDay(DateTime dateTime = default(DateTime))
        {
            if (dateTime == default(DateTime))
            {
                dateTime = DateTime.UtcNow;
            }
            return Convert.ToInt32(dateTime.TimeOfDay.TotalMilliseconds);
        }
    }
}
