using GSS.BusinessLogic.Layer;
using GSS.Data.Model;
using GSS.DataAccess.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GSS.UI.Layer.Controllers
{
    public class LotteryController : ApiController
    {
        /// <summary>
        /// This is Old Lottery Module
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetLotteryMaster(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<LotteryModel> objLotteryMaster = _dalLotteryMaster.SelectLotterMaster(ID);

                return Ok(objLotteryMaster);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        /// <summary>
        /// This is updated Lottery Module
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetLotteryGames(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<LotteryGames> objLotteryMaster = _dalLotteryMaster.GetLotteryGames(ID);

                return Ok(objLotteryMaster);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetApplicableLotteryGames(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<LotteryGames> objLotteryMaster = _dalLotteryMaster.GetApplicableLotteryGames(ID);

                return Ok(objLotteryMaster);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetLotteryBoxes(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<BoxNumbers> objLotteryBox = _dalLotteryMaster.GetLotteryBoxes(ID);

                return Ok(objLotteryBox);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryReceive(SelectLottery objLottery)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<LotteryReceive> objLotteryReceive = _dalLotteryMaster.GetLotteryReceive(objLottery.StoreID, objLottery.Date);

                return Ok(objLotteryReceive);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryActive(SelectLottery objLottery)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<LotteryActive> objLotteryReceive = _dalLotteryMaster.GetLotteryActive(objLottery.StoreID, objLottery.Date);

                return Ok(objLotteryReceive);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryReturn(SelectLottery objLottery)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<LotteryReturn> objLotteryReceive = _dalLotteryMaster.GetLotteryReturn(objLottery.StoreID, objLottery.Date);

                return Ok(objLotteryReceive);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult ScanTicket(ScanTicket obj)
        {
            try
            {
                LotteryMaster _dalLottery = new LotteryMaster();
                BookReceive objLottery = new BookReceive();
                objLottery = _dalLottery.ScanBookReceive(obj.StoreID, obj.ScanSerial);

                return Ok(objLottery);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult ScanTicketForClosingRead(ScanTicket obj)
        {
            try
            {
                LotteryMaster _dalLottery = new LotteryMaster();
                BookActive objLottery = new BookActive();
                objLottery = _dalLottery.ScanBookActive(obj.StoreID, obj.ScanSerial);

                return Ok(objLottery);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetPreviousDayInstantSale(LotteryPreviousDaySale obj)
        {
            try
            {
                LotteryMaster _dalLottery = new LotteryMaster();
                obj = _dalLottery.GetPrevDaySale(obj);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Oil Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddBooksReceive(List<LotteryReceive> objLotteryBooksReceipt)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.AddBooksReceive(objLotteryBooksReceipt);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddBooksActive(List<LotteryActive> objLotteryBooksActive)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                LotteryValidation _valLottery = new LotteryValidation();

                if (_valLottery.BookActive(objLotteryBooksActive))
                    _dalLotteryMaster.AddBooksActive(objLotteryBooksActive);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddLotteryReturn(List<LotteryReturn> objLotteryReturn)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.AddLotteryReturn(objLotteryReturn);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage ActivateLotteryModule(LotteryMapping objLotteryMapping)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.ActivateLotteryModule(objLotteryMapping);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddGame(LotteryGames objGame)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.AddLotteryGame(objGame);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetInventoryReport(int ID)
        {
            try
            {
                // ID - Store ID
                ReportLottery _dalLottery = new ReportLottery();
                List<InventoryReportModel> InventoryReport = _dalLottery.InventoryReport(ID);

                return Ok(InventoryReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetAutoSettle(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<AutoSettle> objLottery = _dalLotteryMaster.SelectAutoSettle(ID);

                return Ok(objLottery);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult SelectBooksActiveButNotSettled(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                List<BooksActiveButNotSettle> objLottery = _dalLotteryMaster.SelectBooksActiveButNotSettled(ID);

                return Ok(objLottery);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetPaymentDue(int ID)
        {
            try
            {
                // ID - Store ID
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                PaymentDueModel objLottery = _dalLotteryMaster.GetLotteryPaymentDue(ID);

                return Ok(objLottery);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateLotteryPayment(AutoSettlePayment objLottery)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.UpdatePayment(objLottery);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateCommission(LotteryCommission objLottery)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.UpdateCommission(objLottery);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateLotterySettle(List<LotterySettlePack> objLottery)
        {
            try
            {
                LotteryMaster _dalLotteryMaster = new LotteryMaster();
                _dalLotteryMaster.UpdateLotterySettle(objLottery);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult UnsavedClosingReading(int ID)
        {
            try
            {
                // ID - Store ID
                ReportLottery _dalLottery = new ReportLottery();
                List<LotterySale> objLottery = _dalLottery.UnsavedClosingReading(ID);

                return Ok(objLottery);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }


        #region Reports

        [HttpPost]
        public IHttpActionResult GetLotteryDailyReport(SelectLotteryReport objLottery)
        {
            try
            {
                
                ReportLottery reportLottery = new ReportLottery();
                List<ReportLotteryModel> DetailedSaleReport = reportLottery.DetailedSaleReport(objLottery.StoreID, objLottery.FromDate, objLottery.ToDate);


                return Ok(DetailedSaleReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryBookActiveReport(SelectLotteryReport objLottery)
        {
            try
            {

                ReportLottery reportLottery = new ReportLottery();
                List<ReportLotteryModel> DetailedBookReport = reportLottery.BookActive(objLottery.StoreID, objLottery.FromDate, objLottery.ToDate);

                return Ok(DetailedBookReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryBookReceiveReport(SelectLotteryReport objLottery)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                List<ReportLotteryModel> DetailedBookReport = reportLottery.BookReceive(objLottery.StoreID, objLottery.FromDate, objLottery.ToDate);

                return Ok(DetailedBookReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryBookReturnReport(SelectLotteryReport objLottery)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                List<ReportLotteryModel> DetailedBookReport = reportLottery.BookReturn(objLottery.StoreID, objLottery.FromDate, objLottery.ToDate);

                return Ok(DetailedBookReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotterySaleReport(SelectLotteryReport objLottery)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                List<SaleReport> DetailedBookReport = reportLottery.SaleReport(objLottery.StoreID, objLottery.FromDate, objLottery.ToDate);

                return Ok(DetailedBookReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryMonthlySaleReport(SelectLottery objLottery)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                List<BaseSale> MonthlySale = reportLottery.MonthlySaleReport(objLottery.StoreID, objLottery.Date.Month, objLottery.Date.Year);

                return Ok(MonthlySale);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryDayEndReport(SelectLotteryDay objLottery)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                DayReport objDayReport = reportLottery.DayEndReport(objLottery.StoreID, objLottery.Date, objLottery.ShiftID);

                return Ok(objDayReport);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetLotteryGameSaleTrend(SelectLottery objLottery)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                List<GameSaleTrend> MonthlySale = reportLottery.GameSaleTrend(objLottery.StoreID, objLottery.Date.Month, objLottery.Date.Year);

                return Ok(MonthlySale);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetDashboard(int ID)
        {
            try
            {
                ReportLottery reportLottery = new ReportLottery();
                DashBoard dashBoard = reportLottery.GetDashBoard(ID);

                return Ok(dashBoard);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetRunningShift(int ID)
        {
            try
            {
                LotteryMaster reportLottery = new LotteryMaster();
                LotteryRunningShift runningShift = reportLottery.GetRunningShift(ID);

                return Ok(runningShift);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }
        
        #endregion


    }

    public class ScanTicket
    {
        public int StoreID { get; set; }
        public string ScanSerial { get; set; }
    }

    public class SelectLottery
    {
        public int StoreID { get; set; }
        public DateTime Date{ get; set; }
    }

    public class SelectGas
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
    }

    public class SelectLotteryDay
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
        public int ShiftID { get; set; }
    }
    
    public class SelectLotteryReport
    {
        public int StoreID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
