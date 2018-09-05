
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
public class Frm521Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport()
    {

        LinqExtensions le = new LinqExtensions();

        /*
W_SQL = "SELECT A.CNO,A.LOCT,A.SZ,A.USEON,A.ONUSE,B.CNPNO,B.DT1,B.DT2,B.JOBTAG, " + ;
		"C.CARNO,C.CARTP,C.DRIVER,D.RMNO FROM CARLOCT A " + ;
		"LEFT JOIN CONTRAP B ON A.CNO = B.PARKNO AND B.CNSTA = ' ' " + ;
		"LEFT JOIN RMCAR C ON B.JOBTAG = C.JOBTAG AND C.RSTA = ' ' " + ;
		"LEFT JOIN CONTRAH D ON B.JOBTAG = D.TAGID AND D.CNSTA = ' ' " + ;
		"WHERE A.USEON LIKE '住宅%' " + ;
		"ORDER BY A.CNO"        
        */
        String sql = @" SELECT A.CNO,A.LOCT,A.SZ,A.USEON,B.CNPNO,CONVERT(VARCHAR(12),B.DT1,111) as DT1,
                     CONVERT(VARCHAR(12),B.DT2,111) as DT2,B.JOBTAG, 
		             case when C.CARTP='1' then C.CARNO else '' end CARNO1,
                     case when C.CARTP='2' then C.CARNO else '' end CARNO2,
                     CAST(C.CARTP as varchar) CARTP,C.DRIVER,D.RMNO FROM CARLOCT A 
		             LEFT JOIN CONTRAP B ON A.CNO = B.PARKNO AND B.CNSTA = ' '
		             LEFT JOIN RMCAR C ON B.JOBTAG = C.JOBTAG AND C.RSTA = ' ' 
		             LEFT JOIN CONTRAH D ON B.JOBTAG = D.TAGID AND D.CNSTA = ' ' 
		             WHERE A.USEON LIKE '住宅%' 
		             ORDER BY A.CNO";

        var query = db.Database.SqlQuery<Frm521Poco>(sql);

        //query = query.Where(t => code.Contains(t.EQUIPNO));

        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    


  
  








}
}