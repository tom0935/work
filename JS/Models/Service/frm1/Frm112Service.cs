
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
using Jasper.Models.Service;
using System.Linq.Dynamic;
using Jasper.Models.Poco;

namespace Jasper.service.frm1{
public class Frm112Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.POSM.AsQueryable();
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},
                        {"LEVL",StringUtils.getString(item.LEVL)}
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







   public int doSave(String mode, POSM param) 
   {       
       int i = 0;
       var query = (from t in db.POSM select t);
       if (param.CNO != null)
       {
           POSM obj = null;
           if (mode.Equals("add"))
           {
               obj = new POSM();
               obj.CNO = param.CNO;
               
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          obj.LEVL = StringUtils.getString(param.LEVL);
           
          if (mode.Equals("add"))
          {
              db.POSM.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }





   public int doRemove(String cno)
   {
       int i = 1;

       POSM obj = (from c in db.POSM
                       where c.CNO == cno
                       select c).Single();
       db.POSM.Remove(obj);
       db.SaveChanges();
       return i;
   }




}
}