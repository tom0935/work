
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

namespace Jasper.service.frm4{
public class Frm401Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDG1(String TAGID)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        Boolean qmode = false;
        /*
W_SQL = "SELECT CNM,SEX,BIRTH,EMAIL,CIDT,TPNM,CNTRY,BLODTP,LOGUSR,LOGDT, " + ;
		"CODT,PID,POSL,MARRI,RSTA,JOBTAG,TAGID,CNHNO,CNCNO,TPNO,MFEE,NOTES FROM MBRM " + ;
		"WHERE JOBTAG = ?W_JOBTAG AND RSTA = ' ' " + ;
		"ORDER BY CIDT"
        */

        var query = from t in db.MBRM where t.RSTA==" " select t;
            if (StringUtils.getString(TAGID) != "")
            {
                query = query.Where(t => t.JOBTAG==TAGID);
                qmode = true;
            }


        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"BIRTH",String.Format("{0:yyyy-MM-dd}" , item.BIRTH)},                        
                        {"BLODTP",item.BLODTP},
                        {"CIDT",String.Format("{0:yyyy-MM-dd}" , item.CIDT)},
                        {"CNCNO",item.CNCNO},
                        {"CNHNO",item.CNHNO},
                        {"CNM",item.CNM},
                        {"CNTRY",item.CNTRY},
                        {"CODT",String.Format("{0:yyyy-MM-dd}" , item.CODT)},
                        {"EMAIL",item.EMAIL},
                        {"JOBTAG",item.JOBTAG},
                        {"MARRI",item.MARRI},
                        {"MFEE",item.MFEE},
                        {"NOTES",item.NOTES},
                        {"PID",item.PID},
                        {"POSL",item.POSL},
                        {"RMNO",item.RMNO},
                        {"SEX",item.SEX},
                        {"TPNM",item.TPNM},
                        {"TPNO",item.TPNO},
                        {"LOGUSR",item.LOGUSR},
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




  


    public int doSave(Frm401Poco param)
    {
        
        int i = 0;
        try
        {
                        
            using (db)
            {
                MBRM obj = (from t in db.MBRM where t.TAGID == param.TAGID select t).Single();
                obj.CNM = StringUtils.getString(param.CNM).Trim();
                obj.CNTRY = StringUtils.getString(param.CNTRY).Trim();
                obj.SEX = StringUtils.getString(param.SEX).Trim();
                obj.BLODTP = StringUtils.getString(param.BLODTP).Trim();
                obj.BIRTH =  DateTimeUtil.getDateTime(param.BIRTH.Trim());
                obj.POSL = StringUtils.getString(param.POSL).Trim();
                obj.EMAIL = StringUtils.getString(param.EMAIL).Trim();
                obj.CIDT = DateTimeUtil.getDateTime(param.CIDT.Trim());
                obj.CODT = DateTimeUtil.getDateTime(param.CODT.Trim());
                obj.PID = StringUtils.getString(param.PID).Trim();
                obj.MARRI = Convert.ToInt32(param.MARRI);
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.TPNM = StringUtils.getString(param.TPNM).Trim();
                obj.TPNO = StringUtils.getString(param.TPNO).Trim();
                obj.MFEE = Convert.ToInt32(param.MFEE.Trim());
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.JOBTAG = StringUtils.getString(param.JOBTAG).Trim();  
                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm401Poco param)
    {

        int i = 0;
        try
        {
            int j1 = db.Database.ExecuteSqlCommand(@"DELETE FROM RMFEEM 
                                               WHERE RMTAG = {0} and
                                               FEETP = '04' and
                                               CNNO = {1}",
                                               param.JOBTAG, param.CNCNO);            
            
            using (db)
            {
                MBRM obj = new MBRM();
                obj.CNM = StringUtils.getString(param.CNM).Trim();
                obj.CNTRY = StringUtils.getString(param.CNTRY).Trim();
                obj.SEX = StringUtils.getString(param.SEX).Trim();
                obj.BLODTP = StringUtils.getString(param.BLODTP).Trim();
                obj.BIRTH = DateTimeUtil.getDateTime(param.BIRTH.Trim());
                obj.POSL = StringUtils.getString(param.POSL).Trim();
                obj.EMAIL = StringUtils.getString(param.EMAIL).Trim();
                obj.CIDT = DateTimeUtil.getDateTime(param.CIDT.Trim());
                obj.CODT = DateTimeUtil.getDateTime(param.CODT.Trim());
                obj.PID = StringUtils.getString(param.PID).Trim();
                obj.MARRI = Convert.ToInt32(param.MARRI);
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.TPNM = StringUtils.getString(param.TPNM).Trim();
                obj.TPNO = StringUtils.getString(param.TPNO).Trim();
                obj.MFEE = Convert.ToInt32(param.MFEE.Trim());
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.JOBTAG = StringUtils.getString(param.JOBTAG).Trim();
                obj.RSTA = " ";
                obj.TAGID = cs.getTagidByDatetime();
                db.MBRM.Add(obj);


                i = db.SaveChanges();
                
            }

        }
        catch (Exception ex)
        {

        }

        return i;
    }




    public void doAddCONTRAC(Frm401Poco param)
    {
        using (RENTEntities db = new RENTEntities())
     {
            var query = from t in db.CONTRAC where t.JOBTAG == param.JOBTAG select t;
            CONTRAC obj = new CONTRAC();
            if (query.Count() > 0)
            {
                foreach (var item in query)
                {
                    obj.DEALERNM = item.DEALERNM;
                    obj.DEALNO = item.DEALNO;
                }              
            }

            
            obj.RNTP = 2;
            obj.RNTERTP = 1;
            
            obj.DT1 = DateTimeUtil.getDateTime(param.CIDT);
            obj.DT2 = DateTimeUtil.getDateTime(param.CODT);
            obj.YRS = 0;
            obj.MNS = 0;
            obj.DYS = 0;
            obj.RNFEE = Convert.ToInt32(param.MFEE);
            obj.RNFEETP = 2;
            obj.NOTES = param.NOTES;
            obj.LOGUSR = param.LOGUSR;
            obj.CNCNO = param.CNCNO;
            obj.CNSTA = " ";
            obj.TAGID = cs.getTagidByDatetime();
            db.CONTRAC.Add(obj);
            db.SaveChanges();

            

        }
    }
    //計算租金
    public void doRunFEE(Frm401Poco param)
    {
        try
        {

            int? mm = 0;
            RENTEntities db2 = new RENTEntities();
            mm = db2.ProcGetMonthDiff(param.CIDT, param.CODT).FirstOrDefault(); //取得月

            using (RENTEntities db = new RENTEntities())
            {





                    DateTime STime = DateTimeUtil.getDateTime(param.CODT);
                    for (int i = 0; i <= mm; i++)
                    {
                        DateTime ETime = DateTimeUtil.getDateTime(param.CODT);    //結束日            
                        DateTime ENDTime = cs.getLastDate(STime);
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

                        int rnfee = Convert.ToInt32(Math.Round(Convert.ToDouble((double)Convert.ToInt32(param.MFEE) * ((double)days / (double)mmday)), 0));
                        RMFEEM obj = new RMFEEM();

                        obj.RMNO = param.RMNO;
                        obj.RMTAG = param.JOBTAG;
                        obj.FEETP = "04";
                        obj.FEEYM = String.Format("{0:yyyyMM}", STime);
                        obj.RSTA = " ";
                        
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
                        obj.TAGID = cs.getTagidByDatetime();
                        obj.CNNO = param.CNCNO;
                        obj.CNDAYS = Convert.ToInt16(days);
                        obj.PAYAMT = 0;
                        obj.DSDAYS = 0;
                        db.RMFEEM.Add(obj);
                        db.SaveChanges();
                        STime = STime.AddMonths(1);

                    }


            }
        }
        catch (Exception ex)
        {
          
        }        
    }


    public int doRemove(String TAGID)
    {

        int i = 0;
        try
        {
            i = db.Database.ExecuteSqlCommand("update MBRM set RSTA='C',CODT={0} where TAGID={1}", System.DateTime.Now,StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}