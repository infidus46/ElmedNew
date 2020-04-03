using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yamed.Reports
{
    public class ReportsClass
    {
        public static string PaymentId;
    }



    public class ReportParams
    {
        public int? ID { get; set; }
        public string s_dates { get; set; }
        public string IDS { get; set; }
        public int? user { get; set; }
        public int? M1 { get; set; }
        public int? M2 { get; set; }
        public int? Y1 { get; set; }
        public int? Y2 { get; set; }
        public DateTime? beg_date { get; set; }
        public DateTime? end_date { get; set; }
        public int? ReqID { get; set; }
        public string num_act { get; set; }

        // LPU
        public string smo { get; set; }

        public string dn { get; set; }
        public DateTime? dd { get; set; }
        public string kb { get; set; }

        public int? os { get; set; }
    }


}
