using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetSystem.Poco
{
    public class CakeOrderDialogQueryParamPoco : CakeOrderParamPoco
    {
        public String SDT { get; set; }
        public String EDT { get; set; }
        public String SNO { get; set; }
        public String ENO { get; set; }
        public String MODE { get; set; }  
    }
}