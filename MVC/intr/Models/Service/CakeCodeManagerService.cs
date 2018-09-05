
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Dynamic;
using System.Linq;
using IntranetSystem.Models;
using IntranetSystem.Poco;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using Oracle.DataAccess.Client;



namespace IntranetSystem.Service
{
public class CakeCodeManagerService 
{
    private Entities orderDb = new Entities();
    private FDPOSEntities fdDb = new FDPOSEntities();
    private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

    /**
       * 訂單明細
       *
       *  */

    /*
    public JObject getOrderDetailDatagrid(OrderParamPoco param, int count)
    {
        JArray ja = new JArray();
        JArray ja2 = new JArray();
        JObject jo = new JObject();

        var query = from t in orderDb.CAKE_ORDERS_DTL_VIEW
                    where t.STATUS == "N" && t.ODDNO == param.ODDNO
                    select new { UUID = t.UUID, QTY = t.QTY, SQTY = t.SQTY, ODDNO = t.ODDNO, PDNO = t.PDNO, PDNAME = t.POSPNM, t.COST, PRICE = t.PRICE, PRICE_COUNT = t.PRICE_COUNT, REMARK = t.REMARK, REMARK_STR = t.REMARK_STR, CAKE_REMARK = t.CAKE_REMARK };

        query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
        query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁    
        query.AsEnumerable();
        JObject itemObject2 = null;
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"UUID",StringUtils.getString(item.UUID)},                
                        {"ODDNO",StringUtils.getString(item.ODDNO)},
                        {"QTY",StringUtils.getString(item.QTY)},
                        {"SQTY",StringUtils.getString(item.SQTY)},
                        {"PDNO",StringUtils.getString(item.PDNO)},
                        {"PDNAME",StringUtils.getString(item.PDNAME)},
                        {"PRICE",StringUtils.getString(item.PRICE)},
                        {"COST",StringUtils.getString(item.COST)},
                        {"REMARK",StringUtils.getString(item.REMARK)},
                        {"REMARK_STR",StringUtils.getString(item.REMARK_STR)},
                        {"AQTY",getSumByAQTY(StringUtils.getString(item.UUID))},
                        {"CAKE_REMARK",item.CAKE_REMARK},
                        
                    };
            itemObject2 = new JObject
                    {   
                        {"PDNO",""},
                        {"PDNAME","合計:"},
                        {"PRICE",StringUtils.getString(item.PRICE_COUNT)},                        
                        {"REMARK_STR",""},  
                    };

            ja.Add(itemObject);

        }
        ja2.Add(itemObject2);
        jo.Add("rows", ja);
        jo.Add("footer", ja2);
        jo.Add("total", count);
        return jo;
    }


    public int getOrderDetailCount(OrderParamPoco param)
    {
        JArray ja = new JArray();
        int i = 0;
        var query = from t in orderDb.CAKE_ORDERS_DTL_VIEW where t.ODDNO == param.ODDNO && t.STATUS != "C" select t;
        i = query.Count();
        return i;
    }*/

