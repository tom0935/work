using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetSystem.Poco
{
    public class OrderFixParamPoco : DbContext
    {
        public DbSet<OrderFixParamPoco> Fix { get; set; }        
    } 

    public class Fix
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Decimal RNO { get; set; }

        public String ODDNO { get; set; }
        public DateTime BDATE { get; set; }
        public String BUSER { get; set; }
        public String COMP { get; set; }
        public String DEPT { get; set; }
        public String STATUS { get; set; }
        public DateTime SDATE { get; set; }
        public DateTime ODATE { get; set; }
        public String OUSER { get; set; }
        public DateTime PDATE { get; set; }
        public String PUSER { get; set; }
        public DateTime FDATE { get; set; }
        public String FUSER { get; set; }
        public String USER_NAME { get; set; }
        public String COMP_NAME { get; set; }
        public String DEPT_NAME { get; set; }
        public String O_NAME { get; set; }
        public Decimal COST { get; set; }
        public String PDNO { get; set; }
        public String POSNO { get; set; }
        public String POSPNM { get; set; }
        public Decimal PRICE { get; set; }
        public Decimal QTY { get; set; }
        public String REMARK { get; set; }
        public Decimal SQTY { get; set; }
        public String DTL_STATUS { get; set; }
        public Decimal UUID { get; set; }
        public String CAKE_REMARK { get; set; }
        public Decimal AQTY { get; set; }
        public String QTY_TYPE { get; set; }

    }
}