using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class TankMaster : Parameters
    {
        public int TankID { get; set; }
        public string TankName { get; set; }
        public float TankCapacity { get; set; }
    }

    public abstract class Parameters
    {
        public string CreatedUserName { get; set; }

        public string CreateTimeStamp
        {
            get { return System.DateTime.Now.ToString(); }
        }

        public string ModifiedUserName { get; set; }

        public string ModifiedTimeStamp
        {
            get { return System.DateTime.Now.ToString(); }
        }
    }
}
