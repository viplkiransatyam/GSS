using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class AccountModel : Parameters
    {
        public int StoreID { get; set; }
        public int LedgerID { get; set; }
        public string DisplayStatus { get; set; }
        public string LedgerName { get; set; }
        public int GroupID { get; set; }
        public int SalesGroupID { get; set; }
        public string SalesGroupName { get; set; }
        public string DisplaySide { get; set; }
        public string Remarks { get; set; }
        public float OpeningBalance { get; set; }
        public string OpeningBalanceType { get; set; }
        public char ActiveStatus { get; set; }

        // Created for amounts to return on Business form
        public string GroupType { get; set; }
        public string DisplayType { get; set; }
        public string PaymentType { get; set; }
        public float Amount { get; set; }
        public DateTime Date { get; set; }
    }


}
