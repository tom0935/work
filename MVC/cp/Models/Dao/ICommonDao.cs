using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HowardCoupon.Poco;

namespace HowardCoupon.Models.Dao
{
    interface ICommonDao
    {

        List<Hashtable> getCodeList(EasyuiParamPoco param, String kind, String type);
        List<Hashtable> getCodeList( String kind, String type);
        Hashtable getCode(String kind,String type,String code);
       
    }
}
