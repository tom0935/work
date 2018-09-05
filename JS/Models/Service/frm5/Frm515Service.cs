
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
public class Frm515Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public DataTable getReport(Frm515Poco obj)
    {

        LinqExtensions le = new LinqExtensions();


      

        var query = from t in db.view_RPT515 where  t.EYY==obj.FEEYM select t;

        if (StringUtils.getString(obj.OPT) != "*")
        {
            query = query.Where(t => t.CTYPE == obj.OPT);
        }

        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    


  
  








}
}