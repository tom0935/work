using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using IntranetSystem.Poco;
using IntranetSystem.Models.Edmx;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using HowardCoupon_vs2010.Poco;

namespace IntranetSystem.Models.Service
{



    public class SignService
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
        private HowardIntraEntities db = new HowardIntraEntities();
        public JObject getSign(PagingParamPoco param)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.SIGN 
                        join b in db.DEPART on t.DEPART equals b.CODE into bb
                        from d in bb.DefaultIfEmpty()
                        select new {UUID= t.UUID,URL=t.URL,IMG=t.IMG,DEPART = d.CODE,DEPART_NAME = d.NAME,EDIT_DT=t.EDIT_DT,EDIT_USER=t.EDIT_USER,REMARK=t.REMARK};
                        

            if (StringUtils.getString(param.search) != "")
            {                                  
                var q1 = query.Where(q => q.IMG.Contains("" + param.search + ""));
                if (q1.Count() == 0)
                {
                    var q2 = query.Where(q => q.URL.Contains("" + param.search + ""));
                    if (q2.Count() > 0)
                    {
                        query = q2;
                    }
                }
                else
                {
                    query = q1;
                }
            }
              
      //      query = query.OrderByDescending(q => q.UUID);
            
            var total = query.Count();
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip(param.offset).Take(param.limit);    //分頁    
           

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {   
                          {"UUID",StringUtils.getString(item.UUID)},
                          {"URL",StringUtils.getString(item.URL)},
                          {"IMG",StringUtils.getString(item.IMG)},                          
                          {"DEPART",StringUtils.getString(item.DEPART)},
                          {"DEPART_NAME",StringUtils.getString(item.DEPART_NAME)},
                          {"EDIT_DT",StringUtils.getString(item.EDIT_DT)},
                          {"EDIT_USER",StringUtils.getString(item.EDIT_USER)},
                          {"REMARK",StringUtils.getString(item.REMARK)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);

            return jo;
      }








        public String getURL(String UUID)
        {
            String url="";
            if (StringUtils.getString(UUID) != "")
            {
                int uuid = System.Convert.ToInt32(UUID);                
                var query = (from t in db.QRCODE_DTL where t.UUID_DTL == uuid select t).SingleOrDefault();
                url = query.URL_DTL;
            }            
            return url;
        }


        public int doCreate(SIGN param)
        {
                int i = 0;
                SIGN obj = new SIGN();
                obj.URL = param.URL;
                
                obj.EDIT_DT = System.DateTime.Now;
                obj.EDIT_USER = param.EDIT_USER;
                obj.REMARK = param.REMARK;
                obj.DEPART = param.DEPART;



                if (StringUtils.getString(param.IMG) != "")
                {
                    obj.IMG = param.IMG;
                }
                db.SIGN.AddObject(obj);

                i = db.SaveChanges();
            return obj.UUID;
        }




        public int doEdit(SIGN param)
        {
            int i = 0;
            var obj = (from t in db.SIGN where t.UUID == param.UUID select t).SingleOrDefault();
            if (obj != null)
            {
                obj.URL = param.URL;                
                obj.EDIT_DT = System.DateTime.Now;
                obj.EDIT_USER = param.EDIT_USER;
                obj.REMARK = param.REMARK;
                obj.DEPART = param.DEPART;

                if (StringUtils.getString(param.IMG) != "")
                {
                    obj.IMG = param.IMG;
                }


                i = db.SaveChanges();
            }
            
            return i;
        }




        public int doEditURL(QRCODE_DTL param)
        {
            int i = 0;
            var obj = (from t in db.QRCODE_DTL where t.UUID_DTL == param.UUID_DTL select t).SingleOrDefault();
            if (obj != null)
            {              
                obj.EDIT_DT_DTL = System.DateTime.Now;
                obj.EDIT_USER_DTL = param.EDIT_USER_DTL;   
                obj.URL_DTL = param.URL_DTL;
                i = db.SaveChanges();
            }

            return i;
        }

        public int doRemove(int UUID)
        {
            int i = 0;
                var query = (from t in db.SIGN where t.UUID == UUID select t).SingleOrDefault();
                if (query != null)
                {
                    db.SIGN.DeleteObject(query);
                     i = db.SaveChanges();
                }          
            return i;
        }





        public JArray getDepart()
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = (from t in db.DEPART select t).OrderBy(t=>t.CODE);

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {                             
                          {"NAME",item.NAME},
                          {"CODE",item.CODE}
                       };
                ja.Add(itemObject);
            }

            return ja;
        }



       
        /*
        public JArray getDepart()
        {

            string query = "SELECT CODE,NAME FROM I_DEPARTMENT where CID='1' AND ENABLE='1' and remark is null";
            JArray ja = new JArray();
            JObject jo = new JObject();

            
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while ((reader.Read()))
                    {

                        if (!reader[0].Equals(DBNull.Value))
                        {
                            var itemObject = new JObject
                            {                             
                                {"NAME",reader[0].ToString() + " " + reader[1].ToString() },
                                {"CODE",reader[0].ToString()}
                            };
                            ja.Add(itemObject);
                        }
                    }
                    reader.Close();
                }
                
            }
            
            return ja;

        }*/


        public JArray getDepartCurrent(int uuid)
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.QRCODE_APPLY_DEPART where t.QRCODE_UUID==uuid select t;

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {                             
                          {"text",item.NAME},
                          {"id",item.CODE}
                       };
                ja.Add(itemObject);
            }

            return ja;
        }


    }
}