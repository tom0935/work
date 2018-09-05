using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetSystem.Poco
{
    public class FixCakeOrderParamPoco
    {
        public String UUID { get; set; }
        public Decimal AQTY { get; set; }
        public Decimal COST { get; set; }
        public Decimal PRICE { get; set; }
        public String ADD_REMARK { get; set; }
        public String POSPNM { get; set; }
        public String POSPNO { get; set; }
        public String COMP { get; set; }
        public String DEPT { get; set; }
        public String CR_DT { get; set; }

    }
}