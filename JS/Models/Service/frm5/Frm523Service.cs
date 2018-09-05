
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
public class Frm523Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDatagrid()
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.BROKER.AsQueryable();

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},                
                        {"CPID",StringUtils.getString(item.CPID)},
                        {"ADR",StringUtils.getString(item.ADR)},
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"FAX",StringUtils.getString(item.FAX)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)}
                    };
            ja.Add(itemObject);
        }

        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }

        return jo;
    }


    public DataTable getReport(Frm523Poco param)
    {

        LinqExtensions le = new LinqExtensions();

        /*
        List<string> code = new List<string>();
        foreach (Frm523Poco item in list)
        {
            code.Add(item.CNO.ToString());
        }*/

   /*
W_SQL = "SELECT RMNO,DEALERNM,BROKNO,BROKERNM,BROKFEE,RNFEE,DT1,DT2 FROM CONTRAH " + ;
		"WHERE DT1 >= ?W_DT1 AND DT1 <= ?W_DT2 " + ;
		"ORDER BY BROKNO,RMNO,DT1 "
        */

        DateTime dt1 = DateTimeUtil.getDateTime(param.SYM);
        DateTime dt2 = DateTimeUtil.getDateTime(param.EYM);
        var query = from t in db.CONTRAH where (t.DT1 >= dt1 && t.DT2 <= dt2)
                    group t by t.BROKERNM  into g
                    orderby g.Count() descending, g.Sum(s => s.BROKFEE) descending
                    select new { BROKERNM = g.Key, SP = g.Count(), BROKFEE = g.Sum(s => s.BROKFEE) };

        if (StringUtils.getString(param.CNT) != "")
        {
            query = query.Take(StringUtils.getInt(param.CNT));
        }

      /*
        var query = from t in db.view_RPT523 where  t.EYY==obj.FEEYM select t;

        if (StringUtils.getString(obj.OPT) != "*")
        {
            query = query.Where(t => t.CTYPE == obj.OPT);
        }*/

        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    


  
  








}
}