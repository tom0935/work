using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using IntranetSystem.Poco;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models.Bulletin
{
    public class Dashboards
    {
        //連線字串
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //取得待審核資料
        public string getAuditData(string aid)
        {
            string result = string.Empty;
            int auditCount = int.Parse(Models.Helper.getFieldValue("COUNT(*) CT", "I_REQUISITIONORDER", "STATUS=2 AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ")"));
            if (auditCount > 0)
            {
                result = "您有 <strong>" + auditCount + " </strong> 筆耗材申請單，等待審核中。";
            }
            return result;
        }

        //查詢庫存不足的耗材
        public List<string> getMaterialData()
        {
            List<string> result = new List<string>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                string query = "SELECT MID,CODE,NAME,SPEC,UNIT,SAFETY,REMARK FROM I_MATERIAL WHERE TID=2 AND SAFETY>0 AND ENABLE=1 ORDER BY CODE";
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    int inAmount = 0; //進貨數量
                    int outAmount = 0; //出貨數量
                    int inventory = 0; //剩餘=進貨-出貨                    

                    while (reader.Read())
                    {
                        int safety = int.Parse(reader["SAFETY"].ToString());
                        inAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_INCOMINGDETAILS A, I_INCOMINGORDER B", "A.MID=" + reader["MID"] + " AND A.IID=B.IID AND A.ENABLE=1 AND B.STATUS>=2"));
                        outAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS>=2"));
                        inventory = inAmount - outAmount;
                        int diff_qty = inventory - safety;
                        if (inventory < safety)
                        {
                            result.Add("<td>" + reader["CODE"] + "</td><td>" + reader["NAME"] + "</td><td><strong>" + safety + "</strong></td><td><strong><font color='red'>" + inventory + "</font></strong></td><td><font color='red'>" + diff_qty + "</font></td>");
                        }
                    }
                }

            }
            return result;
        }

        //查詢待領料 for IMC
        public string getDeliveryData()
        {
            string result = string.Empty;
            int deliveryCount = int.Parse(Models.Helper.getFieldValue("COUNT(*) CT", "I_REQUISITIONORDER", "STATUS=3"));
            if (deliveryCount > 0)
            {
                result = "使用者有 <strong>" + deliveryCount + " </strong> 筆耗材尚未領料。";
            }
            return result;
        }

        //查詢待領料單 for User
        public string getRequisitionData(string aid)
        {
            string result = string.Empty;
            int requisitionCount = int.Parse(Models.Helper.getFieldValue("COUNT(*) CT", "I_REQUISITIONORDER", "STATUS=3 AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ")"));
            if (requisitionCount > 0)
            {
                result = "您或您的部門有 <strong>" + requisitionCount + " </strong> 筆耗材尚未領取，請記得到資訊中心領取。";
            }
            return result;
        }

    }
}