
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
using Jasper.Models.Poco;
namespace Jasper.service.frm3{
    public class Frm304Service
    {
        private RENTEntities db = new RENTEntities();

        //  String Connection1 = ConfigurationManager.ConnectionStrings["JasperOleConnection"].ConnectionString;
        public JObject getDatagrid(Frm304Poco param)
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            /*
    SELECT         dbo.RMLIV.MAN, dbo.RMLIV.SEX, dbo.RMLIV.CNTRY, dbo.RMLIV.BLOD, 
                              dbo.RMLIV.BLODTP, dbo.RMLIV.APPES, dbo.RMLIV.CIDT, dbo.RMLIV.CODT, 
                              dbo.RMLIV.EMAIL, dbo.RMLIV.BIRTH, dbo.RMLIV.PID, dbo.RMLIV.POSL, 
                              dbo.RMLIV.MARRI, dbo.RMLIV.NOTES, dbo.RMLIV.JOBTAG, dbo.RMLIV.RSTA, 
                              dbo.RMLIV.TAGID
            */
            //var query=tmp1;
            int cnt = 0;
            if (StringUtils.getString(param.KIND) == "MAN")
            {

                var query = from t in db.RMLIV
                            join b in db.CONTRAH on t.JOBTAG equals b.TAGID into b1
                            from bb in b1.DefaultIfEmpty()
                            select new
                            {
                                RMNO = bb.RMNO,
                                MAN = t.MAN,
                                SEX = t.SEX,
                                CNTRY = t.CNTRY,
                                BLOD = t.BLOD,
                                BLODTP = t.BLODTP,
                                APPES = t.APPES,
                                CIDT = t.CIDT,
                                CODT = t.CODT,
                                EMAIL = t.EMAIL,
                                BIRTH = t.BIRTH,
                                PID = t.PID,
                                POSL = t.POSL,
                                MARRI = t.MARRI,
                                NOTES = t.NOTES,
                                JOBTAG = t.JOBTAG,
                                RSTA = t.RSTA,
                                CNSTA = bb.CNSTA,
                                CNHNO=bb.CNHNO
                            };

                if (StringUtils.getString(param.QTYPE) == "1")
                {
                    query = query.Where(t => t.RSTA == " " && t.CNSTA == " ");
                }
                else if (StringUtils.getString(param.QTYPE) == "2")
                {
                    query = query.Where(t => t.RSTA != " " && t.CNSTA != " ");
                }


                if (StringUtils.getString(param.NAME) != "")
                {
                    query = query.Where(t => t.MAN.Contains(param.NAME));
                }

                if (StringUtils.getString(param.SEX) == "1")
                {
                    query = query.Where(t => t.SEX == "男");
                }
                else if (StringUtils.getString(param.SEX) == "2")
                {
                    query = query.Where(t => t.SEX == "女");
                }




                foreach (var item in query)
                {
                    var itemObject = new JObject
                            {   
                                {"RMNO",StringUtils.getString(item.RMNO)},  
                                {"CNHNO",StringUtils.getString(item.CNHNO)},
                                {"MAN",StringUtils.getString(item.MAN)},
                                {"SEX",StringUtils.getString(item.SEX)},
                                {"CIDT",String.Format("{0:yyyy-MM-dd}" , item.CIDT)},
                                {"EMAIL",StringUtils.getString(item.EMAIL)}
                            };
                    ja.Add(itemObject);
                }
                cnt = query.Count();
            }
            else if (StringUtils.getString(param.KIND) == "TEL")
            {

                if (StringUtils.getString(param.TEL) != "")
                {
                    var query = from t in db.V_RMTEL where t.TELNO.Contains(param.TEL) select t;

                    if (StringUtils.getString(param.QTYPE) == "1")
                    {
                        query = query.Where(t => t.CNSTA == " ");
                    }
                    else if (StringUtils.getString(param.QTYPE) == "2")
                    {
                        query = query.Where(t => t.CNSTA == "C");
                    }



                    foreach (var item in query)
                    {
                        var itemObject = new JObject
                            {                                           
                                {"RMNO",StringUtils.getString(item.RMNO)},
                                {"CNHNO",StringUtils.getString(item.CNHNO)},
                                {"MAN",StringUtils.getString(item.OWNER)},
                                {"TELNO",StringUtils.getString(item.TELNO)},
                                {"TELTP",StringUtils.getString(item.TELTP)},
                                {"JOBTAG",StringUtils.getString(item.JOBTAG)}
                            };
                        ja.Add(itemObject);
                    }
                    cnt = query.Count();
                }

            }
            else if (StringUtils.getString(param.KIND) == "DEALER")
            {
                if (StringUtils.getString(param.DEALER) != "")
                {
                    var query = from t in db.V_DEALER where t.CNM.Contains(param.DEALER) select t;
                    if (StringUtils.getString(param.QTYPE) == "1")
                    {
                        query = query.Where(t => t.CNSTA == " ");
                    }
                    else if (StringUtils.getString(param.QTYPE) == "2")
                    {
                        query = query.Where(t => t.CNSTA == "C");
                    }



                    foreach (var item in query)
                    {
                        var itemObject = new JObject
                            {                                           
                                {"MAN",StringUtils.getString(item.CNM)},
                                {"RMNO",StringUtils.getString(item.RMNO)}, 
                                {"CNHNO",StringUtils.getString(item.CNHNO)},
                                {"CPID",StringUtils.getString(item.CPID)},
                                {"TELNO",StringUtils.getString(item.TEL)},                                
                                {"TAGID",StringUtils.getString(item.TAGID)}
                            };
                        ja.Add(itemObject);
                    }
                    cnt = query.Count();
                }

            }
            else if (StringUtils.getString(param.KIND) == "BROKER")
            {
                if (StringUtils.getString(param.BROKER) != "")
                {
                    var query = from t in db.V_BROKER where t.CNM.Contains(param.BROKER) select t;
                    if (StringUtils.getString(param.QTYPE) == "1")
                    {
                        query = query.Where(t => t.CNSTA == " ");
                    }
                    else if (StringUtils.getString(param.QTYPE) == "2")
                    {
                        query = query.Where(t => t.CNSTA == "C");
                    }



                    foreach (var item in query)
                    {
                        var itemObject = new JObject
                            {                                           
                                {"MAN",StringUtils.getString(item.CNM)},
                                {"RMNO",StringUtils.getString(item.RMNO)},  
                                {"CNHNO",StringUtils.getString(item.CNHNO)},
                                {"CPID",StringUtils.getString(item.CPID)},
                                {"TELNO",StringUtils.getString(item.TEL)},                                
                                {"TAGID",StringUtils.getString(item.TAGID)}
                            };
                        ja.Add(itemObject);
                    }
                    cnt = query.Count();
                }

            }
            else if (StringUtils.getString(param.KIND) == "CAR")
            {
                if (StringUtils.getString(param.CARNO) != "" || StringUtils.getString(param.PARKNO) != "" || StringUtils.getString(param.DRIVER) != "")
                {
                    var query = from t in db.V_RMCAR select t;

                    if (StringUtils.getString(param.CARNO) != "")
                    {
                        query = query.Where(t => t.CARNO.Contains(param.CARNO));
                    }
                    if (StringUtils.getString(param.PARKNO) != "")
                    {
                        query = query.Where(t => t.PARKNO.Contains(param.PARKNO));
                    }
                    if (StringUtils.getString(param.DRIVER) != "")
                    {
                        query = query.Where(t => t.DRIVER.Contains(param.DRIVER));
                    }

                    if (StringUtils.getString(param.QTYPE) == "1")
                    {
                        query = query.Where(t => t.CNSTA == " ");
                    }
                    else if (StringUtils.getString(param.QTYPE) == "2")
                    {
                        query = query.Where(t => t.CNSTA == "C");
                    }



                    foreach (var item in query)
                    {
                        var itemObject = new JObject
                            {   
                                {"RMNO",StringUtils.getString(item.RMNO)},
                                {"CNHNO",StringUtils.getString(item.CNHNO)},
                                {"MAN",StringUtils.getString(item.DRIVER)},
                                {"PARKNO",StringUtils.getString(item.PARKNO)},
                                {"CARNO",StringUtils.getString(item.CARNO)},                                
                                {"TAGID",StringUtils.getString(item.TAGID)}
                            };
                        ja.Add(itemObject);
                    }
                  }
                }
                else if (StringUtils.getString(param.KIND) == "RMNO")
                {
                    if (StringUtils.getString(param.RMNO) != "" || StringUtils.getString(param.DT1) != "" || StringUtils.getString(param.DT2) != "")
                    {
                        var query = from t in db.V_ROOM select t;

                        if (StringUtils.getString(param.RMNO) != "")
                        {
                            query = query.Where(t => t.RMNO.Contains(param.RMNO));
                        }

                        if (StringUtils.getString(param.DT1) != "")
                        {
                            DateTime dt1 = Convert.ToDateTime(param.DT1);
                            DateTime dt2 = Convert.ToDateTime(param.DT2);
                            query = query.Where(t => t.DT1 >= dt1 && t.DT2 <= dt2);
                        }


                        if (StringUtils.getString(param.QTYPE) == "1")
                        {
                            query = query.Where(t => t.CNSTA == " ");
                        }
                        else if (StringUtils.getString(param.QTYPE) == "2")
                        {
                            query = query.Where(t => t.CNSTA == "C");
                        }



                        foreach (var item in query)
                        {
                            var itemObject = new JObject
                            {   
                                {"RMNO",StringUtils.getString(item.RMNO)},   
                                {"CNHNO",StringUtils.getString(item.CNHNO)},
                                {"MAN",StringUtils.getString(item.DEALMAN)},
                                {"DEALERNM",StringUtils.getString(item.DEALERNM)},
                                {"DT1",String.Format("{0:yyyy-MM-dd}" , item.DT1)},
                                {"DT2",String.Format("{0:yyyy-MM-dd}" , item.DT2)},
                                {"TAGID",StringUtils.getString(item.TAGID)}
                            };
                            ja.Add(itemObject);
                        }
                        cnt = query.Count();
                    }
                }


                if (cnt > 0)
                {
                    jo.Add("rows", ja);
                    jo.Add("total", cnt);
                }
                else
                {
                    jo.Add("rows", "");
                    jo.Add("total", "");
                }
                return jo;

            }
      }
}