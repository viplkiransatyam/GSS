using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class ReportMonthlyTotalNormal
    {
        public DataTable Business { get; set; }
        public DataTable Gas { get; set; }
        public DataTable MoneyOrder { get; set; }
        public DataTable Lottery { get; set; }

    }

    public class ReportTranModel
    {
        public string TranType { get; set; }
        public int LedgerID { get; set; }
        public float Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class ReportLotterySection
    {
            public DateTime Date {get;set;}
            public float OnlineSale { get; set; }
            public float InstantSale { get; set; }
            public float OnlinePaid { get; set; }
            public float InstantPaid { get; set; }
            public float Deposit { get; set; }
            public float DepositInBank { get; set; }
    }

    public class BusinessDailySheet
    {
        public DateTime Date { get; set; }
        public float Business { get; set; }
        public float Lottery { get; set; }
        public float Gas { get; set; }
        public float BorrowFromOthers { get; set; }
        public float LotteryPaidout { get; set; }
        public float CreditCard { get; set; }
        public float CashPaid { get; set; }
        public float LoanToOthers { get; set; } //Business Paid
        public float UPAD { get; set; }
        public float BusinessSavings { get; set; }
        public float BankDeposit { get; set; }
        public float CashBalance { get; set; }
        public float CashOnHand { get; set; }
        public float OverShort { get; set; }
        //public float PayoutCheck { get; set; }
    }
}
