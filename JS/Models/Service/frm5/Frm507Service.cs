
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
public class Frm507Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm507Poco obj)
    {

        LinqExtensions le = new LinqExtensions();

        /*
IF THISFORM.OPG1.VALUE = 6  && 全部
	W_SQL = "SELECT A.*,B.DEALERNM,B.DT1,B.DT2 FROM RMFEEM A " + ;
			"LEFT JOIN CONTRAH B ON A.RMTAG = B.TAGID " + ;
			"WHERE A.FEETP IN ('01','02','03','04','05') AND (A.FEEYM >= ?S_YM1 AND A.FEEYM <= ?S_YM2) " + ;
				"AND (A.ENDYM IS NULL OR A.ENDYM >= A.FEEYM) " + ;
			"ORDER BY A.FEEYM,A.RMNO "
ELSE
	W_SQL = "SELECT A.*,B.DEALERNM,B.DT1,B.DT2 FROM RMFEEM A " + ;
			"LEFT JOIN CONTRAH B ON A.RMTAG = B.TAGID " + ;
			"WHERE A.FEETP = ?W_FEETP AND (A.FEEYM >= ?S_YM1 AND A.FEEYM <= ?S_YM2) " + ;
				"AND (A.ENDYM IS NULL OR A.ENDYM >= A.FEEYM) " + ;
			"ORDER BY A.FEEYM,A.RMNO "
ENDIF   */
        //&& (t.ENDYM == "" || string.Compare(t.ENDYM, t.FEEYM) >= 0)
        int yy = StringUtils.getInt(obj.YY);
        /*
      var query = from t in db.RMFEEM
                  where t.FEEYM.Substring(0, 4) == obj.YY 
                  join b in db.CONTRAH on t.RMTAG equals b.TAGID into bb
                  from b in bb.DefaultIfEmpty()
                  where b.DT1.Year == yy || b.DT2.Year == yy
                  select new { AMTSUM = t.AMTSUM, AMTTX = t.AMTTX, CNDAYS = t.CNDAYS, CNNO = t.CNNO, DSDAYS= t.DSDAYS,ENDDT=t.ENDDT,ENDYM= t.ENDYM,FEEAMT=t.FEEAMT,FEEDT1=t.FEEDT1,FEEDT2=t.FEEDT2,FEETP=t.FEETP,
                  FEEYM=t.FEEYM,PAYAMT=t.PAYAMT,RMNO=t.RMNO,RMTAG=t.RMTAG,TAGID=t.TAGID,YMDAYS=t.YMDAYS,b.DEALERNM,b.DT1,b.DT2
                  };
        */

        
      var query = from t in db.CONTRAH
                   join b in db.RMFEEM on t.TAGID equals b.RMTAG
                   where b.FEEYM.Substring(0, 4) == obj.YY
                  //where (t.DT1.Year == yy || t.DT2.Year == yy) && b.FEEYM.Substring(0, 4) == obj.YY 
                   select new
                   {
                       AMTSUM = b.AMTSUM,
                       AMTTX = b.AMTTX,
                       CNDAYS = b.CNDAYS,
                       CNNO = b.CNNO,
                       DSDAYS = b.DSDAYS,
                       ENDDT = b.ENDDT,
                       ENDYM = b.ENDYM,
                       FEEAMT = b.FEEAMT,
                       FEEDT1 = b.FEEDT1,
                       FEEDT2 = b.FEEDT2,
                       FEETP = b.FEETP,
                       FEEYM = b.FEEYM,
                       PAYAMT = b.PAYAMT,
                       RMNO = b.RMNO,
                       RMTAG = b.RMTAG,
                       TAGID = b.TAGID,
                       YMDAYS = b.YMDAYS,
                       DEALERNM = t.DEALERNM,
                       DT1 = t.DT1,
                       DT2 = t.DT2
                   };

        
         if(StringUtils.getString(obj.OPT)=="*"){
             String[] x={"01","02","03","04","05"};
             query = query.Where(t=> x.Contains(t.FEETP));
         }
         else
         {
             query = query.Where(t => obj.OPT==t.FEETP);
         }



        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }








    public JArray getExcel(Frm507Poco obj)
    {

        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = from t in db.RMFEEM
                    where t.FEEYM.Substring(0, 4) == obj.YY
                    join b in db.CONTRAH on t.RMTAG equals b.TAGID into bb
                    from b in bb.DefaultIfEmpty()
                    select new
                    {
                        AMTSUM = t.AMTSUM,
                        AMTTX = t.AMTTX,
                        CNDAYS = t.CNDAYS,
                        CNNO = t.CNNO,
                        DSDAYS = t.DSDAYS,
                        ENDDT = t.ENDDT,
                        ENDYM = t.ENDYM,
                        FEEAMT = t.FEEAMT,
                        FEEDT1 = t.FEEDT1,
                        FEEDT2 = t.FEEDT2,
                        FEETP = t.FEETP,
                        FEEYM = t.FEEYM,
                        PAYAMT = t.PAYAMT,
                        RMNO = t.RMNO,
                        RMTAG = t.RMTAG,
                        TAGID = t.TAGID,
                        YMDAYS = t.YMDAYS,
                        b.DEALERNM,
                        b.DT1,
                        b.DT2
                    };

        if (StringUtils.getString(obj.OPT) == "*")
        {
            String[] x = { "01", "02", "03", "04", "05" };
            query = query.Where(t => x.Contains(t.FEETP));
        }
        else
        {
            query = query.Where(t => obj.OPT == t.FEETP);
        }



        foreach (var item in query)
        {
            
            var itemObject = new JObject
                    {   
                        {"房屋編號",item.RMNO},
                        {"廠商名稱",item.FEEYM},
                        {"付款金額",item.FEEAMT}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }  
  






}
}