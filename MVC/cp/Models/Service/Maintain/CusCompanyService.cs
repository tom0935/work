using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HowardCoupon.Models.Dao;
using Newtonsoft.Json.Linq;
using System.Collections;
using HowardCoupon.Models.Dao.impl;
using HowardCoupon.Poco;
using HowardCoupon_vs2010.Models.Edmx;
using System.Linq.Dynamic;
using System.Linq;

namespace HowardCoupon_vs2010.Models.Service
{
    public class CusCompanyService
    {
        
        private IntraEntities db = new IntraEntities();

 


        public JObject getCusCompany(EasyuiParamPoco param,String CMPID,String SEARCH)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();         
            var query = from t in db.COUPON_CUS_COMPANY where t.CMPID!="*" select t;

            if (StringUtils.getString(CMPID) !="")
            {
                //Decimal uuid = StringUtils.getDecimal(CMPID);
                query= query.Where(t => t.CMPID == CMPID);
            }
            else if (StringUtils.getString(SEARCH) != "")
            {
                query = query.Where(t => t.CMPID.Contains(SEARCH) || t.ADDR.Contains(SEARCH) || t.CMPNAME.Contains(SEARCH) || t.EMAIL.Contains(SEARCH) || t.TEL.Contains(SEARCH));
            }

            if (query.Count() > 0 && param.sort!=null)
            {
                jo.Add("total", query.Count());
                query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
            }
            else
            {
                jo.Add("total", "");
            }
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},
                        {"CMPID",item.CMPID},
                        {"CMPNAME",item.CMPNAME},
                        {"TEL",item.TEL},
                        {"FAX",item.FAX},
                        {"ZIP",item.ZIP},                              
                        {"EMAIL",item.EMAIL},                              
                        {"ADDR",item.ADDR},                              
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
            }
            
            return jo;
        }



 

        public int doSave(CusCompanyPoco param)
        {
            JObject jo = new JObject();
            int i = 0;
            try
            {
                using (db)
                {
                    COUPON_CUS_COMPANY obj;
                    if (param.UUID != null)
                    {
                        Decimal uuid = StringUtils.getDecimal(param.UUID);
                        obj = (from t in db.COUPON_CUS_COMPANY where t.UUID == uuid select t).Single();
                    }
                    else
                    {
                        obj = new COUPON_CUS_COMPANY();
                    }
                    obj.CMPID = param.CMPID;
                    obj.CMPNAME = param.CMPNAME;
                    obj.EMAIL = param.EMAIL;
                    obj.ADDR = param.ADDR;
                    obj.ZIP = param.ZIP;
                    obj.TEL = param.TEL;
                    obj.FAX = param.FAX;
                    if (param.UUID == null)
                    {
                        db.COUPON_CUS_COMPANY.AddObject(obj);
                    }
                    i = db.SaveChanges();
                }
            }catch(Exception e){
                throw new Exception(e.Message);
            }
            return i;
        }


        public int doDelete(String UUID)
        {
            JObject jo = new JObject();
            int i = 0;
            try
            {
                using (db)
                {
                    Decimal uuid = StringUtils.getDecimal(UUID);
                    COUPON_CUS_COMPANY obj = (from t in db.COUPON_CUS_COMPANY where t.UUID == uuid select t).Single();
                    db.DeleteObject(obj);
                    i = db.SaveChanges();
                }
            }catch(Exception e){
                throw new Exception(e.Message);
            }
            return i;
        }

        public int chkCode(String CMPID)
        {
            var query = from t in db.COUPON_CUS_COMPANY where t.CMPID == CMPID select t;
            return query.Count();
        }

        //取回縣市
        public JArray getCity(){
            JArray ja = new JArray();
            var query = from t in db.IMC_CODE where t.KIND == "COMMON" && t.TYPE == "ZIP" group t by t.NAME into g select g;

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"NAME",item.Key},
                        {"CODE",item.Key},
                    };
                ja.Add(itemObject);
            }
            return ja;
        }

        //取回縣市
        public JArray getCity(String ZIP)
        {
            JArray ja = new JArray();
            var query = from t in db.IMC_CODE where t.KIND == "COMMON" && t.TYPE == "ZIP" && t.CODE==ZIP select t;
            
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"NAME",item.NAME},
                        {"CODE",item.CODE},
                    };
                ja.Add(itemObject);
            }
            return ja;
        }



        //取回區
        public JArray getZIPList(String NAME)
        {
            JArray ja = new JArray();
            var query = from t in db.IMC_CODE where t.KIND == "COMMON" && t.TYPE == "ZIP" && t.NAME == NAME select new {CODE=t.CODE,NAME=t.CITEM1};
 
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"NAME",item.NAME},
                        {"CODE",item.CODE},
                    };
                ja.Add(itemObject);
            }
            return ja;
        }

        //取回區
        public JArray getZIP(String ZIP)
        {
            JArray ja = new JArray();
            var query = from t in db.IMC_CODE where t.KIND == "COMMON" && t.TYPE == "ZIP" && t.CODE== ZIP select new { CODE = t.CODE, NAME = t.CITEM1 };

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"NAME",item.NAME},
                        {"CODE",item.CODE},
                    };
                ja.Add(itemObject);
            }
            return ja;
        }

    }
}