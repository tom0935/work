using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Web.Http;
using IntranetSystem.Service;
using IntranetSystem.Models;
using IntranetSystem.Poco;
using System.Web.Security;
using Microsoft.Reporting.WebForms;

namespace IntranetSystem.Cake
{
     
    public class CakeOrdersController : Controller
    {

        CakeOrdersService cakeOrdersService = new CakeOrdersService();
        CommonService commonService = new CommonService();
        //
        // GET: /Cake/


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderDtlDialog()
        {
            return View();
        }

        public ActionResult GridCount(CakeOrderParamPoco param)
        {
            int i= i = cakeOrdersService.getTotalCount(param);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

        public ActionResult Grid(CakeOrderParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            JArray ja = new JArray();
            int i = 0;
            try
            {
               /*
                if (param.MODE != null)
                {
                    
                    
                    if (param.MODE == "1")
                    {
                        i = cakeOrdersService.getProductSumCount(param);
                    }
                    else
                    {
                        i = cakeOrdersService.getTotalCount(param);
                    }
                }
                */

                if (param.MODE != null)
                {


                    if (param.MODE == "1")
                    {
                        jo = cakeOrdersService.getProductSumDatagrid(param, aid,userid);
                        jo.Add("Q", "Y");
                    }
                    else
                    {
                        jo = cakeOrdersService.getDatagrid(param);
                    }

                    jo.Add("status", "1");
                }
                else
                {
                    jo.Add("total", "");
                    jo.Add("rows", "");
                }
                
                
             //  }else{
             //   jo.Add("total", "0");
             //   jo.Add("rows", ja);
             //  }
               
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

                return Content(JsonConvert.SerializeObject(jo), "application/json");
            //    return Json(jo, JsonRequestBehavior.AllowGet);
            
            // 
        }

        public ActionResult OrderGrid(OrderParamPoco param,String STATUS)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = 0;
                Hashtable htUser = commonService.getUserInfo(aid);
                String DEPART_NAME = HashtableUtil.getValue(htUser,"DEPART_NAME");
                param.DEPART = HashtableUtil.getValue(htUser,"DEPART_CODE");
                param.COMP = HashtableUtil.getValue(htUser, "COMP_CODE");
                param.USERID = userid;
                if (param.ODDNO != null)
                {
                    param.ODDNO = param.ODDNO.ToUpper();
                }
                i = cakeOrdersService.getOrderCount(param);

                if (i > 0)
                {
                    jo.Add("total", i);
                    jo.Add("rows", cakeOrdersService.getOrderDatagrid(param,STATUS));                    
                }
                else
                {
                    jo.Add("total", "");
                    jo.Add("rows", "");                    
                }

                if (StringUtils.getString(param.MODE) == "2")
                {
                    jo.Add("title", DEPART_NAME + "(" + param.SDT + "~" + param.EDT + ")");
                }
                else
                {
                    jo.Add("title", DEPART_NAME + "(最近七日)");
                }

                //  }else{
                //   jo.Add("total", "0");
                //   jo.Add("rows", ja);
                //  }
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult OrderDetailGrid(OrderParamPoco param)
        {

            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = 0;
                 
                i = cakeOrdersService.getOrderDetailCount(param);

                if (i > 0)
                {
                    
                    //jo.Add("total", i);
                    jo= cakeOrdersService.getOrderDetailDatagrid(param,i);
                    
                }
                else
                {
                    jo.Add("total", "");
                    jo.Add("rows", "");
                }


                //  }else{
                //   jo.Add("total", "0");
                //   jo.Add("rows", ja);
                //  }
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getProduct(String POSPNO)
        {
            JObject jo = new JObject();
            try{
                jo.Add("rows",cakeOrdersService.getProduct(POSPNO));
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Combobox(String KIND,String CODE)
        {
            
            JArray jr = new JArray();
            JObject jo = new JObject();
            if (KIND == "1")
            {
                jr = cakeOrdersService.getCombobox1(KIND);
            }
            else
            {
                jr = cakeOrdersService.getCombobox(KIND, CODE);
            }
            return Content(JsonConvert.SerializeObject(jr), "application/json");
        }

        public ActionResult doSave(CAKE_ORDERS odders, CAKE_ORDERS_DTL dtl)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                int i = cakeOrdersService.doSave(htUser, odders, dtl);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult Save(Hashtable user, CAKE_ORDERS odders, List<CAKE_ORDERS_DTL> dtlList)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                int i = 0;
                //int i = cakeOrdersService.doSave(user, odders, dtlList);
                jo.Add("status", i);
            }
            catch(Exception e) {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult queryOrderDetail(String UUID)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            Decimal uuid = StringUtils.getDecimal(UUID);
            try
            {
                jo.Add("rows", cakeOrdersService.queryOrderDetail(uuid));                
            }
            catch(Exception e) {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        


        public ActionResult Remove(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                int i = cakeOrdersService.doRemove(CNO);
                jo.Add("status", i);
            }catch(Exception e){
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult submitOrder(String ODDNO,String SDATE)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                int i = cakeOrdersService.submitOrder(htUser, ODDNO,SDATE);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult cancelOrder(String ODDNO)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                int i = cakeOrdersService.cancelOrder(htUser, ODDNO);
                jo.Add("status", i);
            }catch(Exception e){
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult copyOrder(String ODDNO)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                int i = cakeOrdersService.copyOrder(htUser, ODDNO);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult RemoveDetail( String UUID)
        {
            JObject jo = new JObject();
            //取得使用者資料
            //FormsIdentity id = (FormsIdentity)User.Identity;
            //FormsAuthenticationTicket ticket = id.Ticket;
            //string aid = ticket.UserData;
            //string userid = User.Identity.Name;
            try
            {
              //  Hashtable htUser = commonService.getUserInfo(aid);
              //  htUser.Add("USERID", userid);
              //  htUser.Add("AID", aid);
                Decimal uuid =  StringUtils.getDecimal(UUID);
                int i = cakeOrdersService.doRemoveDetail( uuid);
                jo.Add("status", i);
            }catch(Exception e){
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doAddDetail(String UUID, String AQTY, String ADD_REMARK,String ODDNO)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            Decimal uuid = StringUtils.getDecimal(UUID);
            Decimal aqty= StringUtils.getDecimal(AQTY);
            int i = 0;
            i=cakeOrdersService.doAddDetail(uuid, aqty, ADD_REMARK,userid,ODDNO);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }



        public ActionResult getAddDetailList(String UUID,String QTY,EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            try
            {             
                jo= cakeOrdersService.getAddDetailList(UUID,QTY,param);                
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        
    }
}
