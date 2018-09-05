
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

namespace Jasper.service.frm4{
public class Frm403Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String q,String FEEYM)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = (from t in db.CLUBFEE where t.RSTA == " " select t);

        if (q == "1" && StringUtils.getString(FEEYM)!="")
        {
            query = query.Where(t => t.FEEYM == FEEYM);
        }
        else if (q == "2")
        {
            query = query.Where(t =>  t.PAYAMT < t.AMT );
        }
        else
        {
            query = query.Where(t=> t.DT >= System.DateTime.Now);
        }

        query = query.OrderBy(t => t.DT);

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"ADULT",item.ADULT},
                        {"DT",String.Format("{0:yyyy-MM-dd}" , item.DT)},
                        {"AMT",item.AMT},
                        {"CHILD",item.CHILD},
                        {"FEEYM",item.FEEYM},
                        {"RMNO",item.RMNO},
                        {"RMTAG",item.RMTAG},
                        {"TAGID",item.TAGID},                        
                        {"ITM",item.ITM},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"PAYAMT",item.PAYAMT},
                        {"QTY",item.QTY},
                        {"TPNM",item.TPNM},
                        {"UPR",item.UPR}                        
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

  


  


    public int doSave(Frm403Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                CLUBFEE obj = (from t in db.CLUBFEE where t.TAGID == param.TAGID select t).Single();
                obj.ADULT = Convert.ToInt16(param.ADULT);
                obj.AMT = Convert.ToInt32(param.AMT);
                obj.CHILD = Convert.ToInt16(param.CHILD);
                obj.DT = DateTimeUtil.getDateTime(param.DT);                
                obj.FEEYM = String.Format("{0:yyyyMM}", obj.DT);
                obj.ITM = StringUtils.getString(param.ITM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.PAYAMT = Convert.ToInt32(param.PAYAMT);
                obj.QTY = Convert.ToInt16(param.QTY);
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.TPNM = StringUtils.getString(param.TPNM).Trim();
                obj.UPR = Convert.ToInt32(param.UPR);

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm403Poco param)
    {

        int i = 0;
        try
        {                        
            using (db)
            {
                CLUBFEE obj = new CLUBFEE();
                obj.ADULT = Convert.ToInt16(param.ADULT);
                obj.AMT = Convert.ToInt32(param.AMT);
                obj.CHILD = Convert.ToInt16(param.CHILD);
                obj.DT = DateTimeUtil.getDateTime(param.DT);
                obj.FEEYM = String.Format("{0:yyyyMM}", obj.DT);
                obj.ITM = StringUtils.getString(param.ITM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.PAYAMT = Convert.ToInt32(param.PAYAMT);
                obj.QTY = Convert.ToInt16(param.QTY);
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.TPNM = StringUtils.getString(param.TPNM).Trim();
                obj.UPR = Convert.ToInt32(param.UPR);
                obj.RSTA = " ";                
                obj.TAGID = cs.getTagidByDatetime();

                db.CLUBFEE.Add(obj);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }








    public int doRemove(String TAGID,String LOGUSR)
    {

        int i = 0;
        try
        {
            i = db.Database.ExecuteSqlCommand("update CLUBFEE set RSTA='C',LOGDT={0},LOGUSR={1} where TAGID={2}",  System.DateTime.Now, StringUtils.getString(LOGUSR).Trim(), StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }

    public JArray getITM()
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        var query = from t in db.CONFCODE where t.CODE_KIND == "FRM4031" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
            ja.Add(itemObject);
        }
        return ja;
    }




}
}