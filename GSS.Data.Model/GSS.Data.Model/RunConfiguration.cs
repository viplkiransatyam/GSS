using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class ResponseMessage
    {
        public string MessageCode { get; set; }
        public string MessageDescription { get; set; }
    }
    public class Dropdown 
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RunConfiguration : ResponseMessage
    {
        public int Key { get; set; }
        public string FacilityID { get; set; }
        public string VersionType { get; set; }
        public string VersionNo { get; set; }
        public string VersionDescription { get; set; }
        public string EffectiveDate { get; set; }
        public string EffectiveTime { get; set; }
        public string G2BDate { get; set; }
        public string MSFromDate { get; set; }
        public string MSToDate { get; set; }
        public List<RunConfigurationGroup> ConfigurationGroup { get; set; }
    }

    public class RunConfigurationGroup
    {
        public int GroupNo { get; set; }
        public List<RunConfigurationChild> RCChild { get; set; }
    }

    public class RunConfigurationChild
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class SearchType
    {
        public string Type { get; set; }
    }
}
