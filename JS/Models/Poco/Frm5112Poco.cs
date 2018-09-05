using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jasper.Models.Poco
{
    public class Frm5112Poco 
    {

  /*  @"select CAST(CAST(substring(h.rmno,1,2) as decimal) as varchar) + 'F - 1' +substring(h.rmno,3,2)  as RMTXT,
                    r.PINS,v.MAN,h.DT1,h.DT2,v.CNTRY,
                    t.TELNO,t.TELTP

*/

        
        public Int64 RID { get; set; }
        public String MM { get; set; }
        public String YY { get; set; }     
        public String RMNO { get; set; }
        public decimal PINS { get; set; }
        public String DEALERNM { get; set; }
        
        public String DT2STR { get; set; }

        public DateTime DT2 { get; set; }    
    }
}