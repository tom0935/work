using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Jasper.Models.Service
{


    public class CommonService
    {
        private RENTEntities db = new RENTEntities();
      public String getTagidByDatetime(){
          String rtn="";
          /*
          Random rnd = new Random();
          int i = rnd.Next(0, 999);//回傳0-999的亂數
          return DateTime.Now.ToString("yyMMddHHmmss") + i;
           * */
             //ObjectParameter n =new ObjectParameter("n",0);
            // ObjectResult<String> r = db.ProcGetTagid();
            //String rtnValue = Convert.ToString(db.ProcGetTagid().FirstOrDefault());
          String x= Convert.ToString(db.ProcGetTagid().FirstOrDefault());
                         String rtnValue =DateTime.Now.ToString("yyMMddHHmm") + x.Substring(1,3);
          return rtnValue;
       //   ObjectParameter tagid =new ObjectParameter("tagid","");
          
      //ObjectResult<String> rtnValue = db.ProcGetTagid(tagid);
     // ObjectResult<String> rt = db.ProcGetTagid(tagid).FirstOrDefault();
        // db.ProcGetTagid()
          //proc query = (from t in db.ProcGetTagid() select t).to;

     // return "";
      }
      public String getDateTimeByView(DateTime dt)
      {
          return dt.ToString("yyyy-MM-dd");
      }

      public String getDateTimeStr(DateTime dt)
      {
          return dt.ToString("yyyyMMdd");
      }

      public JArray getCOUNTRY()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();
          
          var query = from t in db.COUNTRY select new { CNO = t.CNO, CNM = t.CNM };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNM},
                        {"CNM",item.CNO +" " +item.CNM},
                        
                    };
              ja.Add(itemObject);
          }
          //  jo.Add("rows", ja);            
          return ja;
      }


      //取得關係
      public JArray getAPPE()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND=="APPE" select new { CNO = t.CODE_NO , CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNM},
                        {"CNM",item.CNM},
                        
                    };
              ja.Add(itemObject);
          }
          //  jo.Add("rows", ja);            
          return ja;
      }

      //取得異動類別
      public JArray getUPDTP()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "CNUPDTP" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          //  jo.Add("rows", ja);            
          return ja;
      }

      public JArray getTELCODE()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "TELCODE" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNM},
                        {"CNM",item.CNM},
                        
                    };
              ja.Add(itemObject);
          }
          //  jo.Add("rows", ja);            
          return ja;
      }


      //取得房屋租約編號(房號[4碼]+年度[2碼]+次數[1碼]+租別[1碼]) by Foxpro
      public String getCNNO(String RMNO,int YR)
      {
          String cnno = "";
          YRHCNT obj = (from t in db.YRHCNT where t.RMNO == RMNO && t.YR == YR select t).SingleOrDefault();
          if (obj !=null)
          {
              int cnt = obj.CNCNT.Value;
              cnt = cnt + 1;
              
              cnno = RMNO + YR.ToString().Substring(2,2) + cnt;
          }
          else
          {
                            
              cnno = RMNO + YR.ToString().Substring(2, 2) + "1";
              
          }
          return cnno;
      }


      //取得房屋租約編號(房號[4碼]+年度[2碼]+次數[1碼]+租別[1碼]) by Foxpro
        
      public String getCNNO1(String CNHNO, String CN)
      {
          String cnno = "";
          YRCNT obj = (from t in db.YRCNT where t.CNHNO == CNHNO && t.CN == CN select t).SingleOrDefault();
          String scnt="";
          if (obj != null)
          {
              int cnt = obj.CNCNT.Value;
              cnt = cnt + 1;
             
              if(cnt < 10){
                  scnt = "0" + cnt;
              }else{
                  scnt = ""+ cnt;
              }
              cnno = CNHNO + CN + scnt;
          }
          else
          {
              cnno = CNHNO + CN + "01";
             
          }
          return cnno;
      }


      public String getConfig(String item)
      {
//          System.Configuration.Configuration rootWebConfig1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
          
          String returnValue = "";
          try
          {

              returnValue = System.Web.Configuration.WebConfigurationManager.AppSettings[item];           
            
          }
          catch (Exception ex)
          {
             
          }
          return returnValue;
      }


      public String getACCNM(String code_no)
      {
          var query = (from t in db.CONFCODE where t.CODE_KIND == "WCOD" && t.CODE_NO == code_no select t).Single();
          String cno = "";
          if (query != null)
          {
              cno = query.CODE_NAME;
          }
          return cno;
      }

      public JArray getCARDTYPE()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "CARDTYPE" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          //  jo.Add("rows", ja);            
          return ja;
      }

      //收入種類
      public JArray getINTP()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "INTP" select new { CNO = t.CODE_NAME, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNM.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }                
          return ja;
      }


      //發文單位
      public JArray getFUNCDEP()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FUNCDEP" select new { CNO = t.CODE_NAME, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNM.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }


      //修繕別
      public JArray getFIX205(String CODE)
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FIX205" && t.CODE_NO.Substring(0,1)==CODE select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }

        //修繕項目
      public JArray getFIX205ITEM(String CODE)
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FIX205ITEM" && t.CODE_NO.Substring(0, 2) == CODE select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }

      public JArray getNEWSTP()
      {
          JArray ja = new JArray();
          var query = db.NEWSTP.AsQueryable();

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)}                     
                    };
              ja.Add(itemObject);
          }

          return ja;
      }


      public JArray getFEETP()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FRM207" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }


      public JObject getMAKR(String q)
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.MAKR orderby t.CNO select new { CNO = t.CNO, CNM = t.CNM };
          if (StringUtils.getString(q) != "")
          {
              query = query.Where(t => t.CNO.Contains(q) || t.CNM.Contains(q));
          }

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"CNM",StringUtils.getString(item.CNM)}
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


      public JArray getACCTP()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();          
          var query = from t in db.ACCTP select new { CNO = t.TPNO, CNM = t.TPNM };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }


      public JArray getV_ACCF(String CNO)
      {
          JObject jo = new JObject();
          JArray ja = new JArray();
          var query = from t in db.V_ACCF where t.TP ==CNO select new { CNO = t.CNO, CNM = t.CNM };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }


      public JArray getOTHERN()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FRM211" select new { CNO = t.CODE_NO, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }

      public JArray getINFTP()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FRM301" select new { CNO = t.CODE_NAME, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }


        //代付人
      public JArray getACPTER()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();

          var query = from t in db.CONFCODE where t.CODE_KIND == "FRM518" select new { CNO = t.CODE_NAME, CNM = t.CODE_NAME };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }


      public DateTime getLastDate(DateTime dt)
      {
          int rmm = dt.Month;
          int? rdd = dt.Day;

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
              rdd = dt2.Day;
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

      public JArray getCARLOCT()
      {
          JObject jo = new JObject();
          JArray ja = new JArray();
           //SELECT * FROM CARLOCT WHERE USEON = '住宅' AND ONUSE = 0 ORDER BY LOCT,CNO
          var query = from t in db.CARLOCT where t.USEON=="住宅" && t.ONUSE == 0 select new { CNO = t.CNO , CNM = t.LOCT };

          foreach (var item in query)
          {
              var itemObject = new JObject
                    {   
                        {"CNO",item.CNO.Trim()},
                        {"CNM",item.CNM.Trim()},
                        
                    };
              ja.Add(itemObject);
          }
          return ja;
      }
       


    }
}