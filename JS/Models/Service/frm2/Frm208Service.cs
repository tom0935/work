
using System.Data.SqlClient;
using System.Transactions;
using System.Configuration;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using Jasper.Models;
using PagedList;
using System.Linq;
using Jasper.Models.Service;
using System.Linq.Dynamic;
using Jasper.Models.Poco;
using Jasper.util;
using System.Data.Objects;

namespace Jasper.service.frm2{
public class Frm208Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDG1(String MKRNO, String FEEYMS,String FEEYME, String VNO, String REQUEST_NO,String ACCTP_NO,String ACCNO,String QPAYSTA,String QPAYSTA2)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        if (StringUtils.getString(QPAYSTA) == "" && StringUtils.getString(QPAYSTA2) == "" && StringUtils.getString(MKRNO) == "" && StringUtils.getString(FEEYMS) == "" && StringUtils.getString(FEEYME) == "" && StringUtils.getString(VNO) == "" && StringUtils.getString(REQUEST_NO) == "" && StringUtils.getString(ACCTP_NO) == "" && StringUtils.getString(ACCNO) == "")
        {
            MKRNO = "*";
        }

        var query = from t in db.MKRPAY where t.RSTA == " " select t;

        if (StringUtils.getString(QPAYSTA) != "*" && StringUtils.getString(QPAYSTA)!="")
        {
            int PAYSTA = Convert.ToInt16(QPAYSTA);
            query = query.Where(t => t.PAYSTA == PAYSTA);
        }

        if (StringUtils.getString(QPAYSTA2) != "*" && StringUtils.getString(QPAYSTA) != "")
        {
            int PAYSTA2 = Convert.ToInt16(QPAYSTA2);
            query = query.Where(t => t.PAYSTA2 == PAYSTA2);
        }

        if (StringUtils.getString(MKRNO) != "")
        {
            query = query.Where(t => t.MKRNO == MKRNO );
        }
        if (StringUtils.getString(FEEYMS) != "" && StringUtils.getString(FEEYME) != "")
        {
            //DateTime ed = Convert.ToDateTime(FEEYME);
           // DateTime ed = Convert.ToDateTime(FEEYME).AddDays(1);
           // query = query.Where(t => t.FEEYM.CompareTo >= sd);
            query = query.Where(t => t.FEEYM.CompareTo(FEEYMS) >= 0 && t.FEEYM.CompareTo(FEEYME) <= 0);
        }

        if (StringUtils.getString(FEEYMS) != "")
        {
            //DateTime ed = Convert.ToDateTime(FEEYME);
            // DateTime ed = Convert.ToDateTime(FEEYME).AddDays(1);
            // query = query.Where(t => t.FEEYM.CompareTo >= sd);
            query = query.Where(t => t.FEEYM.CompareTo(FEEYMS) >= 0 );
        }

        if (StringUtils.getString(VNO) != "")
        {
            query = query.Where(t => t.VNO == VNO);
        }

        if (StringUtils.getString(REQUEST_NO) != "")
        {
            query = query.Where(t => t.REQUEST_NO == REQUEST_NO);
        }

        if (StringUtils.getString(ACCTP_NO) != "")
        {
            query = query.Where(t => t.ACCTP == ACCTP_NO);
        }

        if (StringUtils.getString(ACCNO) != "")
        {
            query = query.Where(t => t.ACCNO == ACCNO);
        }
        //String.Format("{0:yyyy-MM-dd}",item.DT2)

