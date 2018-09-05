
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
public class Frm512Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm512Poco obj)
    {

        LinqExtensions le = new LinqExtensions();

        //DateTime dt = Convert.ToDateTime(obj.SYM + "01");

        var query = from t in db.view_RPT512 where (string.Compare(t.YM,obj.SYM) >=0 && string.Compare(t.YM,obj.EYM) <=0) select t;

        if (StringUtils.getString(obj.OPT) != "*")
        {
            query = query.Where(t => t.CTYPE == obj.OPT);
        }

        /*
        String[] acctp = {"09","10"};
        String sarea="";
        String earea="";
        if(obj.OPT=="*"){
            sarea = "1";
            earea = "3";
        }else{
            sarea = obj.OPT;
            earea = obj.OPT;
        }

        var query = from t in db.YMBUDGET
                    where acctp.Contains(t.ACCTP) && (string.Compare(t.YM, obj.SYM) >= 0 && string.Compare(t.YM, obj.EYM) <= 0)
                    join b in db.MKRPAY on t.YM equals b.FEEYM into bb
                    from b in bb.DefaultIfEmpty()
                    where (string.Compare(b.FEEYM, obj.SYM) >= 0 && string.Compare(b.FEEYM, obj.EYM) <= 0) && b.RSTA == " " &&
                    (string.Compare(b.FIXAREA.ToString(), sarea) >= 0 && string.Compare(b.FIXAREA.ToString(), earea) <= 0)
                    group t by t.YM into g
                    select new { YM=g.Key,
                                 
                    };
              */      


     //   var query = db.Database.SqlQuery<Frm5122Poco>(sql);

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