using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSS.DataAccess.Layer;
using GSS.Data.Model;
using System.Data.SqlClient;

namespace GSS.BusinessLogic.Layer
{
    public class GasValidation
    {
        string _sql = "";

        public bool ValidateGasEntryMadeForDay(SaleMaster objGasSale)
        {
            bool bResult = true;
            try
            {
                using (SqlConnection _conn = new SqlConnection(LotteryValidation.con))
                {
                    _sql = @"SELECT       count( STORE_ID) FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DATE = 'PARMDATE') ";
                    _sql = _sql.Replace("PARMSTOREID", objGasSale.StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", objGasSale.Date.ToString());

                    _conn.Open();
                    SqlCommand cmd = new SqlCommand(_sql, _conn);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) <= 0)
                    {
                        bResult = false;
                        throw new Exception("Sales were not entered for the shift, please enter and then enter receipts");
                    }

                    //_sql = @"SELECT        COUNT(STORE_GAS_SALE_BALANCES.STORE_ID) AS Expr1
                    //            FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                    //                                     STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                    //                                     STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                    //            WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')";

                    //_sql = _sql.Replace("PARMSTOREID", objGasSale.StoreID.ToString());
                    //_sql = _sql.Replace("PARMDATE", objGasSale.Date.ToString());

                    //cmd = new SqlCommand(_sql, _conn);
                    //if (Convert.ToInt16(cmd.ExecuteScalar()) <= 0)
                    //{
                    //    bResult = false;
                    //    throw new Exception("Closing Inventory was not entered for the shift, please enter and then enter receipts");
                    //}
                    _conn.Close();
                    bResult = true;
                    return bResult;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
