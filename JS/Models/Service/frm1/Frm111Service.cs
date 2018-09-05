
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
/* 
合約公司編號 CNO 
公司名稱 CNM
英文名稱 ENM
行業別 RUNTP
CSTTP
來源別 COMETP
客戶等級 CSTLEVL
統一編號 CPID
負責人 CPMGR
公司地址 CPADR
送件地址 SENDADR
電話 TEL
傳真 FAX
EMAIL
 */
namespace Jasper.service.frm1{
public class Frm111Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.DEALER.AsQueryable();
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序  
            if (param.rows != 0)
            {
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
            }
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},
                        {"ENM",StringUtils.getString(item.ENM)},
                        {"RUNTP",StringUtils.getString(item.RUNTP)},
                        {"CSTTP",StringUtils.getString(item.CSTTP)},
                        {"COMETP",StringUtils.getString(item.COMETP)},
                        {"CSTLEVL",StringUtils.getString(item.CSTLEVL)},
                        {"CPID",StringUtils.getString(item.CPID)},
                        {"CPMGR",StringUtils.getString(item.CPMGR)},
                        {"CPADR",StringUtils.getString(item.CPADR)},
                        {"SENDADR",StringUtils.getString(item.SENDADR)},
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"FAX",StringUtils.getString(item.FAX)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)}
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

    public JArray getDatagrid2(String CNO)
    {
        JArray ja = new JArray();
        var query = (from t in db.DEALMAN select t);
        if (CNO != null)
        {
            query = query.Where(q => q.DEALNO == CNO);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {   
                        {"DEALNO",StringUtils.getString(item.DEALNO)},
                        {"MAN",StringUtils.getString(item.MAN)},                
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)},
                        {"NOTES",StringUtils.getString(item.NOTES)},
                        {"TAGID",StringUtils.getString(item.TAGID)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }




   public int getTotalCount()
   {
       return db.DEALER.Count();            
   }


   public int doSave(String mode, DEALER param) 
   {       
       int i = 0;
       var query = (from t in db.DEALER select t);
       if (param.CNO != null)
       {
           DEALER obj = null;
           if (mode.Equals("add"))
           {
               obj = new DEALER();
               obj.CNO = param.CNO;
               
           }
           else
           {                
               obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
           }    
          obj.CNM = StringUtils.getString(param.CNM);
          obj.COMETP = StringUtils.getString(param.COMETP);
          obj.TEL = StringUtils.getString(param.TEL);
          obj.FAX = StringUtils.getString(param.FAX);
          obj.CPADR = StringUtils.getString(param.CPADR);
          obj.CPID = StringUtils.getString(param.CPID);
          obj.CPMGR = StringUtils.getString(param.CPMGR);
          obj.ENM = StringUtils.getString(param.ENM);
          obj.EMAIL = StringUtils.getString(param.EMAIL);
          obj.CSTLEVL = param.CSTLEVL;
          obj.CSTTP = param.CSTTP;
          obj.RUNTP = param.RUNTP;
          obj.SENDADR = param.SENDADR;
           
          if (mode.Equals("add"))
          {
              db.DEALER.Add(obj);
          }
          i= db.SaveChanges();
       }        
       return i;
   }


   public int doSave2(String mode, DEALMAN param)
   {
       int i = 0;
       var query = (from t in db.DEALMAN select t);
       DEALMAN obj = null;
       
       if (param.TAGID != null)
       {
           obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();           
           obj.TEL = StringUtils.getString(param.TEL);
           obj.MAN = StringUtils.getString(param.MAN);
           obj.EMAIL = StringUtils.getString(param.EMAIL);
           obj.NOTES = StringUtils.getString(param.NOTES);
       }
       else
       {
           obj = new DEALMAN();
           obj.DEALNO = param.DEALNO;
           obj.TAGID = cs.getTagidByDatetime();
           obj.TEL = StringUtils.getString(param.TEL);
           obj.MAN = StringUtils.getString(param.MAN);
           obj.EMAIL = StringUtils.getString(param.EMAIL);
           obj.NOTES = StringUtils.getString(param.NOTES);
           db.DEALMAN.Add(obj);
       }

       i = db.SaveChanges();
       return i;
   }



   public int doRemove(String cno)
   {
       int i = 1;

       DEALER obj = (from c in db.DEALER
                       where c.CNO == cno
                       select c).Single();
       db.DEALER.Remove(obj);
       db.SaveChanges();
       return i;
   }

   public int doRemove2(String tagid)
   {
       int i=1;

       DEALMAN obj = (from c in db.DEALMAN
                     where c.TAGID == tagid
                 select c).Single();
       db.DEALMAN.Remove(obj);
    db.SaveChanges();

       return i;
   }


}
}