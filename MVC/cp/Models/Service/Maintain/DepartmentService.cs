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
    public class DepartmentService
    {
        
        private IntraEntities db = new IntraEntities();

 


        public JObject getDepartment(EasyuiParamPoco param,String CODE)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();         
            var query = from t in db.COUPON_DEPARTMENT select t;

            if (CODE != null)
            {
                query.Where(t => t.CODE == CODE);  
            }
            if (query.Count() > 0)
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
                        {"CODE",item.CODE},
                        {"NAME",item.NAME},
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



 

        public int doSave(DepartmentPoco param)
        {
            JObject jo = new JObject();
            int i = 0;
            using (db)
            {
                COUPON_DEPARTMENT obj;
                if (param.UUID != null)
                {
                    Decimal uuid = StringUtils.getDecimal(param.UUID);
                    obj = (from t in db.COUPON_DEPARTMENT where t.UUID == uuid select t).Single();
                }
                else
                {
                    obj = new COUPON_DEPARTMENT();
                }
                obj.NAME = param.NAME;
                obj.CODE = param.CODE;
                if(param.UUID==null){
                    db.COUPON_DEPARTMENT.AddObject(obj);
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
                COUPON_DEPARTMENT obj = (from t in db.COUPON_DEPARTMENT where t.UUID == uuid select t).Single();
                db.DeleteObject(obj);
                i= db.SaveChanges();
            }
           
            return i;
        }

        public int chkCode(String CODE)
        {
            var query = from t in db.COUPON_DEPARTMENT where t.CODE == CODE select t;
            return query.Count();
        }
  


    }
}