using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class IncomingOrder
    {
        public decimal RN { get; set; }
        public decimal IID { get; set; }
        public decimal CREATOR { get; set; }
        public string CNAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string REMARK { get; set; }
        public int STATUS { get; set; }
        public string STATUS_TEXT { get; set; }
        public decimal UPDATE_BY { get; set; }
        public string UNAME { get; set; }
        public string UPDATE_DATE { get; set; }
    }


    public class IncomingOrderDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<IncomingOrder> LoadOrder(int pageSize, int page)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            string query = "SELECT * FROM (SELECT ROWNUM RN, IID, CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, REMARK, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE FROM (SELECT * FROM I_INCOMINGORDER ORDER BY IID DESC) ) WHERE RN BETWEEN " + startRN + " AND " + endRN;

            List<IncomingOrder> orders = new List<IncomingOrder>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IncomingOrder order = new IncomingOrder();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = order.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(order, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }
                        switch (order.STATUS)
                        {
                            default:
                            case 1:
                                order.STATUS_TEXT = "新單";
                                break;
                            case 2:
                                order.STATUS_TEXT = "已入庫";
                                break;
                        }

                        orders.Add(order);
                    }
                }
                reader.Close();
            }
            return orders;
        }

        //建立進貨單
        public IncomingOrder CreateOrder(string aid)
        {
            //取得使用者姓名
            string cname = Models.Helper.getFieldValue("NAME", "I_USER", "AID=" + aid);
            string query = "INSERT INTO I_INCOMINGORDER (CREATOR, CNAME, STATUS, UPDATE_BY, UNAME) VALUES(:CREATOR, :CNAME, :STATUS, :UPDATE_BY, :UNAME)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("CREATOR", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("CNAME", OracleDbType.Varchar2).Value = cname;
                cmd.Parameters.Add("STATUS", OracleDbType.Decimal).Value = 1; //新單
                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Decimal).Value = aid;
                cmd.Parameters.Add("UNAME", OracleDbType.Varchar2).Value = cname;
                cmd.ExecuteNonQuery();
            }

            //取得進貨單主檔資訊
            IncomingOrder order = new IncomingOrder();
            order.IID = decimal.Parse(Models.Helper.getFieldValue("MAX(IID)", "I_INCOMINGORDER", "CREATOR=" + aid));
            order.CREATOR = decimal.Parse(aid);
            order.CNAME = cname;
            order.CREATE_DATE = Models.Helper.getFieldValue("TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI')", "I_INCOMINGORDER", "IID=" + order.IID.ToString());
            order.STATUS = 1;
            order.UPDATE_BY = order.CREATOR;
            order.UNAME = order.CNAME;
            order.UPDATE_DATE = order.CREATE_DATE;
            return order;
        }

        //更新主檔資料
        public void UpdateOrder(string IID, string REMARK, string STATUS)
        {
            string query = "UPDATE I_INCOMINGORDER SET REMARK=:REMARK, STATUS=:STATUS WHERE IID=" + IID;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = REMARK;
                cmd.Parameters.Add("STATUS", OracleDbType.Int16).Value = int.Parse(STATUS);
                cmd.ExecuteNonQuery();
            }
        }

        //新增物料明細檔
        public void InsertOrderDetails(string IID, string MID, string QTY)
        {
            string query = "INSERT INTO I_INCOMINGDETAILS (IID, MID, MNAME, QTY) VALUES(:IID, :MID, :MNAME, :QTY)";
            string MNAME = Models.Helper.getFieldValue("NAME", "I_MATERIAL", "MID=" + MID);

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("IID", OracleDbType.Decimal).Value = decimal.Parse(IID);
                cmd.Parameters.Add("MID", OracleDbType.Decimal).Value = decimal.Parse(MID);
                cmd.Parameters.Add("MNAME", OracleDbType.Varchar2).Value = MNAME;
                cmd.Parameters.Add("QTY", OracleDbType.Decimal).Value = decimal.Parse(QTY);
                cmd.ExecuteNonQuery();
            }
        }


        //編輯進貨單
        public IncomingOrder EditOrder(int id)
        {

            string query = "SELECT CREATOR, CNAME, TO_CHAR(CREATE_DATE,'YYYY-MM-DD HH24:MI') CREATE_DATE, STATUS, UPDATE_BY, UNAME, TO_CHAR(UPDATE_DATE,'YYYY-MM-DD HH24:MI') UPDATE_DATE, REMARK FROM I_INCOMINGORDER WHERE IID=" + id.ToString();

            //進貨單主檔資訊
            IncomingOrder order = new IncomingOrder();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    order.IID = id;
                    order.CREATOR = decimal.Parse(reader["CREATOR"].ToString());
                    order.CNAME = reader["CNAME"].ToString();
                    order.CREATE_DATE = reader["CREATE_DATE"].ToString();
                    order.STATUS = int.Parse(reader["STATUS"].ToString());
                    switch (order.STATUS)
                    {
                        default:
                        case 1:
                            order.STATUS_TEXT = "新單";
                            break;
                        case 2:
                            order.STATUS_TEXT = "已入庫";
                            break;
                    }
                    order.UPDATE_BY = decimal.Parse(reader["UPDATE_BY"].ToString());
                    order.UNAME = reader["UNAME"].ToString();
                    order.UPDATE_DATE = reader["UPDATE_DATE"].ToString();
                    order.REMARK = reader["REMARK"].ToString();
                }
            }
            return order;
        }


        //刪除進貨單資料
        public void DeleteIncomingOrder(int id)
        {
            string query = "DELETE FROM I_INCOMINGORDER WHERE IID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //刪除進貨明細資料
        public void DeleteIncomingDetails(int id)
        {
            string query = "DELETE FROM I_INCOMINGDETAILS WHERE IID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //取得物料類型
        public string TypelLists(string selected = "")
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
                    while (reader.Read())
                    {
                        if (reader["TID"].ToString() == selected)
                            lists += "<option value='" + reader["TID"] + " selected'> " + reader["NAME"] + " </option>";
                        else
                            lists += "<option value='" + reader["TID"] + "'> " + reader["NAME"] + " </option>";
                    }
                }
            }

            return lists;
        }

        //取得物料明細
        public string Details(int id)
        {
            string lists = string.Empty;
            string query = "SELECT MID,MNAME,QTY FROM I_INCOMINGDETAILS WHERE IID=" + id.ToString() + " ORDER BY MID";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    int i = 1;
                    while (reader.Read())
                    {
                        lists += "<tr>"
                            + "<td>" + i.ToString() + "</td>"
                            + "<td>" + Models.Helper.getFieldValue("CODE", "I_MATERIAL", "MID=" + reader["MID"].ToString()) + "</td>"
                            + "<td>" + reader["MNAME"] + "</td>"
                            + "<td><input type='number' class='form-control' id='" + reader["MID"] + "' name='" + reader["MID"] + "' value='" + reader["QTY"] + "'></td>"
                            + "<td>"
                            + "<button type='button' class='btn btn-info' onclick='plusMaterial(" + reader["MID"].ToString() + ");'>＋</button>"
                            + "<button type='button' class='btn btn-warning' onclick='minusMaterial(" + reader["MID"].ToString() + ");'>－</button>"
                            + "<button type='button' class='btn btn-danger' onclick='rmMaterial(this);'>整筆移除</button>"
                            + "</td>"
                            + "<tr>";
                        i++;
                    }
                }
            }

            return lists;
        }

        //取得物料明細
        public string DetailView(int id)
        {
            string lists = string.Empty;
            string query = "SELECT MID,MNAME,QTY FROM I_INCOMINGDETAILS WHERE IID=" + id.ToString() + " ORDER BY MID";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    int i = 1;
                    while (reader.Read())
                    {
                        lists += "<tr>"
                            + "<td>" + i.ToString() + "</td>"
                            + "<td>" + Models.Helper.getFieldValue("CODE", "I_MATERIAL", "MID=" + reader["MID"].ToString()) + "</td>"
                            + "<td>" + reader["MNAME"] + "</td>"
                            + "<td>" + reader["QTY"] + "</td>"
                            + "<tr>";
                        i++;
                    }
                }
            }

            return lists;
        }

    }
}