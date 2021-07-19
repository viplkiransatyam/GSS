using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    private class UserName : Parameters
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
