
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
public class Frm528Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm528Poco obj)
    {

        LinqExtensions le = new LinqExtensions();
        /*
W_SQL = "SELECT * FROM CLUBFEE " + ;
		"WHERE DT >= ?W_DT1 AND DT <= ?W_DT2 " + ;
		" AND RSTA = ' ' " + ;
		"ORDER BY DT "
        */

        DateTime dt1 = DateTimeUtil.getDateTime(obj.SYM);
        DateTime dt2 = DateTimeUtil.getDateTime(obj.EYM);
        var query = from t in db.CLUBFEE where t.RSTA == " " && t.DT >= dt1 && t.DT <= dt2 select t;

        if (StringUtils.getString(obj.PRINTTYPE) != "")
        {
            if (obj.PRINTTYPE == "1")
            {
                query = query.Where(t => t.PAYAMT == t.AMT);
            }
            else if (obj.PRINTTYPE == "2")
            {
                query = query.Where(t => t.PAYAMT < t.AMT);
            }
        }


        if (StringUtils.getString(obj.SORTYPE) != "")
        {
            if(obj.SORTYPE=="1"){
                query = query.OrderBy(t => t.DT).OrderBy(t=>t.RMNO);
            }else{
                query = query.OrderBy(t => t.ITM).OrderBy(t=>t.RMNO);
            }
        }

     //   var query = db.Database.SqlQuery<Frm5282Poco>(sql);

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