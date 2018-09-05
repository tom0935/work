
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
public class Frm520Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(List<Frm520Poco> list)
    {

        LinqExtensions le = new LinqExtensions();
/*
	W_SQL = "SELECT A.CNHNO,A.DT1,A.DT2,A.ENDYM,A.ENDDT,B.APPES,B.CNTRY FROM CONTRAH A " + ;
			"INNER JOIN RMLIV B ON A.TAGID = B.JOBTAG " + ;
			"WHERE ((A.ENDYM IS NULL AND (A.DT1<= ?S_YM1 AND A.DT2 >= ?S_YM2 )) " + ;
			"OR LEFT(A.ENDYM,4) = ?S_YR) " + ;
			"AND B.CNTRY <> ' ' " + ;
			"AND B.APPES LIKE '本人%' " + ;
			"ORDER BY A.DT1 "
        */

        List<string> code = new List<string>();
        foreach (Frm520Poco item in list)
        {
            code.Add(item.CNO.ToString());
        }
        /*
W_SQL = "SELECT A.RMNO,A.EQUIPNO,B.CNM FROM RMEQUIP A " + ;
		"INNER JOIN EQUIP B ON A.EQUIPNO = B.CNO " + ;
		"ORDER BY A.EQUIPNO,A.RMNO "
        */

        

        String sql = @"SELECT A.RMNO,A.EQUIPNO,B.CNM FROM RMEQUIP A 
		               INNER JOIN EQUIP B ON A.EQUIPNO = B.CNO ";

        var query = db.Database.SqlQuery<Frm5201Poco>(sql);

        query = query.Where(t => code.Contains(t.EQUIPNO));

        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    


  
  








}
}