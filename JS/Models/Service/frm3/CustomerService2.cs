
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

namespace Jasper.service.frm2{
    public class CustomerService2 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();




    public JObject getCusTab1DG(String TAGID,String CSTA)
   {

       JArray ja = new JArray();
       JObject jo = new JObject();
       var query = from t in db.RMLIV where t.JOBTAG==TAGID  select t;

       if (StringUtils.getString(CSTA) == "1")
       {
           query = query.Where(t => t.RSTA == "");       

       }else if(StringUtils.getString(CSTA) == "2"){
           query = query.Where(t => t.RSTA != "");
       }
       query = query.OrderBy(t => t.CIDT);

       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                                           
                        {"MAN",item.MAN},
                        {"SEX",item.SEX},
                        {"APPES",item.APPES},
                        {"BIRTH",String.Format("{0:yyyy-MM-dd}",item.BIRTH)},
                        {"EMAIL",item.EMAIL},
                        {"CIDT",String.Format("{0:yyyy-MM-dd}",item.CIDT)},
                        {"CNTRY",item.CNTRY},
                        {"BLOD",item.BLOD},
                        {"BLODTP",item.BLODTP},
                        {"CODT",String.Format("{0:yyyy-MM-dd}",item.CODT)},
                        {"PID",item.PID},
                        {"POSL",item.POSL},
                        {"MARRI",item.MARRI},
                        {"NOTES",item.NOTES},
                        {"RSTA",item.RSTA},
                        {"TAGID",item.TAGID},
                        {"EMAILPW",item.EMAILPW}                        
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


    //取得住戶combobox
    public JArray getOWNER(String TAGID,String CSTA)
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        //var query = from t in db.CONFCODE where t.CODE_KIND == "APPE" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

        var query = (from t in db.RMLIV where t.JOBTAG == TAGID  select new { CNO = t.MAN, CNM = "住戶",RSTA = t.RSTA }).Distinct().Union
                    (from t in db.RMCAR where t.JOBTAG == TAGID  select new { CNO = t.DRIVER, CNM = "司機", RSTA = t.RSTA }).Distinct();

        if (StringUtils.getString(CSTA) == "1")
        {
            query = query.Where(t => t.RSTA == "");

        }
        else if (StringUtils.getString(CSTA) == "2")
        {
            query = query.Where(t => t.RSTA != "");
        }
       
                    

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"CNO",item.CNO},
                        {"CNM",item.CNM + " - " +item.CNO },
                        
                    };
            ja.Add(itemObject);
        }
        //  jo.Add("rows", ja);            
        return ja;
    }

    //取得住戶combobox
    public JArray getOWNER2(String TAGID,String CSTA)
    {
        JObject jo = new JObject();
        JArray ja = new JArray();

        //var query = from t in db.CONFCODE where t.CODE_KIND == "APPE" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

        var query = (from t in db.RMLIV where t.JOBTAG == TAGID  select new { CNO = t.MAN,RSTA=t.RSTA }).Distinct();

        if (StringUtils.getString(CSTA) == "1")
        {
            query = query.Where(t => t.RSTA == "");

        }
        else if (StringUtils.getString(CSTA) == "2")
        {
            query = query.Where(t => t.RSTA != "");
        }


        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"CNO",item.CNO},
                        {"CNM",item.CNO },
                        
                    };
            ja.Add(itemObject);
        }
        //  jo.Add("rows", ja);            
        return ja;
    }

    public JObject getCusTab2DG(String TAGID,String CSTA)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = from t in db.RMTEL where t.JOBTAG == TAGID  select t;
        if (StringUtils.getString(CSTA) == "1")
        {
            query = query.Where(t => t.RSTA == "");

        }
        else if (StringUtils.getString(CSTA) == "2")
        {
            query = query.Where(t => t.RSTA != "");
        }
        query = query.OrderBy(t => t.TAGID);

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"TELTP",item.TELTP},
                        {"TELNO",item.TELNO},
                        {"OWNER",item.OWNER},
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

    public JObject getCusTab3DG(String TAGID,String CSTA)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = from t in db.RMCAR where t.JOBTAG == TAGID  select t;
        if (StringUtils.getString(CSTA) == "1")
        {
            query = query.Where(t => t.RSTA == "");

        }
        else if (StringUtils.getString(CSTA) == "2")
        {
            query = query.Where(t => t.RSTA != "");
        }
        query = query.OrderBy(t => t.TAGID);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CARNO",item.CARNO},
                        {"CARTP",item.CARTP},
                        {"DRIVER",item.DRIVER},
                        {"PARKID",item.PARKID},
                        {"PARKNO",item.PARKNO},
                        {"IDDT",String.Format("{0:yyyy-MM-dd}",item.IDDT)},
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

    public JArray getPARKNO(String TAGID,String CSTA)
    {
        JObject jo = new JObject();
        JArray ja = new JArray();
        var query = (from t in db.CONTRAP where t.JOBTAG == TAGID select new { CNO = t.PARKNO , CNM = t.LOCT,CNSTA=t.CNSTA });
        if (StringUtils.getString(CSTA) == "1")
        {
            query = query.Where(t => t.CNSTA== "");

        }
        else if (StringUtils.getString(CSTA) == "2")
        {
            query = query.Where(t => t.CNSTA != "");
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"CNO",item.CNO},
                        {"CNM",item.CNM + " - " +item.CNO},
                        
                    };
            ja.Add(itemObject);
        }
        //  jo.Add("rows", ja);            
        return ja;
    }


    public JObject getCusTab4DG(String TAGID,String CSTA)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = from t in db.RMAST where t.JOBTAG == TAGID  select t;
        if (StringUtils.getString(CSTA) == "1")
        {
            query = query.Where(t => t.RSTA == "");

        }
        else if (StringUtils.getString(CSTA) == "2")
        {
            query = query.Where(t => t.RSTA != "");
        }
        query = query.OrderBy(t => t.TAGID);

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"ASST",item.ASST},
                        {"CIDT",String.Format("{0:yyyy-MM-dd}",item.CIDT)},
                        {"CODT",String.Format("{0:yyyy-MM-dd}",item.CODT)},
                        {"CNTRY",item.CNTRY},
                        {"NOTES",item.NOTES},
                        {"WORKTP",item.WORKTP},
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

    public JArray getASST(String TAGID,String CSTA)
    {
        JObject jo = new JObject();
        JArray ja = new JArray();
        var query = (from t in db.HOMEAST select new { CNO = t.CNM, CNM = t.CNM,COUNTRY=t.COUNTRY }).OrderBy(t=>t.COUNTRY);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {   
                        {"CNO",item.CNO},
                        {"CNM",item.CNO},
                        
                    };
            ja.Add(itemObject);
        }
        //  jo.Add("rows", ja);            
        return ja;
    }

    public int doSave(Frm201cPoco param)
    {
        int i = 0;
        DateTime? tmp = null;
        using (db)
        {
            switch (param.TYPE)
            {
                case "0":
                    RMLIV obj1;
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        obj1 = new RMLIV();
                        obj1.TAGID = cs.getTagidByDatetime();
                        obj1.JOBTAG = param.JOBTAG;
                        obj1.RSTA = " ";
                    }
                    else
                    {
                        obj1 = (from t in db.RMLIV where t.TAGID == param.TAGID select t).Single();
                    }
                    obj1.APPES = StringUtils.getString(param.APPES);
                    
                    obj1.BIRTH =StringUtils.getString(param.BIRTH) == "" ? tmp : DateTimeUtil.getDateTime(param.BIRTH);

                    obj1.CIDT = StringUtils.getString(param.CIDT) == "" ? tmp : DateTimeUtil.getDateTime(param.CIDT);
                    obj1.CODT = StringUtils.getString(param.CODT) == "" ? tmp : DateTimeUtil.getDateTime(param.CODT);    
                    //obj1.CODT = DateTimeUtil.getDateTime(param.CODT);
                    obj1.BLOD = StringUtils.getString(param.BLOD);
                    obj1.BLODTP = StringUtils.getString(param.BLODTP);
                    obj1.CNTRY = StringUtils.getString(param.CNTRY);
                    obj1.EMAIL = StringUtils.getString(param.EMAIL);
                    obj1.EMAILPW = StringUtils.getString(param.EMAILPW);
                    obj1.NOTES = StringUtils.getString(param.NOTES);
                    obj1.MARRI = StringUtils.getInt(param.MARRI);
                    obj1.MAN = StringUtils.getString(param.MAN);                    
                    obj1.PID = StringUtils.getString(param.PID);
                    obj1.POSL = StringUtils.getString(param.POSL);
                    obj1.SEX = StringUtils.getString(param.SEX);
                    
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        db.RMLIV.Add(obj1);
                    }

                    break;
                case "1":
                    RMTEL obj2;
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        obj2 = new RMTEL();
                        obj2.TAGID = cs.getTagidByDatetime();
                        obj2.JOBTAG = param.JOBTAG;
                        obj2.RSTA = " ";
                    }
                    else
                    {
                        obj2 = (from t in db.RMTEL where t.TAGID == param.TAGID select t).Single();
                    }
                    obj2.OWNER = StringUtils.getString(param.OWNER);
                    obj2.TELNO = StringUtils.getString(param.TELNO);
                    obj2.TELTP = StringUtils.getString(param.TELTP);

                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        db.RMTEL.Add(obj2);
                    }
                    break;
                case "2":
                    RMCAR obj3;
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        obj3 = new RMCAR();
                        obj3.TAGID = cs.getTagidByDatetime();
                        obj3.JOBTAG = param.JOBTAG;
                        obj3.RSTA = " ";
                    }
                    else
                    {
                        obj3 = (from t in db.RMCAR where t.TAGID == param.TAGID select t).Single();
                    }

                    obj3.CARNO = StringUtils.getString(param.CARNO);
                    obj3.CARTP = StringUtils.getInt(param.CARTP);
                    obj3.DRIVER = StringUtils.getString(param.DRIVER);
                    obj3.IDDT = StringUtils.getString(param.IDDT) == "" ? tmp : DateTimeUtil.getDateTime(param.IDDT);
                    obj3.PARKID = StringUtils.getString(param.PARKID);
                    obj3.PARKNO = StringUtils.getString(param.PARKNO);
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        db.RMCAR.Add(obj3);
                    }
                    break;
                case "3":
                    RMAST obj4;
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        obj4 = new RMAST();
                        obj4.TAGID = cs.getTagidByDatetime();
                        obj4.JOBTAG = param.JOBTAG;
                        obj4.RSTA = " ";
                    }
                    else
                    {
                        obj4 = (from t in db.RMAST where t.TAGID == param.TAGID select t).Single();
                    }

                    obj4.ASST = StringUtils.getString(param.ASST);
                    obj4.CIDT = StringUtils.getString(param.CIDT) == "" ? tmp : DateTimeUtil.getDateTime(param.CIDT);
                    obj4.CODT = StringUtils.getString(param.CODT) == "" ? tmp : DateTimeUtil.getDateTime(param.CODT);
                    obj4.CNTRY = StringUtils.getString(param.CNTRY);
                    obj4.NOTES = StringUtils.getString(param.NOTES);
                    obj4.WORKTP = StringUtils.getInt(param.WORKTP);
                    if (StringUtils.getString(param.TAGID) == "")
                    {
                        db.RMAST.Add(obj4);
                    }
                    break;
            }
            i = db.SaveChanges();
        }


        return i;
    }


    public int doRemove(String TYPE,String TAGID,String CODT)
    {
        int i = 0;
        using (db)
        {
            switch (TYPE)
            {
                case "0":                     
                    if (StringUtils.getString(TAGID) != "")
                    {
                        RMLIV obj1 = (from t in db.RMLIV where t.TAGID == TAGID select t).Single();
                        obj1.RSTA = "C";
                        obj1.CODT = DateTimeUtil.getDateTime(CODT);
                    }
                    break;
                case "1":                    
                    if (StringUtils.getString(TAGID) != "")
                    {                         
                         RMTEL obj2 = (from t in db.RMTEL where t.TAGID == TAGID select t).Single();
                         obj2.RSTA = "C";
                    }
                    break;
                case "2":                    
                    if (StringUtils.getString(TAGID) != "")
                    {
                        RMCAR obj3 = (from t in db.RMCAR where t.TAGID == TAGID select t).Single();
                        obj3.RSTA = "C";
                    }
                    break;
                case "3":
                    if (StringUtils.getString(TAGID) != "")
                    {                         
                         RMAST obj4 = (from t in db.RMAST where t.TAGID == TAGID select t).Single();
                         obj4.RSTA = "C";
                         obj4.CODT = DateTimeUtil.getDateTime(CODT);
                    }                    
                    break;
            }
            i = db.SaveChanges();
        }

        return i;
    }




}
}