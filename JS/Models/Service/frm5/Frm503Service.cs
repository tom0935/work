
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
using System.Text;

namespace Jasper.service.frm5{
public class Frm503Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDG1(Frm503Poco param)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();


        var query = (from t in db.MKRPAY where t.RSTA == " " && t.RPTDT == null && t.FEEYM == param.FEEYM select t);

        if (StringUtils.getString(param.FIXAREA) != "*")
        {
            byte b = Convert.ToByte(param.FIXAREA);
            query = query.Where(t => t.FIXAREA ==b);
        }
        if (StringUtils.getString(param.FIXQUIP) != "*")
        {
            byte b = Convert.ToByte(param.FIXQUIP);
            query = query.Where(t => t.FIXQUIP == b);
        }

        if (StringUtils.getString(param.PAYSTA) != "*")
        {
            byte b = Convert.ToByte(param.PAYSTA);
            query = query.Where(t => t.PAYSTA == b);
        }

        //ONUSE,MKRNM,AMTSUM,VNO,FEEAMT,AMTTX,VDT,NOTES,LOGDT,RPTDT,TAGID
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"MKRNM",item.MKRNM},
                        {"AMTSUM",item.AMTSUM},
                        {"VNO",item.VNO},
                        {"FEEAMT",item.FEEAMT},
                        {"PAYSTA",item.PAYSTA},
                        {"AMTTX",item.AMTTX},
                        {"VDT",String.Format("{0:yyyy-MM-dd}" , item.VDT)},
                        {"NOTES",item.NOTES},
                        {"LOGDT",String.Format("{0:yyyy-MM-dd}" , item.LOGDT)},
                        {"RPTDT",String.Format("{0:yyyy-MM-dd}" , item.RPTDT)},
                        {"TAGID",item.TAGID}
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
            jo.Add("total", query.Count());
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;
    }


    /*
    public DataTable getReport( List<Frm503Poco> list)
    {

        LinqExtensions le = new LinqExtensions();

        var query = (from t in db.MKRPAY where t.RSTA == " " && t.RPTDT == null select t);

        if (list != null)
        {
            List<string> code = new List<string>();
            foreach (Frm503Poco item in list)
            {
                code.Add(item.TAGID.ToString());
            }

            if (code.Count > 0)
            {
                query = query.Where(t => code.Contains(t.TAGID));
            }
        }



        DataTable dt = le.LinqQueryToDataTable(query);

        return dt;
    }
    */

    public DataTable getReport(List<Frm503Poco> list)
    {

        LinqExtensions le = new LinqExtensions();
        /*
        var query = (from t in db.MKRPAY
                     where t.RSTA == " " && t.RPTDT == null
                     join b in db.ACCTP on t.ACCTP equals b.TPNO into bb
                     from bg in bb.DefaultIfEmpty()                     
                     select new {t.ACCNM,t.ACCNO,t.ACCTP,t.AMTSUM,t.AMTTX,t.ENDDT,t.FEEAMT,t.FEEYM,t.FIXAREA,t.FIXQUIP,t.INBUDG,t.KPISUB,t.LOGDT,t.LOGUSR
                     ,t.MKRNM,t.MKRNO,t.NOTES,t.PAYSTA,t.REQUEST_NO,t.RPTDT,t.RSTA,t.TAGID,t.VDT,t.VNO,bg.TPNM
                     });
        */
        var query = (from t in db.view_MKRPAY select t);

        if (list != null)
        {
            List<string> code = new List<string>();
            foreach (Frm503Poco item in list)
            {
                code.Add(item.TAGID.ToString());
            }

            if (code.Count > 0)
            {
                query = query.Where(t => code.Contains(t.TAGID));
            }
        }



        DataTable dt = le.LinqQueryToDataTable(query);

        return dt;
    }





    public int doSave( List<Frm503Poco> list)
    {
       
        int i = 0;
        
       try
       {
                        
           /*
    W_TAGID = T_MKRPAY503.TAGID
	W_SQL = "UPDATE MKRPAY SET " + ;
			"RPTDT = ?W_DTTM " + ;
			"WHERE TAGID = ?W_TAGID"
	SQLEXEC(CONNID,W_SQL,"MKRPAY503")*/
           using (db)
           {
               foreach (Frm503Poco obj in list)
               {
                   MKRPAY mkrpay = (from t in db.MKRPAY where t.TAGID == obj.TAGID.Trim() select t).Single();
                   mkrpay.RPTDT = System.DateTime.Now;
               }
               i= db.SaveChanges();
           }
       }catch (Exception ex){

       }


        return i;
    }



 
   


}
}