
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
public class Frm113Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.HOMEAST.AsQueryable();
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
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"PID",StringUtils.getString(item.PID)},
                        {"SEX",StringUtils.getString(item.SEX)},
                        {"TEL1",StringUtils.getString(item.TEL1)},
                        {"TEL2",StringUtils.getString(item.TEL2)},
                        {"TEL3",StringUtils.getString(item.TEL3)},
                        {"CNM",StringUtils.getString(item.CNM)},
                        {"COUNTRY",StringUtils.getString(item.COUNTRY)},
                        {"WORKITEM",StringUtils.getString(item.WORKITEM)},
                        {"WORKNOTE",StringUtils.getString(item.WORKNOTE)},
                        {"WORKWITH",StringUtils.getString(item.WORKWITH)}                        
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







    public int doSave(String mode, HOMEAST param) 
   {       
       int i = 0;
       var query = (from t in db.HOMEAST select t);
       if (param.CNM != null)
       {
           HOMEAST obj = null;
           if (mode.Equals("add"))
           {
               obj = new HOMEAST();
               obj.TAGID = cs.getTagidByDatetime();
               
           }
           else
           {                
               obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           }    
          obj.PID = StringUtils.getString(param.PID);
          obj.TEL1 = StringUtils.getString(param.TEL1);
          obj.TEL2 = StringUtils.getString(param.TEL2);
          obj.TEL3 = StringUtils.getString(param.TEL3);
          obj.SEX = StringUtils.getString(param.SEX);
          obj.COUNTRY = StringUtils.getString(param.COUNTRY);
          obj.CNM = StringUtils.getString(param.CNM);
          obj.WORKITEM = StringUtils.getString(param.WORKITEM);
          obj.WORKNOTE = StringUtils.getString(param.WORKNOTE);
          obj.WORKWITH = StringUtils.getString(param.WORKWITH);
           
          if (mode.Equals("add"))
          {
              db.HOMEAST.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }





   public int doRemove(String tagid)
   {
       int i = 1;

       HOMEAST obj = (from c in db.HOMEAST
                       where c.TAGID == tagid
                       select c).Single();
       db.HOMEAST.Remove(obj);
       db.SaveChanges();
       return i;
   }




}
}