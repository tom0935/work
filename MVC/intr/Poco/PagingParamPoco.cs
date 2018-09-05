using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetSystem.Poco
{     
    public class PagingParamPoco
    {
        public int limit { get; set; }
        public int offset { get; set; }

        public String search { get; set; }
        public String order{ get; set; }
        public String sort { get; set; }


        public PagingParamPoco()
        {

            this.search = "";         
            this.order = "asc";
            this.sort = "";
            
        }
    }
}