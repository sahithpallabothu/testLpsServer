using System;

namespace Arista_LPS_WebApp.Helpers
{
    public static class ConvertToCustomDate
    {
        public static string ConvertToCustomDateFormat(this string dateval)
        {
            string convertedDate = dateval;
            if (!string.IsNullOrEmpty(dateval))
            { 
                DateTime customDate = Convert.ToDateTime(dateval);
                convertedDate = string.Format("{0}/{1}/{2}",
                                              customDate.Month.ToString().Length > 1 ? customDate.Month.ToString() : String.Format("{0:00}", customDate.Month),
                                              customDate.Day.ToString().Length > 1 ? customDate.Day.ToString() : String.Format("{0:00}", customDate.Day),
                                              customDate.Year.ToString().Substring(2, 2));
            }
            return convertedDate;
        }
    }
}
