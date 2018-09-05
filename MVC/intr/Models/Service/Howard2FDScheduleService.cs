
using System.Configuration;
using System.Transactions;
using System;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using IntranetSystem.Models;
using System.ServiceModel;
using System.Net;

using System.Text;
using IntranetSystem.Poco;



using Quartz;
using System.IO;
using System.Net.Mail;
using Oracle.DataAccess.Client;
using MySql.Data.MySqlClient;




namespace IntranetSystem.Service
{

    public class Howard2FDScheduleService:IJob
{
         static String connString = "Server=10.0.20.221;Database=MHotelClub;Uid=intra;Pwd=center2075;charset=utf8;Allow User Variables=True;Connect Timeout=600";

    private String oradb = "Data Source=(DESCRIPTION="
                 + "(ADDRESS=(PROTOCOL=TCP)(HOST=10.0.40.5)(PORT=1521))"
                 + "(CONNECT_DATA=(SERVICE_NAME=ORCL)));"
                 + "User Id=onlinerv;Password=onlinerv;";

        private void Log(string msg)
        {
            System.IO.File.AppendAllText(@"C:\Temp\HowardWeb2Frontdesk_log.txt", System.DateTime.Now + ": " + msg + Environment.NewLine);
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


    public void Execute(IJobExecutionContext context)
    {
        try
        {
            doAdd();
            doDel();
        }
        catch (Exception ex)
        {
            Log(ex.Message);
        }                  


    }


          private void doAdd()
            {
                String htmlStr1 = "";
                String htmlStr2 = "";
                OracleConnection oraCon = null;
                MySqlConnection conn = null;

                try
                {
                 //   oraCon = new OracleConnection("Data Source=G_TEST;User ID=onlinerv;Password=onlinerv;"); //alvin 11g-pc
                    oraCon = new OracleConnection(oradb); //alvin 11g-pc
                    //oraCon = new OracleConnection("Data Source=G_TEST;User ID=onlinerv;Password=onlinerv;"); //alvin 11g-pc

                    //oraCon = new OracleConnection("Data Source=HWTC.WORLD;User ID=onlinerv;Password=onlinerv;"); //HWTC台中
                    conn = new MySqlConnection(connString);
                    oraCon.Open();
                    conn.Open();
                    //String sqli = "select i.qty,i.uprc,c.CCName,ISNULL( i.description1 ,' ') d1,ISNULL( i.description2 ,' ') d2,ISNULL( i.description3 ,' ') d3,ISNULL( i.description4 ,' ') d4,ISNULL( i.description5 ,' ') d5,ISNULL( i.description6 ,' ') d6 from isu i left join CC c on i.CCId = c.CCId where i.isuid=" + isuid;
                    String sqli =
    @"SELECT t.SN,LPAD(LTRIM(CAST(t.SN AS CHAR)),10,'0') PID,IFNULL(t.ChainSn,0) ChainSn,IFNULL(t.HotelSn,0) HotelSn,
case when t.HotelSn='162' then ''
when t.HotelSn='164' then '台中'
when t.HotelSn='165' then '高雄'
end as HotelName,IFNULL(t.ProjectSn,0) ProjectSn,IFNULL(t.PackageSn,0) PackageSn,IFNULL(t.ChannelSn,0) ChannelSn,IFNULL(t.ShopId,0) ShopId,IFNULL(t.RcvShopId,0) RcvShopId,IFNULL(t.StageNo,0) StageNo,IFNULL(t.CancelNo,0) CancelNo,
IFNULL(t.CurrencySn,0) CurrencySn,IFNULL(t.TotalAmount,0) TotalAmount,IFNULL(t.OriginalAmount,0) OriginalAmount,IFNULL(t.Discount,0) Discount,IFNULL(t.TotalRoomNum,0) TotalRoomNum,IFNULL(t.TotalDays,0) TotalDays,t.CheckinDate,t.CheckoutDate,CONVERT(t.ConfirmedTime,CHAR(10)) UPDDT,LPAD(CONVERT(DATE_FORMAT(t.ConfirmedTime,'%k%i'),CHAR(4)),4,'0') UPDTM,
t.StartSearchTime,t.ConfirmedTime,IFNULL(t.CancelAmount,0) CancelAmount,IFNULL(t.CancelIp,0) CancelIp,IFNULL(t.CancelPolicyRate,0) CancelPolicyRate,IFNULL(t.CancelReturnWay,0) CancelReturnWay,
IFNULL(r.AuthorCode,'') AuthorCode,IFNULL(r.CardNo,0) CardNo,r.AUTHDT,r.AUTHTM,
case when SUBSTR(r.CardNo,1,1)='3' THEN '64'
when SUBSTR(r.CardNo,1,1)='4' THEN '59'
when SUBSTR(r.CardNo,1,1)='5' THEN '61'
else '42'
end PAYID,
IFNULL(g.PersonalId,'') PersonalId,IFNULL(g.FirstName,'') FirstName,IFNULL(g.FaxNo,'') FaxNo,
IFNULL(g.LastName,'') LastName,IFNULL(g.Address1,'') Address1,IFNULL(g.Email,'') Email,IFNULL(g.TelNo,'') TelNo,
IFNULL(d.Memo,'') Memo,substr(IFNULL(c.CompanyName,''),1,30) CompanyName,IFNULL(g.MemberSn,0) MemberSn,IFNULL(r.TransNo,'') TransNo,IFNULL(c.NationalID,'') TAXID,
concat(if(IFNULL(c.PostCode,'') ='','',concat('(',c.PostCode,')')), substr(concat(c.Area,c.Address),1,62)) TAXADDR,
substr(IFNULL(c.CompanyName,''),1,30) TAXTITLE,concat(g.LastName , g.FirstName) SENDTO,
concat(if(IFNULL(c.PostCode,'') ='','',concat('(',c.PostCode,')')), substr(concat(c.Area,c.Address),1,62)) SENDADDR
from Transactions t 
left join ReceivedStatusView r on t.SN=r.TransactionSn
left join RoomGuests g on t.SN =g.TransactionSn
left join Customers c on t.SN =c.TransactionSn
left join CustomerDailog d on t.SN = d.TransactionSn
WHERE t.ResultNo = 1 AND t.HotelSn ='164' 
and t.ReserveTime >=current_date()-1";


                    MySqlCommand cmdi = new MySqlCommand(sqli, conn);
                    cmdi.CommandTimeout = 180;
                    MySqlDataAdapter da = new MySqlDataAdapter(cmdi);
                    MySqlDataReader readeri = cmdi.ExecuteReader();
                    //readeri.Close();
                    System.Data.DataTable iDataTable = new System.Data.DataTable();
                  //  da.Fill(iDataTable);
                    iDataTable.Load(readeri);

                    readeri.Close();
                    int i = 0;

                    htmlStr1 = @"<table width='750' border='1' style='font-size:11px'><tr> <td>SN</td><td>轉入店別</td><td>姓名</td><td>Email</td><td>電話</td><td>訂金</td><td>總金額</td><td>Checkin</td><td>Checkout</td></tr>";
                    htmlStr2 = "<table width='750' border='1' style='font-size:11px'><tr><td>SN</td><td>NO</td><td>ARRDT</td><td>DEPDT</td><td>RMTYPE</td><td>QTY</td><td>REMARK</td><td>TotalDay</td><td>UPDDT</td></tr>";

                    foreach (DataRow row in iDataTable.Rows)
                    {

                        String iSN = row["SN"].ToString();
                        String iPID = row["PID"].ToString();
                        String iChainSn = row["ChainSn"].ToString();
                        String iHotelSn = row["HotelSn"].ToString();
                        String iHotelName = row["HotelName"].ToString();
                        String iProjectSn = row["ProjectSn"].ToString();
                        String iPackageSn = row["PackageSn"].ToString();
                        String iChannelSn = row["ChannelSn"].ToString();
                        String iShopId = row["ShopId"].ToString();
                        //      String iRcvShopId = readeri.GetString("RcvShopId");
                        //      String iStageNo = readeri.GetString("StageNo");
                        //      String iCancelNo = readeri.GetString("CancelNo");
                        //      String iCurrencySn = readeri.GetString("CurrencySn");
                        String iTotalAmount = row["TotalAmount"].ToString();
                        String iOriginalAmount = row["OriginalAmount"].ToString();
                        String iDiscount = row["Discount"].ToString();
                        String iTotalRoomNum = row["TotalRoomNum"].ToString();
                        //String iTotalDays = readeri.GetString("TotalDays");

                        DateTime iCheckinDate = (DateTime)row["CheckinDate"];
                        DateTime iCheckoutDate = (DateTime)row["CheckoutDate"];
                        DateTime iStartSearchTime = (DateTime)row["StartSearchTime"];
                        String iUPDDT = row["UPDDT"].ToString();
                        String iUPDTM = row["UPDTM"].ToString();
                        DateTime iConfirmedTime = (DateTime)row["ConfirmedTime"];
                        String iCancelAmount = row["CancelAmount"].ToString();
                        String iCancelIp = row["CancelIp"].ToString();
                        String iCancelPolicyRate = row["CancelPolicyRate"].ToString();
                        //  String iCancelReturnWay = readeri.GetString("CancelReturnWay");

                        String iLastName = row["LastName"].ToString();
                        String iFirstName = row["FirstName"].ToString();
                        String iTelNo = row["TelNo"].ToString();
                        String iFaxNo = row["FaxNo"].ToString();
                        String iEmail = row["Email"].ToString();
                        String iCompanyName = row["CompanyName"].ToString();
                        String iMemberSn = row["MemberSn"].ToString();
                        // String iAuthorCode = row["AuthorCode"].ToString();
                        String iAuthorCode = "";
                        String iAUTHDT = row["AUTHDT"].ToString();
                        String iAUTHTM = row["AUTHTM"].ToString();
                        String iPAYID = row["PAYID"].ToString();


                        String iTAXID = row["TAXID"].ToString();
                        String iTAXADDR = "";
                        String iTAXTITLE = "";
                        String iSENDTO = "";
                        String iSENDADDR = "";

                        if (iTAXID != "")
                        {
                            iTAXID = row["TAXID"].ToString();
                            iTAXADDR = row["TAXADDR"].ToString(); ;
                            iTAXTITLE = row["TAXTITLE"].ToString();
                            iSENDTO = row["SENDTO"].ToString();
                            iSENDADDR = row["TAXADDR"].ToString();
                        }


                        String sql = "select count(1) FROM mdrsvt00 a where a.pid ='" + iPID + "'";
                        OracleCommand cmdcnt = new OracleCommand(sql, oraCon);
                        OracleDataReader readerOraCnt = cmdcnt.ExecuteReader();


                        while (readerOraCnt.Read())
                        {

                            try
                            {
                                if (readerOraCnt.HasRows)
                                {
                                    if (readerOraCnt.GetOracleDecimal(0) == 0)
                                    {
                                        htmlStr1 += "<tr><td>" + iSN + "</td><td>" + iHotelName + "</td><td>" + iLastName + iFirstName + "</td><td>" + iEmail + "</td><td>" + iTelNo + "</td><td>" + iTotalAmount + "</td><td>" + iOriginalAmount + "</td><td>" + iCheckinDate + "</td><td>" + iCheckoutDate + "</td></tr>";

                                        String sql1 = @"insert into MDRSVT00 (PID,BOOK_BY,BOOK_TEL,BOOK_FAX,BOOK_MAIL,BOOK_COMP,ACTION,UPDOPID,UPDDT,UPDTM,MEMBERNO,PREPAID,PAYID,AUTHCODE,AUTHDT,AUTHTM,EARLY_TAKE_INV,TAXID,TAXTITLE,TAXADDR,SENDTO,SENDADDR)VALUES ('" + iPID + "','" + iLastName + iFirstName + "','" + iTelNo + "','" + iFaxNo + "','" + iEmail + "','" + iCompanyName + "','10','FIT','" + iUPDDT + "','" + iUPDTM + "','" + iMemberSn + "'," + iTotalAmount + ",'" + iPAYID + "','" + iAuthorCode + "','" + iAUTHDT + "','" + iAUTHTM + "','N','" + iTAXID + "','" + iTAXTITLE + "','" + iTAXADDR + "','" + iSENDTO + "','" + iSENDADDR + "')";
                                        OracleCommand cmd1 = new OracleCommand(sql1, oraCon);
                                        cmd1.ExecuteNonQuery();

                                        //訂單明細
                                        String sqld = @"select @i := @i + 1 as RMSEQ,b.SN,LPAD(LTRIM(CAST(t.SN AS CHAR)),10,'0') PID,concat(g.LastName ,g.FirstName) NAME ,IFNULL(s.ID,'') ID,if(g.Gender = '1','M','F') SEX,CONVERT(b.CheckinDate,CHAR(10)) ARRDT,CONVERT(ADDDATE(b.CheckinDate,1),CHAR(10)) DEPDT,
t.TotalDays RMNGT,b.OriginalPrice RMRATE,b.AddBedNum * b.AddBedFee SRVAMT,
b.Num QTY,concat(if(b.AddBedNum=0,'',concat('+Bed:',b.AddBedNum)),if(b.AddBedfee=0,'',concat('xNT:',b.AddBedfee)), if(b.SmokingNum> 0,'smk rom','No Smoking')) SPECIAL,
IFNULL(p.Num,0) BBF,substr(CONCAT('Pkg:',IFNULL(w.Name,'-'),'->',IFNULL(w2.Name,'-'), IFNULL(c.Memo,'|Check Memo!!')),1,200)  REMARK,
IFNULL(r.Condensation,'') RMTYPE,'FIT' UPDOPID,
CONVERT(t.ReserveTime,CHAR(10)) UPDDT,LPAD(CONVERT(DATE_FORMAT(t.ReserveTime,'%k%i'),CHAR(4)),4,0) UPDTM
from (select @i := 0) temp,BookingDetail b
left join RoomGuests g on b.TransactionSn = g.TransactionSn
left join Transactions t on b.TransactionSn = t.SN
left join WebPackageInfo w on b.PackageInfoSn = w.PackageInfoSn and w.LangSn='1'
left join WebPackages w2 on b.PackageSn = w2.PackageSn and w2.LangSn='1'
left join PackageMeal p on b.PackageSn = p.PackageSn
left join CustomerDailog c on c.TransactionSn = b.TransactionSn
left join Customers s on s.TransactionSn = b.TransactionSn
left join Rooms r on r.SN = b.RoomTypeSn
where b.TransactionSn=" + iSN + " order by b.SN";
                                        MySqlCommand cmdd = new MySqlCommand(sqld, conn);
                                        cmdd.CommandTimeout = 180;
                                        MySqlDataReader readerd = cmdd.ExecuteReader();
                                        if (readerd.HasRows)
                                        {

                                            while (readerd.Read())
                                            {
                                                String dPID = readerd.GetString("PID");
                                                String dRMSEQ = readerd.GetString("RMSEQ");
                                                String dACTION = "10";
                                                String dNAME = readerd.GetString("NAME");
                                                String dID = readerd.GetString("ID");
                                                String dSEX = readerd.GetString("SEX");
                                                String dARRDT = readerd.GetString("ARRDT");
                                                String dDEPDT = readerd.GetString("DEPDT");
                                                String dRMNGT = readerd.GetString("RMNGT");
                                                String dRMTYPE = readerd.GetString("RMTYPE");
                                                //String dRMTYPE = "";
                                                String dRMRATE = readerd.GetString("RMRATE");
                                                String dSRVAMT = readerd.GetString("SRVAMT");
                                                String dBBF = readerd.GetString("BBF");
                                                String dQTY = readerd.GetString("QTY");
                                                String dSPECIAL = readerd.GetString("SPECIAL");
                                                String dREMARK = readerd.GetString("REMARK");
                                                String dUPDOPID = readerd.GetString("UPDOPID");
                                                String dUPDDT = readerd.GetString("UPDDT");
                                                String dUPDTM = readerd.GetString("UPDTM");
                                                htmlStr2 += "<tr><td>" + iSN + "</td><td>" + dRMSEQ + "</td><td>" + dARRDT + "</td><td>" + dDEPDT + "</td><td>" + dRMTYPE + "</td><td>" + dQTY + "</td><td>" + dREMARK + "</td><td>" + dRMNGT + "</td><td>" + dUPDDT + "</td></tr>";


                                                //寫入明細
                                                String sql2 = @"insert into MDRSVT20 (PID,RMSEQ,ACTION,NAME,ID,SEX,ARRDT,DEPDT,RMNGT,RMTYPE,RMRATE,SRVAMT,BBF,QTY,SPECIAL,REMARK,UPDOPID,UPDDT,UPDTM)VALUES ('" + dPID + "'," + dRMSEQ + ",'" + dACTION + "','" + dNAME + "','" + dID + "','" + dSEX + "','" + dARRDT + "','" + dDEPDT + "'," + dRMNGT + ",'" + dRMTYPE + "'," + dRMRATE + "," + dSRVAMT + "," + dBBF + "," + dQTY + ",'" + dSPECIAL + "','" + dREMARK + "','" + dUPDOPID + "','" + dUPDDT + "','" + dUPDTM + "')";
                                                OracleCommand cmd2 = new OracleCommand(sql2, oraCon);
                                                cmd2.ExecuteNonQuery();
                                            }

                                        }
                                        readerd.Close();

                                        //轉到前台成功的話將ProcessNo改成2-已處理
                                        
                                        String sql3 = @"update Transactions set ProcessNo=2 where SN="+iSN;
                                        MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
                                        cmd3.ExecuteNonQuery();
                                        
                                        i++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //      Log(ex.Message);
                            }
                            finally
                            {

                            }

                        }


                    }
                    
                    htmlStr2 += "</table>";
                    htmlStr1 += "</table>";

                    String html = htmlStr1 + "<br>" + htmlStr2;
                    

                    if (i > 0)
                    {
                       Log("資料成功轉入,共" + i + "筆");
                        SendEmail("官網訂單轉前台成功轉入通知信,共轉入" + i + "筆", html);
                    }






                    //SoccerDataTable.Load(readeri);
                    //dataGridView1.DataSource = SoccerDataTable;

                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
                finally
                {
                    oraCon.Close();
                    conn.Close();
                }
            }


            private void doDel()
            {
                /*
                 * UPDATE Transactions SET ResultNo = 0,ProcessNo=4,IsSysChecked = 1,
									CancelNo = $CancelNo,CancelUserId = '$CCId',
									CancelTime = '".date('Y-m-d H:i:s')."',
									CancelIp = ".ip2long($_SERVER['REMOTE_ADDR']).",
									CancelPolicyRate = 1.00,CancelAmount= TotalAmount,CancelReturnWay = 6,StageNo = 103 
									WHERE SN = $TransSn
                 */


                String htmlStr1 = "";
                String htmlStr2 = "";
                OracleConnection oraCon = null;
                MySqlConnection conn = null;

                try
                {
                   // oraCon = new OracleConnection("Data Source=G_TEST;User ID=onlinerv;Password=onlinerv;"); //alvin 11g-pc
                    oraCon = new OracleConnection(oradb); //alvin 11g-pc
                    //oraCon = new OracleConnection("Data Source=G_TEST;User ID=onlinerv;Password=onlinerv;"); //alvin 11g-pc

                    //oraCon = new OracleConnection("Data Source=HWTC.WORLD;User ID=onlinerv;Password=onlinerv;"); //HWTC台中
                    conn = new MySqlConnection(connString);
                    oraCon.Open();
                    conn.Open();
                    //String sqli = "select i.qty,i.uprc,c.CCName,ISNULL( i.description1 ,' ') d1,ISNULL( i.description2 ,' ') d2,ISNULL( i.description3 ,' ') d3,ISNULL( i.description4 ,' ') d4,ISNULL( i.description5 ,' ') d5,ISNULL( i.description6 ,' ') d6 from isu i left join CC c on i.CCId = c.CCId where i.isuid=" + isuid;
                    String sqli =
    @"SELECT t.SN,LPAD(LTRIM(CAST(t.SN AS CHAR)),10,'0') PID,IFNULL(t.HotelSn,0) HotelSn 
FROM Transactions t 
WHERE t.ResultNo = 0 AND t.HotelSn ='164'
and t.ReserveTime >=current_date()-1";


                    MySqlCommand cmdi = new MySqlCommand(sqli, conn);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmdi);
                    MySqlDataReader readeri = cmdi.ExecuteReader();
                    //readeri.Close();
                    System.Data.DataTable iDataTable = new System.Data.DataTable();
                    //da.Fill(iDataTable);

                    iDataTable.Load(readeri);


                    readeri.Close();
                    int i = 0;

                    htmlStr1 = @"<table width='750' border='1' style='font-size:11px'><tr> <td>SN</td></tr>";
                    htmlStr2 = "<table width='750' border='1' style='font-size:11px'><tr><td>SN</td></tr>";

                    foreach (DataRow row in iDataTable.Rows)
                    {

                        String iSN = row["SN"].ToString();
                        String iPID = row["PID"].ToString();



                        String sql = "select count(1) FROM mdrsvt00 a where a.pid ='" + iPID + "' and action='14'";
                        OracleCommand cmdcnt = new OracleCommand(sql, oraCon);
                        OracleDataReader readerOraCnt = cmdcnt.ExecuteReader();


                        while (readerOraCnt.Read())
                        {

                            try
                            {
                                if (readerOraCnt.HasRows)
                                {
                                    if (readerOraCnt.GetOracleDecimal(0) == 0)
                                    {
                                        htmlStr1 += "<tr><td>" + iSN + "</td></tr>";

                                        String sql1 = @"update MDRSVT00 set ACTION='14' where PID='" + iPID + "'";
                                        OracleCommand cmd1 = new OracleCommand(sql1, oraCon);
                                        cmd1.ExecuteNonQuery();

                                        //訂單明細
                                        String sqld = @"select b.SN,LPAD(LTRIM(CAST(t.SN AS CHAR)),10,'0') PID where b.TransactionSn=" + iSN + " order by b.SN";
                                        MySqlCommand cmdd = new MySqlCommand(sqld, conn);
                                        MySqlDataReader readerd = cmdd.ExecuteReader();
                                        if (readerd.HasRows)
                                        {

                                            while (readerd.Read())
                                            {
                                                String dPID = readerd.GetString("PID");
                                                htmlStr2 += "<tr><td>" + iSN +"</td></tr>";


                                                //寫入明細
                                                String sql2 = @"update MDRSVT20 set ACTION='14' where PID='" + iPID + "'";
                                                OracleCommand cmd2 = new OracleCommand(sql2, oraCon);
                                                cmd2.ExecuteNonQuery();
                                            }

                                        }
                                        readerd.Close();

                                        //轉到前台成功的話將ProcessNo改成2-已處理
                                        /*
                                        String sql3 = @"update Transactions set ProcessNo=2 where SN="+iSN;
                                        MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
                                        cmd3.ExecuteNonQuery();
                                         * */
                                        i++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //      Log(ex.Message);
                            }
                            finally
                            {

                            }

                        }


                    }
                    
                    htmlStr2 += "</table>";
                    htmlStr1 += "</table>";

                    String html = htmlStr1 + "<br>" + htmlStr2;
                    if (i > 0)
                    {
                        Log(" 資料成功轉入(取銷訂單),共" + i + "筆");
                        SendEmail("官網訂單轉前台成功轉入通知信,共轉入(取銷訂單)" + i + "筆", html);
                    }


                    
                  



                    //SoccerDataTable.Load(readeri);
                    //dataGridView1.DataSource = SoccerDataTable;

                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
                finally
                {
                    oraCon.Close();
                    conn.Close();
                    
                }
                //Log("TEST");
                //SendEmail("TEST","TEST");
            }

        }



}
