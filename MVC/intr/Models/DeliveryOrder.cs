using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class DeliveryOrder
    {
        public decimal RN { get; set; }
        public decimal RID { get; set; }
        public decimal CREATOR { get; set; }
        public string CNAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string REMARK { get; set; }
        public int STATUS { get; set; }
        public string STATUS_TEXT { get; set; }
        public decimal UPDATE_BY { get; set; }
        public string UNAME { get; set; }
        public string UPDATE_DATE { get; set; }
        public string RECECIVE_DATE { get; set; }
        public string DNAME { get; set; }
        public decimal DID { get; set; }
        public string CHKNO { get; set; }
        public string FNO { get; set; }
        public string CXNL { get; set; }

        public string MNAME { get; set; }
        public decimal QTY { get; set; }
    }


    public class DeliveryOrderDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<DeliveryOrder> LoadOrder(int id, string aid, int pageSize, int page)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            string query = string.Empty;
            switch (id)
            {
                default://待領
                case 1:
                    query = "SELECT * FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, DID, CHKNO FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=3 ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
                    /*
                    query = "SELECT a.*,b.mname,b.qty FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, DID, CHKNO "+
                            "FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=3 ORDER BY RID DESC) ) a , ( select max(b.mname) mname ,count(*) qty,rid from I_REQUISITIONDETAILS b  group by b.rid) b where a.rid=b.rid  and a.RN BETWEEN " + startRN + " AND " + endRN;
                     * */
                    break;
                case 2://已領
                    query = "SELECT * FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, TO_CHAR(RECECIVE_DATE,'YYYY-MM-DD HH24:MI') RECECIVE_DATE, DID, CHKNO, (LPAD(FNO2,2,'0')||LPAD(FNO3,2,'0')||LPAD(FNO4,2,'0')) FNO FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=4 ORDER BY RECECIVE_DATE DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;

                    /*
                    query = "SELECT a.*,b.mname,b.qty FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE,TO_CHAR(RECECIVE_DATE,'YYYY-MM-DD HH24:MI') RECECIVE_DATE, DID, CHKNO ,(LPAD(FNO2,2,'0')||LPAD(FNO3,2,'0')||LPAD(FNO4,2,'0')) FNO " +
                            "FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=4 ORDER BY RECECIVE_DATE DESC) ) a , ( select max(b.mname) mname ,count(*) qty,rid from I_REQUISITIONDETAILS b  group by b.rid) b where a.rid=b.rid and a.RN BETWEEN " + startRN + " AND " + endRN;
                    */
                    break;
                case 3://未審核
                    query = "SELECT * FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, DID, CHKNO FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=2 ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
                    /*
                    query = "SELECT a.*,b.mname,qty FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, DID, CHKNO " +
                            "FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=2 ORDER BY RID DESC) ) a , ( select max(b.mname) mname ,count(*) qty,rid from I_REQUISITIONDETAILS b  group by b.rid) b where a.rid=b.rid and a.RN BETWEEN " + startRN + " AND " + endRN;
                    */

                    break;
                case 4: //作廢
                    query = "SELECT * FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, DID, CHKNO FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=0 ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
                    /*
                    query = "SELECT a.*,b.mname,b.qty FROM ( SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, DID, CHKNO "+
                            "FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=0 ORDER BY RID DESC) ) a , ( select max(b.mname) mname ,count(*) qty,rid from I_REQUISITIONDETAILS b  group by b.rid) b where a.rid=b.rid  and a.RN BETWEEN " + startRN + " AND " + endRN;
                     * */
                    break;
            }

            List<DeliveryOrder> orders = new List<DeliveryOrder>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DeliveryOrder order = new DeliveryOrder();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = order.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(order, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        switch (order.STATUS)
                        {
                            case 0:
                                order.STATUS_TEXT = "<span style='color:#CCC'>作廢</span>";
                                order.CXNL = Models.Helper.getFieldValue("REMARK", "SYS_LOG", "LOG_TYPE='DenyOrder' AND REMARK LIKE '" + order.RID + ":%'");
                                break;
                            default:
                            case 1:
                                order.STATUS_TEXT = "<span style='color:#428BCA'>新單</span>";
                                break;
                            case 2:
                                order.STATUS_TEXT = "<span style='color:#FF6347'>待審核</span>";
                                break;
                            case 3:
                                order.STATUS_TEXT = "<span style='color:#436EEE'>待領料</span>";
                                break;
                            case 4:
                                order.STATUS_TEXT = "<span style='color:#5CB85C'>已領料</span>";
                                break;
                        }
                        order.DNAME = Models.Helper.getFieldValue("NAME", "I_DEPARTMENT", "DID=" + order.DID);
                        orders.Add(order);
                    }
                }
                reader.Close();
            }
            return orders;
        }

        //編輯領料單
        public DeliveryOrder EditOrder(string aid, int rid)
        {
            string query = "SELECT CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, REMARK, DID, CHKNO FROM I_REQUISITIONORDER WHERE RID=" + rid;
            //取得進貨單主檔資訊
            DeliveryOrder order = new DeliveryOrder();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();

                    order.RID = rid;
                    order.CREATOR = decimal.Parse(reader["CREATOR"].ToString());
                    order.CNAME = reader["CNAME"].ToString();
                    order.CREATE_DATE = reader["CREATE_DATE"].ToString();
                    order.STATUS = int.Parse(reader["STATUS"].ToString());
                    order.UPDATE_BY = decimal.Parse(reader["UPDATE_BY"].ToString());
                    order.UNAME = reader["UNAME"].ToString();
                    order.UPDATE_DATE = reader["UPDATE_DATE"].ToString();
                    order.REMARK = reader["REMARK"].ToString();
                    order.DID = decimal.Parse(reader["DID"].ToString());
                    order.DNAME = Models.Helper.getFieldValue("NAME", "I_DEPARTMENT", "DID=" + order.DID);
                    order.CHKNO = reader["CHKNO"].ToString();
                }
            }

            return order;
        }


        //取消申請單
        public bool DenyOrder(int id, string chkno, string aid)
        {
            bool commit = false;
            string rid = Models.Helper.getFieldValue("RID", "I_REQUISITIONORDER", "CHKNO='" + chkno + "' AND STATUS=3");

            if (rid == id.ToString())
            {
                string name = Models.Helper.getFieldValue("NAME", "I_USER", "AID=" + aid);
                string query = "UPDATE I_REQUISITIONORDER SET STATUS=0,UPDATE_BY=:UPDATE_BY,UNAME=:UNAME,AUDITOR=:AUDITOR WHERE RID=" + id.ToString();

                using (OracleConnection conn = new OracleConnection(constr))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query);
                    cmd.Connection = conn;
                    cmd.Parameters.Add("UPDATE_BY", OracleDbType.Decimal).Value = decimal.Parse(aid);
                    cmd.Parameters.Add("UNAME", OracleDbType.Varchar2).Value = name;
                    cmd.Parameters.Add("AUDITOR", OracleDbType.Decimal).Value = decimal.Parse(aid);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        //信件通知
                        Models.Smtp smtp = new Models.Smtp(rid, "0");
                        commit = true;
                    }
                }
            }
            return commit;
        }
    }
}