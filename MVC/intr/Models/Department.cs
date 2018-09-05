using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Department
    {
        public decimal DID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string REMARK { get; set; }
    }

    public class DepartmentDataContext
    {
        //預設值
        //private decimal cid = 1;
        //private int enable = 1;

        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<Department> LoadDepartment()
        {
            string query = "SELECT DID,CODE,NAME,REMARK FROM I_DEPARTMENT ORDER BY CODE";

            List<Department> departments = new List<Department>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                //cmd.Parameters.Add("CID", OracleDbType.Decimal).Value = cid;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Department department = new Department();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = department.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(department, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        departments.Add(department);
                    }
                }
                reader.Close();
            }
            return departments;
        }


        //新增部門
        public void CreateDepartment(Department depart)
        {
            string query = "INSERT INTO I_DEPARTMENT (NAME, CODE, REMARK) VALUES(:NAME, :CODE, :REMARK)";

            using (OracleConnection conn = new OracleConnection(constr))
            {

                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = depart.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = depart.CODE;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = depart.REMARK;
                cmd.ExecuteNonQuery();
            }
        }


        //部門編輯
        public Department EditDepartment(int id)
        {
            string query = "SELECT DID,CODE,NAME,REMARK FROM I_DEPARTMENT WHERE DID=:DID";

            Department department = new Department();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("DID", OracleDbType.Decimal).Value = id;
                //cmd.Parameters.Add("CID", OracleDbType.Decimal).Value = cid;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    department.DID = decimal.Parse(reader["DID"].ToString());
                    department.CODE = reader["CODE"].ToString();
                    department.NAME = reader["NAME"].ToString();
                    department.REMARK = reader["REMARK"].ToString();
                }
                reader.Close();
            }
            return department;
        }

        //修改部門資料
        public void UpdateDepartment(Department depart)
        {
            string query = "UPDATE I_DEPARTMENT SET CODE=:CODE, NAME=:NAME, REMARK=:REMARK WHERE DID=" + depart.DID.ToString();
            
            using (OracleConnection conn = new OracleConnection(constr))
            {
                
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = depart.CODE;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = depart.NAME;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = depart.REMARK;                
                cmd.ExecuteNonQuery();
            }
        }

        //刪除部門資料
        public void DeleteDepartment(int id)
        {
            string query = "DELETE FROM I_DEPARTMENT WHERE DID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }

}