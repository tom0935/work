using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jasper.util
{
    public class DateTimeUtil
    {
        public static DateTime getDateTime(String dateStr)
        {            
                return Convert.ToDateTime(dateStr);
            
        }
/*
        public static String getDateTime(String dateStr)
        {
            return Convert.ToDateTime(dateStr);
        }
 */
    }
}