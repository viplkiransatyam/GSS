using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSS.Data.Model;

namespace GSS.UI.Layer.Controllers
{
    public class RunConfigurationController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetRunConfiguration(int ID)
        {
            RunConfiguration RC = new RunConfiguration();
            try
            {
                List<RunConfigurationChild> RCChildList;
                List<RunConfigurationGroup> RCGroupList;
                RunConfigurationGroup RCGroup;
                RunConfigurationChild RCChild;
                
                RC.FacilityID = "SP";
                RC.EffectiveDate = "01-02-2018";
                RC.EffectiveTime = "10:30:00";
                RC.G2BDate = "01-03-2018";
                RC.Key = 1;
                RC.MSFromDate = "01-01-2018";
                RC.MSToDate = "01-01-2018";
                RC.VersionNo = "V001";
                RC.VersionDescription = "TEST VERSION";
                RC.VersionType = "TEST";

                #region Adding First Group
                RCGroupList = new List<RunConfigurationGroup>();
                RCGroup = new RunConfigurationGroup();

                RCGroup.GroupNo = 1;

                #region Adding Group Items
                RCChildList = new List<RunConfigurationChild>();
                RCChild = new RunConfigurationChild();
                RCChild.Type = "BMC";
                RCChild.Value = "S01";
                RCChildList.Add(RCChild);

                RCChild = new RunConfigurationChild();
                RCChild.Type = "EIM";
                RCChild.Value = "EIM001";
                RCChildList.Add(RCChild);

                RCChild = new RunConfigurationChild();
                RCChild.Type = "DIST";
                RCChild.Value = "XXXXX";
                RCChildList.Add(RCChild);

                #endregion

                RCGroup.RCChild = RCChildList;
                RCGroupList.Add(RCGroup);

                #endregion

                #region Adding Second Group
                RCGroup = new RunConfigurationGroup();

                RCGroup.GroupNo = 2;

                #region Adding Group Items
                RCChildList = new List<RunConfigurationChild>();
                RCChild = new RunConfigurationChild();
                RCChild.Type = "BMC";
                RCChild.Value = "S02";
                RCChildList.Add(RCChild);

                RCChild = new RunConfigurationChild();
                RCChild.Type = "EIM";
                RCChild.Value = "EIM002";
                RCChildList.Add(RCChild);

                #endregion

                RCGroup.RCChild = RCChildList;
                RCGroupList.Add(RCGroup);

                #endregion

                RC.ConfigurationGroup = RCGroupList;
                RC.MessageCode = "0";

                return Ok(RC);
            }
            catch (Exception ex)
            {
                RC.MessageCode = "1";
                RC.MessageDescription = ex.Message;
                return Ok(RC);
            }

        }

        [HttpGet]
        public IHttpActionResult GetFacility()
        {
            List<Dropdown> facilityList = new List<Dropdown>();

                Dropdown dropdown;

                dropdown = new Dropdown();
                dropdown.id= "CP";
                dropdown.name = "Canton";
                facilityList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id= "SP";
                dropdown.name = "Smyrna";
                facilityList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id= "N1";
                dropdown.name = "Augus";
                facilityList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id= "P1";
                dropdown.name = "Civac";
                facilityList.Add(dropdown);

                dropdown = new Dropdown();
                dropdown.id= "L1";
                dropdown.name = "Augus 2";
                facilityList.Add(dropdown);


                return Ok(facilityList);

            

        }

        [HttpGet]
        public IHttpActionResult GetTypes()
        {
            List<Dropdown> typeList = new List<Dropdown>();

            Dropdown dropdown;

            dropdown = new Dropdown();
            dropdown.id= "BMC";
            dropdown.name = "BMC";
            typeList.Add(dropdown);

            dropdown = new Dropdown();
            dropdown.id= "EIM";
            dropdown.name = "EIM";
            typeList.Add(dropdown);

            dropdown = new Dropdown();
            dropdown.id= "DIST";
            dropdown.name = "DIST";
            typeList.Add(dropdown);

            dropdown = new Dropdown();
            dropdown.id= "PROPERTY_TYPE";
            dropdown.name = "PROPERTY_TYPE";
            typeList.Add(dropdown);

            dropdown = new Dropdown();
            dropdown.id= "PROPERTY_VALUE";
            dropdown.name = "PROPERTY_VALUE";
            typeList.Add(dropdown);
            
            return Ok(typeList);
            
        }

        [HttpPost]
        public IHttpActionResult GetValues(SearchType type)
        {
            List<Dropdown> valueList = new List<Dropdown>();


            if (type.Type == "BMC")
            {
                Dropdown dropdown;

                dropdown = new Dropdown();
                dropdown.id= "S01";
                dropdown.name = "S01";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "S02";
                dropdown.name = "S02";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "S03";
                dropdown.name = "S03";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "S04";
                dropdown.name = "S04";
                valueList.Add(dropdown);
            }

            else if (type.Type == "EIM")
            {
                Dropdown dropdown;

                dropdown = new Dropdown();
                dropdown.id= "EIM001";
                dropdown.name = "EIM001";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "EIM002";
                dropdown.name = "EIM002";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "EIM003";
                dropdown.name = "EIM003";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "EIM004";
                dropdown.name = "EIM004";
                valueList.Add(dropdown);
            }

            else if (type.Type == "DIST")
            {
                Dropdown dropdown;

                dropdown = new Dropdown();
                dropdown.id= "DIST001";
                dropdown.name = "DIST001";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "DIST002";
                dropdown.name = "DIST002";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "DIST003";
                dropdown.name = "DIST003";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "DIST004";
                dropdown.name = "DIST004";
                valueList.Add(dropdown);
            }

            else if (type.Type == "PROPERTY_TYPE")
            {
                Dropdown dropdown;

                dropdown = new Dropdown();
                dropdown.id= "PROP_TYPE_001";
                dropdown.name = "PROP_TYPE_001";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "PROP_TYPE_002";
                dropdown.name = "PROP_TYPE_002";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "PROP_TYPE_003";
                dropdown.name = "PROP_TYPE_003";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "PROP_TYPE_004";
                dropdown.name = "PROP_TYPE_004";
                valueList.Add(dropdown);
            }

            else if (type.Type == "PROPERTY_VALUE")
            {
                Dropdown dropdown;

                dropdown = new Dropdown();
                dropdown.id= "PROP_VALUE_001";
                dropdown.name = "PROP_VALUE_001";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "PROP_VALUE_002";
                dropdown.name = "PROP_VALUE_002";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "PROP_VALUE_003";
                dropdown.name = "PROP_VALUE_003";
                valueList.Add(dropdown);
                dropdown = new Dropdown();
                dropdown.id= "PROP_VALUE_004";
                dropdown.name = "PROP_VALUE_004";
                valueList.Add(dropdown);
            }

            return Ok(valueList);
        }

        [HttpPost]
        public IHttpActionResult SaveConfiguration(RunConfiguration RC)
        {
            try
            {
                RC.MessageCode = "0";
                return Ok(RC);

            }
            catch (Exception ex)
            {
                RC.MessageCode = "1";
                RC.MessageDescription = ex.Message;
                return Ok(RC);
            }
        }

    }


}
