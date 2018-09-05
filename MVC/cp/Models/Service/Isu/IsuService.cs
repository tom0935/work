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
using System.Data;

namespace HowardCoupon_vs2010.Models.Service
{
    public class IsuService
    {
        
        private IntraEntities db = new IntraEntities();




        public JObject getIsu(CouponIsuPoco param, String UUID, String COMP, String DEPART, String QueryMode, String SEARCH)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();  
            string[] strArray = new string [] { "1"};
            var query = from t in db.COUPON_ISU_VIEW  select t;
            if (QueryMode == "1")
            {
                DateTime sdt = DateTimeUtil.getDateTime(param.SDT + " 00:00:00");
                DateTime edt = DateTimeUtil.getDateTime(param.EDT + " 23:59:59");

                if (StringUtils.getString(COMP) != "")
                {
                    query = query.Where(t => t.AENTID == COMP && t.ADPTID == DEPART );
                }

                query = query.Where(q => q.APPLY_DTD >= sdt && q.APPLY_DTD <= edt);
                if (param.STATUS != "*")
                {
                    query = query.Where(q => q.STATUS == param.STATUS);
                }
                if (StringUtils.getString(param.CCID) != "")
                {
                    query = query.Where(q => q.CCID == param.CCID);
                }
                if (StringUtils.getString(param.IENTID) != "")
                {
                    query = query.Where(q => q.IENTID == param.IENTID);
                }
                if (StringUtils.getString(SEARCH) != "")
                {
                    query = query.Where(q => q.CUS_CMP_NAME.Contains(SEARCH) || q.CUS_NAME.Contains(SEARCH));
                }


            }
            else
            {
                if (UUID != null)
                {
                    Decimal uuid = StringUtils.getDecimal(UUID);
                    query = query.Where(t => t.UUID == uuid && strArray.Contains(t.STATUS));
                }
                else
                {
                    query = query.Where(t => t.AENTID == COMP && t.ADPTID == DEPART && strArray.Contains(t.STATUS));
                }
            }

