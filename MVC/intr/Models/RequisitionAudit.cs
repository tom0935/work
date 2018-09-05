using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class RequisitionAudit
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
        public string DNAME { get; set; }
        public decimal DID { get; set; }
    }

    public class RequisitionAuditDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<RequisitionAudit> LoadOrder(int id, string aid, int pageSize, int page)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            string query = string.Empty;

            //判斷要查未審核或已審核的申請單
            switch (id)
            {
                case 1:  //未審核
                    query = "SELECT * FROM (SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=2 AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ") ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
                    break;
                case 2:
                    query = "SELECT * FROM (SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS>2 AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ") ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
                    break;
                case 3:
                    query = "SELECT * FROM (SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE FROM (SELECT * FROM I_REQUISITIONORDER WHERE STATUS=0 AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ") ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
                    break;
            }

            List<RequisitionAudit> orders = new List<RequisitionAudit>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RequisitionAudit order = new RequisitionAudit();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = order.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(order, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        switch (order.STATUS)
                        {
                            case 0:
                                order.STATUS_TEXT = "<span style='color:#CCC'>作廢</span>";
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

                        orders.Add(order);
                    }
                }
                reader.Close();
            }
            return orders;
        }

        //核准申請單
        public bool AllowOrder(int id, string chkno, string aid)
        {
            bool commit = false;
            string rid = Models.Helper.getFieldValue("RID", "I_REQUISITIONORDER", "CHKNO='" + chkno + "' AND STATUS=2");

            if (rid == id.ToString())
            {
                string query = "UPDATE I_REQUISITIONORDER SET STATUS=3,AUDITOR=:AUDITOR WHERE RID=" + id.ToString();

                using (OracleConnection conn = new OracleConnection(constr))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query);
                    cmd.Connection = conn;
                    cmd.Parameters.Add("AUDITOR", OracleDbType.Decimal).Value = decimal.Parse(aid);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        //信件通知
                        Models.Smtp smtp = new Models.Smtp(rid, "3");
                        commit = true;
                    }
                }
            }
            return commit;
        }

        //取消申請單
        public bool DenyOrder(int id, string chkno, string aid)
        {
            bool commit = false;
            string rid = Models.Helper.getFieldValue("RID", "I_REQUISITIONORDER", "CHKNO='" + chkno + "' AND STATUS=2");

            if (rid == id.ToString())
            {
                string query = "UPDATE I_REQUISITIONORDER SET STATUS=0,AUDITOR=:AUDITOR WHERE RID=" + id.ToString();

                using (OracleConnection conn = new OracleConnection(constr))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query);
                    cmd.Connection = conn;
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