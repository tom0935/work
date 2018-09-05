using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowardCoupon.Models.Dao
{
    interface IEmployeeDao
    {
        List<Hashtable> getEmployee();
        Hashtable getEmployee(String userid);
        Hashtable getEmployee(String userid, String password);
        String getRoleAryStr(String userid);        
    }
}
