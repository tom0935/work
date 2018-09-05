
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
public class Frm204Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String TAGID)
    {
        /*
        W_DT = DTOT(THIS.DATER1.PICKDT)
        W_SQL = "SELECT * FROM DESKBOOK " + ;
        "WHERE RSTA = ' ' AND DT1 <= ?W_DT " + ;
        "ORDER BY DT1,RMNO"
         */
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.COMPLNDO where t.jobtag == TAGID orderby t.dt  select t);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"DT",String.Format("{0:yyyy-MM-dd}",item.dt)},
                        {"NOTES",item.notes}, 
                        {"TAGID",item.tagid}
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

    public JObject getDG2()
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        
        var query = (from t in db.COMPLN where t.ISEND != "Y" orderby t.RMNO,t.LOGDT  select t);

        
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"LOGDT",String.Format("{0:yyyy-MM-dd}",item.LOGDT)},
                        {"ENDDT",String.Format("{0:yyyy-MM-dd hh:mm:ss}",item.LOGDT)},
                        {"COMPLNTP",item.COMPLNTP},
                        {"FUNCDEP",item.FUNCDEP},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"ISEND",item.ISEND},
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


    public JObject getDG3()
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        
        var query = (from t in db.COMPLN where t.ISEND == "Y" orderby t.RMNO, t.LOGDT select t);


        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"LOGDT",String.Format("{0:yyyy-MM-dd}",item.LOGDT)},     
                        {"ENDDT",String.Format("{0:yyyy-MM-dd hh:mm:ss}",item.LOGDT)},
                        {"COMPLNTP",item.COMPLNTP},
                        {"FUNCDEP",item.FUNCDEP},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"ISEND",item.ISEND},
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


    public int doSave(Frm204Poco param)
    {
        
        int i = 0;
        try
        {
            DateTime? tmp = null;

            using (db)
            {
                COMPLN obj = (from t in db.COMPLN where t.TAGID==param.TAGID select t).Single();                
                obj.ISEND = param.ISEND.Trim();
                if (param.ISEND.Trim() == "Y")
                {
                    obj.ENDDT = System.DateTime.Now;
                }
                obj.LOGDT = DateTimeUtil.getDateTime(param.LOGDT);
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.FUNCDEP = StringUtils.getString(param.FUNCDEP).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.COMPLNTP = StringUtils.getString(param.COMPLNTP).Trim();

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm204Poco param)
    {

        int i = 0;
        try
        {
            DateTime? tmp = null;
            /*
W_SQL = "INSERT INTO COMPLN " + ;
		"(RMNO,MAN,COMPLNTP,NOTES,FUNCDEP,LOGUSR,LOGDT,ISEND,TAGID) " + ;
		"VALUES (?M.RMNO,?M.MAN,?M.COMPLNTP,?M.NOTES,?M.FUNCDEP,?M.LOGUSR,?M.LOGDT,'N',?W_TAGID) "
             * */
            using (db)
            {
                COMPLN obj = new COMPLN();
                if (param.ISEND.Trim() == "Y")
                {
                    obj.ENDDT = System.DateTime.Now;
                }
                obj.LOGDT = DateTimeUtil.getDateTime(param.LOGDT);
                obj.COMPLNTP = StringUtils.getString(param.COMPLNTP).Trim();
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.ISEND = StringUtils.getString(param.ISEND).Trim();
                obj.FUNCDEP = StringUtils.getString(param.FUNCDEP).Trim();
                obj.TAGID = cs.getTagidByDatetime();
                db.COMPLN.Add(obj);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public int doAdd2(String DT,String NOTES,String TAGID)
    {

        int i = 0;
        try
        {
            using (db)
            {
                COMPLNDO obj = new COMPLNDO();
                obj.dt = DateTimeUtil.getDateTime(DT);
                obj.notes = NOTES;
                obj.jobtag = StringUtils.getString(TAGID).Trim();
                obj.tagid = cs.getTagidByDatetime();
                db.COMPLNDO.Add(obj);
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
            //i = db.Database.ExecuteSqlCommand("update DESKBOOK set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now,TAGID);
            i = db.Database.ExecuteSqlCommand("delete COMPLNDO where JOBTAG={0}", StringUtils.getString(TAGID).Trim());
            i = db.Database.ExecuteSqlCommand("delete COMPLN where TAGID={0}", StringUtils.getString(TAGID).Trim());
        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}