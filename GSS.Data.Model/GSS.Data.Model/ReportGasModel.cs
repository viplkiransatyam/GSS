using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class DayEndModel
    {
        public List<DaySale> DaySales { get; set; }
        public List<DayCardReceipt> DayCardReceipts { get; set; }
        public List<DayStock> DayStocks { get; set; }
        public List<DayPurchase> DayPurchases { get; set; }
    }

    public class DaySale
    {
        public int ShiftCode { get; set; }
        public string GasType { get; set; }
        public float SaleQty { get; set; }
        public float SaleAmount { get; set; }
        public float UnitPrice { get; set; }
        public float SetPrice { get; set; }
    }

    public class DayCardReceipt
    {
        public int ShiftCode { get; set; }
        public string CardName { get; set; }
        public float CardAmount { get; set; }
    }

    /// <summary>
    /// To display closing and previous week Profit or Loss from each gas gap.
    /// </summary>
    public class DashboardPnLRecord
    {
        public string GasType { get; set; }
        public float DayProfit { get; set; }
        public float WeekProfit { get; set; }
    }

    public class DashboardProfitAndLoss
    {
        public DateTime Date { get; set; }
        public int ShiftCode { get; set; }
        public List<DashboardPnLRecord> Data { set; get; }
    }

    public class DayStock
    {
        public int ShiftCode { get; set; }
        public DateTime Date { get; set; }
        public int GasTypeID { get; set; }
        public string GasType { get; set; }
        public float OpenQty { get; set; }
        public float InwardQty { get; set; }
        public float SaleQty { get; set; }
        public float ClosingQty { get; set; }
        public float SystemClosingQty { get; set; }
        public float ShortOver { get; set; }
        public float SalePrice { get; set; }
        public float PurchasePrice { get; set; }
        public float ProfitLoss { get; set; }

    }

    public class DayPurchase
    {
        public string InvoiceNo { get; set; }
        public int ShiftID { get; set; }
        public string GasType { get; set; }
        public float InwardQty { get; set; }
        public float InvoiceQty { get; set; }
        public float InvoiceAmount { get; set; }
        public DateTime DueDate { get; set; }

    }


    public class ReconcilationStatement
    {
        public string Date { get; set; }
        public float CardAmount { get; set; }
        public float PaidAmount { get; set; }
        public float GSSInvoiceAmount { get; set; }
        public float CreditTransaction { get; set; }
        public float Discount { get; set; }
        public float Fee { get; set; }
        public float VendorInvoiceAmount { get; set; }

    }

    public class ReportSaleTrendModel
    {
        public string GasOilType { get; set; }
        public float Sale { get; set; }
    }

    public class ReportGasItem
    {
        public DateTime Date { get; set; }
        public int ShiftCode { get; set; }
        public float Unlead { get; set; }
        public float MidGrade { get; set; }
        public float Premium { get; set; }
        public float Diesel { get; set; }
        public float Kirosene { get; set; }
        public float NonEthnol { get; set; }
        public float Total { get; set; }
    }

    public class ReportMonthlyStatement
    {
        public DateTime Date { get; set; }
        public int ShiftCode { get; set; }
        public float Sales { get; set; }
        public float CardPayment { get; set; }
        public float Discount { get; set; }
        public float TotalPayments { get; set; }
        public float PaidAmount { get; set; }
        public float PurchaseValue { get; set; }
        public float BankCharges { get; set; }
        public float TotalPayables { get; set; }

    }

    public class ReportInventory
    {
        public DateTime SaleDate { get; set; }
        public int ShiftCode { get; set; }
        public string Description { get; set; }
        public float Unlead { get; set; }
        public float MidGrade { get; set; }
        public float Premium { get; set; }
        public float Diesel { get; set; }
        public float Kirosene { get; set; }
        public float NonEthnol { get; set; }
    }

    public class SaleGraph
    {
        public DateTime SaleDate { get; set; }
        public int ShiftCode { get; set; }
        public float SaleAmount { get; set; }
        public float SaleGallons { get; set; }
        public float Profit { get; set; }
    }

    public class Dashboard
    {
        public ReportGasItem Sales { get; set; }
        public List<ReportInventory> Inventory { get; set; }
        public List<SaleGraph> SaleGraph { get; set; }
    }

}
