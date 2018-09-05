
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
public class Frm201gService 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String TAGID)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime? tmp = null;
        /*
         W_SQL = "SELECT EQUIPNO,CNM,MAKR,SPEC,QTY,TAGID FROM V_CNEQUIP " + ;
		"WHERE JOBTAG = ?W_JOBTAG AND RSTA = ' ' " + ;
		"ORDER BY EQUIPNO"*/

        var query = from t in db.V_CNEQUIP where t.JOBTAG == TAGID && t.RSTA == " " orderby t.EQUIPNO select new {EQUIPNO=t.EQUIPNO,CNM=t.CNM,MAKR=t.MAKR,SPEC=t.SPEC,QTY=t.QTY,TAGID=t.TAGID };
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"EQUIPNO",item.EQUIPNO},
                        {"CNM",item.CNM},
                        {"MAKR",item.MAKR},
                        {"SPEC",item.SPEC},
                        {"QTY",item.QTY},
                        {"TAGID",item.TAGID}
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

    public JObject getDG2()
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime? tmp = null;
        var query = from t in db.EQUIP select new { CNO = t.CNO, CNM = t.CNM, MAKR = t.MAKR, SPEC = t.SPEC };
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",item.CNO},
                        {"CNM",item.CNM},
                        {"MAKR",item.MAKR},
                        {"SPEC",item.SPEC}
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

    public int doAdd(Frm201gPoco param)
    {
        int i=0;
        try{
            /*
              W_SQL = "INSERT INTO CNEQUIP " + ;
              "(RMNO,EQUIPNO,QTY,TAGID,JOBTAG,CNHNO) " + ;
              "VALUES (?M.RMNO,?M.EQUIPNO,?M.QTY,?M.TAGID,?W_JOBTAG,?W_CNHNO)"
             */
            using(db){
            CNEQUIP obj = new CNEQUIP();
            obj.RMNO = StringUtils.getString(param.RMNO).Trim();
            obj.EQUIPNO = StringUtils.getString(param.EQUIPNO).Trim();
            obj.QTY =Convert.ToInt16(param.QTY);
            obj.TAGID = cs.getTagidByDatetime();
            obj.JOBTAG = StringUtils.getString(param.JOBTAG).Trim();
            obj.RSTA = " ";
            db.CNEQUIP.Add(obj);
            db.SaveChanges();
           }
        }
        catch (Exception ex)
        {

        }
        return i;
    }

    public int doRemove(String JOBTAG,String TAGID)
    {
        int i = 0;
        try
        {

           i = db.Database.ExecuteSqlCommand(@"UPDATE CNEQUIP SET 
                                               RSTA = 'C'                                               
                                               WHERE JOBTAG = {0} AND TAGID = {1}",
                                               JOBTAG,TAGID);            
            

        }
        catch (Exception ex)
        {

        }
        return i;
    }


  

}
}