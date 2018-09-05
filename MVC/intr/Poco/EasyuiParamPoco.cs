﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetSystem.Poco
{     
    public class EasyuiParamPoco
    {
        public int page { get; set; }
        public int rows { get; set; }
        public String order{ get; set; }
        public String sort { get; set; }
        public EasyuiParamPoco()
        {
            this.page = 1;
            this.rows = 20;
            this.order = "asc";
        }
    }
}