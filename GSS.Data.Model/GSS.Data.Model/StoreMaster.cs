using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class StoreMaster : Parameters
    {
        public int GroupID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress1 { get; set; }
        public string StoreAddress2 { get; set; }
        public string State { get; set; }
        public int NoOfTanks { get; set; }
        public int NoOfShifts { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StoreType { get; set; }
        public string AvailGas { get; set; }
        public string AvailLottery { get; set; }
        public string AvailBusiness { get; set; }
        public int LotteryAutoSettleDays { get; set; }

        public List<TankMaster> TankDetail { get; set; }
    }

    public class Store
    {
        public int GroupID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
    }

    public class StoreUser : Parameters
    {
        public int GroupID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public char AccessType { get; set; }
        public char ActiveStatus { get; set; }
    }

    public class StoreShift
    {
        public int StoreID { get; set; }
        public int ShiftCode { get; set; }
    }

    public class ParameterList
    {
        public List<ParameterMaster> ParameterOpeningBalanceList{ get; set; }
    }

    public class ParameterMaster 
    {
        public int StoreID { get; set; }
        public int ParameterID { get; set; }
        public string ParameterName{ get; set; }
        public float ParameterValue { get; set; }
    }

}
