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
    public class CommonDaoImpl:ICommonDao
    {
        private IntraEntities db = new IntraEntities();


        //取回代碼清單
        public List<Hashtable> getCodeList(EasyuiParamPoco param, String kind, String type)
        {
            List<Hashtable> list = new List<Hashtable>();
            var query = from t in db.IMC_CODE where t.KIND == kind && t.TYPE == type select new { NAME = t.NAME, CODE = t.CODE,UUID=t.UUID };
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序 
            foreach (var item in query)
            {
                Hashtable ht = new Hashtable();
                ht.Add("UUID", item.UUID);
                ht.Add("NAME", item.NAME);
                ht.Add("CODE", item.CODE);
                list.Add(ht);
            }
            return list;
        }

        //取回代碼清單
        public List<Hashtable> getCodeList(String kind, String type)
        {
            List<Hashtable> list = new List<Hashtable>();
            var query = from t in db.IMC_CODE where t.KIND == kind && t.TYPE == type select new { NAME = t.NAME, CODE = t.CODE, UUID = t.UUID };
            query = query.OrderBy(t=>t.CODE); 
            foreach (var item in query)
            {
                Hashtable ht = new Hashtable();
                ht.Add("UUID", item.UUID);
                ht.Add("NAME", item.NAME);
                ht.Add("CODE", item.CODE);
                list.Add(ht);
            }
            return list;
        }


        public Hashtable getCode(String kind, String type,String code)
        {
            var query = from t in db.IMC_CODE where t.KIND == kind && t.TYPE == type && t.CODE ==code select new { NAME = t.NAME, CODE = t.CODE,UUID=t.UUID };

            Hashtable ht = new Hashtable();
            if (query.Count() > 0)
            {
                foreach (var item in query)
                {
                    ht.Add("UUID", item.UUID);
                    ht.Add("NAME", item.NAME);
                    ht.Add("CODE", item.CODE);
                }
            }
            return ht;
        }






    }
}