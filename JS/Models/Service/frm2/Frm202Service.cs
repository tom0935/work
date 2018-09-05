
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
public class Frm202Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String DT)
    {
        /*
        W_DT = DTOT(THIS.DATER1.PICKDT)
        W_SQL = "SELECT * FROM DESKBOOK " + ;
        "WHERE RSTA = ' ' AND DT1 <= ?W_DT " + ;
        "ORDER BY DT1,RMNO"
         */
        JArray ja = new JArray();
        JObject jo = new JObject();        
        DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.DESKBOOK where t.RSTA == " " && t.DT1 <= dt select t);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"ENDDT",item.ENDDT},
                        {"LOGDT",item.LOGDT},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"NOTETP",item.NOTETP},
                        {"RMNO",item.RMNO},                        
                        {"TAGID",item.TAGID}
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

    public JObject getDG2(String DT)
    {/*
W_SQL = "SELECT * FROM DESKBOOK " + ;
		"WHERE RSTA = 'Y' AND (DT1 <= ?W_DT AND DT2 >= ?W_DT) " + ;
		"ORDER BY RMNO,DT1"*/
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.DESKBOOK where t.RSTA == "Y" && (t.DT1 <= dt && t.DT2 >= dt) select t);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"ENDDT",item.ENDDT},
                        {"LOGDT",item.LOGDT},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN.Trim()},
                        {"NOTES",item.NOTES},
                        {"NOTETP",item.NOTETP},
                        {"RMNO",item.RMNO},                        
                        {"TAGID",item.TAGID}
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


    public int doSave(Frm202Poco param)
    {
        
        int i = 0;
        try
        {
            DateTime? tmp = null;
         

            using (db)
            {
                DESKBOOK obj = (from t in db.DESKBOOK where t.TAGID==param.TAGID select t).Single();
                obj.DT1 = DateTimeUtil.getDateTime(param.DT1);
                obj.DT1 = DateTimeUtil.getDateTime(param.DT2);
                obj.LOGDT = System.DateTime.Now;
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.NOTETP = StringUtils.getString(param.NOTETP).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RSTA = " ";
                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm202Poco param)
    {

        int i = 0;
        try
        {
            DateTime? tmp = null;


            using (db)
            {
                DESKBOOK obj = new DESKBOOK();
                obj.DT1 = DateTimeUtil.getDateTime(param.DT1);
                obj.DT2 = DateTimeUtil.getDateTime(param.DT2);
                obj.LOGDT = System.DateTime.Now;
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.NOTETP = StringUtils.getString(param.NOTETP).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RSTA = " ";
                obj.TAGID = cs.getTagidByDatetime();
                db.DESKBOOK.Add(obj);
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
            /*
             * UPDATE DESKBOOK SET " + ;
                    "RSTA = 'Y', " + ;
                    "ENDDT = ?W_DTT " + ;
                    "WHERE TAGID = ?W_TAGID"
             */
            i = db.Database.ExecuteSqlCommand("update DESKBOOK set RSTA='Y',ENDDT={0} where TAGID={1}",  System.DateTime.Now ,StringUtils.getString(TAGID).Trim());
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
            i = db.Database.ExecuteSqlCommand("update DESKBOOK set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now,StringUtils.getString(TAGID).Trim());
        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}