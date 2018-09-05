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

namespace IntranetSystem.Models.Service
{
    public class QRCodeQAService
    {
        private HowardIntraEntities db = new HowardIntraEntities();
        public JObject getQRCodeQA(PagingParamPoco param)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE_QA 
                        //join r in db.QRCODE_QA_REPLY on t.UUID equals r.QA_UUID
                        select t;
                        

            if (StringUtils.getString(param.search) != "")
            {
                var q1 = query.Where(q => q.QUESTION.Contains(""+param.search + "") );
                if (q1.Count() == 0)
                {
                    var q2 = query.Where(q => q.CNAME.Contains(""+param.search+""));
                    if (q2.Count() > 0)
                    {
                        query = q2;
                    }
                }else{
                    query=q1;
                }

                if (query.Count() == 0)
                {
                    var q2 = query.Where(q => q.EMAIL.Contains("" + param.search + ""));
                    if (q2.Count() > 0)
                    {
                        query = q2;
                    }
                }
                if (query.Count() == 0)
                {
                    var q2 = query.Where(q => q.TEL.Contains("" + param.search + ""));
                    if (q2.Count() > 0)
                    {
                        query = q2;
                    }
                }


            }


               
            query = query.OrderByDescending(q => q.UUID);
            
            var total = query.Count();
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip(param.offset).Take(param.limit);    //分頁    
           

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {  
                          {"QRCODE_DTL_UUID",StringUtils.getString(item.QRCODE_DTL_UUID)},
                          {"UUID",StringUtils.getString(item.UUID)},
                          {"CNAME",StringUtils.getString(item.CNAME)},
                          {"DT",StringUtils.getString(item.DT)},
                          {"RDT",StringUtils.getString(item.RDT)},
                          {"EMAIL",StringUtils.getString(item.EMAIL)},
                          {"QUESTION",StringUtils.getString(item.QUESTION)},
                          {"ANSWER",StringUtils.getString(item.ANSWER)},
                          {"TEL",StringUtils.getString(item.TEL)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);
            return jo;
      }



        public JObject getQRCodeQAReply(int UUID)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE_QA_REPLY 
                        where t.QA_UUID == UUID
                        select t;
                        
              
            query = query.OrderBy(q => q.RDT);
            
            var total = query.Count();
           

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {   
                          {"UUID",StringUtils.getString(item.UUID)},
                          {"CONTENT",StringUtils.getString(item.CONTENT)},
                          {"RDT",StringUtils.getString(item.RDT)},
                          {"RUSER",StringUtils.getString(item.RUSER)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);
            return jo;
      }

        


        public int doReply(QRCODE_QA param)
        {
            int i = 0;
            var obj = (from t in db.QRCODE_QA where t.UUID == param.UUID select t).SingleOrDefault();


            if (obj != null)
            {
                if (StringUtils.getString(obj.ANSWER) != "")
                {
                    QRCODE_QA_REPLY qr = new QRCODE_QA_REPLY();
                    qr.CONTENT = param.ANSWER;
                    qr.QA_UUID = param.UUID;
                    qr.RDT = System.DateTime.Now;
                    qr.RUSER = param.RUSER;
                    db.QRCODE_QA_REPLY.AddObject(qr);
                }
                else
                {
                    obj.ANSWER = param.ANSWER;
                    obj.RDT = System.DateTime.Now;
                    obj.RUSER = param.RUSER;
                }
                i = db.SaveChanges();
            }
            
            return i;
        }


        public String getSubject(int UUID)
        {
            String subject = "";
            var obj = (from t in db.QRCODE where t.UUID == UUID select t).SingleOrDefault();
            if (obj != null)
            {
                subject = obj.SUBJECT.ToString();
            }
            else
            {
                subject = "";
            }
            return subject;

        }


        public DataTable getDataTableByQRCode(String SDT,String EDT,String SUBJECT)
        {
            LinqExtensions le = new LinqExtensions();
            var query = from t in db.QRCODE
                        join d in db.QRCODE_DTL on t.UUID equals d.QRCODE_UUID into dd
                        from d in dd.DefaultIfEmpty()
                        where d.ISSHOW_DTL == "1" && t.ISSHOW == "1"
                        select new { d.UUID_DTL, d.URL_DTL,d.SUBJECT_DTL, d.CNT_DTL,  t.EDMURL, t.SUBJECT,t.UUID,d.CREATE_DT_DTL };

            if (SUBJECT != "*")
            {
                query = query.Where(q => q.SUBJECT == SUBJECT);
            }
            if (StringUtils.getString(SDT) != "" && StringUtils.getString(EDT) != "")
            {
                DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
                DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
                query = query.Where(q => q.CREATE_DT_DTL >= sdt && q.CREATE_DT_DTL <= edt);                
            }

            DataTable dt = le.LinqQueryToDataTable(query);
            return dt;
        }


        public void SendEmail(String subject,String question,String body,String emailTo,int? QRCODE_DTL_UUID)
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
                mail.Body = "---此信件為系統發送，請勿直接回覆，若欲提問請點選下方連結或洽詢02-2700-2323轉訂席中心---<br>";
                mail.Body += "<a href='http://reservation.howard-hotels.com.tw/WebBooking/index.php/QRCode/index/" + QRCODE_DTL_UUID + "'>https://reservation.howard-hotels.com.tw/WebBooking/index.php/QRCode/index/" + QRCODE_DTL_UUID + "</a><br><br>";
               // mail.Body += "詢問內容:\n" + question+ "\n\n";
                mail.Body += "貴賓您好，謝謝您來信台北福華大飯店，針對您所提出之疑問說明如下，<br>" + body + "<br><br><br>";
                mail.Body += "再次謝謝您的來信，並期望能盡快為您服務。<br>";
                mail.Body += "台北福華大飯店The Howard Plaza Hotel Taipei<br>";
                mail.Body += "訂席中心Reservation Center<br>";
                mail.Body += "<a href='mailto:breservation-tp@howard-hotels.com.tw'>breservation-tp@howard-hotels.com.tw</a><br>";
                // 若你的內容是HTML格式，則為True
                mail.IsBodyHtml = true;
                
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