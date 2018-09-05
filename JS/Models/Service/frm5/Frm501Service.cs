﻿
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

namespace Jasper.service.frm5{
public class Frm501Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm501Poco obj, List<Frm5012Poco> list)
    {

        LinqExtensions le = new LinqExtensions();


        /*
        var query = (from t in db.OTHERFEE where t.RSTA == " " select new { FEEYM = t.FEEYM, RMNO = t.RMNO, FEETP = t.FEETP, AMTTX = t.AMTTX, AMTSUM = t.AMTSUM, OTHERN = t.OTHERN, RMTAG = t.RMTAG}).Union
                    (from t in db.CLUBFEE where t.RSTA == " " select new { FEEYM = t.FEEYM, RMNO = t.RMNO, FEETP = "C", AMTTX = 0, AMTSUM = t.AMT, OTHERN = t.ITM, RMTAG = t.RMTAG });
        */

        var query = (from t in db.view_RPT501 select t);

        //var query = db.Database.SqlQuery<f3031>(sql,);

        /*
	THISFORM.RMNO.VALUE = CNRM501.RMNO
	THISFORM.RMNO.TEXTWS = LTRIM(STR(VAL(SUBSTR(CNRM501.RMNO,1,2)),2))+" F - "+"1"+SUBSTR(CNRM501.RMNO,3,2)
	THISFORM.TAGID.VALUE = CNRM501.TAGID
	THISFORM.DEALER.VALUE = CNRM501.DEALERNM
	THISFORM.MLIVER.VALUE = CNRM501.MLIVER
        */
        //多筆

        
        if (list!=null)
        {
            if (list.Count() > 0)
            {
                List<string> code = new List<string>();
                foreach (Frm5012Poco item in list)
                {
                    code.Add(item.CNO.ToString());
                }

                if (code.Count > 0)
                {
                    query = query.Where(t => code.Contains(t.RMNO) && t.FEEYM == obj.YM);
                }
            }
        }
        else
        {
            query = query.Where(t => t.RMNO==obj.RMNO && t.FEEYM == obj.YM);
            
        }


        if (obj.SORT == "1")
        {
            query = query.OrderBy(t => t.RMNO); 
        }
        else
        {
            
           // query = query.OrderBy(t => new {p1=t.RMNO.Substring(3, 2),p2=t.RMNO.Substring(1, 2)});
            
              query = query.OrderBy(c => c.RMNO.Substring(2, 2)).ThenBy(t => t.RMNO.Substring(0, 1));
            //query = query.OrderBy(t => t.RMNO.Substring(1, 2));
        }

     //   var str = (query as System.Data.Objects.ObjectQuery).ToTraceString();


        DataTable dt = le.LinqQueryToDataTable(query);

        return dt;
    }


    public Hashtable getCONTRAH(String CNO)
    {
        JArray ja = new JArray();
        var query = from t in db.CONTRAH where t.RMNO == CNO && t.CNSTA == " " select t;
        Hashtable h = new Hashtable();
        foreach (var item in query)
        {
            
            h.Add("DEALERNM", StringUtils.getString(item.DEALERNM));
            h.Add("MLIVER", StringUtils.getString(item.MLIVER));            
        }

        return h;
    }



    public int doSave(Frm501Poco param)
    {

        int i = 0;
        try
        {
            using (db)
            {
                RPTCONF obj = (from t in db.RPTCONF where t.KIND == "501" select t).Single();
                obj.EITEM1 = param.ETX1;
                obj.EITEM2 = param.ETX2;
                obj.EITEM3 = param.ETX3;
                obj.EITEM4 = param.ETX4;
                obj.EITEM5 = param.ETX5;
                obj.CITEM1 = param.CTX1;
                obj.CITEM2 = param.CTX2;
                obj.CITEM3 = param.CTX3;
                obj.CITEM4 = param.CTX4;
                obj.CITEM5 = param.CTX5;
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }
  






    public JArray getCONF()
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        var query = from t in db.RPTCONF where t.KIND == "501" select t;
        if (query.Count() > 0)
        {
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"EITEM1",item.EITEM1},
                        {"EITEM2",item.EITEM2},
                        {"EITEM3",item.EITEM3},
                        {"EITEM4",item.EITEM4},
                        {"EITEM5",item.EITEM5},
                        {"CITEM1",item.CITEM1},
                        {"CITEM2",item.CITEM2},
                        {"CITEM3",item.CITEM3},
                        {"CITEM4",item.CITEM4},
                        {"CITEM5",item.CITEM5},
                    };
                ja.Add(itemObject);
            }
        }
        return ja;
    }




}
}