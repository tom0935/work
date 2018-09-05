
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

namespace Jasper.service.frm2{
public class Frm205Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();


    public JObject getDG1(EasyuiParamPoco param, String DT)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.FIXM where t.RSTA == " " select t);

        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            if (param.sort != null)
            {
                query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            }
            if (param.rows > 0)
            {
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
            }
        }
        else
        {
            jo.Add("total", "");
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"FIXDT",String.Format("{0:yyyy-MM-dd}",item.FIXDT)},                        
                        {"ENDDT",item.ENDDT},
                        {"LOGDT",item.LOGDT},
                        {"LOGUSR",item.LOGUSR},
                        {"FIXAMT",item.FIXAMT},
                        {"NOTES",item.NOTES},
                        {"FIXAREA",item.FIXAREA},
                        {"FIXITEM",item.FIXITEM},
                        {"FIXTP",item.FIXTP},
                        {"ENDNOTE",item.ENDNOTE},
                        {"RMNO",item.RMNO},
                        {"TAGID",item.TAGID}
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);            
        }
        else
        {
            jo.Add("rows", "");         
        }



        return jo;
    }

    public JObject getDG2(EasyuiParamPoco param, String DT)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.FIXM where t.RSTA == "Y" select t);


        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            if (param.sort != null)
            {
                query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            }
            if (param.rows > 0)
            {
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
            }
        }
        else
        {
            jo.Add("total", "");
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"FIXDT",String.Format("{0:yyyy-MM-dd}",item.FIXDT)},                        
                        {"ENDDT",item.ENDDT},
                        {"LOGDT",item.LOGDT},
                        {"LOGUSR",item.LOGUSR},
                        {"FIXAMT",item.FIXAMT},
                        {"NOTES",item.NOTES},
                        {"FIXAREA",item.FIXAREA},
                        {"FIXITEM",item.FIXITEM},
                        {"FIXTP",item.FIXTP},
                        {"ENDNOTE",item.ENDNOTE},
                        {"RMNO",item.RMNO},
                        {"TAGID",item.TAGID}
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);            
        }
        else
        {
            jo.Add("rows", "");            
        }
        return jo;
    }


    public JObject getDG3(EasyuiParamPoco param, String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        
        var query = (from t in db.FIXM select t);
        if (StringUtils.getString(FIXAREA) == "" && StringUtils.getString(END) == "" && StringUtils.getString(FIXTP) == "" && StringUtils.getString(FIXITEM) == "" && StringUtils.getString(RMNO) == "" && StringUtils.getString(SDT) == "")
        {
            query = query.Where(t => t.RSTA == "xxx");
        }
        else
        {

            if (StringUtils.getString(FIXAREA) != "")
            {
                int area = 1;
                area = Convert.ToInt16(FIXAREA);
                query = query.Where(t => t.FIXAREA == area);
            }


            if (StringUtils.getString(END) != "")
            {
                if (END == "1")
                {
                    query = query.Where(t => t.RSTA == " ");
                }
                else if (END == "2")
                {
                    query = query.Where(t => t.RSTA == "Y");
                }
            }
            if (StringUtils.getString(FIXTP) != "")
            {
                query = query.Where(t => t.FIXTP == FIXTP);
            }

            if (StringUtils.getString(FIXITEM) != "")
            {
                query = query.Where(t => t.FIXITEM == FIXITEM);
            }


            if (StringUtils.getString(RMNO) != "")
            {
                query = query.Where(t => t.RMNO == RMNO);
            }
            if (StringUtils.getString(SDT) != "")
            {
                DateTime sdt = Convert.ToDateTime(SDT);
                DateTime edt = Convert.ToDateTime(EDT);
                query = query.Where(t => t.FIXDT >= sdt && t.FIXDT <= edt);
            }
        }
        if (query.Count() > 0)
        {
            jo.Add("total", query.Count());
            if (param.sort != null)
            {
                query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序        
            }
            if (param.rows > 0)
            {
                query = query.Skip((param.page - 1) * param.rows).Take(param.rows);       //分頁                            
            }
        }
        else
        {
            jo.Add("total", "");
        }

        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"FIXDT",String.Format("{0:yyyy-MM-dd}",item.FIXDT)},                        
                        {"ENDDT",item.ENDDT},
                        {"LOGDT",item.LOGDT},
                        {"LOGUSR",item.LOGUSR},
                        {"FIXAMT",item.FIXAMT},
                        {"NOTES",item.NOTES},
                        {"FIXAREA",item.FIXAREA},
                        {"FIXITEM",item.FIXITEM},
                        {"FIXTP",item.FIXTP},
                        {"ENDNOTE",item.ENDNOTE},
                        {"RMNO",item.RMNO},
                        {"TAGID",item.TAGID}
                    };
            ja.Add(itemObject);
        }
        if (query.Count() > 0)
        {
            jo.Add("rows", ja);
        }
        else
        {
            jo.Add("rows", "");
        }
        return jo;
    }

    /*
    private JArray GetExportData()
    {
        var query = db.TaiwanZipCodes
                      .OrderBy(x => x.Sequence)
                      .ThenBy(x => x.ID)
                      .ThenBy(x => x.Zip);

        JArray jObjects = new JArray();

        foreach (var item in query)
        {
            var jo = new JObject();
            jo.Add("ID", item.ID);
            jo.Add("Zip", item.Zip);
            jo.Add("CityName", item.CityName);
            jo.Add("Town", item.Town);
            jo.Add("Sequence", item.Sequence);
            jObjects.Add(jo);
        }
        return jObjects;
    }*/


    public JArray getReport(String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
    {

        LinqExtensions le = new LinqExtensions();
        var query = (from t in db.FIXM select t);
        if (StringUtils.getString(FIXAREA) == "" && StringUtils.getString(END) == "" && StringUtils.getString(FIXTP) == "" && StringUtils.getString(FIXITEM) == "" && StringUtils.getString(RMNO) == "" && StringUtils.getString(SDT) == "")
        {
            query = query.Where(t => t.RSTA == "xxx");            
        }
        else
        {

            if (StringUtils.getString(FIXAREA) != "")
            {
                int area = 1;
                area = Convert.ToInt16(FIXAREA);
                query = query.Where(t => t.FIXAREA == area);                
            }


            if (StringUtils.getString(END) != "")
            {
                if (END == "1")
                {
                    query = query.Where(t => t.RSTA == " ");
                }
                else if (END == "2")
                {
                    query = query.Where(t => t.RSTA == "Y");
                }
            }
            if (StringUtils.getString(FIXTP) != "")
            {
                query = query.Where(t => t.FIXTP == FIXTP);
            }

            if (StringUtils.getString(FIXITEM) != "")
            {
                query = query.Where(t => t.FIXITEM == FIXITEM);
            }


            if (StringUtils.getString(RMNO) != "")
            {
                query = query.Where(t => t.RMNO == RMNO);
            }
            if (StringUtils.getString(SDT) != "")
            {
                DateTime sdt = Convert.ToDateTime(SDT);
                DateTime edt = Convert.ToDateTime(EDT);
                query = query.Where(t => t.FIXDT >= sdt && t.FIXDT <= edt);
            }
        }

        JArray jObjects = new JArray();
        foreach (var item in query)
        {
            String area="";
            if(item.FIXAREA ==1){
                area="住宅區";
            }else if(item.FIXAREA==2){
                area="公共區";
            }else if(item.FIXAREA==3){
                area="俱樂部";
            }
            var jo = new JObject();
            jo.Add("區域", area.Trim());
            jo.Add("修繕日期", String.Format("{0:yyyy-MM-dd}", item.FIXDT));
            jo.Add("房號", item.RMNO.Trim());
            jo.Add("修繕別", item.FIXTP.Trim());
            jo.Add("修繕項目", item.FIXITEM.Trim());
            jo.Add("費用", item.FIXAMT.ToString());
            jo.Add("備註說明",StringUtils.FilterGdName(item.NOTES.Trim()));
            jo.Add("結案日", String.Format("{0:yyyy-MM-dd}", item.ENDDT));
            jObjects.Add(jo);
           // break;
        }
        return jObjects;


    }


    public int doSave(Frm205Poco param)
    {
        
        int i = 0;
        try
        {
            using (db)
            {
                FIXM obj = (from t in db.FIXM where t.TAGID==param.TAGID select t).Single();                
                obj.FIXDT = DateTimeUtil.getDateTime(param.FIXDT);
                obj.LOGDT = System.DateTime.Now;
                obj.FIXAMT = Convert.ToInt16(param.FIXAMT);
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.FIXAREA = Convert.ToInt16(param.FIXAREA);
                obj.FIXITEM = StringUtils.getString(param.FIXITEM).Trim();
                obj.FIXTP = StringUtils.getString(param.FIXTP).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.RSTA = " ";
                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm205Poco param)
    {

        int i = 0;
        try
        {
            using (db)
            {
                FIXM obj = new FIXM();
                obj.FIXDT = DateTimeUtil.getDateTime(param.FIXDT);
                obj.FIXAREA = Convert.ToInt16(param.FIXAREA);
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR =StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.FIXTP =StringUtils.getString(param.FIXTP).Trim();
                obj.FIXITEM =StringUtils.getString(param.FIXITEM).Trim();
                obj.RMNO = StringUtils.getString(param.RMNO).Trim();
                obj.FIXAMT =Convert.ToInt16(param.FIXAMT);
                obj.RSTA = " ";
                obj.TAGID = cs.getTagidByDatetime();
                db.FIXM.Add(obj);
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public int doSuccess(String TAGID,String ENDNOTE)
    {

        int i = 0;
        try
        {

            i = db.Database.ExecuteSqlCommand("update FIXM set RSTA='Y',ENDDT={0},ENDNOTE={1} where TAGID={2}",  System.DateTime.Now ,ENDNOTE,StringUtils.getString(TAGID).Trim());
        }
        catch (Exception ex)
        {

        }

        return i;
    }


    public int doRemove(String TAGID)
    {

        int i = 0;
        try
        {
            i = db.Database.ExecuteSqlCommand("update FIXM set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now,StringUtils.getString(TAGID).Trim());
        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}