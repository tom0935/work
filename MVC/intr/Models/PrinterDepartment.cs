using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class PrinterDepartment
    {
        public string PRINTER;
        public string DEPARTMENT;
        public string TEAM;
        public decimal DID;
        public decimal MID;
    }

    public class PrinterDepartmentDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //取得印表機資料
        public PrinterDepartment GetPrinter(int id)
        {
            PrinterDepartment printer = new PrinterDepartment();
            printer.MID = id;
            printer.PRINTER = Models.Helper.getFieldValue("NAME", "I_MATERIAL", "MID=" + id);
            return printer;
        }

        //取得部門資料
        public string DepartmentlLists(string selected = "")
        {
            string lists = string.Empty;
            string query = "SELECT DID,CODE,NAME FROM I_DEPARTMENT ORDER BY CODE";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader["DID"].ToString() == selected)
                            lists += "<option value='" + reader["DID"] + " selected'> " + reader["CODE"] + "：" + reader["NAME"] + " </option>";
                        else
                            lists += "<option value='" + reader["DID"] + "'> " + reader["CODE"] + "：" + reader["NAME"] + " </option>";
                    }
                }
            }

            return lists;
        }

        //更新資料
        public void UpdatePrinterDepartment(string DID, int MID, string TEAM, int NOSHOW)
        {
            string query = "INSERT INTO I_DT_REF01 (DID,MID,TEAM,NOSHOW) VALUES(:DID,:MID,:TEAM,:NOSHOW)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("DID", OracleDbType.Decimal).Value = decimal.Parse(DID);
                cmd.Parameters.Add("MID", OracleDbType.Decimal).Value = MID;
                cmd.Parameters.Add("TEAM", OracleDbType.Varchar2).Value = TEAM;
                cmd.Parameters.Add("NOSHOW", OracleDbType.Decimal).Value = NOSHOW;

                cmd.ExecuteNonQuery();
            }
        }

        //刪除資料
        public void DeletePrinterDepartment(int MID)
        {
            string query = "DELETE FROM I_DT_REF01 WHERE MID=" + MID.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //取得物料明細
        public string Details(int id)
        {
            string lists = string.Empty;
            string query = "SELECT A.DID,B.CODE,B.NAME,A.TEAM,A.NOSHOW FROM I_DT_REF01 A, I_DEPARTMENT B WHERE A.DID=B.DID AND A.MID=" + id.ToString() + " ORDER BY B.CODE";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    int i = 1;
                    while (reader.Read())
                    {
                        lists += "<tr>"
                            + "<td>" + i.ToString() + "</td>"
                            + "<td>" + reader["CODE"] + "</td>"
                            + "<td>" + reader["NAME"] + "</td>"
                            + "<td><input type='hidden' id='" + reader["DID"].ToString() + "_" + i.ToString() + "' name='" + reader["DID"].ToString() + "_" + i.ToString() + "' value='" + reader["DID"].ToString() + "'>"
                            + "<input type='text' class='form-control' id='" + reader["DID"].ToString() + "_" + i.ToString() + "T' name='" + reader["DID"].ToString() + "_" + i.ToString() + "T' value='" + reader["TEAM"] + "' placeholder='如要細分部門內的小單位，於此填寫名稱'></td>"
                            + "<td>"
                            + "<input class='form-control chk-size' type='checkbox' id='" + reader["DID"].ToString() + "_" + i.ToString() + "NS' name='" + reader["DID"].ToString() + "_" + i.ToString() + "NS' value='1' " + (reader["NOSHOW"].ToString() == "1" ? "checked" : "") + ">"
                            + "</td>"
                            + "<td>"
                            + "<button type='button' class='btn btn-danger' onclick='rmDepartment(this);'>整筆移除</button>"
                            + "</td>"
                            + "<tr>";
                        i++;
                    }
                    lists = "<script type='text/javascript'> var seqno = new Number(" + i + "); </script>" + lists;
                }
            }

            return lists;
        }

    }
}