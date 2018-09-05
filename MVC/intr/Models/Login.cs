using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Login
    {
        public int AID { get; set; }
        public string NAME { get; set; }

        public bool chkUser(string userid, string passwd){
            bool result = false;
            
            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
            
            using (OracleConnection conn = new OracleConnection(constr))
            {
                //密碼雜奏加密
                string pwd = Encryption.getMd5Method2(passwd);
                string query = "SELECT AID,NAME FROM I_USER WHERE USERID=:USERID AND PASSWORD=:PASSWORD AND ENABLE=1";

                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("USERID", OracleDbType.Varchar2).Value = userid;
                cmd.Parameters.Add("PASSWORD", OracleDbType.NVarchar2).Value = pwd;
                
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    result = true;
                    reader.Read();
                    AID = int.Parse(reader["AID"].ToString());
                    NAME = reader["NAME"].ToString();
                }
            }
            return result;
        }
    }
}