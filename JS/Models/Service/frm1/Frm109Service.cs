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
namespace Jasper.service.frm1{
public class Frm109Service 
{
    private RENTEntities db = new RENTEntities();

    public JArray getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc") 
    {
        JArray ja = new JArray();
        /*
var result = enumerableOfSomeClass
    .Join(enumerableOfSomeOtherClass,
          sc => sc.Property1,
          soc => soc.Property2,
          (sc, soc) => new        
        */
        var query = db.ACCF.AsQueryable();        
       /* 
        query = query.GroupJoin(db.ACCTP.AsQueryable() , a => a.TP, b => b.TPNO, (a, b) =>
            new {
                CNO = a.CNO,
                CNM =a.CNM,
                TP=a.TP,
                TAGID =a.TAGID,
                TPNM = b.
                }
            );
        */
        /*
var innerJoinQuery =
    from category in categories
    join prod in products on category.ID equals prod.CategoryID
    select new { ProductName = prod.Name, Category = category.Name }; 
        */

        var query1 = from acf in db.ACCF.AsQueryable()
               join atp in db.ACCTP.AsQueryable() on acf.TP equals atp.TPNO                          
               select new { CNO = acf.CNO, CNM = acf.CNM, TP = acf.TP, TAGID = acf.TAGID, TPNM = atp.TPNM };        
        
        query1 = query1.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query1 = query1.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

        foreach (var item in query1)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},                       
                        {"TP",StringUtils.getString(item.TP)},
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"TPNM",StringUtils.getString(item.TPNM)}
                    };
            ja.Add(itemObject);
        }
    
       return ja;
    }

    public JArray getCombobox()
    {
        JArray ja = new JArray();
        var query = (from t in db.ACCTP select t);

        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {                                           
                        {"VALUE",StringUtils.getString(item.TPNO)},
                        {"ITEM",StringUtils.getString(item.TPNM)}
                    };
            ja.Add(itemObject);
        }        

        return ja;
    }



   public int getTotalCount()
   {
       return db.ACCF.Count();            
   }




   public int doSave(String mode, ACCF param) 
   {       
       int i = 0;
       var query = (from t in db.ACCF select t);
       if (param.CNO != null)
       {
           ACCF obj = null;
           if (mode.Equals("add"))
           {
               obj = new ACCF();
               obj.CNO = param.CNO;
               obj.TAGID = param.TP + param.CNO;
           }
           else
           {                
               obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           }
          obj.TP = StringUtils.getString(param.TP);
          obj.CNM = StringUtils.getString(param.CNM);
          if (mode.Equals("add"))
          {
              db.ACCF.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }



   public int doRemove(String TAGID)
   {
       int i=1;

       ACCF obj = (from c in db.ACCF
                     where c.TAGID == TAGID
                 select c).Single();
       db.ACCF.Remove(obj);
       db.SaveChanges();
       return i;
   }


}
}