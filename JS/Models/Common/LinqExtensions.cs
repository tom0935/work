using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

class LinqExtensions
{
    //REF: http://3.ly/y8rA
    //註: .NET 3.5有個IEnumerable.CopyToDataTable<T>()，但T只接受DataRow()
    
    public  DataTable LinqQueryToDataTable<T>(IEnumerable<T> query)
    {
        DataTable tbl = new DataTable();
        PropertyInfo[] props = null;

        foreach (T item in query)
        {
            if (props == null) //尚未初始化
            {
                Type t = item.GetType();
                props = t.GetProperties();
                foreach (PropertyInfo pi in props)
                {
                    Type colType = pi.PropertyType;
                    //針對Nullable<>特別處理
                    if (colType.IsGenericType
                        && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        colType = colType.GetGenericArguments()[0];
                    //建立欄位
                    tbl.Columns.Add(pi.Name, colType);
                }
            }
            DataRow row = tbl.NewRow();
            foreach (PropertyInfo pi in props)
                row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
            tbl.Rows.Add(row);
            
        }
        return tbl;
    }
    
    /*
    public DataTable LinqQueryToDataTable<T>(IEnumerable<T> varlist)
    {
        DataTable dtReturn = new DataTable();
        // column names
        PropertyInfo[] oProps = null;
        // 檢查
        if (varlist == null) return dtReturn;
        foreach (T rec in varlist)
        {
            // Use reflection to get property names, to create table, Only first time, others will follow
            if (oProps == null)
            {
                oProps = ((Type)rec.GetType()).GetProperties();
                foreach (PropertyInfo pi in oProps)
                {
                    Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                }
            }
            DataRow dr = dtReturn.NewRow();
            foreach (PropertyInfo pi in oProps)
            {
                dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
            }
            dtReturn.Rows.Add(dr);
        }
        return (dtReturn);
    }*/

}