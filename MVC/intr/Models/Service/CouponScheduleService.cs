
using System.Configuration;
using System.Transactions;
using System;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Dynamic;
using System.Linq;
using IntranetSystem.Models;
using System.ServiceModel;
using System.Net;

using System.Text;
using IntranetSystem.Poco;

using System.Data.SqlClient;
using System.Transactions;
using System.Configuration;
using System;
using System.Data.OleDb;

using Quartz;
using IntranetSystem.Models.Edmx;
using System.IO;
using System.Net.Mail;




namespace IntranetSystem.Service
{
    public class CouponScheduleService:IJob
{        


    private void Log(string msg)
    {
        System.IO.File.AppendAllText(@"C:\Temp\CouponSchedulelog.txt",System.DateTime.Now +": "+ msg + Environment.NewLine);
    }


    public void Execute(IJobExecutionContext context)
    {
        int i = 0;
            String mm=System.DateTime.Now.Month.ToString();
            String dd = (System.DateTime.Now.Day - 1).ToString();
            String ISU = "";
            String CINO = "";
             if(mm.Length==1){
                 mm="0"+mm;
             }
             if(dd.Length==1){
                 dd="0"+dd;
             }



            //String ymd=System.DateTime.Now.Year.ToString() + mm + dd;
             String ymd = (System.DateTime.Now.AddDays(-1)).ToString("yyyyMMdd");
            // String ymd = (System.DateTime.Now.Day - 1).ToString("yyyyMMdd");
           String filename=ymd + ".csv";
            String connString = "Asynchronous Processing=true;"
                   + " Pooling=false;User ID=sasa; "
                   + " password=sasasa; "
                   + " Initial Catalog=Coupon; "
                   + " Data Source=10.0.20.207";// + ",1433";
            SqlConnection conn = new SqlConnection(connString);

       try{
            getFile("D:\\coupon\\" + filename, filename);

            List<Hashtable> list = getStringList("D:\\coupon\\" + filename);
          conn.Open();
        //select * from ci where  isuid='385' and seqno='10495'      

           int s4=0;
           int s9=0;
           int cnt = 1;
           String impstr = "<table width='450' border='1' cellpadding='3' cellspacing='0' bordercolor='#CCCCCC'><tr><td width='53' bgcolor='#CCCCCC'><div align='center'>No.</div></td>";
           impstr += "<td width='190' bgcolor='#CCCCCC'><div align='center'>條碼編號</div></td><td width='99' bgcolor='#CCCCCC'><div align='center'>申請單號</div></td><td width='58' bgcolor='#CCCCCC'><div align='center'>票券狀態</div></td></tr>";
           foreach(Hashtable ht in  list){
               String cino = HashtableUtil.getValue(ht, "HOWARD_COUPON_NO");     //條碼編號
               String status = HashtableUtil.getValue(ht, "STATUS"); //票券狀態
               String isuid = HashtableUtil.getValue(ht, "CUS_NO");   //申請單編號
               ISU = isuid;
               CINO = cino;
               String ccid = cino.Substring(0,2);
               String ientid = cino.Substring(2, 2);
               String seqno = cino.Substring(4, 6);
               String chkno = cino.Substring(10, 4);
               //impstr += cino + ","+isuid +","+status + "<br>";

               String sql_chk = "select seqno from ci where isuid=" + isuid + " and seqno='"+seqno+"' and chkno='"+chkno+"' and ccid="+ccid+" and status in('5','9')";
               SqlCommand cmd_chk = new SqlCommand(sql_chk, conn);
               SqlDataReader reader_chk = cmd_chk.ExecuteReader();
               if (reader_chk.HasRows)
               {
                   reader_chk.Read();
                   int count = (int)reader_chk[0];

                   //statusTxt.Text = "轉檔失敗,轉入申請單編號:" + isuid + "資料中有已收券或作廢資料,無法進行轉入!";
                   //   SendEmail("墨攻票券系統轉入失敗,請立即處理","券號:"+ cino + "轉檔失敗,轉入申請單編號:" + isuid + "資料中有已收券或作廢資料,無法進行轉入!請儘速確認狀況");
                   //MessageBox.Show("轉檔失敗,轉入申請單編號:" + isuid + "資料中有已收券或作廢資料,無法進行轉入!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   Log("轉檔失敗,券號:" + cino + "轉入申請單編號:" + isuid + "資料中有已收券或作廢資料,無法進行轉入!請儘速確認狀況");
                   reader_chk.Close();
                   continue;
               }
               else
               {
                   reader_chk.Close();
               }

               


               
               String sql_chk2 = "select HOWARD_COUPON_NO from MOHIST where HOWARD_COUPON_NO='" + cino + "'";
               SqlCommand cmd_chk2 = new SqlCommand(sql_chk2, conn);
               SqlDataReader reader_chk2 = cmd_chk2.ExecuteReader();
               if (!reader_chk2.HasRows)
               {


                   
                   
                   String cancel_dt = StringUtils.getString(HashtableUtil.getValue(ht, "CANCEL_DT"));
                   String use_dt = StringUtils.getString(HashtableUtil.getValue(ht, "USE_DT"));
                   String print_dt = StringUtils.getString(HashtableUtil.getValue(ht, "PRINT_DT"));

                   reader_chk2.Close();


                   //更新CI
                   String sql = "";
                   impstr += "<tr><td><div align='center'>" + cnt + "</div></td><td><div align='center'>" + cino + "</div></td> <td><div align='center'>" + isuid + "</div></td><td><div align='center'>" + status + "</div></td></tr>";
                   if (status == "1")
                   {
                       //若為有效券,將售出票券改為狀態4(有效)
                       sql = "update ci set status='4' where ccid=" + ccid + " and ientid='" + ientid + "' and seqno='" + seqno + "' and chkno='" + chkno + "' and isuid=" + isuid + " and status='0'";
                       s4++;
                   }
                   else if (status == "3")
                   {
                       sql = "update ci set status='0',s9userid='system',s9date=getdate() where ccid=" + ccid + " and ientid='" + ientid + "' and seqno='" + seqno + "' and chkno='" + chkno + "' and isuid=" + isuid + " and status='0'";
                       s9++;
                   }

                   SqlCommand cmd = new SqlCommand(sql, conn);
                   cmd.ExecuteNonQuery();
                   //end 



                   //墨攻轉入備存
                   String sql1 = "insert MOHIST (ORDER_NO,CUS_NO,COUPON_NO,CNAME,PAY_TYPE,CARD_NO,CARD_TYPE,AUTH_NO,PRICE,PRINT_DT,USE_DT,CANCEL_DT,RECYCLY_USER,PRINT_USER,SALE_USER,INVOICE_NO,STATUS,IMPORT_DT,HOWARD_COUPON_NO)values " +
                                 "('" + HashtableUtil.getValue(ht, "ORDER_NO") + "','" + HashtableUtil.getValue(ht, "CUS_NO") + "','" + HashtableUtil.getValue(ht, "COUPON_NO") + "','" + HashtableUtil.getValue(ht, "CNAME") +
                                 "','" + HashtableUtil.getValue(ht, "PAY_TYPE") + "','" + HashtableUtil.getValue(ht, "CARD_NO") + "','" + HashtableUtil.getValue(ht, "CARD_TYPE") + "','" + HashtableUtil.getValue(ht, "AUTH_NO") + "','" + HashtableUtil.getValue(ht, "PRICE") +
                                 "','" + print_dt + "','" + use_dt + "','" + cancel_dt + "','" + HashtableUtil.getValue(ht, "RECYCLY_USER") + "','" +
                   HashtableUtil.getValue(ht, "PRINT_USER") + "','" + HashtableUtil.getValue(ht, "SALE_USER") + "','" + HashtableUtil.getValue(ht, "INVOICE_NO") + "','" + HashtableUtil.getValue(ht, "STATUS") + "',getdate() ," +
                   "'" + HashtableUtil.getValue(ht, "HOWARD_COUPON_NO") + "')";
                   SqlCommand cmd1 = new SqlCommand(sql1, conn);
                   cmd1.ExecuteNonQuery();
               }
               else
               {
                   reader_chk2.Close();
               }
               
               cnt++;
           }
           impstr +="</table>";
           SendEmail("墨攻票券轉入成功通知,有效:" + s4 + "張,作廢:" + s9 + "張", "<br><br>轉入明細:<br>" + impstr);
           Log("墨攻票券轉入成功,有效:" + s4 + "張,作廢:" + s9 + "張");
	     }catch (Exception ex) 
	     {
          //   if (ex.Message.IndexOf("UNIQUE KEY 違反條件約束") < 0)
          //   {
                 Log(ex.Message);
                 SendEmail("墨攻票券系統轉入失敗,請立即查明原因處理", "申請編號:" + ISU + ",條碼:"+CINO+",error message:" + ex.Message);
          //   }
	     }finally{ 	 
	         conn.Close(); 	 	 
	     }                     


    }


