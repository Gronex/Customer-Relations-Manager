using System;

namespace Core.ApplicationServices.ExtentionMethods
{
    public static class DateTimeExtentions
    {
        public static DateTime RoundToMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1); 
        }
        public static DateTime RoundToYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }
    }
}
