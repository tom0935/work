﻿
using System;
public class StringUtils
{
    public static String getString(String str){
        String x;
        if (String.IsNullOrEmpty(str))
        {
            x= "";
        }
        else
        {
            x = str;
        }
        return x;
    }

    public static String getString(Object obj)
    {
        String x="";
        

        if (obj != null)
        {
            if (obj.Equals(DBNull.Value))
            {
                x = "";
            }
            else
            {
                x = System.Convert.ToString(obj);
            }
        }  
        return x;
    }

    public static Decimal getDecimal(Object obj)
    {
        Decimal x = 0;
        

        if (obj != null)
        {
            if (obj.Equals(DBNull.Value))
            {
                x = 0;
            }
            else
            {
                x = System.Convert.ToDecimal(obj);
            }
        }
        return x;
    }


    public static String getDateString(Object obj)
    {
        String x = "";


        if (obj != null)
        {
            if (obj.Equals(DBNull.Value))
            {
                x = "";
            }
            else
            {
                x = System.Convert.ToString(obj);
            }
        }
        return x;
    }



    

}