    public DataTable getFixView(int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO, String QTY_TYPE)
    {

        string query =
 "  SELECT  ODDNO,     " +
 "         BDATE,                                              " +
 "         BUSER,                                              " +
 "         COMP,                                               " +
 "         DEPT,                                               " +
 "         STATUS,                                             " +
 "         SDATE,                                              " +
 "         ODATE,                                              " +
 "         OUSER,                                              " +
 "         PDATE,                                              " +
 "         PUSER,                                              " +
 "         FDATE,                                              " +
 "         FUSER,                                              " +
 "         USER_NAME,                                          " +
 "         COMP_NAME,                                          " +
 "         DEPT_NAME,                                          " +
 "         O_NAME,                                             " +
 "         COST,                                               " +
 "         decode(qty_type,'2','*' || PDNO,PDNO) PDNO,         " +
 "         POSNO,                                              " +
 "         POSPNM,                                             " +
 "         PRICE,                                              " +
 "         QTY,                                                " +
 "         REMARK,                                             " +
 "         SQTY,                                               " +
 "         DTL_STATUS,                                         " +
 "         UUID,                                               " +
 "         CAKE_REMARK,                                        " +
 "         AQTY,                                               " +
 "         qty_type                                            " +
 "    FROM (                                                   " +
"SELECT ODDNO,                                                                   " +
"                  BDATE,                                                        " +
"                  BUSER,                                                        " +
"                  COMP,                                                         " +
"                  DEPT,                                                         " +
"                  STATUS,                                                       " +
"                  SDATE,                                                        " +
"                  ODATE,                                                        " +
"                  OUSER,                                                        " +
"                  PDATE,                                                        " +
"                  PUSER,                                                        " +
"                  FDATE,                                                        " +
"                  FUSER,                                                        " +
"                  USER_NAME,                                                    " +
"                  COMP_NAME,                                                    " +
"                  DEPT_NAME,                                                    " +
"                  O_NAME,                                                       " +
"                  COST,                                                         " +
"                  PDNO,                                                         " +
"                  POSNO,                                                        " +
"                  POSPNM,                                                       " +
"                  PRICE,                                                        " +
"                  QTY,                                                          " +
"                  REMARK,                                                       " +
"                  SQTY,                                                         " +
"                  DTL_STATUS,                                                   " +
"                  UUID,                                                         " +
"                  CAKE_REMARK,                                                  " +
"                  AQTY,                                                         " +
"                  '1' qty_type                                                  " +
"             FROM INTRA.CAKE_ORDERS_MD_VIEW                                     " +
"            WHERE (STATUS = 'F') AND (DTL_STATUS = 'N')                         " +
"           UNION  ALL                                                           " +
"           SELECT '' ODDNO,                                                     " +
"                  NULL BDATE,                                                   " +
"                  NULL BUSER,                                                   " +
"                  o.COMP,                                                       " +
"                  o.DEPT,                                                       " +
"                  o.STATUS,                                                     " +
"                  o.CR_DT SDATE,                                                " +
"                  NULL ODATE,                                                   " +
"                  NULL OUSER,                                                   " +
"                  NULL PDATE,                                                   " +
"                  NULL PUSER,                                                   " +
"                  NULL FDATE,                                                   " +
"                  NULL FUSER,                                                   " +
"                  NULL USER_NAME,                                               " +
"                  i.NAME COMP_NAME,                                             " +
"                  d.NAME DEPT_NAME,                                             " +
"                  NULL O_NAME,                                                  " +
"                  f.COST * o.QTY COST,                                          " +
"                  nvl(o.PDNO,'0000000')  PDNO,                                   " +
"                  NULL POSNO,                                                   " +
"                  o.PDNAME,                                                     " +
"                  o.PRICE,                                                      " +
"                  0 QTY,                                                        " +
"                  o.REMARK,                                                     " +
"                  o.QTY SQTY,                                                   " +
"                  NULL DTL_STATUS,                                              " +
"                  ROWNUM UUID,                                                  " +
"                  NULL cake_remark,                                             " +
"                  0 AQTY,                                                       " +
"                  '2' qty_type                                                  " +
"             FROM INTRA.CAKE_ORDERS_DTL_FIX o,                                  " +
"                  intra.i_department_view d,                                    " +
"                  intra.i_company i,                                             " +
"                  hwtpe.fdpost01 f                            "+
"            WHERE o.DEPT = d.code(+) AND o.comp = I.CID(+) AND o.status = 'F'   " +
"          AND f.posno = '99'   " +
//"          AND f.sale = 'Y'     " +
"          AND f.pospno = o.PDNO " + 
"            ) where 1=1                                                         " +
"            and nvl('" + COMP + "','*') in (COMP,'*')            " +
"            and decode('" + QTY_TYPE + "','Y','*','1') in (QTY_TYPE,'*')            " +
"            and                                                                 " +
"              case when '" + SDT + "' is null then                                       " +
"                '1'                                                             " +
"               else                                                             " +
"                to_char(SDATE,'YYYY-MM-DD')                                     " +
"              end                                                               " +
"             between                                                            " +
"              case when '" + SDT + "' is null then                                       " +
"                '1'                                                             " +
"               else                                                             " +
"               '" + SDT + "'                                      " +
"              end                                                               " +
"             and                                                                " +
"              case when '" + SDT + "' is null then                                       " +
"                '1'                                                             " +
"               else                                                             " +
"                '" + EDT + "'                                      " +
"              end                                                               " +
"            and                                                                 " +
"              case when nvl('" + SDEPART + "','*')='*' then                                   " +
"                '1'                                                             " +
"               else                                                             " +
"                substr(DEPT,4,5)                                                        " +
"              end                                                               " +
"             between                                                            " +
"              case when nvl('" + SDEPART + "','*') ='*' then                                   " +
"                '1'                                                             " +
"               else                                                             " +
"                 '" + SDEPART + "'                                                            " +
"              end                                                               " +
"             and                                                                " +
"              case when nvl('" + SDEPART + "','*')='*' then                                   " +
"                '1'                                                             " +
"               else                                                             " +
"                '" + EDEPART + "'                                                         " +
"              end                                                               " +
"            and                                                                 " +
"              case when nvl('" + SPOSNO + "','*')='*' then                                   " +
"                '1'                                                             " +
"               else                                                             " +
"                PDNO                                                            " +
"              end                                                               " +
"             between                                                            " +
"              case when nvl('" + SPOSNO + "','*') ='*' then                     " +
"                '1'                                                             " +
"               else                                                             " +
"                 '" + SPOSNO + "'                                               " +
"              end                                                               " +
"             and                                                                " +
"              case when nvl('" + SPOSNO + "','*')='*' then                      " +
"                '1'                                                             " +
"               else                                                             " +
"                '" + EPOSNO + "'                                                " +
"              end                                                               " +
"  ORDER BY DEPT,QTY_TYPE, PDNO ";
        
        DataTable dt = new DataTable();
        using (OracleConnection conn = new OracleConnection(constr))
        {
            OracleCommand cmd = new OracleCommand(query, conn);
            /*
            cmd.Parameters.Add("QTY_TYPE", OracleDbType.Varchar2).Value =QTY_TYPE;
            cmd.Parameters.Add("SDT", OracleDbType.Varchar2).Value =SDT;
            cmd.Parameters.Add("EDT",OracleDbType.Varchar2).Value =EDT;
            cmd.Parameters.Add("SDEPART", OracleDbType.Varchar2).Value = SDEPART;
            cmd.Parameters.Add("EDEPART", OracleDbType.Varchar2).Value = EDEPART;
            */

            OracleDataAdapter oda = new OracleDataAdapter(cmd);

            
            oda.Fill(dt);
        }
        return dt;
    }