        query = query.OrderByDescending(t=>t.TAGID);
        foreach (var item in query)
        {
        
            var itemObject = new JObject
                    {                           
                        {"MKRNM",item.MKRNM},                
                        {"MKRNO",item.MKRNO},  
                        {"AMTSUM",item.AMTSUM},
                        {"ACCNM",item.ACCNM},
                        {"ACCNO",item.ACCNO},
                        {"ACCTP",item.ACCTP},                        
                        {"AMTTX",item.AMTTX},
                        {"FEEAMT",item.FEEAMT},
                        {"FIXAREA",item.FIXAREA.ToString()},
                        {"FIXQUIP",item.FIXQUIP.ToString()},
                        {"INBUDG",item.INBUDG},
                        {"KPISUB",item.KPISUB},
                        {"NOTES",item.NOTES},                
                        {"TAGID",item.TAGID},                        
                        {"FEEYM",item.FEEYM},
                        {"PAYSTA",item.PAYSTA},                                               
                        {"PAYSTA2",item.PAYSTA2},  
                        {"VNO",item.VNO},
                        {"REQUEST_NO",item.REQUEST_NO},
                        {"VDT",String.Format("{0:yyyy-MM-dd}",item.VDT)},                        
                        {"LOGUSR",item.LOGUSR}                        
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
            jo.Add("total", query.Count());
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;
    }

    public JObject getDG1_1(String ACCTP)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        //DateTime dt = Convert.ToDateTime(DT);
        var query = from t in db.MKRPAY where t.RSTA == " " select t;


        if (StringUtils.getString(ACCTP) != "")
        {
            query = query.Where(t => t.ACCTP == ACCTP);
        }
        //String.Format("{0:yyyy-MM-dd}",item.DT2)

        query = query.OrderByDescending(t => t.TAGID);
        foreach (var item in query)
        {

            var itemObject = new JObject
                    {                           
                        {"MKRNM",item.MKRNM},                
                        {"MKRNO",item.MKRNO},  
                        {"AMTSUM",item.AMTSUM},
                        {"ACCNM",item.ACCNM},
                        {"ACCNO",item.ACCNO},
                        {"ACCTP",item.ACCTP},                        
                        {"AMTTX",item.AMTTX},
                        {"FEEAMT",item.FEEAMT},
                        {"FIXAREA",item.FIXAREA.ToString()},
                        {"FIXQUIP",item.FIXQUIP.ToString()},
                        {"INBUDG",item.INBUDG},
                        {"KPISUB",item.KPISUB},
                        {"NOTES",item.NOTES},                
                        {"TAGID",item.TAGID},                        
                        {"FEEYM",item.FEEYM},
                        {"PAYSTA",item.PAYSTA},                                               
                        {"PAYSTA2",item.PAYSTA2},   
                        {"VNO",item.VNO},
                        {"REQUEST_NO",item.REQUEST_NO},
                        {"VDT",String.Format("{0:yyyy-MM-dd}",item.VDT)},                        
                        {"LOGUSR",item.LOGUSR}                        
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
            jo.Add("total", query.Count());
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;
    }





    public JObject getDG1ByFrm302(String YM,String ACCNO,String ACCTP)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        //DateTime dt = Convert.ToDateTime(DT);
        var query = from t in db.MKRPAY where t.RSTA == " " select t;


        if (StringUtils.getString(ACCTP) != "")
        {
            query = query.Where(t => t.FEEYM==YM && t.ACCNO==ACCNO && t.ACCTP == ACCTP);
        }
        //String.Format("{0:yyyy-MM-dd}",item.DT2)

        query = query.OrderByDescending(t => t.TAGID);
        foreach (var item in query)
        {

            var itemObject = new JObject
                    {                           
                        {"MKRNM",item.MKRNM},                
                        {"MKRNO",item.MKRNO},  
                        {"AMTSUM",item.AMTSUM},
                        {"ACCNM",item.ACCNM},
                        {"ACCNO",item.ACCNO},
                        {"ACCTP",item.ACCTP},                        
                        {"AMTTX",item.AMTTX},
                        {"FEEAMT",item.FEEAMT},
                        {"FIXAREA",item.FIXAREA.ToString()},
                        {"FIXQUIP",item.FIXQUIP.ToString()},
                        {"INBUDG",item.INBUDG},
                        {"KPISUB",item.KPISUB},
                        {"NOTES",item.NOTES},                
                        {"TAGID",item.TAGID},                        
                        {"FEEYM",item.FEEYM},
                        {"PAYSTA",item.PAYSTA},                                               
                        {"PAYSTA2",item.PAYSTA2},   
                        {"VNO",item.VNO},
                        {"REQUEST_NO",item.REQUEST_NO},
                        {"VDT",String.Format("{0:yyyy-MM-dd}",item.VDT)},                        
                        {"LOGUSR",item.LOGUSR}                        
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
            jo.Add("total", query.Count());
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;
    }  
  


    public int doSave(Frm208Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
          
                MKRPAY obj = (from t in db.MKRPAY where t.TAGID == param.TAGID select t).Single();                
                
                
                //obj.ACCNM = param.ACCNM.Trim();
                //obj.ACCNO = param.ACCNO.Trim();
                //obj.ACCTP = param.ACCTP.Trim();
                //obj.AMTSUM = Convert.ToInt32(param.AMTSUM);

                //obj.FEEAMT = Convert.ToInt32(param.FEEAMT);
                //obj.AMTTX = Convert.ToInt32(param.AMTTX);

                
                obj.FIXAREA =Convert.ToByte(param.FIXAREA);
                obj.FIXQUIP =Convert.ToByte(param.FIXQUIP);
                obj.INBUDG = StringUtils.getString(param.INBUDG).Trim();
                obj.KPISUB = StringUtils.getString(param.KPISUB).Trim();
                //obj.FEEYM = param.FEEYM;
                obj.LOGDT = System.DateTime.Now;

                obj.LOGUSR = param.LOGUSR.Trim();
                obj.NOTES = param.NOTES.Trim();

                obj.MKRNM = param.MKRNM.Trim();
                obj.MKRNO = param.MKRNO.Trim();
                obj.PAYSTA =Convert.ToInt16(param.PAYSTA);
                obj.PAYSTA2 = Convert.ToInt16(param.PAYSTA2);
                           
                obj.VDT = DateTimeUtil.getDateTime(param.VDT);
                obj.VNO = param.VNO.Trim();
                obj.REQUEST_NO = param.REQUEST_NO;
                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm208Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
             //   int txrat = Convert.ToInt32(cs.getConfig("TXRAT"));
                MKRPAY obj = new MKRPAY();

                obj.ACCNM = StringUtils.getString(param.ACCNM).Trim();
                obj.ACCNO = StringUtils.getString(param.ACCNO).Trim();
                obj.ACCTP = StringUtils.getString(param.ACCTP).Trim();
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);

                obj.FEEAMT = Convert.ToInt32(param.FEEAMT);
                obj.AMTTX = Convert.ToInt32(param.AMTTX);


                obj.FIXAREA = Convert.ToByte(param.FIXAREA);
                obj.FIXQUIP = Convert.ToByte(param.FIXQUIP);
                obj.INBUDG = StringUtils.getString(param.INBUDG).Trim();
                obj.KPISUB = param.KPISUB;
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();

                obj.MKRNM = StringUtils.getString(param.MKRNM).Trim();
                obj.MKRNO = StringUtils.getString(param.MKRNO).Trim();
                obj.PAYSTA = Convert.ToInt16(param.PAYSTA);
                obj.PAYSTA2 = Convert.ToInt16(param.PAYSTA2);
              //  obj.RPTDT = DateTimeUtil.getDateTime(param.RPTDT);
                obj.RSTA = " ";
                obj.VDT = DateTimeUtil.getDateTime(param.VDT);
                obj.VNO = param.VNO;
                obj.REQUEST_NO = param.REQUEST_NO;
                //obj.RSTA = " ";
                obj.TAGID = cs.getTagidByDatetime();
                db.MKRPAY.Add(obj);
                i = db.SaveChanges();

                int amtsum = Convert.ToInt32(param.AMTSUM);

                db.Database.ExecuteSqlCommand("update ACCBUDGET set USEAMT=USEAMT + {0} where TAGID={1}", amtsum, param.ACCTP + param.ACCNO +obj.FEEYM.Substring(0,4));
                db.Database.ExecuteSqlCommand("update YMBUDGET set USEAMT=USEAMT + {0} where TAGID={1}", amtsum, param.ACCTP + param.ACCNO +obj.FEEYM);
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }





    public int doSuccess(String TAGID)
    {

        int i = 0;
        try
        {

            i = db.Database.ExecuteSqlCommand("update COMPLN set RSTA='Y',ENDDT={0} where TAGID={1}", System.DateTime.Now, StringUtils.getString(TAGID).Trim());
        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public int doRemove(String TAGID,String AMTSUM,String ACCTP,String ACCNO,String FEEYM)
    {

        int i = 0;
        try
        {
            /*
             * W_SQL = "UPDATE DESKBOOK SET " + ;
		"RSTA = 'C', " + ;
		"ENDDT = ?W_DTT " + ;
		"WHERE TAGID = ?W_TAGID"
             */
            i = db.Database.ExecuteSqlCommand("update MKRPAY set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now, TAGID);

            int amtsum = Convert.ToInt32(Convert.ToInt32(AMTSUM) * (-1));

            db.Database.ExecuteSqlCommand("update ACCBUDGET set USEAMT=USEAMT + {0} where TAGID={1}", amtsum, ACCTP + ACCNO + FEEYM.Substring(0, 4));
            db.Database.ExecuteSqlCommand("update YMBUDGET set USEAMT=USEAMT + {0} where TAGID={1}", amtsum, ACCTP + ACCNO + FEEYM);
        }
        catch (Exception ex)
        {

        }

        return i;
    }

    //檢查發票號是否重覆
    public int doCheckVNO(String FEEYM,String VNO)
    {
        int i=0;
        try
        {
            var query = from t in db.MKRPAY where t.FEEYM == FEEYM && t.VNO == VNO select t;
            if (query.Count() > 0)
            {
                i = 1;
            }
        }catch(Exception ex){
            i = 0;
        }
        return i;
    }
   

    public JArray getUSEAMT(String FEEYM, String ACCTP,String ACCNO){
        JArray ja = new JArray();
        JObject jo = new JObject();   
        ACCBUDGET obj1 = (from t in db.ACCBUDGET where t.YRN == FEEYM.Substring(0,4) && t.ACCTP == ACCTP && t.ACCNO == ACCNO select t).SingleOrDefault();
        YMBUDGET obj2 = (from t in db.YMBUDGET where t.YM == FEEYM && t.ACCTP == ACCTP && t.ACCNO == ACCNO select t).SingleOrDefault();

        /*
W_SQL = "SELECT * FROM ACCBUDGET " + ;
	"WHERE YRN = ?W_YRN AND ACCTP = ?W_ACCTP AND ACCNO = ?W_ACCNO "
SQLEXEC(CONNID,W_SQL,"YRBUDG208")

W_SQL = "SELECT * FROM YMBUDGET " + ;
	"WHERE YM = ?W_YM AND ACCTP = ?W_ACCTP AND ACCNO = ?W_ACCNO "
SQLEXEC(CONNID,W_SQL,"YMBUDG208")

THISFORM.CTNR1.YRLEFT.VALUE = YRBUDG208.BUDGET - YRBUDG208.USEAMT
THISFORM.CTNR1.YMLEFT.VALUE = YMBUDG208.BUDGET - YMBUDG208.USEAMT
        */
        if (obj1 != null)
        {
            jo.Add("AMT1", obj1.BUDGET - obj1.USEAMT);
        }
        if (obj2 != null)
        {
            jo.Add("AMT2", obj2.BUDGET - obj2.USEAMT);
        }
        ja.Add(jo);
        return ja;
    }

    public JArray getReport(String MKRNO, String FEEYM,String VNO)
    {
        /*
SELECT RECNO() AS "項次", MKRNO AS "廠商編號", MKRNM AS "廠商名稱", IIF(PAYSTA=1,"代付","代收") AS "付款狀態", AMTSUM AS "付款金額", ;
	VNO AS "發票號碼", FEEYM AS "費用月份", NOTES AS "備註說明" ;
	FROM MKRPAY2081 ;
	INTO CURSOR CR208*/
        JArray ja = new JArray();
        JObject jo = new JObject();
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.MKRPAY where t.RSTA == " " select t);
        if (StringUtils.getString(MKRNO) != "")
        {
            query = query.Where(t => t.MKRNO == MKRNO);
        }
        if (StringUtils.getString(FEEYM) != "")
        {
            query = query.Where(t => t.FEEYM == FEEYM);
        }
        if (StringUtils.getString(VNO) != "")
        {
            query = query.Where(t => t.VNO == VNO);
        }
        //String.Format("{0:yyyy-MM-dd}",item.DT2)
        foreach (var item in query)
        {

            var itemObject = new JObject
                    {   
                        {"廠商編號",item.MKRNO},                        
                        {"廠商名稱",item.MKRNM},                
                        {"請求書編號",item.REQUEST_NO},
                        {"付款狀態",(item.PAYSTA ==1)?"代付":"代收"},   
                        {"付款金額",item.AMTSUM},
                        {"發票號碼",item.VNO},
                        {"費用月份",item.FEEYM},
                        {"備註說明", StringUtils.FilterGdName(item.NOTES)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }



}
}