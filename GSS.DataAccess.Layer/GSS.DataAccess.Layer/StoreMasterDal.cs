using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSS.Data.Model;
using System.Data.SqlClient;
using System.Configuration;

namespace GSS.DataAccess.Layer
{
    public class StoreMasterDal : IDal<StoreMaster>
    {
        SqlConnection _conn ;
        SqlTransaction _conTran;
        string _sql;

        public bool AddRecord(StoreMaster obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                
                #region Add Store Master
                obj.NoOfTanks = 1;
                dmlExecute.AddFields("STORE_GROUP_ID", obj.GroupID.ToString());
                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("STORE_NAME", obj.StoreName.ToString());
                dmlExecute.AddFields("STORE_ADD1", obj.StoreAddress1.ToString());
                dmlExecute.AddFields("STORE_ADD2", obj.StoreAddress2.ToString());
                dmlExecute.AddFields("STATE_ID", obj.State.ToString());

                dmlExecute.AddFields("NO_OF_GAS_TANKS", obj.NoOfTanks.ToString());

                dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);

                dmlExecute.ExecuteInsert("STORE_MASTER", _conn, _conTran);

                #endregion
                
                #region Add Tank Master
                int iTankID = 0;

                List<TankMaster> objTanks = new List<TankMaster>();
                TankMaster objTank = new TankMaster();
                objTank.TankName = "TANK 1";
                objTanks.Add(objTank);
                obj.TankDetail = objTanks;

                foreach (TankMaster tankDet in obj.TankDetail)
                {
                    iTankID++;

                    dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("GAS_TANK_ID", iTankID.ToString());
                    dmlExecute.AddFields("GAS_TANK_NAME", tankDet.TankName);
                    dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                    dmlExecute.ExecuteInsert("STORE_GAS_TANKS_MASTER", _conn, _conTran);
                }
                #endregion

                #region Add Shifts

                if (obj.NoOfShifts == 0)
                    obj.NoOfShifts = 1;

                for (int i = 1; i <= obj.NoOfShifts; i++ )
                {
                    dmlExecute.AddFields("SHFT_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("SHFT_SHIFT_CODE", i.ToString());
                    dmlExecute.ExecuteInsert("SHIFT_CODES", _conn, _conTran);
                }
                #endregion

                #region Add cards for the store
//                _sql = @"INSERT INTO [MAPPING_STORE_CARD]
//                           ([STORE_ID]
//                           ,[CARD_TYPE_ID]
//                           ,[ACTIVE]
//                           ,[CREATED_BY]
//                           ,[CREATED_TIMESTAMP]
//                           ,[MODIFIED_BY]
//                           ,[MODIFIED_TIMESTAMP])
//		                   SELECT    NEW_STORE_ID,    GAS_CARD_TYPE_ID, 'A' AS Expr1, CREATED_BY, CREATED_TIMESTAMP, MODIFIED_BY, MODIFIED_TIMESTAMP
//			                FROM            GROUP_CARD_TYPE";

//                _sql = _sql.Replace("NEW_STORE_ID", obj.StoreID.ToString());
//                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                #endregion

                #region Account Ledgers for the store
                _sql = @"INSERT INTO ACCOUNTS_ACTLED_STORE
                                       ([STORE_ID]
                                       ,[ACTLED_ID]
                                       ,[ACTLED_NAME]
                                       ,[ACTLED_ACTGRP_ID]
                                       ,[ACTLED_DISP_SIDE]
                                       ,[ACTLED_REMARKS]
		                               ,[ACTLED_ACTIVE]
                                       ,[CREATED_BY]
                                       ,[CREATED_TIMESTAMP]
                                       ,[MODIFIED_BY]
                                       ,[MODIFIED_TIMESTAMP])
                                        SELECT NEW_STORE_ID,
		                                        [ACTLED_ID]
                                              ,[ACTLED_NAME]
                                              ,[ACTLED_ACTGRP_ID]
                                              ,[ACTLED_DISP_SIDE]
                                              ,[ACTLED_REMARKS]
                                              ,[ACTLED_ACTIVE]
                                              ,[CREATED_BY]
                                              ,[CREATED_TIMESTAMP]
                                              ,[MODIFIED_BY]
                                              ,[MODIFIED_TIMESTAMP]
                                          FROM [ACCOUNTS_LEDGER_MASTER] WHERE ACTLED_ACTIVE = 'A'";
                _sql = _sql.Replace("NEW_STORE_ID", obj.StoreID.ToString());
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                #endregion

                #region Adding Lotteries to group from store
                _sql = @"INSERT INTO GROUP_LOTTERY_MASTER (LOTTERY_GROUP_ID, LOTTERY_GAME_ID, LOTTERY_GAME_DESCRIPTION, LOTTERY_NO_OF_TICKETS, LOTTERY_TICKET_VALUE, 
                          LOTTERY_BOOK_FIRST_TICKET_NO, LOTTERY_BOOK_LAST_TICKET_NO, LOTTERY_BOOK_VALUE)
                            SELECT  PARMGROUPID AS GROUP_ID, LOTTERY_GAME_ID, LOTTERY_GAME_DESCRIPTION, LOTTERY_NO_OF_TICKETS, LOTTERY_TICKET_VALUE, 
                                                     LOTTERY_BOOK_FIRST_TICKET_NO, LOTTERY_BOOK_LAST_TICKET_NO, LOTTERY_BOOK_VALUE
                            FROM            STATE_LOTTERY_GAMES INNER JOIN
                                                     STORE_MASTER ON STATE_LOTTERY_GAMES.LOTTERY_STATE_ID = STORE_MASTER.STATE_ID
                            WHERE        (STORE_MASTER.STORE_ID = PARMSTOREID)
                            AND LOTTERY_GAME_ID NOT IN
                            (
                            SELECT   GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            FROM            GROUP_LOTTERY_MASTER INNER JOIN
                                                     STORE_MASTER ON GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                            WHERE        (STORE_MASTER.STORE_ID = PARMSTOREID)
                            ) ";

