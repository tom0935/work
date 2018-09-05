
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
    public class ContractManagerService 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getCmDG1(String TAGID,String CNNO,String FEETP)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime? tmp = null;
        var query = from t in db.FEEFORM where t.RMTAG == TAGID && t.CNNO == CNNO && t.FEETP==FEETP orderby t.YR select t;
        
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"YR",item.YR},
                        {"FDT1",String.Format("{0:yyyy-MM-dd}", item.FDT1)},
                        {"FDT2",String.Format("{0:yyyy-MM-dd}", item.FDT2)},
                        {"DSCON1",item.DSCON1},
                        {"DSCON2",item.DSCON2},
                        {"TUNEDT",String.Format("{0:yyyy-MM-dd}", item.TUNEDT)},
                        {"TUNEUP",item.TUNEUP},
                        {"FEEAMT",item.FEEAMT},
                        {"UPY2",item.UPY2},
                        {"UPY3",item.UPY3},
                        {"UPY4",item.UPY4},
                        {"UPY5",item.UPY5}
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
            var itemObject = new JObject
                    {                                           
                        {"YR",""},
                        {"FDT1",""},
                        {"FDT2",""},
                        {"DSCON1",""},
                        {"DSCON2",""},
                        {"TUNEDT",""},
                        {"TUNEUP",""},
                        {"FEEAMT",""},
                        {"UPY2",""},
                        {"UPY3",""},
                        {"UPY4",""},
                        {"UPY5",""}
                    };
            ja.Add(itemObject);
            jo.Add("rows",ja);
            jo.Add("total", "0");
        }
        return jo;
    }

    public int doSum(ContractManagerPoco param, List<ContractManagerListPoco> list)
    {
        
  
        try
        {
            DateTime? tmp = null;
            using (db)
            {
                int j1;
                 /*
            int j1 = db.Database.ExecuteSqlCommand(@"DELETE FROM RMFEEM 
                                               WHERE RMTAG = {0} and
                                               FEETP = {1} and
                                               CNNO = {2}",
                                               param.TAGID,param.FEETP,param.CNNO);
                  */
                if (param.FEETP == "01" || param.FEETP == "02")
                {
                    j1 = db.Database.ExecuteSqlCommand(@"DELETE FROM RMFEEM 
                                               WHERE RMTAG = {0} and
                                               FEETP = {1} and
                                               CNNO = {2}", param.TAGID, param.FEETP, param.CNNO);
                }
                else
                {
                    j1 = db.Database.ExecuteSqlCommand(@"DELETE FROM RMFEEM 
                                               WHERE RMTAG = {0} and
                                               CNNO = {1}", param.TAGID, param.CNNO);
                }

                

                /*
		W_SQL = "INSERT INTO RMFEEM (RMNO,RMTAG,FEETP,FEEYM,FEEAMT,AMTTX,AMTSUM,LOGDT,FEEDT1,FEEDT2,YMDAYS,TAGID,CNNO,CNDAYS) " + ;
				"VALUES (?W_RMNO,?W_RMTAG,?W_FEETP,?W_FYM,?W_RNFEE,?W_AMTTX,?W_AMTSUM,?W_LOGDT,?W_FDT1,?W_FDT2,?W_DAYS,?W_TAGID,?W_CNNO,?W_CNDAYS)"
		SQLEXEC(CONNID,W_SQL,"RMFEEM201")*/
            List<ContractManagerListPoco> formList = new List<ContractManagerListPoco>();
                int j=0;
               foreach(ContractManagerListPoco cm in list){
                   ContractManagerListPoco cmp=new ContractManagerListPoco();
                  int? mm = 0;
                  mm = db.ProcGetMonthDiff(cm.FDT1, cm.FDT2).FirstOrDefault(); //取得月

                  DateTime STime = DateTimeUtil.getDateTime(cm.FDT1);
                  for (int i = 0; i <= mm; i++)
                  {
                      DateTime ETime = DateTimeUtil.getDateTime(cm.FDT2);    //結束日            
                      DateTime ENDTime = getLastDate(STime);
                      int x = DateTime.Compare(ETime, ENDTime);
                      int mmday = Convert.ToInt32(db.ProcGetLastDay(String.Format("{0:yyyy-MM-dd}", STime)).FirstOrDefault());
                      int days = 0;
                      if (x >= 0)
                      {
                          if (i == mm)
                          {
                              TimeSpan TotalDate = ETime.Subtract(ENDTime);      //日期相減
                              days = Convert.ToInt32(TotalDate.TotalDays.ToString());
                          }
                          else
                          {
                              days = mmday;
                          }
                      }
                      else if (x < 0)
                      {
                          //TimeSpan TotalDate = ETime.Subtract(STime);      //日期相減
                          //days = Convert.ToInt32(TotalDate.TotalDays.ToString()) + 1;
                          days = mmday;
                      }
                          
                      int rnfee = Convert.ToInt32(Math.Round(Convert.ToDouble((double)Convert.ToInt32(cm.FEEAMT)  * ((double)days / (double)mmday)), 0));
                      RMFEEM obj = new RMFEEM();

                      obj.RMNO =  param.RMNO.Trim();
                      obj.RMTAG = param.TAGID;
                      obj.FEETP = param.FEETP;
                      obj.FEEYM = String.Format("{0:yyyyMM}", STime);
                      obj.RSTA =  " ";
                    /*  if (rnfee == Convert.ToDecimal(query.FEEAMT))
                      {
                          obj.AMTTX = query.AMTTX;
                          obj.AMTSUM = query.AMTSUM;
                      }
                      else
                      {*/
                          int txrat = Convert.ToInt32(cs.getConfig("TXRAT"));
                          obj.AMTTX = Convert.ToInt32(Math.Round(Convert.ToDouble((double)(rnfee * txrat) / 100), 0));
                          obj.AMTSUM = rnfee + obj.AMTTX;
                      /*}*/
                      obj.FEEAMT = rnfee;
                      obj.LOGDT = System.DateTime.Now;
                      obj.FEEDT1 = STime;
                      obj.FEEDT2 = ENDTime;
                      obj.YMDAYS = Convert.ToInt16(mmday);
                      String dc = cs.getTagidByDatetime();

                      obj.TAGID = cs.getTagidByDatetime();
                      obj.CNNO = param.CNNO;
                      obj.CNDAYS = Convert.ToInt16(days);
                      obj.PAYAMT = 0;
                      obj.DSDAYS = 0;
                      db.RMFEEM.Add(obj);
                      
                      STime = STime.AddMonths(1);
                   /*   
                    if(i==0 || j==12 || j==24 || j==36 || j==48 || j==60 || j==72 || j==84 || j==96 || j==108){
                      cmp.YR = String.Format("{0:yyyy}", STime);
                      cmp.FEEAMT = obj.FEEAMT.ToString();
                      cmp.FDT1 = String.Format("{0:yyyy-MM-dd}", STime);
                      cmp.FDT2 = String.Format("{0:yyyy-MM-dd}", ENDTime);                      
                      cmp.DSCON2 = cm.DSCON2;
                      cmp.TUNEDT = cm.TUNEDT;
                      formList.Add(cmp);
                    }*/

                    j++;

                  }
                  db.SaveChanges();
               }
               doSaveFeeForm(param, list);
            }
            
        }catch (Exception ex){
            return 0;
        }

        return 1;
    }



    public void doSaveFeeForm(ContractManagerPoco param, List<ContractManagerListPoco> list)
    {
        using (db)
        {


            int j1 = db.Database.ExecuteSqlCommand(@"DELETE FROM FEEFORM  
                                               WHERE CNNO = {0} and
                                               RMTAG = {1} and
                                               FEETP = {2}",
                                               param.CNNO.Trim(), param.TAGID, param.FEETP);


            foreach(ContractManagerListPoco obj in list){
                DateTime? dt = null;
                if (StringUtils.getString(obj.TUNEUP) != "")
                {
                    if (obj.TUNEDT == null)
                    {
                        obj.TUNEDT = "1900-01-01";
                    }
                    dt =  DateTimeUtil.getDateTime(obj.TUNEDT);
                }
                FEEFORM feeform = new FEEFORM();
                feeform.YR = Convert.ToInt16(obj.YR);                
                feeform.CNNO = param.CNNO;
               // feeform.DSCON1 = Convert.ToInt16(obj.DSCON1);
               
                 feeform.DSCON1 = 1;
                 feeform.DSCON2 = Convert.ToInt16(obj.DSCON2);

                feeform.FDT1 = DateTimeUtil.getDateTime(obj.FDT1);
                feeform.FDT2 = DateTimeUtil.getDateTime(obj.FDT2);
                /*
                if (param.FEETP == "02")
                {
                    feeform.FEEAMT = Convert.ToInt32(obj.);
                }
                else
                {
                    feeform.FEEAMT = Convert.ToInt32(obj.FEEAMT);
                }*/
                 feeform.FEEAMT = Convert.ToInt32(obj.FEEAMT);
                feeform.FEETP = param.FEETP;
                feeform.RMNO = param.RMNO.Trim();
                feeform.RMTAG = param.TAGID;
                feeform.TAGID = cs.getTagidByDatetime();
                feeform.TUNEDT = dt;
                feeform.TUNEUP = Convert.ToDecimal(obj.TUNEUP);


                feeform.UPY2 = Convert.ToDecimal(obj.UPY2);
                feeform.UPY3 = Convert.ToDecimal(obj.UPY3);
                feeform.UPY4 = Convert.ToDecimal(obj.UPY4);
                feeform.UPY5 = Convert.ToDecimal(obj.UPY5);


                db.FEEFORM.Add(feeform);
               
            }
            db.SaveChanges();
        }
    }

  

 



  


 


 



    public void doSave2(Frm201jPoco param)
    {
        //求出最後一個月的租金
        //每期租金為每月11號到下一個月10號 (為足月月租金)
       //故最後一期租金為2015/4/11~2015/4/15 共5天租金，並以4月份天數為分母，算出日租金

        String pFEETP = getFeetp(param.CNTRATP, param.OTHERN);       
        var query = (from t in db.RMFEEM
                     where t.RMTAG == param.TAGID && t.CNNO == param.CNNO && t.FEETP == pFEETP 
                     select t).OrderBy(t => t.FEEYM).FirstOrDefault();



        int? mm = 0;
        
        //int? pp = db.ProcGetLastDay(param.DT11).FirstOrDefault();
         mm = db.ProcGetMonthDiff(param.DT11, param.DT21).FirstOrDefault(); //取得月

         DateTime STime = DateTimeUtil.getDateTime(param.DT11);
        for (int i = 1; i <= mm ; i++)
        {           
            DateTime ETime = DateTimeUtil.getDateTime(param.DT21);    //結束日            
            DateTime ENDTime =getLastDate(STime);
            int x = DateTime.Compare(ETime, ENDTime);
            int mmday = Convert.ToInt32(db.ProcGetLastDay(String.Format("{0:yyyy-MM-dd}", STime)).FirstOrDefault());
                int days=0;                   
                   if (x > 0 )
                    {
                        if (i == mm)
                        {
                            TimeSpan TotalDate = ETime.Subtract(ENDTime);      //日期相減
                            days = Convert.ToInt32(TotalDate.TotalDays.ToString());
                        }
                        else
                        {
                            days = mmday;
                        }
                    }
                    else if (x < 0)
                    {                       
                        TimeSpan TotalDate = ETime.Subtract(STime);      //日期相減
                        days = Convert.ToInt32(TotalDate.TotalDays.ToString()) + 1;
                    }

          int rnfee = Convert.ToInt32(Math.Round(Convert.ToDouble((double)query.FEEAMT * ((double)days / (double)mmday)), 0));
            RMFEEM obj = new RMFEEM();
            
            obj.RMNO = param.RMNO.Trim();
            obj.RMTAG = param.TAGID;
            obj.FEETP = getFeetp(param.CNTRATP, param.OTHERN); 
            obj.FEEYM = String.Format("{0:yyyyMM}", STime);
            
            if (rnfee == Convert.ToDecimal(query.FEEAMT))
            {
                obj.AMTTX = query.AMTTX;
                obj.AMTSUM = query.AMTSUM;
            }
            else
            {
                int txrat =Convert.ToInt32(cs.getConfig("TXRAT"));
                obj.AMTTX = Convert.ToInt32(Math.Round(Convert.ToDouble((double)(rnfee * txrat) / 100), 0));
                obj.AMTSUM = rnfee + obj.AMTTX;
            }
            obj.FEEAMT = rnfee;
            obj.LOGDT = System.DateTime.Now;
            obj.FEEDT1 = STime;
            obj.FEEDT2 = ENDTime;
            obj.YMDAYS = Convert.ToInt16(mmday);
            obj.TAGID = cs.getTagidByDatetime();            
            obj.CNNO = param.CNNO;
            obj.CNDAYS = Convert.ToInt16(days);

            db.RMFEEM.Add(obj);
            db.SaveChanges();
            STime = STime.AddMonths(1);
        }       
    }


    //寫入管理費用
    public void doSave3(Frm201jPoco param)
    {
        //求出最後一個月的管理費
        //房屋租約才需寫入管理費
        if (param.CNTRATP == "1")
        {

            //求出最後一個月的管理費
            var query = (from t in db.RMFEEM
                          where t.RMTAG == param.TAGID && t.CNNO == param.CNNO && t.FEETP == "02"
                          select t).OrderBy(t => t.FEEYM).FirstOrDefault();




            int? mm = 0;

            //int? pp = db.ProcGetLastDay(param.DT11).FirstOrDefault();
            mm = db.ProcGetMonthDiff(param.DT11, param.DT21).FirstOrDefault(); //取得月

            DateTime STime = DateTimeUtil.getDateTime(param.DT11);
            for (int i = 1; i <= mm; i++)
            {
                DateTime ETime = DateTimeUtil.getDateTime(param.DT21);    //結束日            
                DateTime ENDTime = getLastDate(STime);
                int x = DateTime.Compare(ETime, ENDTime);
                int mmday = Convert.ToInt32(db.ProcGetLastDay(String.Format("{0:yyyy-MM-dd}", STime)).FirstOrDefault());
                int days = 0;
                if (x > 0)
                {
                    if (i == mm)
                    {
                        TimeSpan TotalDate = ETime.Subtract(ENDTime);      //日期相減
                        days = Convert.ToInt32(TotalDate.TotalDays.ToString());
                    }
                    else
                    {
                        days = mmday;
                    }
                }
                else if (x < 0)
                {
                    TimeSpan TotalDate = ETime.Subtract(STime);      //日期相減
                    days = Convert.ToInt32(TotalDate.TotalDays.ToString()) + 1;
                }

                //     short days=Convert.ToSingle ( Convert.ToInt32(db.ProcGetLastDay(String.Format("{0:yyyy-MM-dd}", STime))));
                /*
                   W_SQL = "INSERT INTO RMFEEM (RMNO,RMTAG,FEETP,FEEYM,FEEAMT,AMTTX,AMTSUM,LOGDT,FEEDT1,FEEDT2,YMDAYS,TAGID,CNNO,CNDAYS) " + ;
                           "VALUES (?W_RMNO,?W_RMTAG,?W_FEETP,?W_FYM,?W_RNFEE1,?W_AMTTX1,?W_AMTSUM1,?W_LOGDT,?W_FDT1,?W_FDT2,?W_DAYS,?W_TAGID,?W_CNNO,?W_CNDAYS)"
                   */


                int rnfee = Convert.ToInt32(Math.Round(Convert.ToDouble((double)query.FEEAMT * ((double)days / (double)mmday)), 0));
                RMFEEM obj = new RMFEEM();

                obj.RMNO = param.RMNO.Trim();
                obj.RMTAG = param.TAGID;
                obj.FEETP = "02";
                obj.FEEYM = String.Format("{0:yyyyMM}", STime);

                if (rnfee == Convert.ToDecimal(query.FEEAMT))
                {
                    obj.AMTTX = query.AMTTX;
                    obj.AMTSUM = query.AMTSUM;
                }
                else
                {
                    int txrat = Convert.ToInt32(cs.getConfig("TXRAT"));
                    obj.AMTTX = Convert.ToInt32(Math.Round(Convert.ToDouble((double)(rnfee * txrat) / 100), 0));
                    obj.AMTSUM = rnfee + obj.AMTTX;
                }
                obj.FEEAMT = rnfee;
                obj.LOGDT = System.DateTime.Now;
                obj.FEEDT1 = STime;
                obj.FEEDT2 = ENDTime;
                obj.YMDAYS = Convert.ToInt16(mmday);
                obj.TAGID = cs.getTagidByDatetime();
                obj.CNNO = param.CNNO;
                obj.CNDAYS = Convert.ToInt16(days);

                db.RMFEEM.Add(obj);
                db.SaveChanges();
                STime = STime.AddMonths(1);
            }
        }
    }


    //更新租約到期日和租約編號
    public void doSave4(Frm201jPoco param)
    { /*
W_RMTAG = THISFORM.TAGID.VALUE
W_CNNO = T_CONTRA.CNNO
W_ENDDT = THISFORM.DT21.VALUE
DO CASE
	CASE THISFORM.CNTRATP.VALUE = 1  && 房屋
			W_SQL = "UPDATE CONTRAH SET DT2 = ?W_ENDDT WHERE TAGID = ?W_RMTAG AND CNHNO = ?W_CNNO "
	CASE THISFORM.CNTRATP.VALUE = 2  && 車位
			W_SQL = "UPDATE CONTRAP SET DT2 = ?W_ENDDT WHERE RMTAG = ?W_RMTAG AND CNPNO = ?W_CNNO "
	CASE THISFORM.CNTRATP.VALUE = 3  && 傢俱
			W_SQL = "UPDATE CONTRAF SET DT2 = ?W_ENDDT WHERE RMTAG = ?W_RMTAG AND CNFNO = ?W_CNNO "
	CASE THISFORM.CNTRATP.VALUE = 4  && 俱樂部
			W_SQL = "UPDATE CONTRAC SET DT2 = ?W_ENDDT WHERE RMTAG = ?W_RMTAG AND CNCNO = ?W_CNNO "
	CASE THISFORM.CNTRATP.VALUE = 5  && 雜項
			W_SQL = "UPDATE CONTRAA SET DT2 = ?W_ENDDT WHERE RMTAG = ?W_RMTAG AND CNANO = ?W_CNNO "        
       * */
        switch (param.CNTRATP)
        {
            case "1":
                db.Database.ExecuteSqlCommand("UPDATE CONTRAH SET DT2 = {0} WHERE TAGID = {1} AND CNHNO = {2}", param.DT21, param.JOBTAG, param.CNNO);
                break;
            case "2":
                db.Database.ExecuteSqlCommand("UPDATE CONTRAP SET DT2 = {0} WHERE RMTAG = {1} AND CNPNO = {2}", param.DT21, param.JOBTAG, param.CNNO);
                break;
            case "3":
                db.Database.ExecuteSqlCommand("UPDATE CONTRAF SET DT2 = {0} WHERE RMTAG = {1} AND CNFNO = {2}", param.DT21, param.JOBTAG, param.CNNO);
                break;
            case "4":
                db.Database.ExecuteSqlCommand("UPDATE CONTRAC SET DT2 = {0} WHERE RMTAG = {1} AND CNCNO = {2}", param.DT21, param.JOBTAG, param.CNNO);
                break;
            case "5":
                db.Database.ExecuteSqlCommand("UPDATE CONTRAA SET DT2 = {0} WHERE RMTAG = {1} AND CNANO = {2}", param.DT21, param.JOBTAG, param.CNNO);
                break;
        }

    }

    public DateTime getLastDate(DateTime dt)
    {
        int rmm =dt.Month;
        int? rdd =dt.Day;

        DateTime dt2 = dt.AddMonths(1);
                 dt2 = dt2.AddDays(-1);
        String strDt2 = String.Format("{0:yyyy-MM-dd}", dt2);
        rdd = db.ProcGetLastDay(strDt2).FirstOrDefault();

        if (dt.Month == 2 && dt.Day > 1 && dt.Day < 28)
        {
           // rdd = rdd - 1;
            rdd = dt2.Day;
        }
        else if (dt.Month == 2 && dt.Day >= 28)
        {
            if (dt.Day == 28)
            {
                rdd = 29;
            }
            else if (dt.Day == 29)
            {
                rdd = 30;
            }
            
        }
        else if (dt.Month == 1 && (dt.Day == 28 || dt.Day == 29))
        {
            rdd = 27;
        }
        else if ((dt.Month == 1 && dt.Day == 30) && (dt2.Month == 2 || dt2.Day == 31))
        {
            rdd= dt2.Day;
        }
        else if (dt.Month != 2 && dt.Day == 31)
        {            
            rdd = rdd - 1;
        }
        else
        {
            rdd = dt2.Day;
        }

        int dd = Convert.ToInt32(rdd);
        DateTime dym = new DateTime(dt2.Year, dt2.Month, dd);
        return dym;
    }


    public Hashtable getDays(Frm201jPoco param)
    {
        Hashtable ht = new Hashtable();
        String pFEETP = getFeetp(param.CNTRATP ,param.OTHERN);        

        DateTime dt = DateTimeUtil.getDateTime(param.DT20 );
        //DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");
        //query = query.Where(q => q.ODATE >= sdt && q.ODATE <= edt);
        var query = (from t in db.RMFEEM
                    where t.RMTAG == param.TAGID && t.CNNO == param.CNNO && t.FEETP == pFEETP && (t.FEEDT1 <= dt && t.FEEDT2 >= dt)
                     select new { FEEYM = t.FEEYM, FEEDT1 = t.FEEDT1, FEEDT2 = t.FEEDT2, YMDAYS = t.YMDAYS }).OrderByDescending(t => t.FEEYM).SingleOrDefault();
        
        DateTime STime = DateTime.Parse(param.DT20); //起始日
        DateTime ETime = query.FEEDT2.Value; //結束日
        TimeSpan Total = ETime.Subtract(STime); //日期相減
        int tot = int.Parse(Total.TotalDays.ToString());
        if (tot < 0)
        {
            tot = 0;
        }
        tot = query.YMDAYS - tot;
        ht.Add("DAYS", tot);
        ht.Add("ENDYM", query.FEEYM);
        return ht;
    }

    public String getFeetp(String feetp,String othern)
    {
        String pFEETP="";
        switch (feetp)
        {
            case "1":
                pFEETP = "01";
                break;
            case "2":
                pFEETP = "02";
                break;
            case "3":
                pFEETP = "03";
                break;
            case "4":
                pFEETP = "04";
                break;
            case "5":
                pFEETP = getACCNO(othern);
                break;
        }
        return pFEETP;
    }


    public String getACCNO(String code_name){
        var query = (from t in db.CONFCODE where t.CODE_KIND == "WCOD" && t.CODE_NAME == code_name select t).Single();
        String cno="";
        if (query != null)
        {
            cno = query.CODE_NO;
        }
        return cno;
    }


}
}