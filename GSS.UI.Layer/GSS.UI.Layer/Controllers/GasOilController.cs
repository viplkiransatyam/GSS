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
    public class GasOilController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetGasType(int ID)
        {
            try
            {
                // ID - Store ID
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasOil> objGasOil = _dalGasOil.SelectRecords(ID);

                return Ok(objGasOil);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Oil"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetGasTypeExcludeDuplicate(int ID)
        {
            try
            {
                // ID - Store ID
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasOil> objGasOil = _dalGasOil.SelectRecordsExcludeOil(ID);

                return Ok(objGasOil);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Oil"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetGroupGasType(int ID)
        {
            try
            {
                // ID - Store ID
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasOil> objGasOil = _dalGasOil.GetGroupGasType(ID);

                return Ok(objGasOil);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Oil"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage PostGasOil(GasOil objGasOil)
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                _dalGasOil.AddRecord(objGasOil);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
        public HttpResponseMessage UpdateGasStockPrice(GasOil objGasOil)
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                _dalGasOil.UpdateRecord(objGasOil);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
        public IHttpActionResult SelectGasTypes(SearchModel objSearchModel)
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasOil> objGasOil = new List<GasOil>();

                if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_GROUPID))
                    objGasOil = _dalGasOil.SelectGasOilPerGroupByGroupID(Convert.ToInt16(objSearchModel.SearchValue));

                else if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_USERID))
                    objGasOil = _dalGasOil.SelectGasOilPerGroupByUserName(objSearchModel.SearchValue);

                return Ok(objGasOil);
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
        public IHttpActionResult SelectAllGasOil()
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasOil> objGasOil = new List<GasOil>();
                objGasOil = _dalGasOil.SelectAllGasOil();

                return Ok(objGasOil);
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
        public IHttpActionResult SelectGasRecord(SearchGasPrice objSearchModel)
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                GasOil objGasOil = new GasOil();

                objGasOil = _dalGasOil.SelectRecord(objSearchModel.StoreID, objSearchModel.GasTypeID);

                return Ok(objGasOil);
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
        public IHttpActionResult GetGasOpeningBalance(SearchGasOil objSearchModel)
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasOpeningBalance> objlstGasOpeningBalance = new List<GasOpeningBalance>();
                int iStoreID = objSearchModel.StoreID;
                int iTankID = objSearchModel.TankID;
                DateTime dDate = objSearchModel.Date;

                objlstGasOpeningBalance = _dalGasOil.GetGasOpeningBalance(iStoreID,iTankID,dDate);

                return Ok(objlstGasOpeningBalance);
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
        public IHttpActionResult GetGasSystemClosingBalance(List<GasSystemClosingBalance> obj)
        {
            try
            {
                GasOilDal _dalGasOil = new GasOilDal();
                List<GasSystemClosingBalance> objlstGasSystemClosingBalance = new List<GasSystemClosingBalance>();

                objlstGasSystemClosingBalance = _dalGasOil.GetGasSystemClosingBalance(obj);

                return Ok(objlstGasSystemClosingBalance);
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

        [HttpGet]
        public IHttpActionResult GetRunningShift(int ID)
        {
            try
            {
                GasOilDal gasOil = new GasOilDal();
                LotteryRunningShift runningShift = gasOil.GetRunningShift(ID);

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

        [HttpPost]
        public IHttpActionResult GetReconcillationStatement(SelectReportDate objSearch)
        {
            try
            {
                ReportGas _dalGasOil = new ReportGas();
                List<ReconcilationStatement> obj = _dalGasOil.ReconcilationStatement(objSearch.StoreID, objSearch.FromDate, objSearch.ToDate);

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


        #region Reports
        [HttpPost]
        public IHttpActionResult GetSaleReportIndividual(SelectLotteryReport obj)
        {
            try
            {
                ReportGas _reportGas = new ReportGas();
                List<ReportGasItem> objReport;

                objReport = _reportGas.GetSaleReportIndividual(obj.StoreID, obj.FromDate,obj.ToDate);

                return Ok(objReport);
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
        public IHttpActionResult GetSaleReportMonth(SelectLotteryReport obj)
        {
            try
            {
                ReportGas _reportGas = new ReportGas();
                List<ReportMonthlyStatement> objReport;

                objReport = _reportGas.GetSaleReportMonthly(obj.StoreID, obj.FromDate, obj.ToDate);

                return Ok(objReport);
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

        [HttpGet]
        public IHttpActionResult DashboardPreviousShift(int ID)
        {
            try
            {
                Dashboard objDashboard = new Dashboard();
                ReportGas _dalGasOil = new ReportGas();
                objDashboard.Sales = _dalGasOil.GasReportPreviousShiftSalesDashboard(ID);
                objDashboard.Inventory = _dalGasOil.GetInventoryReportForPreviousShift(ID);
                objDashboard.SaleGraph = _dalGasOil.GetSaleReportGraph(ID);

                return Ok(objDashboard);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Oil"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult DayEndReport(SelectGasDay obj)
        {
            try
            {
                ReportGas _reportGas = new ReportGas();
                DayEndModel objReport;

                objReport = _reportGas.DayEndReport(obj.StoreID, obj.Date, obj.ShiftID);

                return Ok(objReport);
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


        #endregion
    }

    public class SearchGasPrice
    {
        public int StoreID { get; set; }
        public int GasTypeID { get; set; }
    }

    public class SelectReportDate
    {
        public int StoreID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class SelectGasDay
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
        public int ShiftID { get; set; }
    }
}
