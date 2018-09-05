
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
public class Frm207Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String RMNO,String FEEYM)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.OTHERFEE where t.RSTA==" "  select t);
        if (StringUtils.getString(RMNO) != "")
        {
            query = query.Where(t => t.RMNO == RMNO );
        }
        if (StringUtils.getString(FEEYM) != "")
        {
            query = query.Where(t => t.FEEYM == FEEYM);
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMTSUM",item.AMTSUM},
                        {"MAN",item.MAN},
                        {"AMTTX",item.AMTTX},
                        {"FEEAMT",item.FEEAMT},
                        {"NOTES",item.NOTES},
                        {"RMNO",item.RMNO},
                        {"RMTAG",item.RMTAG},
                        {"TAGID",item.TAGID},
                        {"FEETP",item.FEETP},
                        {"FEEYM",item.FEEYM},
                        {"OTHERN",item.OTHERN},
                        {"PAYAMT",item.PAYAMT},                        
                        {"RNO",item.RNO},
                        {"RNO2",item.RNO2},
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

  


  


    public int doSave(Frm207Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                int txrat = Convert.ToInt32(cs.getConfig("TXRAT"));
                OTHERFEE obj = (from t in db.OTHERFEE where t.TAGID == param.TAGID select t).Single();                
                obj.LOGDT = System.DateTime.Now;
                obj.AMTSUM =Convert.ToInt32(param.AMTSUM);
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.FEEAMT = Convert.ToInt32(Math.Round((Decimal)obj.AMTSUM * 100 / (100 + txrat), 0));
                obj.AMTTX = obj.AMTSUM - obj.FEEAMT;
                obj.OTHERN = StringUtils.getString(param.OTHERN).Trim();
                obj.FEETP = StringUtils.getString(param.FEETP).Trim();
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RNO = StringUtils.getString(param.RNO).Trim();
                obj.RNO2 = StringUtils.getString(param.RNO2).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm207Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                int txrat = Convert.ToInt32(cs.getConfig("TXRAT"));
                OTHERFEE obj = new OTHERFEE();

                obj.LOGDT = System.DateTime.Now;
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);
                //M.FEEAMT = ROUND(M.AMTSUM * 100 / (100+TXRAT),0)   
                //M.AMTTX = M.AMTSUM - M.FEEAMT
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.FEEAMT = Convert.ToInt32(Math.Round((Decimal)obj.AMTSUM * 100 / (100 + txrat), 0));
                obj.AMTTX = obj.AMTSUM - obj.FEEAMT;
                obj.OTHERN = StringUtils.getString(param.OTHERN).Trim();
                obj.FEETP = StringUtils.getString(param.FEETP).Trim();
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RNO = StringUtils.getString(param.RNO).Trim();
                obj.RNO2 = StringUtils.getString(param.RNO2).Trim();
                obj.RMTAG = StringUtils.getString(param.RMTAG).Trim();
                obj.RSTA = " ";
                obj.TAGID = cs.getTagidByDatetime();
                db.OTHERFEE.Add(obj);
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
            i = db.Database.ExecuteSqlCommand("update OTHERFEE set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now, StringUtils.getString(TAGID).Trim());
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