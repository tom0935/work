
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
/*
*****************************
房屋牌價維護  PBRENTAL
*****************************
年度 YR
日期起 DT1
日期迄 DT2
房號 RMNO
RENTAL 月租費
MGFEE 管理費
TAGID
 * */
namespace Jasper.service.frm1{
public class Frm107Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

    public JObject getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc",String q="") 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = from t in db.ROOMF  select t;
        if (q != "")
        {
            query = query.Where(t => t.CNO.Contains(q) || t.DOOR.Contains(q));
        }
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序    
            
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNO2",StringUtils.getString(item.CNO)},
                        {"DOOR",StringUtils.getString(item.DOOR)},
                        {"FLOR",StringUtils.getString(item.FLOR)}
                        
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
        var query = (from t in db.PBRENTAL select t);
        if (CNO != null)
        {
            query = query.Where(q => q.RMNO == CNO);
        }
        foreach (var item in query.ToList())
        {
            DateTime dt = new DateTime();
            
            var itemObject = new JObject
                    {   
                        {"YR",StringUtils.getString(item.YR)},
                        //{"DT1",StringUtils.getString(item.DT1)},                
                 {"DT1",cs.getDateTimeByView(item.DT1)},  
                       // {"DT2",StringUtils.getString(item.DT2)},
                 {"DT2",cs.getDateTimeByView(item.DT2)},
                        {"RMNO",StringUtils.getString(item.RMNO)},
                        {"RENTAL",StringUtils.getString(item.RENTAL)},
                        {"MGFEE",StringUtils.getString(item.MGFEE)},
                        {"TAGID",StringUtils.getString(item.TAGID)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }



   public int getTotalCount()
   {
       return db.PBRENTAL.Count();            
   }




   public int doSave(String mode, PBRENTAL param) 
   {       
       int i = 0;
       var query = (from t in db.PBRENTAL select t);
       if (param.RMNO != null)
       {
           PBRENTAL obj = null;
           if (mode.Equals("add"))
           {
               obj = new PBRENTAL();
               obj.RMNO = param.RMNO;
               obj.TAGID =param.RMNO + " "+cs.getDateTimeStr(param.DT1); //牌價pk編碼(房屋編號 + 空白 + 日期起)
           }
           else
           {                
               obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           }    
           
          obj.YR = StringUtils.getString(param.YR);
          obj.DT1 = param.DT1;
          obj.DT2 = param.DT2;
          obj.RENTAL = param.RENTAL;
          obj.MGFEE = param.MGFEE;
          if (mode.Equals("add"))
          {
              db.PBRENTAL.Add(obj);
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