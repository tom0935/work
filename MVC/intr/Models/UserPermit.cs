using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class UserPermit
    {
        public decimal AID;
        public string NAME;
        public string USERID;
        public string DEPARTMENT;
        public string GROUP;
        public string HASGROUP;
        public string PROGRAM;
        public string HASPROGRAM;
        public string GMapping;
        public string PMapping;
    }

    public class UserPermitDataContext
    {
        //預設值
        private decimal cid = 1;
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public UserPermit LoadUserPermit(int id)
        {
            //使用者資料
            //string query = "SELECT NAME,USERID FROM I_USER WHERE CID=:CID AND AID=" + id.ToString();
            string query = "SELECT NAME,USERID FROM I_USER WHERE AID=" + id.ToString();

            UserPermit permit = new UserPermit();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.Add("CID", OracleDbType.Decimal).Value = cid;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    permit.NAME = reader["NAME"].ToString();
                    permit.USERID = reader["USERID"].ToString();

                    //所屬部門
                    string query2 = "SELECT DID, CODE, NAME FROM I_DEPARTMENT WHERE DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + id.ToString() + ") ORDER BY DID";
                    OracleCommand cmd2 = new OracleCommand(query2);
                    cmd2.Connection = conn;
                    OracleDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        string data = "";
                        int i = 1;
                        while (reader2.Read())
                        {
                            data += ((i == 1) ? "" : "、");
                            data += reader2["NAME"].ToString();
                            i++;
                        }
                        permit.DEPARTMENT = data;
                    }
                    reader2.Close();


                    //未選群組
                    string query3 = "SELECT GID,NAME FROM I_GROUP WHERE GID NOT IN (SELECT GID FROM I_UG_REF01 WHERE AID=" + id.ToString() + ") ORDER BY GID";
                    OracleCommand cmd3 = new OracleCommand(query3);
                    cmd3.Connection = conn;
                    OracleDataReader reader3 = cmd3.ExecuteReader();
                    if (reader3.HasRows)
                    {
                        string selectData = "";
                        while (reader3.Read())
                        {
                            selectData += "<option value=" + reader3["GID"].ToString() + ">"
                                + reader3["NAME"].ToString()
                                + "</option> \n";
                        }
                        permit.GROUP = selectData;
                    }
                    reader3.Close();

                    //已選群組
                    string query4 = "SELECT A.GID,A.NAME FROM I_GROUP A, I_UG_REF01 B WHERE B.AID=" + id.ToString() + " AND A.GID=B.GID ORDER BY GID";
                    OracleCommand cmd4 = new OracleCommand(query4);
                    cmd4.Connection = conn;
                    OracleDataReader reader4 = cmd4.ExecuteReader();
                    if (reader4.HasRows)
                    {
                        string selectData = "";
                        while (reader4.Read())
                        {
                            selectData += "<option value=" + reader4["GID"].ToString() + ">"
                                + reader4["NAME"].ToString()
                                + "</option> \n";
                        }
                        permit.HASGROUP = selectData;
                    }
                    reader4.Close();


                    //未選程式
                    string query5 = "SELECT PID,NAME FROM I_PROGRAM WHERE PID NOT IN (SELECT PID FROM I_UP_REF01 WHERE AID=" + id.ToString() + ") ORDER BY MID,PID";
                    OracleCommand cmd5 = new OracleCommand(query5);
                    cmd5.Connection = conn;
                    OracleDataReader reader5 = cmd5.ExecuteReader();
                    if (reader5.HasRows)
                    {
                        string selectData = "";
                        while (reader5.Read())
                        {
                            selectData += "<option value=" + reader5["PID"].ToString() + ">"
                                + reader5["NAME"].ToString()
                                + "</option> \n";
                        }
                        permit.PROGRAM = selectData;
                    }
                    reader5.Close();

                    //已選程式
                    string query6 = "SELECT A.PID,A.NAME FROM I_PROGRAM A, I_UP_REF01 B WHERE B.AID=" + id.ToString() + " AND A.PID=B.PID ORDER BY MID,PID";
                    OracleCommand cmd6 = new OracleCommand(query6);
                    cmd6.Connection = conn;
                    OracleDataReader reader6 = cmd6.ExecuteReader();
                    if (reader6.HasRows)
                    {
                        string selectData = "";
                        while (reader6.Read())
                        {
                            selectData += "<option value=" + reader6["PID"].ToString() + ">"
                                + reader6["NAME"].ToString()
                                + "</option> \n";
                        }
                        permit.HASPROGRAM = selectData;
                    }
                    reader6.Close();
                }
                reader.Close();
            }
            return permit;
        }

        //更新使用者權限設定
        public void UpdateUserPermit(UserPermit p)
        {
            string[] hasgroup = (string.IsNullOrEmpty(p.GMapping)) ? null : p.GMapping.Split(',');
            string[] hasprogram = (string.IsNullOrEmpty(p.PMapping)) ? null : p.PMapping.Split(',');

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                //先清除舊資料，再寫入GROUP資料
                string query1 = "DELETE FROM I_UG_REF01 WHERE AID=" + p.AID;
                OracleCommand cmd1 = new OracleCommand(query1);
                cmd1.Connection = conn;
                cmd1.ExecuteNonQuery();
                if (hasgroup != null)
                {
                    foreach (string x in hasgroup)
                    {
                        string query2 = "INSERT INTO I_UG_REF01 (AID, GID) VALUES(" + p.AID + ", " + x + ")";
                        OracleCommand cmd2 = new OracleCommand(query2);
                        cmd2.Connection = conn;
                        cmd2.ExecuteNonQuery();
                    }
                }


                //先清除舊資料，再寫入PROGRAM資料
                string query3 = "DELETE FROM I_UP_REF01 WHERE AID=" + p.AID;
                OracleCommand cmd3 = new OracleCommand(query3);
                cmd3.Connection = conn;
                cmd3.ExecuteNonQuery();
                if (hasprogram != null )
                {
                    foreach (string y in hasprogram)
                    {
                        string query4 = "INSERT INTO I_UP_REF01 (AID, PID) VALUES(" + p.AID + ", " + y + ")";
                        OracleCommand cmd4 = new OracleCommand(query4);
                        cmd4.Connection = conn;
                        cmd4.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}