    public DataTable getDataTableByCAKE_ORDERS_VIEW(int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO, String QTY_TYPE)
   {
       LinqExtensions le = new LinqExtensions();
       /* 
       var query = (from t in orderDb.CAKE_ORDERS_MD_VIEW where t.STATUS == "F" && t.DTL_STATUS != "C" && t.COMP == COMP && t.SQTY > 0 select new { ODDNO=t.ODDNO,BDATE=t.BDATE,BUSER=t.BUSER,COMP=t.COMP,DEPT=t.DEPT,STATUS=t.STATUS,SDATE=t.SDATE, ODATE=t.ODATE,OUSER=t.OUSER,PDATE=t.PDATE
           ,PUSER=t.PUSER,FDATE=t.FDATE,FUSER=t.FUSER,USER_NAME=t.USER_NAME,COMP_NAME=t.COMP_NAME,DEPT_NAME=t.DEPT_NAME,O_NAME=t.O_NAME,COST=t.COST,PDNO=t.PDNO,POSPNM=t.POSPNM,PRICE=t.PRICE,QTY=t.QTY,REMARK=t.REMARK,SQTY=t.SQTY,DTL_STATUS=t.DTL_STATUS,
           UUID=t.UUID,CAKE_REMARK=t.CAKE_REMARK,AQTY=t.AQTY
           });
         */
        
       var query = (from t in orderDb.CAKE_ORDERS_MD_VIEW
                    where t.STATUS == "F" && t.DTL_STATUS != "C" && t.COMP == COMP && t.SQTY > 0 select t);
         
      
        /*
       var query = orderDb.ExecuteStoreQuery<OrderFixParamPoco>(" SELECT rownum RNO,ODDNO, BDATE,BUSER, COMP, DEPT, STATUS,SDATE,ODATE,OUSER,PDATE,PUSER,FDATE,FUSER,USER_NAME,COMP_NAME,DEPT_NAME," +
          "O_NAME,COST,PDNO,POSNO,POSPNM,PRICE,QTY,REMARK,SQTY,DTL_STATUS,UUID,CAKE_REMARK,AQTY,qty_type FROM (  SELECT ODDNO, BDATE, BUSER, COMP, DEPT,STATUS,SDATE,ODATE, OUSER,"+
                  "PDATE,PUSER, FDATE,FUSER,USER_NAME,COMP_NAME,DEPT_NAME, O_NAME,COST,PDNO,POSNO,POSPNM,PRICE,QTY,REMARK,SQTY,DTL_STATUS,UUID,CAKE_REMARK,AQTY,'1' qty_type "+
                  "FROM INTRA.CAKE_ORDERS_MD_VIEW WHERE (STATUS = 'F') AND (DTL_STATUS = 'N') UNION ALL SELECT '' ODDNO, NULL BDATE,NULL BUSER, o.COMP, o.DEPT, o.STATUS,o.CR_DT SDATE,"+
                  "NULL ODATE,NULL OUSER,NULL PDATE,NULL PUSER,NULL FDATE,NULL FUSER,NULL USER_NAME,i.NAME COMP_NAME,d.NAME DEPT_NAME,NULL O_NAME,o.COST * o.QTY COST,o.PDNO,NULL POSNO,"+
                  "o.PDNAME,o.PRICE,0 QTY,o.REMARK,o.QTY SQTY,NULL DTL_STATUS,ROWNUM UUID,NULL cake_remark,0 AQTY,'2' qty_type FROM INTRA.CAKE_ORDERS_DTL_FIX o, intra.i_department_view d,"+
                  "intra.i_company i WHERE o.DEPT = d.code AND o.comp = I.CID AND o.status = 'F') where qty_type='2'");
          */

        /*
       var query = (from t in orderDb.CAKE_ORDERS_MD_VIEW
                    where t.STATUS == "F" && t.DTL_STATUS != "C" && t.COMP == COMP && t.SQTY > 0
                    select new { DEPT =t.DEPT,PDNO=t.PDNO,SDATE=t.SDATE});
         */
        
       if (QTY_TYPE == "N")
       {
           query = query.Where(t=>t.QTY_TYPE =="F" );
       }
        
       if (SDT != "")
       {
           DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
           DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
           query = query.Where(q => q.SDATE >= sdt && q.SDATE <= edt);
       }
       if (SDEPART != "")
       {
           query = query.Where(q => string.Compare(q.DEPT.Substring(3,4), SDEPART) >= 0 && string.Compare(q.DEPT.Substring(3,4), EDEPART) <= 0); 
       }
        

     /*
       if (SPOSNO != "" )
       {
           query = query.Where(q => string.Compare(q.PDNO, SPOSNO) >= 0 && string.Compare(q.PDNO, EPOSNO) <= 0);
       }*/

       int x = query.Count();
       DataTable dt = le.LinqQueryToDataTable(query);
        
       return dt;
   }


