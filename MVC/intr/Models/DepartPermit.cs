using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class DepartPermit
    {
        public decimal DID;
        public string NAME;
        public string CODE;
        public string GROUP;
        public string HASGROUP;
        public string PROGRAM;
        public string HASPROGRAM;
        public string GMapping;
        public string PMapping;
    }


    public class DepartPermitDataContext
    {

        //預設值
        private decimal cid = 1;
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public DepartPermit LoadDepartPermit(int id)
        {
            //部門資料
            //string query = "SELECT DID,NAME,CODE FROM I_DEPARTMENT WHERE CID=:CID AND DID=" + id.ToString();
            string query = "SELECT DID,NAME,CODE FROM I_DEPARTMENT WHERE DID=" + id.ToString();

            DepartPermit permit = new DepartPermit();
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
                    permit.CODE = reader["CODE"].ToString();

                    //未選群組
                    string query3 = "SELECT GID,NAME FROM I_GROUP WHERE GID NOT IN (SELECT GID FROM I_DG_REF01 WHERE DID=" + id.ToString() + ") ORDER BY GID";
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
                    string query4 = "SELECT A.GID,A.NAME FROM I_GROUP A, I_DG_REF01 B WHERE B.DID=" + id.ToString() + " AND A.GID=B.GID ORDER BY GID";
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
                    string query5 = "SELECT PID,NAME FROM I_PROGRAM WHERE PID NOT IN (SELECT PID FROM I_DP_REF01 WHERE DID=" + id.ToString() + ") ORDER BY MID,PID";
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
                    string query6 = "SELECT A.PID,A.NAME FROM I_PROGRAM A, I_DP_REF01 B WHERE B.DID=" + id.ToString() + " AND A.PID=B.PID ORDER BY MID,PID";
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


        //更新部門權限設定
        public void UpdateDepartPermit(DepartPermit d)
        {
            string[] hasgroup = (string.IsNullOrEmpty(d.GMapping)) ? null : d.GMapping.Split(',');
            string[] hasprogram = (string.IsNullOrEmpty(d.PMapping)) ? null : d.PMapping.Split(',');

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                //先清除舊資料，再寫入GROUP資料
                string query1 = "DELETE FROM I_DG_REF01 WHERE DID=" + d.DID;
                OracleCommand cmd1 = new OracleCommand(query1);
                cmd1.Connection = conn;
                cmd1.ExecuteNonQuery();
                if (hasgroup != null)
                {
                    foreach (string x in hasgroup)
                    {
                        string query2 = "INSERT INTO I_DG_REF01 (DID, GID) VALUES(" + d.DID + ", " + x + ")";
                        OracleCommand cmd2 = new OracleCommand(query2);
                        cmd2.Connection = conn;
                        cmd2.ExecuteNonQuery();
                    }
                }


                //先清除舊資料，再寫入PROGRAM資料
                string query3 = "DELETE FROM I_DP_REF01 WHERE DID=" + d.DID;
                OracleCommand cmd3 = new OracleCommand(query3);
                cmd3.Connection = conn;
                cmd3.ExecuteNonQuery();
                if (hasprogram != null)
                {
                    foreach (string y in hasprogram)
                    {
                        string query4 = "INSERT INTO I_DP_REF01 (DID, PID) VALUES(" + d.DID + ", " + y + ")";
                        OracleCommand cmd4 = new OracleCommand(query4);
                        cmd4.Connection = conn;
                        cmd4.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}