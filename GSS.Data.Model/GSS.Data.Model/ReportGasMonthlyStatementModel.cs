using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class ReportGasMonthlyStatementModel
    {
        public DataTable GasSale { get; set; }
        public DataTable GasReceipt { get; set; }
        public DataTable ReceivedGallon { get; set; }
        public DataTable PurchasePrice { get; set; }
    }

    public class GasSaleCollection
    {
        public int GasTypeID { get; set; }
        public DateTime Date { get; set; }
        public float SaleGallons { get; set; }
    }

    public class GasCardCollection
    {
        public int GasCardID { get; set; }
        public DateTime Date { get; set; }
        public float CardAmount { get; set; }
    }

    public class GasMasterRecord
    {
        public DateTime Date { get; set; }
        public float SaleAmount { get; set; }
        public float CardTotal { get; set; }
        public float Balance { get; set; }
    }

    public class GasReceived
    {
        public DateTime Date { get; set; }
        public int GasTypeID { get; set; }
        public float GasReceivedQty { get; set; }
        public float GasReceivedPrice { get; set; }
    }
}