    public DataTable getDataTableByCAKE_ORDERS_DTL_FIX_VIEW(int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO)
    {
        LinqExtensions le = new LinqExtensions();
        //var query = (from t in orderDb.CAKE_ORDERS_DTL_FIX where t.STATUS == "1" && t.COMP == COMP select t);
        var query = from t in orderDb.CAKE_ORDERS_DTL_FIX_VIEW where t.STATUS == "F" && t.COMP == COMP select t;

        if (SDT != "")
        {
            DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
            DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
            query = query.Where(q => q.CR_DT >= sdt && q.CR_DT <= edt);
        }
        if (SDEPART != "")
        {
            query = query.Where(q => string.Compare(q.DEPT.Substring(3, 4), SDEPART) >= 0 && string.Compare(q.DEPT.Substring(3, 4), EDEPART) <= 0);
        }
        if (SPOSNO != "")
        {
            query = query.Where(q => string.Compare(q.PDNO, SPOSNO) >= 0 && string.Compare(q.PDNO, EPOSNO) <= 0);
        }

        int x = query.Count();
        DataTable dt = le.LinqQueryToDataTable(query);
        return dt;
    }


    /*
    public JObject getAddDetailList(String UUID, String QTY,EasyuiParamPoco param)
    {
        decimal uuid = StringUtils.getDecimal(UUID);
        var query = from t in orderDb.CAKE_ORDERS_DTL_ADD where t.DTL_UUID == uuid  select t;
        query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序  
        JArray ja = new JArray();
        JObject jo = new JObject();
        foreach (var item in query)
        {
            
            String addremark = StringUtils.getString(item.ADD_REMARK);
            var itemObject = new JObject
                    {   
                        {"UUID",StringUtils.getString(item.UUID)},                
                        {"AQTY",StringUtils.getString(item.AQTY)},
                        {"ADD_REMARK",addremark},
                        {"CR_DT",StringUtils.getString(item.CR_DT)},                        
                        {"USERID",StringUtils.getString(item.USERID)}
                    };
            ja.Add(itemObject);
        }
        JArray ja2 = new JArray();
        Decimal total = getSumByAQTY(UUID);
        var itemObject2 = new JObject
                    {   
                        {"UUID",""},
                        {"AQTY",StringUtils.getString(total)},
                        {"ADD_REMARK",""},
                        {"CR_DT","增減數量小計"},  
                    };
        ja2.Add(itemObject2);
        jo.Add("rows", ja);
        jo.Add("footer", ja2);
        return jo;
    }


    public Decimal getSumByAQTY(String UUID)
    {

        Decimal uuid = StringUtils.getDecimal(UUID);
        Decimal AQTY = 0;
        var query = from t in orderDb.CAKE_ORDERS_DTL_ADD where t.DTL_UUID == uuid  group t by t.DTL_UUID into g select g.Sum(s => s.AQTY);

        foreach (var item in query)
        {
            AQTY = item.Value;
        }
        return AQTY;
    }*/


