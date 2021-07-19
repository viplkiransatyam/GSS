using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class GasOil : Parameters
    {
        public int GroupID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public int GasTypeID { get; set; }
        public string GasTypeName{ get; set; }
        public float GasTankCapacity { get; set; }
        public string GasOilFormula { get; set; }
        public float StockPrice { get; set; }
        public float OpeningBalance { get; set; }
        public float SystemClosingBalance { get; set; }
        public List<GasOilFormula> GasOilConsumption { get; set; }
    }

    public class GasOilFormula
    {
        public int GasTypeID { get; set; }
        public float GasOilConsmptPercent { get; set; }
    }


}
