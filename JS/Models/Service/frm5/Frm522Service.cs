
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
public class Frm522Service 
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


    public DataTable getReport(List<Frm522Poco> list,Frm522Poco param)
    {

        LinqExtensions le = new LinqExtensions();


        List<string> code = new List<string>();
        foreach (Frm522Poco item in list)
        {
            code.Add(item.CNO.ToString());
        }

       /*
        W_SQL = "SELECT RMNO,DEALERNM,BROKNO,BROKERNM,BROKFEE,RNFEE,DT1,DT2 FROM CONTRAH " + ;
		"WHERE DT1 >= ?W_DT1 AND DT1 <= ?W_DT2 " + ;
		"ORDER BY BROKNO,RMNO,DT1 "
     */
        DateTime dt1 = DateTimeUtil.getDateTime(param.SYM);
        DateTime dt2 = DateTimeUtil.getDateTime(param.EYM);
        var query = from t in db.CONTRAH where code.Contains(t.BROKNO) && (t.DT1 >=dt1 && t.DT2 <= dt2) select t;
      /*
        var query = from t in db.view_RPT522 where  t.EYY==obj.FEEYM select t;

        if (StringUtils.getString(obj.OPT) != "*")
        {
            query = query.Where(t => t.CTYPE == obj.OPT);
        }*/

        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    


  
  








}
}