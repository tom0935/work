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
    interface IMenuDao
    {

        List<Hashtable> getMenuItem(String userid,String[] roleid);
        JObject getMenuList();
        JObject getMenuRoleList(EasyuiParamPoco param ,String UUID);
        JObject getMenuUserList(EasyuiParamPoco param, String UUID);
        JObject getCodeList(EasyuiParamPoco param ,String UUID);
    }
}
