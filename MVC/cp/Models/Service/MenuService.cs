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

namespace HowardCoupon_vs2010.Models.Service
{
    public class MenuService
    {
        private IMenuDao menu=new MenuDaoImpl();
        private IntraEntities db = new IntraEntities();

        public JObject getMenuItem(String userid, String[] roleid)
        {

            JObject jo = new JObject();
            
            JArray ja = new JArray();
            JArray jat = new JArray();
           List<Hashtable> list= menu.getMenuItem(userid,roleid);
           String oldItem = "";
           String title = "";
           String newItem = "";
            String lv2 = "";
            int i=0;
            foreach (Hashtable ht in list)
            {
                newItem = HashtableUtil.getValue(ht, "LV1");    
                lv2 = HashtableUtil.getValue(ht, "LV2");    
                if (oldItem != newItem && i > 1)
                {
                     jo = new JObject();
                    jo.Add("TITLE", title);  
                    jo.Add("MENUS", ja);
                    jat.Add(jo);
                    ja = new JArray();                    
                }
                if (lv2 != "0")
                {
                    var itemObject = new JObject
                    {                                           
                        {"TITLE",HashtableUtil.getValue(ht,"MENUITEM")},
                        {"URL",HashtableUtil.getValue(ht,"URL")},
                        {"TARGET",HashtableUtil.getValue(ht,"TARGET")},
                        {"LV1",HashtableUtil.getValue(ht,"LV1")},                        
                        {"LV2",HashtableUtil.getValue(ht,"LV2")},    
                    };
                    ja.Add(itemObject);
                }
                else
                {
                    title = HashtableUtil.getValue(ht, "MENUITEM");
                }
                oldItem = HashtableUtil.getValue(ht, "LV1");
                
                i++;
            }
            jo = new JObject();
            jo.Add("TITLE", title);
            jo.Add("MENUS", ja);            
            jat.Add(jo);

            jo = new JObject();
            jo.Add("MENUS", jat);
            return jo;
        }


        public JObject getMenuList()
        {
            return menu.getMenuList();
        }

        //取回目前選取選單角色
        public JObject getMenuRoleList(EasyuiParamPoco param ,String UUID)
        {
            return menu.getMenuRoleList(param,UUID);
        }

        //取回目前選取人員清單
        public JObject getMenuUserList(EasyuiParamPoco param, String UUID)
        {
            return menu.getMenuUserList(param, UUID);
        }

        public JObject getCodeList(EasyuiParamPoco param ,String UUID){
            return menu.getCodeList(param,UUID);
        }

        public String getMenuLevel2(Decimal LEVEL1)
        {
            var query = from t in db.COMMON_MENU where t.KIND == "COUPON" && t.LEVEL1 == LEVEL1 select new { LEVEL2 = t.LEVEL2};            
            return (query.Max(q => q.LEVEL2)+1).ToString();
        }

        public String getMenuLevel1()
        {
            var query = from t in db.COMMON_MENU where t.KIND == "COUPON" && t.LEVEL2 == 0 select new {LEVEL1 = t.LEVEL1};
            return (query.Max(q => q.LEVEL1)+1).ToString();
        }


        public int addMenu(String USERID,String MENUITEM,String URL,String LEVEL1,String ENABLE,String TARGET)
        {
            
             DateTime currentTime = new DateTime();
             currentTime = System.DateTime.Now;
             int i=0;
            using (db)
            {
                COMMON_MENU obj = new COMMON_MENU();
                obj.KIND = "COUPON";
                if (LEVEL1 == null || LEVEL1=="0")
                {
                    LEVEL1 = getMenuLevel1();
                }
                obj.LEVEL1 = StringUtils.getDecimal(LEVEL1);

                obj.LEVEL2 = StringUtils.getDecimal(getMenuLevel2(StringUtils.getDecimal(LEVEL1))) ;
                obj.MENUITEM = MENUITEM;
                obj.URL = URL;
                obj.ENABLE = ENABLE;
                obj.TARGET = TARGET;
                obj.CR_DT = currentTime;
                obj.CR_USER = USERID;
                 db.COMMON_MENU.AddObject(obj);
                i= db.SaveChanges();
            }            
            return i;
        }


        public int delMenu(String UUID, String LEVEL1,String LEVEL2)
        {
            int i=0;
            using (db)
            {                
                
                if (LEVEL2 == "0")
                {
                   i= db.ExecuteStoreCommand("delete from intra.COMMON_MENU t where t.KIND='COUPON' and t.LEVEL2 = 0 and t.LEVEL1 = " + StringUtils.getDecimal(LEVEL1)); 
                }else{
                    COMMON_MENU obj=new COMMON_MENU();
                    Decimal uuid=StringUtils.getDecimal(UUID);
                    obj = (from t in db.COMMON_MENU where t.UUID == uuid select t).Single();
                    db.COMMON_MENU.DeleteObject(obj);
                    i=db.SaveChanges();
                }
            }
            return i;
        }


