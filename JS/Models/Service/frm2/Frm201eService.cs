﻿
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
public class Frm201eService 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getContactDG(String TAGID)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime? tmp = null;
        var query = (from t in db.CONTRAH where t.TAGID == TAGID && t.CNSTA == " " select new { CNO = "1", CNNO = t.CNHNO, NAME = "房屋", RNTP = "主約", DT1 = (t.DT1 == null ? tmp : t.DT1), DT2 = (t.DT2 == null ? tmp : t.DT2), SNO = "H", TAGID = t.TAGID, OTHERN = "",PARKNO="" }).Union
            (from t in db.CONTRAP where t.JOBTAG == TAGID && t.CNSTA == " " select new { CNO = "2", CNNO = t.CNPNO, NAME = "車位", RNTP = (t.RNTP == 1 ? "主約" : "另約"), DT1 = (t.DT1 == null ? tmp : t.DT1), DT2 = (t.DT2 == null ? tmp : t.DT2), SNO = "P", TAGID = t.TAGID, OTHERN = "",PARKNO=t.PARKNO }).Union
                  (from t in db.CONTRAF where t.JOBTAG == TAGID && t.CNSTA == " " select new { CNO = "3", CNNO = t.CNFNO, NAME = "傢俱", RNTP = (t.RNTP == 1 ? "主約" : "另約"), DT1 = (t.DT1 == null ? tmp : t.DT1), DT2 = (t.DT2 == null ? tmp : t.DT2), SNO = "F", TAGID = t.TAGID, OTHERN = "", PARKNO = "" }).Union
                  (from t in db.CONTRAC where t.JOBTAG == TAGID && t.CNSTA == " " select new { CNO = "4", CNNO = t.CNCNO, NAME = "俱樂部", RNTP = (t.RNTP == 1 ? "主約" : "另約"), DT1 = (t.DT1 == null ? tmp : t.DT1), DT2 = (t.DT2 == null ? tmp : t.DT2), SNO = "C", TAGID = t.TAGID, OTHERN = "", PARKNO = "" }).Union
                  (from t in db.CONTRAA where t.JOBTAG == TAGID && t.CNSTA == " " select new { CNO = "5", CNNO = t.CNANO, NAME = "雜項", RNTP = (t.RNTP == 1 ? "主約" : "另約"), DT1 = (t.DT1 == null ? tmp : t.DT1), DT2 = (t.DT2 == null ? tmp : t.DT2), SNO = "A", TAGID = t.TAGID, OTHERN = t.OTHERN, PARKNO = "" });
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",item.CNO},
                        {"CNNO",item.CNNO},
                        {"NAME",item.NAME},
                        {"DT1",item.DT1},
                        {"DT2",item.DT2},
                        {"SNO",item.SNO},
                        {"RNTP",item.RNTP},
                        {"TAGID",StringUtils.getString(item.TAGID)},                    
                        {"OTHERN",StringUtils.getString(item.OTHERN)},
                        {"PARKNO",item.PARKNO}
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

    public JObject getQueryDG(String TAGID,String FEETP)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();

        var query = (from t in db.RMFEEM where t.RMTAG== TAGID   select t);
        if (StringUtils.getString(FEETP) != "")
        {
            switch (FEETP)
            {
                case "1":
                    string[] strArray = new string[] { "01", "02" };
                    query = query.Where(t => strArray.Contains(t.FEETP));
                    break;
                case "2":
                   query= query.Where(t => t.FEETP == "03");
                    break;
                case "3":
                    query = query.Where(t => t.FEETP == "05");
                    break;
                case "4":
                    query = query.Where(t => t.FEETP == "04");
                    break;
                case "5":
                    query = query.Where(t => t.FEETP == "05");
                    break;
            }
        }

        query = query.OrderBy(t => t.FEEYM).OrderBy(t => t.FEETP);
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"FEEYM",item.FEEYM},
                        {"FEETP",cs.getACCNM(item.FEETP)},
                        {"FEEAMT",item.FEEAMT},
                        {"AMTTX",item.AMTTX},
                        {"AMTSUM",item.AMTSUM},
                        {"CNDAYS",item.CNDAYS},
                        {"TAGID",item.TAGID}
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


    public int doSave(Frm201ePoco param,List<Frm201ePoco> list)
    {
        
        int i = 0;
        try
        {
            DateTime? tmp = null;
/*
W_SQL = "INSERT INTO DISCNTRA " + ;
		"(CNTRATP,CNNO,JOBTP,DEALERNM,ENDDT,AMT,PAYTP,DISTP,NOTES,LOGUSR,LOGDT," + ;
		"JOBTAG,TAGID,OTHERN) " + ;
		"VALUES  " + ;
		"(?M.CNTRATP,?M.CNNO,?M.JOBTP,?M.DEALERNM,?M.ENDDT,?M.AMT,?M.PAYTP,?M.DISTP,?M.NOTES,?M.LOGUSR,?M.LOGDT," + ;
		"?M.JOBTAG,?W_TAGID,?W_OTHERN)"
*/
            


            using (db)
            {
                DISCNTRA dis = new DISCNTRA();
                dis.CNTRATP = Convert.ToInt16(param.CNTRATP);
                dis.CNNO = param.CNNO;
                dis.JOBTP = Convert.ToInt16(param.JOBTP);
                dis.DEALERNM = param.DEALERNM;
                dis.ENDDT = DateTimeUtil.getDateTime(param.ENDDT);
                dis.AMT = param.AMT;
                dis.PAYTP = Convert.ToInt16(param.PAYTP);
                dis.DISTP = param.DISTP;
                dis.NOTES = param.NOTES;
                dis.LOGUSR = param.LOGUSR;
                dis.LOGDT = System.DateTime.Now;
                dis.JOBTAG = param.TAGID;
                dis.TAGID = cs.getTagidByDatetime();
                dis.OTHERN = param.OTHERN;
                db.DISCNTRA.Add(dis);
                 
                UPDNOTE obj = new UPDNOTE();
                obj.CNO = "30";
                obj.UPDTP = StringUtils.getString(param.JOBTP) == "1" ? "到期解約" : "中途解約";
                obj.UPDDT = StringUtils.getString(param.ENDDT) == "" ? tmp : DateTimeUtil.getDateTime(param.ENDDT);
                obj.NOTES = param.NOTES;
                obj.LOGUSR = param.LOGUSR;
                obj.LOGDT = System.DateTime.Now;
                obj.JOBTAG = param.TAGID;
                obj.TAGID = cs.getTagidByDatetime();
                obj.CNNO = param.CNNO;
                obj.CNTAG = param.TAGID;
                db.UPDNOTE.Add(obj);

                i = db.SaveChanges();

                    doDISCNH(param);
                    doDISCNP(param, list);
                    doDISCNC(param, list);
                    doDISCNF(param, list);
                    doDISPARK(param, list);
                    doDISCNA(param, list);
                    doDISROOM(param);

            }
        }catch (Exception ex){

        }

        return i;
    }

    public void doDISCNH(Frm201ePoco param)
    {
        if (param.CNTRATP == "1")
        {

            Hashtable ht = getDays(param);
            String days = HashtableUtil.getValue(ht, "DAYS");
            String endym = HashtableUtil.getValue(ht, "ENDYM");
            DateTime dt = DateTimeUtil.getDateTime(param.ENDDT + DateTime.Now.ToString(" HH:mm:ss"));
            String txrat = cs.getConfig("TXRAT");
            
            using (RENTEntities db = new RENTEntities())
            {

                int j1 = db.Database.ExecuteSqlCommand("update CONTRAH set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where TAGID={3} ", dt, endym, days, param.TAGID);

                //解約當月租金處理
                int j2 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP IN ('01','02') ",
                                                       endym, dt, days, days, txrat, txrat, param.TAGID, param.CNNO, endym);
            }
            using (RENTEntities db = new RENTEntities())
            {
                //解約後各月租金處理
                int j3 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP IN ('01','02')",
                                                       endym, dt, 0, 0, param.TAGID, param.CNNO, endym);
            }
        }
    }

    public void doDISCNP(Frm201ePoco param,List<Frm201ePoco> list)
    {
        if (param.CNTRATP == "1" || param.CNTRATP == "2")
        {
            Hashtable ht = getDays(param);
            String days = HashtableUtil.getValue(ht, "DAYS");
            String endym = HashtableUtil.getValue(ht, "ENDYM");
            DateTime dt = DateTimeUtil.getDateTime(param.ENDDT + DateTime.Now.ToString(" HH:mm:ss"));
            String txrat = cs.getConfig("TXRAT");
            String feetp = "03";
            /*
    IF THISFORM.CNTRATP.VALUE = 1
        W_SQL = "UPDATE CONTRAP SET CNSTA = 'C', ENDDT = ?W_ENDDT, ENDYM = ?W_ENDYM, ENDDAYS = ?W_ENDDAYS " + ;
                "WHERE JOBTAG = ?W_JOBTAG AND RNTP = 1 "
    ELSE
        W_SQL = "UPDATE CONTRAP SET CNSTA = 'C', ENDDT = ?W_ENDDT, ENDYM = ?W_ENDYM, ENDDAYS = ?W_ENDDAYS " + ;
                "WHERE JOBTAG = ?W_JOBTAG AND TAGID = ?W_TAGID "
             */
            /*
        && 解約當月租金處理
            W_SQL = "UPDATE RMFEEM SET " + ;
                "ENDYM = ?W_ENDYM, " + ;
                "ENDDT = ?W_ENDDT, " + ;
                "CNDAYS = ?W_ENDDAYS, " + ;
                "RSTA = ' ', " + ;
                "DSDAYS = YMDAYS - ?W_ENDDAYS, " + ;
                "FEEAMT = ROUND(FEEAMT*CNDAYS/YMDAYS,0), " + ;
                "AMTTX = ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * ?TXRAT/100,0), " + ;
                "AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * ?TXRAT/100,0) " + ;
                "WHERE RMTAG = ?W_JOBTAG AND CNNO = ?W_CNNO " + ;
                "AND FEEYM = ?W_ENDYM " + ;
                "AND FEETP = ?W_FEETP "         
             */
            /*
            && 解約後各月租金處理
            W_ENDDAYS = 0
            W_SQL = "UPDATE RMFEEM SET " + ;
                "ENDYM = ?W_ENDYM, " + ;
                "ENDDT = ?W_ENDDT, " + ;
                "CNDAYS = ?W_ENDDAYS, " + ;
                "RSTA = 'C', " + ;
                "DSDAYS = YMDAYS - ?W_ENDDAYS " + ;
                "WHERE RMTAG = ?W_JOBTAG AND CNNO = ?W_CNNO " + ;
                "AND FEEYM > ?W_ENDYM " + ;
                "AND FEETP = ?W_FEETP " */
            using (RENTEntities db = new RENTEntities())
            {
                if (param.CNTRATP == "1")
                {
                    int j1 = db.Database.ExecuteSqlCommand("update CONTRAP set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and RNTP=1", dt, endym, days, param.JOBTAG);
                }
                else
                {
                    int j2 = db.Database.ExecuteSqlCommand("update CONTRAP set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and TAGID={4}", dt, endym, days, param.JOBTAG, param.TAGID);
                }
            }

            using (RENTEntities db = new RENTEntities())
            {
                if (StringUtils.getString(param.CNTRATP) == "1")
                {
                    foreach (Frm201ePoco obj in list)
                    {
                        if (StringUtils.getString(obj.SNO) == "P")
                        {
                            //解約當月租金處理
                            int j3 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP = {9} ",
                                                                   endym, dt, days, days, txrat, txrat, param.TAGID, obj.CNNO, endym, feetp);
                            //解約後各月租金處理
                            int j4 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                                   endym, dt, 0, 0, param.TAGID, obj.CNNO, endym, feetp);
                        }
                    }
                }
                else
                {

                    int j5 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP IN ('01','02') ",
                                                           endym, dt, days, days, txrat, txrat, param.TAGID, param.CNNO, endym);

                    //解約後各月租金處理
                    int j6 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                           endym, dt, 0, 0, param.TAGID, param.CNNO, endym, feetp);
                }
            }
        }
    }

    public void doDISCNC(Frm201ePoco param, List<Frm201ePoco> list)
    {
        if (param.CNTRATP == "1" || param.CNTRATP == "4")
        {
            Hashtable ht = getDays(param);
            String days = HashtableUtil.getValue(ht, "DAYS");
            String endym = HashtableUtil.getValue(ht, "ENDYM");
            DateTime dt = DateTimeUtil.getDateTime(param.ENDDT + DateTime.Now.ToString(" HH:mm:ss"));
            String txrat = cs.getConfig("TXRAT");
            String feetp = "04";

            using (RENTEntities db = new RENTEntities())
            {
                if (param.CNTRATP == "1")
                {
                    int j1 = db.Database.ExecuteSqlCommand("update CONTRAC set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and RNTP=1", dt, endym, days, param.JOBTAG);
                }
                else
                {
                    int j2 = db.Database.ExecuteSqlCommand("update CONTRAC set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and TAGID={4}", dt, endym, days, param.JOBTAG, param.TAGID);
                }
            }
            using (RENTEntities db = new RENTEntities())
            {
            if (StringUtils.getString(param.CNTRATP) == "1")
            {
                foreach (Frm201ePoco obj in list)
                {
                    if (StringUtils.getString(obj.SNO) == "C")
                    {
                        //解約當月租金處理
                        int j3 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP = {9} ",
                                                               endym, dt, days, days, txrat, txrat, param.TAGID, obj.CNNO, endym, feetp);
                        //解約後各月租金處理
                        int j4 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                               endym, dt, 0, 0, param.TAGID, obj.CNNO, endym, feetp);
                    }
                }
            }
            else
            {
                //解約當月租金處理
                int j5 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP IN ('01','02') ",
                                                       endym, dt, days, days, txrat, txrat, param.TAGID, param.CNNO, endym);

                //解約後各月租金處理
                int j6 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                       endym, dt, 0, 0, param.TAGID, param.CNNO, endym, feetp);
            }

          }
        }
    }

    public void doDISCNF(Frm201ePoco param, List<Frm201ePoco> list)
    {
        if (param.CNTRATP == "1" || param.CNTRATP == "3")
        {
            Hashtable ht = getDays(param);
            String days = HashtableUtil.getValue(ht, "DAYS");
            String endym = HashtableUtil.getValue(ht, "ENDYM");
            DateTime dt = DateTimeUtil.getDateTime(param.ENDDT + DateTime.Now.ToString(" HH:mm:ss"));
            String txrat = cs.getConfig("TXRAT");
            String feetp = "05";

            using (RENTEntities db = new RENTEntities())
            {
                if (param.CNTRATP == "1")
                {
                    int j1 = db.Database.ExecuteSqlCommand("update CONTRAF set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and RNTP=1", dt, endym, days, param.JOBTAG);
                }
                else
                {
                    int j2 = db.Database.ExecuteSqlCommand("update CONTRAF set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and TAGID={4}", dt, endym, days, param.JOBTAG, param.TAGID);
                }
            }
            using (RENTEntities db = new RENTEntities())
            {
            if (StringUtils.getString(param.CNTRATP) == "1")
            {
                foreach (Frm201ePoco obj in list)
                {
                    if (StringUtils.getString(obj.SNO) == "F")
                    {
                        //解約當月租金處理
                        int j3 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP = {9} ",
                                                               endym, dt, days, days, txrat, txrat, param.TAGID, obj.CNNO, endym, feetp);
                        //解約後各月租金處理
                        int j4 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                               endym, dt, 0, 0, param.TAGID, obj.CNNO, endym, feetp);
                    }
                }
            }
            else
            {
                //解約當月租金處理
                int j5 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP IN ('01','02') ",
                                                       endym, dt, days, days, txrat, txrat, param.TAGID, param.CNNO, endym);

                //解約後各月租金處理
                int j6 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                       endym, dt, 0, 0, param.TAGID, param.CNNO, endym, feetp);
            }
         }
        }
    }

    public void doDISCNA(Frm201ePoco param, List<Frm201ePoco> list)
    {
        if (param.CNTRATP == "1" || param.CNTRATP == "5")
        {
          Hashtable ht = getDays(param);
        String days = HashtableUtil.getValue(ht, "DAYS");
        String endym = HashtableUtil.getValue(ht, "ENDYM");
        DateTime dt = DateTimeUtil.getDateTime(param.ENDDT + DateTime.Now.ToString(" HH:mm:ss"));
        String txrat = cs.getConfig("TXRAT");
        String feetp = StringUtils.getString(param.OTHERN);

        using (RENTEntities db = new RENTEntities())
        {
            if (param.CNTRATP == "1")
            {
                int j1 = db.Database.ExecuteSqlCommand("update CONTRAA set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and RNTP=1", dt, endym, days, param.JOBTAG);
            }
            else
            {
                int j2 = db.Database.ExecuteSqlCommand("update CONTRAA set CNSTA='C',ENDDT={0},ENDYM={1},ENDDAYS={2} where JOBTAG={3} and TAGID={4}", dt, endym, days, param.JOBTAG, param.TAGID);
            }
        }

        using (RENTEntities db = new RENTEntities())
        {
            if (StringUtils.getString(param.CNTRATP) == "1")
            {
                foreach (Frm201ePoco obj in list)
                {
                    if (StringUtils.getString(obj.SNO) == "A")
                    {
                        feetp = obj.NAME;
                        //解約當月租金處理
                        int j3 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP = {9} ",
                                                               endym, dt, days, days, txrat, txrat, param.TAGID, obj.CNNO, endym, feetp);
                        //解約後各月租金處理
                        int j4 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                               endym, dt, 0, 0, param.TAGID, obj.CNNO, endym, feetp);
                    }
                }
            }
            else
            {
                //解約當月租金處理
                int j5 = db.Database.ExecuteSqlCommand(@"update RMFEEM set ENDYM={0},ENDDT={1},CNDAYS={2},RSTA=' ',DSDAYS=YMDAYS - {3},
                                               FEEAMT =ROUND(FEEAMT*CNDAYS/YMDAYS,0), 
                                               AMTTX= ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {4}/100,0),
                                               AMTSUM = ROUND(FEEAMT*CNDAYS/YMDAYS,0) + ROUND(ROUND(FEEAMT*CNDAYS/YMDAYS,0) * {5}/100,0)
                                               WHERE RMTAG = {6} AND CNNO = {7} 
                                               AND FEEYM = {8}  AND FEETP IN ('01','02') ",
                                                       endym, dt, days, days, txrat, txrat, param.TAGID, param.CNNO, endym);

                //解約後各月租金處理
                int j6 = db.Database.ExecuteSqlCommand(@"UPDATE RMFEEM SET 
                                               ENDYM = {0},
                                               ENDDT = {1},
                                               CNDAYS = {2},
                                               RSTA = 'C',
                                               DSDAYS = YMDAYS - {3}
                                               WHERE RMTAG = {4} AND CNNO = {5}
                                               AND FEEYM > {6}
                                               AND FEETP ={7}",
                                                       endym, dt, 0, 0, param.TAGID, param.CNNO, endym, feetp);
            }
        }
       }
    }



    public void doDISPARK(Frm201ePoco param,List<Frm201ePoco> list)
    {
        using (RENTEntities db = new RENTEntities())
        {
            if (param.CNTRATP == "2" && StringUtils.getString(param.PARKNO) != "")
            {
                int j1 = db.Database.ExecuteSqlCommand("UPDATE CARLOCT SET ONUSE = 0 WHERE CNO = {0}", param.PARKNO);
            }

            if (param.CNTRATP == "1")
            {
                foreach (Frm201ePoco obj in list)
                {
                    if (StringUtils.getString(obj.PARKNO) != "")
                    {
                        int j1 = db.Database.ExecuteSqlCommand("UPDATE CARLOCT SET ONUSE = 0 WHERE CNO = {0}", obj.PARKNO);
                    }
                }
            }
        }
    }


    public void doNEWCNH(Frm201jPoco param)
    {
        /*SELECT CONTRAH201J
W_RS = "CONTRAH"
W_KEYS = "TAGID"
SPTSET(W_RS,W_KEYS)
W_SQL = "INSERT INTO CONTRAH " + ;
		"(CNHNO,RMNO,CSTTP,DEALERNM,DEALMAN,COMETP,COMENOTE,MLIVER,BROKERNM,BROKMAN,BROKFEE," + ;
		"DT1,DT2,YRS,MNS,DYS,RNFEE,RNFEETP,MGFEE,MGFEETP,GUAFEE,CNHNOTE,LOGUSR,TAGID,CNSTA,LOGDT,DEALNO,BROKNO,SP)" + ;
		"VALUES  " + ;
		"(?M.CNHNO,?M.RMNO,?M.CSTTP,?M.DEALERNM,?M.DEALMAN,?M.COMETP,?M.COMENOTE,?M.MLIVER,?M.BROKERNM,?M.BROKMAN,?M.BROKFEE," + ;
		"?M.DT1,?M.DT2,?M.YRS,?M.MNS,?M.DYS,?M.RNFEE,?M.RNFEETP,?M.MGFEE,?M.MGFEETP,?M.GUAFEE,?M.CNHNOTE,?M.LOGUSR,?W_TAGID,?W_CNSTA,?M.LOGDT,?M.DEALNO,?M.BROKNO,?M.SP)"
         */
        var obj = (from t in db.CONTRAH where t.TAGID == param.TAGID select t).SingleOrDefault();
        if (obj != null)
        {            
            String contrahTagid=cs.getTagidByDatetime();
            String cnhno= cs.getCNNO(obj.RMNO, Convert.ToInt32(System.DateTime.Now.Year.ToString().Substring(3,2)));
            int j1 = db.Database.ExecuteSqlCommand(@"INSERT INTO CONTRAH
                                               (CNHNO,RMNO,CSTTP,DEALERNM,DEALMAN,COMETP,COMENOTE,MLIVER,BROKERNM,BROKMAN,BROKFEE, 
                                               DT1,DT2,YRS,MNS,DYS,RNFEE,RNFEETP,MGFEE,MGFEETP,GUAFEE,CNHNOTE,LOGUSR,TAGID,CNSTA,LOGDT,DEALNO,BROKNO,SP)
                                               VALUES
                                               ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},
                                               {11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28}) ",
                    cnhno, obj.RMNO, obj.CSTTP, obj.DEALERNM, obj.DEALMAN, obj.COMETP, obj.COMENOTE,obj.MLIVER,obj.BROKERNM,obj.BROKMAN,0,
                    param.DT11, param.DT21, obj.YRS, obj.MNS, obj.DYS, obj.RNFEETP, obj.MGFEETP, obj.GUAFEE, obj.CNHNOTE, param.LOGUSR, contrahTagid, " ", System.DateTime.Now, obj.DEALNO, obj.BROKNO, obj.SP);

            //W_SQL = "UPDATE ROOMF SET ONUSE = 1 WHERE CNO = ?M.RMNO"
            int j2 = db.Database.ExecuteSqlCommand("UPDATE ROOMF SET ONUSE = 1 WHERE CNO = {0}", obj.RMNO);
            DateTime? tmp = null;
            using (db)
            {
                UPDNOTE upd = new UPDNOTE();
                upd.CNO = "20";
                upd.UPDTP = "續約新約";
                upd.UPDDT = System.DateTime.Now;
                upd.NOTES = "舊約 "+param.CNNO;//"舊約 " + RTRIM(THISFORM.CNNO.VALUE)
                upd.LOGUSR = param.LOGUSR;
                upd.LOGDT = System.DateTime.Now;
                upd.JOBTAG = contrahTagid;
                upd.TAGID = cs.getTagidByDatetime();
                upd.CNNO = cnhno;
                upd.CNTAG = contrahTagid;
                db.UPDNOTE.Add(upd);
                db.SaveChanges();
            }

        }
    }

    public void doDISROOM(Frm201ePoco param)
    {
        using(RENTEntities db = new RENTEntities()){
        if (param.CNTRATP == "1")
        {
            int j2 = db.Database.ExecuteSqlCommand("UPDATE ROOMF SET ONUSE = 0 WHERE CNO = {0}", param.RMNO);
        }
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
        if (param.CNTRATP == "1")
        {
            //求出最後一個月的管理費
            var query1 = (from t in db.RMFEEM
                         where t.RMTAG == param.TAGID && t.CNNO == param.CNNO && t.FEETP == "02"
                         select t).OrderBy(t => t.FEEYM).FirstOrDefault();
         
        }


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
                   
              //     short days=Convert.ToSingle ( Convert.ToInt32(db.ProcGetLastDay(String.Format("{0:yyyy-MM-dd}", STime))));
 /*
	W_SQL = "INSERT INTO RMFEEM (RMNO,RMTAG,FEETP,FEEYM,FEEAMT,AMTTX,AMTSUM,LOGDT,FEEDT1,FEEDT2,YMDAYS,TAGID,CNNO,CNDAYS) " + ;
			"VALUES (?W_RMNO,?W_RMTAG,?W_FEETP,?W_FYM,?W_RNFEE1,?W_AMTTX1,?W_AMTSUM1,?W_LOGDT,?W_FDT1,?W_FDT2,?W_DAYS,?W_TAGID,?W_CNNO,?W_CNDAYS)"
	*/


            

          int rnfee = Convert.ToInt32(Math.Round(Convert.ToDouble((double)query.FEEAMT * ((double)days / (double)mmday)), 0));
            RMFEEM obj = new RMFEEM();
            
            obj.RMNO = param.RMNO;
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


            STime = STime.AddMonths(1);
        }       
    }


    //寫入管理費用
    public void doSave3(Frm201jPoco param)
    {
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

                obj.RMNO = param.RMNO;
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


                STime = STime.AddMonths(1);
            }
        }
    }


    //更新租約到期日和租約編號
    public void doSave4(Frm201jPoco param)
    { 
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


    public Hashtable getDays(Frm201ePoco param)
    {
        Hashtable ht = new Hashtable();
        String pFEETP = getFeetp(param.CNTRATP ,param.OTHERN);        

        DateTime dt = DateTimeUtil.getDateTime(param.ENDDT );
        //DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");
        //query = query.Where(q => q.ODATE >= sdt && q.ODATE <= edt);



        var query = (from t in db.RMFEEM
                     where t.RMTAG == param.JOBTAG && t.CNNO == param.CNNO && t.FEETP == pFEETP && (t.FEEDT1 <= dt && t.FEEDT2 >= dt)
                     select new { FEEYM = t.FEEYM, FEEDT1 = t.FEEDT1, FEEDT2 = t.FEEDT2, YMDAYS = t.YMDAYS }).OrderByDescending(t => t.FEEYM).DefaultIfEmpty().First();
        
        DateTime STime = DateTime.Parse(param.ENDDT); //起始日
        DateTime ETime;
        if (query != null)
        {
            ETime = query.FEEDT2.Value; //結束日
        }
        else
        {
            ETime = DateTime.Parse(param.DT2);
        }
        TimeSpan Total = ETime.Subtract(STime); //日期相減
        int tot = int.Parse(Total.TotalDays.ToString());
        if (tot <= 0)
        {
            tot = 0;
            ht.Add("ENDYM", param.ENDDT.Substring(0, 4) + param.ENDDT.Substring(5, 2));
        }
        else
        {
            tot = query.YMDAYS - tot;
            ht.Add("ENDYM", query.FEEYM);
            
        }
        
        ht.Add("DAYS", tot);
        
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
                pFEETP = "03";
                break;
            case "3":
                pFEETP = "05";
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