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
    public class CompanyService
    {
        
        private IntraEntities db = new IntraEntities();

 


        public JObject getCompany(EasyuiParamPoco param,String CODE)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();         
            var query = from t in db.COUPON_COMPANY select t;

            if (CODE != null)
            {
                query.Where(t => t.CODE == CODE);  
            }

            jo.Add("total", query.Count());
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},
                        {"CODE",item.CODE},
                        {"NAME",item.NAME},
                        {"FNAME",item.FNAME},
                        {"ISLOCK",item.ISLOCK},
                        {"TAXNO",item.TAXNO},                              
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            
            return jo;
        }



 

        public int doSave(CompanyPoco param)
        {
            JObject jo = new JObject();
            int i = 0;
            using (db)
            {
                COUPON_COMPANY obj;
                if (param.UUID != null)
                {
                    Decimal uuid = StringUtils.getDecimal(param.UUID);
                    obj = (from t in db.COUPON_COMPANY where t.UUID == uuid select t).Single();
                }
                else
                {
                    obj = new COUPON_COMPANY();
                }
                obj.NAME = param.NAME;
                obj.FNAME = param.FNAME;
                obj.EMAIL = param.EMAIL;
                obj.TAXNO = param.TAXNO;
                if(param.UUID==null){
                    db.COUPON_COMPANY.AddObject(obj);
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
                COUPON_COMPANY obj = (from t in db.COUPON_COMPANY where t.UUID == uuid select t).Single();
                db.DeleteObject(obj);
                i= db.SaveChanges();
            }
           
            return i;
        }

        public int chkCode(String CODE)
        {            
            var query= from t in db.COUPON_COMPANY where t.CODE==CODE select t;
            return query.Count();
        }
  


    }
}