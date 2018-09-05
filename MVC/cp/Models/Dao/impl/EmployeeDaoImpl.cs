using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HowardCoupon_vs2010.Models.Edmx;
using System.Linq.Dynamic;
using System.Linq;

namespace HowardCoupon.Models.Dao.impl
{
    public class EmployeeDaoImpl:IEmployeeDao
    {
        private IntraEntities db = new IntraEntities();
        
        public List<Hashtable> getEmployee()
        {
            
            List<Hashtable> list = new List<Hashtable>();
          //  var query = from t in db.COUPON_EMPLOYEE where t.VALID == "1" select t;
            /* intra.i_company,intra.i_department
            var query = from t in db.COUPON_EMPLOYEE
                        join a in db.I_COMPANY on t.ENTID equals a.CODE into comp
                        from aa in comp
                        join b in db.I_DEPARTMENT_VIEW on t.ENTID + "-" + t.DPTID equals b.CODE into dept
                        from bb in dept
                        select new {USERID = t.USERID,USERNAME =t.USERNAME ,ENTID =t.ENTID,DEPTID=t.DPTID ,ENTNAME=aa.NAME,DEPTNAME=bb.NAME ,EMAIL=t.EMAIL};
            */
            var query = from t in db.COUPON_EMPLOYEE
                        join a in db.COUPON_COMPANY on t.ENTID equals a.CODE into comp
                        from aa in comp
                        join b in db.COUPON_DEPARTMENT on t.DPTID equals b.CODE into dept
                        from bb in dept
                        select new { USERID = t.USERID, USERNAME = t.USERNAME, ENTID = t.ENTID, DEPTID = t.DPTID, ENTNAME = aa.NAME, DEPTNAME = bb.NAME, EMAIL = t.EMAIL };

            foreach (var item in query)
            {
                Hashtable ht = new Hashtable();
                ht.Add("USERID", item.USERID);
                ht.Add("USERNAME", item.USERNAME);
                ht.Add("ENTID", item.ENTID);
                ht.Add("ENTNAME", item.ENTNAME);
                ht.Add("DEPTID", item.DEPTID);
                ht.Add("DEPTNAME",item.DEPTNAME);
                ht.Add("EMAIL", item.EMAIL);   
                list.Add(ht);
            }
            return list;
        }

        public Hashtable getEmployee(String userid)
        {
            Hashtable ht = new Hashtable();
            var query = from t in db.COUPON_EMPLOYEE where t.VALID == "1" && t.USERID ==userid  select t;
            foreach (var item in query)
            {
                ht.Add("USERID", item.USERID);
                ht.Add("USERNAME", item.USERNAME);
                ht.Add("UUID", item.UUID);
                ht.Add("EMAIL", item.EMAIL);
                ht.Add("ENTID", item.ENTID);
                ht.Add("DPTID", item.DPTID);
            }
            return ht;
        }


        public Hashtable getEmployee(String userid,String password)
        {
            Hashtable ht = new Hashtable();
            var query = from t in db.COUPON_EMPLOYEE where t.VALID == "1" && t.USERID.ToLower() == userid.ToLower() && t.USERPWD==password select t;
            foreach (var item in query)
            {
                ht.Add("USERID", item.USERID);
                ht.Add("USERNAME", item.USERNAME);
                ht.Add("UUID", item.UUID);
                ht.Add("EMAIL", item.EMAIL);
                ht.Add("ENTID", item.ENTID);
                ht.Add("DPTID", item.DPTID);
            }
            return ht;
        }


        public String getRoleAryStr(String userid)
        {
            //List<Hashtable> list = new List<Hashtable>();
            /*
            var query = (from a in db.COUPON_ROLE_REF                         
                         join b in db.IMC_CODE on a.ROLEID equals b.CODE into imcc
                         from c in imcc.DefaultIfEmpty()
                         where a.ROLEID == userid && c.KIND == "COUPON" && c.TYPE == "ROLE"
                         select new {ITEM = c.NAME,VALUE=c.CODE}
                        ).ToArray();
            */
           // var query = (from t in db.COUPON_ROLE_REF where t.USERID == userid group db.COUPON_ROLE_REF by t.USERID into g select g.First()).ToArray();

            var query =(from t in db.COUPON_ROLE_REF where t.USERID == userid group t by new { t.USERID, t.ROLEID } into g select new { ROLEID = g.Key.ROLEID}).ToArray();
            String[] str =new String[query.Count()];
            int i=0;
            foreach (var item in query)
            {
              str[i] = item.ROLEID;                
              i++;
            }
            String strJoin = string.Join(",", str);
            
           
            /*
            foreach (var item in query)
            {
                Hashtable ht = new Hashtable();
                ht.Add("ITEM", item.ROLEID);
                ht.Add("VALUE", item.ROLEID);
                list.Add(ht);
            }*/

            return strJoin;
        }






    }
}