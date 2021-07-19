using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class GasPurchaseModel : Parameters
    {
        public int StoreID { get; set; }
        public int TransactionID { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime DueDate { get; set; }
        public string BillOfLading { get; set; }
        public int ShiftCode { get; set; }
        public string InvNo { get; set; }
        public float InvAmount { get; set; }
        public DateTime InvDate { get; set; }
        public string GasInvoiceReceiptType { get; set; } // I for Invoice and R for Receipt

        public List<GasInvoiceItems> GasInventory { get; set; }
        public List<GasInvoiceTax> GasTax { get; set; }
        public List<GasDefaultTax> GasDefaultTax { get; set; }
    }

    public class GasInvoiceTax
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public float TaxAmount { get; set; }
    }

    public class GasInvoiceItems
    {
        public int SlNo { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public float GrossGallons { get; set; }
        public float NetGallons { get; set; }
        public float GrossInvGallons { get; set; }
        public float NetInvGallons { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
    }

    public class GasDefaultTax
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; }
    }

    public class TranType
    {
        public int TransactionTypeID { get; set; }
        public string TransactionTypeName { get; set; }
    }

    public class GasDealerTrans : Parameters
    {
        public int StoreID { get; set; }
        public int TranID { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime ReferenceDate { get; set; }
        public char ApplyFee { get; set; }
        public List<GasDealerTransRows> GasDealerTransRows { get; set; }

    }

    public class GasDealerTransRows
    {
        public int SlNo { get; set; }
        public string ReferenceTranNo { get; set; }
        public DateTime ReferenceTranDate { get; set; }
        public int TranTypeIndicator { get; set; }
        public string TranDescription { get; set; }
        public float TranAmount { get; set; }
    }

    public class SearchTranModel
    {
        public int StoreID { get; set; }
        public int TypeID { get; set; }
        // 1 Credit Transaction
        // 3 Invoice
        public string RefNumber { get; set; }
        public DateTime DateOfTransaction { get; set; }
    }

    public class PurchaseRegisterModel
    {
        public int TranId { get; set; }
        public DateTime InvDate { get; set; }
        public string InvNo { get; set; }
        public DateTime DueDate { get; set; }
        public float Amount { get; set; }
    }

    public class InwardModel
    {
        public string BOLNo { get; set; }
        public DateTime RecDate { get; set; }
        public string GasOilName { get; set; }
        public float InwardGrossQty { get; set; }
        public float InwardNetQty { get; set; }
        public float InvoiceGrossQty { get; set; }
        public float InvoiceNetQty { get; set; }
    }

    public class ReconcillationModel
    {
        public string TransactionType { get; set; }
        public string DealerRefNo { get; set; }
        public DateTime DealerDate { get; set; }
        public float DealerAmount { get; set; }
        public string StoreRefNo { get; set; }
        public DateTime StoreDate { get; set; }
        public float StoreAmount { get; set; }
    }

    public class UpdateReconcillation : Parameters
    {
        public int StoreID { get; set; }
        public int TransactionType { get; set; }
        public List<UpdateReconcillationTran> Reference { get; set; }
    }

    public class UpdateReconcillationTran
    {
        public string ReferenceNo { get; set; }
    }

}

