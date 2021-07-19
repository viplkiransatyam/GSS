using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class ReportGroup
    {
        public int StoreID { get; set; }
        public int GroupID { get; set; }
        public int LedgerID { get; set; }
        public string GroupName { get; set; }
    }

    public class ReportCustomGroup
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public float Debit { get; set; }
        public float Credit { get; set; }
    }

    public class ReportCustomGroupAccounts
    {
        public int LedgerID { get; set; }
        public string LedgerName { get; set; }
        public DateTime Date { get; set; }
        public float Debit { get; set; }
        public float Credit { get; set; }
    }

    public class ReportCustomGroupAccountMaster
    {
        public int LedgerID { get; set; }
        public string LedgerName { get; set; }
    }
}