    private List<Hashtable> getStringList(String path)
    {
        IEnumerable<string> lines_B = File.ReadLines(path, Encoding.Default);
        List<Hashtable> list = new List<Hashtable>();
        
        foreach (var line in lines_B)
        {
            Hashtable h = new Hashtable();
            String[] aryS = line.Split(',');
            h.Add("ORDER_NO", aryS[0]);
            h.Add("CUS_NO", aryS[1]);
            h.Add("COUPON_NO", aryS[2]);
            h.Add("CNAME", aryS[3]);
            h.Add("PAY_TYPE", aryS[4]);
            //h.Add("TRANS_NO", aryS[5]);
            h.Add("CARD_NO", aryS[5]);
            h.Add("CARD_TYPE", aryS[6]);
            h.Add("AUTH_NO", aryS[7]);
            h.Add("STATUS", aryS[8]);
            h.Add("PRICE", aryS[9]);
            h.Add("PRINT_DT", aryS[10]);
            h.Add("USE_DT", aryS[11]);
            h.Add("CANCEL_DT", aryS[12]);
            //h.Add("CHECK_DT", aryS[14]);
            h.Add("RECYCLY_USER", aryS[13]);
            h.Add("PRINT_USER", aryS[14]);
            h.Add("SALE_USER", aryS[15]);
            h.Add("INVOICE_NO", aryS[16]);
            //h.Add("OVERCANCEL_DT", aryS[19]);
            h.Add("HOWARD_COUPON_NO", aryS[17]);

            //h.Add("cino", aryS[0]);
            //h.Add("isuid", aryS[1]);
            //h.Add("status", aryS[2]);            
            list.Add(h);            
        }
        return list;
    }


