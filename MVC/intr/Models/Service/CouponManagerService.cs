using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using IntranetSystem.Poco;
using IntranetSystem.Models.Edmx;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Transactions;
using System.Configuration;
using System.Data.SqlClient;
namespace IntranetSystem.Models.Service
{

    public class CouponManagerService
    {

        private static String connString = "Asynchronous Processing=true;"
               + " Pooling=false;User ID=sasa; "
               + " password=sasasa; "
               + " Initial Catalog=Coupon; "
               + " Data Source=10.0.20.207";// + ",1433";
        SqlConnection conn = new SqlConnection(connString);

        private HowardIntraEntities db = new HowardIntraEntities();
        public JObject doSave(String ISUID,String CINO)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            int i = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                String ccid=CINO.Substring(0,2);
                String ientid=CINO.Substring(2,2);
                String seqno=CINO.Substring(4,6);
                String chkno = CINO.Substring(10, 4);


                String sql = "update ci set status='4' ,s5entid=null,s5userid=null,s5date=null,s5dptid=null  where isuid="+ISUID+" and ccid="+ccid+" and ientid='"+ientid+"' and seqno="+seqno+" and chkno="+chkno;

               // String sql1 = "update ci set status='5',s5date='" + sqlFormattedDate + "',s5entid='" + entid + "',s5dptid='" + dptid + "' where ccid='" + ccid + "' and ientid='" + ientid + "' and seqno=" + seqno + " and chkno=" + chkno;
                SqlCommand cmd1 = new SqlCommand(sql, conn);
               i= cmd1.ExecuteNonQuery();

            }
            
