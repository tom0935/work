
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

namespace Jasper.service.frm3{
    public class f3031
    {
        public String CNO { get; set; }
        public String CNM { get; set; }
        public String TP { get; set; }        
        public string YRN { get; set; }
        public string ACCNO { get; set; }
        public string ACCTP { get; set; }
        public int BUDGET { get; set; }
        public int USEAMT { get; set; }
        public string TAGID { get; set; }
    }


    public class f3032
    {
        public String CNO { get; set; }
        public String CNM { get; set; }        
        public string YRN { get; set; }

        public string TP { get; set; }
        public int BUDGET { get; set; }
        public int USEAMT { get; set; }
        public int M1 { get; set; }
        public int M2 { get; set; }
        public int M3 { get; set; }
        public int M4 { get; set; }
        public int M5 { get; set; }
        public int M6 { get; set; }
        public int M7 { get; set; }
        public int M8 { get; set; }
        public int M9 { get; set; }
        public int M10 { get; set; }
        public int M11 { get; set; }
        public int M12 { get; set; }
    }

public class Frm303Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDG1(String yrn)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        
        /*
        var query = from t in db.ACCBUDGET where t.YRN==yrn
                    join b in db.V_ACCF
                    on new{CNO=t.ACCNO,TP=t.ACCTP} equals new{CNO=b.CNO,TP=b.TP} into b1
                    from b in b1.DefaultIfEmpty()
                    select new { CNM = b.CNM, CNO = b.CNO, TP = b.TP, BUDGET = t.BUDGET, USEAMT = t.USEAMT, YRN = t.YRN, ACCNO = t.ACCNO, ACCTP = t.ACCTP, TAGID = t.TAGID };
        */
        String sql = "select a.CODE_NO as CNO,a.CODE_NAME as CNM,'401' as TP,ISNULL(b.YRN,'" + yrn + "') as YRN,b.ACCNO,b.ACCTP,ISNULL(b.BUDGET,0) as BUDGET,ISNULL(b.USEAMT,0) as USEAMT,b.TAGID from CONFCODE as a " +
                    "LEFT JOIN (select * from ACCBUDGET where yrn='"+yrn+"') as b "+
                    " on a.CODE_NO=b.ACCNO and '401'=b.ACCTP WHERE CODE_KIND='FRM303'";

        var query = db.Database.SqlQuery<f3031>(sql);

        /*
        if (query.Count() == 0)
        {
            query = from t in db.V_ACCF select new { CNM = t.CNM, CNO = t.CNO, TP = t.TP, BUDGET = 0, USEAMT = 0, YRN = yrn, ACCNO = "", ACCTP = "", TAGID = "" };
        }*/