        public int saveMenu(String USERID,String UUID, String MENUITEM, String URL, String ENABLE,String TARGET, String LEVEL1, String LEVEL2,String OLDLEVEL1)
        {
            int i=0;
             DateTime currentTime = new DateTime();
             currentTime = System.DateTime.Now;
            using (db)
            {
                if (LEVEL2 == "0")
                {
                    String sql = "update intra.COMMON_MENU set LEVEL1="+LEVEL1;
                    sql += " where KIND='COUPON' and LEVEL1 = " + StringUtils.getDecimal(OLDLEVEL1);
                    i = db.ExecuteStoreCommand(sql);
                }
                    COMMON_MENU obj=new COMMON_MENU();
                    Decimal uuid=StringUtils.getDecimal(UUID);
                    obj = (from t in db.COMMON_MENU where t.UUID == uuid select t).Single();
                    obj.MENUITEM = MENUITEM;
                    obj.URL=URL;
                    obj.ENABLE=ENABLE;
                    obj.TARGET = TARGET;
                    if (LEVEL2 != "0")
                    {
                        obj.LEVEL1 = StringUtils.getDecimal(LEVEL1);
                        obj.LEVEL2 = StringUtils.getDecimal(LEVEL2);
                    }
                    obj.MD_DT=currentTime;
                    obj.MD_USER=USERID;                    
                  i= db.SaveChanges();                  
                
            }
            return i;
        }

        //設定角色權限
        public int setRole(String userid,List<CommonDataGridPoco> list,String MenuUUID)
        {
           int i=0;
           DateTime currentTime = new DateTime();
           currentTime = System.DateTime.Now;
            
            Decimal uuid=StringUtils.getDecimal(MenuUUID);
            using (db)
            {             
            var obj = (from t in db.COMMON_MENU where t.UUID == uuid select t).Single();
            String str =obj.ROLEID;

            List<string> code = new List<string>();
            foreach (CommonDataGridPoco item in list)
            {
                code.Add(item.CODE.ToString());
            }
            String[] strAry = code.ToArray();
                
            if (str!="")
            {
                str += "," + String.Join(",", strAry);                            
                
            }
            else
            {
                str += String.Join(",", strAry);
            }
            
                obj.ROLEID = str;
                obj.MD_DT = currentTime;
                obj.MD_USER = userid;
                i = db.SaveChanges();
           }
            return i;
        }





        //撤回角色權限
        public int getRole(String userid, List<CommonDataGridPoco> list, String MenuUUID)
        {
            int i = 0;
            DateTime currentTime = new DateTime();
            currentTime = System.DateTime.Now;

            Decimal uuid = StringUtils.getDecimal(MenuUUID);
            List<string> code = new List<string>();
            foreach (CommonDataGridPoco item in list)
            {
                code.Add(item.CODE.ToString());
            }
            using (db)
            {
                var obj = (from t in db.COMMON_MENU where t.UUID == uuid select t).Single();

                String[] aryStr = obj.ROLEID.Split(',');
                List<string> aryList = new List<string>(aryStr);                
                var q= (from t in aryList where !code.Contains(t) select t).ToArray();                             
               String joinstr = String.Join(",", q);
               obj.ROLEID = joinstr;
               obj.MD_DT = currentTime;
               obj.MD_USER = userid;
               i = db.SaveChanges();
            }
            return i;
        }


        //新增.編輯.刪除角色
        public int editRole(String action,String NAME,String UUID,List<String> AryUUID){
            int i = 0;

          using(db){
              IMC_CODE obj = new IMC_CODE();
            if (action == "add")
            {
                String code;
                code = getMaxCode("COUPON", "ROLE");                
                obj.KIND = "COUPON";
                obj.TYPE = "ROLE";
                obj.CODE = code;
                obj.NAME = NAME;
                db.IMC_CODE.AddObject(obj);
            }else if(action=="edit"){
                Decimal uuid=StringUtils.getDecimal(UUID);
                obj = (from t in db.IMC_CODE where t.UUID == uuid select t).Single();
                obj.NAME = NAME;
            }else if(action=="del"){
                List<Decimal> listUuid=new List<Decimal>();
                //List<String> listUuid = new List<String>();
                foreach(String tmp in AryUUID){
                  listUuid.Add(StringUtils.getDecimal(tmp));
                //    listUuid.Add(tmp);
                }
                var query = from t in db.IMC_CODE where listUuid.Contains(t.UUID) select t;
                foreach(var item in query){
                    db.IMC_CODE.DeleteObject(item);
                }
                
            }
            i= db.SaveChanges();
          }
            return i;
        }


