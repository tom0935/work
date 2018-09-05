using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class DepartMapping
    {
        public decimal AID;
        public string NAME;
        public string USERID;
        public string DEPARTMENT;
        public string WORKIN;
        public string Mapping;
    }

    public class DepartMappingDataContext
    {
        //預設值
        private decimal cid = 1;

        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
        public DepartMapping LoadDepartMapping(int id)
        {
            //使用者資料
            //string query = "SELECT NAME,USERID FROM I_USER WHERE CID=:CID AND AID=" + id.ToString();
            string query = "SELECT NAME,USERID FROM I_USER WHERE AID=" + id.ToString();

            DepartMapping dm = new DepartMapping();
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
                    dm.NAME = reader["NAME"].ToString();
                    dm.USERID = reader["USERID"].ToString();

                    //未選部門
                    string query2 = "SELECT DID, CODE, NAME FROM I_DEPARTMENT WHERE DID NOT IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + id.ToString() + ") ORDER BY DID";                    
                    OracleCommand cmd2 = new OracleCommand(query2);
                    cmd2.Connection = conn;
                    OracleDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        string selectData = "";
                        while (reader2.Read())
                        {
                            selectData += "<option value=" + reader2["DID"].ToString() + ">"
                                + reader2["CODE"].ToString() + " - " + reader2["NAME"].ToString()
                                + "</option> \n";
                        }
                        dm.DEPARTMENT = selectData;
                    }
                    reader2.Close();

                    //已選部門
                    string query3 = "SELECT A.DID, A.CODE, A.NAME FROM I_DEPARTMENT A, I_UD_REF01 B WHERE B.AID=" + id.ToString() + " AND A.DID=B.DID ORDER BY DID";
                    OracleCommand cmd3 = new OracleCommand(query3);
                    cmd3.Connection = conn;
                    OracleDataReader reader3 = cmd3.ExecuteReader();
                    if (reader3.HasRows)
                    {
                        string selectData = "";
                        while (reader3.Read())
                        {
                            selectData += "<option value=" + reader3["DID"].ToString() + ">"
                                + reader3["CODE"].ToString() + " - " + reader3["NAME"].ToString()
                                + "</option> \n";
                        }
                        dm.WORKIN = selectData;
                    }
                    reader3.Close();
                }
                reader.Close();
            }
            return dm;
        }

        public void UpdateDepartMapping(DepartMapping d)
        {
            string[] workin = (string.IsNullOrEmpty(d.Mapping)) ? null : d.Mapping.Split(',');

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                //先清除舊資料
                string query = "DELETE FROM I_UD_REF01 WHERE AID=" + d.AID;
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                if (workin != null)
                {
                    foreach (string x in workin)
                    {
                        string query1 = "INSERT INTO I_UD_REF01 (AID, DID) VALUES(" + d.AID + ", " + x + ")";
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = conn;
                        cmd1.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}