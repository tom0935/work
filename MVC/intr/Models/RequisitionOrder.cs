using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class RequisitionOrder
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
        public string CHKNO { get; set; }
        public string CXNL { get; set; }
    }

    public class RequisitionOrderDataContext
    {
        //DB連線字串
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
        //耗材審核的PID
        private int auditPID = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["RequisitionAudit"]);

        public List<RequisitionOrder> LoadOrder(int id, string aid, int pageSize, int page)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            string query = string.Empty;

            //判斷要查個人或部門的申請單
            if (id == 2) //部門
            {
                query = "SELECT * FROM (SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE FROM (SELECT * FROM I_REQUISITIONORDER WHERE CREATOR <> " + aid + " AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ") ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
            }
            else //個人
            {
                query = "SELECT * FROM (SELECT ROWNUM RN, RID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE FROM (SELECT * FROM I_REQUISITIONORDER WHERE CREATOR=" + aid + " ORDER BY RID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;
            }

            List<RequisitionOrder> orders = new List<RequisitionOrder>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RequisitionOrder order = new RequisitionOrder();

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

                        orders.Add(order);
                    }
                }
                reader.Close();
            }
            return orders;
        }

        //建立領料單
        public RequisitionOrder CreateOrder(string aid)
        {
            string query = "INSERT INTO I_REQUISITIONORDER (CREATOR, STATUS, UPDATE_BY, DID, CHKNO) VALUES(:CREATOR, :STATUS, :UPDATE_BY, :DID, :CHKNO)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                string did = Models.Helper.getFieldValue("DID", "I_UD_REF01", "AID=" + aid);
                string chkno = Models.Encryption.getMd5Method(DateTime.Now.ToFileTime().ToString());
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("CREATOR", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("STATUS", OracleDbType.Decimal).Value = 1; //新單
                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("DID", OracleDbType.Decimal).Value = decimal.Parse(did);
                cmd.Parameters.Add("CHKNO", OracleDbType.Varchar2).Value = chkno;
                cmd.ExecuteNonQuery();
            }

            //取得進貨單主檔資訊
            RequisitionOrder order = new RequisitionOrder();
            order.RID = decimal.Parse(Models.Helper.getFieldValue("MAX(RID)", "I_REQUISITIONORDER", "CREATOR=" + aid));
            order.CREATOR = decimal.Parse(aid);
            order.CNAME = Models.Helper.getFieldValue("CNAME", "I_REQUISITIONORDER", "RID=" + order.RID);
            order.CREATE_DATE = Models.Helper.getFieldValue("TO_CHAR(CREATE_DATE,'YYYY-MM-DD')", "I_REQUISITIONORDER", "RID=" + order.RID);
            order.STATUS = 1;
            order.UPDATE_BY = order.CREATOR;
            order.UNAME = order.CNAME;
            order.UPDATE_DATE = order.CREATE_DATE;

            //部門資訊
            using (OracleConnection conn1 = new OracleConnection(constr))
            {
                string query1 = "SELECT A.DID,A.NAME FROM I_DEPARTMENT A, I_UD_REF01 B WHERE A.DID=B.DID AND B.AID=" + aid;
                OracleCommand cmd1 = new OracleCommand(query1, conn1);
                conn1.Open();
                OracleDataReader reader = cmd1.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        order.DNAME += "<option value='" + reader["DID"] + "'>" + reader["NAME"] + "</option>";
                    }
                }
            }

            return order;
        }

        //編輯領料單
        public RequisitionOrder EditOrder(string aid, int rid)
        {
            string query = "SELECT CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD') CREATE_DATE, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, REMARK, DID, DNAME, CHKNO FROM I_REQUISITIONORDER WHERE RID=" + rid;
            //取得進貨單主檔資訊
            RequisitionOrder order = new RequisitionOrder();

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
                    order.DNAME = reader["DNAME"].ToString();
                    order.CHKNO = reader["CHKNO"].ToString();
                }
            }

            return order;
        }


        //刪除領料單資料
        public void DeleteRequisitionOrder(int id)
        {
            string query = "DELETE FROM I_REQUISITIONORDER WHERE RID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //刪除領料明細資料
        public void DeleteRequisitionDetails(int id)
        {
            string query = "DELETE FROM I_REQUISITIONDETAILS WHERE RID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //取得耗材明細[編輯模式]
        public string GetMaterialList(string aid, string did, int rid)
        {
            string list = string.Empty;

            //取得印表機資料
            string query = "SELECT A.DID,B.MID,B.CODE,B.NAME,A.TEAM FROM I_DT_REF01 A, I_MATERIAL B"
                + " WHERE A.MID=B.MID AND A.DID=" + did
                + " GROUP BY A.DID,B.MID,B.CODE,B.NAME,A.TEAM"
                + " ORDER BY B.CODE,A.TEAM";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list += "<label class='control-label' for='" + reader["MID"] + "'>"
                            + reader["CODE"] + "：" + reader["NAME"] +
                            ((reader["TEAM"] == null || reader["TEAM"].ToString() == "") ? "" : "《" + reader["TEAM"] + "》") + "</label>";
                        //避免取得重複TEAM名稱，必須另外取得SEQ
                        string seq = (reader["TEAM"] == null || reader["TEAM"].ToString() == "") ? Models.Helper.getFieldValue("SEQ", "I_DT_REF01", "DID=" + did + " AND MID=" + reader["MID"] + " AND TEAM IS NULL") : Models.Helper.getFieldValue("SEQ", "I_DT_REF01", "DID=" + did + " AND MID=" + reader["MID"] + " AND TEAM='" + reader["TEAM"] + "'");

                        //取得碳粉資料
                        //string query1 = "SELECT A.MID,A.CODE,A.NAME,A.UNIT FROM I_MATERIAL A,I_PT_REF01 B WHERE A.TID='" + reader["TID"] + "' AND A.ENABLE=1 AND A.MID=B.TONER AND B.PRINTER=" + reader["MID"];
                        string query1 = "SELECT A.MID,A.CODE,A.NAME,A.UNIT FROM I_MATERIAL A,I_PT_REF01 B WHERE A.TID='2' AND A.ENABLE=1 AND A.MID=B.TONER AND B.PRINTER=" + reader["MID"];
                        using (OracleConnection conn1 = new OracleConnection(constr))
                        {
                            OracleCommand cmd1 = new OracleCommand(query1, conn1);
                            conn1.Open();

                            OracleDataReader reader1 = cmd1.ExecuteReader();
                            if (reader1.HasRows)
                            {
                                list += "<table class='table table-hover table-condensed' >"
                                    + "<tbody>";
                                while (reader1.Read())
                                {
                                    int inAmount = 0; //有效進貨量
                                    int outAmount1 = 0; //有效出貨量
                                    int outAmount2 = 0; //目前申請單出貨量
                                    int inventory1 = 0; //有效剩餘量

                                    inAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_INCOMINGDETAILS A, I_INCOMINGORDER B", "A.MID=" + reader1["MID"] + " AND A.IID=B.IID AND A.ENABLE=1 AND B.STATUS>=2"));
                                    outAmount1 = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader1["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS>=2"));

                                    inventory1 = inAmount - outAmount1;

                                    list += "<tr>"
                                        + "<td>" + reader1["CODE"] + "</td>"
                                        + "<td>" + reader1["NAME"] + "</td>";

                                    //有RID，表示為編輯模式
                                    if (rid > 0)
                                    {
                                        //此項目申請量
                                        int val = int.Parse(Models.Helper.getFieldValue("NVL(SUM(QTY),0)", "I_REQUISITIONDETAILS", "RID=" + rid + " AND MID=" + reader1["MID"] + " AND SEQ=" + seq));
                                        //此單已選的數量
                                        outAmount2 = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + reader1["MID"] + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS>=1 AND B.RID=" + rid));

                                        //扣除此單已選的數量
                                        if ((inventory1 - outAmount2) < 0)
                                        {
                                            //檢查申請數量有沒有大於剩餘數量
                                            val = (val > inventory1) ? inventory1 : val;
                                            inventory1 = 0;
                                        }
                                        else
                                        {
                                            inventory1 = inventory1 - outAmount2;
                                        }
                                        list += "<td><input type='number' value='" + val + "' id='" + seq + "_" + reader1["MID"] + "' name='" + seq + "_" + reader1["MID"] + "' class='form-control input-sm' disabled></td>";
                                    }
                                    else
                                    {
                                        list += "<td><input type='number' value='0' id='" + seq + "_" + reader1["MID"] + "' name='" + seq + "_" + reader1["MID"] + "' class='form-control input-sm' disabled></td>";
                                    }

                                    list += "<td>(剩餘：<span id='inventory_" + reader1["MID"] + "' class='inventory'>" + inventory1 + "</span>)</td>"
                                        + "<td><button type='button' class='btn btn-info' onclick=\"plusMaterial('" + seq + "_" + reader1["MID"] + "');\">＋</button>"
                                        + "<button type='button' class='btn btn-warning' onclick=\"minusMaterial('" + seq + "_" + reader1["MID"] + "');\">－</button></td>"
                                        + "</tr>";
                                }
                                list += "</tbody></table>";
                            }
                        }

                    }
                }
            }

            return list;
        }


        //取得耗材明細[檢視模式]
        public string GetMaterialListView(string aid, string did, int rid)
        {
            string list = string.Empty;

            //取得申請單資料
            string query = "SELECT B.CODE, A.MID, A.MNAME, A.QTY, A.SEQ, A.PREDATE, A.PREQTY"
                + " FROM I_REQUISITIONDETAILS A, I_MATERIAL B"
                + " WHERE A.ENABLE=1 AND A.MID=B.MID AND A.RID=" + rid
                + " ORDER BY B.CODE,A.SEQ";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    string team = string.Empty;
                    int i = 0;
                    list += "<table class='table table-hover table-condensed' ><tbody>"
                        + "<tr><th>＃</th><th>料號</th><th>耗材名稱</th><th>上　　次<br>領用日期</th><th>上　　次<br>領用數量</th><th>請領數</th></tr>";

                    while (reader.Read())
                    {
                        i++;
                        team = Models.Helper.getFieldValue("TEAM", "I_DT_REF01", "SEQ=" + reader["SEQ"]);
                        team = (team == "") ? "" : "《" + team + "》";
                        list += "<tr>"
                            + "<td>" + i + "</td>"
                            + "<td>" + reader["CODE"] + "</td>"
                            + "<td>" + reader["MNAME"] + team + "</td>"
                            + "<td>" + reader["PREDATE"] + "</td>"
                            + "<td>" + reader["PREQTY"] + "</td>"
                            + "<td>" + reader["QTY"] + "</td>"
                            + "</tr>";
                    }
                    list += "</tbody></table>";
                }

            }

            return list;
        }



        //更新主檔資料
        public void UpdateOrder(string RID, string REMARK, string STATUS, string aid, string DID = "", string userid = "")
        {
            string query = string.Empty;
            if (DID == "") //新建單才有DID，編輯單沒有
            {
                query = "UPDATE I_REQUISITIONORDER SET UPDATE_BY=:UPDATE_BY,REMARK=:REMARK,STATUS=:STATUS WHERE RID=" + RID;
            }
            else
            {
                query = "UPDATE I_REQUISITIONORDER SET UPDATE_BY=:UPDATE_BY,REMARK=:REMARK,STATUS=:STATUS,DID=" + DID + " WHERE RID=" + RID;
            }

            //Start
            using (OracleConnection conn = new OracleConnection(constr))
            {
                int status = int.Parse(STATUS);
                conn.Open();
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = REMARK;
                cmd.Parameters.Add("STATUS", OracleDbType.Int16).Value = status;
                cmd.ExecuteNonQuery();

                //申請單為待審核，需要寄送郵件給審核者
                if (status == 2)
                {
                    if (Models.NavigationMenu.chkUserAuth(userid, "RequisitionAudit")) //有審核權限
                    {
                        status++;
                        string query1 = "UPDATE I_REQUISITIONORDER SET STATUS=3,AUDITOR=:AUDITOR WHERE RID=" + RID;
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = conn;
                        cmd1.Parameters.Add("AUDITOR", OracleDbType.Decimal).Value = aid;
                        cmd1.ExecuteNonQuery();
                    }
                    else //無審核權限，寄出審核信
                    {
                        Models.Smtp smtp = new Models.Smtp(RID, "2");
                    }
                }
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
                    + " WHERE A.RID=B.RID AND A.STATUS >=4 AND B.SEQ=" + words[0] + " AND MID=" + words[1] + " AND RECECIVE_DATE IS NOT NULL"
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

        //刪除資料
        public void DeleteOrder(int id)
        {
            string query = "DELETE FROM I_REQUISITIONORDER WHERE RID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //刪除明細資料
        public void DeleteDetails(int id)
        {
            string query = "DELETE FROM I_REQUISITIONDETAILS WHERE RID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //查詢有審核權限的人
        public Dictionary<int, string> GetAuditLists(string did)
        {
            Dictionary<int, string> audit = new Dictionary<int, string>();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                //檢查部門的群組權限
                string query1 = "SELECT A.AID,A.EMAIL FROM I_USER A, I_UD_REF01 B, I_DG_REF01 C, I_GP_REF01 D "
                    + "WHERE A.ENABLE=1 AND A.AID=B.AID AND B.DID=C.DID AND C.GID=D.GID AND D.PID=" + auditPID + " AND B.DID=" + did;
                OracleCommand cmd1 = new OracleCommand(query1);
                cmd1.Connection = conn;
                OracleDataReader reader1 = cmd1.ExecuteReader();
                if (reader1.HasRows)
                {
                    while (reader1.Read())
                    {
                        if (!audit.ContainsKey(int.Parse(reader1["AID"].ToString())))
                        {
                            audit.Add(int.Parse(reader1["AID"].ToString()), reader1["EMAIL"].ToString());
                        }
                    }
                }

                //檢查部門的程式權限
                string query2 = "SELECT A.AID,A.EMAIL FROM I_USER A, I_UD_REF01 B, I_DP_REF01 C "
                    + "WHERE A.ENABLE=1 AND  A.AID=B.AID AND B.DID=C.DID AND C.PID=" + auditPID + " AND B.DID=" + did;
                OracleCommand cmd2 = new OracleCommand(query2);
                cmd2.Connection = conn;
                OracleDataReader reader2 = cmd2.ExecuteReader();
                if (reader2.HasRows)
                {
                    while (reader2.Read())
                    {
                        if (!audit.ContainsKey(int.Parse(reader2["AID"].ToString())))
                        {
                            audit.Add(int.Parse(reader2["AID"].ToString()), reader2["EMAIL"].ToString());
                        }
                    }
                }

                //檢查個人的群組權限
                string query3 = "SELECT A.AID,A.EMAIL FROM I_USER A, I_UG_REF01 B, I_GP_REF01 C, I_UD_REF01 D "
                    + "WHERE A.ENABLE=1 AND A.AID=B.AID AND B.GID=C.GID AND A.AID=D.AID AND C.PID=" + auditPID + " AND D.DID=" + did;
                OracleCommand cmd3 = new OracleCommand(query3);
                cmd3.Connection = conn;
                OracleDataReader reader3 = cmd3.ExecuteReader();
                if (reader3.HasRows)
                {
                    while (reader3.Read())
                    {
                        if (!audit.ContainsKey(int.Parse(reader3["AID"].ToString())))
                        {
                            audit.Add(int.Parse(reader3["AID"].ToString()), reader3["EMAIL"].ToString());
                        }
                    }
                }

                //檢查個人的程式權限
                string query4 = "SELECT A.AID,A.EMAIL FROM I_USER A, I_UP_REF01 B, I_UD_REF01 C "
                    + "WHERE A.ENABLE=1 AND A.AID=B.AID AND A.AID=C.AID AND B.PID=" + auditPID + " AND C.DID=" + did;
                OracleCommand cmd4 = new OracleCommand(query4);
                cmd4.Connection = conn;
                OracleDataReader reader4 = cmd4.ExecuteReader();
                if (reader4.HasRows)
                {
                    while (reader4.Read())
                    {
                        if (!audit.ContainsKey(int.Parse(reader4["AID"].ToString())))
                        {
                            audit.Add(int.Parse(reader4["AID"].ToString()), reader4["EMAIL"].ToString());
                        }
                    }
                }
            }

            return audit;
        }

    }
}