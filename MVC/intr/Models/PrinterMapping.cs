using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class PrinterMapping
    {
        public decimal PRINTER;
        public string TONER;
        public string HASTONER;
        public string CODE;
        public string NAME;
        public string Mapping;
    }

    //處理印表機綁定耗材
    public class PrinterMappingDataContext
    {
        private decimal type = 2; //印表機耗材

        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
        public PrinterMapping LoadPrinterMapping(int id)
        {
            //印表機資料
            string query = "SELECT CODE,NAME FROM I_MATERIAL WHERE MID=" + id.ToString();
            
            PrinterMapping pm = new PrinterMapping();
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

                    //未選耗材
                    string query2 = "SELECT MID,CODE,NAME FROM I_MATERIAL WHERE TID=:TYPE AND MID NOT IN (SELECT TONER FROM I_PT_REF01 WHERE PRINTER=" + id.ToString() + ") ORDER BY CODE";
                    OracleCommand cmd2 = new OracleCommand(query2);
                    cmd2.Connection = conn;
                    cmd2.Parameters.Add("TYPE", OracleDbType.Decimal).Value = type;
                    OracleDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        string selectData = "";
                        while (reader2.Read())
                        {
                            selectData += "<option value=" + reader2["MID"].ToString() + ">"
                                + reader2["CODE"].ToString() + "：" + reader2["NAME"].ToString()
                                + "</option> \n";
                        }
                        pm.TONER = selectData;
                    }
                    reader2.Close();

                    //已選耗材
                    string query3 = "SELECT A.MID, A.CODE, A.NAME FROM I_MATERIAL A, I_PT_REF01 B WHERE A.TID=:TYPE AND B.PRINTER=" + id.ToString() + " AND A.MID=B.TONER ORDER BY CODE";
                    OracleCommand cmd3 = new OracleCommand(query3);
                    cmd3.Connection = conn;
                    cmd3.Parameters.Add("TYPE", OracleDbType.Decimal).Value = type;
                    OracleDataReader reader3 = cmd3.ExecuteReader();
                    if (reader3.HasRows)
                    {
                        string selectData = "";
                        while (reader3.Read())
                        {
                            selectData += "<option value=" + reader3["MID"].ToString() + ">"
                                + reader3["CODE"].ToString() + "：" + reader3["NAME"].ToString()
                                + "</option> \n";
                        }
                        pm.HASTONER = selectData;
                    }
                    reader3.Close();
                }
                reader.Close();
            }
            return pm;
        }

        //更新對應資料
        public void UpdatePrinterMapping(PrinterMapping p)
        {
            string[] hasToner = (string.IsNullOrEmpty(p.Mapping)) ? null : p.Mapping.Split(',');

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                //先清除舊資料
                string query = "DELETE FROM I_PT_REF01 WHERE PRINTER=" + p.PRINTER;
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                if (hasToner != null)
                {
                    foreach (string x in hasToner)
                    {
                        string query1 = "INSERT INTO I_PT_REF01 (PRINTER, TONER) VALUES(" + p.PRINTER + ", " + x + ")";
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = conn;
                        cmd1.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}