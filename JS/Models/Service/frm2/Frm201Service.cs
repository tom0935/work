
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
/* 
廠商編號 CNO 
廠商名稱 CNM
SDEP
行業別 RUNTP
CPID
CPMGR
CPADR
TEL
FAX
EMAIL
 */
namespace Jasper.service.frm2{
public class Frm201Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();
    private Frm201jService f201jService = new Frm201jService();
  //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
    public JObject getDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        var query = db.MAKR.AsQueryable();
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)},
                        {"CPADR",StringUtils.getString(item.CPADR)},
                        {"RUNTP",StringUtils.getString(item.RUNTP)},
                        {"CPID",StringUtils.getString(item.CPID)},
                        {"CPMGR",StringUtils.getString(item.CPMGR)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)},                        
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"FAX",StringUtils.getString(item.FAX)},                        
                        {"SDEP",StringUtils.getString(item.SDEP)}
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

    public JArray getDatagrid2(String cno)
    {
        JArray ja = new JArray();
        var query = (from t in db.MAKRMAN select t);
        if (cno != null)
        {
            query = query.Where(q => q.MAKRNO == cno);
        }
        foreach (var item in query.ToList())
        {
            var itemObject = new JObject
                    {                           
                        {"MAN",StringUtils.getString(item.MAN)},
                        {"TEL",StringUtils.getString(item.TEL)},
                        {"EMAIL",StringUtils.getString(item.EMAIL)},                        
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"MAKRNO",StringUtils.getString(item.MAKRNO)},
                        {"NOTES",StringUtils.getString(item.NOTES)}
                    };
            ja.Add(itemObject);
        }
        return ja;
    }




   public int getTotalCount()
   {
       return db.DEALER.Count();            
   }

   public int doSaveYRCNT(String TYPE,String RMNO,int YR,String CNHNO)
   {
       
       using (db)
       {
           if(TYPE=="1"){
                   YRHCNT obj = (from t in db.YRHCNT where t.RMNO == RMNO && t.YR == YR select t).SingleOrDefault();
                   if (obj != null)
                   {
                       obj.CNCNT = obj.CNCNT + 1;
                   }
                   else
                   {
                       YRHCNT o = new YRHCNT();
                       o.CNCNT = 1;
                       o.RMNO = RMNO;
                       o.YR = YR;
                       db.YRHCNT.Add(o);
                   }                
           }else{
               String CN="";
               switch (TYPE)
               {
                   case "2":
                       CN = "P";
                       break;
                   case "3":
                       CN = "F";
                       break;
                   case "4":
                       CN = "C";
                       break;
                   case "5":
                       CN = "A";
                       break;
               }
               YRCNT obj = (from t in db.YRCNT where t.CNHNO == CNHNO && t.CN == CN select t).SingleOrDefault();
               if (obj != null)
               {
                   obj.CNCNT = obj.CNCNT + 1;                   
               }
               else
               {
                   YRCNT o = new YRCNT();
                   o.CNHNO = CNHNO;
                   o.CNCNT = 1;
                   o.CN = CN;
                   db.YRCNT.Add(o);
               }

           }
          return db.SaveChanges();
       }
   }

   public int doSave(String mode, Frm201Poco param) 
   {       
       int i = 0;
       DateTime? tmp=null;
       int brokfee;
       using(db){
           String strType="";
           String strCNNO ="";
           switch (param.type)
           {
               case "1":
                   strType = "01";
                   strCNNO = param.CNNO;
                   break;
               case "2":
                   strType = "03";
                   strCNNO = param.CNNO;
                   break;
               case "3":
                   strType = "05";
                   strCNNO = param.CNNO;
                   break;
               case "4":
                   strType = "04";
                   strCNNO = param.CNNO;
                   break;
               case "5":
                   strType = f201jService.getACCNO(param.OTHERN);
                   strCNNO = param.CNNO;
                   break;
           }

           if (strCNNO != null)
           {
               DateTime STime = DateTimeUtil.getDateTime(param.DT1);
               DateTime ETime = DateTimeUtil.getDateTime(param.DT2);    //結束日   
               if(Convert.ToUInt32(param.MNS) > 0 || Convert.ToUInt32(param.DYS) > 0){
                   param.YRS = Convert.ToString(Convert.ToUInt32(param.YRS) + 1);
               }

               db.Database.ExecuteSqlCommand("DELETE FEEFORM WHERE FEETP={0} and CNNO = {1}",  strType, strCNNO);
               DateTime SDT = DateTimeUtil.getDateTime(param.DT1);
               DateTime EDT = DateTimeUtil.getDateTime(param.DT2);

               for (int x = 0; x < Convert.ToUInt32(param.YRS); x++)
               {
                   FEEFORM f = new FEEFORM();
                   f.CNNO = strCNNO;
                   f.DSCON1 = 0;
                   f.DSCON2 = 0;
                   f.FEEAMT = Convert.ToInt32(param.RNFEE);
                   f.FEETP = strType;
                   f.RMNO = param.mHOUSE;
                   f.RMTAG = param.mJOBTAG;
                   f.TAGID = cs.getTagidByDatetime();                   
                   f.FDT1 = SDT;
                   f.YR = Convert.ToInt16(String.Format("{0:yyyy}", SDT));
                   if(Convert.ToUInt32(param.YRS)==0){
                       f.FDT2 = EDT;
                   }
                   else if (Convert.ToUInt32(param.YRS) ==x)
                   {
                       f.FDT2 = DateTimeUtil.getDateTime(param.DT2);
                   }
                   else
                   {
                     SDT = SDT.AddYears(1);
                     DateTime dty = SDT.AddDays(-1);
                     f.FDT2 = dty;
                   }
                   
                   db.FEEFORM.Add(f);
               }

               //    db.Database.ExecuteSqlCommand("UPDATE FEEFORM SET FDT1 ={0},FDT2={1},FEEAMT={2} WHERE FEETP={3} and CNNO = {4}", STime, ETime, param.RNFEE, strType, strCNNO);

               if (param.type == "1")
               {
                   db.Database.ExecuteSqlCommand("DELETE FEEFORM WHERE FEETP={0} and CNNO = {1}", "02", strCNNO);
                   

                   DateTime SDT2 = DateTimeUtil.getDateTime(param.DT1);
                   DateTime EDT2 = DateTimeUtil.getDateTime(param.DT2);
                   for (int x = 0; x < Convert.ToUInt32(param.YRS); x++)
                   {
                       FEEFORM f = new FEEFORM();
                       f.CNNO = strCNNO;


                       f.FEEAMT = Convert.ToInt32(param.MGFEE);
                       f.FEETP = "02";
                       f.RMNO = param.mHOUSE;
                       f.RMTAG = param.mJOBTAG;
                       f.TAGID = cs.getTagidByDatetime();
                       f.FDT1 = SDT2;
                       f.YR = Convert.ToInt16(String.Format("{0:yyyy}", SDT));
                       if (Convert.ToUInt32(param.YRS) == 0)
                       {
                           f.FDT2 = EDT2;
                       }
                       else if (Convert.ToUInt32(param.YRS) == x)
                       {
                           f.FDT2 = DateTimeUtil.getDateTime(param.DT2);
                       }
                       else
                       {
                           SDT2 = SDT2.AddYears(1);
                           DateTime dty = SDT2.AddDays(-1);
                           f.FDT2 = dty;
                       }
                       db.FEEFORM.Add(f);
                   }
               }   
           }


           CONTRAH obj1;
           if (param.mJOBTAG != null)
           {
               obj1 = (from t in db.CONTRAH where t.TAGID == param.mJOBTAG select t).SingleOrDefault();
               
               obj1.CNHNO = obj1.CNHNO.Trim();

               obj1.LOGUSR = param.mLOGUSR.Trim();
           }
           else
           {
                obj1 = new CONTRAH();
                obj1.TAGID = cs.getTagidByDatetime();
                obj1.CNHNO = cs.getCNNO(StringUtils.getString(param.mHOUSE).Trim(), System.DateTime.Now.Year);
                obj1.RMNO = StringUtils.getString(param.mHOUSE).Trim();
                obj1.CNSTA = " ";
                db.Database.ExecuteSqlCommand("UPDATE ROOMF SET ONUSE = 1 WHERE CNO = {0}", StringUtils.getString(param.mHOUSE).Trim());
           }

           obj1.BROKERNM = param.mBROKERNM;
           if(StringUtils.getString(param.mBROKFEE)==""){
               brokfee = 0;
           }else{
               brokfee = Convert.ToInt32(param.mBROKFEE);
           }
           obj1.BROKFEE = brokfee;
           obj1.BROKMAN = StringUtils.getString(param.mBROKMAN).Trim();
           obj1.COMENOTE =StringUtils.getString(param.mCOMENOTE).Trim();
           obj1.COMETP = StringUtils.getString(param.mCOMETP).Trim();
           obj1.CSTTP = StringUtils.getShort(param.mCSTTP);
           obj1.DEALERNM = StringUtils.getString(param.mDEALERNM).Trim();
           obj1.DEALMAN = StringUtils.getString(param.mDEALMAN).Trim();
           obj1.DEALNO = StringUtils.getString(param.mDEALNO).Trim();
           obj1.MLIVER = StringUtils.getString(param.mMLIVER).Trim();

           

      switch( param.type){
          case "1":              
              obj1.CNHNOTE = param.NOTES;
              obj1.DT1 = DateTimeUtil.getDateTime(param.DT1);
              obj1.DT2 = DateTimeUtil.getDateTime(param.DT2);
              obj1.DSCON1 = StringUtils.getDecimal(param.DSCON1);
              obj1.DSCON2 = StringUtils.getDecimal(param.DSCON2);
              obj1.YRS =StringUtils.getShort(param.YRS);
              obj1.MNS = StringUtils.getShort(param.MNS);
              obj1.DYS = StringUtils.getShort(param.DYS);
              obj1.RNFEETP = StringUtils.getShort(param.RNFEETP);
              obj1.RNFEE = StringUtils.getInt(param.RNFEE);              
              obj1.MGFEETP = StringUtils.getShort(param.MGFEETP);
              obj1.MGFEE = StringUtils.getInt(param.MGFEE);
              obj1.GUAFEE = StringUtils.getInt(param.GUAFEE);
              obj1.ENDDAYS = System.Convert.ToInt16(param.DYS);
              //obj1.GUAFEE = System.Convert.ToInt32(param.GUAFEE);
              obj1.SP = StringUtils.getShort(param.SP);
              if (param.TAGID == null)
              {
                  db.CONTRAH.Add(obj1);
              }             
              break;
          case "2":
              CONTRAP obj2;               
               if (param.TAGID != null)
               {
                   obj2 = (from t in db.CONTRAP where t.TAGID == param.TAGID select t).SingleOrDefault();
               }
               else
               {
                   if (param.mJOBTAG != null)
                   {
                       obj2= new CONTRAP();
                       obj2.TAGID = cs.getTagidByDatetime();
                       obj2.CNSTA = " ";                       
                       obj2.JOBTAG = StringUtils.getString(param.mJOBTAG).Trim();
                       obj2.CNPNO = cs.getCNNO1(param.CNHNO, "P");
                   }
                   else
                   {
                       break;
                   }
               }

              obj2.NOTES = StringUtils.getString(param.NOTES).Trim();
              obj2.PARKNO = StringUtils.getString(param.PARKNO).Trim();
              obj2.LOCT = StringUtils.getString(param.LOCT).Trim();
              obj2.DT1 = StringUtils.getString(param.DT1) == "" ? tmp : DateTimeUtil.getDateTime(param.DT1);
              obj2.DT2 = StringUtils.getString(param.DT2) == "" ? tmp : DateTimeUtil.getDateTime(param.DT2);
              obj2.DSCON1 = StringUtils.getDecimal(param.DSCON1);
              obj2.DSCON2 = StringUtils.getDecimal(param.DSCON2);
              obj2.YRS =StringUtils.getShort(param.YRS);
              obj2.MNS = StringUtils.getShort(param.MNS);
              obj2.DYS = StringUtils.getShort(param.DYS);
              obj2.RNFEE = StringUtils.getInt(param.RNFEE);
              obj2.RNTP = StringUtils.getShort(param.RNTP);
              obj2.RNTERTP = StringUtils.getShort(param.RNTERTP);
              obj2.DEALERNM = StringUtils.getString(param.DEALERNM).Trim();
              if (param.TAGID == null && param.mJOBTAG != null)
              {
                  db.CONTRAP.Add(obj2);
              }
              break;
          case "3":
              CONTRAF obj3;                
                if (param.TAGID != null)
                {
                    obj3 = (from t in db.CONTRAF where t.TAGID == param.TAGID select t).SingleOrDefault();
                }
                else
                {
                    if (param.mJOBTAG != null)
                    {
                        obj3 = new CONTRAF();
                        obj3.TAGID = cs.getTagidByDatetime();
                        obj3.CNSTA = " ";
                        obj3.JOBTAG = StringUtils.getString(param.mJOBTAG).Trim();
                        obj3.CNFNO = cs.getCNNO1(param.CNHNO, "F");
                    }
                    else
                    {
                        break;
                    }
                }
              obj3.NOTES = param.NOTES;
              obj3.DT1 = StringUtils.getString(param.DT1) == "" ? tmp : DateTimeUtil.getDateTime(param.DT1);
              obj3.DT2 = StringUtils.getString(param.DT2) == "" ? tmp : DateTimeUtil.getDateTime(param.DT2);
              obj3.DSCON1 = StringUtils.getDecimal(param.DSCON1);
              obj3.DSCON2 = StringUtils.getDecimal(param.DSCON2);
              obj3.YRS =StringUtils.getShort(param.YRS);
              obj3.MNS = StringUtils.getShort(param.MNS);
              obj3.DYS = StringUtils.getShort(param.DYS);
              obj3.RNFEE = StringUtils.getInt(param.RNFEE);
              obj3.RNTP = StringUtils.getShort(param.RNTP);
              obj3.RNFEETP = StringUtils.getShort(param.RNFEETP);
              obj3.DEALERNM = StringUtils.getString(param.DEALERNM).Trim();
              if (param.TAGID == null && param.mJOBTAG != null)
              {
                  db.CONTRAF.Add(obj3);
              }
              break;
          case "4":
              CONTRAC obj4;
              if (param.TAGID != null)
              {
                  obj4 = (from t in db.CONTRAC where t.TAGID == param.TAGID select t).SingleOrDefault();
              }
              else
              {
                  if (param.mJOBTAG != null)
                  {
                      obj4 = new CONTRAC();
                      obj4.TAGID = cs.getTagidByDatetime();
                      obj4.CNSTA = " ";
                      obj4.JOBTAG = StringUtils.getString(param.mJOBTAG).Trim();
                      obj4.CNCNO   = cs.getCNNO1(param.CNHNO, "C");
                  }
                  else
                  {
                      break;
                  }
              }
              obj4.NOTES = param.NOTES;
              obj4.DT1 = StringUtils.getString(param.DT1) == "" ? tmp : DateTimeUtil.getDateTime(param.DT1);
              obj4.DT2 = StringUtils.getString(param.DT2) == "" ? tmp : DateTimeUtil.getDateTime(param.DT2);
              obj4.DSCON1 = StringUtils.getDecimal(param.DSCON1);
              obj4.DSCON2 = StringUtils.getDecimal(param.DSCON2);
              obj4.YRS =StringUtils.getShort(param.YRS);
              obj4.MNS = StringUtils.getShort(param.MNS);
              obj4.DYS = StringUtils.getShort(param.DYS);
              obj4.RNFEE = StringUtils.getInt(param.RNFEE);
              obj4.RNTP = StringUtils.getShort(param.RNTP);
              obj4.RNFEETP = StringUtils.getShort(param.RNFEETP);
              obj4.RNTERTP = StringUtils.getShort(param.RNTERTP);
              obj4.DEALERNM = param.DEALERNM;
              if (param.TAGID == null && param.mJOBTAG != null)
              {
                  db.CONTRAC.Add(obj4);
              }             
              break;
          case "5":
              CONTRAA obj5;
              if (param.TAGID != null)
              {
                  obj5 = (from t in db.CONTRAA where t.TAGID == param.TAGID select t).SingleOrDefault();
              }
              else
              {
                  if (param.mJOBTAG != null)
                  {
                      obj5 = new CONTRAA();
                      obj5.TAGID = cs.getTagidByDatetime();
                      obj5.CNSTA = " ";
                      obj5.JOBTAG = StringUtils.getString(param.mJOBTAG).Trim();
                      obj5.CNANO = cs.getCNNO1(param.CNHNO, "A");
                  }
                  else
                  {
                      break;
                  }
              }
              obj5.NOTES = param.NOTES;
              obj5.DT1 = StringUtils.getString(param.DT1) == "" ? tmp : DateTimeUtil.getDateTime(param.DT1);
              obj5.DT2 = StringUtils.getString(param.DT2) == "" ? tmp : DateTimeUtil.getDateTime(param.DT2);
              obj5.DSCON1 = StringUtils.getDecimal(param.DSCON1);
              obj5.DSCON2 = StringUtils.getDecimal(param.DSCON2);
              obj5.YRS =StringUtils.getShort(param.YRS);
              obj5.MNS = StringUtils.getShort(param.MNS);
              obj5.DYS = StringUtils.getShort(param.DYS);
              obj5.RNFEE = StringUtils.getInt(param.RNFEE);              
              obj5.RNFEETP = StringUtils.getShort(param.RNFEETP);              
              obj5.DEALERNM = StringUtils.getString(param.DEALERNM).Trim();
              obj5.OTHERN = StringUtils.getString(param.OTHERN).Trim();
              if (param.TAGID == null && param.mJOBTAG != null)
              {
                  db.CONTRAA.Add(obj5);
              }  
              break;
        }
      if (param.TAGID == null)
      {
          String rmno = StringUtils.getString(param.mHOUSE).Trim();
          if (param.type == "1")
          {
              YRHCNT obj = (from t in db.YRHCNT where t.RMNO == rmno && t.YR == System.DateTime.Now.Year select t).SingleOrDefault();
              if (obj != null)
              {
                  db.Database.ExecuteSqlCommand("UPDATE YRHCNT SET CNCNT=CNCNT+1 WHERE RMNO = {0} AND YR={1}", rmno, System.DateTime.Now.Year);                  
              }
              else
              {
                  db.Database.ExecuteSqlCommand("INSERT INTO YRHCNT (RMNO,YR,CNCNT)values({0},{1},{2})", rmno, System.DateTime.Now.Year,1);
              }
          }
          else
          {
              String CN = "";
              switch (param.type)
              {
                  case "2":
                      CN = "P";
                      break;
                  case "3":
                      CN = "F";
                      break;
                  case "4":
                      CN = "C";
                      break;
                  case "5":
                      CN = "A";
                      break;
              }
              YRCNT obj = (from t in db.YRCNT where t.CNHNO == param.CNHNO && t.CN == CN select t).SingleOrDefault();
              if (obj != null)
              {                  
                  db.Database.ExecuteSqlCommand("UPDATE YRCNT SET CNCNT=CNCNT+1 WHERE CNHNO = {0} AND CN={1}", param.CNHNO,CN);
              }
              else
              {
                  db.Database.ExecuteSqlCommand("INSERT INTO YRCNT (CNHNO,CN,CNCNT)values({0},{1},{2})", param.CNHNO, CN,1);
              }

          }
      }
      
        i = db.SaveChanges();
     }
       
  
       return i;
   }


   public int doSave2(String mode, MAKRMAN param)
   {
       int i = 0;
       var query = (from t in db.MAKRMAN select t);
       if (param.TAGID != null)
       {
           MAKRMAN obj = null;
           if (mode.Equals("add"))
           {
               obj = new MAKRMAN();
               obj.MAKRNO = param.MAKRNO;
               obj.TAGID = cs.getTagidByDatetime();
           }
           else
           {
               obj = query.Where(q => q.TAGID == param.TAGID).SingleOrDefault();
           }
           
           obj.TEL = StringUtils.getString(param.TEL).Trim();
           obj.MAN = StringUtils.getString(param.MAN).Trim();
           obj.EMAIL = StringUtils.getString(param.EMAIL).Trim();
           obj.NOTES = StringUtils.getString(param.NOTES).Trim();
           
           if (mode.Equals("add"))
           {
               db.MAKRMAN.Add(obj);
           }
           i = db.SaveChanges();
       }
       return i;
   }

   public int doRemoveHouse(String TAGID,String HOUSE)
   {
       int i=0;
       /*
W_SQL = "DELETE FROM CONTRAH WHERE TAGID = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELCNH201")
W_SQL = "UPDATE ROOMF SET ONUSE = 0 WHERE CNO = ?W_RMNO"
     * 
SQLEXEC(CONNID,W_SQL,"DELRM201")
W_SQL = "DELETE FROM RMLIV WHERE JOBTAG = ?W_TAGID "
     * 
SQLEXEC(CONNID,W_SQL,"DELLIV201")
W_SQL = "DELETE FROM RMTEL WHERE JOBTAG = ?W_TAGID "
     * 
SQLEXEC(CONNID,W_SQL,"DELTEL201")
W_SQL = "DELETE FROM RMCAR WHERE JOBTAG = ?W_TAGID "
     * 
SQLEXEC(CONNID,W_SQL,"DELCAR201")
W_SQL = "DELETE FROM RMAST WHERE JOBTAG = ?W_TAGID "
     * 
SQLEXEC(CONNID,W_SQL,"DELAST201")
W_SQL = "DELETE FROM DISCNTRA WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELDISCN201")
     * 
W_SQL = "DELETE FROM UPDNOTE WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELNOTE201")
     * 
W_SQL = "DELETE FROM RMNEWS WHERE RMTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELNEWS201")

     * 
&& 雜項
W_SQL = "DELETE FROM CONTRAA WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELCNA201")

     * 
&& 俱樂部
W_SQL = "DELETE FROM CONTRAC WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELCNC201")
     * 
W_SQL = "DELETE FROM MBRM WHERE JOBTAG = ?W_TAGID "
SQLEXEC(ConnID,W_SQL,"DELMBR201")

     * 
&& 傢俱
W_SQL = "DELETE FROM CONTRAF WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELCNF201")
     * 
W_SQL = "DELETE FROM CNEQUIP WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELEQP201")

     * 
     * 
     * 
     * 
     * 
     * 
&& 車位
W_SQL = "SELECT PARKNO FROM CONTRAP WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"PARKNO201")
SELECT PARKNO201
SCAN
	W_PARKNO = PARKNO201.PARKNO
	W_SQL = "UPDATE CARLOCT SET ONUSE = 0 WHERE CNO = ?W_PARKNO"
	SQLEXEC(CONNID,W_SQL,"CARLOCT201")
	SELECT PARKNO201
ENDSCAN
     * 
     * 
     * 
     * 
     * 
     * 
W_SQL = "DELETE FROM CONTRAP WHERE JOBTAG = ?W_TAGID "
SQLEXEC(CONNID,W_SQL,"DELCNP201")
     * 

&& 租金
W_SQL = "DELETE FROM RMFEEM WHERE RMTAG = ?W_TAGID"
SQLEXEC(CONNID,W_SQL,"DELFEE201")
W_SQL = "DELETE FROM OTHERFEE WHERE RMTAG = ?W_TAGID"
SQLEXEC(CONNID,W_SQL,"DELOTHFEE201")*/
       try
       {
           db.Database.ExecuteSqlCommand("DELETE FROM CONTRAH WHERE TAGID = {0}", TAGID);
           db.Database.ExecuteSqlCommand("UPDATE ROOMF SET ONUSE = 0 WHERE CNO = {0}", HOUSE);
           db.Database.ExecuteSqlCommand("DELETE FROM RMLIV WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM RMTEL WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM RMCAR WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM RMAST WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM DISCNTRA WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM UPDNOTE WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM RMNEWS WHERE RMTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM CONTRAA WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM CONTRAC WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM MBRM WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM CONTRAF WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM CNEQUIP WHERE JOBTAG = {0}", TAGID);
           var query = from t in db.CONTRAP where t.JOBTAG == TAGID select t;
           foreach (var item in query)
           {
               db.Database.ExecuteSqlCommand("UPDATE CARLOCT SET ONUSE = 0 WHERE CNO = {0}", item.PARKNO);
           }

           db.Database.ExecuteSqlCommand("DELETE FROM CONTRAP WHERE JOBTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM RMFEEM WHERE RMTAG = {0}", TAGID);
           db.Database.ExecuteSqlCommand("DELETE FROM OTHERFEE WHERE RMTAG = {0}", TAGID);
           i = 1;
       }
       catch (Exception ex)
       {
           
       }
       return i;
   }

   public int doRemove(String cno)
   {
       int i = 1;

       MAKR obj = (from c in db.MAKR
                       where c.CNO == cno
                       select c).Single();
       db.MAKR.Remove(obj);
       db.SaveChanges();
       return i;
   }

   public int doRemove2(String tagid)
   {
       int i=1;

       MAKRMAN obj = (from c in db.MAKRMAN
                     where c.TAGID == tagid
                 select c).Single();
       db.MAKRMAN.Remove(obj);
    db.SaveChanges();

       return i;
   }

    //牌價
   public JArray getPBRENTAL(String CNO)
   {
       JArray ja = new JArray();
       DateTime currentTime = new DateTime();
       currentTime = System.DateTime.Now;

       DateTime dt = DateTimeUtil.getDateTime(currentTime.Year + "-" +  currentTime.Month + "-"+ currentTime.Day + " 00:00:00");
              

       var query = from t in db.PBRENTAL where t.RMNO==CNO && (t.DT1 <= dt && t.DT2 >= dt) select t;

       

       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                           
                        {"RENTAL",StringUtils.getString(item.RENTAL)},
                        {"MGFEE",StringUtils.getString(item.MGFEE)},
                    };
           ja.Add(itemObject);
       }
       
       return ja;
   }

    //房屋基本資料
   public JArray getROOMF(String CNO)
   {
       JArray ja = new JArray();

           var query = from t in db.ROOMF where t.CNO == CNO select t;

           foreach (var item in query)
           {
               var itemObject = new JObject
                    {                           
                        {"PINS",StringUtils.getString(item.PINS).Trim()},
                        {"ADR1",StringUtils.getString(item.ADR1).Trim() + StringUtils.getString(item.DOOR).Trim() +"號" + StringUtils.getString(item.FLOR).Trim() + "樓"},
                        {"IP1",StringUtils.getString(item.IP1).Trim()},
                        {"IP2",StringUtils.getString(item.IP2).Trim()},
                        {"ONUSE",StringUtils.getString(item.ONUSE).Trim()}
                    };
               ja.Add(itemObject);
           }
       
       return ja;
   }

    //房屋合約
   public JArray getCONTRAH(String CNO,String CNHNO)
   {
       JArray ja = new JArray();
       var query = from t in db.CONTRAH where t.RMNO==CNO  select t;
       if (StringUtils.getString(CNHNO)!="")
       {
           query = query.Where(t => t.CNHNO == CNHNO);
       }
       else
       {
           query = query.Where(t => t.CNSTA == " ");           
       }
       
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                           
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"DEALERNM",StringUtils.getString(item.DEALERNM)},
                        {"DEALMAN",StringUtils.getString(item.DEALMAN).Trim()},
                        {"DEALNO",item.DEALNO},
                        {"BROKNO",item.BROKNO},
                        {"COMETP",StringUtils.getString(item.COMETP).Trim()},
                        {"COMENOTE",StringUtils.getString(item.COMENOTE).Trim()},
                        {"MLIVER",StringUtils.getString(item.MLIVER).Trim()},
                        {"BROKERNM",StringUtils.getString(item.BROKERNM).Trim()},
                        {"BROKMAN",StringUtils.getString(item.BROKMAN).Trim()},
                        {"BROKFEE",StringUtils.getString(item.BROKFEE).Trim()},
                        {"LOGUSR",StringUtils.getString(item.LOGUSR).Trim()},
                        {"YRS",StringUtils.getString(item.YRS).Trim()},
                        {"MNS",StringUtils.getString(item.MNS).Trim()},
                        {"DYS",StringUtils.getString(item.DYS).Trim()},
                        {"RNFEE",StringUtils.getString(item.RNFEE).Trim()},
                        {"RNFEETP",StringUtils.getString(item.RNFEETP).Trim()},
                        {"MGFEE",StringUtils.getString(item.MGFEE).Trim()},
                        {"MGFEETP",StringUtils.getString(item.MGFEETP).Trim()},
                        {"GUAFEE",StringUtils.getString(item.GUAFEE).Trim()},
                        {"CSTTP",StringUtils.getString(item.CSTTP ).Trim()},
                        {"CNHNO",StringUtils.getString(item.CNHNO ).Trim()},
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"DSCON1",StringUtils.getString(item.DSCON1).Trim()},
                        {"DSCON2",StringUtils.getString(item.DSCON2).Trim()},
                        {"SP",StringUtils.getString(item.SP).Trim()},
                        {"CNHNOTE",StringUtils.getString(item.CNHNOTE).Trim()}


                    };
           ja.Add(itemObject);
       }

       return ja;
   }


   //房屋合約-前租戶
   public JArray getCONTRAH_LastOne(String CNO)
   {
       JArray ja = new JArray();
       var query = (from t in db.CONTRAH where t.RMNO == CNO && t.CNSTA =="C"  select t).OrderByDescending(t => t.ENDYM).FirstOrDefault() ;


 
           var itemObject = new JObject
                    {                           
                        {"TAGID",StringUtils.getString(query.TAGID)},
                        {"DEALERNM",StringUtils.getString(query.DEALERNM)},
                        {"DEALMAN",StringUtils.getString(query.DEALMAN).Trim()},
                        {"DEALNO",query.DEALNO},
                        {"BROKNO",query.BROKNO},
                        {"COMETP",StringUtils.getString(query.COMETP).Trim()},
                        {"COMENOTE",StringUtils.getString(query.COMENOTE).Trim()},
                        {"MLIVER",StringUtils.getString(query.MLIVER).Trim()},
                        {"BROKERNM",StringUtils.getString(query.BROKERNM).Trim()},
                        {"BROKMAN",StringUtils.getString(query.BROKMAN).Trim()},
                        {"BROKFEE",StringUtils.getString(query.BROKFEE).Trim()},
                        {"LOGUSR",StringUtils.getString(query.LOGUSR).Trim()},
                        {"YRS",StringUtils.getString(query.YRS).Trim()},
                        {"MNS",StringUtils.getString(query.MNS).Trim()},
                        {"DYS",StringUtils.getString(query.DYS).Trim()},
                        {"RNFEE",StringUtils.getString(query.RNFEE).Trim()},
                        {"RNFEETP",StringUtils.getString(query.RNFEETP).Trim()},
                        {"MGFEE",StringUtils.getString(query.MGFEE).Trim()},
                        {"MGFEETP",StringUtils.getString(query.MGFEETP).Trim()},
                        {"GUAFEE",StringUtils.getString(query.GUAFEE).Trim()},
                        {"CSTTP",StringUtils.getString(query.CSTTP ).Trim()},
                        {"CNHNO",StringUtils.getString(query.CNHNO ).Trim()},
                        {"DT1",String.Format("{0:yyyy-MM-dd}",query.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",query.DT2)},
                        {"DSCON1",StringUtils.getString(query.DSCON1).Trim()},
                        {"DSCON2",StringUtils.getString(query.DSCON2).Trim()},
                        {"SP",StringUtils.getString(query.SP).Trim()},
                        {"CNHNOTE",StringUtils.getString(query.CNHNOTE).Trim()}


                    };
           ja.Add(itemObject);
       

       return ja;
   }




    //車位合約
   public JArray getCONTRAP(String TAGID)
   {
       JArray ja = new JArray();
       var query = from t in db.CONTRAP where t.JOBTAG == TAGID && t.CNSTA == " " select t;
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                           
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"CNPNO",StringUtils.getString(item.CNPNO)},
                        {"DEALERNM",StringUtils.getString(item.DEALERNM)},                        
                        {"DEALNO",StringUtils.getString(item.DEALNO).Trim()},                        
                        {"DSCON1",StringUtils.getString(item.DSCON1).Trim()},
                        {"DSCON2",StringUtils.getString(item.DSCON2).Trim()},
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"YRS",StringUtils.getString(item.YRS).Trim()},
                        {"MNS",StringUtils.getString(item.MNS).Trim()},
                        {"DYS",StringUtils.getString(item.DYS).Trim()},
                        {"LOCT",StringUtils.getString(item.LOCT).Trim()},
                        {"NOTES",StringUtils.getString(item.NOTES).Trim()},                        
                        {"PARKNO",StringUtils.getString(item.PARKNO).Trim()},
                        {"RNFEE",StringUtils.getString(item.RNFEE).Trim()},
                        {"RNFEETP",StringUtils.getString(item.RNFEETP).Trim()},                        
                        {"RNTERTP",StringUtils.getString(item.RNTERTP).Trim()},
                        {"RNTP",StringUtils.getString(item.RNTP).Trim()}

                    };
           ja.Add(itemObject);
       }

       return ja;
   }


   //傢俱合約
   public JArray getCONTRAF(String TAGID)
   {
       JArray ja = new JArray();
       var query = from t in db.CONTRAF where t.JOBTAG == TAGID && t.CNSTA == " " select t;
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                           
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"CNFNO",StringUtils.getString(item.CNFNO)},
                        {"DEALERNM",StringUtils.getString(item.DEALERNM)},                      
                        {"DEALNO",StringUtils.getString(item.DEALNO).Trim()},                    
                        {"DSCON1",StringUtils.getString(item.DSCON1).Trim()},
                        {"DSCON2",StringUtils.getString(item.DSCON2).Trim()},
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"YRS",StringUtils.getString(item.YRS).Trim()},
                        {"MNS",StringUtils.getString(item.MNS).Trim()},
                        {"DYS",StringUtils.getString(item.DYS).Trim()},                      
                        {"NOTES",StringUtils.getString(item.NOTES).Trim()},                         
                        {"RNFEE",StringUtils.getString(item.RNFEE).Trim()},
                        {"RNFEETP",StringUtils.getString(item.RNFEETP).Trim()},                        
                        {"RNTERTP",StringUtils.getString(item.RNTERTP).Trim()},
                        {"RNTP",StringUtils.getString(item.RNTP).Trim()}
                    };
           ja.Add(itemObject);
       }

       return ja;
   }

   //俱樂部合約
   public JArray getCONTRAC(String TAGID)
   {
       JArray ja = new JArray();
       var query = from t in db.CONTRAC where t.JOBTAG == TAGID && t.CNSTA == " " select t;
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                           
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"CNCNO",StringUtils.getString(item.CNCNO)},
                        {"DEALERNM",StringUtils.getString(item.DEALERNM)},                      
                        {"DEALNO",StringUtils.getString(item.DEALNO).Trim()},                    
                        {"DSCON1",StringUtils.getString(item.DSCON1).Trim()},
                        {"DSCON2",StringUtils.getString(item.DSCON2).Trim()},
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},
                        {"YRS",StringUtils.getString(item.YRS).Trim()},
                        {"MNS",StringUtils.getString(item.MNS).Trim()},
                        {"DYS",StringUtils.getString(item.DYS).Trim()},                      
                        {"NOTES",StringUtils.getString(item.NOTES).Trim()},                         
                        {"RNFEE",StringUtils.getString(item.RNFEE).Trim()},
                        {"RNFEETP",StringUtils.getString(item.RNFEETP).Trim()},                        
                        {"RNTERTP",StringUtils.getString(item.RNTERTP).Trim()},
                        {"RNTP",StringUtils.getString(item.RNTP).Trim()}
                    };
           ja.Add(itemObject);
       }

       return ja;
   }


   //俱樂部合約
   public JArray getCONTRAA(String TAGID)
   {
       JArray ja = new JArray();
       var query = from t in db.CONTRAA where t.JOBTAG == TAGID && t.CNSTA == " " select t;
       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                           
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"CNANO",StringUtils.getString(item.CNANO)},
                        {"DEALERNM",StringUtils.getString(item.DEALERNM).Trim()},                                                
                        {"OTHERN",StringUtils.getString(item.OTHERN).Trim()},   
                        {"DT1",String.Format("{0:yyyy-MM-dd}",item.DT1)},
                        {"DT2",String.Format("{0:yyyy-MM-dd}",item.DT2)},                    
                        {"NOTES",StringUtils.getString(item.NOTES).Trim()},                         
                        {"RNFEE",StringUtils.getString(item.RNFEE).Trim()},
                        {"RNFEETP",StringUtils.getString(item.RNFEETP).Trim()}
                        //{"RNTERTP",StringUtils.getString(item.RNTERTP).Trim()},
                        //{"RNTP",StringUtils.getString(item.RNTP).Trim()}
                    };
           ja.Add(itemObject);
       }

       return ja;
   }



   public JObject getUPDNOTE(EasyuiParamPoco param,String TAGID)
   {
       JArray ja = new JArray();
       JObject jo = new JObject();
       var query = from t in db.UPDNOTE where t.JOBTAG==TAGID select t;
       if (query.Count() > 0)
       {
           jo.Add("total", query.Count());
           query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
        //   query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    
       }

       foreach (var item in query)
       {
           var itemObject = new JObject
                    {                                           
                        {"UPDDT",String.Format("{0:yyyy-MM-dd}",item.UPDDT)},
                        {"UPDTP",StringUtils.getString(item.UPDTP)},
                        {"NOTES",StringUtils.getString(item.NOTES)},
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"LOGUSR",StringUtils.getString(item.LOGUSR)},
                        {"LOGDT",StringUtils.getString(item.LOGDT)},
                        {"CNNO",StringUtils.getString(item.CNNO )},
                        {"CNTAG",StringUtils.getString(item.CNTAG)},
                        {"TAGID",StringUtils.getString(item.TAGID)}                        
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



}
}