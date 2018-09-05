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
    public class QRCodeApplyService
    {
        private HowardIntraEntities db = new HowardIntraEntities();
        public JObject chkApply(QRCODE_COUPON param)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = (from t in db.QRCODE_COUPON 
                        where t.CODE==param.CODE && t.ADT == null
                         select t).SingleOrDefault();

            if (query!=null)
            {
                query.ADT = System.DateTime.Now;
                query.AUSER = param.AUSER;
                 
                jo.Add("status",1);
            }
            else
            {
                jo.Add("status", 0); //無此券號或已收券
            }
           
            return jo;
      }


        public int doCheck(QRCODE_COUPON param, String code)
        {
            int i = 0;
          

            
            var obj = (from t in db.QRCODE_APPLY_DEPART where t.QRCODE_UUID == param.QRCODE_UUID && (t.CODE == code || t.CODE =="*") select t).SingleOrDefault();
            if (obj != null)
            {
                i = 1;
            }
            else
            {
                i = 3;
            }


            return i;
        }


        public JObject doApply(QRCODE_COUPON param,String userid)
        {
            int i = 0;
            JObject jo = new JObject();


            var obj = (from t in db.QRCODE_COUPON where t.CODE == param.CODE && t.AUSER == null select t).SingleOrDefault();
            if (obj != null)
            {
                //var obj1 = (from t in db.QRCODE_DTL where t.QRCODE_UUID == obj.QRCODE_UUID group t by new { t.QRCODE_UUID } into g select new {UUID=g.Key.QRCODE_UUID}).SingleOrDefault();

                param.QRCODE_UUID = obj.QRCODE_UUID;
                if (doCheck(param, userid) == 1)
                {
                    obj.AUSER = param.AUSER;
                    obj.ADT = System.DateTime.Now;
                    i = db.SaveChanges();
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
            
            
            return jo;
        }


        public JObject getApply(PagingParamPoco param)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            int total=0;
            if (StringUtils.getString(param.search) != "")
            {
            var query = (from t in db.QRCODE_COUPON
                        join a in db.QRCODE_DTL on t.QRCODE_UUID equals a.UUID_DTL into aa
                        from a in aa.DefaultIfEmpty()
                        join c in db.QRCODE on a.QRCODE_UUID equals c.UUID into cc
                        from c in cc.DefaultIfEmpty()
                        join b in db.QRCODE_CUS on t.CUS_UUID equals b.UUID into bb
                        from b in bb.DefaultIfEmpty()
                         //where b.CNAME.Contains(param.search) || b.EMAIL.Contains(param.search) || b.TEL.Contains(param.search) || t.CODE.Contains(param.search)
                        select new {t.CODE,t.CDT,t.ADT,t.AUSER,c.SUBJECT,c.SDT,c.EDT, b.CNAME,b.TEL,b.EMAIL });


            
    //        param.search = "test";
             
           var q1 = query.Where(q => q.EMAIL.Contains("" + param.search + ""));

           if (q1.Count() == 0)
           {
               var q2 = query.Where(q => q.CNAME.Contains("" + param.search + ""));
               if (q2.Count() > 0)
               {
                   query = q2;
               }
           }
           else
           {
               query = q1;
           }

           
            if (query.Count() == 0)
            {
                var q3 = query.Where(q => q.CODE.Contains("" + param.search + ""));
                if (q3.Count() > 0)
                {
                    query = q3;
                }
            }
            if (query.Count() == 0)
            {
                var q4 = query.Where(q => q.TEL.Contains("" + param.search + ""));
                if (q4.Count() > 0)
                {
                    query = q4;
                }
            }
            //query = query.Where(q => param.search.Contains(""+q.TEL+"") || param.search.Contains(""+q.CODE+"") || param.search.Contains(""+q.CNAME+"") || param.search.Contains(""+q.EMAIL+""));
      //      query = query.Where(q => q.TEL.Contains(param.search) || q.CODE.Contains(param.search) || q.CNAME.Contains(param.search) || q.EMAIL.Contains(param.search));
            
            //query = query.OrderByDescending(q => q.UUID);

            total = query.Count();
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip(param.offset).Take(param.limit);    //分頁    

           // jo.Add("query", param.search);
            foreach (var item in query)
            {
                String tel = StringUtils.getString(item.TEL);
                if (tel.Length > 6)
                {
                    tel = tel.Substring(0, 7) + "xxx";
                }
                var itemObject = new JObject
                       {   
                          {"CODE",StringUtils.getString(item.CODE)},
                          {"SUBJECT",StringUtils.getString(item.SUBJECT)},                          
                          {"CDT",String.Format("{0:yyyy-MM-dd}", item.CDT)},
                          {"ADT",String.Format("{0:yyyy-MM-dd}", item.ADT)},
                          {"AUSER",StringUtils.getString(item.AUSER)},
                          {"CNAME",StringUtils.getString(item.CNAME)},
                          {"TEL",tel},
                          {"EMAIL",StringUtils.getString(item.EMAIL)},                                                                                                        
                          {"SDT",String.Format("{0:yyyy-MM-dd}", item.SDT)},
                          {"EDT",String.Format("{0:yyyy-MM-dd}", item.EDT)}                          
                       };
                ja.Add(itemObject);
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