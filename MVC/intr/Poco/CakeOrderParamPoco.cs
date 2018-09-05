using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetSystem.Poco
{
    public class CakeOrderParamPoco : EasyuiParamPoco
    {
        public String TYPE1 { get; set; }
        public String TYPE2 { get; set; }
        public String TYPE3 { get; set; }
        public String TYPE4 { get; set; }
        public String SCODE { get; set; }
        public String SNAME { get; set; }


        public String SDT { get; set; }
        public String EDT { get; set; }
        public String SNO { get; set; }
        public String ENO { get; set; }
        public String MODE { get; set; }
        public String RDO { get; set; }  
    }
}