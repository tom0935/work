using System.Collections.Generic;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Reflection;


namespace IntranetSystem.Models
{
    public class Company
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string VAT { get; set; }
        public string ADDR { get; set; }
        public string TEL { get; set; }
        public string FAX { get; set; }
        public string URL { get; set; }
        public string REMARK { get; set; }
    }


    public class CompanyDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public Company LoadCompany()
        {
            string query = "SELECT CODE,NAME,VAT,ADDR,TEL,FAX,URL,REMARK FROM I_COMPANY";
            Company company = new Company();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    company.CODE = reader["CODE"].ToString();
                    company.NAME = reader["NAME"].ToString();
                    company.VAT = reader["VAT"].ToString();
                    company.ADDR = reader["ADDR"].ToString();
                    company.TEL = reader["TEL"].ToString();
                    company.FAX = reader["FAX"].ToString();
                    company.URL = reader["URL"].ToString();
                    company.REMARK = reader["REMARK"].ToString();
                }
                reader.Close();
            }
            return company;
        }

        //修改公司資料
        public void UpdateCompany(Company com)
        {
            string query = "UPDATE I_COMPANY SET CODE=:CODE, NAME=:NAME, VAT=:VAT, ADDR=:ADDR, TEL=:TEL, FAX=:FAX, URL=:URL, REMARK=:REMARK";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = com.CODE;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = com.NAME;
                cmd.Parameters.Add("VAT", OracleDbType.Varchar2).Value = com.VAT;
                cmd.Parameters.Add("ADDR", OracleDbType.Varchar2).Value = com.ADDR;
                cmd.Parameters.Add("TEL", OracleDbType.Varchar2).Value = com.TEL;
                cmd.Parameters.Add("FAX", OracleDbType.Varchar2).Value = com.FAX;
                cmd.Parameters.Add("URL", OracleDbType.Varchar2).Value = com.URL;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = com.REMARK;
                cmd.ExecuteNonQuery();
            }
        }
    }

}