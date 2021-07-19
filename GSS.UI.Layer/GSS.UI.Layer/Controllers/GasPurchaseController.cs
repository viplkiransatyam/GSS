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
    public class GasPurchaseController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetGasPurchase(SelectBOL objSaleDay)
        {
            try
            {
                GasPurchase clsGasPurchase = new GasPurchase();
                GasPurchaseModel objPurchase = new GasPurchaseModel();

                objPurchase = clsGasPurchase.GasInvReceipt(objSaleDay.StoreID, objSaleDay.BOL);
                return Ok(objPurchase);

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Purchase"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage SavePurchase(GasPurchaseModel objGasPurchase)
        {
            try
            {
                GasPurchase _dalGasPurchaseEntries = new GasPurchase();
                _dalGasPurchaseEntries.AddOrUpdateGasInvoice(objGasPurchase);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Purchase"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetTranTypes()
        {
            try
            {
                GasPurchase _dalGasPurchase = new GasPurchase();

                List<TranType> TranTypes = new List<TranType>();
                TranTypes = _dalGasPurchase.GetTranTypes();

                return Ok(TranTypes);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Store Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddOrUpdateGasDealerStatement(GasDealerTrans objGasStatement)
        {
            try
            {
                GasPurchase _dalGasPurchaseEntries = new GasPurchase();
                _dalGasPurchaseEntries.AddOrUpdateGasDealerStatement(objGasStatement);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Dealer Statement"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetGasStatement(SelectStatement objSaleDay)
        {
            try
            {
                GasPurchase clsGasPurchase = new GasPurchase();
                GasDealerTrans objDealerStatement = new GasDealerTrans();

                objDealerStatement = clsGasPurchase.GetGasDealerStatement(objSaleDay.StoreID, objSaleDay.ReferenceNo);
                return Ok(objDealerStatement);

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Statement"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetTranAmount(SearchTranModel objTran)
        {
            try
            {
                GasPurchase clsGasPurchase = new GasPurchase();
                float fTranAmount = 0;

                fTranAmount = clsGasPurchase.GetTranAmount(objTran);
                return Ok(fTranAmount);

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Statement"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetPurchaseRegister(SelectLedger objLedger)
        {
            try
            {
                List<PurchaseRegisterModel> purchaseRegister = new List<PurchaseRegisterModel>();
                GasPurchase objrep = new GasPurchase();
                purchaseRegister = objrep.PurchaseRegister(objLedger.StoreID, objLedger.FromDate, objLedger.ToDate);

                return Ok(purchaseRegister);
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
        public IHttpActionResult GetStockInward(SelectLedger objLedger)
        {
            try
            {
                List<InwardModel> stockInward = new List<InwardModel>();
                GasPurchase objrep = new GasPurchase();
                stockInward = objrep.StockInward(objLedger.StoreID, objLedger.FromDate, objLedger.ToDate);

                return Ok(stockInward);
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

        public IHttpActionResult ReconcillationDetails(SelectStatement objSelectStatement)
        {
            try
            {
                GasOilDal objrep = new GasOilDal();
                List<ReconcillationModel> objRecDetails = objrep.ReconcillationDetails(objSelectStatement.StoreID, Convert.ToInt16(objSelectStatement.ReferenceNo));

                return Ok(objRecDetails);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Reconcillation Statement"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateReconcillation(UpdateReconcillation objRec)
        {
            try
            {
                GasOilDal _dalGas = new GasOilDal();
                _dalGas.UpdateReconcillation(objRec);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Update Reconcillation"
                };
                throw new HttpResponseException(resp);
            }

        }

    }


    public class SelectBOL
    {
        public int StoreID { get; set; }
        public string BOL { get; set; }
    }

    public class SelectStatement
    {
        public int StoreID { get; set; }
        public string ReferenceNo { get; set; }
    }

}