    //追加數量
    public int doAddDetail(FixCakeOrderParamPoco param, String userid)
    {
        DateTime currentTime = new DateTime();
        currentTime = System.DateTime.Now;
        int i = 0;
        using (Entities context = new Entities())
        {
            CAKE_ORDERS_DTL_FIX obj;
            if (StringUtils.getString(param.UUID) == "")
            {
                obj = new CAKE_ORDERS_DTL_FIX();
            }
            else
            {
                var query = (from t in context.CAKE_ORDERS_DTL_FIX where t.STATUS=="F" select t);
                Decimal uuid = StringUtils.getDecimal(param.UUID);
                obj = query.Where(q => q.UUID == uuid).SingleOrDefault();
            }
             
            obj.REMARK = param.ADD_REMARK;
            obj.QTY = param.AQTY;
            obj.CR_DT = DateTimeUtil.getDateTime(param.CR_DT);            
            obj.USERID = userid;
            obj.COMP = param.COMP;
            obj.DEPT = param.DEPT;
            obj.PDNAME = param.POSPNM;
            obj.PDNO = param.POSPNO;
            obj.COST = param.COST;
            obj.PRICE = param.PRICE;
            obj.STATUS = "F";
            if (StringUtils.getString(param.UUID) == "")
            {
                context.CAKE_ORDERS_DTL_FIX.AddObject(obj);
            }            
            i = context.SaveChanges();
        }
        return i;

    }



