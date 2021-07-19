using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Data.Model
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult
    {
        public string Status { get; set; }
        public string UserName { get; set; }
        public string AccessType { get; set; }
        public int GroupID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string StoreAdd1 { get; set; }
        public string StoreAdd2 { get; set; }
        public string StoreType { get; set; }

        public string AvailGas { get; set; }
        public string AvailLottery { get; set; }
        public string AvailBusiness { get; set; }



    }
}
