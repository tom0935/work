using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HowardCoupon_vs2010.Models.Service;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using HowardCoupon.Poco;

namespace HowardCoupon_vs2010.Controllers
{
 [Authorize]
    public class MenuController : Controller
    {
        private MenuService menuService = new MenuService();
        private CommonService commonService = new CommonService();
        private EmployeeService employeeService = new EmployeeService();
        public ActionResult Index()
        {
                        
            return View();
        }

        public ActionResult RoleManager()
        {                        
            return View();
        }
        
        //依傳入權限顯示 > _MenuView_.cshtml
        public ActionResult getMenuitem()
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            
            string userid = User.Identity.Name;            
            JObject jo =new JObject();
            jo = menuService.getMenuItem(userid,roleid.Split(','));
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回目前專案使用選單列 TreeGrid
        public ActionResult getMenuList()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            jo = menuService.getMenuList();
            ja.Add(jo);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回所有人員列表
        public ActionResult getUserList(EasyuiParamPoco param,String UUID)
        {
            JObject jo = new JObject();
            if (UUID != null)
            {
                jo = employeeService.getUserList2(param,UUID);
            }
            else
            {
                jo = employeeService.getUserList(param);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回所有列表
        public ActionResult getRoleList(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            jo = commonService.getCodeList(param, "COUPON", "ROLE");
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回人員使用角色列表
        public ActionResult getUserRoleList(String userid)
        {
            JArray ja = new JArray();
            ja = employeeService.getUserRoleList(userid);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getUserRoleAdd(String userid,List<CommonDataGridPoco> list)
        {
            JArray ja = new JArray();
            List<String> aryList = new List<String>();
            if (list!=null)
            {
                aryList = (from t in list select t.CODE).ToList();
            }
            ja = employeeService.getUserRoleAdd(userid, aryList);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        //取回所有角色列表
        public ActionResult getCodeList(EasyuiParamPoco param ,String UUID)
        {
            JObject jo = new JObject();
            jo = menuService.getCodeList(param,UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回目前選取選單角色
        public ActionResult getMenuRoleList(EasyuiParamPoco param ,String UUID)
        {
            JObject jo = new JObject();
            jo = menuService.getMenuRoleList(param,UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

       //取回目前選取人員
        public ActionResult getMenuUserList(EasyuiParamPoco param, String UUID)
        {
            JObject jo = new JObject();
            jo = menuService.getMenuUserList(param, UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getEmployee()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            List<Hashtable> list = new List<Hashtable>();
            list = employeeService.getEmployee();
            foreach (Hashtable ht in list)
            {                
                var itemObject = new JObject
                    {                                           
                        {"USERID",HashtableUtil.getValue(ht, "USERID")}, 
                        {"USERNAME",HashtableUtil.getValue(ht, "USERNAME")},                        
                        {"ENTID",HashtableUtil.getValue(ht, "ENTID")},
                        {"ENTNAME",HashtableUtil.getValue(ht, "ENTNAME")},    
                        {"DEPTID",HashtableUtil.getValue(ht, "DEPTID")}, 
                        {"DEPTNAME",HashtableUtil.getValue(ht, "DEPTNAME")}, 
                        {"EMAIL",HashtableUtil.getValue(ht, "EMAIL")},
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows",ja);
            jo.Add("total", ja.Count);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //新增選單
        public ActionResult addMenu(String MENUITEM,String URL,String LEVEL1,String ENABLE,String TARGET)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;   

            JObject jo = new JObject();
            int i = menuService.addMenu(userid,MENUITEM,URL,LEVEL1,ENABLE,TARGET);
            jo.Add("result", i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //刪除選單
        public ActionResult delMenu(String UUID,String LEVEL1,String LEVEL2)        
        {
            JObject jo = new JObject();
            int i = menuService.delMenu(UUID,LEVEL1,LEVEL2);
            jo.Add("result", i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //儲存選單
        public ActionResult saveMenu( String UUID, String MENUITEM, String URL, String ENABLE,String TARGET, String LEVEL1, String LEVEL2,String OLDLEVEL1)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            int i = menuService.saveMenu(userid, UUID, MENUITEM, URL, ENABLE,TARGET, LEVEL1, LEVEL2,OLDLEVEL1);
            jo.Add("result", i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult settingRole(List<CommonDataGridPoco> list,String MenuUUID,String ACTION)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            int i=0;
            if (ACTION == "set")
            {
                i = menuService.setRole(userid, list, MenuUUID);
            }
            else
            {
                i = menuService.getRole(userid, list, MenuUUID);
            }
            jo.Add("result",i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult editRole(String ACTION, String NAME, String UUID, List<CommonDataGridPoco> list)
        {
            JObject jo = new JObject();
            int i=0;        
            List<String> UUIDList=new List<String>();
            if (list != null)
            {
                UUIDList = (from t in list select t.UUID).ToList();              
            }
            i = menuService.editRole(ACTION, NAME, UUID, UUIDList);
             jo.Add("result", i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult editUser(String ACTION, String UUID, String USERNAME, String USERID, String USERPWD,
            String USERDPT, String USERENT, String EMAIL, String VALID,
            List<CommonDataGridPoco> list,List<CommonDataGridPoco> roles
            )
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;

            JObject jo = new JObject();
            int i = 0;
            List<String> UUIDList = new List<String>();
            if (list != null)
            {
                UUIDList = (from t in list select t.UUID).ToList();
            }
            List<String> roleList = new List<String>();
            if (roles != null)
            {
                roleList=(from t in roles select t.CODE).ToList();
            }
            i = menuService.editUser(ACTION, userid, UUID, USERNAME, USERID, USERPWD, USERDPT, USERENT, EMAIL, VALID, UUIDList, roleList);
            jo.Add("result", i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult settingUser(List<CommonDataGridPoco> list, String MenuUUID, String ACTION)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            int i = 0;
            if (ACTION == "set")
            {
                i = menuService.setUser(userid, list, MenuUUID);
            }
            else
            {
                i = menuService.getUser(userid, list, MenuUUID);
            }
            jo.Add("result", i);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getENT()
        {
            JArray ja = new JArray();
            ja = commonService.getENT();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
     /*
        public ActionResult getDPT(String CID)
        {
            JArray ja = new JArray();
            ja = commonService.getDPT(CID);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
     */
        public ActionResult getDPT()
        {
            JArray ja = new JArray();
            ja = commonService.getDPT();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult isUserCreated(String userid)
        {
            int i= employeeService.isUserCreated(userid);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

    }


}
