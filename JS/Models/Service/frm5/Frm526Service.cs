
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
public class Frm526Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm526Poco obj)
    {

        LinqExtensions le = new LinqExtensions();
        /*
  W_SQL = "SELECT CNM,SEX,BIRTH,EMAIL,CIDT,TPNM,CNTRY,BLODTP,LOGUSR,LOGDT, " + ;
		"CODT,PID,POSL,MARRI,RSTA,JOBTAG,TAGID,CNHNO,CNCNO,TPNO,MFEE,NOTES,RMNO FROM MBRM " + ;
		"WHERE RSTA = ' ' " + ;
		"AND CIDT >= ?W_DT1 AND CIDT <= ?W_DT2 " + ;
		"ORDER BY CIDT"
        */

        DateTime dt1 = DateTimeUtil.getDateTime(obj.SYM);
        DateTime dt2 = DateTimeUtil.getDateTime(obj.EYM);
        var query = from t in db.MBRM where t.RSTA==" " && t.CIDT >= dt1 && t.CIDT <= dt2 select t;

        if (StringUtils.getString(obj.SORTYPE) != "")
        {
            if(obj.SORTYPE=="1"){
              query=query.OrderBy(t=>t.CIDT);
            }else{
              query = query.OrderBy(t => t.RMNO);
            }
        }

     //   var query = db.Database.SqlQuery<Frm5262Poco>(sql);

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