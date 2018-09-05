
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
public class Frm519Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm519Poco obj)
    {

        LinqExtensions le = new LinqExtensions();

        /*
W_SQL = "SELECT A.TELNO,B.RMNO FROM RMTEL A " + ;
		"INNER JOIN CONTRAH B ON A.JOBTAG = B.TAGID " + ;
		"WHERE A.TELTP = '專線' AND A.RSTA = ' ' " + ;
		"ORDER BY B.RMNO,A.TELNO "
     */
        String sql = @"SELECT A.TELNO,B.RMNO FROM RMTEL A 
		             INNER JOIN CONTRAH B ON A.JOBTAG = B.TAGID 
		             WHERE A.TELTP = '專線' AND A.RSTA = ' ' ";

        var query = db.Database.SqlQuery<Frm5191Poco>(sql);


        if (StringUtils.getString(obj.OPT) == "1")
        {
            query = query.Where(t => t.RMNO != "");
        }
        else if (StringUtils.getString(obj.OPT) == "2")
        {
            query = query.Where(t => t.RMNO == "");
        }

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