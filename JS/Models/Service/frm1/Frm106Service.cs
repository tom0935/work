
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
using System.Linq.Dynamic;
using Jasper.Models.Service;
using Jasper.Models.Poco;
/*
*****************************
仲介資料維護 BROKER
*****************************
仲介編號 CNO
仲介名稱CNM
統一編號 CPID
地址 ADR
電話 TEL
傳真 FAX
電子信箱 EMAIL 
 * */
namespace Jasper.service.frm1{
public class Frm106Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.BROKER.AsQueryable();
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
                        {"CPID",StringUtils.getString(item.CPID)},
                        {"ADR",StringUtils.getString(item.ADR)},
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"FAX",StringUtils.getString(item.FAX)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)}
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


    public JArray getDatagrid2(String CNO)
    {
        JArray ja = new JArray();
        var query = (from t in db.BROKMAN select t);
        if (CNO != null)
        {
            query = query.Where(q => q.BROKNO == CNO).Where(q=> q.BROKNO !=null);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {   
                        {"MAN",StringUtils.getString(item.MAN)},                
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)},
                        {"TAGID",StringUtils.getString(item.TAGID)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }



   public int getTotalCount()
   {
       return db.BROKER.Count();            
   }




   public int doSave(String mode, BROKER param) 
   {       
       int i = 0;
       var query = (from t in db.BROKER select t);
       if (param.CNO != null)
       {
           BROKER obj = null;
           if (mode.Equals("add"))
           {
               obj = new BROKER();
               obj.CNO = param.CNO;
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          obj.CPID = StringUtils.getString(param.CPID);
          obj.TEL = StringUtils.getString(param.TEL);
          obj.FAX = StringUtils.getString(param.FAX);
          obj.ADR = StringUtils.getString(param.ADR);
          obj.EMAIL = StringUtils.getString(param.EMAIL);
          if (mode.Equals("add"))
          {
              db.BROKER.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }

   public int doSave2(String mode, BROKMAN param)
   {
       int i = 0;
       var query = (from t in db.BROKMAN select t);
       if (param.BROKNO != null)
       {
           BROKMAN obj = null;
           if (mode.Equals("add"))
           {
               obj = new BROKMAN();
               obj.BROKNO = param.BROKNO;               
               obj.TAGID = cs.getTagidByDatetime();
           }
           else
           {
               obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           }
           obj.MAN = StringUtils.getString(param.MAN);
           obj.TEL = StringUtils.getString(param.TEL);
           obj.EMAIL = StringUtils.getString(param.EMAIL);
           if (mode.Equals("add"))
           {
               db.BROKMAN.Add(obj);
           }
           i = db.SaveChanges();
       }
       return i;
   }

   public int doRemove(String cno)
   {
       int i=1;

       BROKER obj = (from c in db.BROKER 
                     where c.CNO == cno
                 select c).Single();
       db.BROKER.Remove(obj);
       db.SaveChanges();
       return i;
   }

   public int doRemove2(String tagid)
   {
       int i = 1;

       BROKMAN obj = (from c in db.BROKMAN
                       where c.TAGID == tagid
                       select c).Single();
       db.BROKMAN.Remove(obj);
       db.SaveChanges();

       return i;
   }

}
}