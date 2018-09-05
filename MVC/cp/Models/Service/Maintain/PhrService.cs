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


namespace HowardCoupon_vs2010.Models.Service
{
    public class PhrService
    {
        
        private IntraEntities db = new IntraEntities();

 


        public JObject getPhr(EasyuiParamPoco param,String UUID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();         
            var query = from t in db.COUPON_PHR select t;

            if (StringUtils.getString(UUID) != "")
            {
                Decimal uuid = StringUtils.getDecimal(UUID);
                query.Where(t => t.UUID==uuid);  
            }

            if (query.Count() > 0)
            {
                jo.Add("total", query.Count());
                if (StringUtils.getString(param.sort) != "")
                {
                    query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
                    query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
                }
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
                        {"CONTENT",item.CONTENT},
                        {"COMPANY_CODE",item.COMPANY_CODE},
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



 

        public int doSave(PhrPoco param)
        {
            JObject jo = new JObject();
            int i = 0;
            using (db)
            {
                COUPON_PHR obj;
                if (param.UUID != null)
                {
                    Decimal uuid = StringUtils.getDecimal(param.UUID);
                    obj = (from t in db.COUPON_PHR where t.UUID == uuid select t).Single();
                }
                else
                {
                    obj = new COUPON_PHR();
                }
                obj.CONTENT = param.CONTENT;
                obj.COMPANY_CODE = param.COMPANY_CODE;
                if(param.UUID==null){
                    db.COUPON_PHR.AddObject(obj);
                }
               i= db.SaveChanges();
            }
           
            return i;
        }


        public int doDelete(String UUID)
        {
            JObject jo = new JObject();
            int i = 0;
            using (db)
            {
                Decimal uuid = StringUtils.getDecimal(UUID);
                COUPON_PHR obj = (from t in db.COUPON_PHR where t.UUID == uuid select t).Single();
                db.DeleteObject(obj);
                i= db.SaveChanges();
            }
           
            return i;
        }

        /*
        public int chkCode(String CODE)
        {
            var query = from t in db.IMC_CODE where t.KIND=="COUPON" && t.TYPE=="REF" && t.CODE == CODE select t;
            return query.Count();
        }*/
  


    }
}