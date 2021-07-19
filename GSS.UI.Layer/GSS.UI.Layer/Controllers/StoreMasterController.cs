using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSS.Data.Model;
using GSS.DataAccess.Layer;

namespace GSS.UI.Layer.Controllers
{
    public class StoreMasterController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostStoreMaster(StoreMaster obj)
        {
            StoreMasterDal _dalStoreCard = new StoreMasterDal();
         
            try
            {
                StoreMasterDal.StoreGroupID _StoreGroupID = new StoreMasterDal.StoreGroupID();

                if (obj.StoreID == 0)
                {
                    _StoreGroupID = _dalStoreCard.SelectStoreGroupID(obj.CreatedUserName);
                    if (_StoreGroupID.GroupID <= 0)
                    {
                        var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent(string.Format("Error Group ID not found")),
                            ReasonPhrase = "Error in Store Master"
                        };
                        throw new HttpResponseException(resp);
                    }
                    else
                    {
                        obj.GroupID = _StoreGroupID.GroupID;
                    }

                    obj.StoreID = _dalStoreCard.SelectMaxID();
                    if (_dalStoreCard.AddRecord(obj))
                    {
                        bool _result = true;
                    }
                }
                else
                {
                    _StoreGroupID = _dalStoreCard.SelectStoreGroupID(obj.ModifiedUserName);
                    if (_StoreGroupID.GroupID <= 0)
                    {
                        var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent(string.Format("Error Group ID not found")),
                            ReasonPhrase = "Error in Store Master"
                        };
                        throw new HttpResponseException(resp);
                    }
                    else
                    {
                        obj.GroupID = _StoreGroupID.GroupID;
                        obj.StoreID = _StoreGroupID.StoreID;
                    }

                    if (_dalStoreCard.UpdateRecord(obj))
                    {
                        bool _result = true;
                    }
                }

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
            finally
            {
                _dalStoreCard = null;
            }

        }

        [HttpPost]
        public HttpResponseMessage AddUser(StoreUser obj)
        {
            StoreMasterDal _dalStoreCard = new StoreMasterDal();

            try
            {
                if (obj.StoreID != 0)
                {
                    if (_dalStoreCard.AddUser(obj))
                    {
                        bool _result = true;
                    }
                }
                else
                {
                    throw new Exception("Invalid Store");
                }
                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
            finally
            {
                _dalStoreCard = null;
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateUser(StoreUser obj)
        {
            StoreMasterDal _dalStoreCard = new StoreMasterDal();

            try
            {
                if (obj.StoreID != 0)
                {
                    if (_dalStoreCard.UpdateUser(obj))
                    {
                        bool _result = true;
                    }
                }
                else
                {
                    throw new Exception("Invalid Store");
                }
                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
            finally
            {
                _dalStoreCard = null;
            }

        }

        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePassword obj)
        {
            StoreMasterDal _dalStore = new StoreMasterDal();
            
            try
            {
                _dalStore.UpdatePassword(obj.UserId, obj.Password);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
            finally
            {
                _dalStore = null;
            }

        }

        [HttpPost]
        public HttpResponseMessage AddCardToStore(List<StoreCard> obj)
        {
            StoreCardDal _dalStoreCard = new StoreCardDal();
            try
            {
                if (_dalStoreCard.AddRecord(obj))
                {
                    bool _result = true;
                }

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

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
            finally
            {
                _dalStoreCard = null;
            }

        }

        [HttpGet]
        public IHttpActionResult GetStoreMaster(int ID)
        {
            try
            {
                StoreMasterDal _dalStoreCard = new StoreMasterDal();

                StoreMaster objStore = _dalStoreCard.SelectRecord(ID);

                return Ok(objStore);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}",  ex.Message)),
                    ReasonPhrase = "Error in Store Master"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetGroupCardTypes(int ID)
        {
            try
            {
                StoreCardDal _dalStoreCard = new StoreCardDal();
                List<ReportStoreCard> objStoreCards = _dalStoreCard.SelectGroupCardTypes(ID);

                return Ok(objStoreCards);
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

        [HttpGet]
        public IHttpActionResult GetStoreCardTypes(int ID)
        {
            try
            {
                StoreCardDal _dalStoreCard = new StoreCardDal();
                List<StoreCard> objStoreCards = _dalStoreCard.SelectRecords(ID);

                return Ok(objStoreCards);
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

        [HttpGet]
        public IHttpActionResult GetShifts(int ID)
        {
            try
            {
                StoreMasterDal _dalStoreCard = new StoreMasterDal();

                List<StoreShift> objShifts = _dalStoreCard.StoreShift(ID);

                return Ok(objShifts);
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

        [HttpGet]
        public IHttpActionResult GetParameterOpeningBalance(int ID)
        {
            try
            {
                StoreMasterDal _dalStoreCard = new StoreMasterDal();

                ParameterList objParameterList = new ParameterList();
                objParameterList = _dalStoreCard.SelectParameterOpeningBalanceList(ID);

                return Ok(objParameterList);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Parameter List"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateParameterOpeningBalance(ParameterList obj)
        {
            StoreMasterDal _dalStore = new StoreMasterDal();
            try
            {
                if (_dalStore.UpdateParameterOpeningBalance(obj))
                {
                    bool _result = true;
                }

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in updating parameter opening balance"
                };
                throw new HttpResponseException(resp);

            }
            finally
            {
                _dalStore = null;
            }

        }

        [HttpPost]
        public IHttpActionResult ValidateUser(Login objLogin)
        {
            try
            {
                LoginDal _dalLoginDal = new LoginDal();
                LoginResult objLoginResult = _dalLoginDal.ValidateUser(objLogin);

                return Ok(objLoginResult);
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

    }

    public class ChangePassword
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
