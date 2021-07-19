using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class LotteryModel
    {
        public int StoreID { get; set; }
        public int LotteryID { get; set; }
        public string LotteryName { get; set; }
        public int TicketValue { get; set; }
        public int NoOfTickets { get; set; }
        public int BundleAmount { get; set; }
        public int BundleStartNumber { get; set; }
        public int BundleEndNumber { get; set; }
    }

    public class LotteryGames
    {
        public int StoreID { get; set; }
        public string GameID { get; set; }
        public string GameName { get; set; }
        public int NoOfTickets { get; set; }
        public int TicketValue { get; set; }
        public int BookValue { get; set; }
        public int TicketStartNumber { get; set; }
        public int TicketEndNumber { get; set; }
    }

    public class LotteryCommission
    {
        public int StoreID { get; set; }
        public DateTime BusinessEndidngDate { get; set; }
        public float LotteryOnlineCommission { get; set; }
        public float LotteryInstantCommission { get; set; }
        public float LotteryCashCommission { get; set; }
    }

    public class LotterySettlePack
    {
        public int StoreID { get; set; }
        public string GameID { get; set; }
        public string PackNo { get; set; }
    }
    
    public class BookReceive : LotteryGames
    {
        public string PackNo { get; set; }
    }

    public class BookActive : BookReceive
    {
        public string BoxNo { get; set; }
        public string PrevTicketNumber { get; set; }
    }
    
    public class LotteryReturn : Parameters
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
        public string GameID { get; set; }
        public string PackNo { get; set; }
        public int ShiftID { get; set; }
        public char ReturnFrom { get; set; }
        public int LastTicketClosing { get; set; }
    }

    public class LotteryPreviousDaySale
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
        public float PrevDaySale { get; set; }
    }

    public class BoxNumbers
    {
        public int StoreID { get; set; }
        public int BoxNo { get; set; }
        public string BoxDescription { get; set; }
    }


    public class BoxNumbersEmptyBoxes
    {
        public int StoreID { get; set; }
        public int BoxID { get; set; }
        public string BoxDescription { get; set; }
        public string GameID { get; set; }
        public string PackNo { get; set; }
    }

    public class LotteryClosingInstantSale 
    {
        public float InstantSale { get; set; }
        public float CashTransfer { get; set; }
        public List<BoxNumbersEmptyBoxes> BoxNumbers { get; set; }
    }

    public class InventoryReportModel
    {
        public string GameID { get; set; }
        public string GameDescription { get; set; }
        public int Received { get; set; }
        public int Activated { get; set; }
        public int Total { get; set; }
    }

    public class LotteryMapping : Parameters
    {
        public int StoreID { get; set; }
        public int NoOfSlots { get; set; }
        public int AutoSettleDays { get; set; }
        public List<Games> Games { get; set; }
    }

    public class Games
    {
        public string  GameID { get; set; }
    }

}
