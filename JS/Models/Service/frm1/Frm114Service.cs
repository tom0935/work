
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
/* 
廠商編號 CNO 
廠商名稱 CNM
SDEP
行業別 RUNTP
CPID
CPMGR
CPADR
TEL
FAX
EMAIL
 */
namespace Jasper.service.frm1{
public class Frm114Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.MAKR.AsQueryable();
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            if (param.rows != 0)
            {
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
            }  
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},
                        {"CPADR",StringUtils.getString(item.CPADR)},
                        {"RUNTP",StringUtils.getString(item.RUNTP)},
                        {"CPID",StringUtils.getString(item.CPID)},
                        {"CPMGR",StringUtils.getString(item.CPMGR)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)},                        
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"FAX",StringUtils.getString(item.FAX)},                        
                        {"SDEP",StringUtils.getString(item.SDEP)}
                    };
            ja.Add(itemObject);
        }

        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }

        return jo;
    }

    public JArray getDatagrid2(String cno)
    {
        JArray ja = new JArray();
        var query = (from t in db.MAKRMAN select t);
        if (cno != null)
        {
            query = query.Where(q => q.MAKRNO == cno);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {                           
                        {"MAN",StringUtils.getString(item.MAN)},
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)},                        
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"MAKRNO",StringUtils.getString(item.MAKRNO)},
                        {"NOTES",StringUtils.getString(item.NOTES)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }




   public int getTotalCount()
   {
       return db.DEALER.Count();            
   }


   public int doSave(String mode, MAKR param) 
   {       
       int i = 0;
       var query = (from t in db.MAKR select t);
       if (param.CNO != null)
       {
           MAKR obj = null;
           if (mode.Equals("add"))
           {
               obj = new MAKR();
               obj.CNO = param.CNO;
               
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          obj.CPADR = StringUtils.getString(param.CPADR);
          obj.TEL = StringUtils.getString(param.TEL);
          obj.FAX = StringUtils.getString(param.FAX);          
          obj.CPID = StringUtils.getString(param.CPID);
          obj.CPMGR = StringUtils.getString(param.CPMGR);
          obj.EMAIL = StringUtils.getString(param.EMAIL);
          obj.RUNTP = StringUtils.getString(param.RUNTP);
          obj.SDEP = StringUtils.getString(param.SDEP);

           
           
          if (mode.Equals("add"))
          {
              db.MAKR.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }


   public int doSave2(String mode, MAKRMAN param)
   {
       int i = 0;
       var query = (from t in db.MAKRMAN select t);
       MAKRMAN obj = null;
       if (param.TAGID != null)
       {
           obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           obj.TEL = StringUtils.getString(param.TEL);
           obj.MAN = StringUtils.getString(param.MAN);
           obj.EMAIL = StringUtils.getString(param.EMAIL);
           obj.NOTES = StringUtils.getString(param.NOTES);
       }
       else
       {
           obj = new MAKRMAN();
           obj.MAKRNO = param.MAKRNO;
           obj.TAGID = cs.getTagidByDatetime();
           obj.TEL = StringUtils.getString(param.TEL);
           obj.MAN = StringUtils.getString(param.MAN);
           obj.EMAIL = StringUtils.getString(param.EMAIL);
           obj.NOTES = StringUtils.getString(param.NOTES);
           db.MAKRMAN.Add(obj);  
       }


       i = db.SaveChanges();
       return i;
   }



   public int doRemove(String cno)
   {
       int i = 1;

       MAKR obj = (from c in db.MAKR
                       where c.CNO == cno
                       select c).Single();
       db.MAKR.Remove(obj);
       db.SaveChanges();
       return i;
   }

   public int doRemove2(String tagid)
   {
       int i=1;

       MAKRMAN obj = (from c in db.MAKRMAN
                     where c.TAGID == tagid
                 select c).Single();
       db.MAKRMAN.Remove(obj);
    db.SaveChanges();

       return i;
   }


}
}