using System.Collections;
using System.Data;
using System.Reflection;


public static class IEnumerableExtensions
{

    /// <summary>

    /// 將 IEnumerable 轉換至 DataTable

    /// </summary>

    /// <param name="list">IEnumerable</param>

    /// <returns>DataTable</returns>

    public static DataTable ToDataTable(this IEnumerable list)
    {

        DataTable dt = new DataTable();
        bool schemaIsBuild = false;
        PropertyInfo[] props = null;
        foreach (object item in list)
        {
            if (!schemaIsBuild)
            {
                props = item.GetType().GetProperties();
                foreach (var pi in props)
                {
                    dt.Columns.Add(new DataColumn(pi.Name, pi.PropertyType));
                }
                schemaIsBuild = true;
            }
            var row = dt.NewRow();
            foreach (var pi in props)
            {
                row[pi.Name] = pi.GetValue(item, null);
            }
            dt.Rows.Add(row);
        }
        dt.AcceptChanges();
        return dt;
    }
}
