
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
public class Frm102Service 
{
    private RENTEntities db = new RENTEntities();
    
  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JArray getDatagrid(int page=1,int pageSize=20,String  propertyName="", String order="asc") 
    {
        JArray ja = new JArray();  
        var query = db.NEWSTP.AsQueryable();
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query = query.Skip((page - 1) * pageSize).Take(pageSize);    //分頁   

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)}                     
                    };
            ja.Add(itemObject);
        }
    
       return ja;
    }




   public int getTotalCount()
   {
       return db.NEWSTP.Count();            
   }


   public int doSave(String mode,NEWSTP param) 
   {       
       int i = 0;
       var query = (from t in db.NEWSTP select t);
       if (param.CNO != null)
       {
           NEWSTP obj=null;
           if (mode.Equals("add"))
           {
               obj = new NEWSTP();
               obj.CNO = param.CNO;
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          if (mode.Equals("add"))
          {
              db.NEWSTP.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }



   public int doRemove(String cno)
   {
       int i=1;
       
    NEWSTP obj = (from c in db.NEWSTP
                     where c.CNO == cno
                 select c).Single();
    db.NEWSTP.Remove(obj);
    db.SaveChanges();

       return i;
   }


}
}