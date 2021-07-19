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
    public class DashboardController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetStoreDashboard(int ID)
        {
            try
            {
                ReportDashBoard objStore = new ReportDashBoard();
                StoreDashBoardModel objStoreDashboard = new StoreDashBoardModel();
                objStoreDashboard = objStore.GetStoreSaleDashboard(ID);

                return Ok(objStoreDashboard);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Dashboard"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetGroupDashboard()
        {
            try
            {
                ReportDashBoard objStore = new ReportDashBoard();
                GroupDashBoardModel objGroupDashboard = new GroupDashBoardModel();
                objGroupDashboard = objStore.GetGroupSaleDashboard();

                return Ok(objGroupDashboard);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Dashboard"
                };
                throw new HttpResponseException(resp);
            }

        }
    }
}
