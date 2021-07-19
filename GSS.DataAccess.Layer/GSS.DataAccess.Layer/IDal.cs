using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    interface IDal<PARMTYPE> where PARMTYPE : Parameters
    {
        bool AddRecord(PARMTYPE obj);
        bool UpdateRecord(PARMTYPE obj);
        bool DeleteRecord(int ID);
        PARMTYPE SelectRecord(int ID);
        List<PARMTYPE> SelectRecords(int ID);
        int SelectMaxID();
    }


}
