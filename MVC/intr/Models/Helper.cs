using System.Configuration;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Helper
    {
        //取得某欄位的值[輔助用]
        public static string getFieldValue(string field, string table, string condition = "")
        {
            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
            string val = string.Empty;
            string query = "SELECT " + field + " FROM " + table;
            //增加條件判斷
            if (condition != "" && condition != null) query += " WHERE " + condition;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    val = reader[0].ToString();
                }
            }
            return val;
        }

        //LOG紀錄
        public static void Logs(string type, string userid, string remark = "")
        {
            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
            string name = Models.Helper.getFieldValue("NAME", "I_USER", "USERID='" + userid + "'");
            string query = "INSERT INTO SYS_LOG (LOG_TYPE, USERID, USERNAME, LOG_DT, REMARK) "
                + " VALUES(:LOG_TYPE, :USERID, :USERNAME, TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS'), :REMARK)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.Add("LOG_TYPE", OracleDbType.Varchar2).Value = type;
                cmd.Parameters.Add("USERID", OracleDbType.Varchar2).Value = userid;
                cmd.Parameters.Add("USERNAME", OracleDbType.Varchar2).Value = name;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = remark;
                cmd.ExecuteNonQuery();
            }
        }
    }
}