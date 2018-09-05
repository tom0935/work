using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Group
    {
        public decimal GID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string REMARK { get; set; }
    }

    public class GroupDataContext
    {
        //預設值
        private int enable = 1;

        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<Group> LoadGroup()
        {
            string query = "SELECT GID,CODE,NAME,REMARK FROM I_GROUP";

            List<Group> groups = new List<Group>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("ENABLE", OracleDbType.Int16).Value = enable;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Group group = new Group();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = group.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(group, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        groups.Add(group);
                    }
                }
                reader.Close();
            }
            return groups;
        }

        //新增群組
        public void CreateGroup(Group group)
        {
            string query = "INSERT INTO I_GROUP (NAME, CODE, REMARK) VALUES(:NAME, :CODE, :REMARK)";

            using (OracleConnection conn = new OracleConnection(constr))
            {

                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = group.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = group.CODE;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = group.REMARK;
                cmd.ExecuteNonQuery();
            }
        }


        //編輯群組
        public Group EditGroup(int id)
        {
            string query = "SELECT GID,NAME,CODE,REMARK FROM I_GROUP WHERE GID=" + id.ToString();

            Group group = new Group();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    group.GID = decimal.Parse(reader["GID"].ToString());
                    group.NAME = reader["NAME"].ToString();
                    group.CODE = reader["CODE"].ToString();
                    group.REMARK = reader["REMARK"].ToString();
                }
                reader.Close();
            }
            return group;
        }

        //更新群組
        public void UpdateGroup(Group group)
        {
            string query = "UPDATE I_GROUP SET NAME=:NAME, CODE=:CODE, REMARK=:REMARK WHERE GID=" + group.GID;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = group.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = group.CODE;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = group.REMARK;

                cmd.ExecuteNonQuery();
            }
        }

        //刪除群組
        public void DeleteGroup(int id)
        {
            string query = "DELETE FORM I_GROUP WHERE GID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}