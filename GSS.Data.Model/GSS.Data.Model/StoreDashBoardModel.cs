using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class GroupDashBoardModel
    {
        public List<StoreDashBoardModel> GroupDashboard { get; set; }
    }


    public class StoreDashBoardModel
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }

        public List<GraphData> BusinessSale { get; set; }
        public List<GraphData> GasSale { get; set; }
        public List<GraphData> LotterySale { get; set; }
        public List<GraphData> MoneyOrderSale { get; set; }
    }

    public class GraphData
    {
        public string Date { get; set; }
        public float Sale { get; set; }
    }
}
