using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Dynamic;
using System.Linq;
using IntranetSystem.Models;
using IntranetSystem.Poco;
namespace IntranetSystem.Service
{
    public class CommonService
    {
        private Entities db = new Entities();
        public Hashtable getUserInfo(String ID)
        {
            Hashtable ht = new Hashtable();
            //SELECT A.NAME FROM I_DEPARTMENT A, I_UD_REF01 B WHERE A.DID=B.DID AND B.AID=

            //var query = from a in db.I_USER.AsQueryable()                        

                        /*
                        join c in db.I_DEPARTMENT on a. equals c.DID into deptg
                        from dept in deptg.DefaultIfEmpty()
                        join d in db.I_COMPANY on a.CID equals d.CID into compg
                        from comp in compg.DefaultIfEmpty()
                        select new { DEPART_NAME = dept.NAME, DEPART_CODE = dept.CODE ,COMP_NAME = comp.NAME,COMP_CODE = comp.CODE };
                        */


            Decimal x = Decimal.Parse(ID);

            var query = from a in db.I_UD_REF01 
                        join b in db.I_DEPARTMENT on a.DID equals b.DID into deptg                        
                        from dept in deptg.DefaultIfEmpty()
                        join c in db.I_COMPANY on dept.CID equals c.CID into compg
                        from comp in compg.DefaultIfEmpty()
                        where a.AID == x
                        select new { DEPART_NAME = dept.NAME, DEPART_CODE = dept.CODE, COMP_NAME = comp.NAME, COMP_CODE = comp.CODE };
            

            foreach (var item in query)
            {
                ht.Add("DEPART_NAME", item.DEPART_NAME);
                ht.Add("DEPART_CODE", item.DEPART_CODE);
                ht.Add("COMP_NAME", item.COMP_NAME);
                ht.Add("COMP_CODE", item.COMP_CODE);
                break;
            }            
            return ht;
        }

        public JArray getDPT(String CODE)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
             //Decimal cid = StringUtils.getDecimal(CODE);
             var query = from t in db.I_DEPARTMENT_VIEW where t.CODE.Substring(0,2) == CODE select t;
            //var query = from t in db.COUPON_DEPARTMENT where t.OUTID == CID select t;
             //var i=query.Count();
            foreach (var item in query)
            {
                var itemObject = new JObject
                    {   
                        {"CODE",item.CODE},
                        //{"NAME",item.CODE.Substring(3,4) + " " +item.NAME},
                        {"NAME",item.CODE + " " +item.NAME},
                        
                    };
                ja.Add(itemObject);
            }

            return ja;
        }


        public String getKR1VLanDefine(String TYPE){
            var query = (from t in db.IMC_CODE where t.KIND=="KR1" && t.TYPE == TYPE select t).SingleOrDefault();
            return  StringUtils.getString(query.CODE);
        }

    }

}