    public JObject getFixCakeOrderList(EasyuiParamPoco param,String UUID,String SEARCH,String SDT,String EDT)
    {
        JObject jo = new JObject();
        JArray ja = new JArray();
        /*
        var query = from t in orderDb.CAKE_ORDERS_DTL_FIX_VIEW
                    where t.STATUS == "1"
                    join b in orderDb.I_DEPARTMENT_VIEW on t.DEPT equals b.CODE into deptg
                    from dept in deptg.DefaultIfEmpty()
                    select new { UUID = t.UUID, PDNAME = t.PDNAME, QTY = t.QTY, REMARK = t.REMARK, COST = t.COST, PRICE = t.PRICE, DEPT = t.DEPT, PDNO = t.PDNO, USERID = t.USERID, COMP = t.COMP, CR_DT = t.CR_DT ,DEPT_NAME=dept.NAME};
         * */

        var query = from t in orderDb.CAKE_ORDERS_DTL_FIX_VIEW select t;
        if (UUID != null)
        {
            Decimal uuid = StringUtils.getDecimal(UUID);            
            query = query.Where(t => t.UUID == uuid); 
        }
        if (SEARCH != null)
        {
            query = query.Where(t => t.COMP_NAME.Contains(SEARCH) || t.COMP.Contains(SEARCH) || t.DEPT.Substring(3, 4).Contains(SEARCH) || t.DEPT_NAME.Contains(SEARCH) || t.PDNAME.Contains(SEARCH) || t.PDNO.Contains(SEARCH));
        }
        else if (SDT != null && EDT != null)
        {
            DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
            DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
            query = query.Where(q => q.CR_DT >= sdt && q.CR_DT <= edt);
        }
        else
        {  //撈取當月第一天跟最後一天
          DateTime sdt = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
          DateTime edt = new DateTime(DateTime.Now.AddMonths(1).Year,DateTime.Now.AddMonths(1).Month,1).AddDays(-1);
            query = query.Where(q => q.CR_DT >= sdt && q.CR_DT <= edt);
        }
        query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序   
        query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁 
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"UUID",StringUtils.getString(item.UUID)},                
                        {"PDNAME",StringUtils.getString(item.PDNAME)},
                        {"QTY",StringUtils.getString(item.SQTY)},
                        {"REMARK",StringUtils.getString(item.REMARK)},
                        {"COST",StringUtils.getString(item.COST)},
                        {"PRICE",StringUtils.getString(item.PRICE)},
                        {"DEPT",StringUtils.getString(item.DEPT)},
                        {"PDNO",StringUtils.getString(item.PDNO)},
                        {"USERID",StringUtils.getString(item.USERID)},
                        {"COMP",StringUtils.getString(item.COMP)},
                        {"CR_DT",item.CR_DT},
                        {"DEPT_NAME",item.DEPT_NAME},
                    };

            ja.Add(itemObject);

        }

        if (ja.Count() > 0)
        {
            jo.Add("rows", ja);
            jo.Add("total", ja.Count());
        }
        else
        {
            jo.Add("rows", "");
            jo.Add("total", "");
        }
        return jo;
        
    }

    public int doRemove(String UUID)
    {
        int i = 0;
        if (UUID != null)
        {
            Decimal uuid = StringUtils.getDecimal(UUID);
            CAKE_ORDERS_DTL_FIX obj = (from c in orderDb.CAKE_ORDERS_DTL_FIX
                                       where c.UUID == uuid
                                       select c).Single();

            obj.STATUS = "C";
          i= orderDb.SaveChanges();

        }

        return i;
    }    

}
}