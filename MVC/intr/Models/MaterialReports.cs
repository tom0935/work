using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Oracle.DataAccess.Client;
using System.Web.Hosting;

namespace IntranetSystem.Models
{
    public class MaterialReports
    {
        public string RECECIVE_DATE { get; set; }
        public decimal RID { get; set; }
        public decimal MID { get; set; }
        public string MNAME { get; set; }
        public decimal QTY { get; set; }
        public string CNAME { get; set; }
        public string DNAME { get; set; }
    }

    public class MaterialReportsDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //統計耗材部門領取
        public List<MaterialReports> GetDeliveryReport(string startDate, string endDate, string mid)
        {
            List<MaterialReports> report = new List<MaterialReports>();
            string query = "SELECT A.RID,TO_CHAR(A.RECECIVE_DATE, 'YYYY-MM-DD') RECECIVE_DATE, B.MID, B.MNAME, B.QTY, A.CNAME, A.DNAME"
                + " FROM I_REQUISITIONORDER A, I_REQUISITIONDETAILS B"
                + " WHERE A.RID=B.RID AND B.MID=" + mid + " AND A.STATUS=4"
                + " AND TO_CHAR(A.RECECIVE_DATE, 'YYYY-MM-DD') BETWEEN '" + startDate + "' AND '" + endDate + "'"
                + " ORDER BY A.RECECIVE_DATE";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MaterialReports mr = new MaterialReports();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = mr.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(mr, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        report.Add(mr);
                    }
                }
                reader.Close();
            }
            return report;
        }

        //取得耗材清單
        public Dictionary<string, string> GetMaterialDictionary()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string query = "SELECT MID,CODE,NAME FROM I_MATERIAL WHERE TID=2 AND ENABLE=1 ORDER BY CODE";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(reader["MID"].ToString(), reader["CODE"].ToString() + " - " + reader["NAME"].ToString());
                    }
                }
            }

            return result;
        }

        //回傳部門耗材盤點表 Stream
        public Stream GetInventoryStream()
        {
            Stream fileStream = new MemoryStream();
            ReportDocument cryRpt = new ReportDocument();

            DataTable dt = new DataTable();
            dt.Columns.Add("CODE");
            dt.Columns.Add("NAME");
            dt.Columns.Add("SQTY");
            dt.Columns.Add("IQTY");
            dt.Columns.Add("PQTY");
            dt.Columns.Add("DQTY");

            string query = "SELECT MID,CODE,NAME,SAFETY FROM I_MATERIAL WHERE TID=2 AND SAFETY>0 AND ENABLE=1 ORDER BY CODE";
            try
            {
                using (OracleConnection conn = new OracleConnection(constr))
                {
                    OracleCommand cmd = new OracleCommand(query, conn);
                    conn.Open();

                    OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int printerAmount = 0; //印表機數量
                            int inAmount = 0; //進貨數量
                            int outAmount = 0; //出貨數量
                            int inventory = 0; //剩餘=進貨-出貨
                            int diff_qty = 0; //差異=剩餘-安全

                            printerAmount = int.Parse(Models.Helper.getFieldValue("COUNT(1)", " I_DT_REF01 A, I_PT_REF01 B", "A.MID=B.PRINTER AND A.NOSHOW=1 AND B.TONER=" + reader["MID"]));
                            inAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_INCOMINGDETAILS A, I_INCOMINGORDER B", "A.MID=" + reader["MID"] + " AND A.IID=B.IID AND A.ENABLE=1 AND B.STATUS>=2"));
                            outAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS>=2"));
                            inventory = inAmount - outAmount;

                            diff_qty = inventory - int.Parse(reader["SAFETY"].ToString());

                            DataRow dr = dt.NewRow();
                            dr["CODE"] = reader["CODE"].ToString();
                            dr["NAME"] = reader["NAME"].ToString();
                            dr["SQTY"] = reader["SAFETY"].ToString();
                            dr["IQTY"] = inventory.ToString();
                            dr["PQTY"] = printerAmount.ToString();
                            dr["DQTY"] = diff_qty.ToString();
                            dt.Rows.Add(dr);
                        }
                    }
                }
                //設定Crystal Reports                    
                cryRpt.Load(HostingEnvironment.MapPath("~/Reports/TonerReport1.rpt"));
                cryRpt.SetDatabaseLogon("INTRA", "INTRA");
                cryRpt.Database.Tables["TonerInventory"].SetDataSource(dt);
                cryRpt.Refresh();
                fileStream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
            }
            catch
            {

            }
            finally
            {
                cryRpt.Close();
                cryRpt.Dispose();
            }
            return fileStream;
        }

        //回傳耗材申請狀態統計總表 Stream
        public Stream GetOrderSummaryStream()
        {
            Stream fileStream = new MemoryStream();
            ReportDocument cryRpt = new ReportDocument();

            DataTable dt = new DataTable();
            dt.Columns.Add("CODE");
            dt.Columns.Add("NAME");
            dt.Columns.Add("STAT0"); //作廢數量
            dt.Columns.Add("STAT1"); //新單數量
            dt.Columns.Add("STAT2"); //待審數量
            dt.Columns.Add("STAT3"); //已審待領
            dt.Columns.Add("STAT4"); //已領數量

            string query = "SELECT MID,CODE,NAME FROM I_MATERIAL WHERE TID=2 AND SAFETY>0 AND ENABLE=1 ORDER BY CODE";
            try
            {
                using (OracleConnection conn = new OracleConnection(constr))
                {
                    OracleCommand cmd = new OracleCommand(query, conn);
                    conn.Open();

                    OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            dr["CODE"] = reader["CODE"].ToString();
                            dr["NAME"] = reader["NAME"].ToString();
                            dr["STAT0"] = Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS=0");
                            dr["STAT1"] = Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS=1");
                            dr["STAT2"] = Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS=2");
                            dr["STAT3"] = Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS=3");
                            dr["STAT4"] = Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS=4");
                            dt.Rows.Add(dr);
                        }
                    }
                }
                //設定Crystal Reports                    
                cryRpt.Load(HostingEnvironment.MapPath("~/Reports/TonerReport2.rpt"));
                cryRpt.SetDatabaseLogon("INTRA", "INTRA");
                cryRpt.Database.Tables["TonerStatus"].SetDataSource(dt);
                cryRpt.Refresh();
                fileStream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
            }
            catch
            {

            }
            finally
            {
                cryRpt.Close();
                cryRpt.Dispose();
            }
            return fileStream;
        }

    }
}