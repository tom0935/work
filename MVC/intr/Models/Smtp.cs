using System.Net.Mail;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Collections.Generic;

namespace IntranetSystem.Models
{
    public class Smtp
    {
        //DB連線字串
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
        //耗材審核的PID
        private int auditPID = int.Parse(ConfigurationManager.AppSettings["RequisitionAudit"].ToString());
        //管理員信箱
        private string adminMail = ConfigurationManager.AppSettings["AdminMail"].ToString();


        string nbody = string.Empty;
        string mailTo = string.Empty;
        string subject = string.Empty;
        string subject1 = string.Empty;
        string body = string.Empty;
        string status = string.Empty;

        public Smtp(string rid, string orderStatus)
        {
            //Models物件
            Models.RequisitionOrderDataContext order = new Models.RequisitionOrderDataContext();

            string query = "SELECT DID,CREATOR,CNAME,REMARK,CHKNO FROM I_REQUISITIONORDER WHERE STATUS=" + orderStatus + " AND RID=" + rid;
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    switch (orderStatus)
                    {
                        case "2":
                            subject1 = "請求審核";
                            status = "<font color='darkred'>待審中</font>";
                            break;
                        case "3":
                            subject1 = "審核通過";
                            status = "<font color='blue'>已審核通過，可領料</font>";
                            mailTo = Models.Helper.getFieldValue("EMAIL", "I_USER", "AID=" + reader["CREATOR"]);
                            break;
                        case "0":
                            subject1 = "取消申請";
                            status = "<font color='red'>已取消申請</font>";
                            mailTo = Models.Helper.getFieldValue("EMAIL", "I_USER", "AID=" + reader["CREATOR"]);
                            break;
                    }
                    subject = "耗材申請單[" + rid.PadLeft(8, '0') + "] - " + subject1;
                    body = "<p>申請單號：<strong>" + rid.PadLeft(8, '0') + "</strong></p>"
                        + "<p>申請人：<strong>" + reader["CNAME"] + "</strong></p>"
                        + "<p>" + order.GetMaterialListView(reader["CREATOR"].ToString(), reader["DID"].ToString(), int.Parse(rid))
                        + "<br>申請單備註：" + reader["REMARK"] + "</p>"
                        + "<p>狀態：" + status + "</p>";

                    if (orderStatus == "2")
                    {
                        //找出部門有審核權限的人,寄出信件
                        string nbody = string.Empty;
                        Dictionary<int, string> auditor = order.GetAuditLists(reader["DID"].ToString());
                        foreach (var item in auditor)
                        {
                            nbody = body + "<p><a href='http://intra.howard-hotels.com.tw/intra/RequisitionAudit/Allow/"
                                + rid + "?chkno=" + reader["CHKNO"]
                                + "&aid=" + item.Key + "' title='通過' style='color: green;'>申請通過</a>　或　"
                                + "<a href='http://intra.howard-hotels.com.tw/intra/RequisitionAudit/Deny/"
                                + rid + "?chkno=" + reader["CHKNO"] + "&aid="
                                + item.Key + "' title='取消' style='color: red;'>取消申請</a></p>";

                            //寄送E-MAIL
                            if (item.Value != null && item.Value != "")
                                SendTonerMail(item.Value, nbody, subject);
                        }
                    }
                    else
                    {
                        //回覆通知信件
                        SendTonerMail(mailTo, body, subject);
                    }
                }
            }
        }

        private void SendTonerMail(string receiver, string body, string subject)
        {
            //設定smtp主機
            SmtpClient mySmtp = new SmtpClient("10.0.20.8");
            //設定smtp帳密
            //mySmtp.Credentials = new System.Net.NetworkCredential("master", "center");
            //信件內容
            body = "<html>\n"
                + "<head>"
                + "<meta http-equiv='Content-Type' content='text/html' charset='utf-8' />"
                + "<title>耗材申請通知</title>"
                + "<style type='text/css'>"
                + "table,td,th { border:1px solid #DDDDDD; border-collapse:collapse; border-spacing:0;}"
                + "td,th { padding: 5px; }"
                + "</style>"
                + "</head>\n"
                + "<body>" + body + "</body>\n"
                + "</html>";
            string pcontect = body;
            //設定mail內容
            MailMessage msgMail = new MailMessage();
            //寄件者
            if (!string.IsNullOrEmpty(this.adminMail))
            {
                msgMail.From = new MailAddress(this.adminMail, "內部系統通知信");
            }
            else
            {
                msgMail.From = new MailAddress("webmaster@howard-hotels.com.tw", "內部系統通知信");
            }
            //收件者
            msgMail.To.Add(receiver);
            //主旨
            msgMail.Subject = subject;
            //信件內容(含HTML時)
            AlternateView alt = AlternateView.CreateAlternateViewFromString(pcontect, null, "text/html");
            msgMail.AlternateViews.Add(alt);
            //寄mail
            mySmtp.Send(msgMail);
        }
    }
}