using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;
public class ContextExtensions
{
    #region -- GetTableName --
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context.</param>
    /// <param name="dbType">Type of the db.</param>
    /// <returns></returns>
    public static string GetTableName<T>(ObjectContext context, DataBaseType dbType = DataBaseType.MS_SQL_SERVER) where T : class
    {
        string sql = context.CreateObjectSet<T>().ToTraceString();
        string matchWords = string.Empty;
        switch (dbType)
        {
            case DataBaseType.MS_SQL_SERVER:
                matchWords = "FROM (?<table>.*) AS";
                break;
            case DataBaseType.Oracle:
                matchWords = "FROM \"(?<schema>.*)\".\"(?<table>.*)\"\\s";
                break;
        }
        Regex regex = new Regex(matchWords);
        Match match = regex.Match(sql);
        string table = match.Groups["table"].Value;
        return table;
    }
    #endregion
    public enum DataBaseType
    {
        MS_SQL_SERVER = 0,
        Oracle = 1
    }
}