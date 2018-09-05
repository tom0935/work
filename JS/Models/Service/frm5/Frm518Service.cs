
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
public class Frm518Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm518Poco obj)
    {

        LinqExtensions le = new LinqExtensions();




        var query = from t in db.MKRPAY where t.RSTA == " " &&  !(t.FIXQUIP ==1 && t.PAYSTA==2) &&  (string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0) select t;

        if (StringUtils.getString(obj.MKRNO) != "")
        {
            query = query.Where(t => t.MKRNO == obj.MKRNO);
        }


        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }





    public JArray getExcel(Frm518Poco obj)
    {

        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = from t in db.MKRPAY
                    where t.RSTA == " " && !(t.FIXQUIP == 1 && t.PAYSTA == 2) && (string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0)
                    group t by new { MKRNO = t.MKRNO, MKRNM = t.MKRNM } into g
                    select new { MKRNO = g.Key.MKRNO, MKRNM = g.Key.MKRNM, FEEAMT = g.Sum(t=>t.FEEAMT),CNT=g.Count()};

        if (StringUtils.getString(obj.MKRNO) != "")
        {
            query = query.Where(t => t.MKRNO == obj.MKRNO);
        }

        

        foreach (var item in query)
        {

            var itemObject = new JObject
                    {   
                        {"廠商編號",item.MKRNO},
                        {"廠商名稱",item.MKRNM},                                        
                        {"付款金額",item.FEEAMT},
                        {"付款筆數",item.CNT}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }  
  








}
}