using System.Configuration;
using IntranetSystem.Poco;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Reflection;

namespace IntranetSystem.Models
{
    public class WebsiteUpload
    {
        private string connStr = ConfigurationManager.ConnectionStrings["MysqlServices"].ConnectionString;

        public List<WebsiteUploadPoco> LoadWebsiteAd()
        {
            List<WebsiteUploadPoco> wup = new List<WebsiteUploadPoco>();

            string query = "SELECT UUID,NAME,SDATE,EDATE,FILE,LINK,SORTNO,HOTELSN FROM MOrgnization.WebAd ORDER BY SORTNO";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Connection = conn;

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        WebsiteUploadPoco wp = new WebsiteUploadPoco();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = wp.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(wp, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }
                        wup.Add(wp);
                    }
                }
                reader.Close();
            }

            return wup;
        }

        //新增資料
        public void InsertData(WebsiteUploadPoco data)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO MOrgnization.WebAd (NAME, SDATE, EDATE, FILE, LINK, AID,HOTELSN) "
                    + "VALUES(@NAME, @SDATE, @EDATE, @FILE, @LINK, @AID,@HOTELSN)";
                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@NAME", data.NAME);
                cmd.Parameters.AddWithValue("@SDATE", data.SDATE);
                cmd.Parameters.AddWithValue("@EDATE", data.EDATE);
                cmd.Parameters.AddWithValue("@FILE", data.FILE);
                cmd.Parameters.AddWithValue("@LINK", data.LINK);
                cmd.Parameters.AddWithValue("@AID", data.AID);
                cmd.Parameters.AddWithValue("@HOTELSN", data.HOTELSN);
                cmd.ExecuteNonQuery();
            }
        }

        //取得編輯資料
        public WebsiteUploadPoco GetWebsiteUploadData(int id)
        {
            WebsiteUploadPoco data = new WebsiteUploadPoco();

            string query = "SELECT NAME,SDATE,EDATE,FILE,LINK,SORTNO,HOTELSN FROM MOrgnization.WebAd WHERE UUID=" + id;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Connection = conn;

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data.UUID = id;
                        data.HOTELSN = int.Parse(reader["HOTELSN"].ToString());
                        data.NAME = reader["NAME"].ToString();
                        data.SDATE = DateTimeUtil.getDateTime(reader["SDATE"].ToString());
                        data.EDATE = DateTimeUtil.getDateTime(reader["EDATE"].ToString());
                        data.FILE = reader["FILE"].ToString();
                        data.LINK = reader["LINK"].ToString();
                        data.SORTNO = int.Parse(reader["SORTNO"].ToString());
                    }
                }
                reader.Close();
            }
            return data;
        }

        //更新資料
        public void UpdateData(WebsiteUploadPoco data)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = string.Empty;
                conn.Open();
                if (data.FILE != null && !string.IsNullOrEmpty(data.FILE))
                {
                    query = "UPDATE MOrgnization.WebAd SET NAME=@NAME, SDATE=@SDATE, EDATE=@EDATE, FILE=@FILE, LINK=@LINK, AID=@AID WHERE UUID=" + data.UUID;
                }
                else
                {
                    query = "UPDATE MOrgnization.WebAd SET NAME=@NAME, SDATE=@SDATE, EDATE=@EDATE, LINK=@LINK, AID=@AID WHERE UUID=" + data.UUID;
                }
                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@NAME", data.NAME);
                cmd.Parameters.AddWithValue("@SDATE", data.SDATE);
                cmd.Parameters.AddWithValue("@EDATE", data.EDATE);
                cmd.Parameters.AddWithValue("@LINK", data.LINK);
                cmd.Parameters.AddWithValue("@AID", data.AID);
                if (data.FILE != null && !string.IsNullOrEmpty(data.FILE))
                {
                    cmd.Parameters.AddWithValue("@FILE", data.FILE);
                }
                cmd.ExecuteNonQuery();
            }
        }


        //刪除資料
        public void Delete(int id)
        {
            string query = "DELETE FROM MOrgnization.WebAd WHERE UUID=" + id;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        //更新排序
        public void SortUpdate(string sortData)
        {
            string[] strs = sortData.Split(',');
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                int i = 1;
                foreach (var item in strs)
                {
                    if (!string.IsNullOrEmpty(item.ToString()))
                    {
                        string query = "UPDATE MOrgnization.WebAd SET SORTNO=" + i + " WHERE UUID=" + item;
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                        i++;
                    }
                }
            }
        }
    }
}