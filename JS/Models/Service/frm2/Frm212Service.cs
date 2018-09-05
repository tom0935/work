
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
public class Frm212Service 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

   
    public JObject getDG1(String q)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();        
        //DateTime dt = Convert.ToDateTime(DT);
        var query = (from t in db.PARKFEE select t);

        if (StringUtils.getString(q) == "")
        {
            query = query.Where(t => t.AMTSUM == 0);
        }
        else
        {
            query = query.Where(t => t.RSTA == " " && t.FEEYM== q).OrderBy(t => t.FEEYM);
        }
        foreach (var item in query)
        {
            var itemObject = new JObject
                    {                                           
                        {"AMTSUM",item.AMTSUM},
                        {"AMTTX",item.AMTTX},
                        {"FEEDT",String.Format("{0:yyyy-MM-dd}" , item.FEEDT)},
                        {"FEEAMT",item.FEEAMT},
                        {"FEEYM",item.FEEYM},
                        {"LOGUSR",item.LOGUSR},
                        {"NOTES",item.NOTES},
                        {"VNO1",item.VNO1},
                        {"VNO2",item.VNO2},
                        {"TAGID",item.TAGID}
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

  


  


    public int doSave(Frm212Poco param)
    {
        
        int i = 0;
        try
        {
                        

            using (db)
            {
                PARKFEE obj = (from t in db.PARKFEE where t.TAGID == param.TAGID select t).Single();                
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);

                if (obj.AMTSUM > 0)
                {
                    int txrat = Convert.ToInt32(cs.getConfig("TXRAT"));
                    obj.FEEAMT = Convert.ToInt32(Math.Round((Decimal)obj.AMTSUM * 100 / (100 + txrat), 0));
                    obj.AMTTX = obj.AMTSUM - obj.FEEAMT;
                }
                        
                obj.VNO1 = StringUtils.getString(param.VNO1).Trim();
                obj.VNO2 = StringUtils.getString(param.VNO2).Trim();
                obj.FEEDT = DateTimeUtil.getDateTime(param.FEEDT);
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();                

                i= db.SaveChanges();
            }
        }catch (Exception ex){

        }

        return i;
    }


    public int doAdd(Frm212Poco param)
    {

        int i = 0;
        try
        {
            
            
            using (db)
            {
                PARKFEE obj = new PARKFEE();
                obj.AMTSUM = Convert.ToInt32(param.AMTSUM);

                if (obj.AMTSUM > 0)
                {
                    int txrat=Convert.ToInt32(cs.getConfig("TXRAT"));
                    obj.FEEAMT = Convert.ToInt32(Math.Round((Decimal)obj.AMTSUM * 100 / (100 + txrat), 0));
                    obj.AMTTX = obj.AMTSUM - obj.FEEAMT;                                        
                }

                obj.FEEDT = DateTimeUtil.getDateTime(param.FEEDT);
                obj.FEEYM = StringUtils.getString(param.FEEYM).Trim();
                obj.LOGDT = System.DateTime.Now;
                obj.LOGUSR = StringUtils.getString(param.LOGUSR).Trim();
                obj.NOTES = StringUtils.getString(param.NOTES).Trim();
                obj.VNO1 = StringUtils.getString(param.VNO1).Trim();
                obj.VNO2 = StringUtils.getString(param.VNO2).Trim();
                obj.RSTA = " ";
                
                obj.TAGID = cs.getTagidByDatetime();
                db.PARKFEE.Add(obj);
                i = db.SaveChanges();
            }
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
            i = db.Database.ExecuteSqlCommand("update PARKFEE set RSTA='C',ENDDT={0} where TAGID={1}", System.DateTime.Now, StringUtils.getString(TAGID).Trim());

        }
        catch (Exception ex)
        {

        }

        return i;
    }
   


}
}