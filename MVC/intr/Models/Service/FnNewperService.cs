using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using IntranetSystem.Poco;
using IntranetSystem.Models.Edmx;
using System.Data;

namespace IntranetSystem.Models.Service
{
    public class FnNewperService
    {
        private DB2NewperEntities db = new DB2NewperEntities();
        public JObject getNewper(PagingParamPoco param)
        {
           
            JArray ja = new JArray();
            JObject jo = new JObject();
            var query = from t in db.CERTIFICATE_FEE
                        join b in db.BACK_COMPID on t.COMP_NO equals b.BACK_COMP_NO                         
                        where t.NEWPER_CO_NO =="01" && t.NEWPER_ID != null
                        select new{UUID=t.UUID,COMP_NO=t.COMP_NO,COMP_NAME=b.BACK_CO_NAME2,NEWPER_DE_NO=t.NEWPER_DE_NO,NEWPER_DE_CODE=t.NEWPER_DE_CODE,NEWPER_CODE=t.NEWPER_CODE,
                                   NEWPER_ID = t.NEWPER_ID,
                                   NEWPER_NAME = t.NEWPER_NAME,
                                   NEWPER_LABOR = t.NEWPER_LABOR,
                                   NEWPER_GROUP = t.NEWPER_GROUP,
                                   COMP_NAME2 = b.BACK_CO_NAME1,
                                   PRINT_MARK = t.PRINT_MARK
                        };
            if (StringUtils.getString(param.search) != "")
            {
                query = query.Where(q => q.COMP_NAME.Contains(param.search) || q.COMP_NO.Contains(param.search) || q.NEWPER_NAME.Replace(" ","").Contains(param.search) || q.NEWPER_CODE.Contains(param.search) );
            }
            query = query.OrderBy(q => q.COMP_NO).ThenBy(q => q.NEWPER_CODE).ThenByDescending(q => q.NEWPER_DE_CODE);
            var total = query.Count();
           // query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            query = query.Skip(param.offset).Take(param.limit);    //分頁    
           

            foreach (var item in query)
            {
                var itemObject = new JObject
                       {   
                          {"UUID",StringUtils.getString(item.UUID)},
                          {"COMP_NO",StringUtils.getString(item.COMP_NO)},
                          {"COMP_NAME",StringUtils.getString(item.COMP_NAME) },
                          {"COMP_NAME2",StringUtils.getString(item.COMP_NAME2) },
                          {"NEWPER_CODE",StringUtils.getString(item.NEWPER_CODE) },
                          {"NEWPER_DE_CODE",StringUtils.getString(item.NEWPER_DE_CODE) },
                          {"NEWPER_DE_NO",StringUtils.getString(item.NEWPER_DE_NO)},
                          {"NEWPER_GROUP",StringUtils.getString(item.NEWPER_GROUP)},
                          {"NEWPER_ID",StringUtils.getString(item.NEWPER_ID)},
                          {"NEWPER_LABOR",StringUtils.getString(item.NEWPER_LABOR)},
                          {"NEWPER_NAME",StringUtils.getString(item.NEWPER_NAME)},
                          {"PRINT_MARK",StringUtils.getString(item.PRINT_MARK)}
                       };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", total);

            return jo;
      }

     
        public JArray getCorp()
        {

            JArray ja = new JArray();
            JObject jo = new JObject();
             string[] strArray = new string [] { "18", "19","21","22","30","33"};
             var query = from t in db.BACK_COMPID where t.BACK_COMP_NO != "**" && strArray.Contains(t.BACK_COMP_NO) select t;
            
            foreach (var item in query)
            {
                var itemObject = new JObject
                       {                             
                          {"CODE",item.BACK_COMP_NO},
                          {"NAME",item.BACK_CO_NAME1}
                       };
                ja.Add(itemObject);
            }
                    
            return ja;
        }


        public int doCreate(CERTIFICATE_FEE param)
        {
                int i = 0;
                CERTIFICATE_FEE obj = new CERTIFICATE_FEE();
                obj.COMP_NO = param.COMP_NO;
                obj.NEWPER_CO_NO = "01";
                obj.NEWPER_CODE =param.NEWPER_CODE;
                obj.NEWPER_DE_CODE = param.NEWPER_DE_CODE;
                obj.NEWPER_DE_NO = param.NEWPER_DE_NO;
                obj.NEWPER_GROUP = param.NEWPER_GROUP;
                obj.NEWPER_ID = param.NEWPER_ID;
                obj.NEWPER_LABOR = param.NEWPER_LABOR;
                obj.NEWPER_NAME = param.NEWPER_NAME;
                obj.PRINT_MARK = param.PRINT_MARK;
                db.CERTIFICATE_FEE.AddObject(obj);
                i = db.SaveChanges();
            return i;
        }

        public int doEdit(CERTIFICATE_FEE param)
        {
            int i = 0;
            var obj = (from t in db.CERTIFICATE_FEE where t.UUID == param.UUID select t).SingleOrDefault();
            if (obj != null)
            {
                obj.COMP_NO = param.COMP_NO;
                
                obj.NEWPER_CODE = param.NEWPER_CODE;
                obj.NEWPER_DE_CODE = param.NEWPER_DE_CODE;
                obj.NEWPER_DE_NO = param.NEWPER_DE_NO;
                obj.NEWPER_GROUP = param.NEWPER_GROUP;
                obj.NEWPER_ID = param.NEWPER_ID;
                obj.NEWPER_LABOR = param.NEWPER_LABOR;
                obj.NEWPER_NAME = param.NEWPER_NAME;
                obj.PRINT_MARK = param.PRINT_MARK;
                i = db.SaveChanges();
            }
            
            return i;
        }

        public int doRemove(List<CERTIFICATE_FEE> list)
        {
            
            foreach (var obj in list)
            {
                var query = (from t in db.CERTIFICATE_FEE where t.UUID == obj.UUID select t).SingleOrDefault();
                if (query != null)
                {
                    db.CERTIFICATE_FEE.DeleteObject(query);
                }
            }
           int i = db.SaveChanges();
            return i;
        }


        public DataTable getDataTableByNewper(String UUID,String CORP)
        {
            LinqExtensions le = new LinqExtensions();
            var query = from t in db.CERTIFICATE_FEE_REPORT  select t;

            if (StringUtils.getString(UUID) != "")
            {
              //  Decimal uuid=StringUtils.getDecimal(UUID);
                query = query.Where(q => q.NEWPER_CODE == UUID);
            }
            else
            {
                /*
                String[] x=new String[list.Count];
                int i =0;
                foreach (var obj in list)
                {
                    x[i] = obj.UUID;
                }                
                query = query.Where(q => x.Contains(UUID));
                 * */
                if (StringUtils.getString(CORP) != "all")
                {
                    query = query.Where(q => q.PRINT_MARK == "Y" && q.COMP_NO == CORP);
                }
                else
                {
                    query = query.Where(q => q.PRINT_MARK == "Y");
                }
            }
            DataTable dt = le.LinqQueryToDataTable(query);
            return dt;
        }





    }
}