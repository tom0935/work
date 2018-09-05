
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
public class Frm402Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String q)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        
        var query = from t in db.CLASBOOK where t.RSTA==" " && t.DT >= System.DateTime.Now select t;

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CLASNM",item.CLASNM},
                        {"DT",String.Format("{0:yyyy-MM-dd}" , item.DT)},
                        {"JOINER",item.JOINER},
                        {"LOGUSR",item.LOGUSR},
                        {"MANS",item.MANS},
                        {"RMNO",item.RMNO},                        
                        {"RMTAG",item.RMTAG},
                        {"TAGID",item.TAGID},                        
                        {"TEACHER",item.TEACHER},
                        {"TM",item.TM}
                        
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

  


  


    public int doSave(Frm402Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                CLASBOOK obj = (from t in db.CLASBOOK where t.TAGID == param.TAGID select t).Single();
                obj.CLASNM = StringUtils.getString(param.CLASNM).Trim();
                obj.DT = DateTimeUtil.getDateTime(param.DT);
                obj.JOINER = StringUtils.getString(param.JOINER).Trim();
                obj.MANS = Convert.ToInt32(param.MANS);
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.TEACHER = StringUtils.getString(param.TEACHER).Trim();
                obj.TM = StringUtils.getString(param.TM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm402Poco param)
    {

        int i = 0;
        try
        {                        
            using (db)
            {
                CLASBOOK obj = new CLASBOOK();
                obj.CLASNM = StringUtils.getString(param.CLASNM).Trim();
                obj.DT = DateTimeUtil.getDateTime(param.DT);
                obj.JOINER = StringUtils.getString(param.JOINER).Trim();
                obj.MANS = Convert.ToInt32(param.MANS);
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.TEACHER = StringUtils.getString(param.TEACHER).Trim();
                obj.TM = StringUtils.getString(param.TM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();                
                obj.TAGID = cs.getTagidByDatetime();
                obj.RSTA = " ";                
                obj.TAGID = cs.getTagidByDatetime();

                db.CLASBOOK.Add(obj);
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
            i = db.Database.ExecuteSqlCommand("update CLASBOOK set RSTA='C' where TAGID={0}", StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }

    public JArray getTM()
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        var query = from t in db.CONFCODE where t.CODE_KIND == "FRM4021" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

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


    public JArray getCLASNM()
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        var query = from t in db.CONFCODE where t.CODE_KIND == "FRM4022" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

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

    public JArray getTEACHER(String CNO)
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        var query = from t in db.CONFCODE where t.CODE_KIND == "FRM4023" && t.CODE_NO == CNO select new { CNO = t.CODE_NAME, CNM = t.CODE_NAME };

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