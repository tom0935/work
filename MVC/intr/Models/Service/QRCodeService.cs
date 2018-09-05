using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using IntranetSystem.Poco;
using IntranetSystem.Models.Edmx;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using HowardCoupon_vs2010.Poco;

namespace IntranetSystem.Models.Service
{

    public class DepartPoco
    {
        public String CODE { get; set; }
        public String NAME { get; set; }
    }

    public class QRCodeService
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
        private HowardIntraEntities db = new HowardIntraEntities();
        public JObject getQRCode(PagingParamPoco param)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE   
                        where t.ISSHOW !="0"           
                        select t;
                        

            if (StringUtils.getString(param.search) != "")
            {                                  
                var q1 = query.Where(q => q.SUBJECT.Contains("" + param.search + ""));
                if (q1.Count() == 0)
                {
                    var q2 = query.Where(q => q.CONTENT.Contains("" + param.search + ""));
                    if (q2.Count() > 0)
                    {
                        query = q2;
                    }
                }
                else
                {
                    query = q1;
                }
            }
              
            query = query.OrderByDescending(q => q.UUID);
            
            var total = query.Count();
           // query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip(param.offset).Take(param.limit);    //分頁    
           

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {   
                          {"UUID",StringUtils.getString(item.UUID)},
                          {"SUBJECT",StringUtils.getString(item.SUBJECT)},
                          {"CONTENT",StringUtils.getString(item.CONTENT) },
                          {"URL",StringUtils.getString(item.URL)},
                          {"EDMURL",StringUtils.getString(item.EDMURL)},
                          {"EDIT_DT",StringUtils.getString(item.EDIT_DT)},
                          {"EDIT_USER",StringUtils.getString(item.EDIT_USER)},
                          {"CNT",StringUtils.getString(item.CNT)},
                          {"ISSHOW",StringUtils.getString(item.ISSHOW)},
                          {"IMG",StringUtils.getString(item.IMG)},
                          {"COUPON_IMG",StringUtils.getString(item.COUPON_IMG)},
                          {"QA_EMAIL",StringUtils.getString(item.QA_EMAIL)},
                          {"SDT",String.Format("{0:yyyy-MM-dd}", item.SDT)},
                          {"EDT",String.Format("{0:yyyy-MM-dd}", item.EDT)},
                          {"CUS_SAVE",StringUtils.getString(item.CUS_SAVE)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);

            return jo;
      }


        public JObject getQRCodeView(PagingParamPoco param)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE                        
                        select t;


            if (StringUtils.getString(param.search) != "")
            {
                var q1 = query.Where(q => q.SUBJECT.Contains("" + param.search + ""));
                if (q1.Count() == 0)
                {
                    var q2 = query.Where(q => q.CONTENT.Contains("" + param.search + ""));
                    if (q2.Count() > 0)
                    {
                        query = q2;
                    }
                }
                else
                {
                    query = q1;
                }
            }

            query = query.OrderByDescending(q => q.UUID);

            var total = query.Count();
            // query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip(param.offset).Take(param.limit);    //分頁    


