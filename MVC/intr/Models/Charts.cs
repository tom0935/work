using System.Collections.Generic;
using System.Configuration;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Charts
    {
        public static Dictionary<string, int> getTonerCount(string aid)
        {
            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
            Dictionary<string, int> toners = new Dictionary<string, int>();
            string query = "SELECT MID,MNAME,SUM(QTY) QTY FROM I_REQUISITIONDETAILS WHERE RID IN"
                + " (SELECT RID FROM I_REQUISITIONORDER WHERE (TO_CHAR(RECECIVE_DATE,'YYYY-MM')=TO_CHAR(SYSDATE,'YYYY-MM')) AND STATUS = 4 AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + "))"
                + " GROUP BY MID,MNAME";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        toners.Add(reader["MNAME"].ToString(), int.Parse(reader["QTY"].ToString()));
                    }
                }
                reader.Close();
            }
            return toners;
        }
    }
}