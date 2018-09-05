
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
using System.Data.Objects;




namespace IntranetSystem.Service
{
public class CakeOrdersManagerService 
{
    private Entities orderDb = new Entities();
    private FDPOSEntities fdDb = new FDPOSEntities();
    private CakeOrdersService cakeOrdersService = new CakeOrdersService();
    /*
 public enum Status : byte

{   B = "預定", O = "申請", P = "出單", F = "結案", C = "作廢"
 }*/




    /**
     * 訂單
     *
     *  */
    public JArray getOrderDatagrid(OrderParamPoco param,String STATUS)
    {
        JArray ja = new JArray();

        string[] strArray = new string [] { "O", "P"};

        if (StringUtils.getString(param.Q) == "Y")
        {
            strArray = new string[] { "O", "P", "F" };
        }

        var query = (from t in orderDb.CAKE_ORDERS_VIEW
                     where strArray.Contains(t.STATUS)
                     select new { ODDNO = t.ODDNO,
                                  OUSER = t.OUSER,
                                  ODATE = t.ODATE,
                                  SDATE =t.SDATE,
                                  STATUS = t.STATUS,
                                  O_NAME =t.O_NAME,  
                                  COMP =t.COMP,
                                  COMP_NAME =t.COMP_NAME,
                                  DEPT =t.DEPT,
                                  DEPT_NAME=t.DEPT_NAME
                                });

        if (StringUtils.getString(param.DT) != "")
        {
            DateTime dt = DateTimeUtil.getDateTime(param.DT );
           // query = query.Where(q => q.SDATE == dt);
           // query = query.Where(q => EntityFunctions.DiffDays(q.SDATE, dt) == 0);
            //query = query.Where(q => q.SDATE >= dt && q.SDATE <= dt);
            query = query.Where(q => q.SDATE.Value.Year  == dt.Year && q.SDATE.Value.Month  == dt.Month && q.SDATE.Value.Day  == dt.Day );
        }

        if (StringUtils.getString(param.ODDNO) != "")
        {
            query = query.Where(q => q.ODDNO.Contains(param.ODDNO));
        }

        if (StringUtils.getString(param.COMP) != "")
        {
            query = query.Where(q => q.COMP == param.COMP);
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
                        {"OUSER",StringUtils.getString(item.OUSER + " " + item.O_NAME) },
                        {"ODATE",String.Format("{0:yyyy-MM-dd HH:mm}",item.ODATE)},
                        {"SDATE",String.Format("{0:yyyy-MM-dd HH:mm}", item.SDATE)},
                        {"STATUS",StringUtils.getString(item.STATUS)},
                        {"STATUS_STR",status_str},
                        {"COMP",StringUtils.getString(item.COMP)},
                        {"COMP_NAME",StringUtils.getString(item.COMP_NAME)},
                        {"DEPT",StringUtils.getString(item.DEPT)},
                        {"DEPT_NAME",StringUtils.getString(item.DEPT_NAME)}
                    };
            ja.Add(itemObject);
        }
      

       return ja;
    }




