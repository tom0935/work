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
public class Frm108Service 
{
    private RENTEntities db = new RENTEntities();

    public JArray getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc") 
    {
        JArray ja = new JArray();
        
        var query = db.ACCTP.AsQueryable();
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query = query.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"TPNO",StringUtils.getString(item.TPNO)},
                        {"TPNM",StringUtils.getString(item.TPNM)}                        
                    };
            ja.Add(itemObject);
        }
    
       return ja;
    }




   public int getTotalCount()
   {
       return db.ACCTP.Count();            
   }




   public int doSave(String mode, ACCTP param) 
   {       
       int i = 0;
       var query = (from t in db.ACCTP select t);
       if (param.TPNO != null)
       {
           ACCTP obj = null;
           if (mode.Equals("add"))
           {
               obj = new ACCTP();
               obj.TPNO = param.TPNO;
           }
           else
           {                
               obj = query.Where(q => q.TPNO == param.TPNO).SingleOrDefault();
           }    
          obj.TPNM = StringUtils.getString(param.TPNM);
          if (mode.Equals("add"))
          {
              db.ACCTP.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }



   public int doRemove(String tpno)
   {
       int i=1;

       ACCTP obj = (from c in db.ACCTP 
                     where c.TPNO == tpno
                 select c).Single();
       db.ACCTP.Remove(obj);
       db.SaveChanges();
       return i;
   }


}
}