            foreach (var item in query)
            {
                var itemObject = new JObject
                       {   
                          {"UUID",StringUtils.getString(item.UUID)},
                          {"SUBJECT",StringUtils.getString(item.SUBJECT)},
                          {"CONTENT",StringUtils.getString(item.CONTENT) },
                          {"URL",StringUtils.getString(item.URL)},
                          {"EDMURL",StringUtils.getString(item.EDMURL)},
                          {"EDIT_DT",StringUtils.getString(item.EDIT_DT)},
                          {"EDIT_USER",StringUtils.getString(item.EDIT_USER)},
                          {"CNT",StringUtils.getString(item.CNT)},
                          {"ISSHOW",StringUtils.getString(item.ISSHOW)},
                          {"IMG",StringUtils.getString(item.IMG)},
                          {"COUPON_IMG",StringUtils.getString(item.COUPON_IMG)},
                          {"QA_EMAIL",StringUtils.getString(item.QA_EMAIL)},
                          {"SDT",String.Format("{0:yyyy-MM-dd}", item.SDT)},
                          {"EDT",String.Format("{0:yyyy-MM-dd}", item.EDT)},
                          {"CUS_SAVE",StringUtils.getString(item.CUS_SAVE)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);

            return jo;
        }


        public JObject getQRCodeDTL(int UUID)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE
                        join d in db.QRCODE_DTL on t.UUID equals d.QRCODE_UUID into dd
                        from d in dd.DefaultIfEmpty()
                        where d.ISSHOW_DTL == "1" && t.UUID == UUID
                        select new {d.UUID_DTL,d.URL_DTL,d.CNT_DTL,d.EDIT_DT_DTL,d.EDIT_USER_DTL,d.SUBJECT_DTL,d.ISSHOW_DTL,t.SUBJECT };

            query = query.OrderByDescending(q => q.UUID_DTL);
            var total = query.Count();
            // query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
           // query = query.Skip(param.offset).Take(param.limit);    //分頁    


            foreach (var item in query)
            {
                var itemObject = new JObject
                       {   
                          {"UUID_DTL",StringUtils.getString(item.UUID_DTL)},
                          {"SUBJECT",StringUtils.getString(item.SUBJECT)},
                          {"SUBJECT_DTL",StringUtils.getString(item.SUBJECT_DTL)},
                          {"URL_DTL",StringUtils.getString(item.URL_DTL)},
                          {"EDIT_USER_DTL",StringUtils.getString(item.EDIT_USER_DTL)},
                          {"EDIT_DT_DTL",StringUtils.getString(item.EDIT_DT_DTL)},
                          {"CNT_DTL",StringUtils.getString(item.CNT_DTL)},
                          {"ISSHOW_DTL",StringUtils.getString(item.ISSHOW_DTL)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);

            return jo;
        }

     /*
        public JArray getCorp()
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
             string[] strArray = new string [] { "18", "19","21","22","30","33"};
             var query = from t in db.BACK_COMPID where t.BACK_COMP_NO != "**" && strArray.Contains(t.BACK_COMP_NO) select t;
            
            foreach (var item in query)
            {
                var itemObject = new JObject
                       {                             
                          {"CODE",item.BACK_COMP_NO},
                          {"NAME",item.BACK_CO_NAME1}
                       };
                ja.Add(itemObject);
            }
                    
            return ja;
        }
        */
        public String getURL(String UUID)
        {
            String url="";
            if (StringUtils.getString(UUID) != "")
            {
                int uuid = System.Convert.ToInt32(UUID);                
                var query = (from t in db.QRCODE_DTL where t.UUID_DTL == uuid select t).SingleOrDefault();
                url = query.URL_DTL;
            }            
            return url;
        }


        public int doCreate(QRCODE param, JArray list)
        {
                int i = 0;
                QRCODE obj = new QRCODE();
                obj.CONTENT = param.CONTENT;
                obj.CORP = "01";
                obj.EDIT_DT = System.DateTime.Now;
                obj.EDIT_USER = param.EDIT_USER;
                obj.SUBJECT = param.SUBJECT;
                obj.CNT = 0;
                obj.ISSHOW = param.ISSHOW;
                obj.URL = param.URL;
                obj.EDMURL = param.EDMURL;
                obj.CREATE_DT = System.DateTime.Now;
                obj.CUS_SAVE = param.CUS_SAVE;
                obj.QA_EMAIL = param.QA_EMAIL;
                obj.SDT = param.SDT;
                obj.EDT = param.EDT;

                if (StringUtils.getString(param.COUPON_IMG) != "" && StringUtils.getString(param.ISSHOW)=="1")
                {
                    obj.ISSHOW = "2";
                }


                if (StringUtils.getString(param.IMG) != "")
                {
                    obj.IMG = param.IMG;
                }
                if (StringUtils.getString(param.COUPON_IMG) != "")
                {
                    obj.COUPON_IMG = param.COUPON_IMG;
                }
                db.QRCODE.AddObject(obj);

                i = db.SaveChanges();
                foreach (JObject o in list)
                {
                    QRCODE_APPLY_DEPART dpt = new QRCODE_APPLY_DEPART();
                    dpt.QRCODE_UUID = obj.UUID;
                    dpt.CODE = o["id"].ToString();
                    dpt.NAME = o["text"].ToString();
                    db.QRCODE_APPLY_DEPART.AddObject(dpt);
                }
                i = db.SaveChanges();
            return obj.UUID;
        }


        public int doCreateDTL(QRCODE_DTL param)
        {
            int i = 0;
            QRCODE_DTL obj = new QRCODE_DTL();
                        
            obj.EDIT_DT_DTL = System.DateTime.Now;
            obj.EDIT_USER_DTL = param.EDIT_USER_DTL;
            obj.SUBJECT_DTL = param.SUBJECT_DTL;
            obj.CNT_DTL = 0;
            obj.ISSHOW_DTL = param.ISSHOW_DTL;
            obj.URL_DTL = param.URL_DTL;
            obj.QRCODE_UUID = param.QRCODE_UUID;
            obj.CREATE_DT_DTL = System.DateTime.Now;
            db.QRCODE_DTL.AddObject(obj);
            i = db.SaveChanges();
            return obj.UUID_DTL;
        }

        public int doEdit(QRCODE param, JArray list)
        {
            int i = 0;
            var obj = (from t in db.QRCODE where t.UUID == param.UUID select t).SingleOrDefault();
            if (obj != null)
            {
                obj.CONTENT = param.CONTENT;                
                obj.EDIT_DT = System.DateTime.Now;
                obj.EDIT_USER = param.EDIT_USER;
                obj.SUBJECT = param.SUBJECT;
                if (param.ISSHOW == "0")
                {
                    obj.ISSHOW = param.ISSHOW;
                }
                obj.CUS_SAVE = param.CUS_SAVE;
                obj.EDMURL = param.EDMURL;
                obj.QA_EMAIL = param.QA_EMAIL;
                obj.SDT = param.SDT;
                obj.EDT = param.EDT;
                if (StringUtils.getString(param.IMG) != "")
                {
                    obj.IMG = param.IMG;
                }
                if (StringUtils.getString(param.COUPON_IMG) != "")
                {
                    obj.COUPON_IMG = param.COUPON_IMG;

                }




                if (StringUtils.getString(param.COUPON_IMG) != "" && StringUtils.getString(param.ISSHOW) == "1")
                {
                    obj.ISSHOW = "2";
                }




                var qdpt = from t in db.QRCODE_APPLY_DEPART where t.QRCODE_UUID == param.UUID select t;

                foreach(var item in qdpt){
                    db.QRCODE_APPLY_DEPART.DeleteObject(item);
                }

                foreach (JObject o in list)
                {
                    QRCODE_APPLY_DEPART dpt = new QRCODE_APPLY_DEPART();
                    dpt.QRCODE_UUID = param.UUID;
                    dpt.CODE = o["id"].ToString();
                    dpt.NAME = o["text"].ToString();
                    db.QRCODE_APPLY_DEPART.AddObject(dpt);
                }


                i = db.SaveChanges();
            }
            
            return i;
        }

        public int doEditDTL(QRCODE_DTL param)
        {
            int i = 0;
            var obj = (from t in db.QRCODE_DTL where t.UUID_DTL == param.UUID_DTL select t).SingleOrDefault();
            if (obj != null)
            {                
                obj.EDIT_DT_DTL = System.DateTime.Now;
                obj.EDIT_USER_DTL = param.EDIT_USER_DTL;
                obj.SUBJECT_DTL = param.SUBJECT_DTL;
                obj.ISSHOW_DTL = param.ISSHOW_DTL;                
                i = db.SaveChanges();
            }

            return i;
        }


        public int doEditURL(QRCODE_DTL param)
        {
            int i = 0;
            var obj = (from t in db.QRCODE_DTL where t.UUID_DTL == param.UUID_DTL select t).SingleOrDefault();
            if (obj != null)
            {              
                obj.EDIT_DT_DTL = System.DateTime.Now;
                obj.EDIT_USER_DTL = param.EDIT_USER_DTL;   
                obj.URL_DTL = param.URL_DTL;
                i = db.SaveChanges();
            }

            return i;
        }

        public int doRemove(int UUID,String userid)
        {
                       
                var query = (from t in db.QRCODE where t.UUID == UUID select t).SingleOrDefault();
                if (query != null)
                {
                    query.ISSHOW = "0";
                    query.EDIT_USER = userid;
                    query.EDIT_DT = System.DateTime.Now;
                }

           int i = db.SaveChanges();
            return i;
        }

        public int doRemoveDTL(int UUID, String userid)
        {


                var query = (from t in db.QRCODE_DTL where t.UUID_DTL == UUID select t).SingleOrDefault();
                if (query != null)
                {
                    query.ISSHOW_DTL = "0";
                    query.EDIT_USER_DTL = userid;
                    query.EDIT_DT_DTL = System.DateTime.Now;
                }
            
            int i = db.SaveChanges();
            return i;
        }


        public DataTable getDataTableByQRCode(String SDT,String EDT,String SUBJECT)
        {
            LinqExtensions le = new LinqExtensions();
            var query = from t in db.QRCODE
                        join d in db.QRCODE_DTL on t.UUID equals d.QRCODE_UUID into dd
                        from d in dd.DefaultIfEmpty()
                        where d.ISSHOW_DTL == "1" && t.ISSHOW == "1"
                        select new { d.UUID_DTL, d.URL_DTL,d.SUBJECT_DTL, d.CNT_DTL,  t.EDMURL, t.SUBJECT,t.UUID,d.CREATE_DT_DTL };

            if (SUBJECT != "*")
            {
                query = query.Where(q => q.SUBJECT == SUBJECT);
            }
            if (StringUtils.getString(SDT) != "" && StringUtils.getString(EDT) != "")
            {
                DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
                DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
                query = query.Where(q => q.CREATE_DT_DTL >= sdt && q.CREATE_DT_DTL <= edt);                
            }

            DataTable dt = le.LinqQueryToDataTable(query);
            return dt;
        }

        public int doChange(String UUID,String ISSHOW)
        {
            int i = 0;
            int uuid = Convert.ToInt32(UUID);

            var obj = (from t in db.QRCODE where t.UUID == uuid select t).SingleOrDefault();
            if (obj != null)
            {               
                obj.ISSHOW = ISSHOW;
                i = db.SaveChanges();
            }

            return i;
        }



       

        public JArray getDepartSelect()
        {

            string query = "SELECT userid,NAME FROM I_USER where substr(userid,0,1)='a'";
            JArray ja = new JArray();
            JObject jo = new JObject();


            ja.Add(new JObject {{"text","全部"},{"id","*"}});
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while ((reader.Read()))
                    {

                        if (!reader[0].Equals(DBNull.Value))
                        {
                            var itemObject = new JObject
                            {                             
                                {"text",reader[1].ToString() },
                                {"id",reader[0].ToString()}
                            };
                            ja.Add(itemObject);
                        }
                    }
                    reader.Close();
                }
                
            }
            
            return ja;

        }


        public JArray getDepartCurrent(int uuid)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE_APPLY_DEPART where t.QRCODE_UUID==uuid select t;
            
            foreach (var item in query)
            {
                var itemObject = new JObject
                       {                             
                          {"text",item.NAME},
                          {"id",item.CODE}
                       };
                ja.Add(itemObject);
            }

            return ja;
        }


    }
}