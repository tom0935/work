
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
public class Frm506Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm506Poco obj)
    {

        LinqExtensions le = new LinqExtensions();




        var query = from t in db.view_RPT506 where string.Compare(t.FEEYM, obj.SYM) >= 0 && string.Compare(t.FEEYM, obj.EYM) <= 0 select t;



        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }





   
  








}
}