            if (query.Count() > 0)
            {
                jo.Add("total", query.Count());
                if (param.sort != null)
                {
                    query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
                }
                if (param.rows > 0)
                {
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
                        {"ACTAMT",item.ACTAMT},
                        {"ADPTID",item.ADPTID},
                        {"AENTID",item.AENTID},
                        {"APPLY_DT",item.APPLY_DT},
                        {"AUSERID",item.AUSERID},
                        {"CANCEL_DT",item.CANCEL_DT},
                        {"CCID",item.CCID},
                        {"CHECK_DT",item.CHECK_DT},                        
                        {"CONTENT1",item.CONTENT1}, 
                        {"CONTENT2",item.CONTENT2}, 
                        {"CONTENT3",item.CONTENT3}, 
                        {"CONTENT4",item.CONTENT4}, 
                        {"CONTENT5",item.CONTENT5}, 
                        {"CONTENT6",item.CONTENT6}, 
                        {"CONTRACT_EDT",item.CONTRACT_EDT}, 
                        {"CONTRACT_SDT",item.CONTRACT_SDT}, 
                        {"CUS_CMP_UUID",item.CUS_CMP_UUID},
                        {"CUS_UUID",item.CUS_UUID},
                        {"EDT",item.EDT},
                        {"ACTIVE_DT",item.ACTIVE_DT},
                        {"IENTID",item.IENTID},
                        {"INVOICE",item.INVOICE},
                        {"INVOICE_TYPE",item.INVOICE_TYPE},
                        {"IUSERID",item.IUSERID},
                        {"MCHGRATE",item.MCHGRATE},
                        {"PAY_CODE",item.PAY_CODE},
                        {"QTY",item.QTY},
                        {"REF_CODE",item.REF_CODE},
                        {"REF2_CODE",item.REF2_CODE},
                        {"REJECT_DT",item.REJECT_DT},
                        {"REMARK",item.REMARK},
                        {"SCHGRATE",item.SCHGRATE},
                        {"SDT",item.SDT},
                        {"STATUS",item.STATUS},
                        {"SUBMIT_DT",item.SUBMIT_DT},
                        {"TEMPLATE",item.TEMPLATE},
                        {"UPRC",item.UPRC},
                        {"USE_CODE",item.USE_CODE},                        
                        {"USE_NAME",item.USE_NAME},  
                        {"AUSER_NAME",item.AUSER_NAME},   
                        {"AENT_NAME",item.AENT_NAME},   
                        {"ADPT_NAME",item.ADPT_NAME},   
                        {"CUS_CMP_NAME",item.CUS_CMP_NAME},   
                        {"CUS_NAME",item.CUS_NAME},   
                        {"IUSER_NAME",item.IUSER_NAME},   
                        {"IENT_NAME",item.IENT_NAME},   
                        {"CC_NAME",item.CC_NAME}, 
                        {"CC_PRC",item.CC_PRC}, 
                        {"CC_MCHGRATE",item.CC_MCHGRATE}, 
                        {"CC_SCHGRATE",item.CC_SCHGRATE}, 
                        {"CC_TEMPLATE",item.CC_TEMPLATE}, 
                        {"CC_NEW_TEMPLATE",item.CC_NEW_TEMPLATE},
                        {"CC_TYPE",item.CC_TYPE}, 
                        {"STATUS_NAME",item.STATUS_NAME}, 
                        {"CMPID",item.CMPID}, 
                        {"APPLY_TEMPLATE",item.APPLY_TEMPLATE}, 
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


        public int doSaveCI(CouponIsuPoco param)
        {
            int x = 0;
            using (IntraEntities context = new IntraEntities())
            {
                Decimal uuid = StringUtils.getDecimal(param.UUID);
              
                var query = from t in context.COUPON_CI where t.UUID == uuid select t;
                
                if (query.Count() == 0  )
                {
                    for (int i = 0; i <= StringUtils.getDecimal(param.QTY); i++)
                    {                        
                        COUPON_CI obj = new COUPON_CI();
                        obj.ISUID = uuid;
                        obj.IENTID = param.IENTID;
                        obj.CCID = param.CCID;
                        obj.SEQNO = i;
                        obj.CHKNO = i;
                        context.COUPON_CI.AddObject(obj);
                        
                        
                    }
                   
                   x= context.SaveChanges();
                }
                
            }
            return x;
        }


        public JObject doSave(CouponIsuPoco param)
        {
            JObject jo = new JObject();
            JObject jo1 = new JObject();
            int i = 0;

            using (db)
            {
                
                
                COUPON_ISU obj;
                if (param.UUID != null)
                {
                    Decimal uuid = StringUtils.getDecimal(param.UUID);
                    obj = (from t in db.COUPON_ISU where t.UUID == uuid select t).Single();
                }
                else
                {
                    obj = new COUPON_ISU();
                    
                }
                obj.ACTAMT =StringUtils.getDecimal(param.ACTAMT);
                obj.ADPTID = param.ADPTID;
                obj.AENTID = param.AENTID;
                
                if (param.STATUS == "2")
                {
                    
                    obj.SUBMIT_DT = System.DateTime.Now;       //提出申請
                   
                }else if(param.STATUS == "3"){
                    obj.CHECK_DT = System.DateTime.Now;    //審核  
                    obj.IUSERID = param.IUSERID;
                    db.PROC_CI_CREATE(obj.UUID);           //審核產生兌換券主檔資料

                }else if(param.STATUS == "4"){
                    obj.ACTIVE_DT = System.DateTime.Now;    //已發行
                }
                else if (param.STATUS == "0")
                {
                    obj.REJECT_DT = System.DateTime.Now;    //取消駁回
                }
                obj.APPLY_DT = DateTimeUtil.getDateTime(param.APPLY_DT);
                obj.AUSERID = param.AUSERID;

                

                obj.CONTENT1 = param.CONTENT1;
                obj.CONTENT2 = param.CONTENT2;
                obj.CONTENT3 = param.CONTENT3;
                obj.CONTENT4 = param.CONTENT4;
                obj.CONTENT5 = param.CONTENT5;
                obj.CONTENT6 = param.CONTENT6;
                if (StringUtils.getString(param.CONTRACT_EDT) != "")
                {
                    obj.CONTRACT_EDT = DateTimeUtil.getDateTime(param.CONTRACT_EDT);
                }
                if (StringUtils.getString(param.CONTRACT_SDT) != "")
                {
                    obj.CONTRACT_SDT = DateTimeUtil.getDateTime(param.CONTRACT_SDT);
                }
                obj.CUS_CMP_UUID = StringUtils.getDecimal(param.CUS_CMP_UUID);
                obj.CUS_UUID = StringUtils.getDecimal(param.CUS_UUID);
                if (StringUtils.getString(param.EDT) != "")
                {
                    obj.EDT = DateTimeUtil.getDateTime(param.EDT);
                }
                if (StringUtils.getString(param.SDT) != "")
                {
                    obj.SDT = DateTimeUtil.getDateTime(param.SDT);
                }

                obj.IENTID = param.IENTID;
                obj.INVOICE = param.INVOICE;
                obj.INVOICE_TYPE = param.INVOICE_TYPE;
                
                obj.MCHGRATE = StringUtils.getDecimal(param.MCHGRATE);
                obj.PAY_CODE = StringUtils.getString(param.PAY_CODE) == "" ? "0" : param.PAY_CODE;
                obj.QTY  = StringUtils.getDecimal(param.QTY);
                obj.REF_CODE = StringUtils.getString(param.REF_CODE) == "" ? "*" : param.REF_CODE;
                obj.REF2_CODE = StringUtils.getString(param.REF2_CODE) == "" ? "*" : param.REF2_CODE;             
                obj.REMARK = param.REMARK;
                obj.SCHGRATE = StringUtils.getDecimal(param.SCHGRATE);
                if (StringUtils.getString(param.STATUS) != "")
                {
                    obj.STATUS = param.STATUS;
                }
               
                obj.TEMPLATE = param.TEMPLATE;
                obj.UPRC =StringUtils.getDecimal(param.UPRC);
                obj.USE_CODE = StringUtils.getString(param.USE_CODE) == "" ? "0" : param.USE_CODE;
                if (StringUtils.getString(param.USE_CODE) == "3" && StringUtils.getString(param.CCID) == "12")    //若選擇連鎖住宿券(12)且為贈送零送(3),則改為券別為連鎖住宿券甑送零送(44)
                {
                    obj.CCID = "44";
                }
                else if (StringUtils.getString(param.USE_CODE) != "3" && StringUtils.getString(param.CCID) == "44") //若選擇連鎖住宿券甑送零送(44)且不為贈送零送(3),則改為券別為連鎖住宿券(12)
                {
                    obj.CCID = "12";
                }
                else
                {
                    obj.CCID = param.CCID;
                }


                if(param.UUID==null){
                    db.COUPON_ISU.AddObject(obj);
                }
                
               i= db.SaveChanges();
                Decimal puuid;
               if (param.UUID == null){
                   puuid = (from t in db.COUPON_ISU select t).Max(x => x.UUID);
               }else{
                   puuid = StringUtils.getDecimal(param.UUID);
               }                   
                   COUPON_ISU_VIEW result2 = (from t in db.COUPON_ISU_VIEW where t.UUID == puuid select t).Single();
                  jo=new JObject{{"UUID", result2.UUID.ToString()},{"CC_NEW_TEMPLATE", result2.CC_NEW_TEMPLATE}};    
            }

            return jo;
        }


        public int doCopy(String UUID,String userid ,String AENTID,String ADPTID)
        {
            JObject jo = new JObject();
            int i = 0;
            using (db)
            {
                Decimal uuid = StringUtils.getDecimal(UUID);
                var query = (from t in db.COUPON_ISU where t.UUID == uuid select t).Single();
                COUPON_ISU obj = new COUPON_ISU();
                DateTime currentTime = new DateTime();
                currentTime = System.DateTime.Now;

                if (query != null)
                {                    
                    obj.APPLY_DT = currentTime;
                    obj.AUSERID = userid;
                    obj.AENTID = AENTID;
                    obj.STATUS = "1";
                    obj.ADPTID = ADPTID;                    

                    //copy
                    obj.ACTAMT = query.ACTAMT;
                    obj.CCID = query.CCID;
                    obj.CONTENT1 = query.CONTENT1;
                    obj.CONTENT2 = query.CONTENT2;
                    obj.CONTENT3 = query.CONTENT3;
                    obj.CONTENT4 = query.CONTENT4;
                    obj.CONTENT5 = query.CONTENT5;
                    obj.CONTENT6 = query.CONTENT6;
                    obj.CUS_CMP_UUID = query.CUS_CMP_UUID;
                    obj.CUS_UUID = query.CUS_UUID;
                    obj.IENTID = query.IENTID;
                    obj.PAY_CODE = query.PAY_CODE;
                    obj.REF_CODE = query.REF_CODE;
                    obj.REF2_CODE = query.REF2_CODE;
                    obj.INVOICE_TYPE = query.INVOICE_TYPE;
                    obj.REMARK = query.REMARK;
                    obj.UPRC = query.UPRC;
                    obj.USE_CODE = query.USE_CODE;
                    obj.TEMPLATE = query.TEMPLATE;
                    obj.QTY = query.QTY;
                    obj.SDT = query.SDT;
                    obj.EDT =query.EDT;
                }
                db.COUPON_ISU.AddObject(obj);
                i = db.SaveChanges();
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
                COUPON_ISU obj = (from t in db.COUPON_ISU where t.UUID == uuid select t).Single();
                db.DeleteObject(obj);
                i= db.SaveChanges();
            }
           
            return i;
        }

        public DataTable getIsuView(String UUID)
        {
            LinqExtensions le = new LinqExtensions();
            //var query = (from t in orderDb.CAKE_ORDERS_DTL_FIX where t.STATUS == "1" && t.COMP == COMP select t);
            Decimal uuid = StringUtils.getDecimal(UUID);
            var query = from t in db.COUPON_ISU_VIEW where t.UUID==uuid select t;
            int x = query.Count();
            DataTable dt = le.LinqQueryToDataTable(query);
            return dt;
        }


        public DataTable getIsuCiView(String UUID)
        {
            LinqExtensions le = new LinqExtensions();
            //var query = (from t in orderDb.CAKE_ORDERS_DTL_FIX where t.STATUS == "1" && t.COMP == COMP select t);
            Decimal uuid = StringUtils.getDecimal(UUID);
            var query = from t in db.COUPON_CI_VIEW where t.ISUID==uuid select t;
            int x = query.Count();
            DataTable dt = le.LinqQueryToDataTable(query);
            return dt;
        }


        /*
        public int chkCode(String CID)
        {
            var query = from t in db.COUPON_ISU where t.CID == CID select t;
            return query.Count();
        }
       */


    }
}