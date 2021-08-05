using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class SaleEntries
    {
        SqlConnection _conn;
        SqlTransaction _conTran;
        string _sql;
        SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();
        Int64 iAccountTranNo = 0;
        int iVouItemSlNo = 0;
        bool bAccountSameVoucher = false;

        public bool AddOrUpdateGasReceipt(List<GasReceipt> objGasReceipt)
        {
            bool bResult = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                
                objGasReceipt.Sort(delegate(GasReceipt x, GasReceipt y)
                {
                    if (x.BillOfLading == null && y.BillOfLading == null) return 0;
                    else if (x.BillOfLading == null) return -1;
                    else if (y.BillOfLading == null) return 1;
                    else return x.BillOfLading.CompareTo(y.BillOfLading);
                });

                #region Adding Gas Receipt

                foreach (GasReceipt obj in objGasReceipt)
                {
                    _sql = "DELETE FROM GAS_RECEIPT_MASTER WHERE GR_STORE_ID = " + obj.StoreID + " AND GR_BOL = '" + obj.BillOfLading + "'";
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                }

                string _bol = "";
                int ReceitpTranID = 0;
                int iRow = 0;
                int iSaleDayTranID = 0;

                foreach (GasReceipt obj in objGasReceipt)
                {
                    iRow++;
                    if (!_bol.Equals(obj.BillOfLading))
                    {
                        _bol = obj.BillOfLading;
                        ReceitpTranID = GetMaxGasTranID(obj.StoreID);
                        dmlExecute.AddFields("GR_STORE_ID", obj.StoreID.ToString());
                        dmlExecute.AddFields("GR_TRAN_ID", ReceitpTranID.ToString());
                        dmlExecute.AddFields("GR_DATE", obj.Date.ToString());
                        dmlExecute.AddFields("GR_SHIFT_CODE", obj.ShiftCode.ToString());
                        dmlExecute.AddFields("GR_DUE_DATE", obj.DueDate.ToString());
                        dmlExecute.AddFields("GR_BOL", obj.BillOfLading);
                        dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                        dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                        dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                        dmlExecute.ExecuteInsert("GAS_RECEIPT_MASTER", _conn, _conTran);
                        iSaleDayTranID = _SaleSupportEntries.DayTranIDWithTransaction(obj.Date, obj.StoreID,obj.ShiftCode, _conn, _conTran);
                    }

                    dmlExecute.AddFields("GRV_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("GRV_TRAN_ID", ReceitpTranID.ToString());
                    dmlExecute.AddFields("GRV_SLNO", iRow.ToString());
                    dmlExecute.AddFields("GRV_GAS_TYPE_ID", obj.GasTypeID.ToString());
                    dmlExecute.AddFields("GRV_GROSS_GALLONS", obj.GrossGallons.ToString());
                    dmlExecute.AddFields("GRV_NET_GALLONS", obj.NetGallons.ToString());
                    dmlExecute.ExecuteInsert("GAS_RECEIPT_DELIVERY", _conn, _conTran);
                    
                    _sql = "UPDATE STORE_GAS_SALE_BALANCES SET GAS_RECV = GAS_RECV + " + obj.NetGallons + " WHERE STORE_ID = " + obj.StoreID;
                    _sql += " AND SALE_DAY_TRAN_ID = " + iSaleDayTranID + " AND GAS_TYPE_ID = " + obj.GasTypeID;
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                }


                _conTran.Commit();

                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
        }

        public bool AddOrUpdateSale(SaleMaster objGasSale, string sTransactionType)
        {
            bool bResult = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                objGasSale.DayTranID = _SaleSupportEntries.DayTranID(objGasSale.Date, objGasSale.StoreID, objGasSale.ShiftCode, _conn);

                if (sTransactionType == "GAS_SALES")
                    bResult = AddGasSale(objGasSale);
                else if (sTransactionType == "GAS_CARD_BREAKUP")
                    bResult = AddGasReceipt(objGasSale);
                else if (sTransactionType == "GAS_INVENTORY_UPDATE")
                    bResult = AddGasInventory(objGasSale);
                else if (sTransactionType == "LOTTERY_SALE")
                    bResult = AddLotterySale(objGasSale);

                else if (sTransactionType == "LOTTERY_TRANSFER")
                    bResult = AddLotteryTransfer(objGasSale);

                else if (sTransactionType == "ACCOUNT_TRAN")
                    bResult = AddAccountPaidReceive(objGasSale);
                else if (sTransactionType == "BUSINESS_SALE")
                    bResult = AddBusinessSale(objGasSale);
                else if (sTransactionType == "CASH_DEPOSIT")
                    bResult = AddBusinessDeposit(objGasSale);
                else if (sTransactionType == "FINALIZE_TRAN")
                    bResult = FinalizeTran(objGasSale);
                else if (sTransactionType == "UNLOCK_DAY") // ACCESS TO ONLY GROUP ADMIN
                    bResult = UnlockDay(objGasSale);
                else if (sTransactionType == "PURCHASE") 
                    bResult = AddPurhaseOrPurchaseReturn(objGasSale,'I');
                else if (sTransactionType == "PURCHASE_RETURN") 
                    bResult = AddPurhaseOrPurchaseReturn(objGasSale, 'C');
                else if (sTransactionType == "CHEQUE_CASHING") 
                    bResult = AddChequeCashing(objGasSale);
                else if (sTransactionType == "CHEQUE_DEPOSIT")
                    bResult = ChequeCashingDeposit(objGasSale);

                return bResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
        }

        #region Gas Sales

        private bool AddGasSale(SaleMaster objGasSale)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Sales Master Fields
                dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objGasSale.ShiftCode.ToString());
                dmlExecute.AddFields("SALE_DATE", objGasSale.Date.ToString());
                dmlExecute.AddFields("SALE_GAS_TOTAL_GALLONS", objGasSale.TotalSaleGallons.ToString());
                dmlExecute.AddFields("SALE_GAS_TOTAL_TOTALIER", objGasSale.TotalTotalizer.ToString());
                dmlExecute.AddFields("SALE_GAS_TOTAL_SALE", objGasSale.TotalSale.ToString());
                dmlExecute.AddFields("CREATED_BY", objGasSale.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objGasSale.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objGasSale.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasSale.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Gas Sales

                _sql = "DELETE FROM STORE_GAS_SALE_TRAN WHERE STORE_ID = " + objGasSale.StoreID + " AND ";
                _sql += "SALE_DAY_TRAN_ID = " + objGasSale.DayTranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                foreach (GasSaleModel objSaleTran in objGasSale.GasSale)
                {
                    if (objSaleTran.SaleGallons > 0)
                    {
                        dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                        dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                        dmlExecute.AddFields("SALE_DATE", objGasSale.Date.ToString());
                        dmlExecute.AddFields("GASTYPE_ID", objSaleTran.GasTypeID.ToString());
                        dmlExecute.AddFields("SALE_TOTALIZER", objSaleTran.Totalizer.ToString());
                        dmlExecute.AddFields("SALE_GALLONS", objSaleTran.SaleGallons.ToString());
                        dmlExecute.AddFields("SALE_AMOUNT", objSaleTran.SaleAmount.ToString());
                        dmlExecute.AddFields("SALE_PRICE", objSaleTran.SalePrice.ToString());
                        dmlExecute.AddFields("SET_PRICE", objSaleTran.SetPrice.ToString());
                        dmlExecute.AddFields("CREATED_BY", objGasSale.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", objGasSale.CreateTimeStamp);
                        dmlExecute.AddFields("MODIFIED_BY", objGasSale.ModifiedUserName);
                        dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasSale.ModifiedTimeStamp);
                        dmlExecute.ExecuteInsert("STORE_GAS_SALE_TRAN", _conn, _conTran);
                    }
                }

                UpdateCashTranForGas(objGasSale.StoreID, objGasSale.DayTranID, objGasSale);
                
                #endregion

                #region Adding to Account Transaction
                float fSale = objGasSale.TotalSale;
                float fCardTotal = 0;
                float fCash = 0;

                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'GS') AND 
                            (ACTTRN_STORE_ID = " + objGasSale.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objGasSale.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                _sql = @"SELECT     SALE_GAS_CARD_TOTAL
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + objGasSale.StoreID.ToString() + ") AND (SALE_DAY_TRAN_ID = " + objGasSale.DayTranID.ToString() + ")";

                SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                    fCardTotal = Convert.ToSingle(cmd.ExecuteScalar());
                fCash = fSale - fCardTotal;

                //Entry: Cash A/c Dr Gas Dealer A/c Dr To Sale - Here the card amount would be debitted to Gas Dealer
                
                // Adding for Cash Entry
                bAccountSameVoucher = false;
                UpdateAccountEntry(objGasSale.StoreID, "DR", 1, objGasSale.Date, fCash, "Gas Sale", objGasSale.DayTranID,"GS");
                UpdateAccountEntry(objGasSale.StoreID, "DR", 38, objGasSale.Date, fCardTotal, "Gas Sale", objGasSale.DayTranID, "GS");
                UpdateAccountEntry(objGasSale.StoreID, "CR", 5, objGasSale.Date, fSale, "Gas Sale", objGasSale.DayTranID, "GS");
                _conTran.Commit();
                #endregion

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private bool AddGasReceipt(SaleMaster objGasSale)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Sales Master Fields
                dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objGasSale.Date.ToString());
                dmlExecute.AddFields("SALE_GAS_CARD_TOTAL", objGasSale.CardTotal.ToString());
                dmlExecute.AddFields("CREATED_BY", objGasSale.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objGasSale.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objGasSale.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasSale.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Gas Sales Card BreakUp

                _sql = "DELETE FROM STORE_GAS_SALE_CARD_BREAKUP WHERE STORE_ID = " + objGasSale.StoreID + " AND ";
                _sql += "SALE_DAY_TRAN_ID = " + objGasSale.DayTranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                foreach (GasSaleReceipt objSaleTran in objGasSale.GasReceipt)
                {
                    if (objSaleTran.CardAmount > 0)
                    {
                        dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                        dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                        dmlExecute.AddFields("CARD_TYPE_ID", objSaleTran.CardTypeID.ToString());
                        dmlExecute.AddFields("CARD_AMOUNT", objSaleTran.CardAmount.ToString());
                        dmlExecute.AddFields("CREATED_BY", objGasSale.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", objGasSale.CreateTimeStamp);
                        dmlExecute.AddFields("MODIFIED_BY", objGasSale.ModifiedUserName);
                        dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasSale.ModifiedTimeStamp);
                        dmlExecute.ExecuteInsert("STORE_GAS_SALE_CARD_BREAKUP", _conn, _conTran);
                    }
                }

                _sql = "DELETE FROM STORE_RECONCILLATION_ENTRIES WHERE RECON_STORE_ID = " + objGasSale.StoreID + " AND ";
                _sql += "RECON_REF_NO = " + objGasSale.DayTranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                _sql = @"SELECT  SUM(STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT) AS CARD_AMOUNT
                            FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                        MAPPING_STORE_CARD ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = MAPPING_STORE_CARD.STORE_ID AND 
                                                        STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID = MAPPING_STORE_CARD.CARD_TYPE_ID
                            WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = PARMSTOREID) AND (STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = PARMDAYTRANID) AND 
                                                        (MAPPING_STORE_CARD.CARD_CREDIT_TYPE = 'G')";

                _sql = _sql.Replace("PARMSTOREID",objGasSale.StoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID",objGasSale.DayTranID.ToString());

                SqlCommand cmd;
                float fGasdealerCardTotal = 0;
                float fVendorCardTotal = 0;

                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                {
                    if (Convert.ToSingle(cmd.ExecuteScalar()) > 0)
                    {
                        fGasdealerCardTotal = Convert.ToSingle(cmd.ExecuteScalar());
                        dmlExecute.AddFields("RECON_STORE_ID", objGasSale.StoreID.ToString());
                        dmlExecute.AddFields("RECON_TYPE_ID", "1");
                        dmlExecute.AddFields("RECON_REF_NO", objGasSale.DayTranID.ToString());
                        dmlExecute.AddFields("RECON_REF_DATE", objGasSale.Date.ToString());
                        dmlExecute.AddFields("RECON_AMOUNT", fGasdealerCardTotal.ToString());

                        dmlExecute.ExecuteInsert("STORE_RECONCILLATION_ENTRIES", _conn, _conTran);
                    }
                }

                _sql = @"SELECT  SUM(STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT) AS CARD_AMOUNT
                            FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                        MAPPING_STORE_CARD ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = MAPPING_STORE_CARD.STORE_ID AND 
                                                        STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID = MAPPING_STORE_CARD.CARD_TYPE_ID
                            WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = PARMSTOREID) AND (STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = PARMDAYTRANID) AND 
                                                        (MAPPING_STORE_CARD.CARD_CREDIT_TYPE = 'V')";

                _sql = _sql.Replace("PARMSTOREID", objGasSale.StoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID", objGasSale.DayTranID.ToString());
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                {
                    if (Convert.ToSingle(cmd.ExecuteScalar()) > 0)
                    {
                        fVendorCardTotal = Convert.ToSingle(cmd.ExecuteScalar());
                    }
                }
                // Adding for Cash Entry
                UpdateCashTranForGas(objGasSale.StoreID, objGasSale.DayTranID, objGasSale);

                #endregion

                #region Adding to Account Transaction
                float fSale = 0;
                float fCardTotal = objGasSale.CardTotal;
                float fCash = 0;

                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'GS') AND 
                            (ACTTRN_STORE_ID = " + objGasSale.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objGasSale.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                _sql = @"SELECT     SALE_GAS_TOTAL_SALE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + objGasSale.StoreID.ToString() + ") AND (SALE_DAY_TRAN_ID = " + objGasSale.DayTranID.ToString() + ")";

                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                    fSale = Convert.ToSingle(cmd.ExecuteScalar());
                fCash = fSale - fCardTotal;

                //Entry: Cash A/c Dr Gas Dealer A/c Dr Bank Account Dr To Sale - Here the card amount would be debitted to Gas Dealer
                
                bAccountSameVoucher = false;
                if (fCash > 0)
                    UpdateAccountEntry(objGasSale.StoreID, "DR", 1, objGasSale.Date, fCash, "Gas Sale", objGasSale.DayTranID, "GS");
                if (fGasdealerCardTotal > 0)
                    UpdateAccountEntry(objGasSale.StoreID, "DR", 38, objGasSale.Date, fGasdealerCardTotal, "Gas Sale", objGasSale.DayTranID, "GS");
                if (fVendorCardTotal > 0)
                    UpdateAccountEntry(objGasSale.StoreID, "DR", 25, objGasSale.Date, fVendorCardTotal, "Gas Sale", objGasSale.DayTranID, "GS");

                UpdateAccountEntry(objGasSale.StoreID, "CR", 5, objGasSale.Date, fSale, "Gas Sale", objGasSale.DayTranID, "GS");
                _conTran.Commit();

                #endregion

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private bool AddGasInventory(SaleMaster objGasSale)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                SqlCommand sqlcmd ;
                

                #region Adding Sales Master Fields
                dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objGasSale.Date.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objGasSale.ShiftCode.ToString());
                dmlExecute.AddFields("CREATED_BY", objGasSale.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objGasSale.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objGasSale.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasSale.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Gas Inventory Updates
                float fPurchase = 0;

                foreach (GasInventoryUpdate objInventory in objGasSale.GasInventory)
                {
                    if (objInventory.TankID == 0)
                        objInventory.TankID = 1;

                    _sql = "DELETE FROM STORE_GAS_SALE_BALANCES WHERE STORE_ID = " + objGasSale.StoreID + " AND ";
                    _sql += "SALE_DAY_TRAN_ID = " + objGasSale.DayTranID + " AND STORE_GAS_TANK_ID = " + objInventory.TankID ;
                    _sql += " AND GAS_TYPE_ID = " + objInventory.GasTypeID;
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                    if ((objInventory.OpeningBalance > 0) || (objInventory.GasReceived > 0) || (objInventory.GasPrice > 0) || (objInventory.ActualClosingBalance > 0)
                            || (objInventory.StickInches > 0) || (objInventory.StickGallons > 0))
                    {
                        _sql = @"SELECT  SUM(GAS_RECEIPT_DELIVERY.GRV_NET_GALLONS) AS GRV_NET_GALLONS
                                    FROM            GAS_RECEIPT_DELIVERY INNER JOIN
                                        GAS_RECEIPT_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND 
                                        GAS_RECEIPT_DELIVERY.GRV_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID
                                    WHERE        (GAS_RECEIPT_DELIVERY.GRV_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = PARMGASTYPEID) AND 
                                        (GAS_RECEIPT_MASTER.GR_DATE = 'PARMDATE') AND (GAS_RECEIPT_MASTER.GR_SHIFT_CODE = PARMSHIFTID)";

                        _sql = _sql.Replace("PARMDATE", objGasSale.Date.ToString());
                        _sql = _sql.Replace("PARMSTOREID", objGasSale.StoreID.ToString());
                        _sql = _sql.Replace("PARMGASTYPEID", objInventory.GasTypeID.ToString());
                        _sql = _sql.Replace("PARMSHIFTID", objGasSale.ShiftCode.ToString());
                        sqlcmd = new SqlCommand(_sql, _conn, _conTran);
                        if (sqlcmd.ExecuteScalar().ToString().Length > 0)
                            objInventory.GasReceived = Convert.ToSingle(sqlcmd.ExecuteScalar());

                        dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                        dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                        dmlExecute.AddFields("STORE_GAS_TANK_ID", objInventory.TankID.ToString());
                        dmlExecute.AddFields("GAS_TYPE_ID", objInventory.GasTypeID.ToString());

                        dmlExecute.AddFields("GAS_OP_BAL", objInventory.OpeningBalance.ToString());
                        dmlExecute.AddFields("GAS_RECV", objInventory.GasReceived.ToString());
                        dmlExecute.AddFields("GAS_RECV_PRICE", objInventory.GasPrice.ToString());
                        dmlExecute.AddFields("GAS_ACT_CL_BAL", objInventory.ActualClosingBalance.ToString());
                        dmlExecute.AddFields("GAS_STICK_INCHES", objInventory.StickInches.ToString());
                        dmlExecute.AddFields("GAS_STICK_GALLONS", objInventory.StickGallons.ToString());

                        dmlExecute.AddFields("CREATED_BY", objGasSale.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", objGasSale.CreateTimeStamp);
                        dmlExecute.AddFields("MODIFIED_BY", objGasSale.ModifiedUserName);
                        dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasSale.ModifiedTimeStamp);
                        dmlExecute.ExecuteInsert("STORE_GAS_SALE_BALANCES", _conn, _conTran);

                        fPurchase += objInventory.GasPrice;
                    }
                }

                #endregion

                #region Adding Account Transaction


                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'GP') AND 
                            (ACTTRN_STORE_ID = " + objGasSale.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objGasSale.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                //Entry: Purchase A/c Dr To Gas Dealer 

                // Adding for Cash Entry
                if (fPurchase > 0)
                {
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objGasSale.StoreID, "DR", 6, objGasSale.Date, fPurchase, "Gas Purchase", objGasSale.DayTranID, "GP");
                    UpdateAccountEntry(objGasSale.StoreID, "CR", 38, objGasSale.Date, fPurchase, "Gas Purchase", objGasSale.DayTranID, "GP");
                }
                _conTran.Commit();

                #endregion

                #region Gas System Closing Stock
                DayEndModel objDayEndModel = new ReportGas().GasSystemClosingBalanance(objGasSale.StoreID, objGasSale.Date, objGasSale.ShiftCode);
                _conTran = _conn.BeginTransaction();

                foreach (DayStock o in objDayEndModel.DayStocks)
                {
                    dmlExecute.AddFields("GAS_SYSTEM_CL_BAL", o.SystemClosingQty.ToString());
                    dmlExecute.AddFields("STORE_ID", objGasSale.StoreID.ToString());
                    dmlExecute.AddFields("SALE_DAY_TRAN_ID", objGasSale.DayTranID.ToString());
                    dmlExecute.AddFields("GAS_TYPE_ID", o.GasTypeID.ToString());

                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID", "GAS_TYPE_ID" };
                    dmlExecute.ExecuteUpdate("STORE_GAS_SALE_BALANCES", _conn, KeyFields, _conTran);
                }

                _conTran.Commit();


                #endregion


                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private void UpdateCashTranForGas(int iStoreID, int iDayTranID, SaleMaster objGasSale)
        {
            float fCash = 0;
            SqlCommand cmd;

            _sql = @"SELECT        SUM(SALE_AMOUNT) AS Expr1
                    FROM            STORE_GAS_SALE_TRAN
                    WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DAY_TRAN_ID = " + iDayTranID + ")";

            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0) 
                fCash = Convert.ToSingle(cmd.ExecuteScalar());

            _sql = @"SELECT        SUM(CARD_AMOUNT) AS Expr1
                        FROM            STORE_GAS_SALE_CARD_BREAKUP
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DAY_TRAN_ID = " + iDayTranID + ")";

            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0) 
                fCash = fCash - Convert.ToSingle(cmd.ExecuteScalar());

            DMLExecute dmlExecute = new DMLExecute();

            #region  Delete from cash transaction before inserts

            _sql = "SELECT ACTCASH_VOUCHER_NO FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + iStoreID + " AND ACTCASH_TRAN_TYPE = 'GC' ";
            _sql += " AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + iDayTranID ;

            SqlDataReader dr;
            Int32 iVoucherNo = 0;
            cmd = new SqlCommand(_sql, _conn, _conTran);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                iVoucherNo = Convert.ToInt32(dr[0]);
                dr.Close();
            }
            else
            {
                dr.Close();
                _sql = "SELECT MAX(ACTCASH_VOUCHER_NO) FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + iStoreID;
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0) 
                    iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                else
                    iVoucherNo = 1;
            }

            _sql = "DELETE FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + iStoreID + " AND ACTCASH_TRAN_TYPE = 'GC' ";
            _sql += " AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + iDayTranID ;
            dmlExecute.ExecuteDMLSQL(_sql,_conn,_conTran);
            #endregion

            dmlExecute.AddFields("ACTCASH_STORE_ID",iStoreID.ToString());
            dmlExecute.AddFields("ACTCASH_VOUCHER_NO",iVoucherNo.ToString());
            dmlExecute.AddFields("ACTCASH_DATE",objGasSale.Date.ToString());
            dmlExecute.AddFields("ACTCASH_TRAN_TYPE","GC");
            dmlExecute.AddFields("ACTCASH_SALE_PMT_DAY_TRAN_ID",iDayTranID.ToString());
            dmlExecute.AddFields("ACTCASH_INWARD_OUTWARD_TYPE","I");
            dmlExecute.AddFields("ACTCASH_AMOUNT",fCash.ToString());
            dmlExecute.ExecuteInsert("ACTCASH_TRAN",_conn,_conTran);
        }

        #endregion

        #region Lottery Sale
        private bool AddLotterySale(SaleMaster objLotterySale)
        {
            bool bResult = false;
            NextShift nextShift = new NextShift();
            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                float fSale = 0;
                float fPaid = 0;
                float fInstantPurchase = 0;
                float fOnlinePurchase = 0;
                float fSystemClosingBalance = 0;
                LotteryMaster objLotteryMaster = new LotteryMaster();
                int iGameLastTicketNumber = 0;
                Int32 SlNo = 0;

                #region Adding Sales Master Fields
                dmlExecute.AddFields("STORE_ID", objLotterySale.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objLotterySale.Date.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objLotterySale.ShiftCode.ToString());
                dmlExecute.AddFields("SALE_LOTTERY_RETURN", objLotterySale.LotteryReturn.ToString());

                dmlExecute.AddFields("SALE_LOTTERY_BOOKS_ACTIVE", _SaleSupportEntries.GetBookActivePerShift(objLotterySale.Date, objLotterySale.StoreID, objLotterySale.ShiftCode, _conn, _conTran).ToString());

                dmlExecute.AddFields("SALE_LOTTERY_ONLINE", objLotterySale.LotteryOnline.ToString());
                dmlExecute.AddFields("SALE_LOTTERY_CASH_INSTANT_PAID", objLotterySale.LotteryCashInstantPaid.ToString());
                dmlExecute.AddFields("SALE_LOTTERY_CASH_ONLINE_PAID", objLotterySale.LotteryCashOnlinePaid.ToString());
                dmlExecute.AddFields("SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE", objLotterySale.LotteryCashPhysicalOpeningBalance.ToString());
                dmlExecute.AddFields("SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE", objLotterySale.LotteryCashPhysicalClosingBalance.ToString());


                dmlExecute.AddFields("CREATED_BY", objLotterySale.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objLotterySale.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objLotterySale.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objLotterySale.ModifiedTimeStamp);
                
                fPaid += objLotterySale.LotteryCashInstantPaid;
                fPaid += objLotterySale.LotteryCashOnlinePaid;
                fInstantPurchase += objLotterySale.LotteryBooksActive;
                fOnlinePurchase += objLotterySale.LotteryOnline;

                if (_SaleSupportEntries.IsEdit == true)
                {
                    if (!_SaleSupportEntries.GetLotteryShiftStatus(objLotterySale.DayTranID, objLotterySale.StoreID, _conn, _conTran))
                    {
                        throw new Exception("Lottery Shift is closed and the entries cannot be edited");
                    }

                    OnlyMaster onlyMaster = new OnlyMaster();
                    onlyMaster = _SaleSupportEntries.GetSaleMasterEntries(objLotterySale.DayTranID, objLotterySale.StoreID, _conn, _conTran);

                    objLotterySale.LotteryCashTransfer = onlyMaster.LotteryCashTransfer;
                    onlyMaster = null;

                    dmlExecute.AddFields("LOTTERY_SHIFT_OPEN", "CLOSE");
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.AddFields("LOTTERY_SHIFT_OPEN", "OPEN");
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                LoginDal objLoginDal = new LoginDal();
                objLoginDal.UpdateLotteryAutoSettle(objLotterySale.StoreID, objLotterySale.Date,_conn,_conTran);

                #endregion

                #region Adding Lottery Sale

                //Lottery closing reading edit is de-activated because to protect lottery updates in reports
                //_sql = "DELETE FROM STORE_SALE_LOTTERY_CLOSING WHERE LTRSAL_STORE_ID = " + objLotterySale.StoreID + " AND ";
                //_sql += " LTRSAL_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID;
                //dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                foreach (LotterySale objLottery in objLotterySale.LotteryClosingCount)
                {
                    dmlExecute.AddFields("LTRSAL_STORE_ID", objLotterySale.StoreID.ToString());
                    dmlExecute.AddFields("LTRSAL_SALE_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
                    dmlExecute.AddFields("LTRSAL_BOX_ID", objLottery.BoxID.ToString());
                    if (objLottery.GameID == null)
                        objLottery.GameID = "";

                    if ((objLottery.LastTicketClosing >= 0) && (objLottery.NoOfTickets >0))
                    {
                        dmlExecute.AddFields("LTRSAL_GAME_ID", objLottery.GameID.ToString());
                        dmlExecute.AddFields("LTRSAL_PACK_NO", objLottery.PackNo.ToString());
                        dmlExecute.AddFields("LTRSAL_LAST_TICKET_NO", objLottery.LastTicketClosing.ToString());
                    }
                    else
                    {
                        _sql = @"SELECT   REPORT_LOTTERY_DAILY.REPL_BOX_ID, REPORT_LOTTERY_DAILY.REPL_GAME_ID, REPORT_LOTTERY_DAILY.REPL_PACK_NO, 
                                                                 GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO
                                        FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                                                 STORE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                                 REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                                        WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = PARMSTOREID) AND (REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = PARMDAYTRANID) AND 
                                                                 (REPORT_LOTTERY_DAILY.REPL_BOX_ID = PARMBOXID) AND (REPORT_LOTTERY_DAILY.REPL_END_TICKET IS NULL)";

                        _sql = _sql.Replace("PARMSTOREID",objLotterySale.StoreID.ToString());
                        _sql = _sql.Replace("PARMDAYTRANID",objLotterySale.DayTranID.ToString());
                        _sql = _sql.Replace("PARMBOXID", objLottery.BoxID.ToString());

                        SqlDataReader drEmptyBox;
                        SqlCommand cmdEmptyBox = new SqlCommand(_sql, _conn, _conTran);
                        drEmptyBox = cmdEmptyBox.ExecuteReader();
                        if (drEmptyBox.Read())
                        {
                            iGameLastTicketNumber = (int)drEmptyBox["LOTTERY_BOOK_LAST_TICKET_NO"] + 1;
                            dmlExecute.AddFields("LTRSAL_GAME_ID", drEmptyBox["REPL_GAME_ID"].ToString());
                            dmlExecute.AddFields("LTRSAL_PACK_NO", drEmptyBox["REPL_PACK_NO"].ToString());
                            dmlExecute.AddFields("LTRSAL_LAST_TICKET_NO", iGameLastTicketNumber.ToString());
                        }
                        else
                        {
                            drEmptyBox.Close();
                            throw new Exception("Book not found");
                        }
                        drEmptyBox.Close();
                    }

                    dmlExecute.AddFields("CREATED_BY", objLotterySale.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", objLotterySale.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", objLotterySale.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", objLotterySale.ModifiedTimeStamp);
                    dmlExecute.ExecuteInsert("STORE_SALE_LOTTERY_CLOSING", _conn, _conTran);

                    if ((objLottery.LastTicketClosing >= 0) && (objLottery.NoOfTickets > 0))
                    {
                        _sql = "UPDATE REPORT_LOTTERY_DAILY SET REPL_END_TICKET = " + objLottery.LastTicketClosing;
                        _sql += " WHERE REPL_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + " AND REPL_STORE_ID = " + objLotterySale.StoreID;
                        _sql += " AND REPL_GAME_ID = " + objLottery.GameID + " AND REPL_PACK_NO = " + objLottery.PackNo;
                        _sql += " AND REPL_BOX_ID = " + objLottery.BoxID;
                    }
                    else // If they dont scan box, then upon confirmation, system consider no end ticket is available and sold all the tickets, hence updating to last ticket number 
                    {
                        _sql = "UPDATE REPORT_LOTTERY_DAILY SET REPL_END_TICKET = " + iGameLastTicketNumber;
                        _sql += " WHERE REPL_END_TICKET IS NULL AND REPL_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + " AND REPL_STORE_ID = " + objLotterySale.StoreID;
                        _sql += " AND REPL_GAME_ID = " + objLottery.GameID + " AND REPL_PACK_NO = " + objLottery.PackNo;
                    }
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                    #region Inserting row to move current lottery balances to next shift
                    if (nextShift.NextDayTranID == 0)
                    {
                        nextShift = _SaleSupportEntries.NextShift(objLotterySale.Date, objLotterySale.StoreID, objLotterySale.ShiftCode, _conn, _conTran);
                    }

                    #region Adding Sales Master Fields for next day

                        dmlExecute.AddFields("STORE_ID", objLotterySale.StoreID.ToString());
                        dmlExecute.AddFields("SALE_DAY_TRAN_ID", nextShift.NextDayTranID.ToString());
                        dmlExecute.AddFields("SALE_DATE", nextShift.NextDate.ToString());
                        dmlExecute.AddFields("SALE_SHIFT_CODE", nextShift.NextShiftID.ToString());
                        dmlExecute.AddFields("LOTTERY_SHIFT_OPEN", "OPEN");
                        dmlExecute.AddFields("CREATED_BY", objLotterySale.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", objLotterySale.CreateTimeStamp);
                        dmlExecute.AddFields("SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE", objLotterySale.LotteryCashPhysicalClosingBalance.ToString());

                        _sql = "SELECT * FROM STORE_SALE_MASTER WHERE STORE_ID = PARMSTOREID AND SALE_DAY_TRAN_ID = PARMDAYTRANID";
                        _sql = _sql.Replace("PARMSTOREID", objLotterySale.StoreID.ToString());
                        _sql = _sql.Replace("PARMDAYTRANID", nextShift.NextDayTranID.ToString());
                        SqlDataReader drCheckDay;
                        SqlCommand cmdCheckDay = new SqlCommand(_sql, _conn, _conTran);
                        drCheckDay = cmdCheckDay.ExecuteReader();
                        if (drCheckDay.HasRows)
                            _SaleSupportEntries.IsEdit = true;
                        drCheckDay.Close();

                    if (_SaleSupportEntries.IsEdit == true)
                        {
                            string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                            dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                        }
                        else
                        {
                            dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                            _SaleSupportEntries.IsEdit = true;
                        }
                    #endregion

                    if ((objLottery.LastTicketClosing >= 0) && (objLottery.NoOfTickets > 0)) // Ticket is scanned
                    {
                        SlNo = GetSlNoFromDailyReport(objLotterySale.StoreID, objLottery.BoxID);
                        dmlExecute.AddFields("REPL_STORE_ID", objLotterySale.StoreID.ToString());
                        dmlExecute.AddFields("REPL_SALE_DAY_TRAN_ID", nextShift.NextDayTranID.ToString());
                        dmlExecute.AddFields("REPL_GAME_ID", objLottery.GameID.ToString());
                        dmlExecute.AddFields("REPL_BOX_ID", objLottery.BoxID.ToString());
                        dmlExecute.AddFields("REPL_PACK_NO", objLottery.PackNo.ToString());
                        dmlExecute.AddFields("REPL_SLNO", SlNo.ToString());
                        dmlExecute.AddFields("REPL_START_TICKET", (objLottery.LastTicketClosing).ToString());
                        dmlExecute.AddFields("CREATED_BY", objLotterySale.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", objLotterySale.CreateTimeStamp);
                        dmlExecute.ExecuteInsert("REPORT_LOTTERY_DAILY", _conn, _conTran);
                    }
                    #endregion
                }

                #endregion

                #region Update Lottery Instant sale amount
                // Since lottery startning starts with 0, I added the value 1 to number of tickets i.e. ending ticket - starting ticket

                _sql = @"SELECT   SUM(((REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET)) 
                               * GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE) AS Sold
                            FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                                     STORE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                     REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = " + objLotterySale.StoreID + ") AND (REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + ")";
                SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);

                if (cmd.ExecuteScalar() != null)
                {
                    objLotterySale.LotterySale = Convert.ToSingle(cmd.ExecuteScalar());

                    fSystemClosingBalance = objLotterySale.LotteryOnline + objLotterySale.LotterySale + objLotterySale.LotteryCashPhysicalOpeningBalance - fPaid - objLotterySale.LotteryCashTransfer;

                    dmlExecute.AddFields("STORE_ID", objLotterySale.StoreID.ToString());
                    dmlExecute.AddFields("SALE_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
                    dmlExecute.AddFields("SALE_LOTTERY_SALE", objLotterySale.LotterySale.ToString());
                    dmlExecute.AddFields("SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE", fSystemClosingBalance.ToString());

                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }


                #endregion

                UpdateCashTranForLotterySale(objLotterySale);

                #region Adding Account Transaction
                fSale = objLotterySale.LotterySale + objLotterySale.LotteryOnline;

                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'LS') AND 
                            (ACTTRN_STORE_ID = " + objLotterySale.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                //Entry: Cash A/c Dr To LOTTERY SALE 

                // Adding for Cash Entry
                bAccountSameVoucher = false;
                UpdateAccountEntry(objLotterySale.StoreID, "DR", 21, objLotterySale.Date, fSale, "Lottery Sale", objLotterySale.DayTranID, "LS");
                UpdateAccountEntry(objLotterySale.StoreID, "CR", 4, objLotterySale.Date, fSale, "Lottery Sale", objLotterySale.DayTranID, "LS");

                //Adding paidout entries
                //Entry: Lottery Owner A/c   Dr  To Cash A/c
                if (fPaid > 0)
                {
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objLotterySale.StoreID, "DR", 11, objLotterySale.Date, fPaid, "Lottery Paidout", objLotterySale.DayTranID, "LS");
                    UpdateAccountEntry(objLotterySale.StoreID, "CR", 21, objLotterySale.Date, fPaid, "Lottery Paidout", objLotterySale.DayTranID, "LS");
                }

                StoreMasterDal objStoreMaster = new StoreMasterDal();
                float fTemp2 = 0;

                //Adding Instant Lottery Purchase 
                //Entry: Purchase A/c   Dr  To Lottery Owner  
                if (fInstantPurchase > 0)
                {
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objLotterySale.StoreID, "DR", 13, objLotterySale.Date, fInstantPurchase, "Instant Lottery Purchase", objLotterySale.DayTranID, "LS");
                    UpdateAccountEntry(objLotterySale.StoreID, "CR", 11, objLotterySale.Date, fInstantPurchase, "Instant Lottery Purchase", objLotterySale.DayTranID, "LS");
                }

                //Adding Online Lottery Purchase 
                //Entry: Purchase A/c   Dr  To Lottery Owner 
                if (fOnlinePurchase > 0)
                {
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objLotterySale.StoreID, "DR", 13, objLotterySale.Date, fOnlinePurchase, "Online Lottery Purchase", objLotterySale.DayTranID, "LS");
                    UpdateAccountEntry(objLotterySale.StoreID, "CR", 11, objLotterySale.Date, fOnlinePurchase, "Online Lottery Purchase", objLotterySale.DayTranID, "LS");
                }

                //Adding Online Lottery commission entries
                fTemp2 = 0;
                fTemp2 = objLotterySale.LotteryInstantCommission + objLotterySale.LotteryOnlineCommission + objLotterySale.LotteryCashCommission;

                if (fTemp2 > 0)
                {
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objLotterySale.StoreID, "DR", 11, objLotterySale.Date, fTemp2, "Online Lottery Paidout Commission", objLotterySale.DayTranID, "LS");
                    UpdateAccountEntry(objLotterySale.StoreID, "CR", 12, objLotterySale.Date, fTemp2, "Online Lottery Paidout Commission", objLotterySale.DayTranID, "LS");
                }

                _conTran.Commit();

                #endregion

                _sql = "DELETE FROM LOTTERY_TEMPORARY_CLOSING_READING WHERE TMPCLS_STORE_ID = " + objLotterySale.StoreID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private bool AddLotteryTransfer(SaleMaster objLotterySale)
        {
            bool bResult = false;
            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                LotteryMaster objLotteryMaster = new LotteryMaster();
                Int16 iLedgerID = 0;
                string sText = string.Empty;

                #region Adding Sales Master Fields

                
                dmlExecute.AddFields("STORE_ID", objLotterySale.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objLotterySale.Date.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objLotterySale.ShiftCode.ToString());

                dmlExecute.AddFields("SALE_LOTTERY_CASH_TOTAL_TRANSFER", objLotterySale.LotteryTransferList.Sum(i => i.TranAmount).ToString());

                dmlExecute.AddFields("CREATED_BY", objLotterySale.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objLotterySale.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objLotterySale.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objLotterySale.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.AddFields("LOTTERY_SHIFT_OPEN", "OPEN");
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding to Business Entry
                _sql = @"DELETE FROM            STORE_BUSINESS_TRANS
                            WHERE        (BUSINESS_TRAN_FROM = 'LC') AND 
                            (BUSINESS_STORE_ID = " + objLotterySale.StoreID + ") AND (BUSINESS_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                foreach (LotteryTransfer objLottery in objLotterySale.LotteryTransferList)
                {
                    if (objLottery.TranAmount > 0)
                    {
                        if (objLottery.TranType == "LD")
                        {
                            iLedgerID = 24;
                            sText = "LOTTERY BANK A/C";
                        }
                        else if (objLottery.TranType == "GC")
                        {
                            iLedgerID = 22;
                            sText = "GAS CASH";
                        }
                        else if (objLottery.TranType == "BC")
                        {
                            iLedgerID = 23;
                            sText = "BUSINESS CASH";
                        }

                        dmlExecute.AddFields("BUSINESS_STORE_ID", objLotterySale.StoreID.ToString());
                        dmlExecute.AddFields("BUSINESS_SALE_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
                        dmlExecute.AddFields("BUSINESS_TRAN_TYPE", objLottery.TranType);
                        dmlExecute.AddFields("BUSINESS_TRAN_FROM", "LC");
                        dmlExecute.AddFields("BUSINESS_ACTLED_ID", iLedgerID.ToString());
                        dmlExecute.AddFields("BUSINESS_AMOUNT", objLottery.TranAmount.ToString());
                        dmlExecute.AddFields("BUSINESS_DISPLAY_NAME", sText);
                        dmlExecute.AddFields("BUSINESS_PMT_TYPE", "CS");
                        dmlExecute.AddFields("BUSINESS_REMARKS", "CASH TRANSFERRED TO " + sText + " from Lottery Cash");

                        dmlExecute.ExecuteInsert("STORE_BUSINESS_TRANS", _conn, _conTran);
                    }
                }

                #endregion

                UpdateCashTranForLotteryTransfer(objLotterySale);

                #region Adding Account Transaction

                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'LC') AND 
                            (ACTTRN_STORE_ID = " + objLotterySale.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);


                // Adding for Cash Entry Outward
                bAccountSameVoucher = false;
                foreach (LotteryTransfer objLottery in objLotterySale.LotteryTransferList)
                {
                    if (objLottery.TranType == "LD")
                    {
                        iLedgerID = 24;
                        sText = "LOTTERY BANK A/C";
                    }
                    else if (objLottery.TranType == "GC")
                    {
                        iLedgerID = 22;
                        sText = "GAS CASH";
                    }
                    else if (objLottery.TranType == "BC")
                    {
                        iLedgerID = 23;
                        sText = "BUSINESS CASH";
                    }
                    UpdateAccountEntry(objLotterySale.StoreID, "DR", iLedgerID, objLotterySale.Date, objLottery.TranAmount, "Amount transferred to " + sText, objLotterySale.DayTranID, "LC");
                }
                UpdateAccountEntry(objLotterySale.StoreID, "CR", 21, objLotterySale.Date, Convert.ToSingle(objLotterySale.LotteryTransferList.Sum(i => i.TranAmount).ToString()), "Amount transferred from Lottery Cash", objLotterySale.DayTranID, "LC");

                _conTran.Commit();

                #endregion

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private void UpdateCashTranForLotterySale(SaleMaster objLotterySale)
        {
            float fCash = 0;
            SqlCommand cmd;

            fCash = objLotterySale.LotterySale + objLotterySale.LotteryOnline;
            fCash = fCash - objLotterySale.LotteryCashInstantPaid - objLotterySale.LotteryCashOnlinePaid - objLotterySale.LotteryReturn;

            DMLExecute dmlExecute = new DMLExecute();

            #region  Delete from cash transaction before inserts

            _sql = "SELECT ACTCASH_VOUCHER_NO FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objLotterySale.StoreID + " AND ACTCASH_TRAN_TYPE = 'LC' ";
            _sql += " AND ACTCASH_INWARD_OUTWARD_TYPE = 'I' AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + objLotterySale.DayTranID;
            SqlDataReader dr;
            Int32 iVoucherNo = 0;
            cmd = new SqlCommand(_sql, _conn, _conTran);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                iVoucherNo = Convert.ToInt32(dr[0]);
                dr.Close();
            }
            else
            {
                dr.Close();
                _sql = "SELECT MAX(ACTCASH_VOUCHER_NO) FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objLotterySale.StoreID;
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                    iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                else
                    iVoucherNo = 1;
            }

            _sql = "DELETE FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objLotterySale.StoreID + " AND ACTCASH_TRAN_TYPE = 'LC' ";
            _sql += "  AND ACTCASH_INWARD_OUTWARD_TYPE = 'I'  AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + objLotterySale.DayTranID;
            dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
            #endregion

            dmlExecute.AddFields("ACTCASH_STORE_ID", objLotterySale.StoreID.ToString());
            dmlExecute.AddFields("ACTCASH_VOUCHER_NO", iVoucherNo.ToString());
            dmlExecute.AddFields("ACTCASH_DATE", objLotterySale.Date.ToString());
            dmlExecute.AddFields("ACTCASH_TRAN_TYPE", "LC");
            dmlExecute.AddFields("ACTCASH_SALE_PMT_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
            dmlExecute.AddFields("ACTCASH_INWARD_OUTWARD_TYPE", "I");
            dmlExecute.AddFields("ACTCASH_AMOUNT", fCash.ToString());
            dmlExecute.ExecuteInsert("ACTCASH_TRAN", _conn, _conTran);
        }

        private void UpdateCashTranForLotteryTransfer(SaleMaster objLotterySale)
        {
            SqlCommand cmd;
            Int64 iVoucherNo;

            DMLExecute dmlExecute = new DMLExecute();

            #region  Delete from cash transaction before inserts

            _sql = "SELECT MAX(ACTCASH_VOUCHER_NO) FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objLotterySale.StoreID;
            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iVoucherNo = 1;

            _sql = "DELETE FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objLotterySale.StoreID + " AND ACTCASH_TRAN_TYPE = 'LC' ";
            _sql += "  AND ACTCASH_INWARD_OUTWARD_TYPE = 'O'  AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + objLotterySale.DayTranID;
            dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
            #endregion

            foreach (LotteryTransfer obj in objLotterySale.LotteryTransferList)
            {
                dmlExecute.AddFields("ACTCASH_STORE_ID", objLotterySale.StoreID.ToString());
                dmlExecute.AddFields("ACTCASH_VOUCHER_NO", iVoucherNo.ToString());
                dmlExecute.AddFields("ACTCASH_DATE", objLotterySale.Date.ToString());
                dmlExecute.AddFields("ACTCASH_TRAN_TYPE", "LC");
                dmlExecute.AddFields("ACTCASH_TRAN_TYPE_TO", obj.TranType);
                dmlExecute.AddFields("ACTCASH_SALE_PMT_DAY_TRAN_ID", objLotterySale.DayTranID.ToString());
                dmlExecute.AddFields("ACTCASH_INWARD_OUTWARD_TYPE", "O");
                dmlExecute.AddFields("ACTCASH_AMOUNT", obj.TranAmount.ToString());
                dmlExecute.ExecuteInsert("ACTCASH_TRAN", _conn, _conTran);
                iVoucherNo++;
            }
        }
        #endregion

        #region Account Paid / Receivables (Cash paid / Cheque paid / Cheque Received)
        private bool AddAccountPaidReceive(SaleMaster objAccountTran)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Account Tran Master Fields
                dmlExecute.AddFields("STORE_ID", objAccountTran.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objAccountTran.Date.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objAccountTran.ShiftCode.ToString());
                dmlExecute.AddFields("CREATED_BY", objAccountTran.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objAccountTran.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objAccountTran.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objAccountTran.ModifiedTimeStamp);
                dmlExecute.AddFields("BUSINESS_SHIFT_OPEN", "OPEN");

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Account Transaction
                int iActId = 0;
                float fCash = 0;

                foreach (AccountPaidReceivables objAccount in objAccountTran.PaymentAccounts)
                {
                    if (objAccount.Amount > 0)
                    {
                        dmlExecute.AddFields("BUSINESS_STORE_ID", objAccountTran.StoreID.ToString());
                        dmlExecute.AddFields("BUSINESS_SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                        dmlExecute.AddFields("BUSINESS_TRAN_TYPE", objAccount.AccountTranType.ToString());
                        dmlExecute.AddFields("BUSINESS_ACTLED_ID", objAccount.AccountLedgerID.ToString());
                        dmlExecute.AddFields("BUSINESS_AMOUNT", objAccount.Amount.ToString());
                        dmlExecute.AddFields("BUSINESS_DISPLAY_NAME", objAccount.DisplayName.ToString());
                        dmlExecute.AddFields("BUSINESS_REMARKS", objAccount.PaymentRemarks.ToString());

                        dmlExecute.ExecuteInsert("STORE_BUSINESS_TRANS", _conn, _conTran);

                        switch (objAccount.AccountTranType)
                        {
                            case "CP" :
                                iActId = 1;
                                break;
                            case "QP":
                                iActId = 26;
                                break;
                            case "CR":
                                iActId = 1;
                                break;
                            case "QR":
                                iActId = 26;
                                break;
                            default:
                                throw new Exception("Account tran type is invalid");
                        }

                        if (objAccount.AccountTranType.ToString() == "CP")
                        {
                            iActId = 1; // Setting ID to Cash ID
                        }

                        //Entry: Ledger Paid (Individual ledger id) A/c Dr To Cash / Bank

                        // Adding for Cash / Cheque paidEntry
                        bAccountSameVoucher = false;
                        if (objAccount.AccountTranType != "QR" && objAccount.AccountTranType != "CR")
                        {
                            UpdateAccountEntry(objAccountTran.StoreID, "DR", objAccount.AccountLedgerID, objAccountTran.Date, objAccount.Amount, "Paid to " + objAccount.DisplayName + " " + objAccount.PaymentRemarks, objAccountTran.DayTranID, objAccount.AccountTranType);
                            UpdateAccountEntry(objAccountTran.StoreID, "CR", iActId, objAccountTran.Date, objAccount.Amount, "Paid to " + objAccount.DisplayName + " " + objAccount.PaymentRemarks, objAccountTran.DayTranID, objAccount.AccountTranType);
                        }
                        else
                        {
                            UpdateAccountEntry(objAccountTran.StoreID, "DR", iActId, objAccountTran.Date, objAccount.Amount, "Received from " + objAccount.DisplayName + " " + objAccount.PaymentRemarks, objAccountTran.DayTranID, objAccount.AccountTranType);
                            UpdateAccountEntry(objAccountTran.StoreID, "CR", objAccount.AccountLedgerID, objAccountTran.Date, objAccount.Amount, "Received from " + objAccount.DisplayName + " " + objAccount.PaymentRemarks, objAccountTran.DayTranID, objAccount.AccountTranType);
                        }
                    }
                }

                //if (fCash > 0)
                //    UpdateCashTranForCashPaid(objAccountTran, fCash);

                _conTran.Commit();
                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private void UpdateCashTranForCashPaid(SaleMaster objAccountTran, float fCash)
        {
            SqlCommand cmd;

            DMLExecute dmlExecute = new DMLExecute();

            #region  Delete from cash transaction before inserts

            _sql = "SELECT ACTCASH_VOUCHER_NO FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objAccountTran.StoreID + " AND ACTCASH_TRAN_TYPE = 'CP' ";
            _sql += " AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + objAccountTran.DayTranID;
            SqlDataReader dr;
            Int32 iVoucherNo = 0;
            cmd = new SqlCommand(_sql, _conn, _conTran);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                iVoucherNo = Convert.ToInt32(dr[0]);
                dr.Close();
            }
            else
            {
                dr.Close();
                _sql = "SELECT MAX(ACTCASH_VOUCHER_NO) FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objAccountTran.StoreID;
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                    iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                else
                    iVoucherNo = 1;
            }

            _sql = "DELETE FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + objAccountTran.StoreID + " AND ACTCASH_TRAN_TYPE = 'CP' ";
            _sql += " AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + objAccountTran.DayTranID;
            dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
            #endregion

            dmlExecute.AddFields("ACTCASH_STORE_ID", objAccountTran.StoreID.ToString());
            dmlExecute.AddFields("ACTCASH_VOUCHER_NO", iVoucherNo.ToString());
            dmlExecute.AddFields("ACTCASH_DATE", objAccountTran.Date.ToString());
            dmlExecute.AddFields("ACTCASH_TRAN_TYPE", "CP");
            dmlExecute.AddFields("ACTCASH_SALE_PMT_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
            dmlExecute.AddFields("ACTCASH_INWARD_OUTWARD_TYPE", "O");
            dmlExecute.AddFields("ACTCASH_AMOUNT", fCash.ToString());
            dmlExecute.ExecuteInsert("ACTCASH_TRAN", _conn, _conTran);
        }


        #endregion

        #region Business Sale / Business Paid 
        private bool AddBusinessSale(SaleMaster objAccountTran)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Account Tran Master Fields
                dmlExecute.AddFields("STORE_ID", objAccountTran.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objAccountTran.Date.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objAccountTran.ShiftCode.ToString());
                dmlExecute.AddFields("BUSINESS_SHIFT_OPEN", "OPEN");

                dmlExecute.AddFields("CREATED_BY", objAccountTran.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objAccountTran.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objAccountTran.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objAccountTran.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Business Transaction
                string sTranType = "";
                foreach (AccountModel objAccount in objAccountTran.BusinessSaleCollection)
                {
                    if (!sTranType.Contains(objAccount.DisplaySide))
                        sTranType = sTranType + "'" + objAccount.DisplaySide + "',";
                }
                sTranType = sTranType.Substring(0, sTranType.Length - 1);

                _sql = "DELETE FROM STORE_BUSINESS_TRANS WHERE BUSINESS_STORE_ID = " + objAccountTran.StoreID + " AND ";
                _sql += "BUSINESS_SALE_DAY_TRAN_ID = " + objAccountTran.DayTranID + " AND BUSINESS_TRAN_TYPE IN (" + sTranType + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                float fBusinessSale = 0;
                float fBusinessPaid = 0;
                float fMoneyOrderSale = 0;
                float fMoneyOrderPaidOut = 0;
                int iActId = 0;

                AccountMasterDal _AccountDal = new AccountMasterDal();

                // Deleting the Business Entries before inserting
                _sql = @"DELETE FROM            ACTTRN_TRANS
                                    WHERE        (ACTTRN_RECORD_TYPE = 'BE') AND 
                                    (ACTTRN_STORE_ID = " + objAccountTran.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objAccountTran.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);


                foreach (AccountModel objAccount in objAccountTran.BusinessSaleCollection)
                {
                    if (objAccount.Amount > 0)
                    {
                        //if (objAccount.DisplayType.ToString() == "Write") // Reading the transactions other than gas / lottery / credit card
                        // I need to check for ledger id 31 (CREDIT CARD) whether i need insert into BUSINESS_TRANS, so that on monthly statement, it displays the cash in hand
                        if ((objAccount.LedgerID != 4) && (objAccount.LedgerID != 5) &&
                            (objAccount.LedgerID != 31) && (objAccount.LedgerID != 32) &&
                            (objAccount.LedgerID != 33) && (objAccount.LedgerID != 40))
                        {
                            dmlExecute.AddFields("BUSINESS_STORE_ID", objAccountTran.StoreID.ToString());
                            dmlExecute.AddFields("BUSINESS_SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                            dmlExecute.AddFields("BUSINESS_TRAN_TYPE", objAccount.DisplaySide.ToString());
                            dmlExecute.AddFields("BUSINESS_ACTLED_ID", objAccount.LedgerID.ToString());
                            dmlExecute.AddFields("BUSINESS_AMOUNT", objAccount.Amount.ToString());
                            dmlExecute.AddFields("BUSINESS_DISPLAY_NAME", objAccount.LedgerName.ToString());

                            if (objAccount.PaymentType != null)
                            {
                                if (objAccount.PaymentType.Length == 0)
                                    objAccount.PaymentType = "CS";
                                else
                                {
                                    if (objAccount.PaymentType == "Automatic Bank Payments(EFT)")
                                        objAccount.PaymentType = "AP";
                                    else if (objAccount.PaymentType == "Automatic Bank Receipts")
                                        objAccount.PaymentType = "AR";
                                }
                            }
                            else
                            {
                                objAccount.PaymentType = "CS";
                            }


                            dmlExecute.AddFields("BUSINESS_PMT_TYPE", objAccount.PaymentType.ToString());

                            dmlExecute.ExecuteInsert("STORE_BUSINESS_TRANS", _conn, _conTran);

                            if (objAccount.DisplaySide.ToString() == "BS")
                            {
                                iActId = 3; // Initially setting to Bank Ledger ID
                                if (_AccountDal.GetAccountGroupID(objAccountTran.StoreID, objAccount.LedgerID) != 6)
                                {
                                    iActId = 1;  // Setting to Cash Ledger ID
                                    if ((objAccount.LedgerID == 34) || (objAccount.LedgerID == 35) || (objAccount.LedgerID == 36) || (objAccount.LedgerID == 37))
                                        fMoneyOrderSale += objAccount.Amount;
                                    else
                                        fBusinessSale += objAccount.Amount;
                                }

                                #region Adding Account Transaction

                                //Entry: Cash / Bank A/c Dr To Sale Ledger ID 

                                // Adding for Cash Entry
                                bAccountSameVoucher = false;
                                UpdateAccountEntry(objAccountTran.StoreID, "DR", iActId, objAccountTran.Date, objAccount.Amount, "Sale for " + objAccount.LedgerName, objAccountTran.DayTranID, "BE");
                                UpdateAccountEntry(objAccountTran.StoreID, "CR", objAccount.LedgerID, objAccountTran.Date, objAccount.Amount, "Sale for " + objAccount.LedgerName, objAccountTran.DayTranID, "BE");

                                #endregion
                            }
                            else if (objAccount.DisplaySide.ToString() == "BP")
                            {
                                if (_AccountDal.GetAccountGroupID(objAccountTran.StoreID, objAccount.LedgerID) != 6)
                                {
                                    iActId = 1;
                                    if ((objAccount.LedgerID == 34) || (objAccount.LedgerID == 35) || (objAccount.LedgerID == 36) || (objAccount.LedgerID == 37))
                                        fMoneyOrderPaidOut += objAccount.Amount;
                                    else
                                    {
                                        if (objAccount.PaymentType.ToString().ToUpper() == "CS")
                                            fBusinessPaid += objAccount.Amount;
                                        else
                                            iActId = 3;
                                    }
                                    //TO BE ADDED
                                    
                                    #region Adding Account Transaction

                                    //Entry: Sale Ledger ID Dr To Cash / Bank A/c

                                    // Adding for Cash Entry
                                    bAccountSameVoucher = false;
                                    UpdateAccountEntry(objAccountTran.StoreID, "DR", objAccount.LedgerID, objAccountTran.Date, objAccount.Amount, "Paid for " + objAccount.LedgerName, objAccountTran.DayTranID, "BE");
                                    UpdateAccountEntry(objAccountTran.StoreID, "CR", iActId, objAccountTran.Date, objAccount.Amount, "Paid for " + objAccount.LedgerName, objAccountTran.DayTranID, "BE");

                                    #endregion
                                }
                            }
                        }
                    }
                }

                if (fBusinessSale > 0)
                    UpdateCashTranForBusinessEntry("BS",fBusinessSale,objAccountTran.Date,objAccountTran.StoreID,objAccountTran.DayTranID,"I");

                if (fBusinessPaid > 0)
                    UpdateCashTranForBusinessEntry("BP", fBusinessPaid, objAccountTran.Date, objAccountTran.StoreID, objAccountTran.DayTranID, "O");

                if (fMoneyOrderSale > 0)
                    UpdateCashTranForBusinessEntry("BS", fMoneyOrderSale, objAccountTran.Date, objAccountTran.StoreID, objAccountTran.DayTranID, "I", "MO");

                if (fMoneyOrderPaidOut > 0)
                    UpdateCashTranForBusinessEntry("BP", fMoneyOrderPaidOut, objAccountTran.Date, objAccountTran.StoreID, objAccountTran.DayTranID, "O", "MO");

                _conTran.Commit();
                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private void UpdateCashTranForBusinessEntry(string sTranType, float fCash, DateTime Date, int iStoreID, int iDayTranID, string cInwOut, string sTranSubStat = "N")
        {
            SqlCommand cmd;

            DMLExecute dmlExecute = new DMLExecute();

            #region  Delete from cash transaction before inserts

            _sql = "SELECT ACTCASH_VOUCHER_NO FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + iStoreID + " AND ACTCASH_TRAN_TYPE = '" + sTranType + "' ";
            _sql += " AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + iDayTranID + " AND ACTCAST_SUB_TRAN_TYPE = '" + sTranSubStat + "'";
            SqlDataReader dr;
            Int32 iVoucherNo = 0;
            cmd = new SqlCommand(_sql, _conn, _conTran);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                iVoucherNo = Convert.ToInt32(dr[0]);
                dr.Close();
            }
            else
            {
                dr.Close();
                _sql = "SELECT MAX(ACTCASH_VOUCHER_NO) FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + iStoreID;
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                    iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                else
                    iVoucherNo = 1;
            }

            if (sTranSubStat == "N") // Making sure not to delete for money order transaction.  Because, if it deletes for MO, it would delete business transactions also
            {
                _sql = "DELETE FROM ACTCASH_TRAN WHERE ACTCASH_STORE_ID = " + iStoreID + " AND ACTCASH_TRAN_TYPE = '" + sTranType + "' ";
                _sql += " AND ACTCASH_SALE_PMT_DAY_TRAN_ID = " + iDayTranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
            }
            #endregion

            // It update tran type to MD for money order deposit.  For sale and paid out, it will log as BS / BD and sub tran type to MD


            dmlExecute.AddFields("ACTCASH_STORE_ID", iStoreID.ToString());
            dmlExecute.AddFields("ACTCASH_VOUCHER_NO", iVoucherNo.ToString());
            dmlExecute.AddFields("ACTCASH_DATE", Date.ToString());
            dmlExecute.AddFields("ACTCASH_TRAN_TYPE", sTranType);
            dmlExecute.AddFields("ACTCASH_SALE_PMT_DAY_TRAN_ID", iDayTranID.ToString());
            dmlExecute.AddFields("ACTCASH_INWARD_OUTWARD_TYPE", cInwOut.ToString());
            dmlExecute.AddFields("ACTCASH_AMOUNT", fCash.ToString());
            dmlExecute.AddFields("ACTCAST_SUB_TRAN_TYPE", sTranSubStat);
            dmlExecute.ExecuteInsert("ACTCASH_TRAN", _conn, _conTran);
        }

        #endregion

        public bool DeletePayment(SaleMaster objAccountTran)
        {
            bool bResult = false;

            string _sql = string.Empty;
            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Delete Account Transaction
                int iVouID = 0;

                foreach (AccountPaidReceivables objAccount in objAccountTran.PaymentAccounts)
                {
                    if (objAccount.Amount > 0)
                    {
                        _sql = @"DELETE FROM STORE_BUSINESS_TRANS WHERE BUSINESS_STORE_ID = PARM_STORE_ID AND BUSINESS_SALE_DAY_TRAN_ID = PARM_DAY_TRAN_ID 
                            AND BUSINESS_ACTLED_ID = PARM_ACTLED_ID AND BUSINESS_AMOUNT = PARM_AMOUNT";

                        _sql = _sql.Replace("PARM_ACTLED_ID", objAccount.AccountLedgerID.ToString());
                        _sql = _sql.Replace("PARM_AMOUNT", objAccount.Amount.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objAccountTran.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                        _sql = @"SELECT ACTTRN_ID  FROM ACTTRN_TRANS
                                    WHERE(ACTTRN_STORE_ID = PARM_STORE_ID) AND(ACTTRN_ACTLED_ID = PARM_ACTLED_ID) AND(ACTTRN_AMOUNT = PARM_AMOUNT) 
                                    AND (ACTTRN_SALE_DAY_TRAN_ID = PARM_DAY_TRAN_ID)";

                        _sql = _sql.Replace("PARM_ACTLED_ID", objAccount.AccountLedgerID.ToString());
                        _sql = _sql.Replace("PARM_AMOUNT", objAccount.Amount.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objAccountTran.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());

                        SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
                        SqlDataReader sdr = cmd.ExecuteReader();
                        if (sdr.Read())
                        {
                            iVouID = Convert.ToInt16(sdr["ACTTRN_ID"]);
                        }
                        sdr.Close();

                        _sql = @"DELETE FROM ACTTRN_TRANS WHERE (ACTTRN_STORE_ID = PARM_STORE_ID) AND (ACTTRN_ID = PARM_ACTTRN_ID) 
                                    AND (ACTTRN_SALE_DAY_TRAN_ID = PARM_DAY_TRAN_ID)";

                        _sql = _sql.Replace("PARM_ACTTRN_ID", iVouID.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objAccountTran.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                        _sql = @"DELETE FROM ACTCASH_TRAN WHERE (ACTCASH_STORE_ID = PARM_STORE_ID) AND (ACTCASH_VOUCHER_NO = PARM_ACTTRN_ID) 
                                    AND (ACTCASH_SALE_PMT_DAY_TRAN_ID = PARM_DAY_TRAN_ID)";

                        _sql = _sql.Replace("PARM_ACTTRN_ID", iVouID.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objAccountTran.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                    }
                }

                _conTran.Commit();
                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        public bool DeleteJournalVoucher(JournalVoucher objEntry)
        {
            bool bResult = false;

            string _sql = string.Empty;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                long DayTranID = _SaleSupportEntries.DayTranID(objEntry.Date, objEntry.StoreID, objEntry.ShiftCode, _conn);

                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Delete Account Transaction
                int iVouID = 0;


                _sql = @"DELETE FROM STORE_BUSINESS_TRANS WHERE BUSINESS_STORE_ID = PARM_STORE_ID AND BUSINESS_SALE_DAY_TRAN_ID = PARM_DAY_TRAN_ID 
                            AND BUSINESS_ACTLED_ID = PARM_ACTLED_ID AND BUSINESS_AMOUNT = PARM_AMOUNT";

                        _sql = _sql.Replace("PARM_ACTLED_ID", objEntry.AccountLedgerID.ToString());
                        _sql = _sql.Replace("PARM_AMOUNT", objEntry.Amount.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objEntry.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", DayTranID.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                        _sql = @"SELECT ACTTRN_ID  FROM ACTTRN_TRANS
                                    WHERE(ACTTRN_STORE_ID = PARM_STORE_ID) AND(ACTTRN_ACTLED_ID = PARM_ACTLED_ID) AND(ACTTRN_AMOUNT = PARM_AMOUNT) 
                                    AND (ACTTRN_SALE_DAY_TRAN_ID = PARM_DAY_TRAN_ID)";

                        _sql = _sql.Replace("PARM_ACTLED_ID", objEntry.AccountLedgerID.ToString());
                        _sql = _sql.Replace("PARM_AMOUNT", objEntry.Amount.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objEntry.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", DayTranID.ToString());

                        SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
                        SqlDataReader sdr = cmd.ExecuteReader();
                        if (sdr.Read())
                        {
                            iVouID = Convert.ToInt16(sdr["ACTTRN_ID"]);
                        }
                        sdr.Close();

                        _sql = @"DELETE FROM ACTTRN_TRANS WHERE (ACTTRN_STORE_ID = PARM_STORE_ID) AND (ACTTRN_ID = PARM_ACTTRN_ID) 
                                    AND (ACTTRN_SALE_DAY_TRAN_ID = PARM_DAY_TRAN_ID)";

                        _sql = _sql.Replace("PARM_ACTTRN_ID", iVouID.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objEntry.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", DayTranID.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                        _sql = @"DELETE FROM ACTCASH_TRAN WHERE (ACTCASH_STORE_ID = PARM_STORE_ID) AND (ACTCASH_VOUCHER_NO = PARM_ACTTRN_ID) 
                                    AND (ACTCASH_SALE_PMT_DAY_TRAN_ID = PARM_DAY_TRAN_ID)";

                        _sql = _sql.Replace("PARM_ACTTRN_ID", iVouID.ToString());
                        _sql = _sql.Replace("PARM_STORE_ID", objEntry.StoreID.ToString());
                        _sql = _sql.Replace("PARM_DAY_TRAN_ID", DayTranID.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                _conTran.Commit();
                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private bool AddBusinessDeposit(SaleMaster objAccountTran)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Account Tran Master Fields

                objAccountTran.CashDeposited = 0;
                foreach (BankDeposit o in objAccountTran.BankDepositDetail)
                {
                    objAccountTran.CashDeposited += o.Deposit;
                }

                dmlExecute.AddFields("STORE_ID", objAccountTran.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objAccountTran.Date.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objAccountTran.ShiftCode.ToString());
                dmlExecute.AddFields("CASH_OPENING_BALANCE", objAccountTran.CashOpeningBalance.ToString());
                dmlExecute.AddFields("CASH_DEPOSITED_IN_BANK", objAccountTran.CashDeposited.ToString());
                dmlExecute.AddFields("CASH_CLOSING_BALANCE", objAccountTran.CashClosingBalance.ToString());
                dmlExecute.AddFields("CREATED_BY", objAccountTran.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objAccountTran.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objAccountTran.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objAccountTran.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Account Transaction
                string sTranType = "";

                // Deleting Gas deposit, lottery deposit and business deposit
                _sql = "DELETE FROM STORE_BUSINESS_TRANS WHERE BUSINESS_STORE_ID = " + objAccountTran.StoreID + " AND ";
                _sql += "BUSINESS_SALE_DAY_TRAN_ID = " + objAccountTran.DayTranID + " AND BUSINESS_TRAN_TYPE IN ('GD','LD','BD','MD')";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                

                foreach (BankDeposit objAccount in objAccountTran.BankDepositDetail)
                {
                    if (objAccount.Deposit > 0)
                    {
                        if (objAccount.LedgerID == 26)
                            sTranType = "BD";
                        else if (objAccount.LedgerID == 25)
                            sTranType = "GD";
                        else if (objAccount.LedgerID == 24)
                            sTranType = "LD";
                        else if (objAccount.LedgerID == 34)
                            sTranType = "MD";

                        dmlExecute.AddFields("BUSINESS_STORE_ID", objAccountTran.StoreID.ToString());
                        dmlExecute.AddFields("BUSINESS_SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                        dmlExecute.AddFields("BUSINESS_TRAN_TYPE", sTranType);
                        dmlExecute.AddFields("BUSINESS_ACTLED_ID", objAccount.LedgerID.ToString());
                        dmlExecute.AddFields("BUSINESS_AMOUNT", objAccount.Deposit.ToString());
                        dmlExecute.AddFields("BUSINESS_DISPLAY_NAME", objAccount.LedgerName.ToString());
                        dmlExecute.ExecuteInsert("STORE_BUSINESS_TRANS", _conn, _conTran);

                        UpdateCashTranForBusinessEntry(sTranType, objAccount.Deposit, objAccountTran.Date, objAccountTran.StoreID, objAccountTran.DayTranID, "O");
                    }
                }

                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'BD') AND 
                            (ACTTRN_STORE_ID = " + objAccountTran.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objAccountTran.DayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                //Entry: Bank A/c Dr To Cash A/c

                // Adding for Cash Entry
                objAccountTran.CashDeposited = 0;
                foreach (BankDeposit objAccount in objAccountTran.BankDepositDetail)
                {
                    if (objAccount.Deposit > 0)
                    {
                        if (objAccount.LedgerID == 26)
                            sTranType = "BD";
                        else if (objAccount.LedgerID == 25)
                            sTranType = "GD";
                        else if (objAccount.LedgerID == 24)
                            sTranType = "LD";
                        else if (objAccount.LedgerID == 34)
                            sTranType = "MD";

                        objAccountTran.CashDeposited += objAccount.Deposit;
                        UpdateAccountEntry(objAccountTran.StoreID, "DR", objAccount.LedgerID, objAccountTran.Date, objAccount.Deposit, "Cash deposited into bank", objAccountTran.DayTranID, sTranType);
                    }
                }

                UpdateAccountEntry(objAccountTran.StoreID, "CR", 1, objAccountTran.Date, objAccountTran.CashDeposited, "Cash deposited into bank", objAccountTran.DayTranID, "BD");

                _conTran.Commit();
                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        public bool FinalizeTran(SaleMaster objAccountTran)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Account Tran Master Fields
                dmlExecute.AddFields("STORE_ID", objAccountTran.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objAccountTran.ShiftCode.ToString());

                dmlExecute.AddFields("CASH_PHYSICAL_AT_STORE", objAccountTran.CashPhysicalAtStore.ToString());
                dmlExecute.AddFields("CASH_CLOSING_BALANCE", objAccountTran.CashClosingBalance.ToString());
                dmlExecute.AddFields("CASH_OPENING_BALANCE", objAccountTran.CashOpeningBalance.ToString());
                dmlExecute.AddFields("BUSINESS_SHIFT_OPEN", "CLOSE");

                dmlExecute.AddFields("SALE_ENTRY_LOCKED", "N");
                dmlExecute.AddFields("CREATED_BY", objAccountTran.ModifiedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objAccountTran.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objAccountTran.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objAccountTran.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                _conTran.Commit();
                //return true;
                #endregion

                if (objAccountTran.StoreID != 3)
                {
                    #region Send eMail
                    string sBody;
                    string sBodyDeposits;
                    string sBodyGasSale;
                    string sBodyLottery;
                    float totalSale = 0;

                    EmailModel eMail = new EmailModel();
                    EmailService objEmailService = new EmailService();

                    if (objAccountTran.ModifiedUserName == null)
                        objAccountTran.ModifiedUserName = "admin";

                    string sql = @"SELECT  EMAIL_ID
                                FROM            USER_LOGIN
                                WHERE        (USER_ID = 'USERID')
                                UNION 
                                select EMAIL_ID FROM            USER_LOGIN WHERE USER_ACCESS_TYPE = 'G'";
                    sql = sql.Replace("USERID", objAccountTran.ModifiedUserName);
                    SqlCommand cmd = new SqlCommand(sql, _conn, _conTran);
                    SqlDataReader dr = cmd.ExecuteReader();

                    StoreMasterDal dalStoreMaster = new StoreMasterDal();
                    StoreMaster objStoreMaster = new StoreMaster();
                    objStoreMaster = dalStoreMaster.SelectRecord(objAccountTran.StoreID);
                    string sTemp = "";

                    while (dr.Read())
                    {
                        sTemp += dr["EMAIL_ID"].ToString() + ";";
                    }
                    dr.Close();
                    sTemp = sTemp.Substring(0, sTemp.Length - 1);
                    eMail.EmailTo = sTemp;

                    SaleMaster objSaleMaster = new SaleMaster();
                    objSaleMaster = GetSaleTransaction(objAccountTran.StoreID, objAccountTran.Date, objAccountTran.ShiftCode, "FINALIZE");

                    #region Gas Sales
                    sBodyGasSale = "<strong><u>Gas Sale</u></strong>";
                    sBodyGasSale += "<br>";

                    sBodyGasSale += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'>";
                    sBodyGasSale += "<tr>";
                    sBodyGasSale += "<td><strong>Particulars</strong></td><td style='text-align: center'><strong>Gallons</strong></td><td style='text-align: center'><strong>Sale</strong></td>";
                    sBodyGasSale += "</tr>";

                    foreach (GasSaleModel t in objSaleMaster.GasSale)
                    {
                        sBodyGasSale += "<tr>";
                        sBodyGasSale += "<td>" + t.GasTypeName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.SaleGallons) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.SaleAmount) + "</td>";
                        sBodyGasSale += "</tr>";
                        totalSale += t.SaleAmount;
                    }

                    sBodyGasSale += "<tr>";
                    sBodyGasSale += "<td><strong>Total Sale</strong></td><td></td><td style='text-align: right'><strong>" + String.Format("{0:#,0.000}", totalSale) + "</strong></td>";
                    sBodyGasSale += "</tr></strong>";
                    sBodyGasSale += "</table>";
                    sBodyGasSale += "</br>";
                    sBodyGasSale += "</br>";

                    sBodyGasSale += "<strong><u>Cards Received</u></strong>";
                    sBodyGasSale += "<br>";

                    sBodyGasSale += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'>";

                    sBodyGasSale += "<tr>";
                    sBodyGasSale += "<td><strong>Particulars</strong></td><td style='text-align: center'><strong>Amount</strong></td>";
                    sBodyGasSale += "</tr>";
                    totalSale = 0;
                    foreach (GasSaleReceipt t in objSaleMaster.GasReceipt)
                    {
                        sBodyGasSale += "<tr>";
                        sBodyGasSale += "<td>" + t.CardName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.CardAmount) + "</td>";
                        sBodyGasSale += "</tr>";
                        totalSale += t.CardAmount;
                    }


                    sBodyGasSale += "<tr>";
                    sBodyGasSale += "<td><strong>Total Card</strong></td><td style='text-align: right'><strong>" + String.Format("{0:#,0.000}", totalSale) + "</strong></td>";
                    sBodyGasSale += "</tr></strong>";

                    sBodyGasSale += "</table>";

                    totalSale = 0;

                    #endregion

                    #region Lotter Sales
                    sBodyLottery = "<strong><u>Lottery Sale</u></strong>";
                    sBodyLottery += "<br>";

                    sBodyLottery += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    sBodyLottery += "<td><strong>Particulars</strong></td><td style='text-align: right'><strong>Amount</strong></td>";
                    sBodyLottery += "</tr>";

                    sBodyLottery += "<tr><td>Instant Sale</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objSaleMaster.LotterySale) + "</td></tr>";
                    sBodyLottery += "<tr><td>Online Sale</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objSaleMaster.LotteryOnline) + "</td></tr>";
                    sBodyLottery += "<tr><td><strong>Net Sale</strong></td><td style='text-align: right'><strong>" + String.Format("{0:#,0.000}", objSaleMaster.LotterySale + objSaleMaster.LotteryOnline) + "</strong></td></tr>";

                    sBodyLottery += "<tr><td>Instant Paid</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objSaleMaster.LotteryCashInstantPaid) + "</td></tr>";
                    sBodyLottery += "<tr><td>Online Paid</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objSaleMaster.LotteryCashOnlinePaid) + "</td></tr>";

                    sBodyLottery += "</table>";

                    #endregion

                    #region Bank Deposit Entries
                    BankDepositForm _bankDepositForm = _SaleSupportEntries.BankDepositDetails(objAccountTran.StoreID, objAccountTran.Date, objAccountTran.ShiftCode);

                    sBodyDeposits = "<strong><u>Sales and Deposits</u></strong><br>";
                    sBodyDeposits += "<table  border='1' style='width: 40%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    sBodyDeposits += "<td><strong>Particulars</strong></td><td style='text-align: center'><strong>Received</strong></td>";
                    sBodyDeposits += "<td style='text-align: center'><strong>Paid Out</strong></td>";
                    sBodyDeposits += "<td style='text-align: center'><strong>Cash Balance</strong></td>";
                    sBodyDeposits += "<td style='text-align: center'><strong>Deposit</strong></td>";
                    sBodyDeposits += "</tr>";
                    foreach (BankDeposit t in _bankDepositForm.LedgerDetail)
                    {
                        if (t.LedgerID == 9)
                            t.LedgerName = "TOTAL BUSINESS COLLECTION";

                        sBodyDeposits += "<tr>";
                        sBodyDeposits += "<td>" + t.LedgerName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.LedgerSale) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.LedgerPaid) + "</td>";
                        sBodyDeposits += "<td style='text-align: right'>" + String.Format("{0:#,0.000}", t.Balance) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.Deposit) + "</td>";
                        sBodyDeposits += "</tr>";
                        string s = String.Format("{0:#,0.000}", t.Deposit);
                    }
                    sBodyDeposits += "</table>";
                    sBodyDeposits += "<br><br>";
                    sBodyDeposits += "<strong>Cash On Hand : " + String.Format("{0:#,0.000}", objSaleMaster.CashClosingBalance) + "</strong>";


                    #endregion

                    #region Business Sales
                    string sBodyStore;
                    sBodyStore = "<strong><u>Store Sales</u></strong>";
                    sBodyStore += "<br>";

                    sBodyStore += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    sBodyStore += "<td><strong>Particulars</strong></td><td style='text-align: right'><strong>Amount</strong></td>";
                    sBodyStore += "</tr>";
                    totalSale = 0;
                    foreach (AccountModel t in objSaleMaster.BusinessSaleCollection)
                    {
                        if ((t.DisplaySide == "BS") && (t.LedgerID != 8))
                        {
                            sBodyStore += "<tr>";
                            sBodyStore += "<td>" + t.LedgerName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.Amount) + "</td>";
                            sBodyStore += "</tr>";
                            totalSale += t.Amount;
                        }
                    }

                    if (totalSale > 0)
                    {
                        sBodyStore += "<tr>";
                        sBodyStore += "<td>Total Sale</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", totalSale) + "</td>";
                        sBodyStore += "</tr>";
                    }
                    sBodyStore += "</table>";

                    #endregion

                    #region Tax Collected
                    string sBodyStoreTaxCollected;
                    sBodyStoreTaxCollected = "<strong><u>Sale Tax</u></strong>";
                    sBodyStoreTaxCollected += "<br>";

                    sBodyStoreTaxCollected += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    sBodyStoreTaxCollected += "<td><strong>Particulars</strong></td><td style='text-align: right'><strong>Amount</strong></td>";
                    sBodyStoreTaxCollected += "</tr>";
                    totalSale = 0;
                    foreach (AccountModel t in objSaleMaster.BusinessSaleCollection)
                    {
                        if ((t.DisplaySide == "BS") && (t.LedgerID == 8))
                        {
                            sBodyStoreTaxCollected += "<tr>";
                            sBodyStoreTaxCollected += "<td>" + t.LedgerName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.Amount) + "</td>";
                            sBodyStoreTaxCollected += "</tr>";
                            totalSale += t.Amount;
                        }
                    }
                    sBodyStoreTaxCollected += "</table>";

                    #endregion

                    #region Business Paid
                    string sBodyStoreCashPaid;
                    sBodyStoreCashPaid = "<strong><u>Cash Paid</u></strong>";
                    sBodyStoreCashPaid += "<br>";

                    sBodyStoreCashPaid += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    sBodyStoreCashPaid += "<td><strong>Particulars</strong></td><td style='text-align: right'><strong>Amount</strong></td>";
                    sBodyStoreCashPaid += "</tr>";
                    totalSale = 0;
                    foreach (AccountModel t in objSaleMaster.BusinessSaleCollection)
                    {
                        if ((t.DisplaySide == "BP") && (t.PaymentType == "CS"))
                        {
                            sBodyStoreCashPaid += "<tr>";
                            sBodyStoreCashPaid += "<td>" + t.LedgerName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.Amount) + "</td>";
                            sBodyStoreCashPaid += "</tr>";
                            totalSale += t.Amount;
                        }
                    }
                    sBodyStoreCashPaid += "</table>";

                    #endregion

                    #region Business Paid Cheque
                    string sBodyStoreChequePaid;
                    sBodyStoreChequePaid = "<strong><u>Cheque Paid</u></strong>";
                    sBodyStoreChequePaid += "<br>";

                    sBodyStoreChequePaid += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    sBodyStoreChequePaid += "<td><strong>Particulars</strong></td><td style='text-align: right'><strong>Amount</strong></td>";
                    sBodyStoreChequePaid += "</tr>";
                    totalSale = 0;
                    foreach (AccountModel t in objSaleMaster.BusinessSaleCollection)
                    {
                        if ((t.DisplaySide == "BP") && (t.PaymentType == "CQ"))
                        {
                            sBodyStoreChequePaid += "<tr>";
                            sBodyStoreChequePaid += "<td>" + t.LedgerName + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.Amount) + "</td>";
                            sBodyStoreChequePaid += "</tr>";
                            totalSale += t.Amount;
                        }
                    }
                    sBodyStoreChequePaid += "</table>";

                    #endregion

                    #region Gas Stocks
                    DayEndModel objDayEndModel;
                    ReportGas objReportGas = new ReportGas();
                    string sBodyGasStock;
                    objDayEndModel = objReportGas.DayEndReport(objAccountTran.StoreID, objAccountTran.Date, objAccountTran.ShiftCode);
                    sBodyGasStock = "<strong><u>Gas Stocks</u></strong>";
                    sBodyGasStock += "<br>";

                    sBodyGasStock += "<table border='1' style='width: 25%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'>";
                    sBodyGasStock += "<tr>";
                    sBodyGasStock += "<td width='40%'><strong>Particulars</strong></td><td style='text-align: center'><strong>Opening</strong></td><td style='text-align: center'><strong>ReceivingGallon</strong></td><td style='text-align: center'><strong>Consumption</strong></td><td style='text-align: center'><strong>Closing Balance</strong></td><td style='text-align: center'><strong>Tank Reading</strong></td>";
                    sBodyGasStock += "</tr>";

                    foreach (DayStock t in objDayEndModel.DayStocks)
                    {
                        sBodyGasStock += "<tr>";
                        sBodyGasStock += "<td>" + t.GasType + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.OpenQty) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.InwardQty) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.SaleQty) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.SystemClosingQty) + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", t.ClosingQty) + "</td>";
                        sBodyGasStock += "</tr>";
                    }

                    sBodyGasStock += "</table>";
                    #endregion

                    sBody = sBodyGasSale;
                    sBody += "<br>";

                    sBody += sBodyLottery;
                    sBody += "<br>";

                    sBody += sBodyStore;
                    sBody += "<br>";

                    sBody += sBodyStoreTaxCollected;
                    sBody += "<br>";

                    sBody += sBodyStoreCashPaid;
                    sBody += "<br>";
                    sBody += sBodyStoreChequePaid;
                    sBody += "<br>";

                    sBody += sBodyDeposits;
                    sBody += "<br>";

                    sBody += sBodyGasStock;
                    sBody += "<br>";

                    sBody += "<strong><u>Cash at Counter</u></strong>";
                    sBody += objAccountTran.CashPhysicalAtStore;
                    sBody += "<br>";


                    //eMail.BodyInfo = "";

                    #region Adding Gas Profit and Loss Account

                    //ReportGasBalanceSheet objGasBS = new ReportGasBalanceSheet();
                    //ReportBSGas objBS = new ReportBSGas();
                    //objGasBS = objBS.GetBalanceSheet(objAccountTran.StoreID, objAccountTran.Date, objAccountTran.Date.AddDays(1));
                    //string sGasBS;

                    //sGasBS = "<table>";
                    //sGasBS = "<strong><u>Profit and Loss Account - Gas</u></strong><br>";
                    //sGasBS += "<table  border='1' style='width: 40%; table-layout: auto; border-collapse: separate; border-spacing: 1px; caption-side: top;'><tr>";
                    //sGasBS += "<td><strong>Particulars</strong></td><td style='text-align: center'><strong>Amount</strong></td>";
                    //sGasBS += "<td style='text-align: center'><strong>Particulars</strong></td>";
                    //sGasBS += "<td style='text-align: center'><strong>Amount</strong></td>";
                    //sGasBS += "</tr>";

                    //sGasBS += "<tr>";
                    //sGasBS += "<td>Opening Stock" + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objGasBS.OpeningStock) + "</td><td>Sales</td>";
                    //sGasBS += "<td style='text-align: right'>" + String.Format("{0:#,0.000}", objGasBS.Sales) + "</td>";
                    //sGasBS += "</tr>";

                    //sGasBS += "<tr>";
                    //sGasBS += "<td>Purchases" + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objGasBS.Purchase) + "</td><td>Closing Stock</td>";
                    //sGasBS += "<td style='text-align: right'>" + String.Format("{0:#,0.000}", objGasBS.ClosingStock) + "</td>";
                    //sGasBS += "</tr>";

                    //sGasBS += "<tr>";
                    //sGasBS += "<td>Gross Profit" + "</td><td style='text-align: right'>" + String.Format("{0:#,0.000}", objGasBS.GrossProfit) + "</td><td>Gross Loss</td>";
                    //sGasBS += "<td style='text-align: right'>" + String.Format("{0:#,0.000}", objGasBS.GrossLoss) + "</td>";
                    //sGasBS += "</tr>";

                    //sGasBS += "<tr>";
                    //sGasBS += "<td>Total" + "</td><td style='text-align: right'><b>" + String.Format("{0:#,0.000}", objGasBS.Total) + "</b></td><td>Total</td>";
                    //sGasBS += "<td style='text-align: right'><b>" + String.Format("{0:#,0.000}", objGasBS.Total) + "</b></td>";
                    //sGasBS += "</tr>";

                    //sGasBS += "</table>";
                    //sGasBS += "<br><br>";

                    #endregion

                    //sBody += sGasBS;

                    eMail.Subject = "Sales for the date of " + objAccountTran.Date + " for " + objStoreMaster.StoreName;
                    eMail.BodyInfo = sBody;
                    objEmailService.SendEmail(eMail);

                    #endregion
                }

                bResult = true;
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private bool UnlockDay(SaleMaster objAccountTran)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Account Tran Master Fields
                dmlExecute.AddFields("STORE_ID", objAccountTran.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objAccountTran.DayTranID.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", objAccountTran.ShiftCode.ToString());

                dmlExecute.AddFields("SALE_ENTRY_LOCKED", "N");
                dmlExecute.AddFields("CREATED_BY", objAccountTran.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objAccountTran.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objAccountTran.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objAccountTran.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    throw new Exception("Sales details were not entered for the selected date");
                }

                _conTran.Commit();
                #endregion
                
                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }


        public SaleMaster GetSaleTransaction(int iStoreID, DateTime dDate, int ShiftCode, string RequestType)
        {
            try
            {
                SqlCommand cmd;
                SqlDataReader dr;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                SaleMaster objSaleMaster = new SaleMaster();
                objSaleMaster.DayTranID = _SaleSupportEntries.DayTranID(dDate, iStoreID,ShiftCode, _conn);

                GasOilDal gasDal = new GasOilDal();

                objSaleMaster.GasTypes = gasDal.SelectRecords(iStoreID);

                //This is added before finding entry for set gas price
                objSaleMaster.GasSale = _SaleSupportEntries.GasSale(iStoreID, objSaleMaster.DayTranID, objSaleMaster.Date, _conn);

                _sql = @"SELECT    STORE_ID, SALE_DAY_TRAN_ID, SALE_DATE, SALE_GAS_TOTAL_GALLONS, SALE_GAS_TOTAL_TOTALIER, SALE_GAS_TOTAL_SALE, 
                             SALE_GAS_CARD_TOTAL, SALE_LOTTERY_RETURN, SALE_LOTTERY_SALE, SALE_LOTTERY_BOOKS_ACTIVE, SALE_LOTTERY_ONLINE, 
                             SALE_LOTTERY_CASH_INSTANT_PAID, SALE_LOTTERY_CASH_ONLINE_PAID, SALE_LOTTERY_ONLINE_COMMISSION, SALE_LOTTER_INSTANT_COMMISSION,
                                SALE_LOTTERY_CASH_COMMISSION, CASH_OPENING_BALANCE, CASH_PHYSICAL_AT_STORE, SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE,
                            SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE, SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE, SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE,
                             CASH_DEPOSITED_IN_BANK, CASH_CLOSING_BALANCE, SALE_ENTRY_LOCKED
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DAY_TRAN_ID = " + objSaleMaster.DayTranID + ")";

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    objSaleMaster.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objSaleMaster.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                    objSaleMaster.TotalSaleGallons = Convert.ToSingle(dr["SALE_GAS_TOTAL_GALLONS"]);
                    objSaleMaster.TotalTotalizer = Convert.ToSingle(dr["SALE_GAS_TOTAL_TOTALIER"]);
                    objSaleMaster.TotalSale = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"]);
                    objSaleMaster.CardTotal = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"]);
                    objSaleMaster.LotteryReturn = Convert.ToSingle(dr["SALE_LOTTERY_RETURN"]);
                    objSaleMaster.LotterySale = Convert.ToSingle(dr["SALE_LOTTERY_SALE"]);
                    objSaleMaster.LotteryBooksActive = Convert.ToSingle(dr["SALE_LOTTERY_BOOKS_ACTIVE"]);
                    objSaleMaster.LotteryOnline = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"]);
                    objSaleMaster.LotteryCashInstantPaid = Convert.ToSingle(dr["SALE_LOTTERY_CASH_INSTANT_PAID"]);
                    objSaleMaster.LotteryCashOnlinePaid = Convert.ToSingle(dr["SALE_LOTTERY_CASH_ONLINE_PAID"]);
                    objSaleMaster.LotteryInstantCommission = Convert.ToSingle(dr["SALE_LOTTER_INSTANT_COMMISSION"]);
                    objSaleMaster.LotteryOnlineCommission = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE_COMMISSION"]);
                    objSaleMaster.LotteryCashCommission = Convert.ToSingle(dr["SALE_LOTTERY_CASH_COMMISSION"]);
                    objSaleMaster.LotteryCashSystemOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE"]);
                    objSaleMaster.LotteryCashPhysicalOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE"]);
                    objSaleMaster.LotteryCashSystemClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE"]);
                    objSaleMaster.LotteryCashPhysicalClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE"]);

                    objSaleMaster.CashOpeningBalance = Convert.ToSingle(dr["CASH_OPENING_BALANCE"]);
                    objSaleMaster.CashPhysicalAtStore = Convert.ToSingle(dr["CASH_PHYSICAL_AT_STORE"]);
                    objSaleMaster.CashDeposited = Convert.ToSingle(dr["CASH_DEPOSITED_IN_BANK"]);
                    objSaleMaster.CashClosingBalance = Convert.ToSingle(dr["CASH_CLOSING_BALANCE"]);
                    objSaleMaster.EntryLockStatus = dr["SALE_ENTRY_LOCKED"].ToString();
                    dr.Close();
                    
                    if ((RequestType == "LOTTERY-SALE") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        LotteryMaster _lotterymaster = new LotteryMaster();
                        LotteryPreviousDaySale _objPrevDayLotterySale = new LotteryPreviousDaySale();
                        _objPrevDayLotterySale.Date = objSaleMaster.Date;
                        _objPrevDayLotterySale.StoreID = objSaleMaster.StoreID;
                        //_objPrevDayLotterySale = _lotterymaster.GetPrevDaySale(_objPrevDayLotterySale);
                        //objSaleMaster.LotteryInstantPreviousDaySale = _objPrevDayLotterySale.PrevDaySale;
                        objSaleMaster.LotteryClosingCount = _SaleSupportEntries.LotteryClosingReading(iStoreID, objSaleMaster.DayTranID, _conn);
                    }

                    if ((RequestType == "LOTTERY-TRANSFER") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.LotteryTransferList = _SaleSupportEntries.LotteryCashTransfer(iStoreID, objSaleMaster.DayTranID, _conn);
                    }

                    if ((RequestType == "GAS-STORE-SALE") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.GasReceipt = _SaleSupportEntries.GasSaleReceipts(iStoreID, objSaleMaster.DayTranID, _conn);
                        objSaleMaster.GasInventory = _SaleSupportEntries.GasSaleInvetory(iStoreID, objSaleMaster.DayTranID, _conn, objSaleMaster);
                        objSaleMaster.GasInvReceipt = _SaleSupportEntries.GasInvReceipt(iStoreID, objSaleMaster.Date, _conn);
                    }

                    if (RequestType == "GAS-STORE-INVENTORY")
                    {
                        objSaleMaster.GasInventory = _SaleSupportEntries.GasSaleInvetory(iStoreID, objSaleMaster.DayTranID, _conn, objSaleMaster);
                    }

                    if ((RequestType == "IN-STORE-SALE") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        //objSaleMaster.BusinessSaleCollection = _SaleSupportEntries.BusinessTran(iStoreID, objSaleMaster.Date);
                        objSaleMaster.BusinessSaleCollection = _SaleSupportEntries.BusinessTran(iStoreID, objSaleMaster.DayTranID,_conn);
                    }

                    if ((RequestType == "PAYMENTS") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.PaymentAccounts = _SaleSupportEntries.ListOfPayments(iStoreID, objSaleMaster.DayTranID, _conn);
                    }

                    if ((RequestType == "RECEIPTS") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.ReceiptAccounts = _SaleSupportEntries.ListOfReceipts(iStoreID, objSaleMaster.DayTranID, _conn);
                    }

                    if ((RequestType == "BANK-DEPOSIT") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        BankDepositForm _bankDepositForm = _SaleSupportEntries.BankDepositDetails(iStoreID, objSaleMaster.Date, objSaleMaster.ShiftCode);
                        objSaleMaster.CashOpeningBalance = _bankDepositForm.CashOpeningBalance;
                        objSaleMaster.BankDepositDetail = _bankDepositForm.LedgerDetail;
                    }

                    if ((RequestType == "PURCHASE") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.Purchase = _SaleSupportEntries.ListOfPurchases(iStoreID, objSaleMaster.DayTranID, _conn);
                        objSaleMaster.PurchaseReturn = _SaleSupportEntries.ListOfPurchasesReturns(iStoreID, objSaleMaster.DayTranID, _conn);
                    }

                    if ((RequestType == "CHEQUE-CASHING") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.ChequeCashingEntries = _SaleSupportEntries.ListOfChequeCashingEntries(iStoreID, objSaleMaster.DayTranID, _conn);
                    }

                    if ((RequestType == "CHEQUE-DEPOSIT") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.ChequeDeposits = _SaleSupportEntries.ListOfChequeToBeDeposited(iStoreID, _conn);
                    }
                    return objSaleMaster;
                }
                else
                {
                    dr.Close();
                    if ((RequestType == "CHEQUE-DEPOSIT") || (RequestType == "FINALIZE") || (RequestType == null))
                    {
                        objSaleMaster.ChequeDeposits = _SaleSupportEntries.ListOfChequeToBeDeposited(iStoreID, _conn);
                    }
                    return objSaleMaster;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _conn.Close();
                _conn.Dispose();
            }


        }

        private void UpdateAccountEntry(int iStoreID, string sDrCrType, int iLedgerID, DateTime dDate, float fAmount, string sRemarks, int iDayTranID, string sRecordType)
        {
            DMLExecute dmlExecute = new DMLExecute();

            if (bAccountSameVoucher == false) // This used to generate same voucher number for the complete entry
            {
                iVouItemSlNo = 1;

                if (iAccountTranNo == 0)
                    iAccountTranNo = GetAccountTranNo(iStoreID);
                else
                    iAccountTranNo += 1;

                bAccountSameVoucher = true;
            }
            else
                iVouItemSlNo += 1;

            dmlExecute.AddFields("ACTTRN_STORE_ID", iStoreID.ToString());
            dmlExecute.AddFields("ACTTRN_ID", iAccountTranNo.ToString());
            dmlExecute.AddFields("ACTTRN_TYPE", sDrCrType);
            dmlExecute.AddFields("ACTTRN_ACTLED_ID", iLedgerID.ToString());
            dmlExecute.AddFields("ACTTRN_SLNO", iVouItemSlNo.ToString());
            dmlExecute.AddFields("ACTTRN_DATE", dDate.ToShortDateString());
            dmlExecute.AddFields("ACTTRN_AMOUNT", fAmount.ToString());
            dmlExecute.AddFields("ACTTRN_REMARKS", sRemarks);
            dmlExecute.AddFields("ACTTRN_SALE_DAY_TRAN_ID", iDayTranID.ToString());
            dmlExecute.AddFields("ACTTRN_RECORD_TYPE", sRecordType);

            dmlExecute.ExecuteInsert("ACTTRN_TRANS", _conn, _conTran);
        }

        private Int64 GetAccountTranNo(int iStoreID)
        {
            Int64 iVoucherNo = 0;
            _sql = "SELECT MAX(ACTTRN_ID) FROM ACTTRN_TRANS WHERE ACTTRN_STORE_ID = " + iStoreID;
            SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iVoucherNo = 1;

            return iVoucherNo;
        }

        public bool UpdateOpeningBalance(AccountModel obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                int iTranID = 0;
                DateTime dDate = DateTime.Now;
                SqlCommand cmd;
                SqlDataReader dr;

                _sql = @"SELECT        min(ACTTRN_DATE)
                            FROM            ACTTRN_TRANS WHERE 
                            (ACTTRN_STORE_ID = " + obj.StoreID + ") AND (ACTTRN_ACTLED_ID = " + obj.LedgerID + ")";
                cmd = new SqlCommand(_sql, _conn, _conTran);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr[0].ToString().Length > 0)
                    {
                        dDate = Convert.ToDateTime(dr[0]);
                        dDate = dDate.AddDays(-1);
                    }
                    else
                        dDate = DateTime.Now;
                }
                dr.Close();

                _sql = @"SELECT ACTTRN_ID FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'OP') AND 
                            (ACTTRN_STORE_ID = " + obj.StoreID + ") AND (ACTTRN_ACTLED_ID = " + obj.LedgerID + ")";
                cmd = new SqlCommand(_sql, _conn, _conTran);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    iTranID = Convert.ToInt16(dr["ACTTRN_ID"]);
                }
                dr.Close();

                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'OP') AND 
                            (ACTTRN_STORE_ID = " + obj.StoreID + ") AND (ACTTRN_ID = " + iTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                //Entry: Cash A/c Dr To LOTTERY SALE 

                // Adding for Cash Entry
                bAccountSameVoucher = false;
                if (obj.OpeningBalanceType == "DR")
                {
                    UpdateAccountEntry(obj.StoreID, "DR", obj.LedgerID, dDate, obj.OpeningBalance, "Opening Balance", 0, "OP");
                    UpdateAccountEntry(obj.StoreID, "CR", 14, dDate, obj.OpeningBalance, obj.LedgerName, 0, "OP");
                }
                else if (obj.OpeningBalanceType == "CR")
                {
                    UpdateAccountEntry(obj.StoreID, "DR", 14, dDate, obj.OpeningBalance, obj.LedgerName, 0, "OP");
                    UpdateAccountEntry(obj.StoreID, "CR", obj.LedgerID, dDate, obj.OpeningBalance, "Opening Balance", 0, "OP");
                }

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Account already exists");
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
        }

        #region Purchases and Purchase Return
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objPurchaseOrPurchaseReturn"></param>
        /// <param name="PurchaseOrPurchaseReturnStatus">I means Invoice and C Credit Note (Purchase return)</param>
        /// <returns></returns>
        private bool AddPurhaseOrPurchaseReturn(SaleMaster objPurchaseOrPurchaseReturn,char PurchaseOrPurchaseReturnStatus)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Sales Master Fields
                dmlExecute.AddFields("STORE_ID", objPurchaseOrPurchaseReturn.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objPurchaseOrPurchaseReturn.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objPurchaseOrPurchaseReturn.Date.ToString());
                dmlExecute.AddFields("CREATED_BY", objPurchaseOrPurchaseReturn.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objPurchaseOrPurchaseReturn.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objPurchaseOrPurchaseReturn.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objPurchaseOrPurchaseReturn.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Purchases or Purchase Return

                _sql = "DELETE FROM PURCHASE_MASTER WHERE PRC_STORE_ID = " + objPurchaseOrPurchaseReturn.StoreID + " AND PRC_INV_CRD_TYPE = '" + PurchaseOrPurchaseReturnStatus + "'";
                _sql += " AND PRC_SALE_TRAN_ID = " + objPurchaseOrPurchaseReturn.DayTranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                if (PurchaseOrPurchaseReturnStatus.Equals('C'))
                {
                    _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'PR') AND 
                            (ACTTRN_STORE_ID = " + objPurchaseOrPurchaseReturn.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objPurchaseOrPurchaseReturn.DayTranID + ")";
                }
                else if (PurchaseOrPurchaseReturnStatus.Equals('I'))
                {
                    _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'PP') AND 
                            (ACTTRN_STORE_ID = " + objPurchaseOrPurchaseReturn.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objPurchaseOrPurchaseReturn.DayTranID + ")";
                }

                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                int RowID = GetMaxPurchaseTranID(objPurchaseOrPurchaseReturn.StoreID);

                foreach (Purchase objPurTran in objPurchaseOrPurchaseReturn.Purchase)
                {
                    if (objPurTran.InvCrdAmount > 0)
                    {


                        dmlExecute.AddFields("PRC_STORE_ID", objPurchaseOrPurchaseReturn.StoreID.ToString());
                        dmlExecute.AddFields("PRC_SALE_TRAN_ID", objPurchaseOrPurchaseReturn.DayTranID.ToString());
                        dmlExecute.AddFields("PRC_TRAN_ID", RowID.ToString());
                        dmlExecute.AddFields("PRC_SUPPLIER_ID", objPurTran.SupplierNo.ToString());
                        dmlExecute.AddFields("PRC_INV_CRD_TYPE", PurchaseOrPurchaseReturnStatus.ToString());
                        dmlExecute.AddFields("PRC_INV_CRD_DT", objPurTran.InvCrdDate.ToString());
                        dmlExecute.AddFields("PRC_INV_CRD_NO", objPurTran.InvCrdNumber.ToString());
                        dmlExecute.AddFields("PRC_INV_CRD_AMOUNT", objPurTran.InvCrdAmount.ToString());

                        if (objPurTran.DueDate.Year != 1)
                            dmlExecute.AddFields("PRC_DUE_DATE", objPurTran.DueDate.ToString());
                        if (objPurTran.Remarks != null)
                            dmlExecute.AddFields("PRC_INVCRD_REMARKS", objPurTran.Remarks.ToString());

                        dmlExecute.AddFields("PRC_DISP_STAT", "D");

                        dmlExecute.AddFields("CREATED_BY", objPurchaseOrPurchaseReturn.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", objPurchaseOrPurchaseReturn.CreateTimeStamp);
                        dmlExecute.AddFields("MODIFIED_BY", objPurchaseOrPurchaseReturn.ModifiedUserName);
                        dmlExecute.AddFields("MODIFIED_TIMESTAMP", objPurchaseOrPurchaseReturn.ModifiedTimeStamp);
                        dmlExecute.ExecuteInsert("PURCHASE_MASTER", _conn, _conTran);

                        #region Adding to Account Transaction
                        float fPurchase = objPurTran.InvCrdAmount;

                        //Purchase  :
                        //Entry     : Purchase A/c Dr To Supplier A/c 

                        //Purchase Return  :
                        //Entry     : Supplier A/c Dr To Purchase A/c  Reverse entry

                        bAccountSameVoucher = false;

                        if (PurchaseOrPurchaseReturnStatus.Equals('I'))
                        {
                            UpdateAccountEntry(objPurchaseOrPurchaseReturn.StoreID, "DR", 18, objPurchaseOrPurchaseReturn.Date, fPurchase, "Purchase of items", objPurchaseOrPurchaseReturn.DayTranID, "PP");
                            UpdateAccountEntry(objPurchaseOrPurchaseReturn.StoreID, "CR", objPurTran.SupplierNo, objPurchaseOrPurchaseReturn.Date, fPurchase, "Purchase or items", objPurchaseOrPurchaseReturn.DayTranID, "PP");
                        }
                        else if (PurchaseOrPurchaseReturnStatus.Equals('C'))
                        {
                            UpdateAccountEntry(objPurchaseOrPurchaseReturn.StoreID, "DR", objPurTran.SupplierNo, objPurchaseOrPurchaseReturn.Date, fPurchase, "Purchase return of items", objPurchaseOrPurchaseReturn.DayTranID, "PR");
                            UpdateAccountEntry(objPurchaseOrPurchaseReturn.StoreID, "CR", 18, objPurchaseOrPurchaseReturn.Date, fPurchase, "Purchase return of items", objPurchaseOrPurchaseReturn.DayTranID, "PR");
                        }

                        #endregion

                        RowID++;
                    }
                }
                _conTran.Commit();

                #endregion

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }


        private int GetMaxPurchaseTranID(int StoreID)
        {
            _sql = "SELECT MAX(PRC_TRAN_ID) FROM PURCHASE_MASTER WHERE PRC_STORE_ID = " + StoreID ;

            SqlCommand cmd;
            Int32 iVoucherNo = 0;

            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iVoucherNo = 1;

            return iVoucherNo;
        }

        private int GetMaxGasTranID(int StoreID)
        {
            _sql = "SELECT MAX(GR_TRAN_ID) FROM GAS_RECEIPT_MASTER WHERE GR_STORE_ID = " + StoreID;

            SqlCommand cmd;
            Int32 iVoucherNo = 0;

            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iVoucherNo = 1;

            return iVoucherNo;
        }
        #endregion

        #region Cheque Cashing
        private bool AddChequeCashing(SaleMaster objChequeCashing)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Sales Master Fields
                dmlExecute.AddFields("STORE_ID", objChequeCashing.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", objChequeCashing.DayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objChequeCashing.Date.ToString());
                dmlExecute.AddFields("CREATED_BY", objChequeCashing.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objChequeCashing.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", objChequeCashing.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objChequeCashing.ModifiedTimeStamp);

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    dmlExecute.ExecuteInsert("STORE_SALE_MASTER", _conn, _conTran);
                }

                #endregion

                #region Adding Cheque Cashing

                _sql = "DELETE FROM CHEQUE_CASHING_TRAN WHERE CHQCS_STORE_ID = " + objChequeCashing.StoreID;
                _sql += " AND CHQCS_SALE_TRAN_ID = " + objChequeCashing.DayTranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                _sql = @"DELETE FROM            ACTTRN_TRANS
                        WHERE        (ACTTRN_RECORD_TYPE = 'CC') AND 
                        (ACTTRN_STORE_ID = " + objChequeCashing.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + objChequeCashing.DayTranID + ")";

                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                int RowID = GetMaxChequeCashingTranID(objChequeCashing.StoreID);

                foreach (ChequeCashingTran objCCTran in objChequeCashing.ChequeCashingEntries)
                {
                    if (objCCTran.ChequeAmount > 0)
                    {
                        dmlExecute.AddFields("CHQCS_STORE_ID", objChequeCashing.StoreID.ToString());
                        dmlExecute.AddFields("CHQCS_SALE_TRAN_ID", objChequeCashing.DayTranID.ToString());
                        dmlExecute.AddFields("CHQCS_TRAN_ID", RowID.ToString());
                        dmlExecute.AddFields("CHQCS_CHQNO", objCCTran.ChequeNo.ToString());
                        dmlExecute.AddFields("CHQCS_BANK_NAME", objCCTran.BankName.ToString());
                        dmlExecute.AddFields("CHQCS_CHQ_AMOUNT", objCCTran.ChequeAmount.ToString());
                        dmlExecute.AddFields("CHQCS_PAID_AMOUNT", objCCTran.PaidAmount.ToString());
                        dmlExecute.AddFields("CHQCS_COMMISSION", objCCTran.Commission.ToString());
                        dmlExecute.AddFields("CHQCS_REMARKS", objCCTran.Remarks);

                        dmlExecute.ExecuteInsert("CHEQUE_CASHING_TRAN", _conn, _conTran);

                        #region Adding to Account Transaction

                        //Cheque Cashing  :
                        //Entry     : Cheque Cashing A/c Dr To Cash A/c  To Cheque Cashing Commission


                        bAccountSameVoucher = false;

                        UpdateAccountEntry(objChequeCashing.StoreID, "DR", 19, objChequeCashing.Date, objCCTran.ChequeAmount, "Cheque Cashing", objChequeCashing.DayTranID, "CC");
                        UpdateAccountEntry(objChequeCashing.StoreID, "CR", 1, objChequeCashing.Date, objCCTran.PaidAmount, "Cheque Cashing", objChequeCashing.DayTranID, "CC");
                        UpdateAccountEntry(objChequeCashing.StoreID, "CR", 20, objChequeCashing.Date, objCCTran.Commission, "Cheque Cashing", objChequeCashing.DayTranID, "CC");

                        #endregion

                        RowID++;
                    }
                }
                _conTran.Commit();

                #endregion

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        private int GetMaxChequeCashingTranID(int StoreID)
        {
            _sql = "SELECT MAX(CHQCS_TRAN_ID) FROM CHEQUE_CASHING_TRAN WHERE CHQCS_STORE_ID = " + StoreID;

            SqlCommand cmd;
            Int32 iVoucherNo = 0;

            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iVoucherNo = 1;

            return iVoucherNo;
        }

        private bool ChequeCashingDeposit(SaleMaster objChequeCashing)
        {
            bool bResult = false;

            try
            {
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Cheque Updates

                foreach (ChequeCashingTran objCCTran in objChequeCashing.ChequeDeposits)
                {
                    if (objCCTran.ChequeTranID > 0)
                    {
                        dmlExecute.AddFields("CHQCS_STORE_ID", objChequeCashing.StoreID.ToString());
                        dmlExecute.AddFields("CHQCS_TRAN_ID", objCCTran.ChequeTranID.ToString());
                        dmlExecute.AddFields("CHQCS_CHQNO", objCCTran.ChequeNo.ToString());
                        dmlExecute.AddFields("CHQCS_DEPOSIT_STATUS", "Y");
                        dmlExecute.AddFields("CHQCS_DEPOSIT_DATE", objChequeCashing.Date.ToString());

                        string[] KeyFields = { "CHQCS_STORE_ID", "CHQCS_TRAN_ID","CHQCS_CHQNO" };
                        dmlExecute.ExecuteUpdate("CHEQUE_CASHING_TRAN", _conn, KeyFields, _conTran);
                    }
                }
                _conTran.Commit();

                #endregion

                return bResult;
            }
            catch (Exception ex)
            {
                _conTran.Rollback();
                throw ex;
            }
        }

        #endregion


        private Int32 GetSlNoFromDailyReport(int iStoreID, int iBoxNo)
        {
            Int32 iSlNo = 0;
            _sql = "SELECT MAX(REPL_SLNO) FROM REPORT_LOTTERY_DAILY WHERE REPL_STORE_ID = " + iStoreID + " AND REPL_BOX_ID = " + iBoxNo;
            SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iSlNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iSlNo = 1;

            return iSlNo;
        }

    }
}
