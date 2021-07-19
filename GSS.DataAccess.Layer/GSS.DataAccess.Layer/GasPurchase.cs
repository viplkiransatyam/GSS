using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class GasPurchase
    {
        SqlConnection _conn;
        SqlTransaction _conTran;
        string _sql;

        public GasPurchaseModel GasInvReceipt(int iStoreID, string sBillOfLading)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            GasPurchaseModel gasPurchase = new GasPurchaseModel();
            List<GasInvoiceItems> gasInvItems = new List<GasInvoiceItems>();
            GasInvoiceItems objgasInvItems;

            List<GasInvoiceTax> gasTax = new List<GasInvoiceTax>();
            GasInvoiceTax objgasTax;

            List<GasDefaultTax> gasDefaultTax = new List<GasDefaultTax>();
            GasDefaultTax objgasDefaultTax;

            #region Adding Gas Inventory
            _sql = @"SELECT    GAS_RECEIPT_MASTER.GR_TRAN_ID, GAS_RECEIPT_MASTER.GR_BOL, GAS_RECEIPT_MASTER.GR_INV_RECT_STATUS, GAS_RECEIPT_MASTER.GR_DATE,GAS_RECEIPT_MASTER.GR_SHIFT_CODE, 
                         GAS_RECEIPT_MASTER.GR_DUE_DATE, GAS_RECEIPT_MASTER.GR_INV_NO, GAS_RECEIPT_MASTER.GR_INV_DATE, GAS_RECEIPT_MASTER.GR_INV_AMOUNT, 
                        GAS_RECEIPT_DELIVERY.GRV_SLNO, GROUP_GASTYPE_MASTER.GASTYPE_ID, 
                         GROUP_GASTYPE_MASTER.GASTYPE_NAME, GAS_RECEIPT_DELIVERY.GRV_GROSS_GALLONS, GAS_RECEIPT_DELIVERY.GRV_NET_GALLONS, 
                         GAS_RECEIPT_DELIVERY.GRV_INV_GROSS_GALLONS, GAS_RECEIPT_DELIVERY.GRV_INV_NET_GALLONS, GAS_RECEIPT_DELIVERY.GRV_INV_PRICE, 
                         GAS_RECEIPT_DELIVERY.GRP_INV_AMOUNT
                        FROM            GAS_RECEIPT_MASTER INNER JOIN
                                                 GAS_RECEIPT_DELIVERY ON GAS_RECEIPT_MASTER.GR_STORE_ID = GAS_RECEIPT_DELIVERY.GRV_STORE_ID AND 
                                                 GAS_RECEIPT_MASTER.GR_TRAN_ID = GAS_RECEIPT_DELIVERY.GRV_TRAN_ID INNER JOIN
                                                 MAPPING_STORE_GAS ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                                 GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID INNER JOIN
                                                 STORE_MASTER ON MAPPING_STORE_GAS.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID AND 
                                                 STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID
                        WHERE        (GAS_RECEIPT_MASTER.GR_STORE_ID = " + iStoreID + ") AND (GAS_RECEIPT_MASTER.GR_BOL = N'" + sBillOfLading + "')";
            _sql += " ORDER BY GAS_RECEIPT_DELIVERY.GRV_SLNO";

            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            bool IsFirstRow = false;
            while (dr.Read())
            {
                if (IsFirstRow == false)
                {
                    gasPurchase.StoreID = iStoreID;
                    gasPurchase.TransactionID = Convert.ToInt16(dr["GR_TRAN_ID"].ToString()); ;

                    if (dr["GR_INV_RECT_STATUS"].ToString().Equals("N"))
                        gasPurchase.GasInvoiceReceiptType = "Receipt";
                    else
                        gasPurchase.GasInvoiceReceiptType = "Invoice";
                    
                    gasPurchase.ShiftCode = Convert.ToInt16(dr["GR_SHIFT_CODE"].ToString());
                    gasPurchase.ReceiptDate = Convert.ToDateTime(dr["GR_DATE"]);
                    gasPurchase.InvNo = dr["GR_INV_NO"].ToString();
                    if (dr["GR_INV_DATE"].ToString().Length > 0)
                        gasPurchase.InvDate = Convert.ToDateTime(dr["GR_INV_DATE"]);

                    if (dr["GR_INV_AMOUNT"].ToString().Length > 0)
                        gasPurchase.InvAmount = Convert.ToSingle(dr["GR_INV_AMOUNT"]);

                    if (dr["GR_BOL"].ToString().Length > 0)
                        gasPurchase.BillOfLading = dr["GR_BOL"].ToString();

                    if (dr["GR_DUE_DATE"].ToString().Length > 0) 
                        gasPurchase.DueDate = Convert.ToDateTime(dr["GR_DUE_DATE"]);
                    IsFirstRow = true;
                }

                objgasInvItems = new GasInvoiceItems();
                objgasInvItems.SlNo = Convert.ToInt16(dr["GRV_SLNO"]);
                objgasInvItems.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"].ToString());
                objgasInvItems.GasTypeName = dr["GASTYPE_NAME"].ToString();
                objgasInvItems.GrossGallons = Convert.ToSingle(dr["GRV_GROSS_GALLONS"].ToString());
                objgasInvItems.NetGallons = Convert.ToSingle(dr["GRV_NET_GALLONS"].ToString());
                objgasInvItems.GrossInvGallons = Convert.ToSingle(dr["GRV_INV_GROSS_GALLONS"].ToString());
                objgasInvItems.NetInvGallons = Convert.ToSingle(dr["GRV_INV_NET_GALLONS"].ToString());
                objgasInvItems.Price = Convert.ToSingle(dr["GRV_INV_PRICE"].ToString());
                objgasInvItems.Amount = Convert.ToSingle(dr["GRP_INV_AMOUNT"].ToString());
                gasInvItems.Add(objgasInvItems);
            }
            dr.Close();
            gasPurchase.GasInventory = gasInvItems;
            #endregion

            #region Return Gas Tax

            _sql = @"SELECT        GLOBAL_GAS_TAX_MASTER.TAX_ID, GLOBAL_GAS_TAX_MASTER.TAX_DESCRIPTION, GAS_RECEIPT_TAX.GRT_TAX_AMOUNT
                        FROM            GAS_RECEIPT_TAX INNER JOIN
                                                 GAS_RECEIPT_MASTER ON GAS_RECEIPT_TAX.GRT_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND 
                                                 GAS_RECEIPT_TAX.GRT_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID INNER JOIN
                                                 GLOBAL_GAS_TAX_MASTER ON GAS_RECEIPT_TAX.GRT_TAX_ID = GLOBAL_GAS_TAX_MASTER.TAX_ID
                    WHERE        (GAS_RECEIPT_MASTER.GR_STORE_ID = "+ iStoreID + ") AND (GAS_RECEIPT_MASTER.GR_BOL = N'" + sBillOfLading + "')";


            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                objgasTax = new GasInvoiceTax();
                objgasTax.TaxId = Convert.ToInt16(dr["TAX_ID"]);
                objgasTax.TaxName = dr["TAX_DESCRIPTION"].ToString();
                objgasTax.TaxAmount = Convert.ToSingle(dr["GRT_TAX_AMOUNT"]);
                gasTax.Add(objgasTax);
            }
            dr.Close();
            gasPurchase.GasTax = gasTax;
            #endregion

            #region Return Tax
            _sql = @"SELECT   GLOBAL_GAS_TAX_MASTER.TAX_ID, GLOBAL_GAS_TAX_MASTER.TAX_DESCRIPTION
                        FROM            STORE_MASTER INNER JOIN
                                                 GROUP_GAS_TAX ON STORE_MASTER.STORE_GROUP_ID = GROUP_GAS_TAX.GASTX_GROUP_ID INNER JOIN
                                                 GLOBAL_GAS_TAX_MASTER ON GROUP_GAS_TAX.GASTX_TAX_ID = GLOBAL_GAS_TAX_MASTER.TAX_ID
                        WHERE        (STORE_MASTER.STORE_ID = " + iStoreID + ")";
           _sql += " ORDER BY GROUP_GAS_TAX.GASTX_SLNO";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objgasDefaultTax = new GasDefaultTax();
                objgasDefaultTax.TaxId = Convert.ToInt16(dr["TAX_ID"]);
                objgasDefaultTax.TaxName = dr["TAX_DESCRIPTION"].ToString();
                gasDefaultTax.Add(objgasDefaultTax);
            }
            dr.Close();
            gasPurchase.GasDefaultTax = gasDefaultTax;
            #endregion

            return gasPurchase;
        }

        public bool AddOrUpdateGasInvoice(GasPurchaseModel objGasPurchase)
        {
            bool bResult = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Gas Invoice Update

                dmlExecute.AddFields("GR_STORE_ID", objGasPurchase.StoreID.ToString());
                dmlExecute.AddFields("GR_TRAN_ID", objGasPurchase.TransactionID.ToString());
                dmlExecute.AddFields("GR_INV_RECT_STATUS", "I");
                dmlExecute.AddFields("GR_INV_NO", objGasPurchase.InvNo);
                dmlExecute.AddFields("GR_INV_DATE", objGasPurchase.InvDate.ToString());
                dmlExecute.AddFields("GR_INV_AMOUNT", objGasPurchase.InvAmount.ToString());
                dmlExecute.AddFields("MODIFIED_BY", objGasPurchase.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasPurchase.ModifiedTimeStamp);
                dmlExecute.AddFields("CREATED_BY", objGasPurchase.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objGasPurchase.CreateTimeStamp);


                string[] KeyFields = { "GR_STORE_ID", "GR_TRAN_ID" };
                dmlExecute.ExecuteUpdate("GAS_RECEIPT_MASTER", _conn, KeyFields, _conTran);

                string[] KeyFieldsInventory = { "GRV_STORE_ID", "GRV_TRAN_ID", "GRV_SLNO", "GRV_GAS_TYPE_ID" };

                foreach (GasInvoiceItems obj in objGasPurchase.GasInventory)
                {
                    dmlExecute.AddFields("GRV_STORE_ID", objGasPurchase.StoreID.ToString());
                    dmlExecute.AddFields("GRV_TRAN_ID", objGasPurchase.TransactionID.ToString());
                    dmlExecute.AddFields("GRV_SLNO", obj.SlNo.ToString());
                    dmlExecute.AddFields("GRV_GAS_TYPE_ID", obj.GasTypeID.ToString());
                    dmlExecute.AddFields("GRV_INV_GROSS_GALLONS", obj.GrossInvGallons.ToString());
                    dmlExecute.AddFields("GRV_INV_NET_GALLONS", obj.NetInvGallons.ToString());
                    dmlExecute.AddFields("GRV_INV_PRICE", obj.Price.ToString());
                    dmlExecute.AddFields("GRP_INV_AMOUNT", obj.Amount.ToString());

                    dmlExecute.ExecuteUpdate("GAS_RECEIPT_DELIVERY", _conn, KeyFieldsInventory, _conTran);
                }

                _sql = "DELETE FROM GAS_RECEIPT_TAX WHERE GRT_STORE_ID = " + objGasPurchase.StoreID + " AND GRT_TRAN_ID = " + objGasPurchase.TransactionID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                foreach (GasInvoiceTax obj in objGasPurchase.GasTax)
                {
                    dmlExecute.AddFields("GRT_STORE_ID", objGasPurchase.StoreID.ToString());
                    dmlExecute.AddFields("GRT_TRAN_ID", objGasPurchase.TransactionID.ToString());
                    dmlExecute.AddFields("GRT_TAX_ID", obj.TaxId.ToString());
                    dmlExecute.AddFields("GRT_TAX_AMOUNT", obj.TaxAmount.ToString());

                    dmlExecute.ExecuteInsert("GAS_RECEIPT_TAX", _conn, _conTran);
                }

                dmlExecute.AddFields("RECON_STORE_ID", objGasPurchase.StoreID.ToString());
                dmlExecute.AddFields("RECON_TYPE_ID", "3");
                dmlExecute.AddFields("RECON_REF_NO", objGasPurchase.InvNo.ToString());
                dmlExecute.AddFields("RECON_REF_DATE", objGasPurchase.InvDate.ToString());
                dmlExecute.AddFields("RECON_AMOUNT", objGasPurchase.InvAmount.ToString());

                dmlExecute.ExecuteInsert("STORE_RECONCILLATION_ENTRIES", _conn, _conTran);

                _conTran.Commit();

                #endregion
                bResult = true;
                return bResult;
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

        #region Gas Dealer Statement
        public List<TranType> GetTranTypes()
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<TranType> TranTypes = new List<TranType>();
            TranType TranType;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                #region Return Transactions
                _sql = @"SELECT   GTP_TRAN_ID, GTP_TRAN_DESC
                        FROM            GLOBAL_GAS_TRAN_TYPES
                        ORDER BY GTP_TRAN_ID";

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TranType = new TranType();
                    TranType.TransactionTypeID = Convert.ToInt16(dr["GTP_TRAN_ID"]);
                    TranType.TransactionTypeName = dr["GTP_TRAN_DESC"].ToString();
                    TranTypes.Add(TranType);
                }
                dr.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
            return TranTypes;
        }

        public bool AddOrUpdateGasDealerStatement(GasDealerTrans objGasStatement)
        {
            bool bResult = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Gas Dealer Statement

                _sql = "DELETE FROM GAS_DEALER_TRAN_TRANS WHERE GDTT_STORE_ID = " + objGasStatement.StoreID + " AND GDTT_TRAN_ID = " + objGasStatement.TranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                _sql = "DELETE FROM GAS_DEALER_TRAN_MASTER WHERE GDT_STORE_ID = " + objGasStatement.StoreID + " AND GDT_TRAN_ID = " + objGasStatement.TranID;
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                objGasStatement.TranID = GetMaxGasTranID(objGasStatement.StoreID);

                dmlExecute.AddFields("GDT_STORE_ID", objGasStatement.StoreID.ToString());
                dmlExecute.AddFields("GDT_TRAN_ID", objGasStatement.TranID.ToString());
                dmlExecute.AddFields("GDT_REF_NO", objGasStatement.ReferenceNo);
                dmlExecute.AddFields("GDT_REF_DATE", objGasStatement.ReferenceDate.ToString());
                dmlExecute.AddFields("MODIFIED_BY", objGasStatement.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", objGasStatement.ModifiedTimeStamp);
                dmlExecute.AddFields("CREATED_BY", objGasStatement.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", objGasStatement.CreateTimeStamp);
                dmlExecute.ExecuteInsert("GAS_DEALER_TRAN_MASTER", _conn, _conTran);

                int i = 1;
                foreach (GasDealerTransRows obj in objGasStatement.GasDealerTransRows)
                {
                    dmlExecute.AddFields("GDTT_STORE_ID", objGasStatement.StoreID.ToString());
                    dmlExecute.AddFields("GDTT_TRAN_ID", objGasStatement.TranID.ToString());
                    dmlExecute.AddFields("GDTT_SL_NO", i.ToString());
                    dmlExecute.AddFields("GDTT_REF_TRAN_ID", obj.ReferenceTranNo.ToString());
                    dmlExecute.AddFields("GDTT_REF_TRAN_DATE", obj.ReferenceTranDate.ToString());
                    dmlExecute.AddFields("GDTT_TRAN_TYPE", obj.TranTypeIndicator.ToString());
                    dmlExecute.AddFields("GDTT_TRAN_AMOUNT", obj.TranAmount.ToString());
                    i = i + 1 ;

                    dmlExecute.ExecuteInsert("GAS_DEALER_TRAN_TRANS", _conn,  _conTran);
                }

                if (objGasStatement.ApplyFee == Convert.ToChar("Y"))
                {
                    foreach (GasDealerTransRows obj in objGasStatement.GasDealerTransRows)
                    {
                        if (obj.TranTypeIndicator == 2 || obj.TranTypeIndicator == 4)
                        {
                            dmlExecute.AddFields("RECON_STORE_ID", objGasStatement.StoreID.ToString());
                            dmlExecute.AddFields("RECON_TYPE_ID", obj.TranTypeIndicator.ToString());
                            dmlExecute.AddFields("RECON_REF_NO", obj.ReferenceTranNo.ToString());
                            dmlExecute.AddFields("RECON_REF_DATE", obj.ReferenceTranDate.ToString());
                            dmlExecute.AddFields("RECON_AMOUNT", obj.TranAmount.ToString());

                            dmlExecute.ExecuteInsert("STORE_RECONCILLATION_ENTRIES", _conn, _conTran);
                        }
                    }
                }

                _conTran.Commit();

                #endregion

                bResult = true;
                return bResult;
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

        public float GetTranAmount(SearchTranModel objTran)
        {
            float fTranAmount = 0;
            string sSQL = string.Empty;
            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            if (objTran.TypeID == 3)
            {
                sSQL = @"SELECT  GR_INV_AMOUNT AS AMOUNT
                            FROM            GAS_RECEIPT_MASTER
                            WHERE        (GR_STORE_ID = PARMSTOREID) AND (GR_INV_RECT_STATUS = 'I') AND (GR_BOL = 'PARMINVNO')";
                sSQL = sSQL.Replace("PARMSTOREID", objTran.StoreID.ToString());
                sSQL = sSQL.Replace("PARMINVNO", objTran.RefNumber.ToString());
            }
            else if (objTran.TypeID == 1)
            {
                sSQL = @"SELECT  SUM(STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT) AS AMOUNT
                            FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                        STORE_SALE_MASTER ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                        STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                                        MAPPING_STORE_CARD ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = MAPPING_STORE_CARD.STORE_ID AND 
                                                        STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID = MAPPING_STORE_CARD.CARD_TYPE_ID
                            WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE') AND 
                                                        (MAPPING_STORE_CARD.CARD_CREDIT_TYPE = 'G')";
                sSQL = sSQL.Replace("PARMSTOREID", objTran.StoreID.ToString());
                sSQL = sSQL.Replace("PARMDATE", objTran.DateOfTransaction.ToString());
            }

            SqlCommand cmd;
            SqlDataReader dr;

            cmd = new SqlCommand(sSQL, _conn);
            dr = cmd.ExecuteReader();
            if (dr.Read())
                if (dr["AMOUNT"].ToString().Length > 0)
                    fTranAmount = Convert.ToSingle(dr["AMOUNT"]);

            dr.Close();
            return fTranAmount;

        }

        private int GetMaxGasTranID(int StoreID)
        {
            _sql = "SELECT MAX(GDT_TRAN_ID) FROM GAS_DEALER_TRAN_MASTER WHERE GDT_STORE_ID = " + StoreID;
            
            SqlCommand cmd;
            Int32 iVoucherNo = 0;

            cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iVoucherNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iVoucherNo = 1;

            return iVoucherNo;
        }

        public GasDealerTrans GetGasDealerStatement(int iStoreID, string sDealerStatementRef)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            GasDealerTrans gasStatement = new GasDealerTrans();
            List<GasDealerTransRows> gasTrans = new List<GasDealerTransRows>();
            GasDealerTransRows gasTran;

            #region Adding Gas Inventory
            _sql = @"SELECT   GAS_DEALER_TRAN_MASTER.GDT_TRAN_ID, GAS_DEALER_TRAN_MASTER.GDT_REF_NO, GAS_DEALER_TRAN_MASTER.GDT_REF_DATE, 
                         GAS_DEALER_TRAN_TRANS.GDTT_SL_NO, GAS_DEALER_TRAN_TRANS.GDTT_REF_TRAN_ID, GAS_DEALER_TRAN_TRANS.GDTT_REF_TRAN_DATE, 
                         GAS_DEALER_TRAN_TRANS.GDTT_TRAN_TYPE, GAS_DEALER_TRAN_TRANS.GDTT_TRAN_AMOUNT, GAS_DEALER_TRAN_MASTER.CREATED_BY, 
                         GAS_DEALER_TRAN_MASTER.CREATED_TIMESTAMP, GAS_DEALER_TRAN_MASTER.MODIFIED_BY, GAS_DEALER_TRAN_MASTER.MODIFIED_TIMESTAMP, 
                         GLOBAL_GAS_TRAN_TYPES.GTP_TRAN_DESC
                        FROM            GAS_DEALER_TRAN_MASTER INNER JOIN
                                                 GAS_DEALER_TRAN_TRANS ON GAS_DEALER_TRAN_MASTER.GDT_STORE_ID = GAS_DEALER_TRAN_TRANS.GDTT_STORE_ID AND 
                                                 GAS_DEALER_TRAN_MASTER.GDT_TRAN_ID = GAS_DEALER_TRAN_TRANS.GDTT_TRAN_ID INNER JOIN
                                                 GLOBAL_GAS_TRAN_TYPES ON GAS_DEALER_TRAN_TRANS.GDTT_TRAN_TYPE  = GLOBAL_GAS_TRAN_TYPES.GTP_TRAN_ID
                        WHERE        (GAS_DEALER_TRAN_MASTER.GDT_STORE_ID = " + iStoreID + ") AND (GAS_DEALER_TRAN_MASTER.GDT_REF_NO = '" + sDealerStatementRef + "')";
            _sql += " ORDER BY GAS_DEALER_TRAN_TRANS.GDTT_SL_NO ";

            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            bool IsFirstRow = false;
            while (dr.Read())
            {
                if (IsFirstRow == false)
                {
                    gasStatement.StoreID = iStoreID;
                    gasStatement.TranID = Convert.ToInt16(dr["GDT_TRAN_ID"].ToString()); ;

                    gasStatement.CreatedUserName = dr["CREATED_BY"].ToString();
                    gasStatement.ModifiedUserName = dr["MODIFIED_BY"].ToString();

                    if (dr["GDT_REF_NO"].ToString().Length > 0)
                        gasStatement.ReferenceNo = dr["GDT_REF_NO"].ToString();

                    if (dr["GDT_REF_DATE"].ToString().Length > 0)
                        gasStatement.ReferenceDate = Convert.ToDateTime(dr["GDT_REF_DATE"]);

                    IsFirstRow = true;
                }

                gasTran = new GasDealerTransRows();
                gasTran.SlNo = Convert.ToInt16(dr["GDTT_SL_NO"]);

                if (dr["GDTT_REF_TRAN_ID"].ToString().Length > 0)
                    gasTran.ReferenceTranNo = dr["GDTT_REF_TRAN_ID"].ToString();

                if (dr["GDTT_REF_TRAN_DATE"].ToString().Length > 0)
                    gasTran.ReferenceTranDate = Convert.ToDateTime(dr["GDTT_REF_TRAN_DATE"].ToString());

                gasTran.TranTypeIndicator = Convert.ToInt16(dr["GDTT_TRAN_TYPE"].ToString());
                gasTran.TranDescription = dr["GTP_TRAN_DESC"].ToString();
                gasTran.TranAmount = Convert.ToSingle(dr["GDTT_TRAN_AMOUNT"].ToString());
                gasTrans.Add(gasTran);
            }
            dr.Close();
            gasStatement.GasDealerTransRows = gasTrans;
            #endregion


            return gasStatement;
        }

        public List<PurchaseRegisterModel> PurchaseRegister(int iStoreID, DateTime dFromDate, DateTime dToDate)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<PurchaseRegisterModel> purchaseRegister = new List<PurchaseRegisterModel>();
            PurchaseRegisterModel obj;

            #region Adding Purchases
            _sql = @"SELECT  GR_TRAN_ID, GR_DUE_DATE, GR_BOL, GR_INV_DATE, GR_INV_AMOUNT
                        FROM            GAS_RECEIPT_MASTER
                        WHERE        (GR_STORE_ID = PARMSTOREID) AND (GR_INV_RECT_STATUS = 'I') AND (GR_INV_DATE >= 'PARMFROMDATE' AND 
                                                 GR_INV_DATE <= 'PARMTODATE')
                        ORDER BY GR_INV_DATE";

            _sql = _sql.Replace("PARMSTOREID",iStoreID.ToString());
            _sql = _sql.Replace("PARMFROMDATE",dFromDate.ToString());
            _sql = _sql.Replace("PARMTODATE",dToDate.ToString());

            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                obj = new PurchaseRegisterModel();
                obj.TranId = Convert.ToInt16(dr["GR_TRAN_ID"].ToString());
                obj.InvNo = dr["GR_BOL"].ToString();
                obj.InvDate = Convert.ToDateTime(dr["GR_INV_DATE"]);
                obj.DueDate = Convert.ToDateTime(dr["GR_DUE_DATE"]);
                obj.Amount = Convert.ToSingle(dr["GR_INV_AMOUNT"]);

                purchaseRegister.Add(obj);
            }
            dr.Close();
            #endregion


            return purchaseRegister;
        }

        public List<InwardModel> StockInward(int iStoreID, DateTime dFromDate, DateTime dToDate)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<InwardModel> stockInward = new List<InwardModel>();
            InwardModel obj;

            #region Adding Stock Inward

            _sql = @"SELECT  GAS_RECEIPT_MASTER.GR_DATE, GAS_RECEIPT_MASTER.GR_BOL, GROUP_GASTYPE_MASTER.GASTYPE_NAME, 
                         GAS_RECEIPT_DELIVERY.GRV_GROSS_GALLONS, GAS_RECEIPT_DELIVERY.GRV_NET_GALLONS, GAS_RECEIPT_DELIVERY.GRV_INV_NET_GALLONS, 
                         GAS_RECEIPT_DELIVERY.GRV_INV_GROSS_GALLONS
                    FROM            STORE_MASTER INNER JOIN
                                             GROUP_GASTYPE_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID INNER JOIN
                                             GAS_RECEIPT_MASTER INNER JOIN
                                             GAS_RECEIPT_DELIVERY ON GAS_RECEIPT_MASTER.GR_STORE_ID = GAS_RECEIPT_DELIVERY.GRV_STORE_ID AND 
                                             GAS_RECEIPT_MASTER.GR_TRAN_ID = GAS_RECEIPT_DELIVERY.GRV_TRAN_ID ON STORE_MASTER.STORE_ID = GAS_RECEIPT_DELIVERY.GRV_STORE_ID AND 
                                             GROUP_GASTYPE_MASTER.GASTYPE_ID = GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID
                    WHERE        (GAS_RECEIPT_MASTER.GR_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_MASTER.GR_DATE >= 'PARMFROMDATE' AND 
                                             GAS_RECEIPT_MASTER.GR_DATE <= 'PARMTODATE')
                    ORDER BY GAS_RECEIPT_MASTER.GR_DATE, GAS_RECEIPT_DELIVERY.GRV_SLNO";

            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMFROMDATE", dFromDate.ToString());
            _sql = _sql.Replace("PARMTODATE", dToDate.ToString());

            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                obj = new InwardModel();
                obj.BOLNo = dr["GR_BOL"].ToString();
                obj.RecDate = Convert.ToDateTime(dr["GR_DATE"]);
                obj.GasOilName = dr["GASTYPE_NAME"].ToString();
                obj.InwardGrossQty = Convert.ToSingle(dr["GRV_GROSS_GALLONS"]);
                obj.InwardNetQty = Convert.ToSingle(dr["GRV_NET_GALLONS"]);
                obj.InvoiceGrossQty = Convert.ToSingle(dr["GRV_INV_GROSS_GALLONS"]);
                obj.InvoiceNetQty = Convert.ToSingle(dr["GRV_INV_NET_GALLONS"]);

                stockInward.Add(obj);
            }
            dr.Close();
            #endregion


            return stockInward;
        }

        #endregion
    }
}
