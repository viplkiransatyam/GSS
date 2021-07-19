using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class ReportGroupConfiguration
    {
        SqlConnection _conn;
        SqlTransaction _conTran;
        string _sql;

        public bool AddGroup(ReportGroup obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add Report Group Master

                _sql = "SELECT * FROM REPORT_GROUP WHERE REPORT_GROUP_NAME = '" + obj.GroupName + "'";
                SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
                SqlDataReader dr ;
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    dr.Close(); 
                    throw new Exception("Group name is already exists");
                }
                else
                    dr.Close();
                
                 Int64 iGroupID = 0;
                _sql = "SELECT MAX(REPORT_GROUP_ID) FROM REPORT_GROUP WHERE REPORT_STORE_ID = " + obj.StoreID;
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (cmd.ExecuteScalar().ToString().Length > 0)
                    iGroupID = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                else
                    iGroupID = 1;

                dmlExecute.AddFields("REPORT_STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("REPORT_GROUP_ID", iGroupID.ToString());
                dmlExecute.AddFields("REPORT_GROUP_NAME", obj.GroupName.ToString());

                dmlExecute.ExecuteInsert("REPORT_GROUP", _conn, _conTran);

                
                #endregion


                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Record already exists");
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

        public bool AddLedgerToGroup(List<ReportGroup> obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add Ledger to Group

                //foreach(ReportGroup objGroup in obj)
                //{
                //    _sql = "DELETE FROM REPORT_GROUP_MAPPING WHERE RPTLED_STORE_ID = " + objGroup.StoreID + " AND RPTLED_GROUP_ID = " + objGroup.GroupID;
                //    SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
                //    cmd.ExecuteNonQuery();
                //    break;
                //}

                foreach (ReportGroup objGroup in obj)
                {
                    dmlExecute.AddFields("RPTLED_GROUP_ID", objGroup.GroupID.ToString());
                    dmlExecute.AddFields("RPTLED_STORE_ID", objGroup.StoreID.ToString());
                    dmlExecute.AddFields("RPTLED_ACTLED_ID", objGroup.LedgerID.ToString());

                    dmlExecute.ExecuteInsert("REPORT_GROUP_MAPPING", _conn, _conTran);
                }

                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Record already exists");
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

        public List<ReportCustomGroup> GetCustomGroupReport(int iStoreID, DateTime dFrom, DateTime dTo)
        {
            List<ReportCustomGroup> objGroupList = new List<ReportCustomGroup>();
            ReportCustomGroup objGroup;
            SqlCommand cmd;
            SqlDataReader dr;
 
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Getting group wise totals
                _sql = @"SELECT REPORT_GROUP.REPORT_GROUP_ID, REPORT_GROUP.REPORT_GROUP_NAME,
	                         SUM(VW_CUSTOM_GROUP_RECORDS.DEBIT) AS DEBIT, 
						     SUM(VW_CUSTOM_GROUP_RECORDS.CREDIT) AS CREDIT
                            FROM            VW_CUSTOM_GROUP_RECORDS INNER JOIN
                                                     REPORT_GROUP ON VW_CUSTOM_GROUP_RECORDS.RPTLED_GROUP_ID = REPORT_GROUP.REPORT_GROUP_ID AND 
                                                     VW_CUSTOM_GROUP_RECORDS.RPTLED_STORE_ID = REPORT_GROUP.REPORT_STORE_ID
                            WHERE    (VW_CUSTOM_GROUP_RECORDS.RPTLED_STORE_ID = MAPSTOREID) AND 
                                                     (VW_CUSTOM_GROUP_RECORDS.ACTTRN_DATE >= 'FROMDATE') AND 
						                             (VW_CUSTOM_GROUP_RECORDS.ACTTRN_DATE <= 'TODATE')
                            GROUP BY REPORT_GROUP.REPORT_GROUP_ID, REPORT_GROUP.REPORT_GROUP_NAME
                            ORDER BY REPORT_GROUP.REPORT_GROUP_NAME";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("FROMDATE", dFrom.ToString());
                _sql = _sql.Replace("TODATE", dTo.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                        objGroup = new ReportCustomGroup();
                        objGroup.GroupName  = dr["REPORT_GROUP_NAME"].ToString();
                        objGroup.GroupID    = Convert.ToInt16(dr["REPORT_GROUP_ID"]);
                        objGroup.Debit      = Convert.ToSingle( dr["DEBIT"]);
                        objGroup.Credit     = Convert.ToSingle(dr["CREDIT"]);

                        objGroupList.Add(objGroup);
                }

                dr.Close();

                #endregion
                return objGroupList;
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

        public List<ReportGroup> GetCustomGroups(int iStoreID)
        {
            List<ReportGroup> objGroupList = new List<ReportGroup>();
            ReportGroup objGroup;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Getting group wise totals
                _sql = @"SELECT   REPORT_STORE_ID, REPORT_GROUP_ID, REPORT_GROUP_NAME
                                FROM            REPORT_GROUP
                                WHERE        (REPORT_STORE_ID = MAPSTOREID)
                                ORDER BY REPORT_GROUP_NAME";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objGroup = new ReportGroup();
                    objGroup.GroupName = dr["REPORT_GROUP_NAME"].ToString();
                    objGroup.GroupID = Convert.ToInt16(dr["REPORT_GROUP_ID"]);
                    objGroup.StoreID = Convert.ToInt16(dr["REPORT_STORE_ID"]);
                    objGroupList.Add(objGroup);
                }

                dr.Close();

                #endregion
                return objGroupList;
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

        public List<ReportCustomGroupAccountMaster> GetLedgerPerCustomGroup(int iStoreID, int iGroupID)
        {
            List<ReportCustomGroupAccountMaster> objLedgerList = new List<ReportCustomGroupAccountMaster>();
            ReportCustomGroupAccountMaster objLedger;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Getting group wise totals
                _sql = @"SELECT   REPORT_GROUP_MAPPING.RPTLED_ACTLED_ID, ACCOUNTS_ACTLED_STORE.ACTLED_NAME
                            FROM            REPORT_GROUP_MAPPING INNER JOIN
                                                     ACCOUNTS_ACTLED_STORE ON REPORT_GROUP_MAPPING.RPTLED_STORE_ID = ACCOUNTS_ACTLED_STORE.STORE_ID AND 
                                                     REPORT_GROUP_MAPPING.RPTLED_ACTLED_ID = ACCOUNTS_ACTLED_STORE.ACTLED_ID
                            WHERE        (REPORT_GROUP_MAPPING.RPTLED_STORE_ID = MAPSTOREID) AND (REPORT_GROUP_MAPPING.RPTLED_GROUP_ID = MAPGROUPID)
                            ORDER BY ACCOUNTS_ACTLED_STORE.ACTLED_NAME";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("MAPGROUPID", iGroupID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objLedger = new ReportCustomGroupAccountMaster();
                    objLedger.LedgerName = dr["ACTLED_NAME"].ToString();
                    objLedger.LedgerID = Convert.ToInt16(dr["RPTLED_ACTLED_ID"]);
                    objLedgerList.Add(objLedger);
                }

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

        public List<ReportCustomGroupAccounts> GetCustomGroupReport(int iStoreID, int iGroupID, DateTime dFrom, DateTime dTo)
        {
            List<ReportCustomGroupAccounts> objAccountList = new List<ReportCustomGroupAccounts>();
            ReportCustomGroupAccounts objAccount;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Account group wise totals
                _sql = @"SELECT    VW_CUSTOM_GROUP_RECORDS.ACTTRN_DATE, ACCOUNTS_ACTLED_STORE.ACTLED_NAME, VW_CUSTOM_GROUP_RECORDS.DEBIT, 
                         VW_CUSTOM_GROUP_RECORDS.CREDIT, VW_CUSTOM_GROUP_RECORDS.RPTLED_ACTLED_ID
                        FROM            VW_CUSTOM_GROUP_RECORDS INNER JOIN
                                                 ACCOUNTS_ACTLED_STORE ON VW_CUSTOM_GROUP_RECORDS.RPTLED_STORE_ID = ACCOUNTS_ACTLED_STORE.STORE_ID AND 
                                                 VW_CUSTOM_GROUP_RECORDS.RPTLED_ACTLED_ID = ACCOUNTS_ACTLED_STORE.ACTLED_ID
                        WHERE        (VW_CUSTOM_GROUP_RECORDS.RPTLED_GROUP_ID = MAPGROUPID) AND (VW_CUSTOM_GROUP_RECORDS.RPTLED_STORE_ID = MAPSTOREID) AND 
                                                 (VW_CUSTOM_GROUP_RECORDS.ACTTRN_DATE >= 'FROMDATE') AND 
                                                 (VW_CUSTOM_GROUP_RECORDS.ACTTRN_DATE <= 'TODATE')
                        ORDER BY VW_CUSTOM_GROUP_RECORDS.ACTTRN_DATE";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("MAPGROUPID", iGroupID.ToString());
                _sql = _sql.Replace("FROMDATE", dFrom.ToString());
                _sql = _sql.Replace("TODATE", dTo.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objAccount = new ReportCustomGroupAccounts();
                    objAccount.Date = Convert.ToDateTime(dr["ACTTRN_DATE"]);
                    objAccount.LedgerID = Convert.ToInt16(dr["RPTLED_ACTLED_ID"].ToString());
                    objAccount.LedgerName = dr["ACTLED_NAME"].ToString();
                    objAccount.Debit = Convert.ToSingle(dr["DEBIT"]);
                    objAccount.Credit = Convert.ToSingle(dr["CREDIT"]);

                    objAccountList.Add(objAccount);
                }

                dr.Close();

                #endregion
                return objAccountList;
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