        //query = query.OrderBy(t => t.ACCNO);
                                           /*
   var query = (from t in db.OTHERFEE where t.RMTAG == TAGID && t.AMTSUM != t.PAYAMT && t.RSTA == " " select new { CNO = "2", TAGID = t.TAGID, FEETP = t.FEETP, FEEYM = t.FEEYM, FEENM = t.OTHERN, FEEAMT = t.AMTSUM, PAYAMT = t.PAYAMT, LOGUSR = t.LOGUSR }).Union
                  (from t in db.RMFEEM where t.RMTAG == TAGID && t.AMTSUM != t.PAYAMT && t.RSTA == " "
                   join b in db.CONFCODE on t.FEETP equals b.CODE_NO into b1
                   from bb in b1 where bb.CODE_KIND == "WCOD"
                   select new { CNO = "1", TAGID = t.TAGID, FEETP = t.FEETP, FEEYM = t.FEEYM, FEENM = bb.CODE_NAME, FEEAMT = t.AMTSUM, PAYAMT = t.PAYAMT, LOGUSR = "" }).Union
                  (from t in db.CLUBFEE where t.RMTAG == TAGID && t.AMT != t.PAYAMT && t.RSTA == " " select new { CNO = "3",TAGID=t.TAGID,FEETP="91", FEEYM = t.FEEYM, FEENM =t.ITM,  FEEAMT = t.AMT, PAYAMT = (int)t.PAYAMT,LOGUSR=t.LOGUSR });
                                           */
//            .OrderBy(t => t.ACCTP).OrderBy(t => t.ACCNO);
 /*
SELECT * FROM ACCBUDGET a,V_ACCF b
where a.ACCNO=CNO
and a.ACCTP =b.TP
        */
        int budget = 0;
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                                                                           
                        {"CNO",item.CNO},
                        {"CNM",item.CNM },                        
                        {"TP",item.TP },                        
                        {"BUDGET",item.BUDGET},
                        {"USEAMT",item.USEAMT},
                        {"YRN",item.YRN},
                        {"TAGID",item.TAGID}
                    };
            budget += item.BUDGET;
            ja.Add(itemObject);
        }

        
        if (query.Count() > 0)
        {
            jo.Add("tot",budget);
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
     * SELECT YM,BUDGET,USEAMT,ACCNO,ACCTP,JOBTAG,TAGID FROM YMBUDGET " + ;
		"WHERE JOBTAG = ?W_JOBTAG " + ;
		"ORDER BY YM
     */

    public JObject getDG2(String JOBTAG)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        

        var query = from t in db.YMBUDGET
                    where t.JOBTAG==JOBTAG orderby t.YM 
                    select t;

        int budget = 0;
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"ACCNO",item.ACCNO},                                                
                        {"ACCTP",item.ACCTP},                        
                        {"TAGID",item.TAGID},                        
                        {"BUDGET",item.BUDGET},
                        {"USEAMT",item.USEAMT},
                        {"YM",item.YM}                        
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


    public int getYMSum(String JOBTAG)
    {

        var query = (from t in db.YMBUDGET
                     where t.JOBTAG == JOBTAG
                     group t by t.JOBTAG into g
                    select new { AMTSUM = g.Sum(t => t.BUDGET) }).SingleOrDefault();
        return query.AMTSUM;
    }


    public int getPrvYRAMT(String yrn,String acctp,String accno)
    {

        /*"SELECT * FROM ACCBUDGET " + ;
		"WHERE YRN = ?W_DT0 " + ;
		"ORDER BY ACCNO*/
        
        int ym = Convert.ToInt32(yrn) - 1;
        yrn = ym.ToString();
        /*
        var query = (from t in db.ACCBUDGET
                    where t.YRN==yrn && t.ACCNO ==accno
                    group t by t.YRN into g
                    select new {AMTSUM = g.Sum(t=> t.BUDGET)}).SingleOrDefault();
         */
        var query = (from t in db.ACCBUDGET
                     where t.YRN == yrn && t.ACCNO == accno && t.ACCTP==acctp                      
                     select new { BUDGET =  t.BUDGET });
        int budget=0;
        foreach (var item in query)
        {
            budget = item.BUDGET;
        }
        return budget;              
    }

    public int doSave(Frm303Poco param, String isSplit)
    {
        
        int i = 0;
        try
        {
            using (db)
            {
                if (StringUtils.getString(param.TAGID) != "")
                {
                    ACCBUDGET obj = (from t in db.ACCBUDGET where t.TAGID == param.TAGID select t).Single();
                    obj.BUDGET = Convert.ToInt32(param.BUDGET);
                    i = db.SaveChanges();
                }
                else
                {
                    ACCBUDGET obj = new ACCBUDGET();
                    obj.YRN = param.YRN;
                    obj.ACCNO = param.ACCNO;
                    obj.ACCTP = param.ACCTP;
                    obj.BUDGET =Convert.ToInt32(param.BUDGET);
                    //W_TAGID = RTRIM(T_BUDGET.CTP)+RTRIM(T_BUDGET.CNO)+W_YRN
                    param.TAGID = param.ACCTP + param.ACCNO + param.YRN;                    
                    obj.TAGID = param.TAGID.Trim();
                    db.ACCBUDGET.Add(obj);
                }
                if (isSplit == "Y")
                {
                    db.Database.ExecuteSqlCommand("delete from YMBUDGET where JOBTAG={0}",  StringUtils.getString(param.TAGID).Trim());

                    int budget = Convert.ToInt32(param.BUDGET) / 12;
                    int leftamt = Convert.ToInt32(param.BUDGET) - (budget * 11);
                    for (int j = 1; j <= 12; j++)
                    {
                        String ym="";
                        if(j<=9){
                            ym="0"+j;
                        }else{
                            ym=j.ToString();
                        }
                        YMBUDGET obj = new YMBUDGET();
                        obj.ACCNO = param.ACCNO;
                        obj.ACCTP = param.ACCTP;
                        obj.YM = param.YRN + ym;
                        obj.JOBTAG = param.TAGID;
                        if (j == 12)
                        {
                            obj.BUDGET = Convert.ToInt32(leftamt);
                        }
                        else
                        {
                            obj.BUDGET = Convert.ToInt32(Math.Round((decimal)Convert.ToInt32(budget)));
                        }
                        obj.TAGID = param.ACCTP + param.ACCNO + param.YRN + ym;
                        db.YMBUDGET.Add(obj);                        
                    }
                }
                db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }

    public int doSaveYM(String YMAMT,String JOBTAG,String TAGID)
    {
        int i = 0;
        try
        {

            using (db)
            {
                
                YMBUDGET obj = (from t in db.YMBUDGET where t.JOBTAG == JOBTAG && t.TAGID == TAGID select t).Single();
                obj.BUDGET =Convert.ToInt32(YMAMT);                                
                i= db.SaveChanges();

                                
                ACCBUDGET obj1 = (from t in db.ACCBUDGET where t.TAGID == JOBTAG select t).Single();
                obj1.BUDGET =getYMSum(JOBTAG);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }

    public int doAdd(Frm303Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                /*
                CTINF obj = new CTINF();
                obj.ADR = StringUtils.getString(param.ADR).Trim();
                obj.CNM = StringUtils.getString(param.CNM).Trim();
                obj.ENM = StringUtils.getString(param.ENM).Trim();
                obj.INFTP = StringUtils.getString(param.INFTP).Trim();
                obj.LOCT = StringUtils.getString(param.LOCT).Trim();
                obj.RUNTP = StringUtils.getString(param.RUNTP).Trim();
                obj.TEL = StringUtils.getString(param.TEL).Trim();
                obj.WEBSITE = StringUtils.getString(param.WEBSITE).Trim();                                
                obj.TAGID = cs.getTagidByDatetime();
                db.CTINF.Add(obj);
                i = db.SaveChanges();*/
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
            i = db.Database.ExecuteSqlCommand("delete from CTINF where TAGID={0}", StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public JArray getReport(String YY,string ACCNO,String ACCTP)
    {

        JArray ja = new JArray();
        JObject jo = new JObject();
        //DateTime dt = Convert.ToDateTime(DT);
        /*
select a.CNO,a.CNM,a.TP,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '01' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m1,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '02' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m2,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '03' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m3,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '04' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m4,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '05' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m5,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '06' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m6,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '07' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m7,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '08' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m8,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '09' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m9,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '10' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m10,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '11' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m11,
isnull((select b.BUDGET from YMBUDGET as b where b.YM='2016'  + '12' and b.ACCNO=a.CNO and b.ACCTP=a.TP ),0) as m12,
isnull(c.USEAMT,0) as USEAMT,
isnull(c.BUDGET,0) as BUDGET
from V_ACCF as a
 LEFT JOIN (select * from ACCBUDGET where yrn='2016') as c 
                     on a.CNO=c.ACCNO and a.TP=c.ACCTP*/         

String sql ="select a.CODE_NO as CNO,a.CODE_NAME as CNM,'401' as TP, "+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '01' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m1,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '02' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m2,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '03' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m3,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '04' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m4,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '05' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m5,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '06' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m6,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '07' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m7,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '08' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m8,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '09' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m9,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '10' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m10,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '11' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m11,"+
            "isnull((select b.BUDGET from YMBUDGET as b where b.YM='"+YY+"'  + '12' and b.ACCNO=a.CODE_NO and b.ACCTP='401' ),0) as m12,"+
            "isnull(c.USEAMT,0) as USEAMT,"+
            "isnull(c.BUDGET,0) as BUDGET "+
            "from CONFCODE as a "+
            "LEFT JOIN (select * from ACCBUDGET where yrn='"+YY+"') as c "+
                  "on a.CODE_NO=c.ACCNO and c.ACCTP='401' "+
                  "WHERE CODE_KIND='FRM303'";


        var query = db.Database.SqlQuery<f3032>(sql);
        foreach (var item in query)
        {

            var itemObject = new JObject
                    {   
                        {"分類代號",item.TP},
                        {"項目代號",item.CNO},                
                        {"項目名稱",item.CNM},   
                        {"一月",item.M1},
                        {"二月",item.M2},
                        {"三月",item.M3},
                        {"四月",item.M4},
                        {"五月",item.M5},
                        {"六月",item.M6},
                        {"七月",item.M7},
                        {"八月",item.M8},
                        {"九月",item.M9},
                        {"十月",item.M10},
                        {"十一月",item.M11},
                        {"十二月",item.M12},
                        {"動支合計",item.USEAMT},
                        {"預算金額",item.BUDGET}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }
   


}
}