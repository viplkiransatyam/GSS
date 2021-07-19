using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class ReportLedgerModel
    {
        public DateTime Date { get; set; }
        public string LedgerName { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public float Balance { get; set; }
        public string BalanceType { get; set; }
        public string Remarks { get; set; }
    }

    public class ReportGasBalanceSheet
    {
        public float OpeningStock { get; set; }
        public float Purchase { get; set; }
        public float GrossProfit { get; set; }
        public float GrossLoss { get; set; }
        public float Sales { get; set; }
        public float ClosingStock { get; set; }
        public float Total { get; set; }
    }
}
