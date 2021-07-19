using GSS.Data.Model;
using GSS.DataAccess.Layer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GSS.UI.Layer.Controllers
{
    public class ReportController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetMonthYear(int ID)
        {
            try
            {
                // ID - Store ID
                ReportMonthlyTotal _dalReport = new ReportMonthlyTotal();
                return Ok(_dalReport.GetMonthYear(ID.ToString()));
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
        public IHttpActionResult GetMonthlyStatement(SelectDate objSelectDay)
        {
            try
            {
                ReportMonthlyTotalNormal objMonthlyTotal = new ReportMonthlyTotalNormal();
                ReportMonthlyTotal RepMonthTot = new ReportMonthlyTotal();
                objMonthlyTotal = RepMonthTot.GetMonthlyStatementNormal(objSelectDay.StoreID, objSelectDay.Month, objSelectDay.Year);
                return Ok(objMonthlyTotal);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Login"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetGasProfitLoss(SelectLedger objSelectDay)
        {
            try
            {
                ReportGas objGas = new ReportGas();
                List<DayStock> objDayStockColl = new List<DayStock>();
                objDayStockColl = objGas.ReturnProfitLoss(objSelectDay.StoreID, objSelectDay.FromDate, objSelectDay.ToDate, objSelectDay.LedgerID);
                return Ok(objDayStockColl);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Profit Loss"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetDashboardProfitAndLoss(SelectLedger request)
        {
            DashboardProfitAndLoss response = new DashboardProfitAndLoss();
            try
            {
                LotteryRunningShift runningShift = new GasOilDal().GetRunningShift(request.StoreID);
                if (runningShift != null && runningShift.CurrentDate != null)
                {
                    response.Date = runningShift.CurrentDate;
                    response.ShiftCode = runningShift.ShiftCode;
                    response.Data = new List<DashboardPnLRecord>();

                    DateTime lastWeek = runningShift.CurrentDate.AddDays(-6);

                    List<GasOil> gastypeList = new GasOilDal().SelectRecords(request.StoreID);
                    foreach (GasOil gasType in gastypeList)
                    {
                        if (gasType.GasTypeID != 2)
                        {
                            List<DayStock> dayStockList = new ReportGas().ReturnProfitLoss(request.StoreID, runningShift.CurrentDate.AddDays(-1), runningShift.CurrentDate.AddDays(-1), gasType.GasTypeID);
                            List<DayStock> weekStockList = new ReportGas().ReturnProfitLoss(request.StoreID, lastWeek, runningShift.CurrentDate, gasType.GasTypeID);

                            DashboardPnLRecord record = new DashboardPnLRecord();
                            record.GasType = gasType.GasTypeName;
                            record.DayProfit = dayStockList.Sum(t => t.ProfitLoss);
                            record.WeekProfit = weekStockList.Sum(t => t.ProfitLoss);

                            response.Data.Add(record);
                        }
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in getting Dashboard Gas Profit Loss"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetMonthlyBusinessSheet(SelectDate objSelectDay)
        {
            try
            {
                List<BusinessDailySheet> objRepColl = new ReportBusinessDailySheet().GetBusinessDailySheet(objSelectDay.StoreID, objSelectDay.Month, objSelectDay.Year);
                return Ok(objRepColl);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Report"
                };
                throw new HttpResponseException(resp);
            }
        }


        [HttpPost]
        public IHttpActionResult GetGasMonthStatement(SelectDate objSelectDay)
        {
            try
            {
                ReportGasMonthlyStatement objMonthlyTotal = new ReportGasMonthlyStatement();
                ReportGasMonthlyStatementModel GasMonthlyStatementModel = new ReportGasMonthlyStatementModel();
                GasMonthlyStatementModel = objMonthlyTotal.GetMonthlyStatementNormal(objSelectDay.StoreID, objSelectDay.Month, objSelectDay.Year);
                return Ok(GasMonthlyStatementModel);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Login"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetLedger(SelectLedger objLedger)
        {
            try
            {
                List<ReportLedgerModel> objLedgerData = new List<ReportLedgerModel>();
                ReportLedger objrepLedger = new ReportLedger();
                objLedgerData = objrepLedger.GetLedger(objLedger.StoreID, objLedger.LedgerID, objLedger.FromDate, objLedger.ToDate);

                return Ok(objLedgerData);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Ledger"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetGasBalanceSheet(SelectLedger objSelectDate)
        {
            try
            {
                ReportGasBalanceSheet objGasBS = new ReportGasBalanceSheet();
                ReportBSGas objBS = new ReportBSGas();
                objGasBS = objBS.GetBalanceSheet(objSelectDate.StoreID, objSelectDate.FromDate, objSelectDate.ToDate);

                return Ok(objGasBS);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Ledger"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetGasSaleTrend(SelectGas objGas)
        {
            try
            {
                ReportGas reportGas = new ReportGas();
                List<ReportSaleTrendModel> MonthlySale = reportGas.GetSaleTrend(objGas.StoreID, objGas.Date.Month, objGas.Date.Year);

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
        public HttpResponseMessage AddReportGroup(ReportGroup obj)
        {
            ReportGroupConfiguration _dalReportGroup = new ReportGroupConfiguration();
            try
            {
                _dalReportGroup.AddGroup(obj);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Report Group Creation"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddAccountsToReportGroup(List<ReportGroup> obj)
        {
            ReportGroupConfiguration _dalReportGroup = new ReportGroupConfiguration();
            try
            {
                _dalReportGroup.AddLedgerToGroup(obj);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Report Group Creation"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetGroupCustomReport(SelectLedger objGroup)
        {
            try
            {
                List<ReportCustomGroup> objGroupData = new List<ReportCustomGroup>();
                ReportGroupConfiguration objGroupReport = new ReportGroupConfiguration();
                objGroupData = objGroupReport.GetCustomGroupReport(objGroup.StoreID, objGroup.FromDate, objGroup.ToDate);

                return Ok(objGroupData);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Ledger"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetGroupCustomDetailedReport(SelectLedger objGroup)
        {
            try
            {
                List<ReportCustomGroupAccounts> objGroupDetailedData = new List<ReportCustomGroupAccounts>();
                ReportGroupConfiguration objGroupReport = new ReportGroupConfiguration();
                objGroupDetailedData = objGroupReport.GetCustomGroupReport(objGroup.StoreID, objGroup.LedgerID, objGroup.FromDate, objGroup.ToDate);

                return Ok(objGroupDetailedData);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Ledger"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetCustomGroups(int ID)
        {
            try
            {
                // ID - Store ID
                ReportGroupConfiguration _dalReport = new ReportGroupConfiguration();
                return Ok(_dalReport.GetCustomGroups(ID));
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
        public IHttpActionResult GetGroupLedgers(SelectGroup objGroup)
        {
            try
            {
                List<ReportCustomGroupAccountMaster> objLedgerList = new List<ReportCustomGroupAccountMaster>();
                ReportGroupConfiguration objGroupReport = new ReportGroupConfiguration();
                objLedgerList = objGroupReport.GetLedgerPerCustomGroup(objGroup.StoreID, objGroup.GroupID);

                return Ok(objLedgerList);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Report"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetGasMonthlySaleAbstractReport(SelectLottery objLottery)
        {
            try
            {
                ReportGas reportGas = new ReportGas();
                List<SaleGraph> MonthlySale = reportGas.MonthlySaleReport(objLottery.StoreID, objLottery.Date.Month, objLottery.Date.Year);

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

    }

    public class SelectDate
    {
        public int StoreID { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
    }

    public class SelectLedger
    {
        public int StoreID { get; set; }
        public int LedgerID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class SelectGroup
    {
        public int StoreID { get; set; }
        public int GroupID { get; set; }
    }
}