                _sql = _sql.Replace("PARMGROUPID", obj.GroupID.ToString());
                _sql = _sql.Replace("PARMSTOREID", obj.StoreID.ToString());
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                #endregion

                //                #region Add Lottery Master
//                _sql = @"INSERT INTO MAPPING_STORE_LOTTERY
//                                       (STORE_ID
//                                       ,LOTTER_ID
//                                       ,LOTTERY_NAME
//                                       ,CREATED_BY
//                                       ,CREATED_TIMESTAMP
//                                       ,MODIFIED_BY
//                                       ,MODIFIED_TIMESTAMP)
//                            SELECT        NEW_STORE_ID, LOTTER_ID, LOTTERY_NAME,  CREATED_BY
//                                       ,CREATED_TIMESTAMP
//                                       ,MODIFIED_BY
//                                       ,MODIFIED_TIMESTAMP
//                            FROM            GROUP_LOTTERY_MASTER";

//                _sql = _sql.Replace("NEW_STORE_ID", obj.StoreID.ToString());
//                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
//                #endregion

                #region Add Store Parameter Master
                _sql = @"INSERT INTO STORE_PARAMETER
                                       (STORE_ID
                                       ,PARAMETER_ID
                                       ,PARAMETER_TEXT
                                       ,PARAMETER_VALUE, PARAMETER_OP_BAL_TYPE)
                            SELECT        NEW_STORE_ID, PARAMETER_ID, PARAMETER_TEXT,  PARAMETER_VALUE, PARAMETER_OP_BAL_TYPE
                            FROM            DEFAULT_PARAMETER";

                _sql = _sql.Replace("NEW_STORE_ID", obj.StoreID.ToString());
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Store already exists");
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

        public bool AddUser(StoreUser obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _sql = "SELECT COUNT(*) FROM USER_LOGIN WHERE USER_ID = '" + obj.UserName.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(_sql, _conn);
                if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                    throw new Exception("The user name cannot be duplicated");

                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add User to Store

                dmlExecute.AddFields("USER_ID", obj.UserName.ToString());
                dmlExecute.AddFields("USER_PWD", obj.Password.ToString());
                dmlExecute.AddFields("USER_ACCESS_TYPE", obj.AccessType.ToString());
                dmlExecute.AddFields("GROUP_ID", obj.GroupID.ToString());
                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("EMAIL_ID", obj.UserName.ToString());
                dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);

                dmlExecute.ExecuteInsert("USER_LOGIN", _conn, _conTran);

                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Store already exists");
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

