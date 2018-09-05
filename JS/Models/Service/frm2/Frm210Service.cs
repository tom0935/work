
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
public class Frm210Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String TAGID)
    {
       
        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = (from t in db.OTHERFEE where t.RMTAG == TAGID && t.AMTSUM != t.PAYAMT && t.RSTA == " " select new { CNO = "2", TAGID = t.TAGID, FEETP = t.FEETP, FEEYM = t.FEEYM, FEENM = t.OTHERN, FEEAMT = t.AMTSUM, PAYAMT = t.PAYAMT, LOGUSR = t.LOGUSR }).Union
                  (from t in db.RMFEEM where t.RMTAG == TAGID && t.AMTSUM != t.PAYAMT && t.RSTA == " "
                   join b in db.CONFCODE on t.FEETP equals b.CODE_NO into b1
                   from bb in b1 where bb.CODE_KIND == "WCOD"
                   select new { CNO = "1", TAGID = t.TAGID, FEETP = t.FEETP, FEEYM = t.FEEYM, FEENM = bb.CODE_NAME, FEEAMT = t.AMTSUM, PAYAMT = t.PAYAMT, LOGUSR = "" }).Union
                  (from t in db.CLUBFEE where t.RMTAG == TAGID && t.AMT != t.PAYAMT && t.RSTA == " " select new { CNO = "3",TAGID=t.TAGID,FEETP="91", FEEYM = t.FEEYM, FEENM =t.ITM,  FEEAMT = t.AMT, PAYAMT = (int)t.PAYAMT,LOGUSR=t.LOGUSR });




        foreach (var item in query)
        {
            var itemObject = new JObject
                    {     
                        {"CNO",item.CNO}, 
                        {"TAGID",item.TAGID}, 
                        {"FEETP",item.FEETP}, 
                        {"FEEYM",item.FEEYM}, 
                        {"FEENM",item.FEENM}, 
                        {"FEEAMT",item.FEEAMT}, 
                        {"PAYAMT",item.PAYAMT},
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

    public JObject getDG2(String RMTAG)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        /*
W_SQL = "SELECT FEEYM,OTHERN,PAYAMT,LOGDT,RMNO,MAN,FEETP,FEEAMT,NOTES,LOGUSR, " + ;
		"TAGID,FEEFROM,FEETAG,RMTAG FROM PAYFEE " + ;
		"WHERE RMTAG = ?W_RMTAG AND RSTA = ' ' "
        */

        var query = (from t in db.PAYFEE where t.RMTAG == RMTAG && t.RSTA == " " orderby t.FEEYM ,t.LOGDT select t);

        
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"FEEYM",item.FEEYM},
                        {"FEETP",item.FEETP},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"FEENM",item.OTHERN},
                        {"RMNO",item.RMNO}, 
                        {"PAYAMT",item.PAYAMT}, 
                        {"FEEAMT",item.FEEAMT},                         
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


    public JObject getDG3(String TAGID)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = (from t in db.OTHERFEE where t.RMTAG == TAGID && t.AMTSUM != t.PAYAMT && t.RSTA == " " select new { CNO = "2", TAGID = t.TAGID, FEETP = t.FEETP, FEEYM = t.FEEYM, FEENM = t.OTHERN, FEEAMT = t.AMTSUM, PAYAMT = t.AMTSUM - t.PAYAMT, LOGUSR = t.LOGUSR }).Union
                  (from t in db.RMFEEM
                   where t.RMTAG == TAGID && t.AMTSUM != t.PAYAMT && t.RSTA == " "
                   join b in db.CONFCODE on t.FEETP equals b.CODE_NO into b1
                   from bb in b1
                   where bb.CODE_KIND == "WCOD"
                   select new { CNO = "1", TAGID = t.TAGID, FEETP = t.FEETP, FEEYM = t.FEEYM, FEENM = bb.CODE_NAME, FEEAMT = t.AMTSUM, PAYAMT = t.AMTSUM - t.PAYAMT, LOGUSR = "" }).Union
                  (from t in db.CLUBFEE where t.RMTAG == TAGID && t.AMT != t.PAYAMT && t.RSTA == " " select new { CNO = "3", TAGID = t.TAGID, FEETP = "91", FEEYM = t.FEEYM, FEENM = t.ITM, FEEAMT = t.AMT, PAYAMT = t.AMT - (int)t.PAYAMT, LOGUSR = t.LOGUSR });




        foreach (var item in query)
        {
            var itemObject = new JObject
                    {     
                        {"CNO",item.CNO}, 
                        {"TAGID",item.TAGID}, 
                        {"FEETP",item.FEETP}, 
                        {"FEEYM",item.FEEYM}, 
                        {"FEENM",item.FEENM}, 
                        {"FEEAMT",item.FEEAMT}, 
                        {"PAYAMT",item.PAYAMT},
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


    public int doSave(Frm210Poco param)
    {
        
        int i = 0;
        try
        {
            using (db)
            {
                PAYFEE obj = (from t in db.PAYFEE where t.TAGID == param.TAGID select t).SingleOrDefault();
                obj.FEEAMT = Convert.ToInt32(param.FEEAMT);
                obj.FEETP = StringUtils.getString(param.FEETP).Trim();
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.OTHERN = StringUtils.getString(param.OTHERN).Trim();
                obj.FEETAG = StringUtils.getString(param.FEETAG).Trim();
                obj.FEEFROM = StringUtils.getString(param.CNO).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.PAYAMT = Convert.ToInt32(param.PAYAMT);                                
                
                i = db.SaveChanges();

                doSaveSum(StringUtils.getString(param.RMTAG).Trim(), StringUtils.getString(param.FEETAG).Trim(), StringUtils.getString(param.CNO).Trim());

            }

        }catch (Exception ex){

        }

        return i;
    }


    public int doSave2(Frm210Poco param,List<Frm210aPoco> LIST)
    {

        int i = 0;
        try
        {
            using (db)
            {

                foreach (Frm210aPoco o in LIST)
                {
                    PAYFEE obj = new PAYFEE();
                    obj.FEEAMT = Convert.ToInt32(param.FEEAMT);
                    obj.FEETP = StringUtils.getString(o.FEETP).Trim();
                    obj.FEEYM = StringUtils.getString(o.FEEYM).Trim();
                    obj.LOGDT = System.DateTime.Now;
                    obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                    obj.MAN = StringUtils.getString(param.MAN).Trim();
                    obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                    obj.OTHERN = StringUtils.getString(o.FEENM).Trim();
                    obj.FEETAG = StringUtils.getString(o.TAGID).Trim();
                    obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                    obj.PAYAMT = Convert.ToInt32(param.PAYAMT);
                    obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                    obj.RSTA = " ";
                    obj.FEEFROM = StringUtils.getString(o.CNO).Trim();
                    obj.TAGID = cs.getTagidByDatetime();
                    db.PAYFEE.Add(obj);
                    i = db.SaveChanges();
                    doSaveSum(StringUtils.getString(param.RMTAG).Trim(), StringUtils.getString(o.TAGID).Trim(), StringUtils.getString(o.CNO).Trim());
                }
                
            }

        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public int doAdd(Frm210Poco param)
    {

        int i = 0;
        try
        {
        /*
        "INSERT INTO PAYFEE " + ;
		"(RMNO,MAN,OTHERN,FEETP,NOTES,FEEYM,FEEAMT,PAYAMT,LOGUSR,LOGDT,RMTAG,TAGID,FEEFROM,FEETAG) " + ;
		"VALUES (?M.RMNO,?M.MAN,?M.OTHERN,?M.FEETP,?M.NOTES,?M.FEEYM,?M.FEEAMT,?M.PAYAMT,?M.LOGUSR,?M.LOGDT,?M.RMTAG,?W_TAGID,?M.FEEFROM,?M.FEETAG) "
        */

            using (db)
            {
                PAYFEE obj = new PAYFEE();
                obj.FEEAMT = Convert.ToInt32(param.FEEAMT);
                obj.FEETP = StringUtils.getString(param.FEETP).Trim();
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();                
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.OTHERN = StringUtils.getString(param.OTHERN).Trim();
                obj.FEETAG = StringUtils.getString(param.FEETAG).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.PAYAMT =Convert.ToInt32(param.PAYAMT);
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.RSTA = " ";
                obj.FEEFROM = StringUtils.getString(param.CNO).Trim();
                obj.TAGID = cs.getTagidByDatetime();                
                db.PAYFEE.Add(obj);
                i = db.SaveChanges();

                doSaveSum(param.RMTAG,param.FEETAG,param.CNO);

            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public void doSaveSum(String RMTAG,String FEETAG,String CNO)
    {
        var query = (from t in db.PAYFEE
                     where t.RMTAG == RMTAG && t.RSTA == " " && t.FEETAG == FEETAG
                     group t by t.FEETAG into g
                     select new { PAYSUM = g.Sum(t => t.PAYAMT) }).SingleOrDefault();

        if (CNO == "1")
        {
            db.Database.ExecuteSqlCommand("update RMFEEM set PAYAMT={0} where TAGID={1}", query.PAYSUM,StringUtils.getString(FEETAG).Trim());
        }
        else if (CNO == "2")
        {
            db.Database.ExecuteSqlCommand("update OTHERFEE set PAYAMT={0} where TAGID={1}", query.PAYSUM,StringUtils.getString(FEETAG).Trim());
        }
        else if (CNO == "3")
        {
            db.Database.ExecuteSqlCommand("update CLUBFEE set PAYAMT={0} where TAGID={1}", query.PAYSUM,StringUtils.getString(FEETAG).Trim());
        }
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
                obj.notes = StringUtils.getString(NOTES).Trim();
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
            using (db)
            {
                PAYFEE obj = (from t in db.PAYFEE where t.TAGID == TAGID select t).SingleOrDefault();
                obj.ENDDT = System.DateTime.Now;
                obj.RSTA = "C";
                i = db.SaveChanges();                
            }           
        }
        catch (Exception ex)
        {

        }

        return i;
    }



    public JObject getHouseGrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc", String q = "")
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        /*
        _SQL = "SELECT RMNO,MLIVER,ENDDT,TAGID FROM CONTRAH " + ;
        "WHERE CNSTA = 'C' 
        */
        var query = from t in db.CONTRAH where t.CNSTA=="C" select t;
        if (q != "")
        {
            query = query.Where(t => t.RMNO.Contains(q) || t.MLIVER.Contains(q));
        }
        query = query.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序    

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"RMNO",StringUtils.getString(item.RMNO)},
                        {"MLIVER",StringUtils.getString(item.MLIVER)},
                        {"ENDDT",String.Format("{0:yyyy-MM-dd}", item.ENDDT)},
                        {"TAGID",StringUtils.getString(item.TAGID)}
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
   


}
}