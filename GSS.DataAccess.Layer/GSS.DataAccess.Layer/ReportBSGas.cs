using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace GSS.DataAccess.Layer
{
    public class ReportBSGas
    {
        SqlConnection _conn;
        SqlConnection _conn2;
        string sql;
        int _storeID;
        SqlCommand cmd;

        public ReportGasBalanceSheet GetBalanceSheet(int iStoreID, DateTime dFromDate, DateTime dToDate)
        {
            ReportGasBalanceSheet objBS = new ReportGasBalanceSheet();
            SqlDataReader dr;
            float gasBalance;
            List<GasOpeningBalance> gasBalanceCollection;
            GasOilDal gasDal = new GasOilDal();
            float DebitTotal;
            float CreditTotal;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _conn2 = new SqlConnection(DMLExecute.con);
                _conn2.Open();

                _storeID = iStoreID;

                #region Getting Opening Balance
                sql = "SELECT  GAS_TANK_ID FROM   STORE_GAS_TANKS_MASTER  WHERE        (STORE_ID = " + iStoreID + ")";
                cmd = new SqlCommand(sql, _conn);
                dr = cmd.ExecuteReader();
                gasBalance = 0;
                while(dr.Read())
                {
                    gasBalanceCollection = gasDal.GetGasOpeningBalance(iStoreID, Convert.ToInt16(dr["GAS_TANK_ID"]), dFromDate);
                    foreach(GasOpeningBalance objGasBalance in gasBalanceCollection)
                    {
                        gasBalance += GetStockValue(objGasBalance.GasTypeID, objGasBalance.OpeningBalance);
                    }
                }
                dr.Close();
                objBS.OpeningStock = gasBalance;

                #endregion

                #region Getting Closing Balance
                sql = "SELECT  GAS_TANK_ID FROM   STORE_GAS_TANKS_MASTER  WHERE        (STORE_ID = " + iStoreID + ")";
                cmd = new SqlCommand(sql, _conn);
                dr = cmd.ExecuteReader();
                gasBalance = 0;
                while (dr.Read())
                {
                    gasBalanceCollection = gasDal.GetGasOpeningBalance(iStoreID, Convert.ToInt16(dr["GAS_TANK_ID"]), dToDate.AddDays(1));
                    foreach (GasOpeningBalance objGasBalance in gasBalanceCollection)
                    {
                        gasBalance += GetStockValue(objGasBalance.GasTypeID, objGasBalance.OpeningBalance);
                    }
                }
                dr.Close();
                objBS.ClosingStock = gasBalance;

                #endregion

                #region Getting Purchase Price
                sql = @"SELECT  SUM(STORE_GAS_SALE_BALANCES.GAS_RECV_PRICE ) AS PURCHASE_PRICE
                            FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                            WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = STOREID) AND (STORE_SALE_MASTER.SALE_DATE >= 'FROMDATE' AND 
                                                     STORE_SALE_MASTER.SALE_DATE < 'TODATE')";

                sql = sql.Replace("STOREID", iStoreID.ToString());
                sql = sql.Replace("FROMDATE", dFromDate.ToShortDateString());
                sql = sql.Replace("TODATE", dToDate.ToShortDateString());
                cmd = new SqlCommand(sql, _conn);
                dr = cmd.ExecuteReader();
                gasBalance = 0;
                while (dr.Read())
                {
                    if (dr["PURCHASE_PRICE"].ToString().Length > 0)
                        gasBalance += Convert.ToSingle(dr["PURCHASE_PRICE"]);
                }
                dr.Close();
                objBS.Purchase = gasBalance;
                #endregion

                #region Getting Sale Price
                sql = @"SELECT   sum(SALE_AMOUNT) AS SALE_PRICE  FROM            STORE_GAS_SALE_TRAN
                        WHERE        (STORE_ID = STOREID) AND 
                        (SALE_DATE >= 'FROMDATE' AND SALE_DATE < 'TODATE')";

                sql = sql.Replace("STOREID", iStoreID.ToString());
                sql = sql.Replace("FROMDATE", dFromDate.ToShortDateString());
                sql = sql.Replace("TODATE", dToDate.ToShortDateString());
                cmd = new SqlCommand(sql, _conn);
                dr = cmd.ExecuteReader();
                gasBalance = 0;
                while (dr.Read())
                {
                    if (dr["SALE_PRICE"].ToString().Length > 0)
                        gasBalance += Convert.ToSingle(dr["SALE_PRICE"]);
                }
                dr.Close();
                objBS.Sales = gasBalance;
                #endregion

                DebitTotal = objBS.OpeningStock + objBS.Purchase;
                CreditTotal = objBS.ClosingStock + objBS.Sales;

                if (CreditTotal > DebitTotal)
                    objBS.GrossProfit = CreditTotal - DebitTotal;
                else
                    objBS.GrossLoss = DebitTotal - CreditTotal;

                objBS.Total = objBS.OpeningStock + objBS.Purchase + objBS.GrossProfit;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objBS;
        }

        private float GetStockValue(int iGasTypeID, float fStockQty)
        {
            float StockValue = 0;
            sql = "SELECT  GAS_STOCK_PRICE  FROM   MAPPING_STORE_GAS WHERE (STORE_ID = " + _storeID + ") AND (GASTYPE_ID = " + iGasTypeID + ")";
            cmd = new SqlCommand(sql, _conn2);
            StockValue = Convert.ToSingle(cmd.ExecuteScalar());
            StockValue = StockValue * fStockQty;
            return StockValue;
        }
    }

    
}
