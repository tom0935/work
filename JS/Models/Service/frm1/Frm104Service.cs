
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
public class Frm104Service 
{
    private RENTEntities db = new RENTEntities();

    public JArray getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc") 
    {
        JArray ja = new JArray();
        var query = db.COUNTRY.AsQueryable();
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query = query.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},                
                        {"CNTRYAREA",StringUtils.getString(item.CNTRYAREA)}
                    };
            ja.Add(itemObject);
        }
    
       return ja;
    }




   public int getTotalCount()
   {
       return db.COUNTRY.Count();            
   }




   public int doSave(String mode, COUNTRY param) 
   {       
       int i = 0;
       var query = (from t in db.COUNTRY select t);
       if (param.CNO != null)
       {
           COUNTRY obj = null;
           if (mode.Equals("add"))
           {
               obj = new COUNTRY();
               obj.CNO = param.CNO;
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          if (mode.Equals("add"))
          {
              db.COUNTRY.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }



   public int doRemove(String cno)
   {
       int i=1;

       COUNTRY obj = (from c in db.COUNTRY 
                     where c.CNO == cno
                 select c).Single();
       db.COUNTRY.Remove(obj);
       db.SaveChanges();
       return i;
   }


}
}