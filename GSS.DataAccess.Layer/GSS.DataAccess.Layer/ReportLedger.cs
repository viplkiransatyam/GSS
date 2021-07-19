using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class ReportLedger
    {
        SqlConnection _conn;
        string _sql;
        
        public List<ReportLedgerModel> GetLedger (int iStoreID, int iLedgerID, DateTime dFrom, DateTime dTo)
        {
            List<ReportLedgerModel> objLedgerList = new List<ReportLedgerModel>();
            ReportLedgerModel objLedger;
            SqlCommand cmd;
            SqlDataReader dr;
            float fDebit = 0;
            float fCredit = 0;
            float fBalance = 0;

            try
            {
                 _conn = new SqlConnection(DMLExecute.con);
                 _conn.Open();

                #region Calculate Opening Balance
                _sql = @"SELECT ACTTRN_DATE,TAB1.ACTTRN_ID, PARTICULARS, DEBIT, CREDIT, ACTTRN_REMARKS, TAB1.ACTTRN_SLNO FROM VW_REPORT_ACCOUNT_LEDGER TAB1
                            INNER JOIN 
		                            (
			                            SELECT ACTTRN_ID , MIN(ACTTRN_SLNO) AS ACTTRN_SLNO FROM VW_REPORT_ACCOUNT_LEDGER
				                            WHERE ACTTRN_STORE_ID = MAPSTOREID AND ACTTRN_ACTLED_ID = MAPLEDGERID
				                            GROUP BY ACTTRN_ID
		                            ) TAB2
                            ON TAB1.ACTTRN_ID = TAB2.ACTTRN_ID AND TAB1.ACTTRN_SLNO = TAB2.ACTTRN_SLNO
                            WHERE ACTTRN_STORE_ID = MAPSTOREID AND ACTTRN_ACTLED_ID = MAPLEDGERID
                                    AND (DEBIT > 0  OR CREDIT > 0) AND ACTTRN_DATE <= 'TODATE' 
                            ORDER BY ACTTRN_DATE,TAB1.ACTTRN_ID";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("MAPLEDGERID", iLedgerID.ToString());
                _sql = _sql.Replace("TODATE", dTo.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                bool IsOpeningBalance = false;

                while(dr.Read())
                {
                    if (Convert.ToDateTime(dr["ACTTRN_DATE"]) < dFrom)
                    {
                        fDebit += Convert.ToSingle(dr["DEBIT"]);
                        fCredit += Convert.ToSingle(dr["CREDIT"]);
                    }
                    else
                    {
                        if (IsOpeningBalance == false)
                        {
                            #region Adding for Opening Balance
                            objLedger = new ReportLedgerModel();
                            objLedger.Date = dFrom;
                            objLedger.LedgerName = "Opening Balance";
                            if (fDebit > fCredit)
                            {
                                if (fDebit - fCredit > 0)
                                    objLedger.Debit = Convert.ToString(fDebit - fCredit);
                                fBalance = fDebit - fCredit;
                            }
                            else
                            {
                                if (fCredit - fDebit > 0)
                                    objLedger.Credit = Convert.ToString(fCredit - fDebit);
                                fBalance = fDebit - fCredit;
                            }
                            
                            objLedger.Balance = fBalance;
                            objLedger.Balance = objLedger.Balance ;

                            if (objLedger.Balance > 0)
                                objLedger.BalanceType = " (Dr)";
                            else
                            {
                                objLedger.BalanceType = " (Cr)";
                                objLedger.Balance = objLedger.Balance * -1;
                            }
                            
                            objLedgerList.Add(objLedger);
                            IsOpeningBalance = true;
                            #endregion
                        }

                        #region Adding for Ledger Records
                        objLedger = new ReportLedgerModel();
                        objLedger.Date = Convert.ToDateTime(dr["ACTTRN_DATE"]);
                        objLedger.LedgerName = dr["PARTICULARS"].ToString();
                        fDebit = Convert.ToSingle(dr["DEBIT"]);
                        fCredit = Convert.ToSingle(dr["CREDIT"]);

                        fBalance = fBalance + fDebit - fCredit;
                        if (fDebit > 0) 
                            objLedger.Debit = fDebit.ToString();
                        if (fCredit > 0)
                            objLedger.Credit = fCredit.ToString();

                        objLedger.Balance = fBalance;

                        if (objLedger.Balance > 0)
                            objLedger.BalanceType = " (Dr)";
                        else
                        {
                            objLedger.BalanceType = " (Cr)";
                            objLedger.Balance = objLedger.Balance * -1;
                        }
                        objLedger.Remarks = dr["ACTTRN_REMARKS"].ToString();

                        objLedgerList.Add(objLedger);
                        #endregion
                    }
                }

                #region If no records found between start and end dates, display opening balance

                if (IsOpeningBalance == false)
                {
                    objLedger = new ReportLedgerModel();
                    objLedger.Date = dFrom;
                    objLedger.LedgerName = "Opening Balance";
                    if (fDebit > fCredit)
                    {
                        if (fDebit - fCredit > 0)
                            objLedger.Debit = Convert.ToString(fDebit - fCredit);
                        fBalance = fDebit - fCredit;
                    }
                    else
                    {
                        if (fCredit - fDebit > 0)
                            objLedger.Credit = Convert.ToString(fCredit - fDebit);
                        fBalance = fDebit - fCredit;
                    }

                    objLedger.Balance = fBalance;
                    objLedger.Balance = objLedger.Balance;

                    if (objLedger.Balance > 0)
                        objLedger.BalanceType = " (Dr)";
                    else
                    {
                        objLedger.BalanceType = " (Cr)";
                        objLedger.Balance = objLedger.Balance * -1;
                    }
                    objLedgerList.Add(objLedger);
                }

                #endregion

                dr.Close();



                #endregion
                return objLedgerList;
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

    }
}
