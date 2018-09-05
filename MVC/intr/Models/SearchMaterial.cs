using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class SearchMaterial
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //物料類型
        public string getTypelLists(string selected = "")
        {
            string lists = string.Empty;
            string query = "SELECT TID,NAME FROM I_MATERIALTYPE ORDER BY TID";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    lists += "<select name='MaterialType' id='MaterialType'>";
                    while (reader.Read())
                    {
                        if (reader["TID"].ToString() == selected)
                            lists += "<option value='" + reader["TID"] + " selected'> " + reader["NAME"] + " </option>";
                        else
                            lists += "<option value='" + reader["TID"] + "'> " + reader["NAME"] + " </option>";
                    }
                    lists += "</select>";
                }
            }

            return lists;
        }

        //物料
        public string getMaterialLists(string type, string selected = "")
        {
            string lists = string.Empty;
            string query = "SELECT MID,CODE,NAME FROM I_MATERIAL WHERE TID=" + type + " ORDER BY CODE";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader["MID"].ToString() == selected)
                            lists += "<option value='" + reader["MID"] + "' selected> " + reader["CODE"] + "：" + reader["NAME"] + " </option>";
                        else
                            lists += "<option value='" + reader["MID"] + "'> " + reader["CODE"] + "：" + reader["NAME"] + " </option>";
                    }
                }
            }

            return lists;
        }
    }
}