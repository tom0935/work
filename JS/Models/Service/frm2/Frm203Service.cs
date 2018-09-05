
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

namespace Jasper.service.frm2{
public class Frm203Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String DT)
    {

        JArray ja = new JArray();
        JObject jo = new JObject();        

        DateTime sdt = DateTimeUtil.getDateTime(DT + " 00:00:00");
        DateTime edt = DateTimeUtil.getDateTime(DT + " 23:59:59");
        
        var query = (from t in db.DESKCASH where t.RSTA == " " && t.LOGDT >= sdt && t.LOGDT<=edt select t);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMT",item.AMT},
                        {"AMTTP",item.AMTTP},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"CHKNO",item.CHKNO},
                        {"RMNO",item.RMNO},
                        {"INOUT",item.INOUT},
                        {"INTP",item.INTP},
                        {"PAYTP",item.PAYTP},                        
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


    public JObject getDG3(EasyuiParamPoco param, String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
    {


        JArray ja = new JArray();
        JObject jo = new JObject();
        JArray ja2 = new JArray();
        var query = (from t in db.DESKCASH where t.RSTA == " " select t);
        if (StringUtils.getString(FIXAREA) == "" && StringUtils.getString(END) == "" && StringUtils.getString(FIXTP) == "" && StringUtils.getString(FIXITEM) == "" && StringUtils.getString(RMNO) == "" && StringUtils.getString(SDT) == "")
        {
            query = query.Where(t => t.RSTA == "xxx");
        }
        else
        {

            if (StringUtils.getString(FIXAREA) != "")
            {
                int area = 1;
                area = Convert.ToInt16(FIXAREA);
                query = query.Where(t => t.AMTTP  == area);
            }


            if (StringUtils.getString(END) != "")
            {  
                if (END == "1")
                {
                    query = query.Where(t => t.INOUT == 1);
                }
                else if (END == "2")
                {
                    query = query.Where(t => t.INOUT == 2);
                }                
                
            }
            if (StringUtils.getString(FIXTP) != "")
            {
                query = query.Where(t => t.INTP == FIXTP);
            }

            if (StringUtils.getString(FIXITEM) != "")
            {
                query = query.Where(t => t.CHKNO == FIXITEM);
            }


            if (StringUtils.getString(RMNO) != "")
            {
                query = query.Where(t => t.RMNO == RMNO);
            }
            if (StringUtils.getString(SDT) != "")
            {
                DateTime sdt = Convert.ToDateTime(SDT + " 00:00:00");
                DateTime edt = Convert.ToDateTime(EDT + " 23:59:59");
                query = query.Where(t => t.LOGDT >= sdt && t.LOGDT <= edt);
            }
        }
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            if (param.sort != null)
            {
                query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            }
            if (param.rows > 0)
            {
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
            }
        }
        else
        {
          //  jo.Add("total", "");
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMT",item.AMT},
                        {"AMTTP",item.AMTTP},
                        {"LOGUSR",item.LOGUSR},
                        {"MAN",item.MAN},
                        {"NOTES",item.NOTES},
                        {"CHKNO",item.CHKNO},
                        {"RMNO",item.RMNO},
                        {"INOUT",item.INOUT},
                        {"INTP",item.INTP},
                        {"PAYTP",item.PAYTP},                        
                        {"TAGID",item.TAGID},
                        {"LOGDT",item.LOGDT}
                    };
            ja.Add(itemObject);
        }

        /*
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
        }
        else
        {
            jo.Add("rows", "");
        }
        return jo;
        */

     //   var totQuery = from t in query group t by t.RMNO into g select new { FEEAMT = g.Sum(t => t.FEEAMT), AMTTX = g.Sum(t => t.AMTTX), AMTSUM = g.Sum(t => t.AMTSUM) };
        //var totQuery = from t in query select new { AMT = t.Sum(t => t.AMT), AMTTX = g.Sum(t => t.AMTTX), AMTSUM = g.Sum(t => t.AMTSUM) };
        var totQuery = from t in query select t;
        int tot = 0;

        foreach (var item in totQuery)
        {
            tot = tot + item.AMT;
        }

        JObject itemObject2 = new JObject
                    {   
                        {"AMT",tot},
                        {"AMTTP",""},
                        {"LOGUSR",""},
                        {"MAN",""},
                        {"NOTES",""},
                        {"CHKNO","合計:"},
                        {"RMNO",""},
                        {"INOUT",""},
                        {"INTP",""},
                        {"PAYTP",""},                        
                        {"TAGID",""},
                        {"LOGDT",""}
                    };
        ja2.Add(itemObject2);


        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
           // jo.Add("total", query.Count());
            jo.Add("footer", ja2);
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;


    }



    public JObject getFormSum(String DT)
    {

        JArray ja = new JArray();
        JObject jo = new JObject();            

        DateTime sdt = DateTimeUtil.getDateTime(DT + " 00:00:00");
        DateTime edt = DateTimeUtil.getDateTime(DT + " 23:59:59");

        var query = (from t in db.DESKCASH where t.RSTA == " " && t.LOGDT >= sdt && t.LOGDT <= edt && t.INOUT == 1 group t by t.INOUT into g select new { INOUT = g.Max(t => t.INOUT), TOT_AMT = g.Sum(t => t.AMT), CASH_AMT = g.Sum(t => t.AMTTP == 1 ? t.AMT : 0), CHK_AMT = g.Sum(t => t.AMTTP == 2 ? t.AMT : 0), CHK_CNT = g.Sum(t => t.AMTTP == 2 ? 1 : 0) }).Union
                    (from t in db.DESKCASH where t.RSTA == " " && t.LOGDT >= sdt && t.LOGDT <= edt && t.INOUT == 2 group t by t.INOUT into g select new { INOUT = g.Max(t => t.INOUT), TOT_AMT = g.Sum(t => t.AMT), CASH_AMT = g.Sum(t => t.AMTTP == 1 ? t.AMT : 0), CHK_AMT = g.Sum(t => t.AMTTP == 2 ? t.AMT : 0), CHK_CNT = g.Sum(t => t.AMTTP == 2 ? 1 : 0) });

        int sum1 = 0;
        int sum2 = 0;
        int totSum = 0;
        foreach (var item in query)
        {
            if (item.INOUT == 1)
            {
                jo.Add("TOT_AMT1", item.TOT_AMT);
                jo.Add("CASH_AMT1", item.CASH_AMT);
                jo.Add("CHK_AMT1", item.CHK_AMT);
                jo.Add("CHK_CNT1", item.CHK_CNT);
                sum1 = item.TOT_AMT;
            }
            else
            {
                jo.Add("TOT_AMT2", item.TOT_AMT);
                jo.Add("CASH_AMT2", item.CASH_AMT);
                jo.Add("CHK_AMT2", item.CHK_AMT);
                jo.Add("CHK_CNT2", item.CHK_CNT);
                sum2 = item.TOT_AMT;
            }            
        }
        //totSum = (3000 + sum1) - sum2;
        totSum =  sum1 - sum2;
        jo.Add("LFAMT", totSum);
        ja.Add(jo);
        return jo;
    }




    public int doAdd(Frm203Poco param)
    {

        int i = 0;
        try
        {
            using (db)
            {
                DESKCASH obj = new DESKCASH();
                obj.LOGDT = System.DateTime.Now;
                obj.MAN = StringUtils.getString(param.MAN).Trim();
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.AMT = Convert.ToInt32(param.AMT);
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RSTA = " ";
                obj.PAYTP =Convert.ToInt16(param.PAYTP);
                obj.INTP = StringUtils.getString(param.INTP).Trim();
                obj.INOUT =Convert.ToInt16(param.INOUT);                
                obj.CHKNO = StringUtils.getString(param.CHKNO).Trim();
                obj.AMTTP = Convert.ToInt16(param.AMTTP);
                obj.TAGID = cs.getTagidByDatetime();
                db.DESKCASH.Add(obj);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }





    public int doRemove(String TAGID)
    {

        int i = 0;
        try
        {
            i = db.Database.ExecuteSqlCommand("update DESKCASH set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now,StringUtils.getString(TAGID).Trim());
        }
        catch (Exception ex)
        {

        }

        return i;
    }

    public DataTable getReportDetailAll(String DT)
    {
        LinqExtensions le = new LinqExtensions();
        DateTime sdt = DateTimeUtil.getDateTime(DT + " 00:00:00");
        DateTime edt = DateTimeUtil.getDateTime(DT + " 23:59:59");
        var query = from t in db.DESKCASH where t.RSTA == " " && t.LOGDT >= sdt && t.LOGDT <= edt orderby t.RMNO select new { INOUT = (t.INOUT == 1 ? "收入" : "支出"), AMTTP = (t.AMTTP == 1 ? "現金" : "支票"),
        AMT=t.AMT,CHKNO=t.CHKNO,RMNO=t.RMNO,MAN=t.MAN,INTP=t.INTP,LOGUSR=t.LOGUSR  };
        int x = query.Count();
        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }

    public DataTable getReportSumAll(String DT)
    {
        LinqExtensions le = new LinqExtensions();
        JArray ja = new JArray();
        JObject jo = new JObject();

        DateTime sdt = DateTimeUtil.getDateTime(DT + " 00:00:00");
        DateTime edt = DateTimeUtil.getDateTime(DT + " 23:59:59");

        var query = (from t in db.DESKCASH where t.RSTA == " " && t.LOGDT >= sdt && t.LOGDT <= edt && t.INOUT == 1 group t by t.INOUT into g select new { INOUT = "收入", TOT_AMT = g.Sum(t => t.AMT), CASH_AMT = g.Sum(t => t.AMTTP == 1 ? t.AMT : 0), CHK_AMT = g.Sum(t => t.AMTTP == 2 ? t.AMT : 0), CHK_CNT = g.Sum(t => t.AMTTP == 2 ? 1 : 0) }).Union
                    (from t in db.DESKCASH where t.RSTA == " " && t.LOGDT >= sdt && t.LOGDT <= edt && t.INOUT == 2 group t by t.INOUT into g select new { INOUT = "支出", TOT_AMT = g.Sum(t => t.AMT), CASH_AMT = g.Sum(t => t.AMTTP == 1 ? t.AMT : 0), CHK_AMT = g.Sum(t => t.AMTTP == 2 ? t.AMT : 0), CHK_CNT = g.Sum(t => t.AMTTP == 2 ? 1 : 0) });
        

        int x = query.Count();
        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }

    public DataTable getReportType(String SDT, String EDT)
    {
        LinqExtensions le = new LinqExtensions();
        JArray ja = new JArray();
        JObject jo = new JObject();

        DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
        DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
        /*
        Decimal[] dArray = new Decimal[] { 1, 2 };
        && dArray.Contains(t.AMTTP)
        */

        var query = from t in db.DESKCASH where t.RSTA == " "  && t.LOGDT >= sdt && t.LOGDT <= edt && t.INOUT != 0 group t by new { t.INTP, t.INOUT, t.AMTTP } into g select new { INTP = g.Key.INTP, INOUT = g.Key.INOUT, AMT = g.Sum(t => t.AMT), AMTTP = g.Key.AMTTP };

        int x = query.Count();
        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    public JArray getReport(String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
    {

        LinqExtensions le = new LinqExtensions();
        var query = (from t in db.DESKCASH select t);
        if (StringUtils.getString(FIXAREA) == "" && StringUtils.getString(END) == "" && StringUtils.getString(FIXTP) == "" && StringUtils.getString(FIXITEM) == "" && StringUtils.getString(RMNO) == "" && StringUtils.getString(SDT) == "")
        {
            query = query.Where(t => t.RSTA == "xxx");
        }
        else
        {
            if (StringUtils.getString(FIXAREA) != "")
            {
                int area = 1;
                area = Convert.ToInt16(FIXAREA);
                query = query.Where(t => t.AMTTP == area);
            }


            if (StringUtils.getString(END) != "")
            {
                if (END == "1")
                {
                    query = query.Where(t => t.INOUT == 1);
                }
                else if (END == "2")
                {
                    query = query.Where(t => t.INOUT == 2);
                }

            }
            if (StringUtils.getString(FIXTP) != "")
            {
                query = query.Where(t => t.INTP == FIXTP);
            }

            if (StringUtils.getString(FIXITEM) != "")
            {
                query = query.Where(t => t.CHKNO == FIXITEM);
            }


            if (StringUtils.getString(RMNO) != "")
            {
                query = query.Where(t => t.RMNO == RMNO);
            }
            if (StringUtils.getString(SDT) != "")
            {
                DateTime sdt = Convert.ToDateTime(SDT);
                DateTime edt = Convert.ToDateTime(EDT);
                query = query.Where(t => t.LOGDT >= sdt && t.LOGDT <= edt);
            }

        }

        JArray jObjects = new JArray();
        foreach (var item in query)
        {
            String area = "";
            if (item.INOUT == 1)
            {
                area = "收入";
            }
            else if (item.INOUT == 2)
            {
                area = "支出";
            }
            var jo = new JObject();
            jo.Add("收支方式", area.Trim());
            jo.Add("收支日期", String.Format("{0:yyyy-MM-dd}", item.LOGDT));
            jo.Add("房號", item.RMNO.Trim());
            jo.Add("費用類別", item.INTP.Trim());
            jo.Add("支票號碼", item.CHKNO.Trim());
            jo.Add("費用", item.AMT.ToString());
            jo.Add("備註說明", StringUtils.FilterGdName(item.NOTES.Trim()));
            
            jObjects.Add(jo);
            // break;
        }
        return jObjects;


    }


}
}