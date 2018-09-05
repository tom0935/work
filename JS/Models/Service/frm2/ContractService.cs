
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
using Jasper.util;
/* 
廠商編號 CNO 
廠商名稱 CNM
SDEP
行業別 RUNTP
CPID
CPMGR
CPADR
TEL
FAX
EMAIL
 */
namespace Jasper.service.frm2{
    public class ContractService 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

  
 

   public JObject getContactDG(String TAGID)
   {   
       JArray ja = new JArray();
       JObject jo = new JObject();
       var query = (from t in db.CONTRAH where t.TAGID == TAGID && t.CNSTA == "" group t by new { t.TAGID, t.CNHNO } into g select new { CNO = "1", CNNO = g.Key.CNHNO, NAME = "房屋合約",RNTP="主約", TAGID = g.Key.TAGID, COUNT = g.Count() }).Union
           (from t in db.CONTRAP where t.JOBTAG == TAGID && t.CNSTA == "" group t by new { t.JOBTAG, t.CNPNO,t.RNTP } into g select new { CNO = "3", CNNO = g.Key.CNPNO, NAME = "車位合約",RNTP=(g.Key.RNTP==1?"主約":"另約") , TAGID = g.Key.JOBTAG, COUNT = g.Count() }).Union
                   (from t in db.CONTRAF where t.JOBTAG == TAGID && t.CNSTA == "" group t by new { t.JOBTAG, t.CNFNO, t.RNTP } into g select new { CNO = "4", CNNO = g.Key.CNFNO, NAME = "傢俱合約", RNTP = (g.Key.RNTP == 1 ? "主約" : "另約"), TAGID = g.Key.JOBTAG, COUNT = g.Count() }).Union
                   (from t in db.CONTRAC where t.JOBTAG == TAGID && t.CNSTA == "" group t by new { t.JOBTAG, t.CNCNO, t.RNTP } into g select new { CNO = "5", CNNO = g.Key.CNCNO, NAME = "俱樂部合約", RNTP = (g.Key.RNTP == 1 ? "主約" : "另約"), TAGID = g.Key.JOBTAG, COUNT = g.Count() }).Union
                   (from t in db.CONTRAA where t.JOBTAG == TAGID && t.CNSTA == "" group t by new { t.JOBTAG, t.CNANO, t.RNTP } into g select new { CNO = "*", CNNO = g.Key.CNANO, NAME = "雜項合約", RNTP = (g.Key.RNTP == 1 ? "主約" : "另約"), TAGID = g.Key.JOBTAG, COUNT = g.Count() });
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO).Trim()},
                        {"CNNO",StringUtils.getString(item.CNNO).Trim()},
                        {"NAME",StringUtils.getString(item.NAME).Trim()},
                        {"RNTP",StringUtils.getString(item.RNTP).Trim()},
                        {"TAGID",StringUtils.getString(item.TAGID).Trim()},
                        {"CNT",StringUtils.getString(item.COUNT).Trim()}
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


   public JObject getFeeformDG(String TAGID,String FEEYM,String CNNO)
   {
       JArray ja = new JArray();
       JArray ja2 = new JArray();
       JObject jo = new JObject();
       /*
       var query = (from t in db.CONTRAH where t.TAGID == TAGID group t.TAGID by t.TAGID into g select new { CNO = "1", NAME = "房屋合約", TAGID = g.Key, COUNT = g.Count() }).Union
                   (from t in db.CONTRAP where t.JOBTAG == TAGID group t.JOBTAG by t.JOBTAG into g select new { CNO = "2", NAME = "車位合約", TAGID = g.Key, COUNT = g.Count() }).Union
                   (from t in db.CONTRAF where t.JOBTAG == TAGID group t.JOBTAG by t.JOBTAG into g select new { CNO = "3", NAME = "傢俱合約", TAGID = g.Key, COUNT = g.Count() }).Union
                   (from t in db.CONTRAC where t.JOBTAG == TAGID group t.JOBTAG by t.JOBTAG into g select new { CNO = "4", NAME = "俱樂部合約", TAGID = g.Key, COUNT = g.Count() }).Union
                   (from t in db.CONTRAA where t.JOBTAG == TAGID group t.JOBTAG by t.JOBTAG into g select new { CNO = "5", NAME = "雜項合約", TAGID = g.Key, COUNT = g.Count() });
        * */
       //var query=from t in db.FEEFORM where t.
       var query = from t in db.RMFEEM 
                   join b in db.CONFCODE on t.FEETP equals b.CODE_NO into bg
                   from b1 in bg.DefaultIfEmpty()
                   where b1.CODE_KIND=="contract" && t.RMTAG == TAGID && t.FEEYM==FEEYM 
                   group t by new{t.FEETP,t.FEEYM,b1.CODE_NAME,t.CNNO,t.RMNO}  into g
                   select new { FEEYM=g.Key.FEEYM,RMNO=g.Key.RMNO,CNNO=g.Key.CNNO,FEETP = g.Key.FEETP ,FEETP_NAME=g.Key.CODE_NAME ,FEEAMT = g.Sum(t => t.FEEAMT) , AMTTX=g.Sum(t=>t.AMTTX) ,AMTSUM=g.Sum(t=>t.AMTSUM)};
       
       if (StringUtils.getString(CNNO) != "")
       {
           query = query.Where(t => t.CNNO==CNNO);
       }
        query = query.OrderBy(string.Format("{0} {1}", "FEETP", "asc")); //排序
  
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                                           
                       {"FEETP",item.FEETP.Trim()},
                       {"CNNO",item.CNNO.Trim()},
                       {"FEEYM",item.FEEYM.Trim()},
                       {"FEETP_NAME",item.FEETP_NAME},
                       {"FEEAMT",item.FEEAMT},
                       {"AMTTX",item.AMTTX},
                       {"AMTSUM",item.AMTSUM}
                       
                    };
           ja.Add(itemObject);
       }
       var totQuery = from t in query group t by t.RMNO into g select new { FEEAMT = g.Sum(t => t.FEEAMT), AMTTX = g.Sum(t => t.AMTTX), AMTSUM = g.Sum(t => t.AMTSUM) };
       foreach (var item in totQuery)
       {
           JObject itemObject2 = new JObject
                    {   
                       {"FEETP",""},
                       {"FEETP_NAME","合計:"},
                       {"FEEAMT",item.FEEAMT},
                       {"AMTTX",item.AMTTX},
                       {"AMTSUM",item.AMTSUM} 
                    };
           ja2.Add(itemObject2);
       }
       if (query.Count() > 0)
       {
           jo.Add("rows", ja);
           jo.Add("total", query.Count());
           jo.Add("footer", ja2);
       }
       else
       {
           jo.Add("rows", "");
           jo.Add("total", "");
       }
       return jo;
   }



   public JObject getFeeformDtlDG(String TAGID,String FEETP)
   {
       JArray ja = new JArray();
       JObject jo = new JObject();
       var query = from t in db.RMFEEM
                   where t.RMTAG == TAGID && t.FEETP==FEETP                   
                   select new { FEEYM = t.FEEYM, FEEAMT = t.FEEAMT, AMTTX = t.AMTTX, AMTSUM = t.AMTSUM ,TAGID=t.TAGID};

       query = query.OrderBy(string.Format("{0} {1}", "FEEYM", "asc")); //排序

       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                                           
                       {"FEEYM",item.FEEYM},                       
                       {"FEEAMT",item.FEEAMT},
                       {"AMTTX",item.AMTTX},
                       {"AMTSUM",item.AMTSUM},
                       {"TAGID",item.TAGID.Trim()}
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


   public int doSaveFeeformDG(List<RMFEEM> list)
   {
       int i = 0;
       using(db){
           int j = 0;
           foreach (RMFEEM x in list)
           {
               RMFEEM obj = (from t in db.RMFEEM where t.FEETP == x.FEETP && t.FEEYM == x.FEEYM && t.CNNO == x.CNNO select t).SingleOrDefault();
               if (StringUtils.getString(x.AMTSUM) == "")
               {
                   x.AMTSUM = 0;
               }
               obj.AMTSUM = StringUtils.getInt(x.AMTSUM);
               i += db.SaveChanges();               
               j++;
           }
       }
       return i;
   }

   public int doSaveFeeformDtlDG(List<RMFEEM> list)
   {
       int i = 0;
       using (db)
       {
           int j = 0;
           foreach (RMFEEM x in list)
           {
               RMFEEM obj = (from t in db.RMFEEM where t.TAGID == x.TAGID select t).SingleOrDefault();
               if (StringUtils.getString(x.AMTSUM) == "")
               {
                   x.AMTSUM = 0;
               }
               obj.AMTSUM = StringUtils.getInt(x.AMTSUM);
               i += db.SaveChanges();
               j++;               
           }
       }
       return i;
   }


}
}