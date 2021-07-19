using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class GameModel
    {
        public string Game { get; set; }
        public string GameName { get; set; }
        public int NoOfTicket { get; set; }
        public float TicketValue { get; set; }
        public float BookValue { get; set; }
        public string StartTicket { get; set; }
        public string EndTicket { get; set; }
    }

    public class ReportLotteryModel
    {
        public DateTime Date { get; set; }
        public string ShiftCode { get; set; }
        public string Game { get; set; }
        public string Pack { get; set; }
        public string GameName { get; set; }
        public string TicketValue { get; set; }
        public string BoxNo { get; set; }
        public string StartTicket { get; set; }
        public string EndTicket { get; set; }
        public string NoOfTicketSold { get; set; }
        public float AmountSold { get; set; }
        public string PackStatus { get; set; }
    }

    public class SaleReport : BaseSale
    {
        public int ShiftCode { get; set; }
        public float BookActive { get; set; }
        public float CashPaid { get; set; }
    }

    public class BaseSale
    {
        public DateTime Date { get; set; }
        public float InstantSale { get; set; }
        public float OnlineSale { get; set; }
        public float TotalSale { get; set; }
        public float OnlineCommission { get; set; }
        public float InstantCommission { get; set; }
        public float CashCommission { get; set; }
    }

    public class DashBoard
    {
        public List<SaleReport> DashboardSaleReport { get; set; }
        public List<SaleReport> DashboardPreviousDaySales { get; set; }
        public List<ReportLotteryModel> PreviousShiftBalance { get; set; }
    }

    public class AutoSettle
    {
        public string GameID { get; set; }
        public string PackNo { get; set; }
        public DateTime AutoSettleDate { get; set; }
        public float BookAmount { get; set; }
        public float SettleAmount { get; set; }
    }

    public class AutoSettlePayment
    {
        public int StoreID { get; set; }
        public DateTime PaymentSweapDate { get; set; }
        public DateTime BusinessEndingDate { get; set; }
        public float TotalPayment { get; set; }
        public List<AutoSettlePaymentBooks> AutoSettlePaymentBooks { get; set; }
    }

    public class AutoSettlePaymentBooks
    {
        public string GameID { get; set; }
        public string PackNo { get; set; }
    }

    public class BooksActiveButNotSettle
    {
        public DateTime Date { get; set; }
        public string GameID { get; set; }
        public string PackNo { get; set; }
        public float BookAmount { get; set; }
        public float SettleAmount { get; set; }
    }

    public class DayReport
    {
        public string DayClosedBy { get; set; }
        public DateTime DayClosedOn { get; set; }
        public DayAbstractSale AbstractSale { get; set; }
        public DetailedDayTrans Balance { get; set; }
        public List<DetailedDayInstantSale> InstantSale { get; set; }
        public List<DayInventory> Inventory { get; set; }
    }

    public class DayAbstractSale
    {
        public float OpeningBalance { get; set; }
        public float Sales { get; set; }
        public float CashTransfer { get; set; }
        public float Paidout { get; set; }
        public float InsystemClosingBalance { get; set; }
        public float PhysicalClosingBalance { get; set; }
    }

    public class DetailedDayTrans
    {
        public float InsystemOpeningBalance { get; set; }
        public float PhysicalOpeningBalance { get; set; }
        public float InsystemClosingBalance { get; set; }
        public float PhysicalClosingBalance { get; set; }
        public float OpeningBalanceShort { get; set; }
        public float ClosingBalanceShort { get; set; }

        public float InsystemCashPaidOut { get; set; }
        public float PhysicalCashPaidOut { get; set; }
        public float TotalCashPaidOut { get; set; }

        public float BankDeposit { get; set; }
        public float BusinessCash { get; set; }
        public float GasCash { get; set; }

    }

    public class DetailedDayInstantSale
    {
        public string GameID { get; set; }
        public string PackNo { get; set; }
        public float TicketValue { get; set; }
        public int NoOfTickets { get; set; }
        public float AmountSold { get; set; }
    }

    public class DayInventory
    {
        public string GameID { get; set; }
        public int NoOfBooksReceived { get; set; }
        public int NoOfBooksActivated { get; set; }
        public int NoOfBooksReturnFromInventory { get; set; }
        public int NoOfBooksReturnFromActive { get; set; }
    }

    public class PaymentDueModel
    {
        public int StoreID { get; set; }
        public DateTime PreviousBusinessEndedDate { get; set; }
        public float TotalBooksActive { get; set; }
        public float OnlineSale { get; set; }
        public float InstantCommission { get; set; }
        public float SaleCommission { get; set; }
        public float CashCommission { get; set; }
        public float InstantPaid{ get; set; }
        public float OnlinePaid { get; set; }
        public float LotteryReturn { get; set; }
        public float TotalDueAmount { get; set; }
        public List<AutoSettle> SettledBooks { get; set; }

    }

    public class GameSaleTrend
    {
        public string GameID { get; set; }
        public float SaleAmount { get; set; }
    }
}
