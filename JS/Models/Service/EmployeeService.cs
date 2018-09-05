using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jasper.Models.Service
{
    public class EmployeeService
    {
        private RENTEntities db = new RENTEntities();
        public Hashtable getEmployee(String userid, String password)
        {
            Hashtable ht = new Hashtable();
            var query = from t in db.EMPLOYEE where t.USERID.ToLower() == userid.ToLower() && t.PASSWORD == password select t;
            
            foreach (var item in query)
            {
                ht.Add("USERID", item.USERID);
                ht.Add("USERNAME", item.USERNAME);
                ht.Add("UUID", item.UUID);
                ht.Add("ROLEID", item.ROLEID);
            }
            return ht;
        }


        public String getRole(String userid)
        {
            
            var query = from t in db.EMPLOYEE where t.USERID.ToLower() == userid.ToLower() select t;
            String roleid = "";
            foreach (var item in query)
            {
                roleid =StringUtils.getString(item.ROLEID);
            }
            return roleid;
        }


        //取得人員清單
        public JArray getLOGUSR()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();

            var query = from t in db.EMPLOYEE where t.ROLEID > 0 select new { CNO = t.UUID, CNM = t.USERNAME };

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"CNO",item.CNO},
                        {"CNM",item.CNM.Trim()},
                        
                    };
                ja.Add(itemObject);
            }
            //  jo.Add("rows", ja);            
            return ja;
        }
      
    }
}