    public int getOrderCount(OrderParamPoco param)
    {
        JArray ja = new JArray();
        int i = 0;
        string[] strArray = new string[] { "O", "P" };
        if (StringUtils.getString(param.Q) == "Y")
        {
            strArray = new string[] { "O", "P", "F" };
        }
        var query = (from t in orderDb.CAKE_ORDERS_VIEW where strArray.Contains(t.STATUS) select t);
        if (StringUtils.getString(param.DT) != "")
        {
            DateTime dt = DateTimeUtil.getDateTime(param.DT);
//            query = query.Where(q => q.SDATE == dt );

            //query = query.Where(q => EntityFunctions.DiffDays(q.SDATE, dt) == 0);
            query = query.Where(q => q.SDATE.Value.Year == dt.Year && q.SDATE.Value.Month == dt.Month && q.SDATE.Value.Day == dt.Day);
            
        }

        int x = query.Count();

        if (StringUtils.getString(param.ODDNO) != "")
        {
            query = query.Where(q => q.ODDNO.Contains(param.ODDNO));
        }

        if (StringUtils.getString(param.COMP) != "")
        {
            query = query.Where(q => q.COMP == param.COMP);
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
                     where t.ODDNO == param.ODDNO && t.STATUS!="C"
                    select new {UUID = t.UUID, QTY = t.QTY,SQTY=t.SQTY , ODDNO = t.ODDNO, PDNO = t.PDNO, PDNAME = t.POSPNM, t.COST, PRICE = t.PRICE, PRICE_COUNT = t.PRICE_COUNT, REMARK = t.REMARK, REMARK_STR = t.REMARK_STR,CAKE_REMARK =t.CAKE_REMARK };

        query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
    //    query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁    
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
                        {"SELQTY",StringUtils.getString(item.SQTY)},
                        {"PDNO",StringUtils.getString(item.PDNO)},
                        {"PDNAME",StringUtils.getString(item.PDNAME)},
                        {"PRICE",StringUtils.getString(item.PRICE)},
                        {"COST",StringUtils.getString(item.COST)},
                        {"REMARK",StringUtils.getString(item.REMARK)},
                        {"REMARK_STR",StringUtils.getString(item.REMARK_STR)},
                        {"CAKE_REMARK",StringUtils.getString(item.CAKE_REMARK)},
                        {"AQTY",StringUtils.getString(cakeOrdersService.getSumByAQTY(StringUtils.getString(item.UUID))+ item.QTY)}
                    };
            /*
             itemObject2 = new JObject
                    {                                           
                        {"PDNAME","合計:"},
                        {"PRICE",StringUtils.getString(item.PRICE_COUNT)},                        
                        {"REMARK_STR",""},  
                    };*/
            
            ja.Add(itemObject);
            
        }
        ja2.Add(itemObject2);
        jo.Add("rows", ja);
      //  jo.Add("footer", ja2);
        if (count == 0)
        {
            jo.Add("total", "");
        }
        else
        {
            jo.Add("total", count);
        }
        return jo;
    }


    public int getOrderDetailCount(OrderParamPoco param)
    {
        JArray ja = new JArray();
        int i = 0;
        var query = from t in orderDb.CAKE_ORDERS_DTL where t.ODDNO == param.ODDNO && t.STATUS != "C" select t;
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



  

    
   public int doSave(Hashtable user,CAKE_ORDERS odders,CAKE_ORDERS_DTL dtl) 
   {       
       int i = 0;
       using (Entities context = new Entities())
       {

           var query = (from t in context.CAKE_ORDERS select t);
           var queryDtl = (from t in context.CAKE_ORDERS_DTL select t);
           var orders = context.CAKE_ORDERS.Include("CAKE_ORDERS_DTL");
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;
           CAKE_ORDERS obj = null;
           CAKE_ORDERS_DTL objDtl = null;
           if (odders.ODDNO == "" || odders.ODDNO == null)
           {
               obj = new CAKE_ORDERS();
               

               obj.BDATE = currentTime;
               obj.ODATE = currentTime;
               obj.BUSER = HashtableUtil.getValue(user, "USERID");
               obj.COMP = HashtableUtil.getValue(user, "COMP_CODE");
               obj.DEPT = HashtableUtil.getValue(user, "DEPART_CODE");
               obj.STATUS = "B"; //新增狀態為預定 B   
               
               objDtl = new CAKE_ORDERS_DTL();
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

  


   public JArray queryOrderDetail(Decimal UUID)
   {
       JArray ja = new JArray();
       JArray ja2 = new JArray();
       JObject jo = new JObject();

       var query = from t in orderDb.CAKE_ORDERS_DTL_VIEW
                   where t.UUID == UUID
                   select new { UUID = t.UUID, QTY = t.QTY, SQTY = t.SQTY, ODDNO = t.ODDNO, PDNO = t.PDNO, PDNAME = t.POSPNM, t.COST, PRICE = t.PRICE, PRICE_COUNT = t.PRICE_COUNT, REMARK = t.REMARK, REMARK_STR = t.REMARK_STR,CAKE_REMARK=t.CAKE_REMARK };

       query.AsEnumerable();
       
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
                        {"CAKE_REMARK",StringUtils.getString(item.CAKE_REMARK)}
                    };
           ja.Add(itemObject);
       }
       return ja;
   }


   //作廢
   public int cancelOrder(Hashtable user, String ODDNO,String REMARK)
   {
       int i = 1;
       using (Entities context = new Entities())
       {

           CAKE_ORDERS obj = (from c in context.CAKE_ORDERS
                              where c.ODDNO == ODDNO
                              select c).Single();
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;
           obj.STATUS = "C";   //作廢
           obj.REMARK = REMARK;
           obj.CUSER = HashtableUtil.getValue(user, "USERID");
           obj.CDATE = currentTime;
           i = context.SaveChanges();
       }
       return i;
   }


   //結案
   public int closeOrder(Hashtable user, String ODDNO)
   {
       int i = 1;
       using (Entities context = new Entities())
       {
           
           CAKE_ORDERS obj = (from c in context.CAKE_ORDERS
                              where c.ODDNO == ODDNO
                              select c).Single();
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;

           obj.STATUS = "F";   //結案
           obj.FDATE = currentTime;
           obj.FUSER = HashtableUtil.getValue(user, "USERID");
           i = context.SaveChanges();
       }
       return i;
   }

   //結案
   public int closeOrderNew(Hashtable user, List<CAKE_ORDERS_DTL> list)
   {
       int i = 1;
       using (Entities context = new Entities())
       {
           String oddno = list[0].ODDNO.ToString();
           CAKE_ORDERS obj = (from c in context.CAKE_ORDERS
                              where c.ODDNO == oddno
                              select c).Single();
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;

           obj.FDATE = currentTime;
           obj.FUSER = HashtableUtil.getValue(user, "USERID");
           obj.STATUS = "F";   //結案
           
           foreach (CAKE_ORDERS_DTL dtl in list)
           {
               Decimal uuid = StringUtils.getDecimal(dtl.UUID);
               CAKE_ORDERS_DTL objDtl = (from c in context.CAKE_ORDERS_DTL
                                         where c.UUID == uuid
                                         select c).Single();

               //若數量異動寫入log
               /*
               if (StringUtils.getString(dtl.SQTY) != "" && dtl.SQTY != dtl.QTY && objDtl.SQTY != dtl.SQTY)
               {
                   CAKE_ORDERS_DTL_LOG log = new CAKE_ORDERS_DTL_LOG();
                   log.CR_DT = currentTime;
                   log.CR_USER = HashtableUtil.getValue(user, "USERID");
                   log.ODDNO = dtl.ODDNO;
                   log.PDNO = dtl.PDNO;
                   log.QTY = dtl.QTY;
                   log.SQTY = dtl.SQTY;
                   context.CAKE_ORDERS_DTL_LOG.AddObject(log);
               }*/
               if (dtl.SQTY == null )
               {
                   objDtl.SQTY = dtl.QTY;
               }
               else
               {
                   objDtl.SQTY = dtl.SQTY;
               }
               objDtl.CAKE_REMARK = dtl.CAKE_REMARK;
               obj.CAKE_ORDERS_DTL.Add(objDtl);
           }

           i = context.SaveChanges();
       }
       return i;
   }



   //出單
   public int printOrder(Hashtable user, List<CAKE_ORDERS_DTL> list)
   {
       int i = 1;
       using (Entities context = new Entities())
       {
            String oddno= list[0].ODDNO.ToString();
           CAKE_ORDERS obj = (from c in context.CAKE_ORDERS
                              where c.ODDNO == oddno
                              select c).Single();
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;

           obj.PDATE = currentTime;
           obj.PUSER = HashtableUtil.getValue(user, "USERID");
           obj.STATUS = "P";   //出單
           foreach (CAKE_ORDERS_DTL dtl in list)
           {
               Decimal uuid = StringUtils.getDecimal(dtl.UUID);
               CAKE_ORDERS_DTL objDtl = (from c in context.CAKE_ORDERS_DTL
                                         where c.UUID == uuid
                                         select c).Single();

               //若數量異動寫入log
               /*
               if (StringUtils.getString(dtl.SQTY) != "" && dtl.SQTY != dtl.QTY && objDtl.SQTY != dtl.SQTY)
               {
                   CAKE_ORDERS_DTL_LOG log = new CAKE_ORDERS_DTL_LOG();
                   log.CR_DT = currentTime;
                   log.CR_USER = HashtableUtil.getValue(user, "USERID");
                   log.ODDNO = dtl.ODDNO;
                   log.PDNO = dtl.PDNO;
                   log.QTY = dtl.QTY;
                   log.SQTY = dtl.SQTY;
                   context.CAKE_ORDERS_DTL_LOG.AddObject(log);
               }*/

               if (dtl.SQTY == null)
               {
                   objDtl.SQTY = dtl.QTY;
               }
               else
               {
                   objDtl.SQTY = dtl.SQTY;
               }

               objDtl.CAKE_REMARK = dtl.CAKE_REMARK;               
               obj.CAKE_ORDERS_DTL.Add(objDtl);
           }

           i = context.SaveChanges();
       }
       return i;
   }

   



   public DataTable getDataTableByCAKE_ORDERS_VIEW(String ODDNO)
   {
       LinqExtensions le = new LinqExtensions();
       var query = (from t in orderDb.CAKE_ORDERS_VIEW where t.ODDNO == ODDNO && t.STATUS != "C" select t);
       DataTable dt = le.LinqQueryToDataTable(query);
       return dt;
   }

   public DataTable getDataTableByCAKE_ORDERS_DTL_VIEW(String ODDNO)
   {
       LinqExtensions le = new LinqExtensions();
       var query = (from t in orderDb.CAKE_ORDERS_DTL_VIEW where t.ODDNO==ODDNO && t.STATUS !="C" select t);
       DataTable dt = le.LinqQueryToDataTable(query);
       return dt;
   }


   public JArray getInfoDatagrid(OrderParamPoco param)
   {
       JArray ja = new JArray();



       var query = (from t in orderDb.CAKE_ORDERS_VIEW
                    where t.STATUS =="P"
                    select new
                    {
                        ODDNO = t.ODDNO,
                        OUSER = t.OUSER,
                        ODATE = t.ODATE,
                        SDATE = t.SDATE,
                        STATUS = t.STATUS,
                        O_NAME = t.O_NAME,
                        COMP = t.COMP,
                        COMP_NAME = t.COMP_NAME,
                        DEPT = t.DEPT,
                        DEPT_NAME = t.DEPT_NAME
                    });

       DateTime currentTimeSdt = new DateTime();
       DateTime currentTimeEdt = new DateTime();
       currentTimeSdt = System.DateTime.Now.AddMonths(-2);
       currentTimeEdt = System.DateTime.Now;
       query = query.Where(q => q.SDATE >= currentTimeSdt && q.SDATE <= currentTimeEdt);

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
                        {"OUSER",StringUtils.getString(item.OUSER + " " + item.O_NAME) },
                        {"ODATE",String.Format("{0:yyyy-MM-dd HH:mm}",item.ODATE)},
                        {"SDATE",String.Format("{0:yyyy-MM-dd HH:mm}", item.SDATE)},
                        {"STATUS",StringUtils.getString(item.STATUS)},
                        {"STATUS_STR",status_str},
                        {"COMP",StringUtils.getString(item.COMP)},
                        {"COMP_NAME",StringUtils.getString(item.COMP_NAME)},
                        {"DEPT",StringUtils.getString(item.DEPT)},
                        {"DEPT_NAME",StringUtils.getString(item.DEPT_NAME)}
                    };
           ja.Add(itemObject);
       }


       return ja;
   }



   public int getInfoCount(OrderParamPoco param)
   {
       JArray ja = new JArray();
       var query = (from t in orderDb.CAKE_ORDERS_VIEW
                    where t.STATUS == "P"
                    select new
                    {
                        ODDNO = t.ODDNO,
                        OUSER = t.OUSER,
                        ODATE = t.ODATE,
                        SDATE = t.SDATE,
                        STATUS = t.STATUS,
                        O_NAME = t.O_NAME,
                        COMP = t.COMP,
                        COMP_NAME = t.COMP_NAME,
                        DEPT = t.DEPT,
                        DEPT_NAME = t.DEPT_NAME
                    });

       DateTime currentTimeSdt = new DateTime();
       DateTime currentTimeEdt = new DateTime();
       currentTimeSdt = System.DateTime.Now.AddMonths(-2);
       currentTimeEdt = System.DateTime.Now;
       query = query.Where(q => q.SDATE >= currentTimeSdt && q.SDATE <= currentTimeEdt);

       return query.Count();
   }

   public JObject getAddDetailList(String UUID, EasyuiParamPoco param)
   {
       decimal uuid = StringUtils.getDecimal(UUID);
       var query = from t in orderDb.CAKE_ORDERS_DTL_ADD where t.DTL_UUID == uuid select t;
       query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序  
       JArray ja = new JArray();
       JObject jo = new JObject();
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {   
                        {"UUID",StringUtils.getString(item.UUID)},                
                        {"AQTY",StringUtils.getString(item.AQTY)},
                        {"ADD_REMARK",StringUtils.getString(item.ADD_REMARK)},
                        {"CR_DT",StringUtils.getString(item.CR_DT)}
                    };
           ja.Add(itemObject);
       }

       jo.Add("rows", ja);
       return jo;
   }

}
}