        //取回檔案
        /*
   private void getFile(String  localpath,String filename){
            // SFTP下載
     
           string destFile = "/var/www/html/HW2011/beta/upload/" + filename;
           Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.211", "22", "root", "mka89444");
        // sftp.Put(srcFile, localpath);
           sftp.Get(destFile, localpath);
         
           //sftp.GetFileList
     
   }*/
    private void getFile(String localpath, String filename)
    {

        try
        {
            String path = "ftp://www.mohist.com.tw/" + filename;

            FtpWebRequest requestFileDownload = (FtpWebRequest)WebRequest.Create(new Uri(path ));
            

            //FtpWebRequest requestFileDownload = (FtpWebRequest)WebRequest.Create("ftp://210.241.131.225/" + filename);
            requestFileDownload.Credentials = new NetworkCredential("HDH", "HDHadmin");
            requestFileDownload.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse responseFileDownload = (FtpWebResponse)requestFileDownload.GetResponse();

            Stream responseStream = responseFileDownload.GetResponseStream();
            FileStream writeStream = new FileStream(localpath , FileMode.Create);

            int Length = 2048;
            Byte[] buffer = new Byte[Length];
            int bytesRead = responseStream.Read(buffer, 0, Length);

            while (bytesRead > 0)
            {               
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = responseStream.Read(buffer, 0, Length);
            }
            responseStream.Close();
            writeStream.Close();
            requestFileDownload = null;
            responseFileDownload = null;
        }
        catch (Exception ex)
        {
            Log(ex.Message);
        }


    }   


    private void SendEmail(String subject, String body)
    {
        //設定smtp主機
        string smtpAddress = "10.0.20.8";
       // string smtpAddress = "mail.howard-hotels.com.tw";
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
            mail.To.Add("tomtsao-tp@howard-hotels.com.tw");                        
            mail.Subject = subject;            
            mail.Body = body;
            mail.Body += "<br>---此信件為系統發送，請勿直接回覆---<br>";
            // 若你的內容是HTML格式，則為True
            mail.IsBodyHtml = true;


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
