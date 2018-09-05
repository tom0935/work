
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
public class Frm213Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String q)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.WATERFEE select t);

        if (StringUtils.getString(q) == "")
        {
            query = query.Where(t => t.AMTSUM == 0);
        }
        else
        {
            query = query.Where(t => t.RSTA == " " && t.FEEYM== q).OrderBy(t => t.FEEYM).OrderBy(t=>t.ADR);
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMTSUM",item.AMTSUM},
                        {"ADR",item.ADR},
                        {"DGRES1",item.DGRES1},
                        {"DGRES2",item.DGRES2},
                        {"DGRESN",item.DGRESN},
                        {"FEEDT1",String.Format("{0:yyyy-MM-dd}" , item.FEEDT1)},
                        {"FEEDT2",String.Format("{0:yyyy-MM-dd}" , item.FEEDT2)},
                        {"FEEYM",item.FEEYM},
                        {"WATRNO",item.WATRNO},
                        //{"FEEDT",String.Format("{0:yyyy-MM-dd}" , item.FEEDT)},
                        {"LOGUSR",item.LOGUSR},
                        {"TAGID",item.TAGID}
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

  


  


    public int doSave(Frm213Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                WATERFEE obj = (from t in db.WATERFEE where t.TAGID == param.TAGID select t).Single();                
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);
                obj.ADR = StringUtils.getString(param.ADR).Trim();
                obj.DGRES1 = Convert.ToInt32(param.DGRES1);
                obj.DGRES2 = Convert.ToInt32(param.DGRES2);
                obj.DGRESN = Convert.ToInt32(param.DGRESN);
                obj.FEEDT1 = DateTimeUtil.getDateTime(param.FEEDT1);
                obj.FEEDT2 = DateTimeUtil.getDateTime(param.FEEDT2);
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.WATRNO = StringUtils.getString(param.WATRNO).Trim();

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm213Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                WATERFEE obj = new WATERFEE();
             //   obj.AMTSUM = Convert.ToInt32(param.AMTSUM);



                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);
                obj.ADR = StringUtils.getString(param.ADR).Trim();
                obj.DGRES1 = Convert.ToInt32(param.DGRES1);
                obj.DGRES2 = Convert.ToInt32(param.DGRES2);
                obj.DGRESN = Convert.ToInt32(param.DGRESN);
                obj.FEEDT1 = DateTimeUtil.getDateTime(param.FEEDT1);
                obj.FEEDT2 = DateTimeUtil.getDateTime(param.FEEDT2);
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.WATRNO = StringUtils.getString(param.WATRNO).Trim();
                obj.RSTA = " ";
                
                obj.TAGID = cs.getTagidByDatetime();
                db.WATERFEE.Add(obj);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }








    public int doRemove(String TAGID)
    {

        int i = 0;
        try
        {
            i = db.Database.ExecuteSqlCommand("update WATERFEE set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now, StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }

    public JArray getLOCT()
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        var query = from t in db.CONFCODE where t.CODE_KIND == "FRM213" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

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