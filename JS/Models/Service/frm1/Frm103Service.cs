
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
/* 
報社編號 CNO
報社名稱 CNM
連絡人 CNTACT
公司電話 TEL
傳真 FAX
行動電話 MTEL
 */
namespace Jasper.service.frm1{
public class Frm103Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JArray getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc") 
    {
        JArray ja = new JArray();
        var query = db.PUBLISHER.AsQueryable();
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
        query = query.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},
                        {"CNTACT",StringUtils.getString(item.CNTACT)},
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"FAX",StringUtils.getString(item.FAX)},
                        {"MTEL",StringUtils.getString(item.MTEL)}
                    };
            ja.Add(itemObject);
        }
    
       return ja;
    }

    public JArray getDatagrid2(String CNO)
    {
        JArray ja = new JArray();        
        var query = (from t in db.PUBLISHF select t);
        if (CNO != null)
        {
            query = query.Where(q => q.CNO == CNO);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {   
                        {"CNO",StringUtils.getString(item.CNO)},                
                        {"PUBLISHS",StringUtils.getString(item.PUBLISHS)},
                        {"TAGID",StringUtils.getString(item.TAGID)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }




   public int getTotalCount()
   {
       return db.PUBLISHER.Count();            
   }


   public int doSave(String mode, PUBLISHER param) 
   {       
       int i = 0;
       var query = (from t in db.PUBLISHER select t);
       if (param.CNO != null)
       {
           PUBLISHER obj = null;
           if (mode.Equals("add"))
           {
               obj = new PUBLISHER();
               obj.CNO = param.CNO;
               
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          obj.CNTACT = StringUtils.getString(param.CNTACT);
          obj.TEL = StringUtils.getString(param.TEL);
          obj.FAX = StringUtils.getString(param.FAX);
          obj.MTEL = StringUtils.getString(param.MTEL);
          if (mode.Equals("add"))
          {
              db.PUBLISHER.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }


   public int doSave2(String mode, PUBLISHF param)
   {
       int i = 0;        
       var query = (from t in db.PUBLISHF select t);
       if (param.CNO != null)
       {
           PUBLISHF obj = null;
           if (mode.Equals("add"))
           {
               obj = new PUBLISHF();
               obj.CNO = param.CNO;
               obj.TAGID = cs.getTagidByDatetime();
           }
           else
           {
               obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           }
           obj.PUBLISHS = StringUtils.getString(param.PUBLISHS);
           if (mode.Equals("add"))
           {
               db.PUBLISHF.Add(obj);
           }
           i = db.SaveChanges();
       }
       return i;
   }



   public int doRemove(String cno)
   {
       int i = 1;

       PUBLISHER obj = (from c in db.PUBLISHER
                       where c.CNO == cno
                       select c).Single();
       db.PUBLISHER.Remove(obj);
       db.SaveChanges();
       return i;
   }

   public int doRemove2(String tagid)
   {
       int i=1;

       PUBLISHF obj = (from c in db.PUBLISHF
                     where c.TAGID == tagid
                 select c).Single();
       db.PUBLISHF.Remove(obj);
    db.SaveChanges();

       return i;
   }


}
}