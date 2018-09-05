using System;
using System.Collections.Generic;

namespace IntranetSystem.Poco
{
    //多Model
    public class BulletinViewModels
    {
        public List<BulletinPoco> SBD
        {
            get;
            set;
        }

        public List<BulletinPoco> DOC
        {
            get;
            set;
        }
    }

    public class BulletinPoco
    {
        public decimal RN { get; set; }
        public decimal UUID { get; set; }
        public string NAME { get; set; }
        public string CATEGORY { get; set; }
        public DateTime BDATE { get; set; }
        public string PATH { get; set; }
        public string REMARK { get; set; }
        public decimal AID { get; set; }
        public decimal DID { get; set; }
        public decimal BTYPE { get; set; }
    }
}