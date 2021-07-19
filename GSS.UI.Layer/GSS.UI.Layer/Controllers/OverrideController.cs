using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSS.Data.Model;
using System.Web.Http.Cors;

namespace GSS.UI.Layer.Controllers
{
    public class OverrideController : ApiController
    {
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]

        [HttpGet]
        public IHttpActionResult GetOverrideDetails(int ID)
        {
            RunConfiguration RC = new RunConfiguration();
            try
            {
                List<Override> overridedetails = new List<Override>();
                Override overridedetail;

                #region First Row
                overridedetail = new Override();
                overridedetail.grouppnumber = "1";
                overridedetail.groupcount = 1;
                overridedetail.description = "Test Description 1";
                overridedetail.position = "7";
                overridedetail.value = "Test Value";
                overridedetail.startdate = "2018-01-01";
                overridedetail.endDate = "2018-07-01";
                overridedetail.BMCcriteriatype = "BMC";
                overridedetail.BMCcriteriavalue = "P01";
                overridedetail.EIMcriteriatype = "EIM";
                overridedetail.EIMcriteriavalue = "ABCD";
                overridedetail.PROPERTYcriteriatype = "PROPERTY";
                overridedetail.PROPERTYcriteriavalue = "PROPERTY VALUE";
                overridedetail.OPTIONcriteriatype = "";
                overridedetail.OPTIONcriteriavalue = "";
                overridedetail.DESTcriteriatype = "";
                overridedetail.DESTcriteriavalue = "";
                overridedetail.BCPositionType = "";
                overridedetail.BCPositionValue = "";
                overridedetail.BCValueType = "";
                overridedetail.BCValueValue = "";
                overridedetails.Add(overridedetail);

                #endregion

                #region Second Row
                overridedetail = new Override();
                overridedetail.grouppnumber = "2";
                overridedetail.groupcount = 1;
                overridedetail.description = "Test Description 2";
                overridedetail.position = "8";
                overridedetail.value = "Test Value";
                overridedetail.startdate = "2018-08-01";
                overridedetail.endDate = "2018-09-01";
                overridedetail.BMCcriteriatype = "BMC";
                overridedetail.BMCcriteriavalue = "P02";
                overridedetail.EIMcriteriatype = "EIM";
                overridedetail.EIMcriteriavalue = "xyz";
                overridedetail.PROPERTYcriteriatype = "";
                overridedetail.PROPERTYcriteriavalue = "";
                overridedetail.OPTIONcriteriatype = "";
                overridedetail.OPTIONcriteriavalue = "";
                overridedetail.DESTcriteriatype = "";
                overridedetail.DESTcriteriavalue = "";
                overridedetail.BCPositionType = "";
                overridedetail.BCPositionValue = "";
                overridedetail.BCValueType = "";
                overridedetail.BCValueValue = "";
                overridedetails.Add(overridedetail);

                #endregion

                return Ok(overridedetails);
            }
            catch (Exception ex)
            {
                RC.MessageCode = "1";
                RC.MessageDescription = ex.Message;
                return Ok(RC);
            }

        }

        [HttpGet]
        public IHttpActionResult getcriteriavalue(string facility, string criteriatype)
        {
            List<Dropdown> criteriaList = new List<Dropdown>();

            Dropdown dropdown;

            if (criteriatype == "EIM")
            {
                #region EIM
                dropdown = new Dropdown();
                dropdown.id = "EIM1";
                dropdown.name = "EIM1";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "EIM2";
                dropdown.name = "EIM2";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "EIM3";
                dropdown.name = "EIM3";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "EIM4";
                dropdown.name = "EIM4";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "EIM4";
                dropdown.name = "EIM4";
                criteriaList.Add(dropdown);
                #endregion

            }
            else if(criteriatype == "BMC")
            {
                #region BMC
                dropdown = new Dropdown();
                dropdown.id = "S01";
                dropdown.name = "S01";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "S02";
                dropdown.name = "S02";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "S03";
                dropdown.name = "S03";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "S04";
                dropdown.name = "S04";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "S05";
                dropdown.name = "S05";
                criteriaList.Add(dropdown);
                #endregion

            }
            else if (criteriatype == "DEST")
            {
                #region BMC
                dropdown = new Dropdown();
                dropdown.id = "DEST1";
                dropdown.name = "DEST1";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "DEST2";
                dropdown.name = "DEST2";
                criteriaList.Add(dropdown);
                
                #endregion

            }
            else if (criteriatype == "PROPERTY")
            {
                #region BMC
                dropdown = new Dropdown();
                dropdown.id = "Property1";
                dropdown.name = "Property1";
                criteriaList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id = "Property2";
                dropdown.name = "Property3";
                criteriaList.Add(dropdown);

                #endregion

            }
            else if (criteriatype == "OPTION")
            {
                #region BMC
                dropdown = new Dropdown();
                dropdown.id = "Option1";
                dropdown.name = "Option1";
                criteriaList.Add(dropdown);

                #endregion

            }
            else if (criteriatype == "BCPOSITION")
            {
                #region BMC
                dropdown = new Dropdown();
                dropdown.id = "BCPosition1";
                dropdown.name = "BCPosition1";
                criteriaList.Add(dropdown);
         
                #endregion

            }
            else if (criteriatype == "BCVALUE")
            {
                #region BMC
                dropdown = new Dropdown();
                dropdown.id = "BCvalue1";
                dropdown.name = "BCvalue1";
                criteriaList.Add(dropdown);

                #endregion
            }

            return Ok(criteriaList);



        }
    }
}
