using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
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
    
    public class EmployeeService
    {
        private IEmployeeDao iEmployeeDao= new EmployeeDaoImpl();
        private IntraEntities db = new IntraEntities();    
        public Hashtable getEmployee(String userid, String password)
        {
            JArray jr = new JArray();
            Hashtable ht = iEmployeeDao.getEmployee(userid, password);
            return ht;
        }

        public String getRoleAryStr(String userid)
        {
            return iEmployeeDao.getRoleAryStr(userid);
        }

        public List<Hashtable> getEmployee()
        {
            JArray jr = new JArray();
            List<Hashtable> list = iEmployeeDao.getEmployee();
            return list;
        }

        public JObject getUserList(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();    
           /*
            var query = from t in db.COUPON_EMPLOYEE
                        join a in db.I_COMPANY on t.ENTID equals a.CODE into comp
                        from aa in comp
                        join b in db.I_DEPARTMENT_VIEW on t.ENTID + "-" + t.DPTID equals b.CODE into dept
                        from bb in dept 
                        select new {UUID=t.UUID,USERID = t.USERID,USERPWD=t.USERPWD,USERNAME =t.USERNAME ,ENTID =t.ENTID,DPTID=t.DPTID ,ENTNAME=aa.NAME,DPTNAME=bb.NAME ,EMAIL=t.EMAIL,VALID=t.VALID,CID=aa.CID,DID=bb.DID};
            */
            var query = from t in db.COUPON_EMPLOYEE
                        join a in db.COUPON_COMPANY on t.ENTID equals a.CODE into comp
                        from aa in comp
                        join b in db.COUPON_DEPARTMENT on t.DPTID equals b.CODE into dept
                        from bb in dept
                        select new { UUID = t.UUID, USERID = t.USERID, USERPWD = t.USERPWD, USERNAME = t.USERNAME, ENTID = t.ENTID, DPTID = t.DPTID, ENTNAME = aa.NAME, DPTNAME = bb.NAME, EMAIL = t.EMAIL, VALID = t.VALID, CID = aa.CODE, DID = bb.UUID };


            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序 
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},
                        {"USERID",item.USERID},
                        {"USERNAME",item.USERNAME},
                        {"USERPWD",item.USERPWD},
                        {"ENTID",item.ENTID},
                        {"DPTID",item.DPTID},
                        {"ENTNAME",item.ENTNAME},
                        {"DPTNAME",item.DPTNAME},
                        {"EMAIL",item.EMAIL},
                        {"VALID",item.VALID},
                        {"CID",item.CID},
                        {"DID",item.DID},
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", ja.Count());
            return jo;
        }



        public JObject getUserList2(EasyuiParamPoco param, String UUID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            Decimal uuid = StringUtils.getDecimal(UUID);
            /*
            var query =(
                      from i in db.COUPON_EMPLOYEE
                      join a in db.I_COMPANY on i.ENTID equals a.CODE into comp
                      from aa in comp
                      join b in db.I_DEPARTMENT_VIEW on i.ENTID + "-" + i.DPTID equals b.CODE into dept
                      from bb in dept
                      where !(
                        from t in db.COUPON_MENU_USER_VIEW where t.UUID==uuid
                        select t.USERID).Contains(i.USERID) 
                      select new { USERNAME = i.USERNAME, USERID = i.USERID,USERPWD=i.USERPWD, UUID = i.UUID,ENTID=i.ENTID,DPTID=i.DPTID,ENTNAME=aa.NAME,DPTNAME=bb.NAME,EMAIL=i.EMAIL,VALID=i.VALID,CID=aa.CID,DID=bb.DID });
            */
            var query = (
                      from i in db.COUPON_EMPLOYEE
                      join a in db.COUPON_COMPANY on i.ENTID equals a.CODE into comp
                      from aa in comp
                      join b in db.COUPON_DEPARTMENT on  i.DPTID equals b.CODE into dept
                      from bb in dept
                      where !(
                        from t in db.COUPON_MENU_USER_VIEW
                        where t.UUID == uuid
                        select t.USERID).Contains(i.USERID)
                      select new { USERNAME = i.USERNAME, USERID = i.USERID, USERPWD = i.USERPWD, UUID = i.UUID, ENTID = i.ENTID, DPTID = i.DPTID, ENTNAME = aa.NAME, DPTNAME = bb.NAME, EMAIL = i.EMAIL, VALID = i.VALID, CID = aa.CODE, DID = bb.UUID });

            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
        //    query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁    

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"UUID",item.UUID},
                        {"USERID",item.USERID},
                        {"USERNAME",item.USERNAME},
                        {"USERPWD",item.USERPWD},
                        {"ENTID",item.ENTID},
                        {"DPTID",item.DPTID},
                        {"ENTNAME",item.ENTNAME},
                        {"DPTNAME",item.DPTNAME},
                        {"EMAIL",item.EMAIL},
                        {"VALID",item.VALID},
                        {"CID",item.CID},
                        {"DID",item.DID},
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", ja.Count());
            return jo;
        }


        public int isUserCreated(String userid)
        {
            var query = from i in db.COUPON_EMPLOYEE where i.USERID == userid select i;
            return query.Count();
        }



        //取回目前人員選取角色列表
        public JArray getUserRoleList(String userid)
        {
            JArray ja =new JArray();
            var query = (from t in db.COUPON_ROLE_REF
                        join a in db.IMC_CODE on t.ROLEID equals a.CODE into code
                        from aa in code
                        where aa.KIND == "COUPON" && aa.TYPE == "ROLE"
                        && t.USERID==userid
                        select new { aa.CODE, aa.NAME }).Distinct();
            foreach(var item in query){
                var itemObject = new JObject
                    {                           
                        {"CODE",item.CODE},
                        {"NAME",item.NAME},
                    };
                ja.Add(itemObject);                 
            }
            return ja;
        }


        //取回人員新增角色列表,排除已選取角色
        public JArray getUserRoleAdd(String userid,List<String> list)
        {
            JArray ja = new JArray();
            /*
            var query = from t in db.IMC_CODE                         
                        where t.KIND == "COUPON" && t.TYPE == "ROLE"
                        && !(from r in db.COUPON_ROLE_REF where r.USERID==userid
                            select r.ROLEID).Contains(t.CODE)
                        select new { t.CODE, t.NAME };
             */
            var query = from t in db.IMC_CODE
                        where t.KIND == "COUPON" && t.TYPE == "ROLE"
                        && !(list).Contains(t.CODE)
                        select new { t.CODE, t.NAME };

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {                           
                        {"CODE",item.CODE},
                        {"NAME",item.NAME},
                    };
                ja.Add(itemObject);
            }
            return ja;
        }


    }
}