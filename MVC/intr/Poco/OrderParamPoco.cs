using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetSystem.Poco
{
    public class OrderParamPoco : EasyuiParamPoco 
    {
        public String ODDNO { get; set; }
        public String DEPART { get; set; }
        public String COMP { get; set; }
        public String AID { get; set; }
        public String USERID { get; set; }
        public String SDT { get; set; }
        public String EDT { get; set; }
        public String MODE { get; set; }
        public String DT { get; set; }
        public String Q { get; set; }
    }
}