        public bool UpdateRecord(StoreMaster obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add Store Master
                dmlExecute.AddFields("STORE_GROUP_ID", obj.GroupID.ToString());
                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("STORE_NAME", obj.StoreName.ToString());
                dmlExecute.AddFields("STORE_ADD1", obj.StoreAddress1.ToString());
                dmlExecute.AddFields("STORE_ADD2", obj.StoreAddress2.ToString());
                dmlExecute.AddFields("NO_OF_GAS_TANKS", obj.NoOfTanks.ToString());
                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);

                string[] KeyFields = { "STORE_ID" };
                dmlExecute.ExecuteUpdate("STORE_MASTER", _conn, KeyFields, _conTran);
                #endregion

                #region Add Tank Master
                foreach (TankMaster tankDet in obj.TankDetail)
                {
                    dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("GAS_TANK_ID", tankDet.TankID.ToString());
                    dmlExecute.AddFields("GAS_TANK_NAME", tankDet.TankName);
                    dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);

                    string[] _keyfields = { "GAS_TANK_ID", "STORE_ID" };

                    dmlExecute.ExecuteUpdate("STORE_GAS_TANKS_MASTER", _conn, _keyfields, _conTran);
                }
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

        public bool UpdateUser(StoreUser obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Update User to Store

                dmlExecute.AddFields("USER_ID", obj.UserName.ToString());
                dmlExecute.AddFields("USER_ACCESS_TYPE", obj.AccessType.ToString());
                dmlExecute.AddFields("USER_ACTIVE_STATUS", obj.ActiveStatus.ToString());
                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("EMAIL_ID", obj.UserName.ToString());
                dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);

                string[] KeyFields = { "STORE_ID", "USER_ID" };
                dmlExecute.ExecuteUpdate("USER_LOGIN", _conn, KeyFields, _conTran);

                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Store already exists");
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

        public bool UpdatePassword(string userid, string newpassword)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                dmlExecute.AddFields("USER_ID", userid);
                dmlExecute.AddFields("USER_PWD", newpassword);

                string[] _keyfields = { "USER_ID"};
                dmlExecute.ExecuteUpdate("USER_LOGIN", _conn,_keyfields, _conTran);

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

