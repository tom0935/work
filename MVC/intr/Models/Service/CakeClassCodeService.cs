
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Dynamic;
using System.Linq;
using IntranetSystem.Models;

namespace IntranetSystem.Service
{
public class CakeClassCodeService 
{
    private Entities db = new Entities();
    
  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JArray getDatagrid(int page=1,int pageSize=20,String propertyName="CNO",String order="asc") 
    {
        JArray ja = new JArray();
        var query = db.CAKE_CLASSCODE.AsQueryable();
        
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query=query.Skip((page - 1) * pageSize).Take(pageSize);    //分頁   
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CODE1",StringUtils.getString(item.CODE1)},
                        {"NAME1",StringUtils.getString(item.NAME1)},                       
                    };
            ja.Add(itemObject);
        }
    
       return ja;
    }

    /*
    public JArray getDailog(String rmno)
    {
        JArray ja = new JArray();
        
        //var list = db.PBRENTAL.OrderBy(x => x.YR).Where(x=> x.RMNO=@rmno);
        var query = (from t in db.CAKE_ORDERS select t);
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
    */

   public int getTotalCount()
   {
       return db.CAKE_ORDERS.Count();            
   }


   public int doSave(String mode, CAKE_ORDERS param) 
   {       
       int i = 0;
       var query = (from t in db.CAKE_ORDERS select t);
       if (param.ODDNO != null)
       {
           CAKE_ORDERS obj = null;
           if (mode.Equals("add"))
           {
               obj = new CAKE_ORDERS();
               obj.ODDNO = param.ODDNO;
           }
           else
           {                
               obj = query.Where(q => q.ODDNO == param.ODDNO).SingleOrDefault();
           }

           obj.ODATE = param.ODATE;
           /*
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
            */
          if (mode.Equals("add"))
          {
         //     db.CAKE_ORDERS.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }

   public int doCreate(CAKE_ORDERS param)
   {
       int i = 0;
       /*
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
        * */
       
   //}
    return i;
   
   }

   public int doRemove(String oddno)
   {
       int i=1;
       
    CAKE_ORDERS obj = (from c in db.CAKE_ORDERS
                     where  c.ODDNO== oddno
                 select c).Single();
 //   db.CAKE_ORDERS.
    db.SaveChanges();

       return i;
   }


}
}