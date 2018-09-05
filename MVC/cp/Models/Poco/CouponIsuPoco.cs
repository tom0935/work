using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  HowardCoupon.Poco
{     
    public class CouponIsuPoco :EasyuiParamPoco
    {
        public String UUID { get; set; }
        public String ACTAMT { get; set; }      //實收總額
        public String ADPTID { get; set; }      //申請部門
        public String AENTID { get; set; }      //申請公司
        public String APPLY_DT { get; set; }    //申請日期  
        public String AUSERID { get; set; }     //申請人員
        public String CANCEL_DT { get; set; }   //作廢日期
        public String CCID { get; set; }        //券別 
        public String CHECK_DT { get; set; }    //審核日期
        public String CONTENT1 { get; set; }    //說明1
        public String CONTENT2 { get; set; }
        public String CONTENT3 { get; set; }
        public String CONTENT4 { get; set; }
        public String CONTENT5 { get; set; }
        public String CONTENT6 { get; set; }
        public String CONTRACT_EDT { get; set; }    //履約迄日
        public String CONTRACT_SDT { get; set; }    //履約起日
        public String CUS_CMP_UUID { get; set; }    //客戶公司UUID
        public String CUS_UUID { get; set; }        //客戶UUID 
        public String EDT { get; set; }             //有效迄日
        public String ACTIVE_DT { get; set; }          //生效日期
        public String IENTID { get; set; }               //發行關係企業代碼
        public String INVOICE { get; set; }              //發票號碼
        public String INVOICE_TYPE { get; set; }         //發票種類    
        public String IUSERID { get; set; }              //核准人員代碼
        public String MCHGRATE { get; set; }             //手續費率 
        public String PAY_CODE { get; set; }             //付款方式 
        public String QTY { get; set; }                  //數量
        public String REF_CODE { get; set; }             //結帳方式1  
        public String REF2_CODE { get; set; }            //結帳方式2
        public String REJECT_DT { get; set; }             //駁回日期
        public String REMARK { get; set; }                //備註
        public String SCHGRATE { get; set; }             //服務費率
        public String SDT { get; set; }                    //有效起日
        public String STATUS { get; set; }                 //狀態
        public String SUBMIT_DT { get; set; }              //提交日期
        public String TEMPLATE { get; set; }              
        public String UPRC { get; set; }              //單價
        public String USE_CODE { get; set; }          //用途

    }
}