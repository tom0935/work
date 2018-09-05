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
using System.Linq.Dynamic;
namespace Jasper.service.frm1
{
    public class Frm110Service
    {
        private RENTEntities db = new RENTEntities();


        /*
W_SQL = "SELECT A.CNO,A.LOCT,A.SZ,A.USEON,B.CNPNO,A.ONUSE FROM CARLOCT A " + ;
		"LEFT JOIN CONTRAP B ON A.CNO = B.PARKNO AND B.CNSTA = ' ' " + ;
		"ORDER BY A.CNO          
         **/

        public JArray getDatagrid(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc")
        {
            JArray ja = new JArray();
            /*
var leftOuterJoinQuery =
    from category in categories
    join prod in products on category.ID equals prod.CategoryID into prodGroup
    from item in prodGroup.DefaultIfEmpty(new Product { Name = String.Empty, CategoryID = 0 })
    select new { CatName = category.Name, ProdName = item.Name };
             * */


            var query1 = from a in db.CARLOCT.AsQueryable()
                         join b in db.CONTRAP.AsQueryable() on a.CNO equals b.PARKNO into bgroup
                         from item in bgroup.DefaultIfEmpty()                             
                         select new { CNO = a.CNO, LOCT = a.LOCT, SZ = a.SZ, USEON = a.USEON,ONUSE =a.ONUSE, CNPNO = item.CNPNO };
            query1 = query1.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
            query1 = query1.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

            foreach (var item in query1)
            {
                var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"LOCT",StringUtils.getString(item.LOCT)},
                        {"SZ",StringUtils.getString(item.SZ)},
                        {"USEON",StringUtils.getString(item.USEON)},
                        {"ONUSE",StringUtils.getString(item.ONUSE)},
                        {"CNPNO",StringUtils.getString(item.CNPNO)}
                    };
                ja.Add(itemObject);
            }

            return ja;
        }


        /*
         * W_SQL = "SELECT A.CNO,A.LOCT,A.SZ,A.USEON,B.CNPNO,A.ONUSE FROM CARLOCT A " + ;
		"LEFT JOIN CONTRAP B ON A.CNO = B.PARKNO AND B.CNSTA = ' ' " + ;
		"WHERE A.USEON = '住宅' AND A.ONUSE = 0 " + ;
		"ORDER BY A.CNO"
         */
         
        public JArray getDatagridQuery(int page = 1, int pageSize = 20, String propertyName = "", String order = "asc")
        {
            JArray ja = new JArray();
            var query1 = from a in db.CARLOCT.AsQueryable()
                         join b in db.CONTRAP.AsQueryable() on a.CNO equals b.PARKNO  into bgroup
                         from item in bgroup.DefaultIfEmpty()
                         where  a.USEON == "住宅" && a.ONUSE ==0
                         select new { CNO = a.CNO, LOCT = a.LOCT, SZ = a.SZ, USEON = a.USEON,ONUSE =a.ONUSE, CNPNO = item.CNPNO };
            query1 = query1.OrderBy(string.Format("{0} {1}", propertyName, order)); //排序        
            query1 = query1.Skip((page - 1) * pageSize).Take(pageSize);    //分頁  

            foreach (var item in query1)
            {
                var itemObject = new JObject
                    {                                           
                        {"CNO",StringUtils.getString(item.CNO)},
                        {"LOCT",StringUtils.getString(item.LOCT)},
                        {"SZ",StringUtils.getString(item.SZ)},
                        {"USEON",StringUtils.getString(item.USEON)},
                        {"ONUSE",StringUtils.getString(item.ONUSE)},
                        {"CNPNO",StringUtils.getString(item.CNPNO)}
                    };
                ja.Add(itemObject);
            }

            return ja;
        }


        public int getTotalCount()
        {
            var query1 = from a in db.CARLOCT.AsQueryable()
                         join b in db.CONTRAP.AsQueryable() on a.CNO equals b.PARKNO into bgroup
                         from item in bgroup.DefaultIfEmpty()
                         select new { CNO = a.CNO, LOCT = a.LOCT, SZ = a.SZ, USEON = a.USEON, ONUSE = a.ONUSE, CNPNO = item.CNPNO };

            return query1.Count();
        }

        public int getTotalCount2()
        {
            var query1 = from a in db.CARLOCT.AsQueryable()
                         join b in db.CONTRAP.AsQueryable() on a.CNO equals b.PARKNO into bgroup
                         from item in bgroup.DefaultIfEmpty()
                         where a.USEON == "住宅" && a.ONUSE == 0
                         select new { CNO = a.CNO, LOCT = a.LOCT, SZ = a.SZ, USEON = a.USEON, ONUSE = a.ONUSE, CNPNO = item.CNPNO };
            return query1.Count();
        }

/*
 * SELECT CARLOCT110
*W_CARS = RECCOUNT("CARLOCT110")
COUNT FOR USEON = "住宅" TO W_CARS
COUNT FOR USEON = "住宅" AND ONUSE = 0 TO W_EMPTY
*/
        public JArray getCountView()
        {
            JArray ja = new JArray();
            var query1 = from a in db.CARLOCT.AsQueryable()                         
                         where a.USEON == "住宅" 
                         select a;
            var query2 = from a in db.CARLOCT.AsQueryable()
                         where a.USEON == "住宅" && a.ONUSE == 0
                         select a;

                var itemObject = new JObject
                    {                                           
                        {"TOT",query1.Count().ToString()},
                        {"USETOT",query2.Count().ToString()}
                    };
                ja.Add(itemObject);

            return ja;
        }


        public int doSave(String mode, CARLOCT param)
        {
            int i = 0;
            var query = (from t in db.CARLOCT select t);
            if (param.CNO != null)
            {
                CARLOCT obj = null;
                if (mode.Equals("add"))
                {
                    obj = new CARLOCT();
                    obj.CNO = param.CNO;
                }
                else
                {
                    obj = query.Where(q => q.CNO == param.CNO).SingleOrDefault();
                }
                obj.LOCT = StringUtils.getString(param.LOCT);
                obj.SZ = StringUtils.getString(param.SZ);
                obj.USEON = StringUtils.getString(param.USEON);
                obj.ONUSE = param.ONUSE;
                if (mode.Equals("add"))
                {
                    db.CARLOCT.Add(obj);
                }
                i = db.SaveChanges();
            }
            return i;
        }



        public int doRemove(String cno)
        {
            int i = 1;

            CARLOCT obj = (from c in db.CARLOCT
                         where c.CNO == cno
                         select c).Single();
            db.CARLOCT.Remove(obj);
            db.SaveChanges();
            return i;
        }


    }
}