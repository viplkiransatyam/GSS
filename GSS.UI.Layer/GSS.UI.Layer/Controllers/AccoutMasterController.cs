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
    public class AccoutMasterController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostAccountMaster(AccountModel obj)
        {
            AccountMasterDal _dalAccountMaster = new AccountMasterDal();

            try
            {
                if (obj.LedgerID == 0)
                {
                    obj.LedgerID = _dalAccountMaster.SelectMaxID(obj.StoreID);
                    if (obj.SalesGroupID <= 0)
                        _dalAccountMaster.AddRecord(obj);
                    else
                        _dalAccountMaster.AddSaleHeadRecord(obj);
                }
                else if (obj.LedgerID > 0)
                {
                    _dalAccountMaster.UpdateRecord(obj);
                }

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);

            }
            finally
            {
                _dalAccountMaster = null;
            }

        }

        [HttpPost]
        public HttpResponseMessage DeleteAccountMaster(AccountModel obj)
        {
            AccountMasterDal _dalAccountMaster = new AccountMasterDal();

            try
            {
                _dalAccountMaster.DeleteRecord(obj.StoreID,obj.LedgerID);
                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);

            }
            finally
            {
                _dalAccountMaster = null;
            }
        }


        [HttpGet]
        public IHttpActionResult GetAccountGroups()
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<GSS.DataAccess.Layer.AccountMasterDal.AccountGroups> _AccountGroups;
                _AccountGroups = _dalAccountMaster.SelectAccountGroups();

                return Ok(_AccountGroups);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetBusinessAccountGroups()
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<GSS.DataAccess.Layer.AccountMasterDal.AccountGroups> _AccountGroups;
                _AccountGroups = _dalAccountMaster.SelectBusinessAccountGroups();

                return Ok(_AccountGroups);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetSectionWiseAccounts(SectionWiseAccounts objSearchModel)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;

                if (objSearchModel.Date.ToString().Length > 0)
                    objAccounts = _dalAccountMaster.SelectRecords(objSearchModel.StoreID, objSearchModel.DisplaySide, objSearchModel.Date);
                else
                    objAccounts = _dalAccountMaster.SelectRecords(objSearchModel.StoreID,objSearchModel.DisplaySide);

                return Ok(objAccounts);
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
        public IHttpActionResult GetAccounts(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID, "ALL");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetCommonAccounts(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID, "COMMON");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetSalesGroups(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID,"SALES_GROUPS");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetBusinessSalesGroups(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID, "BUSINESS_SALES_GROUPS");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetBusinessPaidGroups(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID, "BUSINESS_PAID_GROUPS");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetSalesLedgers(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID, "SALES_LEDGERS");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult GetBusinessPaidLedgers(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectRecords(ID, "BUSINESS_PAID");

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetLotteryAccounts(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectLotteryRecords(ID);

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetGasAccounts(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectGasRecords(ID);

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetStoreAccounts(int ID)
        {
            try
            {
                AccountMasterDal _dalAccountMaster = new AccountMasterDal();
                List<AccountModel> objAccounts;
                objAccounts = _dalAccountMaster.SelectStoreRecords(ID);

                return Ok(objAccounts);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateOpeningBalance(AccountModel obj)
        {
            SaleEntries _dalSaleEntries = new SaleEntries();

            try
            {
                _dalSaleEntries.UpdateOpeningBalance(obj);
                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in updating Opening Balance for Account"
                };
                throw new HttpResponseException(resp);

            }
            finally
            {
                _dalSaleEntries = null;
            }

        }

    }

    public class SectionWiseAccounts
    {
        public int StoreID { get; set; }
        public string DisplaySide { get; set; }
        public DateTime Date { get; set; }
    }

}
