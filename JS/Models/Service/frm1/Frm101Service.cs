
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
using System.Linq.Dynamic;
using System.Linq;
using Jasper.Models.Poco;
namespace Jasper.service.frm1{
public class Frm101Service 
{
    private RENTEntities db = new RENTEntities();
    
  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = from t in db.ROOMF select t;

        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
        }
//        var list = db.ROOMF.OrderBy(x => x.CNO);       
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"PINS",StringUtils.getString(item.PINS)},                       
                        {"ADR1",StringUtils.getString(item.ADR1)},
                        {"DOOR",StringUtils.getString(item.DOOR)},
                        {"FLOR",StringUtils.getString(item.FLOR)},
                        {"PINS0",StringUtils.getString(item.PINS0)},
                        {"PRCN",StringUtils.getString(item.PRCN)},
                        {"EXTNO",StringUtils.getString(item.EXTNO)},
                        {"LOCT",StringUtils.getString(item.LOCT)},
                        {"STY",StringUtils.getString(item.STY)},
                        {"PICNM",StringUtils.getString(item.PICNM)},
                        {"IP1",StringUtils.getString(item.IP1)},
                        {"IP2",StringUtils.getString(item.IP2)},
                        {"WATERNO",StringUtils.getString(item.WATERNO)},
                        {"ELECNO",StringUtils.getString(item.ELECNO)},
                        {"GASNO",StringUtils.getString(item.GASNO)}
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

    public JArray getDailog(String rmno)
    {
        JArray ja = new JArray();
        
        //var list = db.PBRENTAL.OrderBy(x => x.YR).Where(x=> x.RMNO=@rmno);
        var query = (from t in db.PBRENTAL select t);
        if (rmno != null)
        {
            query = query.Where(q => q.RMNO == rmno);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {                                           
                        {"YR",StringUtils.getString(item.YR)},
                        {"DT1",StringUtils.getString(item.DT1)},
                        {"DT2",StringUtils.getString(item.DT2)},
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
    return db.ROOMF.Count();            
   }


   public int doSave(String mode,ROOMF param) 
   {       
       int i = 0;
       var query = (from t in db.ROOMF select t);
       if (param.CNO != null)
       {
           ROOMF obj=null;
           if (mode.Equals("add"))
           {
               obj = new ROOMF();
               obj.CNO = param.CNO;
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.PINS = StringUtils.getDecimal(param.PINS);
          obj.ADR1 = StringUtils.getString(param.ADR1);
          obj.DOOR = StringUtils.getString(param.DOOR);
          obj.FLOR = StringUtils.getString(param.FLOR);
          obj.PINS0 =StringUtils.getDecimal(param.PINS0);
          obj.PRCN = StringUtils.getDecimal(param.PRCN);
          obj.EXTNO =StringUtils.getString(param.EXTNO);
          obj.LOCT = StringUtils.getString(param.LOCT);
          obj.STY = StringUtils.getString(param.STY);
          obj.PICNM = StringUtils.getString(param.PICNM);
          obj.IP1 = StringUtils.getString(param.IP1);
          obj.IP2 = StringUtils.getString(param.IP2);
          obj.WATERNO = StringUtils.getString(param.WATERNO);
          obj.ELECNO = StringUtils.getString(param.ELECNO);
          obj.GASNO = StringUtils.getString(param.GASNO);
          if (mode.Equals("add"))
          {
              db.ROOMF.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }

   public int doCreate(ROOMF param)
   {
       int i = 0;
     if (param.CNO != null) { 
       ROOMF obj = new ROOMF();
       obj.CNO = param.CNO;
       obj.PINS = param.PINS;
       obj.ADR1 = param.ADR1;
       obj.DOOR = param.DOOR;
       obj.FLOR = param.FLOR;
       obj.PINS0 = param.PINS0;
       obj.PRCN = param.PRCN;
       obj.EXTNO = param.EXTNO;
       obj.LOCT = param.LOCT;
       obj.STY = param.STY;
       obj.PICNM = param.PICNM;
       obj.IP1 = param.IP1;
       obj.IP2 = param.IP2;
       obj.WATERNO = param.WATERNO;
       obj.ELECNO = param.ELECNO;
       obj.GASNO = param.GASNO;       
       db.ROOMF.Add(obj);
       i = db.SaveChanges();
   }

       return i;
   }

   public int doRemove(String cno)
   {
       int i=1;
       
    ROOMF obj = (from c in db.ROOMF
                     where c.CNO == cno
                 select c).Single();
    db.ROOMF.Remove(obj);
    db.SaveChanges();

       return i;
   }


}
}