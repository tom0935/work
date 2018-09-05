
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

namespace Jasper.service.frm3{
public class Frm301Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDG1(Frm301Poco param)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        Boolean qmode = false;
        var query = from t in db.CTINF select t;
            if (StringUtils.getString(param.INFTP) != "")
            {
                query = query.Where(t => t.INFTP.Contains(param.INFTP));
                qmode = true;
            }
             if (StringUtils.getString(param.CNM) != "")
            {
                query = query.Where(t => t.CNM.Contains(param.CNM));
                qmode = true;
            }
             if (StringUtils.getString(param.ENM) != "")
            {
                query = query.Where(t => t.ENM.Contains(param.ENM));
                qmode = true;
            }
             if (StringUtils.getString(param.LOCT) != "")
            {
                query = query.Where(t => t.LOCT.Contains(param.LOCT));
                qmode = true;
            }
             if (StringUtils.getString(param.RUNTP) != "")
            {
                query = query.Where(t => t.RUNTP.Contains(param.RUNTP));
                qmode = true;
            }

             if (qmode == false)
             {
                 query = query.OrderBy(t => t.INFTP);
             }


        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"ADR",item.ADR},                                                
                        {"CNM",item.CNM},
                        {"ENM",item.ENM},
                        {"INFTP",item.INFTP},
                        {"LOCT",item.LOCT},
                        {"RUNTP",item.RUNTP},
                        {"TEL",item.TEL},
                        {"WEBSITE",item.WEBSITE},
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




  


    public int doSave(Frm301Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                CTINF obj = (from t in db.CTINF where t.TAGID == param.TAGID select t).Single();
                obj.ADR = StringUtils.getString(param.ADR).Trim();
                obj.CNM = StringUtils.getString(param.CNM).Trim();
                obj.ENM = StringUtils.getString(param.ENM).Trim();
                obj.INFTP = StringUtils.getString(param.INFTP).Trim();
                obj.LOCT = StringUtils.getString(param.LOCT).Trim();
                obj.RUNTP = StringUtils.getString(param.RUNTP).Trim();
                obj.TEL = StringUtils.getString(param.TEL).Trim();               
                obj.WEBSITE = StringUtils.getString(param.WEBSITE).Trim();
                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm301Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                CTINF obj = new CTINF();
                obj.ADR = StringUtils.getString(param.ADR).Trim();
                obj.CNM = StringUtils.getString(param.CNM).Trim();
                obj.ENM = StringUtils.getString(param.ENM).Trim();
                obj.INFTP = StringUtils.getString(param.INFTP).Trim();
                obj.LOCT = StringUtils.getString(param.LOCT).Trim();
                obj.RUNTP = StringUtils.getString(param.RUNTP).Trim();
                obj.TEL = StringUtils.getString(param.TEL).Trim();
                obj.WEBSITE = StringUtils.getString(param.WEBSITE).Trim();                                
                obj.TAGID = cs.getTagidByDatetime();
                db.CTINF.Add(obj);
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
            i = db.Database.ExecuteSqlCommand("delete from CTINF where TAGID={0}", StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}