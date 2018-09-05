
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
public class Frm214Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String q)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.RTNFEE select t);

        if (StringUtils.getString(q) == "")
        {
            query = query.Where(t => t.AMTSUM == 0);
        }
        else
        {
            if (q.Length > 4)
            {
                DateTime dt = DateTimeUtil.getDateTime(q);
                query = query.Where(t => t.RSTA == " " && t.FEEDT >= dt);
            }
            else
            {
                query = query.Where(t => t.RSTA == " " && t.FEEDT.ToString().Substring(0,4) == q);
            }
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMTSUM",item.AMTSUM},
                        {"FEEDT",String.Format("{0:yyyy-MM-dd}" , item.FEEDT)},
                        {"LOGUSR",item.LOGUSR},
                        {"NOTES",item.NOTES},
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

  


  


    public int doSave(Frm214Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                RTNFEE obj = (from t in db.RTNFEE where t.TAGID == param.TAGID select t).Single();                
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);
                obj.FEEDT = DateTimeUtil.getDateTime(param.FEEDT);
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();                

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm214Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                RTNFEE obj = new RTNFEE();
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);
                obj.FEEDT = DateTimeUtil.getDateTime(param.FEEDT);
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RSTA = " ";
                
                obj.TAGID = cs.getTagidByDatetime();
                db.RTNFEE.Add(obj);
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
            i = db.Database.ExecuteSqlCommand("update RTNFEE set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now, StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}