            jo.Add("status", i);           
            return jo;
      }


        public JObject doSave1(String ISUID, String CINO)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            int i = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                String ccid = CINO.Substring(0, 2);
                String ientid = CINO.Substring(2, 2);
                String seqno = CINO.Substring(4, 6);
                String chkno = CINO.Substring(10, 4);


                String sql = "select s4xdate,xdy from isucustomer where isuid="+ISUID;
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                String xdate="";
                String xdy="" ;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while ((reader.Read()))
                    {
                        if (!reader[0].Equals(DBNull.Value))
                        {
                            xdate = reader[0].ToString();
                            xdy = reader[1].ToString();
                        }
                    }
                    reader.Close();
                }
                
                String sql1 = "update ci set xdate='"+xdate+"' ,xdy='"+xdy+"'  where isuid=" + ISUID + " and ccid=" + ccid + " and ientid='" + ientid + "' and seqno=" + seqno + " and chkno=" + chkno;

                // String sql1 = "update ci set status='5',s5date='" + sqlFormattedDate + "',s5entid='" + entid + "',s5dptid='" + dptid + "' where ccid='" + ccid + "' and ientid='" + ientid + "' and seqno=" + seqno + " and chkno=" + chkno;
                SqlCommand cmd1 = new SqlCommand(sql, conn);
                i = cmd1.ExecuteNonQuery();

            }
            jo.Add("status", i);
            return jo;
        }


        public JArray getCi(String CINO)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            int i = 0;
                String ccid = CINO.Substring(0, 2);
                String ientid = CINO.Substring(2, 2);
                String seqno = CINO.Substring(4, 6);
                String chkno = CINO.Substring(10, 4);


                String sql = "select isuid,status,s5date,s5entid,s5userid,xdate,xdy from ci where ccid=" + ccid + " and ientid='" + ientid + "' and seqno=" + seqno + " and chkno=" + chkno;          

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while ((reader.Read()))
                        {

                            if (!reader[0].Equals(DBNull.Value))
                            {
                                var itemObject = new JObject
                            {                             
                                {"ISUID",reader[0].ToString() },
                                {"STATUS",reader[1].ToString()},
                                {"S5DATE",reader[2].ToString()},
                                {"S5ENTID",reader[3].ToString()},
                                {"S5USERID",reader[4].ToString()},
                                {"XDATE",reader[5].ToString()},
                                {"XDY",reader[6].ToString()}
                            };
                                ja.Add(itemObject);
                            }
                        }
                        reader.Close();
                    }

                }
                        
            return ja;
        }



        public JObject doApply(CouponITFParamPoco param,String userid)
        {
            int i = 0;
            JObject jo = new JObject();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String sql_chk = "select cino from ci_2016itf where cino_print='" + param.CINO + "' and chkno='" + param.CHKNO + "' and  COALESCE(apply_user,'')=''";
                    SqlCommand cmd_chk = new SqlCommand(sql_chk, conn);
                    SqlDataReader reader_chk = cmd_chk.ExecuteReader();


                    if (reader_chk.HasRows)
                    {
                        reader_chk.Read();
                        String cino = (String)reader_chk[0];
                        reader_chk.Close();

                        String sql_user = "select entid,dptid from appuser where userid='" + param.USERID + "'";
                        SqlCommand cmd_user = new SqlCommand(sql_user, conn);
                        SqlDataReader reader_user = cmd_user.ExecuteReader();

                        if (reader_user.HasRows)
                        {
                            //var obj1 = (from t in db.QRCODE_DTL where t.QRCODE_UUID == obj.QRCODE_UUID group t by new { t.QRCODE_UUID } into g select new {UUID=g.Key.QRCODE_UUID}).SingleOrDefault();


                            reader_user.Read();

                            String entid = (String)reader_user[0];
                            String dptid = (String)reader_user[1];
                            reader_user.Close();

                            String ccid = cino.Substring(0, 2);
                            String ientid = cino.Substring(2, 2);
                            String seqno = cino.Substring(4, 6);
                            String chkno = cino.Substring(10, 4);
                            string sqlFormattedDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            String sql1 = "update ci set status='5',s5date='" + sqlFormattedDate + "',s5entid='" + entid + "',s5dptid='" + dptid + "' where ccid='" + ccid + "' and ientid='" + ientid + "' and seqno=" + seqno + " and chkno=" + chkno;
                            SqlCommand cmd1 = new SqlCommand(sql1, conn);
                            cmd1.ExecuteNonQuery();
                            String sql2 = "update ci_2016itf set apply_user='" + userid + "',apply_dt='" + sqlFormattedDate + "' where cino='" + param.CINO + "' and chkno='" + param.CHKNO + "'";
                            SqlCommand cmd2 = new SqlCommand(sql2, conn);
                            cmd2.ExecuteNonQuery();


                            jo.Add("status", 1);


                        }
                        else
                        {
                            jo.Add("status", 3);
                        }

                    }
                    else
                    {
                        jo.Add("status", 0);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return jo;
        }


        public JObject getApply(PagingParamPoco param)
        {

            String userid = "";
            JArray ja = new JArray();
            JObject jo = new JObject();
            int total=0;
            if (StringUtils.getString(param.search) != "")
            {

                String sql_chk = @"select a.cino,a.cino_print,case when b.status='0' then '未生效' when  b.status='4' then '有效券' when b.status='5' then '已收券' when b.status='9' then '作廢' end status,a.apply_user,a.apply_dt 
              from ci_2016itf a,ci b where a.cino='" + param.search + "' and a.cino = RIGHT(REPLICATE('0', 2) + b.ccid , 2) + b.ientid + RIGHT(REPLICATE('0', 6) + CAST(b.seqno as NVARCHAR)  , 6) + RIGHT(REPLICATE('0', 4) + CAST(b.chkno as NVARCHAR)  , 4)";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(sql_chk, conn);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while ((reader.Read()))
                        {

                            if (!reader[0].Equals(DBNull.Value))
                            {
                                var itemObject = new JObject
                            {                             
                                {"CINO",reader[0].ToString() },
                                {"CINO_PRINT",reader[1].ToString()},
                                {"STATUS",reader[2].ToString()},
                                {"APPLY_USER",reader[3].ToString()},
                                {"APPLY_DT",reader[4].ToString()}
                            };
                                ja.Add(itemObject);
                            }
                        }
                        reader.Close();
                    }

                }
            }
    
            jo.Add("rows", ja);
            jo.Add("total", total);

            return jo;
        }



        public void SendEmail(String subject,String body,String emailTo)
        {
            //設定smtp主機
            //string smtpAddress = "10.0.20.8";
            string smtpAddress = "mail.howard-hotels.com.tw";
            //設定Port
            //int portNumber = 587;
            int portNumber = 25;
            bool enableSSL = false;
            //填入寄送方email和密碼
            string emailFrom = "webmaster@howard-hotels.com.tw";
            string account = "webmaster";
            string password = "center2075";
            //收信方email
            //string emailTo = "someone@domain.com";
            //主旨
            //string subject = "Hello";
            //內容
            //string body = "Hello, I'm just writing this to say Hi!";

            using (MailMessage mail = new MailMessage())
            {
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                
                
                mail.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body + "\n\n";
                mail.Body += "-------------------此信件透過系統發送,請勿直接回覆-------------------";                
                // 若你的內容是HTML格式，則為True
                mail.IsBodyHtml = false;
                
                //夾帶檔案
                //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));
                //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.UseDefaultCredentials = true;    
                    
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                    
                }
            }
        }




    }
}