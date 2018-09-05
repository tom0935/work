using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models.Service
{
    public class BookingWebFdService
    {
        private string connStr = ConfigurationManager.ConnectionStrings["MysqlServices"].ConnectionString;
        private string oConnStr = ConfigurationManager.ConnectionStrings["HwtpeServices"].ConnectionString;

        //取得訂單資料列表
        public JObject getTransData()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            int i = 0; //筆數
            Dictionary<string, string[]> fdrsvt00 = new Dictionary<string, string[]>();

            //儲存前台訂單資料
            using (OracleConnection oConn = new OracleConnection(oConnStr))
            {
                OracleCommand oComm = oConn.CreateCommand();
                oConn.Open();
                string oSql = "SELECT PID, RSVNO, RSVSTAT FROM FDRSVT00 WHERE PID IS NOT NULL AND DEPDT >= TO_CHAR(SYSDATE,'YYYY-MM-DD')";
                oComm.CommandText = oSql;
                OracleDataReader oReader = oComm.ExecuteReader();
                if (oReader.HasRows)
                {
                    while (oReader.Read())
                    {
                        string[] data = {
                                            oReader["RSVNO"].ToString(),
                                            oReader["RSVSTAT"].ToString()
                                        };
                        fdrsvt00.Add(oReader["PID"].ToString(), data);
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand comm = conn.CreateCommand();
                conn.Open();

                string sql = "SELECT A.SN, D.FirstName, D.LastName, A.HotelSn, C.Name, B.WebName, A.ResultNo, A.ProcessNo, A.TotalAmount, A.StartSearchTime, A.CheckinDate, A.CheckoutDate "
                    + "FROM Transactions A, ProjectPackages B, PackageInfo C, Customers D "
                    + "WHERE B.SN=A.PackageSn AND C.SN=B.PackageInfoSn AND D.TransactionSn=A.SN "
                    + "AND A.HotelSn IN (162) AND A.ResultNo IN (1)" //(0,1,3,9,20,22,24,28) "
                    + "AND date(A.CheckoutDate) >= curdate()"
                    + "ORDER BY A.SN DESC";
                comm.CommandText = sql;
                MySqlDataReader reader = comm.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string StartSearchTime = reader["StartSearchTime"].ToString();
                        string[] BookingDate = StartSearchTime.Split(' ');
                        string trans = StringUtils.getString(reader["SN"]).PadLeft(10, '0');
                        //WEB訂單資料轉JSON
                        var itemObject = new JObject
                        {
                            {"SN", trans},
                            {"HotelSn", StringUtils.getDecimal(reader["HotelSn"])},
                            {"Name", StringUtils.getString(reader["LastName"]) + StringUtils.getString(reader["FirstName"])},
                            {"PackageName", StringUtils.getString(reader["Name"])},
                            {"WebName", StringUtils.getString(reader["WebName"])},
                            {"ResultNo", StringUtils.getDecimal(reader["ResultNo"])},
                            {"ProcessNo", StringUtils.getDecimal(reader["ProcessNo"])},
                            {"TotalAmount", StringUtils.getDecimal(reader["TotalAmount"]).ToString("#,0")},
                            {"StartSearchTime", StartSearchTime},
                            {"BookingDate", BookingDate[0]}
                        };

                        //訂單為成功或取消時，查詢前台是否有對應資料
                        //前台資料轉JSON

                        if (StringUtils.getDecimal(reader["ResultNo"]) == 0 || StringUtils.getDecimal(reader["ResultNo"]) == 1)
                        {
                            if (fdrsvt00.ContainsKey(trans))
                            {
                                itemObject.Add("PID", trans);
                                itemObject.Add("RSVNO", fdrsvt00[trans][0]);
                                itemObject.Add("RSVSTAT", fdrsvt00[trans][1]);
                            }
                        }

                        ja.Add(itemObject);
                        i++;
                    }
                }
            }

            jo.Add("rows", ja);
            jo.Add("total", i); //總筆數
            return jo;
        }

        //取得HETPW前台訂單狀態
        public string getFdStatus(string pid)
        {
            string status = string.Empty;
            using (OracleConnection oConn = new OracleConnection(oConnStr))
            {
                OracleCommand oComm = oConn.CreateCommand();
                oConn.Open();

                string oSql = "SELECT PID, RSVNO, RSVSTAT FROM FDRSVT00 WHERE PID='" + pid + "'";
                oComm.CommandText = oSql;
                OracleDataReader oReader = oComm.ExecuteReader();
                if (oReader.HasRows)
                {
                    oReader.Read();
                    status = StringUtils.getString(oReader["RSVSTAT"]);
                }
            }

            return status;
        }


        //移除FDRSVT00的PID
        public string removePid(string pid)
        {
            string status = string.Empty;
            try
            {
                using (OracleConnection oConn = new OracleConnection(oConnStr))
                {
                    OracleCommand oComm = oConn.CreateCommand();
                    oConn.Open();

                    string oSql = "UPDATE FDRSVT00 SET PID=NULL WHERE PID='" + pid + "'";
                    oComm.CommandText = oSql;
                    oComm.ExecuteNonQuery();
                    status = "OK";
                }
            }
            catch (Exception e)
            {
                status = e.Message;
            }
            return status;
        }
    }
}