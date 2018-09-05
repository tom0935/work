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
    public class IsuPrintService
    {
        
        private IntraEntities db = new IntraEntities();




        public JObject getIsu(EasyuiParamPoco param ,String STATUS,String UUID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            var query = from t in db.COUPON_ISU_VIEW  select t;
            if (StringUtils.getString(UUID) != "")
            {
                Decimal uuid = StringUtils.getDecimal(UUID);
                query = query.Where(t => t.UUID == uuid && t.STATUS == STATUS);
            }
            else
            {
                query = query.Where(t => t.STATUS == STATUS);
            }
            //query = query.Where(t => t.STATUS == STATUS);

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

        //列印兌換券前先寫入狀態和生效日期
        public int doSave(String UUID,String TEMPLATE)
        {
            int i = 0;
            using (db)
            {
                COUPON_ISU obj;
                Decimal uuid = StringUtils.getDecimal(UUID);
                obj = (from t in db.COUPON_ISU where t.UUID == uuid select t).Single();
                if (obj.STATUS != "4")
                {
                    obj.ACTIVE_DT = System.DateTime.Now;    //審核
                    obj.STATUS = "4";
                    obj.APPLY_TEMPLATE = TEMPLATE;
                }
                i = db.SaveChanges();
            }         
            return i;
        }

        public List<Hashtable> getIsuCiView(String UUID)
        {

            List<Hashtable> list = new List<Hashtable>();
            Decimal uuid = StringUtils.getDecimal(UUID);
            
            var query = (from t in db.COUPON_CI_VIEW where t.ISUID == uuid select t).OrderBy(t=> t.SEQNO);
                        
            foreach (var item in query)
            {
                Hashtable ht = new Hashtable();
                ht.Add("BARCODE", item.BARCODE);
                ht.Add("SEQNO", item.SEQNO);
                ht.Add("REF_CODE", item.REF_CODE);
                ht.Add("REF2_CODE", item.REF2_CODE);
                ht.Add("INVOICE", item.INVOICE);
                ht.Add("UPRC_CODE", item.UPRC_CODE);
                ht.Add("ADPTID", item.ADPTID);
                list.Add(ht);
            }
            return list;
        }

    }
}