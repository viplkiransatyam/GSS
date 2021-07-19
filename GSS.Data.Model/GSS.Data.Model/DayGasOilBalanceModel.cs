using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class DayGasOilSaleModel
    {
        public int StoreID { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public DateTime Date { get; set; }
        public long Totalizer { get; set; }
        public int SaleGallons { get; set; }
        public float Amount { get; set; }
        public float Price { get; set; }
    }

    public class DayGasOilBalanceModel
    {
        public int StoreID { get; set; }
        public int TankID { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public DateTime Date { get; set; }
        public float OpeningBalance { get; set; }
        public float GasReceived { get; set; }
        public float ActualClosingBalance { get; set; }
        public float StickInches { get; set; }
        public float StickGallons { get; set; }
    }

    public class GasOpeningBalance
    {
        public int StoreID { get; set; }
        public int TankID { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public float TankCapacity { get; set; }
        public DateTime Date { get; set; }
        public float OpeningBalance { get; set; }
    }


    /// <summary>
    /// Created for displaying Gas System closing balance
    /// </summary>
    public class GasSystemClosingBalance
    {
        public int StoreID { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName { get; set; }
        public float SaleQty { get; set; }
        public float GasOpeningBalance { get; set; }
        public float Received { get; set; }
        public float ActualClosingBalance { get; set; }
        public float SystemClosingBalance { get; set; }
        public float ShortOrExcess { get; set; }
        public List<GasOilFormula> ConsumedPercent { get; set; }
    }
   
}
