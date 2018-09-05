
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
using System.Data.Objects;




namespace IntranetSystem.Service
{
public class CakeOrdersService 
{
    private Entities orderDb = new Entities();
    private FDPOSEntities fdDb = new FDPOSEntities();
   private CommonService commonService = new CommonService();
 
    /*
 public enum Status : byte

{   B = "預定", O = "申請", P = "出單", F = "結案", C = "作廢"
 }*/

    public JObject getDatagrid(CakeOrderParamPoco param) 
    {

        JObject jo = new JObject();
        //取得 Entity 所有的 Property 名稱
       // var entityPropertyNames = EntityHelper.EntityPropertyNames<FDPOST01>();
        /*
        if (!entityPropertyNames.Contains(param.sort))
        {
            param.sort = "POSPNO";
        }
        */
        param.sort = "POSPNO";
        if (!(param.order.Equals("asc", StringComparison.OrdinalIgnoreCase)
              || param.order.Equals("desc", StringComparison.OrdinalIgnoreCase)))
        {
            param.order = "asc";
        }


        JArray ja = new JArray();

        var query = (from t in fdDb.FDPOST01 where t.POSNO == "99" && t.SALE == "Y" select t);
        
        if (StringUtils.getString(param.MODE) == "Q")
        {            
            if (param.TYPE1 != null && param.TYPE1 != "0")
            {
                query = query.Where(q => q.POSPNO.Substring(0, 1) == param.TYPE1);
            }
            if (param.TYPE2 != null && param.TYPE2 != "0")
            {
                query = query.Where(q => q.POSPNO.Substring(1, 1) == param.TYPE2);
            }
            if (param.TYPE3 != null && param.TYPE3 != "0")
            {
                query = query.Where(q => q.POSPNO.Substring(2, 1) == param.TYPE3);
            }
            if (param.TYPE4 != null && param.TYPE4 != "0")
            {
                query = query.Where(q => q.POSPNO.Substring(3, 1) == param.TYPE4);
            }
            if (StringUtils.getString(param.SCODE) != "")
            {
                query = query.Where(q => q.POSPNO.Contains(param.SCODE));
            }
            if (StringUtils.getString(param.SNAME) != "")
            {
                query = query.Where(q => q.POSPNM.Contains(param.SNAME) || q.POSPNO.Contains(param.SNAME));
            }
        }
        else
        {
            if (StringUtils.getString(param.MODE)=="P")  //主廚推薦
            {
                //select posno,pospno,pospnm,sale,package,to_char(cost) cost from fdpost01 where posno='99' and sale='Y' and package='P' order by posno
                query = query.Where(q => q.PACKAGE=="P");
            }
            else if (StringUtils.getString(param.MODE) == "1")  //訂購排行
            {

            }
         
        }
        if (query.Count() > 0)
        {
            jo.Add("total",query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"POSPNO",StringUtils.getString(item.POSPNO)},
                        {"POSPNM",StringUtils.getString(item.POSPNM)},
                        {"COST",StringUtils.getString(item.COST)},
                        {"PRICE",StringUtils.getString(item.PRICE)}
                        
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


    /**
     * 訂單
     *
     *  */
    public JArray getOrderDatagrid(OrderParamPoco param,String STATUS)
    {
        JArray ja = new JArray();


        var query = (from t in orderDb.CAKE_ORDERS_VIEW where t.DEPT == param.DEPART
                     select new { ODDNO = t.ODDNO,
                                  BUSER = t.BUSER,
                                  OUSER = t.OUSER,
                                  ODATE = t.ODATE,
                                  SDATE =t.SDATE,
                                  STATUS = t.STATUS,
                                  USER_NAME =t.USER_NAME,
                                  CUSER =t.CUSER,
                                  CDATE = t.CDATE,
                                  REMARK = t.REMARK
                                });
        if (StringUtils.getString(param.ODDNO) != "")
        {
            query = query.Where(q => q.ODDNO.Contains(param.ODDNO));
        }
        else
        {
            if (StringUtils.getString(param.MODE) == "2")
            {
                if (StringUtils.getString(param.SDT) != "" && StringUtils.getString(param.EDT) != "")
                {
                    DateTime sdt = DateTimeUtil.getDateTime(param.SDT + " 00:00:00");
                    DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");
                    query = query.Where(q => q.ODATE >= sdt && q.ODATE <= edt);
                }
            }
            else
            {
                DateTime currentTimeSdt = new DateTime();
                DateTime currentTimeEdt = new DateTime();
                currentTimeSdt = System.DateTime.Now.AddDays(-7);
                currentTimeEdt = System.DateTime.Now;
                query = query.Where(q => q.ODATE >= currentTimeSdt && q.ODATE <= currentTimeEdt);
            }
        }



        query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
        query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁    

        
        foreach (var item in query)
        {
            String status = StringUtils.getString(item.STATUS);
            String status_str = "";
            switch (status)
            {
                case "B":
                    status_str = "B " + "預定";
                    break;
                case "O":
                    status_str = "O " + "申請";
                    break;
                case "P":
                    status_str = "P " + "出單";
                    break;
                case "F":
                    status_str = "F " + "結案";
                    break;
                case "C":
                    status_str = "C " + "作廢";
                    break;
            }
            var itemObject = new JObject
                    {                                           
                        {"ODDNO",StringUtils.getString(item.ODDNO)},
                        {"BUSER",StringUtils.getString(item.BUSER + " " + item.USER_NAME) },
                        {"ODATE",String.Format("{0:yyyy-MM-dd HH:mm}",item.ODATE)},
                        {"SDATE",String.Format("{0:yyyy-MM-dd HH:mm}", item.SDATE)},
                        {"STATUS",StringUtils.getString(item.STATUS)},
                        {"STATUS_STR",status_str},
                        {"BUSER2",StringUtils.getString(item.BUSER)},
                        {"USER",param.USERID},
                        {"CUSER",StringUtils.getString(item.CUSER)},
                        {"CDATE",String.Format("{0:yyyy-MM-dd HH:mm}", item.CDATE)},
                        {"REMARK",StringUtils.getString(item.REMARK)},
                    };
            ja.Add(itemObject);
        }
      

       return ja;
    }


    public JObject getProductSumDatagrid(CakeOrderParamPoco param, String aid,String userid)
    {
        JObject jo = new JObject();

        //取得 Entity 所有的 Property 名稱
     //   var entityPropertyNames = EntityHelper.EntityPropertyNames<FDPOST01>();
        /*
        if (!entityPropertyNames.Contains(param.sort))
        {
            param.sort = "QTY";
        }*/
        param.sort = "QTY";

        if (!(param.order.Equals("asc", StringComparison.OrdinalIgnoreCase)
              || param.order.Equals("desc", StringComparison.OrdinalIgnoreCase)))
        {
            param.order = "desc";
        }


        JArray ja = new JArray();
        

            DateTime sdt = DateTimeUtil.getDateTime(param.SDT + " 00:00:00");
            DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");
            //query = query.Where(q => q.ODATE >= sdt && q.ODATE <= edt);
           
            if (param.RDO == "*")
            {
              var query = from t in orderDb.CAKE_ORDERS_MD_VIEW
                            where t.STATUS != "C" && t.DTL_STATUS == "N" && (t.ODATE >= sdt && t.ODATE <= edt)
                            group t by new { PDNO = t.PDNO, POSPNM = t.POSPNM } into g
                            orderby g.Sum(p => p.QTY) descending
                            select new { PDNO = g.Key.PDNO, POSPNM = g.Key.POSPNM, QTY = g.Sum(p => p.QTY) };
              if (query.Count() > 0)
              {
                  jo.Add("total", query.Count());
                  query = query.OrderBy(string.Format("{0} {1}", "QTY", "desc")); //排序        
                  query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
              }
              foreach (var item in query)
              {
                  var itemObject = new JObject
                    {                                           
                        {"POSPNO",StringUtils.getString(item.PDNO )},
                        {"POSPNM",StringUtils.getString(item.POSPNM)},
                        {"COST",StringUtils.getString(item.QTY)}
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
            }
            else if (param.RDO == "d")
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                String DEPART = HashtableUtil.getValue(htUser, "DEPART_CODE");
                var  query = from t in orderDb.CAKE_ORDERS_MD_VIEW
                        where t.STATUS != "C" && t.DTL_STATUS == "N" && (t.ODATE >= sdt && t.ODATE <= edt) && t.DEPT==DEPART
                        group t by new { PDNO = t.PDNO, POSPNM = t.POSPNM } into g
                        orderby g.Sum(p => p.QTY) descending
                        select new { PDNO = g.Key.PDNO, POSPNM = g.Key.POSPNM, QTY = g.Sum(p => p.QTY) };
                if (query.Count() > 0)
                {
                    jo.Add("total", query.Count());
                    query = query.OrderBy(string.Format("{0} {1}", "QTY", "desc")); //排序        
                    query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
                }
                foreach (var item in query)
                {
                    var itemObject = new JObject
                    {                                           
                        {"POSPNO",StringUtils.getString(item.PDNO )},
                        {"POSPNM",StringUtils.getString(item.POSPNM)},
                        {"COST",StringUtils.getString(item.QTY)}
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
            }
            else if (param.RDO == "p")
            {
             var query= from t in orderDb.CAKE_ORDERS_MD_VIEW                   
                        where t.STATUS != "C" && t.DTL_STATUS == "N" && (t.ODATE >= sdt && t.ODATE <= edt) && t.OUSER ==userid
                        group t by new { PDNO = t.PDNO, POSPNM = t.POSPNM } into g
                        orderby g.Sum(p => p.QTY) descending
                        select new { PDNO = g.Key.PDNO, POSPNM = g.Key.POSPNM, QTY = g.Sum(p => p.QTY) };

             if (query.Count() > 0)
             {
                 jo.Add("total", query.Count());
                 query = query.OrderBy(string.Format("{0} {1}", "QTY", "desc")); //排序        
                 query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
             }
             foreach (var item in query)
             {
                 var itemObject = new JObject
                    {                                           
                        {"POSPNO",StringUtils.getString(item.PDNO )},
                        {"POSPNM",StringUtils.getString(item.POSPNM)},
                        {"COST",StringUtils.getString(item.QTY)}
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
            }

            




        return jo;
    }

    public int getProductSumCount(CakeOrderParamPoco param)
    {
        JArray ja = new JArray();
        int i = 0;
        DateTime sdt = DateTimeUtil.getDateTime(param.SDT + " 00:00:00");
        DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");
        //query = query.Where(q => q.ODATE >= sdt && q.ODATE <= edt);


        var query = from t in orderDb.CAKE_ORDERS_MD_VIEW
                    where t.STATUS != "C" && t.DTL_STATUS == "N" && (t.ODATE >= sdt && t.ODATE <= edt)
                    group t by new { ODDNO = t.ODDNO, POSPNM = t.POSPNM } into g
                    select new { ODDNO = g.Key.ODDNO, POSPNM = g.Key.POSPNM, QTY = g.Sum(p => p.QTY) };
        i = query.Count();
        return i;
    }


    public int getOrderCount(OrderParamPoco param)
    {
        JArray ja = new JArray();
        int i = 0;
        var query = (from t in orderDb.CAKE_ORDERS_VIEW where t.DEPT == param.DEPART select t);
        if (StringUtils.getString(param.ODDNO) != "")
        {
            query = query.Where(q => q.ODDNO.Contains(param.ODDNO));
        }
        else
        {
            if (StringUtils.getString(param.MODE) == "2")
            {
                if (StringUtils.getString(param.SDT) != "" && StringUtils.getString(param.EDT) != "")
                {
                    DateTime sdt = DateTimeUtil.getDateTime(param.SDT + " 00:00:00");
                    DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");
                    query = query.Where(q => q.ODATE >= sdt && q.ODATE <= edt);
                }
            }
            else
            {
                DateTime currentTimeSdt = new DateTime();
                DateTime currentTimeEdt = new DateTime();
                currentTimeSdt = System.DateTime.Now.AddDays(-7);
                currentTimeEdt = System.DateTime.Now;
                query = query.Where(q => q.ODATE >= currentTimeSdt && q.ODATE <= currentTimeEdt);
            }
        }
        i = query.Count();
        return i;
    }

   /**
    * 訂單明細
    *
    *  */
    public JObject getOrderDetailDatagrid(OrderParamPoco param,int count)
    {
        JArray ja = new JArray();
        JArray ja2 = new JArray();
        JObject jo = new JObject();

        var query = from t in orderDb.CAKE_ORDERS_DTL_VIEW
                     where t.STATUS == "N" && t.ODDNO == param.ODDNO
                    select new {UUID = t.UUID, QTY = t.QTY,SQTY=t.SQTY, ODDNO = t.ODDNO, PDNO = t.PDNO, PDNAME = t.POSPNM, t.COST, PRICE = t.PRICE, PRICE_COUNT = t.PRICE_COUNT, REMARK = t.REMARK, REMARK_STR = t.REMARK_STR,CAKE_REMARK=t.CAKE_REMARK };

        query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
        query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁    
        query.AsEnumerable();
        JObject itemObject2=null;
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
        var query = from t in orderDb.CAKE_ORDERS_DTL_VIEW where t.ODDNO == param.ODDNO && t.STATUS !="C" select t;
        i = query.Count();
        return i;
    }





    public JArray getCombobox1(String kind)
    {    
        JArray ja = new JArray();
        var query = (from t in orderDb.CAKE_CLASSCODE
                     where t.KIND == kind 
                     select new
                     {
                         VALUE = t.CODE1,
                         ITEM =t.NAME1

                     }
        );
        var itemObject0 = new JObject();
        itemObject0.Add("VALUE", "0");
        itemObject0.Add("ITEM", "0 大類選擇");
        ja.Add(itemObject0);
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    { 
                        {"VALUE",StringUtils.getString(item.VALUE)},
                        {"ITEM",StringUtils.getString(item.VALUE) + " " +StringUtils.getString(item.ITEM)}
                    };

            ja.Add(itemObject);
        }

        return ja;
    }

    public JArray getProduct(String POSPNO)
    {
        JArray ja = new JArray();        
        var query = (from t in fdDb.FDPOST01 where t.POSPNO == POSPNO select t);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"POSPNO",StringUtils.getString(item.POSPNO)},
                        {"POSPNM",StringUtils.getString(item.POSPNM)},
                        {"COST",StringUtils.getString(item.COST)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }

    public JArray getCombobox(String kind,String code)
    {
        //select kind,code1,code2,name2 from cssett00 where kind='1'
        JArray ja = new JArray();        
        var query = (from t in orderDb.CAKE_CLASSCODE where t.KIND == kind && t.CODE1 ==code select new
        {
            VALUE = 
                (
                    kind == "1" ? t.CODE1 :
                    kind == "2" ? t.CODE2 :
                    kind == "3" ? t.CODE3 :
                    kind == "4" ? t.CODE4 : ""                                         
                ),
            ITEM =
                (
                    kind == "1" ? t.NAME1 :
                    kind == "2" ? t.NAME2 :
                    kind == "3" ? t.NAME3 :
                    kind == "4" ? t.NAME4 : ""
                ),
                CODE1 =t.CODE1

        }        
        );
        if (kind != "1")
        {
           // query.Where(t=> t.CODE1  ==code);            
        }
        
        var itemObject0 = new JObject();
        itemObject0.Add("VALUE", "0");
        if (kind == "1")
        {            
            itemObject0.Add("ITEM", "0 大類選擇");            
        }
        else if (kind == "2")
        {         
            itemObject0.Add("ITEM", "0 中分類");
        }
        else if (kind == "3")
        {
            itemObject0.Add("ITEM", "0 小分類");
        }
        else if (kind == "4")
        {
            itemObject0.Add("ITEM", "0 細分類");
        }
        ja.Add(itemObject0);
        foreach (var item in query.ToList())
        {
              var itemObject = new JObject
                    { 
                        {"VALUE",StringUtils.getString(item.VALUE)},
                        {"ITEM",StringUtils.getString(item.VALUE) + " " +StringUtils.getString(item.ITEM)}
                    };

            ja.Add(itemObject);
        }

        return ja;
    }



    public int getTotalCount(CakeOrderParamPoco param)
   {
       JArray ja = new JArray();
        int i= 0;
       var query = (from t in fdDb.FDPOST01 where t.POSNO == "99" && t.SALE == "Y" select t);
       if (StringUtils.getString(param.MODE)=="Q")
       {
           if (param.TYPE1 != null && param.TYPE1 != "0")
           {
               query = query.Where(q => q.POSPNO.Substring(0, 1) == param.TYPE1);
           }
           if (param.TYPE2 != null && param.TYPE2 != "0")
           {
               query = query.Where(q => q.POSPNO.Substring(1, 1) == param.TYPE2);
           }
           if (param.TYPE3 != null && param.TYPE3 != "0")
           {
               query = query.Where(q => q.POSPNO.Substring(2, 1) == param.TYPE3);
           }
           if (param.TYPE4 != null && param.TYPE4 != "0")
           {
               query = query.Where(q => q.POSPNO.Substring(3, 1) == param.TYPE4);
           }
           if (StringUtils.getString(param.SCODE) != "")
           {
               query = query.Where(q => q.POSPNO.Contains(param.SCODE));
           }
           if (StringUtils.getString(param.SNAME) != "")
           {
               query = query.Where(q => q.POSPNM.Contains(param.SNAME) || q.POSPNO.Contains(param.SNAME));
           }
       }
       else
       {
           if (StringUtils.getString(param.MODE)=="0")  //主廚推薦
           {
               //select posno,pospno,pospnm,sale,package,to_char(cost) cost from fdpost01 where posno='99' and sale='Y' and package='P' order by posno
               query = query.Where(q => q.PACKAGE == "P");
           }
       }
       i = query.Count();
       return i; 
   }

    
   public int doSave(Hashtable user,CAKE_ORDERS odders,CAKE_ORDERS_DTL dtl) 
   {       
       int i = 0;
       using (Entities context = new Entities())
       {

           var query = (from t in context.CAKE_ORDERS select t);
           var queryDtl = (from t in context.CAKE_ORDERS_DTL select t);
         //  var orders = context.CAKE_ORDERS.Include("CAKE_ORDERS_DTL");
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;
           CAKE_ORDERS obj = null;
           CAKE_ORDERS_DTL objDtl = null;
           if (odders.ODDNO == "" || odders.ODDNO == null)
           {
               obj = new CAKE_ORDERS();
               

               obj.BDATE = currentTime;
               obj.ODATE = currentTime;
               //obj.OUSER = HashtableUtil.getValue(user, "USERID");
               obj.BUSER = HashtableUtil.getValue(user, "USERID");
               obj.COMP = HashtableUtil.getValue(user, "COMP_CODE");
               obj.DEPT = HashtableUtil.getValue(user, "DEPART_CODE");
               obj.STATUS = "B"; //新增狀態為預定 B   
               
               objDtl = new CAKE_ORDERS_DTL();
               objDtl.STATUS = "N";
           }
           else
           {
               if (dtl.UUID > 0 )
               {
                   //修改明細
                   objDtl = queryDtl.Where(q => q.UUID == dtl.UUID).SingleOrDefault();
                   obj = query.Where(q => q.ODDNO == odders.ODDNO).SingleOrDefault();
               }
               else
               {
                   //新增一筆明細
                   objDtl = new CAKE_ORDERS_DTL();
                   objDtl.ODDNO = dtl.ODDNO;
                   objDtl.STATUS = "N";
                   obj = query.Where(q => q.ODDNO == dtl.ODDNO).SingleOrDefault();
               }

           }
           
               
           objDtl.COST = dtl.COST;
           objDtl.QTY = dtl.QTY;
           objDtl.PDNO = dtl.PDNO;
           objDtl.REMARK = dtl.REMARK;
          

               obj.CAKE_ORDERS_DTL.Add(objDtl);
               if (odders.ODDNO == "" || odders.ODDNO == null)
               {                   
                   context.CAKE_ORDERS.AddObject(obj);
                   
               }
               //orderDb.CAKE_ORDERS_DTL.AddObject(objDtl);
           
           i = context.SaveChanges();
   
       }
       return i;
   }

   public int doCreate(CAKE_ORDERS param)
   {
       int i = 0;
       /*
     if (param.CNO != null) { 
       ROOMF obj = new ROOMF();
       obj.CNO = param.CNO;
       obj.PINS = param.PINS;
       obj.ADR1 = param.ADR1;
       obj.DOOR = param.DOOR;
       obj.FLOR = param.FLOR;
       obj.PINS0 = param.PINS0;
       obj.PRCN = param.PRCN;
       obj.EXTNO = param.EXTNO;
       obj.LOCT = param.LOCT;
       obj.STY = param.STY;
       obj.PICNM = param.PICNM;
       obj.IP1 = param.IP1;
       obj.IP2 = param.IP2;
       obj.WATERNO = param.WATERNO;
       obj.ELECNO = param.ELECNO;
       obj.GASNO = param.GASNO;       
       db.ROOMF.Add(obj);
       i = db.SaveChanges();
        * */
       
   //}
    return i;
   
   }


   public JArray queryOrderDetail(Decimal UUID)
   {
       JArray ja = new JArray();
       JArray ja2 = new JArray();
       JObject jo = new JObject();

       var query = from t in orderDb.CAKE_ORDERS_DTL_VIEW
                   where t.UUID == UUID
                   select new { UUID = t.UUID, QTY = t.QTY, ODDNO = t.ODDNO, PDNO = t.PDNO, PDNAME = t.POSPNM, t.COST, PRICE = t.PRICE, PRICE_COUNT = t.PRICE_COUNT, REMARK = t.REMARK, REMARK_STR = t.REMARK_STR };

       query.AsEnumerable();
       
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {   
                        {"UUID",StringUtils.getString(item.UUID)},                
                        {"ODDNO",StringUtils.getString(item.ODDNO)},
                        {"QTY",StringUtils.getString(item.QTY)},
                        {"PDNO",StringUtils.getString(item.PDNO)},
                        {"PDNAME",StringUtils.getString(item.PDNAME)},
                        {"PRICE",StringUtils.getString(item.PRICE)},
                        {"COST",StringUtils.getString(item.COST)},
                        {"REMARK",StringUtils.getString(item.REMARK)},
                        {"REMARK_STR",StringUtils.getString(item.REMARK_STR)}
                    };
           ja.Add(itemObject);
       }
       return ja;
   }


   public int doRemove(String oddno)
   {
       int i=1;

       CAKE_ORDERS obj = (from c in orderDb.CAKE_ORDERS
                     where  c.ODDNO== oddno
                 select c).Single();
 //   db.CAKE_ORDERS.
       orderDb.SaveChanges();

       return i;
   }

   public int submitOrder(Hashtable user, String ODDNO,String SDATE)
   {
       int i = 1;
       CAKE_ORDERS obj = (from c in orderDb.CAKE_ORDERS
                          where c.ODDNO == ODDNO
                          select c).Single();

       DateTime currentTime = new DateTime();
       currentTime = System.DateTime.Now;

       obj.STATUS = "O";   //申請
       obj.ODATE = currentTime;  //申請日期
       obj.OUSER = HashtableUtil.getValue(user, "USERID");  // 申請人員
       obj.SDATE = DateTimeUtil.getDateTime(SDATE);   //預定日期     
       orderDb.SaveChanges();
       return i;
   }    

   public int cancelOrder(Hashtable user,String ODDNO)
   {
       int i = 1;
       CAKE_ORDERS obj = (from c in orderDb.CAKE_ORDERS
                          where c.ODDNO == ODDNO
                          select c).Single();

          DateTime currentTime = new DateTime();
          currentTime = System.DateTime.Now;              

         
       
         obj.STATUS = "C";   //作廢
         obj.CDATE = currentTime;
       
       orderDb.SaveChanges();
       return i;
   }

   public int copyOrder(Hashtable user, String ODDNO)
   {
       int i = 1;
       using (Entities context = new Entities())
       {
           var query = (from c in context.CAKE_ORDERS
                        where c.ODDNO == ODDNO
                        select c).Single();



           var dtl_query = (from t in context.CAKE_ORDERS_DTL where t.ODDNO == ODDNO && t.STATUS !="C" select t);


           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;

           if (query != null)
           {
               CAKE_ORDERS obj = new CAKE_ORDERS();               
               obj.BDATE = currentTime;
               obj.ODATE = currentTime;
               //obj.OUSER = HashtableUtil.getValue(user, "USERID");
               obj.BUSER = HashtableUtil.getValue(user, "USERID");               
               obj.STATUS = "B";   //預定           
               obj.COMP = HashtableUtil.getValue(user, "COMP_CODE");
               obj.DEPT = HashtableUtil.getValue(user, "DEPART_CODE");

               foreach (var item in dtl_query)
               {
                   CAKE_ORDERS_DTL objDtl = new CAKE_ORDERS_DTL();
                   objDtl.PDNO = item.PDNO;
                   objDtl.QTY = item.QTY;
                   objDtl.COST = item.COST;
                   objDtl.REMARK = item.REMARK;
                   objDtl.STATUS = "N";
                   obj.CAKE_ORDERS_DTL.Add(objDtl);
               }                
                context.CAKE_ORDERS.AddObject(obj);
                context.SaveChanges();
           }
           
       }
       return i;
   }



   public int doRemoveDetail( Decimal UUID)
   {
       int i = 1;

       CAKE_ORDERS_DTL obj = (from c in orderDb.CAKE_ORDERS_DTL
                          where c.UUID == UUID
                          select c).Single();
       //orderDb.CAKE_ORDERS_DTL.DeleteObject(obj);
       obj.STATUS = "C";
       orderDb.SaveChanges();
       return i;
   }


    //追加數量
   public int doAddDetail(Decimal UUID,Decimal AQTY,String ADD_REMARK,String userid,String ODDNO)
   {
       DateTime currentTime = new DateTime();
       currentTime = System.DateTime.Now;
       int i=0;
       using (Entities context = new Entities())
       {
           CAKE_ORDERS_DTL_ADD obj = new CAKE_ORDERS_DTL_ADD();
           obj.DTL_UUID = UUID;
           obj.ADD_REMARK = ADD_REMARK;
           obj.AQTY = AQTY;
           obj.CR_DT = currentTime;
           obj.ODDNO = ODDNO;
           obj.USERID = userid;
           context.CAKE_ORDERS_DTL_ADD.AddObject(obj);
           i = context.SaveChanges();
       }
       return i;

   }


   public JObject getAddDetailList(String UUID,String QTY,EasyuiParamPoco param)
   {
       decimal uuid = StringUtils.getDecimal(UUID);
       var query =from t in orderDb.CAKE_ORDERS_DTL_ADD where t.DTL_UUID == uuid select t;
       query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序  
       JArray ja = new JArray();
       JObject jo = new JObject();
       foreach (var item in query)
       {
           /*
           String addremark = StringUtils.getString(item.ADD_REMARK);
           if (StringUtils.getString(item.ODDNO) != "")
           {
               addremark += " (手工調整單,調整人員:" + item.USERID + ")";
           }*/
           var itemObject = new JObject
                    {   
                        {"UUID",StringUtils.getString(item.UUID)},                
                        {"AQTY",StringUtils.getString(item.AQTY)},
                        {"ADD_REMARK",StringUtils.getString(item.ADD_REMARK)},
                        {"CR_DT",StringUtils.getString(item.CR_DT)}
                    };
           ja.Add(itemObject);
       }
       JArray ja2 = new JArray();
       Decimal total = getSumByAQTY(UUID) ;
       var itemObject2 = new JObject
                    {   
                        {"UUID",""},
                        {"AQTY",StringUtils.getString(total)},
                        {"ADD_REMARK","預定數:"+QTY+",合計共:"+ StringUtils.getString(total+StringUtils.getDecimal(QTY))},
                        {"CR_DT","增減數量小計"},  
                    };
       ja2.Add(itemObject2);
       jo.Add("rows", ja);
       jo.Add("footer", ja2);
       return jo;
   }

   // select sum(aqty) aqty from cake_orders_dtl_add where dtl_uuid=2226063 group by dtl_uuid
   public Decimal getSumByAQTY(String UUID)
   {

       Decimal uuid=StringUtils.getDecimal(UUID);
       Decimal AQTY=0;
       var query = from t in orderDb.CAKE_ORDERS_DTL_ADD where t.DTL_UUID == uuid group t by t.DTL_UUID into g select g.Sum(s => s.AQTY);

       foreach (var item in query)
       {
           AQTY = item.Value;
       }
       return AQTY;
   }
}
}