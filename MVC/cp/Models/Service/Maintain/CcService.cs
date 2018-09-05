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
    public class CcService
    {
        
        private IntraEntities db = new IntraEntities();

 


        public JObject getCc(EasyuiParamPoco param,String UUID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();         
            var query = from t in db.COUPON_CC  select t;

            if (UUID != null)
            {
                Decimal uuid = StringUtils.getDecimal(UUID);
                query.Where(t => t.UUID == uuid);  
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
                        {"CID",item.CID},
                        {"NAME",item.NAME},
                        {"SDT",item.SDT},
                        {"EDT",item.EDT},
                        {"PRC",item.PRC},
                        {"PROTECT",item.PROTECT},
                        {"TEMPLATE",item.TEMPLATE},
                        {"NEW_TEMPLATE",item.NEW_TEMPLATE},
                        {"TYPE",item.TYPE},                        
                        {"SCOPE",item.SCOPE},                        
                        {"SCHGRATE",item.SCHGRATE},
                        {"MCHGRATE",item.MCHGRATE},
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


        public JArray getCcList()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            var query= from t in db.COUPON_CC select t;
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"CID",item.CID},
                        {"NAME",item.NAME},  
                        {"TYPE",item.TYPE}, 
                        {"PRC",item.PRC},                         
                    };
                ja.Add(itemObject);
            }
            return ja;
        }

        public JArray getCcList(String[] notinStrArray)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            var query = from t in db.COUPON_CC where !notinStrArray.Contains(t.CID) select t;
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"CID",item.CID},
                        {"NAME",item.NAME},  
                        {"TYPE",item.TYPE}, 
                        {"PRC",item.PRC},                         
                    };
                ja.Add(itemObject);
            }
            return ja;
        }


        public int doSave(CcPoco param)
        {
            JObject jo = new JObject();
            int i = 0;
            using (db)
            {
                COUPON_CC obj;
                if (param.UUID != null)
                {
                    Decimal uuid = StringUtils.getDecimal(param.UUID);
                    obj = (from t in db.COUPON_CC where t.UUID == uuid select t).Single();
                }
                else
                {
                    obj = new COUPON_CC();
                }
                obj.NAME = param.NAME;
                obj.CID = param.CID;
                obj.PRC = StringUtils.getDecimal(param.PRC);
                obj.MCHGRATE = StringUtils.getDecimal(param.MCHGRATE);
                obj.SCHGRATE = StringUtils.getDecimal(param.SCHGRATE);
                obj.SDT = DateTimeUtil.getDateTime(param.SDT);
                obj.EDT = DateTimeUtil.getDateTime(param.EDT);
                obj.TYPE = param.TYPE;
                obj.TEMPLATE = param.TEMPLATE;
                obj.NEW_TEMPLATE = param.NEW_TEMPLATE;
                obj.SCOPE = param.SCOPE;
                obj.PROTECT = param.PROTECT;

                if(param.UUID==null){
                    db.COUPON_CC.AddObject(obj);
                }
               i= db.SaveChanges();
            }
           
            return i;
        }

        public int doSaveTemplate(String CID,String TEMPLATE)
        {
            int i = 0;
            using (db)
            {
                COUPON_CC obj;
                if (CID != null)
                {                    
                    obj = (from t in db.COUPON_CC where t.CID == CID select t).Single();
                    obj.NEW_TEMPLATE = "";
                    i = db.SaveChanges();
                }
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
                COUPON_CC obj = (from t in db.COUPON_CC where t.UUID == uuid select t).Single();
                db.DeleteObject(obj);
                i= db.SaveChanges();
            }
           
            return i;
        }

        public int chkCode(String CID)
        {
            var query = from t in db.COUPON_CC where t.CID == CID select t;
            return query.Count();
        }
  


    }
}