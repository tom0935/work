using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class User
    {
        public decimal RN { get; set; }
        public decimal AID { get; set; }
        public string USERID { get; set; }
        public string PASSWORD { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string REMARK { get; set; }
        public string CAKE_COMPANY { get; set; }
        public string CAKE_DEPARTMENT { get; set; }
        public int ENABLE { get; set; }
    }


    public class UserDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;


        public List<User> LoadUser(int pageSize, int page)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            string query = "SELECT * FROM"
                + " (SELECT ROWNUM RN,AID,USERID,NAME,EMAIL,REMARK,CAKE_COMPANY,CAKE_DEPARTMENT,ENABLE"
                + " FROM (SELECT * FROM I_USER ORDER BY USERID))"
                + " WHERE RN BETWEEN " + startRN + " AND " + endRN;

            List<User> users = new List<User>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User user = new User();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = user.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(user, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        users.Add(user);
                    }
                }
                reader.Close();
            }
            return users;
        }

        //新增使用者
        public void CreateUser(User user)
        {
            string query = "INSERT INTO I_USER (USERID, PASSWORD, NAME, EMAIL, REMARK, CAKE_COMPANY, CAKE_DEPARTMENT) VALUES(:USERID, :PASSWORD, :NAME, :EMAIL, :REMARK, :CAKE_COMPANY, :CAKE_DEPARTMENT)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                //密碼雜奏加密
                string passwd = Encryption.getMd5Method2(user.PASSWORD);
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("USERID", OracleDbType.Varchar2).Value = user.USERID;
                cmd.Parameters.Add("PASSWORD", OracleDbType.Varchar2).Value = passwd;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = user.NAME;
                cmd.Parameters.Add("EMAIL", OracleDbType.Varchar2).Value = user.EMAIL;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = user.REMARK;
                cmd.Parameters.Add("CAKE_COMPANY", OracleDbType.Varchar2).Value = user.CAKE_COMPANY;
                cmd.Parameters.Add("CAKE_DEPARTMENT", OracleDbType.Varchar2).Value = user.CAKE_DEPARTMENT;
                cmd.ExecuteNonQuery();
            }
        }

        //編輯使用者
        public User EditUser(int id)
        {
            string query = "SELECT AID,USERID,NAME,EMAIL,REMARK,CAKE_COMPANY,CAKE_DEPARTMENT FROM I_USER WHERE AID=:AID AND ENABLE=1";

            User user = new User();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("AID", OracleDbType.Decimal).Value = id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    user.AID = decimal.Parse(reader["AID"].ToString());
                    user.USERID = reader["USERID"].ToString();
                    user.NAME = reader["NAME"].ToString();
                    user.EMAIL = reader["EMAIL"].ToString();
                    user.REMARK = reader["REMARK"].ToString();
                    user.CAKE_COMPANY = reader["CAKE_COMPANY"].ToString();
                    user.CAKE_DEPARTMENT = reader["CAKE_DEPARTMENT"].ToString();
                }
                reader.Close();
            }
            return user;
        }

        //更新使用者
        public void UpdateUser(User user)
        {
            string query = string.Empty;
            string passwd = string.Empty;
            if (user.PASSWORD != null)
            {
                query = "UPDATE I_USER SET USERID=:USERID, PASSWORD=:PASSWORD, NAME=:NAME, EMAIL=:EMAIL, REMARK=:REMARK, CAKE_COMPANY=:CAKE_COMPANY, CAKE_DEPARTMENT=:CAKE_DEPARTMENT WHERE AID=" + user.AID;
                //密碼雜奏加密
                passwd = Encryption.getMd5Method2(user.PASSWORD);
            }
            else
            {
                query = "UPDATE I_USER SET USERID=:USERID, NAME=:NAME, EMAIL=:EMAIL, REMARK=:REMARK, CAKE_COMPANY=:CAKE_COMPANY, CAKE_DEPARTMENT=:CAKE_DEPARTMENT WHERE AID=" + user.AID;
            }

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                
                cmd.Parameters.Add("USERID", OracleDbType.Varchar2).Value = user.USERID;
                if (user.PASSWORD != null) cmd.Parameters.Add("PASSWORD", OracleDbType.Varchar2).Value = passwd;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = user.NAME;
                cmd.Parameters.Add("EMAIL", OracleDbType.Varchar2).Value = user.EMAIL;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = user.REMARK;
                cmd.Parameters.Add("CAKE_COMPANY", OracleDbType.Varchar2).Value = user.CAKE_COMPANY;
                cmd.Parameters.Add("CAKE_DEPARTMENT", OracleDbType.Varchar2).Value = user.CAKE_DEPARTMENT;
                
                cmd.ExecuteNonQuery();
            }
        }


        //刪除使用者
        public void DeleteUser(int id)
        {
            string query = "DELETE I_USER WHERE AID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //禁用使用者
        public void DisableUser(int id)
        {
            string query = "UPDATE I_USER SET ENABLE=0 WHERE AID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //啟用使用者
        public void EnableUser(int id)
        {
            string query = "UPDATE I_USER SET ENABLE=1 WHERE AID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //更新變更完成
        public string ChangePassword(User user)
        {
            string msg = string.Empty;

            if (user.PASSWORD != null)
            {
                //密碼雜奏加密
                string passwd = Encryption.getMd5Method2(user.PASSWORD);

                string query = "UPDATE I_USER SET PASSWORD=:PASSWORD WHERE AID=" + user.AID;
                using (OracleConnection conn = new OracleConnection(constr))
                {
                    OracleCommand cmd = new OracleCommand(query, conn);
                    conn.Open();
                    cmd.Parameters.Add("PASSWORD", OracleDbType.Varchar2).Value = passwd;
                    cmd.ExecuteNonQuery();
                }
                msg = "密碼已經變更完成，請登出後再使用新密碼重新登入。";
            }
            else
            {
                msg = "密碼是空白的，無法變更密碼。";
            }

            return msg;
        }

        //搜尋使用者
        public List<User> SearchUser(string keywords)
        {
            string query = "SELECT distinct i.AID,i.USERID,i.NAME,i.EMAIL,i.REMARK,i.CAKE_COMPANY,i.CAKE_DEPARTMENT,i.ENABLE"
                + " FROM I_USER i,I_UD_REF01 d,I_DEPARTMENT d1 "
                + " where i.AID=d.AID(+) and d.DID=d1.DID(+) and "
                + " (i.USERID LIKE '%" + keywords + "%' OR i.NAME LIKE '%" + keywords + "%' OR i.EMAIL LIKE '%" + keywords + "%' OR d1.NAME LIKE '%"+keywords+"%' )";

            List<User> users = new List<User>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User user = new User();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = user.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(user, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        users.Add(user);
                    }
                }
                reader.Close();
            }
            return users;
        }
    }
}