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
    public class GroupMasterController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SelectStores(SearchModel objSearchModel)
        {
            try
            {
                StoreMasterDal _dalStore = new StoreMasterDal();
                List<StoreMaster> objStores = new List<StoreMaster>();

                if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_GROUPID))
                    objStores = _dalStore.SelectRecords(Convert.ToInt16(objSearchModel.SearchValue));
                
                else if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_USERID))
                    objStores = _dalStore.SelectRecords(objSearchModel.SearchValue);

                return Request.CreateResponse(HttpStatusCode.Created, objStores);    
                //return Ok(objStores);
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
        public HttpResponseMessage SelectUsers(SearchModel objSearchModel)
        {
            try
            {
                StoreMasterDal _dalStore = new StoreMasterDal();
                List<StoreUser> objStores = new List<StoreUser>();

                if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_GROUPID))
                    objStores = _dalStore.SelectUsers(Convert.ToInt16(objSearchModel.SearchValue));

                return Request.CreateResponse(HttpStatusCode.Created, objStores);    
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
        public IHttpActionResult GetSelectStores(string ID)
        {
            try
            {
                SearchModel objSearchModel = new SearchModel();
                string[] sTemp = ID.ToString().Split(',');
                objSearchModel.SearchKey = Convert.ToInt16(sTemp[0].ToString());
                objSearchModel.SearchValue = sTemp[1].ToString();

                StoreMasterDal _dalStore = new StoreMasterDal();
                List<StoreMaster> objStores = new List<StoreMaster>();

                if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_GROUPID))
                    objStores = _dalStore.SelectRecords(Convert.ToInt16(objSearchModel.SearchValue));

                else if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_USERID))
                    objStores = _dalStore.SelectRecords(objSearchModel.SearchValue);

                return Ok(objStores);
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
        public IHttpActionResult GetLotteryStores(int ID)
        {
            try
            {
                StoreMasterDal _dalStore = new StoreMasterDal();
                List<Store> objStores = new List<Store>();

                objStores = _dalStore.GetLotteryStores(ID);

                return Ok(objStores);
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
        public IHttpActionResult SelectGroupCardTypes(SearchModel objSearchModel)
        {
            try
            {
                StoreMasterDal _dalStore = new StoreMasterDal();
                List<GSS.DataAccess.Layer.StoreMasterDal.GroupCards> objGroupCards = new List<StoreMasterDal.GroupCards>();

                if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_GROUPID))
                    objGroupCards = _dalStore.SelectGroupCardTypes(Convert.ToInt16(objSearchModel.SearchValue));

                else if (objSearchModel.SearchKey == Convert.ToInt16(SearchKeys.SEARCH_BY_USERID))
                    objGroupCards = _dalStore.SelectGroupCardTypes(objSearchModel.SearchValue);

                return Ok(objGroupCards);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Group Controller"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        public IHttpActionResult SelectRemainingStoreCardTypes(int ID)
        {
            try
            {
                StoreMasterDal _dalStore = new StoreMasterDal();
                List<GSS.DataAccess.Layer.StoreMasterDal.GroupCards> objGroupCards = new List<StoreMasterDal.GroupCards>();

                objGroupCards = _dalStore.SelectRemainingStoreCardTypes(ID);
                return Ok(objGroupCards);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Group Controller"
                };
                throw new HttpResponseException(resp);
            }
        }


        [HttpGet]
        public IHttpActionResult GetCardPerStore(int ID)
        {
            try
            {

                StoreMasterDal _dalStore = new StoreMasterDal();
                List<GSS.DataAccess.Layer.StoreMasterDal.GroupCards> objStoreCards = new List<StoreMasterDal.GroupCards>();

                objStoreCards = _dalStore.SelectStoreCardTypes(ID);

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
    }
}
