using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  HowardCoupon.Poco
{     
    public class CcPoco
    {
    
        public String UUID { get; set; }
        public String CID { get; set; }
        public String NAME { get; set; }
        public String TYPE { get; set; }
        public String SCOPE { get; set; }
        public String SDT { get; set; }
        public String EDT { get; set; }
        public String PRC { get; set; }
        public String SCHGRATE { get; set; }
        public String MCHGRATE { get; set; }
        public String TEMPLATE { get; set; }
        public String NEW_TEMPLATE { get; set; }
        public String PROTECT { get; set; }
    }
}