using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class StoreCard : Parameters
    {
        public int StoreID { get; set; }
        public int CardType { get; set; }
        public string CardName { get; set; }
        public char CardCreditType { get; set; }
    }
    
    public class ReportStoreCard : Parameters
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public int CardType { get; set; }
        public string CardName { get; set; }
        public string CardCreditType { get; set; }
    }
}
