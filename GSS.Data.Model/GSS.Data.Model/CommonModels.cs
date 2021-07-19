using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class SearchModel
    {
        public int SearchKey { get; set; }
        public string SearchValue { get; set; }
    }

    public class SearchGasOil
    {
        public int StoreID { get; set; }
        public int TankID { get; set; }
        public DateTime Date { get; set; }
    }

    public class RepMonth
    {
        public List<string> MonthName;
        public List<int> YearNo;
    }

    public enum SearchKeys
    {
        SEARCH_BY_GROUPID = 0,
        SEARCH_BY_USERID = 1
    }
}
