
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
public class Frm105Service 
{
    private RENTEntities db = new RENTEntities();

    public JObject getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc") 
    {
        JArray ja = new JArray();
        var query = db.EQUIP.AsQueryable();
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        //query = query.Skip((page - 1) * pageSize).Take(pageSize);            //分頁  
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},                
                        {"MAKR",StringUtils.getString(item.MAKR)},
                        {"SPEC",StringUtils.getString(item.SPEC)},
                        {"KIND",StringUtils.getString(item.KIND)},
                        {"ASET",StringUtils.getString(item.ASET)}
                    };
            ja.Add(itemObject);
        }
        JObject jo = new JObject();
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            jo.Add("rows", ja);
            jo.Add("status", "1");
        }
        else
        {
            jo.Add("total","");
            jo.Add("rows", "");
            jo.Add("status", "0");            
        }
    
       return jo;
    }


    public JArray getDatagrid2(String CNO)
    {
        JArray ja = new JArray();
        var query = (from t in db.RMEQUIP select t);
        if (CNO != null)
        {
            query = query.Where(q => q.EQUIPNO == CNO);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {   
                        {"RMNO",StringUtils.getString(item.RMNO)},                
                        {"QTY",StringUtils.getString(item.QTY)}                        
                    };
            ja.Add(itemObject);
        }
        return ja;
    }

    public JArray getDatagrid2Sum(String CNO)
    {
        JArray ja = new JArray();
        var query = db.RMEQUIP.Where(q => q.EQUIPNO == CNO).AsEnumerable().Sum(o => o.QTY);            
            var itemObject = new JObject
                    {   
                        {"RMNO","總數量"},
                        {"QTY",StringUtils.getString(query)}                                     
                    };
            ja.Add(itemObject);
        return ja;
    }

   public int getTotalCount()
   {
       return db.EQUIP.Count();            
   }




   public int doSave(String mode, EQUIP param) 
   {       
       int i = 0;
       var query = (from t in db.EQUIP select t);
       if (param.CNO != null)
       {
           EQUIP obj = null;
           if (mode.Equals("add"))
           {
               obj = new EQUIP();
               obj.CNO = param.CNO;
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          obj.KIND = StringUtils.getString(param.KIND);
          obj.MAKR = StringUtils.getString(param.MAKR);
          obj.SPEC = StringUtils.getString(param.SPEC);
          obj.ASET = StringUtils.getString(param.ASET);
          if (mode.Equals("add"))
          {
              db.EQUIP.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }



   public int doRemove(String cno)
   {
       int i=1;

       EQUIP obj = (from c in db.EQUIP 
                     where c.CNO == cno
                 select c).Single();
       db.EQUIP.Remove(obj);
       db.SaveChanges();
       return i;
   }


}
}