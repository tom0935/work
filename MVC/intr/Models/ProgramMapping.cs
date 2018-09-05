using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class ProgramMapping
    {
        public decimal GID;
        public string NAME;
        public string CODE;
        public string PROGRAM;
        public string HASPROGRAM;
        public string Mapping;
    }

    public class ProgramMappingDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public ProgramMapping LoadProgramMapping(int id)
        {
            //群組資料
            string query = "SELECT NAME,CODE FROM I_GROUP WHERE GID=" + id.ToString();

            ProgramMapping pm = new ProgramMapping();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    pm.NAME = reader["NAME"].ToString();
                    pm.CODE = reader["CODE"].ToString();

                    //未歸屬的程式
                    string query2 = "SELECT PID,CODE,NAME FROM I_PROGRAM WHERE PID NOT IN (SELECT PID FROM I_GP_REF01 WHERE GID=" + id.ToString() + ") ORDER BY MID,PID";
                    OracleCommand cmd2 = new OracleCommand(query2);
                    cmd2.Connection = conn;
                    OracleDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        string selectData = "";
                        while (reader2.Read())
                        {
                            selectData += "<option value=" + reader2["PID"].ToString() + ">"
                                + reader2["CODE"].ToString() + " - " + reader2["NAME"].ToString()
                                + "</option> \n";
                        }
                        pm.PROGRAM = selectData;
                    }
                    reader2.Close();

                    //已歸屬的程式
                    string query3 = "SELECT A.PID,A.CODE,A.NAME FROM I_PROGRAM A, I_GP_REF01 B WHERE B.GID=" + id.ToString() + " AND A.PID=B.PID ORDER BY MID,PID";
                    OracleCommand cmd3 = new OracleCommand(query3);
                    cmd3.Connection = conn;
                    OracleDataReader reader3 = cmd3.ExecuteReader();
                    if (reader3.HasRows)
                    {
                        string selectData = "";
                        while (reader3.Read())
                        {
                            selectData += "<option value=" + reader3["PID"].ToString() + ">"
                                + reader3["CODE"].ToString() + " - " + reader3["NAME"].ToString()
                                + "</option> \n";
                        }
                        pm.HASPROGRAM = selectData;
                    }
                    reader3.Close();
                }
                reader.Close();
            }
            return pm;
        }

        //更新群組與程式歸屬
        public void UpdateProgramMapping(ProgramMapping p)
        {
            string[] hasProgram = (string.IsNullOrEmpty(p.Mapping)) ? null : p.Mapping.Split(',');

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                //先清除舊資料
                string query = "DELETE FROM I_GP_REF01 WHERE GID=" + p.GID;
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                if (hasProgram != null)
                {
                    foreach (string x in hasProgram)
                    {
                        string query1 = "INSERT INTO I_GP_REF01 (GID, PID) VALUES(" + p.GID + ", " + x + ")";
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = conn;
                        cmd1.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}