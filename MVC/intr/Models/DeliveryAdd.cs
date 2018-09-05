using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class DeliveryAdd
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
    }

    public class DeliveryAddDataContext
    {
        //DB連線字串
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //建立領料單
        public DeliveryAdd CreateOrder(string aid)
        {
            string query = "INSERT INTO I_REQUISITIONORDER (CREATOR, STATUS, UPDATE_BY, DID, CHKNO) VALUES(:CREATOR, :STATUS, :UPDATE_BY, :DID, :CHKNO)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                string did = Models.Helper.getFieldValue("DID", "I_UD_REF01", "AID=" + aid);
                string chkno = Models.Encryption.getMd5Method(DateTime.Now.ToFileTime().ToString());
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("CREATOR", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("STATUS", OracleDbType.Decimal).Value = 3; //補單,待領料
                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("DID", OracleDbType.Decimal).Value = decimal.Parse(did);
                cmd.Parameters.Add("CHKNO", OracleDbType.Varchar2).Value = chkno;
                cmd.ExecuteNonQuery();
            }

            //取得進貨單主檔資訊
            DeliveryAdd order = new DeliveryAdd();
            order.RID = decimal.Parse(Models.Helper.getFieldValue("MAX(RID)", "I_REQUISITIONORDER", "CREATOR=" + aid));
            order.CREATOR = decimal.Parse(aid);
            order.CNAME = Models.Helper.getFieldValue("CNAME", "I_REQUISITIONORDER", "RID=" + order.RID);
            order.CREATE_DATE = Models.Helper.getFieldValue("TO_CHAR(CREATE_DATE,'YYYY-MM-DD')", "I_REQUISITIONORDER", "RID=" + order.RID);
            order.STATUS = 4; //已領料
            order.UPDATE_BY = order.CREATOR;
            order.UNAME = order.CNAME;
            order.UPDATE_DATE = order.CREATE_DATE;

            //部門資訊
            using (OracleConnection conn1 = new OracleConnection(constr))
            {
                string query1 = "SELECT DID,CODE,NAME FROM I_DEPARTMENT WHERE ENABLE=1 ORDER BY CODE"; //AND CID>=3";
                OracleCommand cmd1 = new OracleCommand(query1, conn1);
                conn1.Open();
                OracleDataReader reader = cmd1.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        order.DNAME += "<option value='" + reader["DID"] + "'>" + reader["CODE"] + "：" + reader["NAME"] + "</option>";
                    }
                }
            }

            return order;
        }


        //更新主檔資料
        public void UpdateOrder(string RID, string REMARK, string AID, string DID)
        {
            string query = "UPDATE I_REQUISITIONORDER SET UPDATE_BY=:UPDATE_BY,AUDITOR=:AUDITOR,REMARK=:REMARK,STATUS=4,DID=" + DID + " WHERE RID=" + RID;

            //Start
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Decimal).Value = decimal.Parse(AID);
                cmd.Parameters.Add("AUDITOR", OracleDbType.Decimal).Value = decimal.Parse(AID);
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = REMARK;
                cmd.ExecuteNonQuery();

                //填寫財務部NO
                string FNO1 = Models.Helper.getFieldValue("TO_CHAR(SYSDATE,'YYYY')", "DUAL");
                string FNO2 = Models.Helper.getFieldValue("TO_CHAR(SYSDATE,'MM')", "DUAL");
                string FNO3 = Models.Helper.getFieldValue("A.CID", "I_DEPARTMENT A,I_REQUISITIONORDER B", "A.DID=B.DID AND B.RID=" + RID);
                //判斷本店外店
                FNO3 = (FNO3 == "1") ? "90" : "98";
                string FNO4 = Models.Helper.getFieldValue("NVL(MAX(FNO4),0)+1", "I_REQUISITIONORDER", "FNO1=" + FNO1 + " AND FNO2=" + FNO2 + " AND FNO3=" + FNO3);


                string query1 = "UPDATE I_REQUISITIONORDER SET FNO1=:FNO1,FNO2=:FNO2,FNO3=:FNO3,FNO4=:FNO4 WHERE RID=" + RID;
                OracleCommand cmd1 = new OracleCommand(query1);
                cmd1.Connection = conn;
                cmd1.Parameters.Add("FNO1", OracleDbType.Varchar2).Value = FNO1;
                cmd1.Parameters.Add("FNO2", OracleDbType.Varchar2).Value = FNO2;
                cmd1.Parameters.Add("FNO3", OracleDbType.Varchar2).Value = FNO3;
                cmd1.Parameters.Add("FNO4", OracleDbType.Int16).Value = int.Parse(FNO4);
                cmd1.ExecuteNonQuery();
            }
        }

        //新增物料明細檔
        public void InsertOrderDetails(string RID, string MID, string QTY)
        {
            char[] delimiterChars = { '_' };
            string[] words = MID.Split(delimiterChars);
            string preDate = string.Empty;
            string preQty = string.Empty;

            string query = "INSERT INTO I_REQUISITIONDETAILS (RID, MID, MNAME, QTY, SEQ, PREDATE, PREQTY) VALUES(:RID, :MID, :MNAME, :QTY, :SEQ, :PREDATE, :PREQTY)";
            string MNAME = Models.Helper.getFieldValue("NAME", "I_MATERIAL", "MID=" + words[1]);

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                string query1 = "SELECT TO_CHAR(A.RECECIVE_DATE,'YYYY-MM-DD') PREDATE, B.QTY PREQTY"
                    + " FROM I_REQUISITIONORDER A, I_REQUISITIONDETAILS B"
                    + " WHERE A.RID=B.RID AND A.STATUS >=4 AND B.SEQ=" + words[0] + " AND RECECIVE_DATE IS NOT NULL"
                    + " ORDER BY A.RECECIVE_DATE DESC";
                OracleCommand cmd1 = new OracleCommand(query1);
                cmd1.Connection = conn;
                OracleDataReader reader1 = cmd1.ExecuteReader();
                if (reader1.HasRows)
                {
                    reader1.Read();
                    preDate = reader1["PREDATE"].ToString();
                    preQty = reader1["PREQTY"].ToString();
                }

                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.Add("RID", OracleDbType.Decimal).Value = decimal.Parse(RID);
                cmd.Parameters.Add("MID", OracleDbType.Decimal).Value = decimal.Parse(words[1]);
                cmd.Parameters.Add("MNAME", OracleDbType.Varchar2).Value = MNAME;
                cmd.Parameters.Add("QTY", OracleDbType.Decimal).Value = decimal.Parse(QTY);
                cmd.Parameters.Add("SEQ", OracleDbType.Decimal).Value = decimal.Parse(words[0]);
                cmd.Parameters.Add("PREDATE", OracleDbType.Varchar2).Value = preDate;
                cmd.Parameters.Add("PREQTY", OracleDbType.Varchar2).Value = preQty;
                cmd.ExecuteNonQuery();
            }
        }



        //刪除申請單資料
        public void DeleteOrder(int id, string chkNo)
        {
            string query = "DELETE FROM I_REQUISITIONORDER WHERE RID=" + id + " AND CHKNO='" + chkNo + "'";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        //取消申請單
        public void DenyOrder(int rid, string reason, string chkno, string aid, string userid)
        {
            string query1 = "UPDATE I_REQUISITIONORDER SET FNO1='',FNO2='',FNO3='',FNO4='',STATUS=0,AUDITOR=:AUDITOR WHERE RID=" + rid + " AND CHKNO='" + chkno + "'";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                OracleCommand cmd = new OracleCommand(query1);
                cmd.Connection = conn;
                cmd.Parameters.Add("AUDITOR", OracleDbType.Decimal).Value = decimal.Parse(aid);
                cmd.ExecuteNonQuery();

                //寫入LOG紀錄
                Models.Helper.Logs("DenyOrder", userid, rid + ":" + reason);

            }
        }

    }
}