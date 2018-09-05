using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Program
    {
        public decimal PID { get; set; }
        public decimal MID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string REMARK { get; set; }
        public string MENU { get; set; }
        public string EXTRA { get; set; }
        public decimal SORT { get; set; }
    }

    public class ProgramDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<Program> LoadProgram(int id = 1)
        {
            string query = "SELECT PID,MID,CODE,NAME,EXTRA,REMARK,SORT FROM I_PROGRAM WHERE MID=" + id.ToString() + " ORDER BY SORT";

            List<Program> programs = new List<Program>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Program program = new Program();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = program.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(program, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        //歸屬選單名稱
                        program.MENU = Models.Helper.getFieldValue("NAME", "I_MENU", "MID=" + reader["MID"]);
                        programs.Add(program);
                    }
                }
                reader.Close();
            }
            return programs;
        }

        //新增程式資料
        public void CreateProgram(Program program)
        {
            string query = "INSERT INTO I_PROGRAM (MID, NAME, CODE, REMARK, EXTRA) VALUES(:MID, :NAME, :CODE, :REMARK, :EXTRA)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("MID", OracleDbType.Decimal).Value = program.MID;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = program.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = program.CODE;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = program.REMARK;
                cmd.Parameters.Add("EXTRA", OracleDbType.Varchar2).Value = program.EXTRA;
                cmd.ExecuteNonQuery();
            }
        }



        //編輯程式
        public Program EditProgram(int id)
        {
            string query = "SELECT PID,MID,NAME,CODE,REMARK,EXTRA FROM I_PROGRAM WHERE PID=" + id;

            Program program = new Program();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    program.PID = decimal.Parse(reader["PID"].ToString());
                    program.MID = decimal.Parse(reader["MID"].ToString());
                    program.NAME = reader["NAME"].ToString();
                    program.CODE = reader["CODE"].ToString();
                    program.REMARK = reader["REMARK"].ToString();
                    program.EXTRA = reader["EXTRA"].ToString();
                }
                reader.Close();
            }
            return program;
        }

        //更新程式
        public void UpdateProgram(Program program)
        {
            string query = "UPDATE I_PROGRAM SET MID=:MID, NAME=:NAME, CODE=:CODE, REMARK=:REMARK, EXTRA=:EXTRA WHERE PID=" + program.PID;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("MID", OracleDbType.Decimal).Value = program.MID;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = program.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = program.CODE;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = program.REMARK;
                cmd.Parameters.Add("EXTRA", OracleDbType.Varchar2).Value = program.EXTRA;
                cmd.ExecuteNonQuery();
            }
        }

        //刪除程式
        public void DeleteProgram(int id)
        {
            string query = "DELETE FROM I_PROGRAM WHERE PID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //讀取頁籤
        public string LoadTab()
        {
            string tab = string.Empty;

            string query = "SELECT MID,NAME FROM I_MENU WHERE ENABLE=1";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                }
            }
            return tab;
        }

        //更新程式排序
        public void UpdateProgramSort(string pid, string sort)
        {
            string query = "UPDATE I_PROGRAM SET SORT=:SORT WHERE PID=" + pid;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("SORT", OracleDbType.Decimal).Value = decimal.Parse(sort);
                cmd.ExecuteNonQuery();
            }
        }


        //取得歸屬選單SELECT，新增與修改時使用
        public string getMidList(){
            string val = string.Empty;
            string query = "SELECT MID,NAME FROM I_MENU WHERE ENABLE=1";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        val += "<option value=\"" + reader["MID"] + "\">" + reader["NAME"] + "</option>";
                    }
                }
            }
            return val;
        }


        //有預設值的取得歸屬選單SELECT
        public string getMidList(decimal mid)
        {
            string val = string.Empty;
            string query = "SELECT MID,NAME FROM I_MENU WHERE ENABLE=1";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader["MID"].ToString() == mid.ToString())
                            val += "<option value=\"" + reader["MID"] + "\" selected>" + reader["NAME"] + "</option>";
                        else
                            val += "<option value=\"" + reader["MID"] + "\">" + reader["NAME"] + "</option>";
                    }
                }
            }
            return val;
        } 


    }
}