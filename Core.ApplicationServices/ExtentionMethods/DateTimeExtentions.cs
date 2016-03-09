using System;
using System.Globalization;

namespace Core.ApplicationServices.ExtentionMethods
{
    public static class DateTimeExtentions
    {
        public static DateTime RoundToMonth(this DateTime date)
        {
            return DateTime.SpecifyKind(new DateTime(date.Year, date.Month, 1), DateTimeKind.Utc); 
        }
        public static DateTime RoundToYear(this DateTime date)
        {
            return DateTime.SpecifyKind(new DateTime(date.Year, 1, 1), DateTimeKind.Utc);
        }
    }
}
