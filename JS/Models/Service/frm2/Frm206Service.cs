
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
public class Frm206Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1()
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.RMNEWS where t.RSTA==" " select t);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMT",item.AMT},
                        {"MAN",item.MAN},
                        {"NEWSNM",item.NEWSNM},
                        {"NEWSTP",item.NEWSTP},
                        {"NOTES",item.NOTES},
                        {"RMNO",item.RMNO},
                        {"RMTAG",item.RMTAG},
                        {"TAGID",item.TAGID},
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"LOGUSR",item.LOGUSR}                        
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

  


  


    public int doSave(Frm206Poco param)
    {
        
        int i = 0;
        try
        {
            
            DateTime? tmp = null;

            using (db)
            {
                RMNEWS obj = (from t in db.RMNEWS where t.TAGID==param.TAGID select t).Single();                
                obj.DT1 = DateTimeUtil.getDateTime(param.DT1);
                obj.DT2 = DateTimeUtil.getDateTime(param.DT2);
                obj.LOGDT = System.DateTime.Now;
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.AMT = Convert.ToInt16(param.AMT);
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.NEWSNM = StringUtils.getString(param.NEWSNM).Trim();
                obj.NEWSTP = StringUtils.getString(param.NEWSTP).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm206Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                RMNEWS obj = new RMNEWS();

                obj.DT1 = DateTimeUtil.getDateTime(param.DT1);
                obj.DT2 = DateTimeUtil.getDateTime(param.DT2);
                obj.LOGDT = System.DateTime.Now;
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.AMT = Convert.ToInt16(param.AMT);
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.NEWSNM = StringUtils.getString(param.NEWSNM).Trim();
                obj.NEWSTP = StringUtils.getString(param.NEWSTP).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.RSTA = " ";
                obj.TAGID = cs.getTagidByDatetime();
                db.RMNEWS.Add(obj);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }





    public int doSuccess(String TAGID)
    {

        int i = 0;
        try
        {

            i = db.Database.ExecuteSqlCommand("update COMPLN set RSTA='Y',ENDDT={0} where TAGID={1}", System.DateTime.Now, StringUtils.getString(TAGID).Trim());
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
            /*
             * W_SQL = "UPDATE DESKBOOK SET " + ;
		"RSTA = 'C', " + ;
		"ENDDT = ?W_DTT " + ;
		"WHERE TAGID = ?W_TAGID"
             */
            i = db.Database.ExecuteSqlCommand("update RMNEWS set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now,StringUtils.getString(TAGID).Trim());
            //i = db.Database.ExecuteSqlCommand("delete COMPLNDO where JOBTAG={0}", TAGID);
            //i = db.Database.ExecuteSqlCommand("delete COMPLN where TAGID={0}", TAGID);
        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}