using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jasper.Models.Poco
{
    public class Frm5132Poco 
    {

  /*  @"select CAST(CAST(substring(h.rmno,1,2) as decimal) as varchar) + 'F - 1' +substring(h.rmno,3,2)  as RMTXT,
                    r.PINS,v.MAN,h.DT1,h.DT2,v.CNTRY,
                    t.TELNO,t.TELTP

*/

        
        public String RMTXT { get; set; }
        public String PINS { get; set; }
        public String MAN { get; set; }     
        public String DT1 { get; set; }
        public String DT2 { get; set; }
        public String DEALERNM { get; set; }
        
        public String CNTRY { get; set; }
        public String TELNO { get; set; }
        public String TELTP { get; set; }

    }
}