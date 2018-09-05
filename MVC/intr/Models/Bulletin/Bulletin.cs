using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using IntranetSystem.Poco;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Data;

namespace IntranetSystem.Models.Bulletin
{
    public class Bulletin
    {
        //連線字串
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //取得公佈欄清單
        public List<BulletinPoco> LoadBulletin(int id, int page = 1, int pageSize = 25)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            //string query = "SELECT * FROM (SELECT ROWNUM RN, UUID, BDATE, CATEGORY, NAME, PATH, REMARK FROM I_BULLETIN WHERE BTYPE=" + id + " ORDER BY BDATE DESC) WHERE RN BETWEEN " + startRN + " AND " + endRN + " ORDER BY BDATE DESC";
           // string query = "SELECT * FROM (SELECT ROWNUM RN, UUID, BDATE, CATEGORY, NAME, PATH, REMARK FROM I_BULLETIN WHERE BTYPE=" + id + " ORDER BY BDATE DESC) ORDER BY BDATE DESC";
            string query = "SELECT * FROM (SELECT ROWNUM RN, UUID, BDATE, CATEGORY, NAME, PATH, REMARK FROM I_BULLETIN WHERE BTYPE="+id+" order by rownum desc) where  rownum <= 11 ORDER BY BDATE DESC";

            List<BulletinPoco> bulletin = new List<BulletinPoco>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                int j = 0;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        BulletinPoco bulletinPoco = new BulletinPoco();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = bulletinPoco.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(bulletinPoco, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);                            
                        }

                        if (reader.RowSize > 9 && j == 9)
                        {
                            bulletinPoco.RN = -1;
                        }

                        if (j <= 9)
                        {
                            bulletin.Add(bulletinPoco);
                        }
                        j++;
                    }
                }
                reader.Close();
            }
            return bulletin;
        }


        public JObject getGrid(int id,int limit,int offset)
        {
            JArray ja = new JArray();
            JArray ja2 = new JArray();
            JObject jo = new JObject();
            List<BulletinPoco> bulletin = new List<BulletinPoco>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                offset = offset + 1;
                limit = (limit + offset);
                string query = "SELECT ROWNUM RN, UUID, BDATE, CATEGORY, NAME, PATH, REMARK FROM I_BULLETIN WHERE BTYPE=" + id + " order by bdate desc";
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                int j = 0;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        BulletinPoco bulletinPoco = new BulletinPoco();
                        /*
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = bulletinPoco.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(bulletinPoco, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                            jo.Add(property);    
                        }   */

                    var itemObject = new JObject
                        {                                           
                            {"NAME",StringUtils.getString(reader["NAME"])},
                            {"BDATE",StringUtils.getString(reader["BDATE"])},                        
                            {"CATEGORY",StringUtils.getString(reader["CATEGORY"])},  
                            {"PATH",StringUtils.getString(reader["PATH"])}  
                        };

                        ja.Add(itemObject);
                        j++;
                    }
                }
                reader.Close();
                if (j > 0)
                {
                    jo.Add("rows", ja);
                    jo.Add("total", j);
                }
                else
                {
                    jo.Add("rows", "");
                    jo.Add("total", "");
                }
                
            }
            return jo;
        }



        //新增資料
        public void InsertData(BulletinPoco bulletin)
        {
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                string query = "INSERT INTO I_BULLETIN (NAME, CATEGORY, BTYPE, BDATE, PATH, REMARK, AID) "
                    + "VALUES(:NAME, :CATEGORY, :BTYPE, :BDATE, :PATH, :REMARK, :AID)";
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = bulletin.NAME;
                cmd.Parameters.Add("CATEGORY", OracleDbType.Varchar2).Value = bulletin.CATEGORY;
                cmd.Parameters.Add("BTYPE", OracleDbType.Decimal).Value = bulletin.BTYPE;
                cmd.Parameters.Add("BDATE", OracleDbType.Date).Value = bulletin.BDATE;
                cmd.Parameters.Add("PATH", OracleDbType.Varchar2).Value = bulletin.PATH;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = bulletin.REMARK;
                cmd.Parameters.Add("AID", OracleDbType.Decimal).Value = bulletin.AID;
                cmd.ExecuteNonQuery();
            }
        }

        //刪除資料
        public void DeleteData(int uuid)
        {
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                string query = "DELETE FROM I_BULLETIN WHERE UUID=" + uuid;
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }


        //取得公佈欄類別
        public List<BulletinTypePoco> LoadBulletinType()
        {
            string query = "SELECT UUID,CODE,NAME FROM I_BULLETINTYPE";

            List<BulletinTypePoco> bulletin = new List<BulletinTypePoco>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        BulletinTypePoco bulletinTypePoco = new BulletinTypePoco();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = bulletinTypePoco.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(bulletinTypePoco, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        bulletin.Add(bulletinTypePoco);
                    }
                }
                reader.Close();
            }
            return bulletin;
        }

        //取得公佈欄類別
        public BulletinTypePoco GetBulletinType(int btype)
        {
            string query = "SELECT CODE,NAME FROM I_BULLETINTYPE WHERE UUID=" + btype;

            BulletinTypePoco bulletinType = new BulletinTypePoco();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    bulletinType.UUID = btype;
                    bulletinType.CODE = reader["CODE"].ToString();
                    bulletinType.NAME = reader["NAME"].ToString();
                    reader.Close();
                }
            }
            return bulletinType;
        }

        //取得MAC
        public string GetMacAddress(string IP)
        {
            string dirResults = "";
            ProcessStartInfo psi = new ProcessStartInfo();
            Process proc = new Process();
            psi.FileName = "nbtstat";
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = "-a " + IP;
            psi.UseShellExecute = false;
            proc = Process.Start(psi);
            dirResults = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //匹配mac地址
            Match m = Regex.Match(dirResults, "\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w\\w");

            //若匹配成功则返回mac，否则返回找不到主机信息
            if (m.ToString() != "")
            {
                return m.ToString();
            }
            else
            {
                return "";
            }

        }
    }
}