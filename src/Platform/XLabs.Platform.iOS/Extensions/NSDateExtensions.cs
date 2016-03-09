using Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace XLabs.Platform.Extensions
{
    public static class NSDateExtensions
    {
        public static DateTime NSDateToDateTime(this NSDate date)
        {
            DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0);
            DateTime currentDate = reference.AddSeconds(date.SecondsSinceReferenceDate);
            DateTime localDate = currentDate.ToLocalTime();
            return localDate;
        }

        public static NSDate DateTimeToNSDate(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);
            return (NSDate)date;
        }
    }
}
