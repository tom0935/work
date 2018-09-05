using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Menu
    {
        public decimal MID { get; set; }
        public string NAME { get; set; }
        public string REMARK { get; set; }
        public decimal SORT { get; set; }
    }

    public class MenuDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<Menu> LoadMenu()
        {
            string query = "SELECT MID,NAME,REMARK,SORT FROM I_MENU ORDER BY SORT";

            List<Menu> menus = new List<Menu>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Menu menu = new Menu();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = menu.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(menu, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        menus.Add(menu);
                    }
                }
                reader.Close();
            }
            return menus;
        }

        //新增選單
        public void CreateMenu(Menu menu)
        {
            string query = "INSERT INTO I_MENU (NAME, REMARK) VALUES(:NAME, :REMARK)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = menu.NAME;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = menu.REMARK;
                cmd.ExecuteNonQuery();
            }
        }

        //編輯選單
        public Menu EditMenu(int id)
        {
            string query = "SELECT MID,NAME,REMARK FROM I_MENU WHERE MID=" + id.ToString();

            Menu menu = new Menu();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    menu.MID = decimal.Parse(reader["MID"].ToString());
                    menu.NAME = reader["NAME"].ToString();
                    menu.REMARK = reader["REMARK"].ToString();
                }
                reader.Close();
            }
            return menu;
        }



        //更新選單
        public void UpdateMenu(Menu menu)
        {
            string query = "UPDATE I_MENU SET NAME=:NAME, REMARK=:REMARK WHERE MID=" + menu.MID;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = menu.NAME;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = menu.REMARK;

                cmd.ExecuteNonQuery();
            }
        }

        //刪除選單
        public void DeleteMenu(int id)
        {
            string query = "DELETE FROM I_MENU WHERE MID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //更新選單排序
        public void UpdateMenuSort(string mid, string sort)
        {
            string query = "UPDATE I_MENU SET SORT=:SORT WHERE MID=" + mid;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("SORT", OracleDbType.Decimal).Value = decimal.Parse(sort);
                cmd.ExecuteNonQuery();
            }
        }
    }
}