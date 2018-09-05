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
public class Frm115Service 
{
    private RENTEntities db = new RENTEntities();

    public JArray getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc") 
    {
        JArray ja = new JArray();


        var query1 = from t in db.EMPLOYEE select t;
        
        query1 = query1.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query1 = query1.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

        foreach (var item in query1)
        {
            var itemObject = new JObject
                    {                                           
                        {"USERID",StringUtils.getString(item.USERID)},
                        {"USERNAME",StringUtils.getString(item.USERNAME)},
                        {"ROLEID",StringUtils.getString(item.ROLEID)},
                        {"PASSWORD",StringUtils.getString(item.PASSWORD)},
                        {"UUID",StringUtils.getString(item.UUID)}
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
       return db.EMPLOYEE.Count();            
   }




   public int doSave(String mode, EMPLOYEE param) 
   {       
       int i = 0;
       var query = (from t in db.EMPLOYEE select t);
       if (param.USERID != null)
       {
           EMPLOYEE obj = null;
           if (mode.Equals("add"))
           {
               obj = new EMPLOYEE();
               obj.USERID = StringUtils.getString(param.USERID);
           }
           else
           {                
               obj = query.Where(q => q.UUID == param.UUID).SingleOrDefault();
           }
          
          obj.USERNAME = StringUtils.getString(param.USERNAME);
          obj.PASSWORD = StringUtils.getString(param.PASSWORD);
          obj.ROLEID = param.ROLEID;
          if (mode.Equals("add"))
          {
              db.EMPLOYEE.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }



   public int doRemove(int UUID)
   {
       int i=1;

       EMPLOYEE obj = (from c in db.EMPLOYEE
                     where c.UUID == UUID
                 select c).Single();
       db.EMPLOYEE.Remove(obj);
       db.SaveChanges();
       return i;
   }


}
}