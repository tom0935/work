
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
using System.Data.Objects;

namespace Jasper.service.frm2{
public class Frm201fService 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String TAGID)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime? tmp = null;
        /*
SELECT CNM,SEX,BIRTH,EMAIL,CIDT,TPNM,CNTRY,BLODTP,LOGUSR,LOGDT, " + ;
		"CODT,PID,POSL,MARRI,RSTA,JOBTAG,TAGID,CNHNO,CNCNO,TPNO,MFEE,NOTES,RMNO FROM MBRM " + ;
		"WHERE JOBTAG = ?W_JOBTAG AND RSTA = ' ' " + ;
		"ORDER BY CIDT"*/

        var query = from t in db.MBRM where t.JOBTAG == TAGID && t.RSTA == " " orderby t.CIDT select new {
            CNM=t.CNM,SEX=t.SEX,BIRTH=t.BIRTH,EMAIL=t.EMAIL,CIDT=t.CIDT,TPNM=t.TPNM,CNTRY=t.CNTRY,BLODTP=t.BLODTP,
            LOGUSR=t.LOGUSR,LOGDT=t.LOGDT,CODT=t.CODT,PID=t.PID,POSL=t.POSL,MARRI=t.MARRI,RSTA=t.RSTA,JOBTAG=t.JOBTAG,TAGID=t.TAGID,
            CNHNO=t.CNHNO,CNCNO=t.CNCNO,TPNO=t.TPNO,MFEE=t.MFEE,NOTES=t.NOTES,RMNO=t.RMNO
           };
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNM",item.CNM.Trim()},
                        {"SEX",item.SEX},
                        {"BIRTH",String.Format("{0:yyyy-MM-dd}", item.BIRTH)},
                        {"EMAIL",item.EMAIL},
                        {"CIDT",String.Format("{0:yyyy-MM-dd}", item.CIDT)},
                        //{"TPNM",item.TPNM.Trim()},
                        {"CNTRY",item.CNTRY.Trim()},
                        {"BLODTP",item.BLODTP},
                        {"LOGUSR",item.LOGUSR},
                        {"LOGDT",String.Format("{0:yyyy-MM-dd}", item.LOGDT)},
                        {"CODT",String.Format("{0:yyyy-MM-dd}", item.CODT)},
                        {"PID",item.PID},
                        {"POSL",item.POSL},
                        {"MARRI",item.MARRI},
                        {"TAGID",item.TAGID},
                        {"CNHNO",item.CNHNO},
                        {"CNCNO",item.CNCNO},
                        {"TPNO",item.TPNO},
                        {"MFEE",item.MFEE},
                        {"NOTES",item.NOTES},
                        {"RMNO",item.RMNO}
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
            jo.Add("total", query.Count());
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;
    }



      public JObject getCNM(String JOBTAG)
      {
          JObject jo = new JObject();
          JArray ja = new JArray();


        var query = (from t in db.RMLIV where t.JOBTAG == JOBTAG && t.RSTA == " " select new { TYPE = "住戶", USER =t.MAN  }).Union
            (from t in db.RMCAR where t.JOBTAG == JOBTAG && t.RSTA == " " select new { TYPE = "司機",USER=t.DRIVER }).Distinct().Union
            (from t in db.RMAST where t.JOBTAG == JOBTAG && t.RSTA == " " select new { TYPE = "幫傭", USER = t.ASST}).Distinct();

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"TYPE",item.TYPE},
                        {"USER",item.USER},
                        
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


      public int doSave(Frm201fPoco param)
      {
          int i = 0;
          try
          {

              using (db)
              {
                  MBRM obj = (from t in db.MBRM where t.TAGID == param.TAGID select t).Single();                                    
                  obj.BIRTH = DateTimeUtil.getDateTime(param.BIRTH);
                  obj.BLODTP = StringUtils.getString(param.BLODTP).Trim();
                  obj.CIDT = DateTimeUtil.getDateTime(param.CIDT);
                  obj.CNCNO = StringUtils.getString(param.CNCNO).Trim();
                  obj.CNHNO = StringUtils.getString(param.CNHNO).Trim();
                  obj.CNM = StringUtils.getString(param.CNM).Trim();
                  obj.CNTRY = StringUtils.getString(param.CNTRY).Trim();
                  obj.CODT = DateTimeUtil.getDateTime(param.CODT);
                  obj.EMAIL = StringUtils.getString(param.EMAIL).Trim();
                  obj.LOGDT = System.DateTime.Now;
                  obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                  obj.MARRI = Convert.ToInt32(param.MARRI);
                  obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                  obj.PID = StringUtils.getString(param.PID).Trim();
                  obj.POSL = StringUtils.getString(param.POSL).Trim();
                  obj.SEX = StringUtils.getString(param.SEX).Trim();
                  obj.TPNM = StringUtils.getString(param.TPNM).Trim();
                  obj.TPNO = StringUtils.getString(param.TPNO).Trim();
                i=  db.SaveChanges();
              }
          }
          catch (Exception ex)
          {

          }
          return i;
      }

    public int doAdd(Frm201fPoco param)
    {
        int i=0;
        MBRM obj = new MBRM(); 
        try{      
            using(db){                  
            obj.BIRTH =DateTimeUtil.getDateTime(param.BIRTH);
            obj.BLODTP = StringUtils.getString(param.BLODTP).Trim();
            obj.CIDT = DateTimeUtil.getDateTime(param.CIDT);
            obj.CNCNO = StringUtils.getString(param.CNCNO).Trim();
            obj.CNHNO = StringUtils.getString(param.CNHNO).Trim();
            obj.CNM = StringUtils.getString(param.CNM).Trim();
            obj.CNTRY = StringUtils.getString(param.CNTRY).Trim();
            obj.CODT=DateTimeUtil.getDateTime(param.CODT);
            obj.EMAIL = StringUtils.getString(param.EMAIL).Trim();
            obj.JOBTAG = StringUtils.getString(param.JOBTAG).Trim();
            obj.LOGDT = System.DateTime.Now;
            obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
            obj.MARRI =Convert.ToInt32(param.MARRI);
            obj.MFEE = 0;
            obj.NOTES = StringUtils.getString(param.NOTES).Trim();
            obj.PID = StringUtils.getString(param.PID).Trim();
            obj.POSL = StringUtils.getString(param.POSL).Trim();
            obj.RMNO = StringUtils.getString(param.RMNO).Trim();
            obj.RSTA = " ";
            obj.SEX = StringUtils.getString(param.SEX).Trim();
            obj.TPNM = StringUtils.getString(param.TPNM).Trim();
            obj.TPNO = StringUtils.getString(param.TPNO).Trim();
            obj.TAGID = cs.getTagidByDatetime();            
                
            db.MBRM.Add(obj);
           i= db.SaveChanges();
           }
        }
        catch (Exception ex)
        {

        }
        return i;
    }

    public int doRemove(String TAGID)
    {
        int i = 0;
        try
        {
            using (db)
            {
                MBRM obj = (from t in db.MBRM where t.TAGID == TAGID select t).Single();
                db.MBRM.Remove(obj);
                i = db.SaveChanges();
            }

        }
        catch (Exception ex)
        {

        }
        return i;
    }


  

}
}