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
    public class IsuCheckService
    {
        
        private IntraEntities db = new IntraEntities();




        public JObject getIsu(CouponIsuPoco param, String UUID, String COMP, String DEPART, String QueryMode)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();  
            string[] strArray = new string [] { "2"};
            var query = from t in db.COUPON_ISU_VIEW  select t;

            if (UUID != null)
            {
                Decimal uuid = StringUtils.getDecimal(UUID);
                query = query.Where(t => t.UUID == uuid && strArray.Contains(t.STATUS));
            }
            else
            {
                query = query.Where(t => strArray.Contains(t.STATUS));
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


        /*
        public int doSave(CouponIsuPoco param)
        {
            JObject jo = new JObject();
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
                obj.ACTAMT = StringUtils.getDecimal(param.ACTAMT);
                obj.ADPTID = param.ADPTID;
                obj.AENTID = param.AENTID;
                if (param.STATUS == "2")
                {
                    obj.APPLY_DT = System.DateTime.Now;
                }
                else if (param.STATUS == "3")
                {
                    obj.APPLY_DT = System.DateTime.Now;
                }
                obj.AUSERID = param.AUSERID;
                if (StringUtils.getString(param.CANCEL_DT) != "")
                {
                    obj.CANCEL_DT = DateTimeUtil.getDateTime(param.CANCEL_DT);
                }
                obj.CCID = param.CCID;
                if (StringUtils.getString(param.CANCEL_DT) != "")
                {
                    obj.CHECK_DT = DateTimeUtil.getDateTime(param.CHECK_DT);
                }
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
                if (StringUtils.getString(param.ACTIVE_DT) != "")
                {
                    obj.ACTIVE_DT = DateTimeUtil.getDateTime(param.ACTIVE_DT);
                }
                obj.IENTID = param.IENTID;
                obj.INVOICE = param.INVOICE;
                obj.INVOICE_TYPE = param.INVOICE_TYPE;
                obj.IUSERID = param.IUSERID;
                obj.MCHGRATE = StringUtils.getDecimal(param.MCHGRATE);
                obj.PAY_CODE = StringUtils.getString(param.PAY_CODE) == "" ? "0" : param.PAY_CODE;
                obj.QTY = StringUtils.getDecimal(param.QTY);
                obj.REF_CODE = StringUtils.getString(param.REF_CODE) == "" ? "*" : param.REF_CODE;
                obj.REF2_CODE = StringUtils.getString(param.REF2_CODE) == "" ? "*" : param.REF2_CODE;
                obj.REJECT_DT = DateTimeUtil.getDateTime(param.REJECT_DT);
                obj.REMARK = param.REMARK;
                obj.SCHGRATE = StringUtils.getDecimal(param.SCHGRATE);

                obj.STATUS = param.STATUS;
                obj.SUBMIT_DT = DateTimeUtil.getDateTime(param.SUBMIT_DT);
                obj.TEMPLATE = param.TEMPLATE;
                obj.UPRC = StringUtils.getDecimal(param.UPRC);
                obj.USE_CODE = StringUtils.getString(param.USE_CODE) == "" ? "0" : param.USE_CODE;

                if (param.UUID == null)
                {
                    db.COUPON_ISU.AddObject(obj);
                }

                i = db.SaveChanges();


            }

            return i;
        }

        */


    }
}