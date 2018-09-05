using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HowardCoupon_vs2010.Models.Edmx;
using System.Linq.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;
using HowardCoupon.Poco;

namespace HowardCoupon.Models.Dao.impl
{
    public class MenuDaoImpl:IMenuDao
    {
        private IntraEntities db = new IntraEntities();
        /**          
         * 依權限取回選單(web上方主選單)
         * */
        public List<Hashtable> getMenuItem(String userid, String[] roleid)
        {
            if (roleid == null)
            {
                roleid=new String[0];
            }
           // var query = from t in db.IMC_CODE where t.KIND == "COUPON" && t.TYPE=="MENU" orderby t.CITEM1,t.CITEM2 select t;

            var query = (from a in db.COUPON_MENU_ROLE_VIEW where roleid.Contains(a.ROLEID) || a.LEVEL2 ==0 select new { LEVEL1 = a.LEVEL1, LEVEL2 = a.LEVEL2, MENUITEM = a.MENUITEM, URL = a.URL,TARGET=a.TARGET }).Union(from b in db.COUPON_MENU_USER_VIEW where b.USERID == userid select new { LEVEL1 = b.LEVEL1, LEVEL2 = b.LEVEL2, MENUITEM = b.MENUITEM, URL = b.URL,TARGET=b.TARGET }).OrderBy(x => x.LEVEL1).ThenBy(x=>x.LEVEL2);

           
            List<Hashtable> list = new List<Hashtable>();
            foreach (var item in query)
            {
                Hashtable ht = new Hashtable();
                ht.Add("MENUITEM",StringUtils.getString(item.MENUITEM));
                ht.Add("URL", StringUtils.getString(item.URL));
                ht.Add("TARGET", StringUtils.getString(item.TARGET));
                ht.Add("LV1", item.LEVEL1);
                ht.Add("LV2", item.LEVEL2);
                list.Add(ht);
            }
            return list;
        }

        public JObject getMenuList()
        {
            var query =(from t in db.COMMON_MENU where t.KIND=="COUPON" && t.LEVEL2==0  select t).OrderBy(o1 => o1.LEVEL1).ThenBy(o2 => o2.LEVEL2);

          //  query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序 

            JArray ja = new JArray();
            
            JObject jo = new JObject();
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},                 
                        {"ID",item.LEVEL1 + item.LEVEL2}, 
                        {"MENUITEM",item.MENUITEM},
                        {"URL",item.URL},
                        {"TARGET",item.TARGET},
                        {"ENABLE",item.ENABLE},
                        {"LEVEL1",item.LEVEL1},
                        {"LEVEL2",item.LEVEL2},
                        {"USERID",item.USERID},                        
                        {"ROLEID",item.ROLEID},
                        {"MD_USER",item.MD_USER}, 
                        {"MD_DT",String.Format("{0:yyyy-MM-dd HH:mm}",item.MD_DT)}, 
                        {"CR_USER",item.CR_USER}, 
                        {"CR_DT",String.Format("{0:yyyy-MM-dd HH:mm}",item.CR_DT)},                            
                    }; 
                //ja.Add(itemObject);

                var query2 = (from t in db.COMMON_MENU where t.KIND == "COUPON" && t.LEVEL2 != 0 && t.LEVEL1==item.LEVEL1  select t).OrderBy(o1 => o1.LEVEL1).ThenBy(o2 => o2.LEVEL2);
                JArray ja2 = new JArray();
                foreach (var item2 in query2)
                {
                    var itemObject2 = new JObject
                    {   
                        {"UUID",item2.UUID},                
                        {"ID",item2.LEVEL1 + item2.LEVEL2}, 
                        {"MENUITEM",item2.MENUITEM},
                        {"URL",item2.URL},
                        {"TARGET",item2.TARGET},
                        {"ENABLE",item2.ENABLE},
                        {"LEVEL1",item2.LEVEL1},
                        {"LEVEL2",item2.LEVEL2},
                        {"USERID",item2.USERID},
                        {"ROLEID",item2.ROLEID},    
                        {"MD_USER",item2.MD_USER}, 
                        {"MD_DT",String.Format("{0:yyyy-MM-dd HH:mm}",item2.MD_DT)}, 
                        {"CR_USER",item2.CR_USER}, 
                        {"CR_DT",String.Format("{0:yyyy-MM-dd HH:mm}",item2.CR_DT)},
                    };
                    ja2.Add(itemObject2);
                }
             
