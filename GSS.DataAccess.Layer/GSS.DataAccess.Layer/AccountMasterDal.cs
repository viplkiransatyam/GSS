using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class AccountMasterDal  : IDal<AccountModel>
    {
        SqlConnection _conn;
        SqlTransaction _conTran;
        string _sql;

        public bool AddRecord(AccountModel obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add Account Master

                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("ACTLED_ID", obj.LedgerID.ToString());
                dmlExecute.AddFields("ACTLED_NAME", obj.LedgerName.ToString());
                dmlExecute.AddFields("ACTLED_ACTGRP_ID", obj.GroupID.ToString());
                if (obj.DisplaySide != null)
                    dmlExecute.AddFields("ACTLED_DISP_SIDE", obj.DisplaySide.ToString().Remove(obj.DisplaySide.Length - 1));
                dmlExecute.AddFields("ACTLED_REMARKS", obj.Remarks.ToString());
                dmlExecute.AddFields("ACTLED_ACTIVE", obj.ActiveStatus.ToString());
                dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                dmlExecute.AddFields("ACTLED_DISP_STATUS", "Y");
                dmlExecute.ExecuteInsert("ACCOUNTS_ACTLED_STORE", _conn, _conTran);
                #endregion

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
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
        }

        public bool AddSaleHeadRecord(AccountModel obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _sql = @"SELECT ACTLED_ACTGRP_ID FROM ACCOUNTS_ACTLED_STORE WHERE STORE_ID = PARMSTOREID 
                            AND ACTLED_ID = PARMSALESLEDGERID ";

                _sql = _sql.Replace("PARMSTOREID", obj.StoreID.ToString());
                _sql = _sql.Replace("PARMSALESLEDGERID", obj.SalesGroupID.ToString());

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                if (sqlcmd.ExecuteScalar() != null)
                {
                    if (sqlcmd.ExecuteScalar().ToString().Length > 0)
                        obj.GroupID = Convert.ToInt16(sqlcmd.ExecuteScalar());
                }

                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add Account Master

                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("ACTLED_ID", obj.LedgerID.ToString());
                dmlExecute.AddFields("ACTLED_NAME", obj.LedgerName.ToString());
                dmlExecute.AddFields("ACTLED_MAIN_LED_ID", obj.SalesGroupID.ToString());
                dmlExecute.AddFields("ACTLED_REMARKS", obj.Remarks.ToString());

                dmlExecute.AddFields("ACTLED_ACTGRP_ID", obj.GroupID.ToString());

                dmlExecute.AddFields("ACTLED_ACTIVE", obj.ActiveStatus.ToString());
                dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);

                dmlExecute.ExecuteInsert("ACCOUNTS_ACTLED_STORE", _conn, _conTran);
                #endregion

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
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
        }

        public bool UpdateRecord(AccountModel obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Update Account Master

                if (obj.DisplayStatus.Trim().Length == 0)
                {
                    obj.DisplayStatus = "Y";
                }
                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("ACTLED_ID", obj.LedgerID.ToString());
                dmlExecute.AddFields("ACTLED_NAME", obj.LedgerName.ToString());
                dmlExecute.AddFields("ACTLED_ACTGRP_ID", obj.GroupID.ToString());
                dmlExecute.AddFields("ACTLED_REMARKS", obj.Remarks.ToString());
                dmlExecute.AddFields("ACTLED_ACTIVE", obj.ActiveStatus.ToString());
                dmlExecute.AddFields("ACTLED_DISP_STATUS", obj.DisplayStatus.ToString());

                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);

                string[] KeyFields = { "STORE_ID", "ACTLED_ID" };

                dmlExecute.ExecuteUpdate("ACCOUNTS_ACTLED_STORE", _conn, KeyFields, _conTran);
                #endregion

                _conTran.Commit();
                return true;
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

        public bool DeleteRecord(int ID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRecord(int StoreID, int LedgerID)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                DMLExecute dmlExecute = new DMLExecute();

                #region Delete Account Master

                _sql = @"SELECT        COUNT(ACTTRN_STORE_ID) AS ACTTRN_STORE_ID
                            FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_STORE_ID = PARMSTOREID) AND (ACTTRN_ACTLED_ID = PARMLEDGERID)";
                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMLEDGERID", LedgerID.ToString());

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.Read())
                {
                    if (Convert.ToInt16(dr["ACTTRN_STORE_ID"]) > 0)
                    {
                        dr.Close();
                        throw new Exception("Transactions exists for the Account, so you cannot delete");
                    }
                }
                dr.Close();

                _conTran = _conn.BeginTransaction();
                _sql = @"DELETE FROM ACCOUNTS_ACTLED_STORE WHERE STORE_ID = PARMSTOREID AND ACTLED_ID = PARMLEDGERID";
                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMLEDGERID", LedgerID.ToString());

                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                #endregion

                _conTran.Commit();
                return true;
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

        public AccountModel SelectRecord(int ID)
        {
            throw new NotImplementedException();
        }

        public List<AccountModel> SelectRecords(int ID)
        {
            throw new NotImplementedException();
        }

        public List<AccountModel> SelectLotteryRecords(int ID)
        {
            try
            {
                List<AccountModel> objStoreAccounts = new List<AccountModel>();
                AccountModel objStoreAccount ;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_ID, ACTLED_ID, ACTLED_NAME, ACTLED_ACTGRP_ID, ACTLED_DISP_SIDE, ACTLED_REMARKS, ACTLED_ACTIVE
                        FROM            ACCOUNTS_ACTLED_STORE
                        WHERE        (ACTLED_ACTIVE = 'A') AND ACTLED_NAME LIKE '%LOTTERY%'
                        AND   (STORE_ID = " + ID + ") ORDER BY ACTLED_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStoreAccount = new AccountModel();
                    objStoreAccount.LedgerID = Convert.ToInt16(dr["ACTLED_ID"].ToString());
                    objStoreAccount.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStoreAccount.GroupID = Convert.ToInt16(dr["ACTLED_ACTGRP_ID"].ToString());
                    objStoreAccount.LedgerName = dr["ACTLED_NAME"].ToString();
                    objStoreAccount.DisplaySide = dr["ACTLED_DISP_SIDE"].ToString();
                    objStoreAccount.Remarks = dr["ACTLED_REMARKS"].ToString();
                    objStoreAccount.ActiveStatus = Convert.ToChar(dr["ACTLED_ACTIVE"].ToString());

                    objStoreAccounts.Add(objStoreAccount);
                }

                dr.Close();
                return objStoreAccounts;
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

        public List<AccountModel> SelectStoreRecords(int ID)
        {
            try
            {
                List<AccountModel> objStoreAccounts = new List<AccountModel>();
                AccountModel objStoreAccount;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_ID, ACTLED_ID, ACTLED_NAME, ACTLED_ACTGRP_ID, ACTLED_DISP_SIDE, ACTLED_REMARKS, ACTLED_ACTIVE
                        FROM            ACCOUNTS_ACTLED_STORE
                        WHERE        (ACTLED_ACTIVE = 'A') AND (ACTLED_NAME NOT LIKE '%LOTTERY%') AND (ACTLED_NAME NOT LIKE '%GAS%') 
                        AND   (STORE_ID = " + ID + ") ORDER BY ACTLED_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStoreAccount = new AccountModel();
                    objStoreAccount.LedgerID = Convert.ToInt16(dr["ACTLED_ID"].ToString());
                    objStoreAccount.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStoreAccount.GroupID = Convert.ToInt16(dr["ACTLED_ACTGRP_ID"].ToString());
                    objStoreAccount.LedgerName = dr["ACTLED_NAME"].ToString();
                    objStoreAccount.DisplaySide = dr["ACTLED_DISP_SIDE"].ToString();
                    objStoreAccount.Remarks = dr["ACTLED_REMARKS"].ToString();
                    objStoreAccount.ActiveStatus = Convert.ToChar(dr["ACTLED_ACTIVE"].ToString());

                    objStoreAccounts.Add(objStoreAccount);
                }

                dr.Close();
                return objStoreAccounts;
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

        public List<AccountModel> SelectGasRecords(int ID)
        {
            try
            {
                List<AccountModel> objStoreAccounts = new List<AccountModel>();
                AccountModel objStoreAccount;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_ID, ACTLED_ID, ACTLED_NAME, ACTLED_ACTGRP_ID, ACTLED_DISP_SIDE, ACTLED_REMARKS, ACTLED_ACTIVE
                        FROM            ACCOUNTS_ACTLED_STORE
                        WHERE        (ACTLED_ACTIVE = 'A') AND ACTLED_NAME LIKE '%GAS%'
                        AND   (STORE_ID = " + ID + ") ORDER BY ACTLED_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStoreAccount = new AccountModel();
                    objStoreAccount.LedgerID = Convert.ToInt16(dr["ACTLED_ID"].ToString());
                    objStoreAccount.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStoreAccount.GroupID = Convert.ToInt16(dr["ACTLED_ACTGRP_ID"].ToString());
                    objStoreAccount.LedgerName = dr["ACTLED_NAME"].ToString();
                    objStoreAccount.DisplaySide = dr["ACTLED_DISP_SIDE"].ToString();
                    objStoreAccount.Remarks = dr["ACTLED_REMARKS"].ToString();
                    objStoreAccount.ActiveStatus = Convert.ToChar(dr["ACTLED_ACTIVE"].ToString());

                    objStoreAccounts.Add(objStoreAccount);
                }

                dr.Close();
                return objStoreAccounts;
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

        public List<AccountModel> SelectRecords(int ID, string AccountGroups = "ALL")
        {
            try
            {
                List<AccountModel> objStoreAccounts = new List<AccountModel>();
                AccountModel objStoreAccount;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_ID, ACTLED_ID, ACTLED_NAME, ACTLED_ACTGRP_ID, ACTLED_DISP_SIDE, ACTLED_REMARKS, ACTLED_ACTIVE,  
                        ACCOUNTS_ACTGRP.ACTGRP_NAME AS GROUP_NAME, ACTLED_DISP_STATUS
                        FROM            ACCOUNTS_ACTLED_STORE INNER JOIN ACCOUNTS_ACTGRP  ON 
                        ACCOUNTS_ACTLED_STORE.ACTLED_ACTGRP_ID = ACCOUNTS_ACTGRP.ACTGRP_ID
                        WHERE        (ACTLED_ACTIVE = 'A') AND (ACCOUNTS_ACTLED_STORE.ACTLED_ACTGRP_ID <> 2) 
                        AND   (STORE_ID = " + ID + ") ";

                if (AccountGroups.Equals("SALES_GROUPS"))
                    _sql = _sql + " AND ACTLED_ACTGRP_ID IN (13, 14) AND (ACTLED_MAIN_LED_ID = 0)";

                else if (AccountGroups.Equals("BUSINESS_SALES_GROUPS"))
                    _sql = _sql + " AND ACTLED_ACTGRP_ID IN (13) AND (ACTLED_MAIN_LED_ID = 0)";

                else if (AccountGroups.Equals("BUSINESS_PAID_GROUPS"))
                    _sql = _sql + " AND ACTLED_ACTGRP_ID IN (14) AND (ACTLED_MAIN_LED_ID = 0)";

                else if (AccountGroups.Equals("SALES_LEDGERS"))
                {
                    _sql = @"SELECT ACCOUNTS_ACTLED_STORE.ACTLED_ID, ACCOUNTS_ACTLED_STORE.ACTLED_NAME, ACCOUNTS_ACTLED_STORE.STORE_ID, 
                                       ACCOUNTS_ACTLED_STORE.ACTLED_DISP_STATUS, ACCOUNTS_ACTLED_STORE_1.ACTLED_NAME AS GROUP_NAME
                                FROM            ACCOUNTS_ACTLED_STORE INNER JOIN
                                                            ACCOUNTS_ACTLED_STORE AS ACCOUNTS_ACTLED_STORE_1 ON ACCOUNTS_ACTLED_STORE.STORE_ID = ACCOUNTS_ACTLED_STORE_1.STORE_ID AND 
                                                            ACCOUNTS_ACTLED_STORE.ACTLED_MAIN_LED_ID = ACCOUNTS_ACTLED_STORE_1.ACTLED_ID
                                WHERE        (ACCOUNTS_ACTLED_STORE.STORE_ID = PARMSTOREID) AND (ACCOUNTS_ACTLED_STORE.ACTLED_ACTIVE = 'A') AND 
                                                            (ACCOUNTS_ACTLED_STORE.ACTLED_MAIN_LED_ID > 0)";

                    _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                }

                else if (AccountGroups.Equals("COMMON"))
                {
                    _sql = @"SELECT ACCOUNTS_ACTLED_STORE.ACTLED_ID, ACCOUNTS_ACTLED_STORE.ACTLED_NAME, ACCOUNTS_ACTLED_STORE.STORE_ID, 
                                       ACCOUNTS_ACTLED_STORE_1.ACTLED_NAME AS GROUP_NAME, ACCOUNTS_ACTLED_STORE_1.ACTLED_ACTGRP_ID,
                                        ACCOUNTS_ACTLED_STORE.ACTLED_DISP_STATUS, ACCOUNTS_ACTLED_STORE.ACTLED_DISP_SIDE, 
                                        ACCOUNTS_ACTLED_STORE.ACTLED_REMARKS, ACCOUNTS_ACTLED_STORE.ACTLED_ACTIVE
                                FROM            ACCOUNTS_ACTLED_STORE INNER JOIN
                                                            ACCOUNTS_ACTLED_STORE AS ACCOUNTS_ACTLED_STORE_1 ON ACCOUNTS_ACTLED_STORE.STORE_ID = ACCOUNTS_ACTLED_STORE_1.STORE_ID AND 
                                                            ACCOUNTS_ACTLED_STORE.ACTLED_MAIN_LED_ID = ACCOUNTS_ACTLED_STORE_1.ACTLED_ID
                                WHERE        (ACCOUNTS_ACTLED_STORE.STORE_ID = PARMSTOREID) AND (ACCOUNTS_ACTLED_STORE.ACTLED_ACTIVE = 'A')  
                                     AND (ACCOUNTS_ACTLED_STORE.ACTLED_ACTGRP_ID = 2) AND 
                                                            (ACCOUNTS_ACTLED_STORE.ACTLED_MAIN_LED_ID = 0)";

                    _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                }

                else if (AccountGroups.Equals("ALL"))
                {
                    _sql = @"SELECT ACCOUNTS_ACTLED_STORE.ACTLED_ID, ACCOUNTS_ACTLED_STORE.ACTLED_NAME, ACCOUNTS_ACTLED_STORE.STORE_ID, 
                                       ACCOUNTS_ACTLED_STORE_1.ACTLED_NAME AS GROUP_NAME, ACCOUNTS_ACTLED_STORE_1.ACTLED_ACTGRP_ID,
                                        ACCOUNTS_ACTLED_STORE.ACTLED_DISP_STATUS, ACCOUNTS_ACTLED_STORE.ACTLED_DISP_SIDE, 
                                        ACCOUNTS_ACTLED_STORE.ACTLED_REMARKS, ACCOUNTS_ACTLED_STORE.ACTLED_ACTIVE
                                FROM            ACCOUNTS_ACTLED_STORE INNER JOIN
                                                            ACCOUNTS_ACTLED_STORE AS ACCOUNTS_ACTLED_STORE_1 ON ACCOUNTS_ACTLED_STORE.STORE_ID = ACCOUNTS_ACTLED_STORE_1.STORE_ID AND 
                                                            ACCOUNTS_ACTLED_STORE.ACTLED_MAIN_LED_ID = ACCOUNTS_ACTLED_STORE_1.ACTLED_ID
                                WHERE        (ACCOUNTS_ACTLED_STORE.STORE_ID = PARMSTOREID) AND (ACCOUNTS_ACTLED_STORE.ACTLED_ACTIVE = 'A')  
                                     AND (ACCOUNTS_ACTLED_STORE.ACTLED_MAIN_LED_ID = 0)";

                    _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                }
                _sql = _sql + " ORDER BY ACTLED_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                if (AccountGroups.Equals("SALES_LEDGERS") || AccountGroups.Equals("BUSINESS_PAID_GROUPS"))
                {
                    while (dr.Read())
                    {
                        objStoreAccount = new AccountModel();
                        objStoreAccount.LedgerID = Convert.ToInt16(dr["ACTLED_ID"].ToString());
                        objStoreAccount.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                        objStoreAccount.SalesGroupName = dr["GROUP_NAME"].ToString();
                        objStoreAccount.LedgerName = dr["ACTLED_NAME"].ToString();
                        objStoreAccount.DisplayStatus = dr["ACTLED_DISP_STATUS"].ToString();

                        objStoreAccounts.Add(objStoreAccount);
                    }
                }
                else
                {
                    while (dr.Read())
                    {
                        objStoreAccount = new AccountModel();
                        objStoreAccount.LedgerID = Convert.ToInt16(dr["ACTLED_ID"].ToString());
                        objStoreAccount.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                        objStoreAccount.GroupID = Convert.ToInt16(dr["ACTLED_ACTGRP_ID"].ToString());
                        objStoreAccount.SalesGroupName = dr["GROUP_NAME"].ToString();
                        objStoreAccount.LedgerName = dr["ACTLED_NAME"].ToString();
                        objStoreAccount.DisplayStatus = dr["ACTLED_DISP_STATUS"].ToString();
                        if (!AccountGroups.Equals("BUSINESS_PAID"))
                        {
                            objStoreAccount.DisplaySide = dr["ACTLED_DISP_SIDE"].ToString();
                            objStoreAccount.Remarks = dr["ACTLED_REMARKS"].ToString();
                            objStoreAccount.ActiveStatus = Convert.ToChar(dr["ACTLED_ACTIVE"].ToString());
                        }
                        objStoreAccounts.Add(objStoreAccount);
                    }
                }
                dr.Close();
                return objStoreAccounts;
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

        public int GetAccountGroupID(int iStoreID, int iLedgerID)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                int iAccountGroupID = 0;

                _sql = "SELECT ACTLED_ACTGRP_ID FROM ACCOUNTS_ACTLED_STORE WHERE STORE_ID = " + iStoreID + " AND ACTLED_ID = " + iLedgerID;
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.Read())
                {
                    iAccountGroupID = Convert.ToInt16(dr["ACTLED_ACTGRP_ID"]);
                }
                dr.Close();
                return iAccountGroupID;
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

        public List<AccountModel> SelectRecords(int ID, string DisplaySide, DateTime? Date = null )
        
        {
            try
            {
                List<AccountModel> objStoreAccounts = new List<AccountModel>();
                AccountModel objStoreAccount;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_ID, ACTLED_ID, ACTLED_NAME, ACTLED_ACTGRP_ID, ACTLED_DISP_SIDE, ACTLED_REMARKS, ACTLED_ACTIVE,
                        ACTLED_OP_BAL, ACTLED_OP_BAL_TYPE 
                        FROM            ACCOUNTS_ACTLED_STORE
                        WHERE        (ACTLED_ACTIVE = 'A')
                        AND   (STORE_ID = " + ID + ") AND (ACTLED_DISP_SIDE LIKE '%" + DisplaySide + "%') ORDER BY ACTLED_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objStoreAccount = new AccountModel();
                    objStoreAccount.LedgerID = Convert.ToInt16(dr["ACTLED_ID"].ToString());
                    objStoreAccount.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStoreAccount.GroupID = Convert.ToInt16(dr["ACTLED_ACTGRP_ID"].ToString());
                    objStoreAccount.LedgerName = dr["ACTLED_NAME"].ToString().Trim();
                    objStoreAccount.DisplaySide = dr["ACTLED_DISP_SIDE"].ToString();
                    objStoreAccount.Remarks = dr["ACTLED_REMARKS"].ToString();
                    objStoreAccount.OpeningBalance = Convert.ToSingle(dr["ACTLED_OP_BAL"].ToString());
                    objStoreAccount.OpeningBalanceType = dr["ACTLED_OP_BAL_TYPE"].ToString();
                    objStoreAccount.ActiveStatus = Convert.ToChar(dr["ACTLED_ACTIVE"].ToString());
                    objStoreAccount.Date = Convert.ToDateTime(Date);

                    if (Convert.ToInt16(dr["ACTLED_ACTGRP_ID"]) == 6)
                        objStoreAccount.GroupType = "Bank";
                    else
                        objStoreAccount.GroupType = "Cash";

                    objStoreAccounts.Add(objStoreAccount);
                }

                dr.Close();


                foreach(AccountModel obj in objStoreAccounts)
                {
                    if ((obj.LedgerID == 4) || (obj.LedgerID == 5) ||
                            (obj.LedgerID == 31) || (obj.LedgerID == 32) ||
                            (obj.LedgerID == 33) || (obj.LedgerID == 40) ||
                            (obj.LedgerID == 19))
                    {
                        obj.DisplayType = "ReadOnly";
                        obj.Date = (DateTime)Date;
                        obj.Amount = GetDayTranAmount(obj.StoreID, Convert.ToDateTime(Date), obj.LedgerID, obj.DisplaySide);
                    }
                    else
                    {
                        obj.DisplayType = "Write";
                        obj.Amount = GetDayTranAmount(obj.StoreID, Convert.ToDateTime(Date), obj.LedgerID, obj.DisplaySide);
                    }
                }

                return objStoreAccounts;
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

        private float GetDayTranAmount(int StoreID, DateTime Date, int LedgerID, string DisplaySide)
        {
            float fAmount = 0;

            if (LedgerID == 4) // Total Lottery sale
                _sql = @"SELECT        SALE_LOTTERY_ONLINE + SALE_LOTTERY_SALE AS TOTAL_LOTTERY_SALE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + StoreID + ") AND (SALE_DATE = '" + Date + "')";

            else if (LedgerID == 19)  // Cheque Cashing Paid
                _sql = @"SELECT        SUM(CHEQUE_CASHING_TRAN.CHQCS_PAID_AMOUNT) AS PAID_AMOUNT
                            FROM            CHEQUE_CASHING_TRAN INNER JOIN
                                                     STORE_SALE_MASTER ON CHEQUE_CASHING_TRAN.CHQCS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     CHEQUE_CASHING_TRAN.CHQCS_SALE_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                            WHERE        (CHEQUE_CASHING_TRAN.CHQCS_STORE_ID = "+ StoreID + ") AND (STORE_SALE_MASTER.SALE_DATE = '" + Date + "')";

            else if (LedgerID == 5)  // Total Gas Sale
                _sql = @"SELECT        SALE_GAS_TOTAL_SALE 
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + StoreID + ") AND (SALE_DATE = '" + Date + "')";

            else if (LedgerID == 31)  // Total Card Receipt 
                _sql = @"SELECT        SUM(STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT) AS TOTAL_CARD_RECEIPT
                            FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                            WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = " + StoreID + ") AND (STORE_SALE_MASTER.SALE_DATE = '" + Date + "')";

            else if (LedgerID == 32)  // Total Lottery Paid
                _sql = @"SELECT        SALE_LOTTERY_CASH_INSTANT_PAID + SALE_LOTTERY_CASH_ONLINE_PAID AS TOTAL_LOTTERY_SALE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + StoreID + ") AND (SALE_DATE = '" + Date + "')";

            else if (LedgerID == 33)  // Total Cash Paid
                _sql = @"SELECT        SUM(ACTCASH_AMOUNT)
                            FROM            ACTCASH_TRAN
                            WHERE        (ACTCASH_INWARD_OUTWARD_TYPE = 'O') AND 
                            (ACTCASH_STORE_ID =" + StoreID + ") AND (ACTCASH_DATE = '" + Date + "') AND (ACTCASH_TRAN_TYPE = 'CP')";

            else if (LedgerID == 40)  // Total Cheque Paid
                _sql = @"SELECT        SUM(STORE_BUSINESS_TRANS.BUSINESS_AMOUNT) 
                            FROM            STORE_BUSINESS_TRANS INNER JOIN
                                             STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                             STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                            WHERE        (STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE = 'QP') AND 
                                      (STORE_SALE_MASTER.SALE_DATE = '" + Date + "') AND (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = " + StoreID + ")";

            else if (LedgerID == 1)  // Total Gas Sale
                _sql = @"SELECT        CASH_PHYSICAL_AT_STORE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + StoreID + ") AND (SALE_DATE = '" + Date + "')";

            else
            {
                _sql = @"SELECT        STORE_BUSINESS_TRANS.BUSINESS_AMOUNT
                            FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                            WHERE        (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = " + StoreID + ") AND (STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID = " + LedgerID + ") AND ";
                _sql += " (STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE = '" + DisplaySide + "') AND (STORE_SALE_MASTER.SALE_DATE = '" + Date + "')";
            }

            SqlCommand cmd = new SqlCommand(_sql, _conn);
            SqlDataReader dr;
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                if (dr[0].ToString().Length > 0)
                    fAmount = Convert.ToSingle(dr[0]);
            }
            dr.Close();

            return fAmount;
        }

        public int SelectMaxID(int StoreID)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = "SELECT MAX(ACTLED_ID) + 1 FROM ACCOUNTS_ACTLED_STORE WHERE STORE_ID = " + StoreID ;
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                int MaxID = 0;
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.Read())
                {
                    if ((dr[0].ToString() == null) || (dr[0].ToString().Length == 0))
                        MaxID = 1;
                    else
                        MaxID =(int) dr[0];
                }
                dr.Close();
                return MaxID;
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

        public int SelectMaxID()
        {
            throw new NotImplementedException();
        }

        public List<AccountGroups> SelectAccountGroups()
        {
            List<AccountGroups> AccountGroupsCollection = new List<AccountGroups>();
            AccountGroups _AccountGroups;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   ACTGRP_ID, ACTGRP_NAME
                            FROM            ACCOUNTS_ACTGRP WHERE ACTGRP_ID NOT IN (13,14)
                            ORDER BY ACTGRP_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _AccountGroups = new AccountGroups();
                    _AccountGroups.GroupID = (int)dr["ACTGRP_ID"];
                    _AccountGroups.GroupName = dr["ACTGRP_NAME"].ToString();
                    AccountGroupsCollection.Add(_AccountGroups);
                }
                dr.Close();
                return AccountGroupsCollection;
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

        public List<AccountGroups> SelectBusinessAccountGroups()
        {
            List<AccountGroups> AccountGroupsCollection = new List<AccountGroups>();
            AccountGroups _AccountGroups;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   ACTGRP_ID, ACTGRP_NAME
                            FROM            ACCOUNTS_ACTGRP WHERE ACTGRP_ID IN (13,14)
                            ORDER BY ACTGRP_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _AccountGroups = new AccountGroups();
                    _AccountGroups.GroupID = (int)dr["ACTGRP_ID"];
                    _AccountGroups.GroupName = dr["ACTGRP_NAME"].ToString();
                    AccountGroupsCollection.Add(_AccountGroups);
                }
                dr.Close();
                return AccountGroupsCollection;
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

        public struct AccountGroups
        {
            public int GroupID;
            public string GroupName;
        }

    }
}
