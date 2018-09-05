
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
public class Frm525Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm525Poco obj)
    {

        LinqExtensions le = new LinqExtensions();
        /*
        W_SQL = "SELECT RMNO,CNHNO,DEALERNM,DEALNO,MLIVER,TAGID FROM CONTRAH " + ;
		"WHERE CNSTA = ' ' " + ;
		"ORDER BY RMNO"
         * 
         * 
         * 
W_SQL = "SELECT * FROM OTHERFEE WHERE RMTAG = ?W_RMTAG " + ;
		"AND RSTA = ' ' AND AMTSUM > PAYAMT " + ;
		"AND FEEYM >= ?W_FEEYM1 AND FEEYM <= ?W_FEEYM2 " + ;
		"AND FEETP > '20' AND FEETP < '31' "         
        */

        var query = from t in db.view_RPT525 where string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0 select t;

        
        //var queryTmp = from t in db.view_RPT525 where string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0 select t;


    /*
        var query = (from t in db.CONTRAH
                     join b in db.view_RPT525 on t.RMNO equals b.RMNO into bg
                     from b1 in bg.DefaultIfEmpty()
                     where string.Compare(b1.FEEYM, obj.SYM) >= 0 && string.Compare(b1.FEEYM, obj.EYM) <= 0
                     select new { b1.RID, t.RMNO, t.DEALERNM, b1.FEEYM, b1.AMTSUM, b1.RMTXT, b1.RPT31, b1.RPT32, b1.RPT33, b1.RPT34, b1.RPT35, b1.RPT36, b1.RPT37, b1.RPT38, b1.RPT39, b1.RPT40, b1.RPT41, b1.RPT42, b1.RPT45, b1.RPTSUM }).Distinct();
      */  

        /*
        var query = (from t in queryTmp
                     join b in db.view_RPT525 on new { t.RMNO, obj.FEEYM } equals new { b.RMNO, b.FEEYM } into bg
                     from b1 in bg.DefaultIfEmpty()
                     where t.CNSTA == " " && t.RMNO != ""
                     select new { b1.TAGID, b1.CPID, t.RMNO, t.DEALERNM, b1.FEEYM, b1.AMTSUM, b1.RMTXT, b1.RPT31, b1.RPT32, b1.RPT33, b1.RPT34, b1.RPT35, b1.RPT36, b1.RPT37, b1.RPT38, b1.RPT39, b1.RPT40, b1.RPT41, b1.RPT42, b1.RPT45, b1.RPTSUM }).Distinct();
        */


        /*
        var query = (from t in db.CONTRAH
                     join b in db.view_RPT524 on new { t.RMNO, obj.FEEYM } equals new { b.RMNO, b.FEEYM } into bg
                     from b1 in bg.DefaultIfEmpty()
                     where t.CNSTA == " " && t.RMNO != ""
                     select new { b1.TAGID, b1.CPID, t.RMNO, t.DEALERNM, b1.FEEYM, b1.AMTSUM, b1.RMTXT, b1.RPT31, b1.RPT32, b1.RPT33, b1.RPT34, b1.RPT35, b1.RPT36, b1.RPT37, b1.RPT38, b1.RPT39, b1.RPT40, b1.RPT41, b1.RPT42, b1.RPT45, b1.RPTSUM }).Distinct();
        */


     //   var query = db.Database.SqlQuery<Frm5252Poco>(sql);

    //    String x = "1234567890";
    //    System.Diagnostics.Debug.Print(x.Substring(0,4)+x.Substring(5,2));



        /*
        foreach (var item in query)
        {
            System.Diagnostics.Debug.Print("DT2:"+item.DT2);
          
        }*/
        


        //var query =from t in db.ROOMF select t;
        //var query = db.Database.SqlQuery<f3031>(sql,);
        
      //  query = query.Where(t => t.RMNO == obj.RMNO && (string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0));
        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    


  
  








}
}