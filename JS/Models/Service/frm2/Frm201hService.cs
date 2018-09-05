
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
public class Frm201hService 
{
    private RENTEntities db = new RENTEntities();
    private CommonService cs = new CommonService();

    public String getDealer(String CNO)
    {
        JArray ja = new JArray();
        String rtn=null;
        var query = (from t in db.DEALER where t.CNO == CNO  select t).SingleOrDefault();
        if (query != null) {
            rtn =StringUtils.getString(query.CPMGR).Trim();
        }
        return rtn;
    }


    public int doSave(Frm201hPoco param)
    {
        int i = 0;
        DateTime? tmp = null;
        
        using (db)
        {
            switch (param.TYPE)
            {
                case "01":   //更換合約條文
                    
                    break;
                case "02":    //更換公司名稱                                           
                    int j1 = db.Database.ExecuteSqlCommand("update CONTRAH set DEALERNM = {0} where TAGID={1} and DEALERNM={2}", param.TX2, param.TAGID,param.TX1); 
                    int j2 = db.Database.ExecuteSqlCommand("update CONTRAP set DEALERNM = {0} where JOBTAG={1} and DEALERNM={2}", param.TX2, param.TAGID, param.TX1);
                    int j3 = db.Database.ExecuteSqlCommand("update CONTRAF set DEALERNM = {0} where JOBTAG={1} and DEALERNM={2}", param.TX2, param.TAGID, param.TX1);
                    int j4 = db.Database.ExecuteSqlCommand("update CONTRAC set DEALERNM = {0} where JOBTAG={1} and DEALERNM={2}", param.TX2, param.TAGID, param.TX1);
                    int j5 = db.Database.ExecuteSqlCommand("update CONTRAA set DEALERNM = {0} where JOBTAG={1} and DEALERNM={2}", param.TX2, param.TAGID, param.TX1);

                    break;
                case "03":  //更換負責人
                    DEALER obj3= (from t in db.DEALER where t.CNO==param.DEALNO select t).Single();
                    obj3.CPMGR = param.TX2;
                    break;
                case "04":
                    CONTRAH obj4= (from t in db.CONTRAH where t.TAGID ==param.TAGID select t).Single();
                    obj4.MLIVER = param.TX2;
                    break;
                case "05":
                    break;
            }
            UPDNOTE obj = new UPDNOTE();
            obj.CNO = param.CNO;
            obj.UPDTP = param.UPDTP;
            obj.UPDDT = StringUtils.getString(param.UPDDT) == "" ? tmp : DateTimeUtil.getDateTime(param.UPDDT);
            obj.NOTES = param.NOTES;
            obj.LOGUSR = param.LOGUSR;
            obj.LOGDT = System.DateTime.Now;
            obj.JOBTAG = param.TAGID;
            obj.TAGID = cs.getTagidByDatetime();
            obj.CNNO = param.CNNO;
            obj.CNTAG = param.TAGID;
            db.UPDNOTE.Add(obj);
            i = db.SaveChanges();
        }


        return i;
    }

}
}