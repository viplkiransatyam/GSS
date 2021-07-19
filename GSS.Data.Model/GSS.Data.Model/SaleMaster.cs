using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class SaleMaster : Parameters
    {
        public int StoreID { get; set; }
        public int DayTranID { get; set; }
        public int ShiftCode { get; set; }
        public DateTime Date { get; set; }
        public float TotalSaleGallons { get; set; }
        public float TotalTotalizer { get; set; }
        public float TotalSale { get; set; }
        public float CardTotal { get; set; }
        public float LotteryReturn { get; set; }
        public float LotterySale { get; set; }
        public float LotteryBooksActive { get; set; }
        public float LotteryOnline { get; set; }
        public float LotteryCashInstantPaid { get; set; }
        public float LotteryCashOnlinePaid { get; set; }
        public float LotteryOnlineCommission { get; set; }
        public float LotteryInstantCommission { get; set; }
        public float LotteryCashCommission { get; set; }
        public float LotteryCashSystemOpeningBalance { get; set; }
        public float LotteryCashPhysicalOpeningBalance { get; set; }
        public float LotteryCashSystemClosingBalance { get; set; }
        public float LotteryCashPhysicalClosingBalance { get; set; }
        public float LotteryCashTransfer { get; set; }

        public float CashOpeningBalance { get; set; }
        public float CashPhysicalAtStore { get; set; }
        public float CashDeposited { get; set; }
        public float CashClosingBalance { get; set; }
        public float LotteryInstantPreviousDaySale { get; set; }
        public string EntryLockStatus { get; set; }
        public List<GasSaleModel> GasSale { get; set; }
        public List<GasSaleReceipt> GasReceipt { get; set; }
        public List<GasInventoryUpdate> GasInventory { get; set; }
        public List<LotterySale> LotteryClosingCount { get; set; }
        public List<LotteryTransfer> LotteryTransferList { get; set; }
        public List<AccountPaidReceivables> PaymentAccounts { get; set; }
        public List<AccountPaidReceivables> ReceiptAccounts { get; set; }
        public List<AccountModel> BusinessSaleCollection { get; set; }
        public List<BankDeposit> BankDepositDetail { get; set; }
        public List<Purchase> Purchase{ get; set; }
        public List<Purchase> PurchaseReturn { get; set; }
        public List<ChequeCashingTran> ChequeCashingEntries { get; set; }
        public List<ChequeCashingTran> ChequeDeposits { get; set; }
        public List<GasReceipt> GasInvReceipt { get; set; }
        public List<GasOil> GasTypes { get; set; }
    }

    #region Gas Sale Model
    public class GasSaleModel
    {
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public float Totalizer { get; set; }
        public float SaleGallons { get; set; }
        public float SaleAmount { get; set; }
        public float SalePrice { get; set; }
        public float SetPrice { get; set; }
    }

    public class GasSaleReceipt
    {
        public int CardTypeID { get; set; }
        public string CardName { get; set; }
        public float CardAmount { get; set; }
    }

    public class GasInventoryUpdate
    {
        public int TankID { get; set; }
        public string TankName { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public float OpeningBalance { get; set; }
        public float GasReceived { get; set; }
        public float GasPrice { get; set; }
        public float ActualClosingBalance { get; set; }
        public float SystemClosingBalance { get; set; }
        public float StickInches { get; set; }
        public float StickGallons { get; set; }
        public float TankCapacity { get; set; }

    }

    public class GasReceipt : Parameters
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public string BillOfLading { get; set; }
        public int ShiftCode { get; set; }
        public string InvNo { get; set; }
        public DateTime InvDate { get; set; }
        public int SlNo { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public float GrossGallons { get; set; }
        public float NetGallons { get; set; }
    }

    #endregion

    #region Lotter Sale Model
    public class LotterySale
    {
        public int BoxID { get; set; }
        public string GameID { get; set; }
        public string GameDescription { get; set; }
        public string PackNo { get; set; }
        public int LastTicketClosing { get; set; }
        public int EachTicketValue { get; set; }
        public int NoOfTickets{ get; set; }
    }

    public class LotteryTransfer
    {
        public string TranType { get; set; }
        public float TranAmount { get; set; }
    }

    public class LotteryReceive : Parameters
    {
        public int StoreID { get; set; }
        public int GameID { get; set; }
        public string PackNo { get; set; }
        public string GameName { get; set; }
        public int NoOfTickets { get; set; }
        public DateTime Date { get; set; }
        public int ShiftID { get; set; }
    }

    public class LotteryActive : LotteryReceive
    {
        public int BoxNo { get; set; }
        public int SlNo { get; set; }
    }

    #endregion

    #region Accounts Paid / Receivables
    public class AccountPaidReceivables
    {
        public string AccountTranType { get; set; }
        public int AccountLedgerID { get; set; }
        public float Amount { get; set; }
        public string DisplayName { get; set; }
        public string PaymentType { get; set; }
        public string PaymentRemarks { get; set; }
        public int VoucherNo { get; set; }
    }

    public class JournalVoucher : AccountPaidReceivables
    {
        public int StoreID { get; set; }
        public int ShiftCode { get; set; }
        public DateTime Date { get; set; }
        public string VoucherType { get; set; }
        public string CreatedUserName { get; set; }
    }

    #endregion

    public class OnlyMaster
    {
        public int StoreID { get; set; }
        public int DayTranID { get; set; }
        public int ShiftCode { get; set; }
        public DateTime Date { get; set; }
        public float TotalSaleGallons { get; set; }
        public float TotalTotalizer { get; set; }
        public float TotalSale { get; set; }
        public float CardTotal { get; set; }
        public float LotteryReturn { get; set; }
        public float LotterySale { get; set; }
        public float LotteryBooksActive { get; set; }
        public float LotteryOnline { get; set; }
        public float LotteryCashInstantPaid { get; set; }
        public float LotteryCashOnlinePaid { get; set; }
        public float LotteryOnlineCommission { get; set; }
        public float LotteryInstantCommission { get; set; }
        public float LotteryCashCommission { get; set; }
        public float LotteryCashSystemOpeningBalance { get; set; }
        public float LotteryCashPhysicalOpeningBalance { get; set; }
        public float LotteryCashSystemClosingBalance { get; set; }
        public float LotteryCashPhysicalClosingBalance { get; set; }
        public float LotteryCashTransfer { get; set; }

        public float CashOpeningBalance { get; set; }
        public float CashPhysicalAtStore { get; set; }
        public float CashDeposited { get; set; }
        public float CashClosingBalance { get; set; }
        public float LotteryInstantPreviousDaySale { get; set; }
    }

    public class ChequeCashingTran
    {
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public float ChequeAmount { get; set; }
        public float PaidAmount { get; set; }
        public float Commission { get; set; }
        public string Remarks { get; set; }
        public DateTime ChequeDepositDate { get; set; }
        public long ChequeTranID { get; set; }

    }

    public class AccountEntry
    {
        public int StoreID { get; set; }
        public int TranID { get; set; }
        public string TranType { get; set; } // DR / CR
        public int LedgerID { get; set; }
        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public string Remarks { get; set; }
        public int DayTranID { get; set; }
    }

    #region Bank Deposit Form

    public class BankDepositForm
    {
        public float CashOpeningBalance { get; set; }
        public float CashInHand { get; set; }
        public float DepositedInBank { get; set; }
        public float CashClosingBalance { get; set; }
        public List<BankDeposit> LedgerDetail { get; set; }
    }

    public class BankDeposit
    {
        public int LedgerID { get; set; }
        public string LedgerName { get; set; }
        public float LedgerSale { get; set; }
        public float LedgerPaid { get; set; } // Paid out from Lottery sale or business sale or Gas sale
        public float Balance { get; set; }
        public float Deposit { get; set; }
    }

    #endregion

    public class Purchase
    {
        public int SupplierNo;
        public string SupplierName;
        public DateTime InvCrdDate;
        public string InvCrdNumber;
        public float InvCrdAmount;
        public DateTime DueDate;
        public string Remarks;
    }

    public class NextShift
    {
        public DateTime NextDate { get; set; }
        public int NextShiftID { get; set; }
        public int NextDayTranID { get; set; }
    }

    public class LotteryRunningShift
    {
        public DateTime CurrentDate { get; set; }
        public int ShiftCode { get; set; }
        public int StoreID { get; set; }
        public float SystemOpeningBalance { get; set; }
    }
}