        //新增.編輯.刪除人員
        public int editUser(
            String action, String userid, String UUID, String USERNAME, String USERID, String USERPWD,
            String USERDPT, String USERENT, String EMAIL, String VALID,            
            List<String> AryUUID,List<String> roleList)
        {
            int i = 0;

            using (db)
            {
                DateTime currentTime = new DateTime();
                currentTime = System.DateTime.Now;
                COUPON_EMPLOYEE obj = new COUPON_EMPLOYEE();
                if (roleList != null && action != "del")
                {
                    
                     i= db.ExecuteStoreCommand("delete from intra.COUPON_ROLE_REF where USERID='" + USERID+"'");
                     
                    foreach (String code in roleList)
                    {
                        COUPON_ROLE_REF refEntity = new COUPON_ROLE_REF();   
                        refEntity.ROLEID = code;
                        refEntity.USERID = USERID;
                        refEntity.CR_USER = userid;
                        refEntity.CR_DT = currentTime;
                        db.COUPON_ROLE_REF.AddObject(refEntity);
                    }
                }

                if (action == "add")
                {
                    String code;
                    code = getMaxCode("COUPON", "ROLE");
                    obj.USERNAME =USERNAME;
                    obj.USERPWD = USERPWD;
                    obj.USERID = USERID;
                    obj.ENTID = USERENT;
                    obj.DPTID =USERDPT;
                    obj.EMAIL=EMAIL;
                    obj.VALID=VALID;
                    db.COUPON_EMPLOYEE.AddObject(obj);
                }
                else if (action == "edit")
                {
                    //Decimal uuid = StringUtils.getDecimal(UUID);
                    long uuid = System.Convert.ToInt64(UUID);
                    obj = (from t in db.COUPON_EMPLOYEE where t.UUID == uuid select t).Single();
                    obj.USERNAME =USERNAME;
                    obj.USERPWD = USERPWD;
                    obj.USERID = USERID;
                    obj.ENTID = USERENT;
                    obj.DPTID = USERDPT;
                    obj.EMAIL = EMAIL;
                    obj.VALID = VALID;
                }
                else if (action == "del")
                {
                    List<Decimal> listUuid = new List<Decimal>();
                    //List<String> listUuid = new List<String>();
                    foreach (String tmp in AryUUID)
                    {
                        listUuid.Add(StringUtils.getDecimal(tmp));
                        //    listUuid.Add(tmp);
                    }
                    var query = from t in db.COUPON_EMPLOYEE where listUuid.Contains(t.UUID) select t;
                    foreach (COUPON_EMPLOYEE item in query)
                    {
                        item.VALID = "0";
                        //db.COUPON_EMPLOYEE
                    }

                }
                i = db.SaveChanges();
            }
            return i;
        }

        //設定人員權限
        public int setUser(String userid, List<CommonDataGridPoco> list, String MenuUUID)
        {
            int i = 0;
            DateTime currentTime = new DateTime();
            currentTime = System.DateTime.Now;

            Decimal uuid = StringUtils.getDecimal(MenuUUID);
            using (db)
            {
                var obj = (from t in db.COMMON_MENU where t.UUID == uuid select t).Single();
                String str = obj.USERID;

                List<string> code = new List<string>();
                foreach (CommonDataGridPoco item in list)
                {
                    code.Add(item.USERID.ToString());
                }
                String[] strAry = code.ToArray();
                
                if (str!="")
                {                                                            
                    str += "," + String.Join(",", strAry);
                }
                else
                {
                    str += String.Join(",", strAry);
                }
                str += String.Join(",", strAry);
                obj.USERID = str;
                obj.MD_DT = currentTime;
                obj.MD_USER = userid;
                i = db.SaveChanges();
            }
            return i;
        }





        //撤回人員權限
        public int getUser(String userid, List<CommonDataGridPoco> list, String MenuUUID)
        {
            int i = 0;
            DateTime currentTime = new DateTime();
            currentTime = System.DateTime.Now;

            Decimal uuid = StringUtils.getDecimal(MenuUUID);
            List<string> code = new List<string>();
            foreach (CommonDataGridPoco item in list)
            {
                code.Add(item.USERID.ToString());
            }
            using (db)
            {
                var obj = (from t in db.COMMON_MENU where t.UUID == uuid select t).Single();

                String[] aryStr = obj.USERID.Split(',');
                List<string> aryList = new List<string>(aryStr);
                var q = (from t in aryList where !code.Contains(t) select t).ToArray();
                String joinstr = String.Join(",", q);
                obj.USERID = joinstr;
                obj.MD_DT = currentTime;
                obj.MD_USER = userid;
                i = db.SaveChanges();
            }
            return i;
        }



        //取回IMC_CODE編號最後碼
        public String getMaxCode(String kind, String type)
        {
            String code = "";
            var query = db.ExecuteStoreQuery<String>("select to_char(max(to_number(code)) + 1) code from intra.imc_code where kind='" + kind + "' and type='" + type + "'"); 
            foreach(var item in query){
                code = item;
            }
            return code;
        }


        




    }
}