                itemObject.Add("children", ja2);
                ja.Add(itemObject);

            }
            jo.Add("UUID", "00");  
            jo.Add("ID", "00");            
            jo.Add("MENUITEM", "福華兌換券");
            jo.Add("URL","");
            jo.Add("TARGET", "0");
            jo.Add("ENABLE", "1");
            jo.Add("LEVEL1","0");
            jo.Add("LEVEL2", "0");
            jo.Add("USERID", "");
            jo.Add("ROLEID", "");
            jo.Add("children", ja);
            return jo;
        }


        public JObject getRoleid(String MenuUUID)
        {
            JObject jo=new JObject();
            Decimal uuid =StringUtils.getDecimal(MenuUUID);
            /*
            var query = from t in db.COUPON_MENU_ROLE_VIEW where t.UUID == uuid 
                        join b in db.IMC_CODE on t.ROLEID.co
                         from c in imcc.DefaultIfEmpty()*/
                        /*
            foreach (var item in query)
            {

            }*/
            return jo;
        }

        public JObject getUserid(String MenuUUID)
        {
            JObject jo = new JObject();
            Decimal uuid = StringUtils.getDecimal(MenuUUID);
            return jo;
        }

        public JObject getCodeList(EasyuiParamPoco param ,String UUID){
            JObject jo = new JObject();
            JArray ja = new JArray();
            Decimal uuid = StringUtils.getDecimal(UUID);
            var query =
                      from i in db.IMC_CODE                                            
                      where !(
                        from t in db.IMC_CODE
                            join a in db.COUPON_MENU_ROLE_VIEW on t.CODE equals a.ROLEID into code
                            from aa in code
                            where t.KIND == "COUPON" && t.TYPE == "ROLE" && aa.UUID== uuid
                      select t.CODE).Contains(i.CODE) && i.KIND=="COUPON"  && i.TYPE=="ROLE"                    
                      select new { NAME = i.NAME, CODE = i.CODE,UUID =i.UUID};
            
           // query = query.OrderBy(q => Int32.Parse(q.CODE));
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序   
            
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},
                        {"NAME",item.NAME},
                        {"CODE",item.CODE},
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", ja.Count());
            return jo;
        }


        public JObject getMenuRoleList(EasyuiParamPoco param ,String UUID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            Decimal uuid = StringUtils.getDecimal(UUID);
            var query = from t in db.IMC_CODE
                        join a in db.COUPON_MENU_ROLE_VIEW on t.CODE equals a.ROLEID into code
                        from aa in code
                        where t.KIND == "COUPON" && t.TYPE == "ROLE" && aa.UUID== uuid                        
                        select new {CODE_NAME = t.NAME, CODE = t.CODE ,UUID=t.UUID};
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序 
         //   query = query.OrderBy(q => Int32.Parse(q.CODE));
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},                 
                        {"NAME",item.CODE_NAME}, 
                        {"CODE",item.CODE},
                        
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", ja.Count());
            return jo;
        }


        public JObject getMenuUserList(EasyuiParamPoco param, String UUID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            Decimal uuid = StringUtils.getDecimal(UUID);
            /*
            var query = from a in db.COUPON_MENU_USER_VIEW
                        join t in db.COUPON_EMPLOYEE on a.USERID equals t.USERID into code
                        from aa in code
                        join b in db.I_DEPARTMENT_VIEW on aa.ENTID + "-" + aa.DPTID equals b.CODE into dept
                        from bb in dept
                        where  a.UUID == uuid
                        select new { USERNAME = aa.USERNAME, USERID = aa.USERID,EMAIL=aa.EMAIL,DEPTID= bb.CODE,DEPTNAME=bb.NAME, UUID = a.UUID };
             */
            var query = from a in db.COUPON_MENU_USER_VIEW
                        join t in db.COUPON_EMPLOYEE on a.USERID equals t.USERID into code
                        from aa in code
                        join b in db.COUPON_DEPARTMENT on aa.DPTID equals b.CODE into dept
                        from bb in dept
                        where a.UUID == uuid
                        select new { USERNAME = aa.USERNAME, USERID = aa.USERID, EMAIL = aa.EMAIL, DEPTID = bb.CODE, DEPTNAME = bb.NAME, UUID = a.UUID };

            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序 
            //   query = query.OrderBy(q => Int32.Parse(q.CODE));
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},                 
                        {"USERNAME",item.USERNAME}, 
                        {"USERID",item.USERID},
                        {"EMAIL",item.EMAIL},
                        //{"DEPT",item.DEPTID.Substring(3,4) + " "+ item.DEPTNAME},
                        {"DEPT",item.DEPTID + " "+ item.DEPTNAME},
                        
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);            
            return jo;
        }


    }
}