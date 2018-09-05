
using System.Data.SqlClient;
using System.Transactions;
using System.Configuration;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using Jasper.Models;
using PagedList;
using System.Linq;
using Jasper.Models.Service;
using System.Linq.Dynamic;
using Jasper.Models.Poco;
using Jasper.util;
using System.Data.Objects;

namespace Jasper.service.frm3
{
    public class Frm305Service
    {
        private RENTEntities db = new RENTEntities();
        private CommonService cs = new CommonService();


        public JObject getDG1()
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
        
            var query = (from t in db.RMBUDGET select t).OrderByDescending(t=>t.YRN);

            foreach (var item in query)
            {
                var itemObject = new JObject
                    {                                           
                        {"M01",StringUtils.getString(item.M01)},
                        {"M02",StringUtils.getString(item.M02)},
                        {"M03",StringUtils.getString(item.M03)},
                        {"M04",StringUtils.getString(item.M04)},
                        {"M05",StringUtils.getString(item.M05)},
                        {"M06",StringUtils.getString(item.M06)},
                        {"M07",StringUtils.getString(item.M07)},
                        {"M08",StringUtils.getString(item.M08)},
                        {"M09",StringUtils.getString(item.M09)},
                        {"M10",StringUtils.getString(item.M10)},
                        {"M11",StringUtils.getString(item.M11)},
                        {"M12",StringUtils.getString(item.M12)},
                        {"TAGID",StringUtils.getString(item.TAGID)},
                        {"YRN",StringUtils.getString(item.YRN)}
                    };
                ja.Add(itemObject);
            }
            if (query.Count() > 0)
            {
                jo.Add("rows", ja);
                jo.Add("total", query.Count());
            }
            else
            {
                jo.Add("rows", "");
                jo.Add("total", "");
            }
            return jo;
        }







        public int doSave(Frm305Poco param)
        {

            int i = 0;
            try
            {
                using (db)
                {
                    RMBUDGET obj = (from t in db.RMBUDGET where t.TAGID == param.TAGID select t).Single();
                    obj.M01 = StringUtils.getDecimal(param.M01);
                    obj.M02 = StringUtils.getDecimal(param.M02);
                    obj.M03 = StringUtils.getDecimal(param.M03);
                    obj.M04 = StringUtils.getDecimal(param.M04);
                    obj.M05 = StringUtils.getDecimal(param.M05);
                    obj.M06 = StringUtils.getDecimal(param.M06);
                    obj.M07 = StringUtils.getDecimal(param.M07);
                    obj.M08 = StringUtils.getDecimal(param.M08);
                    obj.M09 = StringUtils.getDecimal(param.M09);
                    obj.M10 = StringUtils.getDecimal(param.M10);
                    obj.M11 = StringUtils.getDecimal(param.M11);
                    obj.M12 = StringUtils.getDecimal(param.M12);

                    i = db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return i;
        }


        public int doAdd(Frm305Poco param)
        {

            int i = 0;
            try
            {

                using (db)
                {
                    RMBUDGET obj = new RMBUDGET();
                    obj.M01 = StringUtils.getDecimal(param.M01);
                    obj.M02 = StringUtils.getDecimal(param.M02);
                    obj.M03 = StringUtils.getDecimal(param.M03);
                    obj.M04 = StringUtils.getDecimal(param.M04);
                    obj.M05 = StringUtils.getDecimal(param.M05);
                    obj.M06 = StringUtils.getDecimal(param.M06);
                    obj.M07 = StringUtils.getDecimal(param.M07);
                    obj.M08 = StringUtils.getDecimal(param.M08);
                    obj.M09 = StringUtils.getDecimal(param.M09);
                    obj.M10 = StringUtils.getDecimal(param.M10);
                    obj.M11 = StringUtils.getDecimal(param.M11);
                    obj.M12 = StringUtils.getDecimal(param.M12);
                    obj.YRN = param.YRN;
                    obj.TAGID = cs.getTagidByDatetime();
                    db.RMBUDGET.Add(obj);
                    i = db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return i;
        }






    }
}