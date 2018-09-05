using System;

namespace IntranetSystem.Poco
{
    public class WebsiteUploadPoco
    {
        public int UUID { get; set; }
        public string NAME { get; set; }
        public DateTime SDATE { get; set; }
        public DateTime EDATE { get; set; }
        public string FILE { get; set; }
        public string LINK { get; set; }
        public int SORTNO { get; set; }
        public int AID { get; set; }
        public int HOTELSN { get; set; }
    }
}