        public StoreMaster SelectRecord(int ID)
        {
            try
            {
                StoreMaster objStore = new StoreMaster();
                TankMaster objTank;
                List<TankMaster> objTanks = new List<TankMaster>();

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_MASTER.STORE_GROUP_ID, STORE_MASTER.STORE_ID, STORE_MASTER.STORE_NAME, STORE_MASTER.STORE_ADD1,  
                                 STORE_MASTER.STORE_ADD2, STORE_MASTER.NO_OF_GAS_TANKS,STORE_MASTER.STORE_TYPE, STORE_GAS_TANKS_MASTER.GAS_TANK_ID, 
                                 STORE_GAS_TANKS_MASTER.GAS_TANK_NAME, AVAIL_GAS_STATION, 
                                 AVAIL_LOTTERY, AVAIL_BUSINESS, LOTTERY_AUTO_SETTLE_DAYS
                          FROM  STORE_MASTER INNER JOIN
                                 STORE_GAS_TANKS_MASTER ON STORE_MASTER.STORE_ID = STORE_GAS_TANKS_MASTER.STORE_ID
                          WHERE        (STORE_MASTER.STORE_ID = " + ID + ")";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                if (dr.Read())
                {
                    objStore.GroupID = Convert.ToInt16(dr["STORE_GROUP_ID"].ToString());
                    objStore.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStore.StoreAddress1 = dr["STORE_ADD1"].ToString();
                    objStore.StoreAddress2 = dr["STORE_ADD2"].ToString();
                    objStore.StoreName = dr["STORE_NAME"].ToString();
                    objStore.NoOfTanks = Convert.ToInt16(dr["NO_OF_GAS_TANKS"].ToString());
                    objStore.StoreType = dr["STORE_TYPE"].ToString();
                    objStore.LotteryAutoSettleDays = Convert.ToInt16(dr["LOTTERY_AUTO_SETTLE_DAYS"].ToString());
                    objStore.AvailGas = dr["AVAIL_GAS_STATION"].ToString();
                    objStore.AvailLottery = dr["AVAIL_LOTTERY"].ToString();
                    objStore.AvailBusiness = dr["AVAIL_BUSINESS"].ToString();

                    objTank = new TankMaster();
                    objTank.TankID = Convert.ToInt16(dr["GAS_TANK_ID"].ToString());
                    objTank.TankName = dr["GAS_TANK_NAME"].ToString();
                    objTanks.Add(objTank);

                    while(dr.Read())
                    {
                        objTank = new TankMaster();
                        objTank.TankID = Convert.ToInt16(dr["GAS_TANK_ID"].ToString());
                        objTank.TankName = dr["GAS_TANK_NAME"].ToString();
                        objTanks.Add(objTank);
                    }
                    objStore.TankDetail = objTanks;

                }

                dr.Close();
                return objStore;
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

        public List<StoreMaster> SelectRecords(int iGroupID)
        {
            try
            {
                List<StoreMaster> objStores = new List<StoreMaster>();
                StoreMaster objStore   ;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_MASTER.STORE_GROUP_ID, STORE_MASTER.STORE_ID, STORE_MASTER.STORE_NAME, STORE_MASTER.STORE_ADD1,
                                 STORE_MASTER.STORE_ADD2, STORE_MASTER.NO_OF_GAS_TANKS, STATE_ID
                          FROM  STORE_MASTER 
                          WHERE        (STORE_MASTER.STORE_GROUP_ID = " + iGroupID + ")  ORDER BY STORE_MASTER.STORE_ID";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStore = new StoreMaster();
                    objStore.GroupID = Convert.ToInt16(dr["STORE_GROUP_ID"].ToString());
                    objStore.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStore.StoreAddress1 = dr["STORE_ADD1"].ToString();
                    objStore.StoreAddress2 = dr["STORE_ADD2"].ToString();
                    objStore.StoreName = dr["STORE_NAME"].ToString();
                    objStore.State = dr["STATE_ID"].ToString();

                    objStores.Add(objStore);
                }

                dr.Close();
                return objStores;
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

        public List<StoreUser> SelectUsers(int iGroupID)
        {
            try
            {
                List<StoreUser> objStores = new List<StoreUser>();
                StoreUser objStore;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_MASTER.STORE_NAME, STORE_MASTER.STORE_ID, USER_LOGIN.USER_ID, USER_LOGIN.USER_ACCESS_TYPE, 
                         USER_LOGIN.USER_ACTIVE_STATUS
                        FROM            STORE_MASTER INNER JOIN
                                                    USER_LOGIN ON STORE_MASTER.STORE_ID = USER_LOGIN.STORE_ID
                        WHERE        (STORE_MASTER.STORE_GROUP_ID = PARMGROUP)
                        ORDER BY STORE_MASTER.STORE_NAME, USER_LOGIN.USER_ID";

                _sql = _sql.Replace("PARMGROUP", iGroupID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStore = new StoreUser();
                    objStore.GroupID = iGroupID;
                    objStore.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStore.StoreName = dr["STORE_NAME"].ToString();
                    objStore.UserName = dr["USER_ID"].ToString();
                    objStore.AccessType = Convert.ToChar(dr["USER_ACCESS_TYPE"].ToString());
                    objStore.ActiveStatus = Convert.ToChar(dr["USER_ACTIVE_STATUS"].ToString());

                    objStores.Add(objStore);
                }

                dr.Close();
                return objStores;
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

        public ParameterList SelectParameterOpeningBalanceList(int iStoreID)
        {
            try
            {
                List<ParameterMaster> objParmList = new List<ParameterMaster>();
                ParameterMaster obParm;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   STORE_ID, PARAMETER_ID, PARAMETER_TEXT, PARAMETER_VALUE
                            FROM            STORE_PARAMETER
                            WHERE        (STORE_ID = MAPSTOREID) AND (PARAMETER_OP_BAL_TYPE = 'Y')
                            ORDER BY PARAMETER_TEXT";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    obParm = new ParameterMaster();
                    obParm.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    obParm.ParameterID = Convert.ToInt16(dr["PARAMETER_ID"].ToString());
                    obParm.ParameterName = dr["PARAMETER_TEXT"].ToString();
                    obParm.ParameterValue = Convert.ToSingle(dr["PARAMETER_VALUE"].ToString());

                    objParmList.Add(obParm);
                }

                dr.Close();

                ParameterList objList = new ParameterList();
                objList.ParameterOpeningBalanceList = objParmList;
                return objList;
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

        public bool UpdateParameterOpeningBalance(ParameterList obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Update Parameter Opening Balance
                foreach (ParameterMaster objParm in obj.ParameterOpeningBalanceList)
                {
                    dmlExecute.AddFields("STORE_ID", objParm.StoreID.ToString());
                    dmlExecute.AddFields("PARAMETER_ID", objParm.ParameterID.ToString());
                    dmlExecute.AddFields("PARAMETER_VALUE", objParm.ParameterValue.ToString());

                    string[] KeyFields = { "STORE_ID", "PARAMETER_ID" };
                    dmlExecute.ExecuteUpdate("STORE_PARAMETER", _conn, KeyFields, _conTran);
                }
                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Store already exists");
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

        public List<StoreMaster> SelectRecords(string sUserName)
        {
            try
            {
                List<StoreMaster> objStores = new List<StoreMaster>();
                StoreMaster objStore ;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   USER_LOGIN.GROUP_ID,STORE_MASTER.STORE_ID, STORE_MASTER.STORE_NAME, STORE_MASTER.STORE_ADD1, STORE_MASTER.STORE_ADD2
                            FROM            USER_LOGIN INNER JOIN
                            STORE_MASTER ON USER_LOGIN.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                            WHERE        (USER_LOGIN.USER_ACTIVE_STATUS = 'A') 
                            AND (USER_LOGIN.USER_ID = '" + sUserName + "') AND  (USER_LOGIN.USER_ACCESS_TYPE = 0)";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStore = new StoreMaster();
                    objStore.GroupID = Convert.ToInt16(dr["GROUP_ID"].ToString());
                    objStore.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStore.StoreAddress1 = dr["STORE_ADD1"].ToString();
                    objStore.StoreAddress2 = dr["STORE_ADD2"].ToString();
                    objStore.StoreName = dr["STORE_NAME"].ToString();

                    objStores.Add(objStore);
                }

                dr.Close();
                return objStores;
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

        public List<Store> GetLotteryStores(int iGroupID)
        {
            try
            {
                List<Store> objStores = new List<Store>();
                Store objStore;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_MASTER.STORE_GROUP_ID, STORE_MASTER.STORE_ID, STORE_MASTER.STORE_NAME, STORE_MASTER.STORE_ADD1,
                                 STORE_MASTER.STORE_ADD2, STORE_MASTER.NO_OF_GAS_TANKS, STATE_ID
                          FROM  STORE_MASTER 
                          WHERE     AVAIL_LOTTERY = 'Y' AND   (STORE_MASTER.STORE_GROUP_ID = " + iGroupID + ")  ORDER BY STORE_MASTER.STORE_ID";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();


                while (dr.Read())
                {
                    objStore = new Store();
                    objStore.GroupID = Convert.ToInt16(dr["STORE_GROUP_ID"].ToString());
                    objStore.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objStore.StoreName = dr["STORE_NAME"].ToString();

                    objStores.Add(objStore);
                }

                dr.Close();
                return objStores;
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

        public List<StoreShift> StoreShift(int iStoreID)
        {
            try
            {
                List<StoreShift> objShifts = new List<StoreShift>();
                StoreShift objShift;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT SHFT_STORE_ID, SHFT_SHIFT_CODE  FROM            SHIFT_CODES
                        WHERE        (SHFT_STORE_ID = " + iStoreID + ")";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objShift = new StoreShift();
                    objShift.StoreID = Convert.ToInt16(dr["SHFT_STORE_ID"].ToString());
                    objShift.ShiftCode = Convert.ToInt16(dr["SHFT_SHIFT_CODE"].ToString());
                    objShifts.Add(objShift);
                }

                dr.Close();
                return objShifts;
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

        public float GetParameterValue(int iStoreID, int iParmID)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                float fParmValue = 0;

                _sql = "SELECT PARAMETER_VALUE FROM   STORE_PARAMETER WHERE STORE_ID = " + iStoreID + " AND PARAMETER_ID = " + iParmID;
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.Read())
                {
                    fParmValue = Convert.ToSingle(dr["PARAMETER_VALUE"]);
                }
                dr.Close();
                return fParmValue;
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
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = "SELECT MAX(STORE_ID) + 1 FROM STORE_MASTER";
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

        public StoreGroupID SelectStoreGroupID(string sUserID)
        {
            StoreGroupID _StoreGroupID = new StoreGroupID();
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = "SELECT STORE_ID, GROUP_ID FROM USER_LOGIN WHERE USER_ID = '" + sUserID + "'";
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.Read())
                {
                    _StoreGroupID.StoreID = (int)dr["STORE_ID"];
                    _StoreGroupID.GroupID = (int)dr["GROUP_ID"];
                }
                dr.Close();
                return _StoreGroupID;
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

        public List<GroupCards> SelectGroupCardTypes(string sUserID)
        {
            List<GroupCards> GroupCardCollection = new List<GroupCards>();
            GroupCards _GroupCards ;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  GROUP_CARD_TYPE.GROU_CARD_NAME, GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                            FROM            GROUP_CARD_TYPE INNER JOIN
                            USER_LOGIN ON GROUP_CARD_TYPE.GAS_GROUP_ID = USER_LOGIN.GROUP_ID
                            WHERE        (USER_LOGIN.USER_ID = '" + sUserID + "') ORDER BY GROUP_CARD_TYPE.GROU_CARD_NAME";
                
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _GroupCards = new GroupCards();
                    _GroupCards.CardTypeID = (int)dr["GAS_CARD_TYPE_ID"];
                    _GroupCards.CardTypeName =  dr["GROU_CARD_NAME"].ToString();
                    GroupCardCollection.Add(_GroupCards);
                }
                dr.Close();
                return GroupCardCollection;
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

        public List<GroupCards> SelectGroupCardTypes(int iGroupID)
        {
            List<GroupCards> GroupCardCollection = new List<GroupCards>();
            GroupCards _GroupCards;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  GROUP_CARD_TYPE.GROU_CARD_NAME, GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                            FROM            GROUP_CARD_TYPE 
                            WHERE        (GROUP_CARD_TYPE.GAS_GROUP_ID = " + iGroupID + ") ORDER BY GROUP_CARD_TYPE.GROU_CARD_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _GroupCards = new GroupCards();
                    _GroupCards.CardTypeID = (int)dr["GAS_CARD_TYPE_ID"];
                    _GroupCards.CardTypeName = dr["GROU_CARD_NAME"].ToString();
                    GroupCardCollection.Add(_GroupCards);
                }
                dr.Close();
                return GroupCardCollection;
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

        public List<GroupCards> SelectRemainingStoreCardTypes(int iStoreID)
        {
            List<GroupCards> GroupCardCollection = new List<GroupCards>();
            GroupCards _GroupCards;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  GROUP_CARD_TYPE.GROU_CARD_NAME, GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                            FROM            GROUP_CARD_TYPE 
                            WHERE        (GROUP_CARD_TYPE.GAS_GROUP_ID = 1) AND 
							GAS_CARD_TYPE_ID NOT IN
							(SELECT CARD_TYPE_ID FROM MAPPING_STORE_CARD
							WHERE STORE_ID = PARMSTOREID)
							ORDER BY GROUP_CARD_TYPE.GROU_CARD_NAME";

                _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _GroupCards = new GroupCards();
                    _GroupCards.CardTypeID = (int)dr["GAS_CARD_TYPE_ID"];
                    _GroupCards.CardTypeName = dr["GROU_CARD_NAME"].ToString();
                    GroupCardCollection.Add(_GroupCards);
                }
                dr.Close();
                return GroupCardCollection;
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

        public List<GroupCards> SelectStoreCardTypes(int iStoreID)
        {
            List<GroupCards> StoreCardCollection = new List<GroupCards>();
            GroupCards _StoreCards;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT        MAPPING_STORE_CARD.CARD_TYPE_ID, GROUP_CARD_TYPE.GROU_CARD_NAME
                            FROM            MAPPING_STORE_CARD INNER JOIN
                                                     GROUP_CARD_TYPE ON MAPPING_STORE_CARD.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                            WHERE        (MAPPING_STORE_CARD.STORE_ID = PARMSTOREID) AND (MAPPING_STORE_CARD.ACTIVE = 'A')
                            ORDER BY GROUP_CARD_TYPE.GROU_CARD_NAME";

                _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _StoreCards = new GroupCards();
                    _StoreCards.CardTypeID = (int)dr["CARD_TYPE_ID"];
                    _StoreCards.CardTypeName = dr["GROU_CARD_NAME"].ToString();
                    StoreCardCollection.Add(_StoreCards);
                }
                dr.Close();
                return StoreCardCollection;
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

        public struct StoreGroupID
        {
            public int StoreID;
            public int GroupID;
        }

        public struct GroupCards
        {
            public int CardTypeID;
            public string CardTypeName;
        }


    }
}
