
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
public class Frm504Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm504Poco obj)
    {

        LinqExtensions le = new LinqExtensions();
        /*
W_SQL = "SELECT RMNO,CNHNO,DT1,DT2,DEALERNM,ENDDT,CNSTA,TAGID FROM CONTRAH " + ;
		"WHERE ENDDT IS NULL OR ENDDT >= ?W_YMMIN " + ;
		"ORDER BY RMNO,ENDDT "
        */
        var query =from t in db.BONUS select t;
        //var query = db.Database.SqlQuery<f3031>(sql,);


        

      //  query = query.Where(t => t.RMNO == obj.RMNO && (string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0));



        DataTable dt = le.LinqQueryToDataTable(query);

        return dt;
    }





    public int doSave(Frm504Poco param)
    {

        int i = 0;
        try
        {
            using (db)
            {
                RPTCONF obj = (from t in db.RPTCONF where t.KIND == "504" select t).Single();
                obj.EITEM1 = param.SENDDT;
                obj.EITEM2 = param.ACP1;
                obj.EITEM3 = param.ACP2;
                obj.EITEM4 = param.SENDER;
                obj.EITEM5 = param.TXNO;
                obj.EITEM6 = param.ATTN;
                obj.CITEM1 = param.UPRC;
                obj.CITEM2 = param.PINS;                
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

        var query = from t in db.RPTCONF where t.KIND == "504" select t;
        if (query.Count() > 0)
        {
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {                           
                        {"SENDDT",item.EITEM1},
                        {"ACP1",item.EITEM2},
                        {"ACP2",item.EITEM3},
                        {"SENDER",item.EITEM4},
                        {"TXNO",item.EITEM5},
                        {"ATTN",item.EITEM6},
                        {"UPRC",item.CITEM1},
                        {"PINS",item.CITEM2}
                    };
                ja.Add(itemObject);
            }
        }
        return ja;
    }




}
}