using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class LotteryMaster
    {
        SqlConnection _conn;
        SqlTransaction _conTran;
        string _sql;
        bool bAccountSameVoucher;
        Int64 iAccountTranNo = 0;
        int iVouItemSlNo = 0;

        public List<LotteryModel> SelectLotterMaster(int ID)
        {
            try
            {
                List<LotteryModel> objLotteryColl = new List<LotteryModel>();
                LotteryModel objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   MAPPING_STORE_LOTTERY.STORE_ID, MAPPING_STORE_LOTTERY.LOTTER_ID, MAPPING_STORE_LOTTERY.LOTTERY_NAME, 
                             GROUP_LOTTERY_MASTER.LOTTERY_EACH_TICKET_VALUE, GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, 
                             GROUP_LOTTERY_MASTER.LOTTERY_BUNDLE_AMOUNT, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_START, 
                             GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_END
                            FROM    MAPPING_STORE_LOTTERY INNER JOIN
                                    STORE_MASTER ON MAPPING_STORE_LOTTERY.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                    GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.GROUP_ID AND 
                                    MAPPING_STORE_LOTTERY.LOTTER_ID = GROUP_LOTTERY_MASTER.LOTTER_ID
                            WHERE        (MAPPING_STORE_LOTTERY.ACTIVE = 'A') AND (MAPPING_STORE_LOTTERY.STORE_ID = " + ID + ") ORDER BY MAPPING_STORE_LOTTERY.LOTTER_ID";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objLottery = new LotteryModel();
                    objLottery.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objLottery.LotteryID = Convert.ToInt16(dr["LOTTER_ID"]);
                    objLottery.LotteryName = dr["LOTTERY_NAME"].ToString();
                    objLottery.TicketValue = Convert.ToInt16(dr["LOTTERY_EACH_TICKET_VALUE"].ToString());
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"]);
                    objLottery.BundleAmount = Convert.ToInt16(dr["LOTTERY_BUNDLE_AMOUNT"].ToString());
                    objLottery.BundleStartNumber = Convert.ToInt16(dr["LOTTERY_TICKET_NUMBER_START"].ToString());
                    objLottery.BundleEndNumber = Convert.ToInt16(dr["LOTTERY_TICKET_NUMBER_END"].ToString());
                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
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

        public LotteryPreviousDaySale GetPrevDaySale(LotteryPreviousDaySale obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   (GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_START - STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_CLOSING_SERIAL) 
                                                     * GROUP_LOTTERY_MASTER.LOTTERY_EACH_TICKET_VALUE AS AMOUNT_SOLD, STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID, 
                                                     STORE_LOTTERY_SALE_INDIVIDUAL.SALE_DAY_TRAN_ID, STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_CLOSING_SERIAL, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_EACH_TICKET_VALUE, GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_START, SALE_LOTTERY_RETURN, LOTTERY_SERIAL_NO, LOTTERY_NUMBER_OF_BOOKS_ACTIVE
                            FROM            STORE_LOTTERY_SALE_INDIVIDUAL INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_LOTTERY_SALE_INDIVIDUAL.LOTTER_ID = GROUP_LOTTERY_MASTER.LOTTER_ID INNER JOIN
                                                     STORE_MASTER ON STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                     GROUP_LOTTERY_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_LOTTERY_SALE_INDIVIDUAL.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID AND 
                                                     STORE_MASTER.STORE_ID = STORE_SALE_MASTER.STORE_ID
                            WHERE	STORE_SALE_MASTER.SALE_DATE IN 
                            (

                                SELECT        MAX(STORE_SALE_MASTER.SALE_DATE) AS Expr1
                                FROM            STORE_SALE_MASTER INNER JOIN
                                                         STORE_LOTTERY_SALE_INDIVIDUAL ON STORE_SALE_MASTER.STORE_ID = STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID AND 
                                                         STORE_SALE_MASTER.SALE_DAY_TRAN_ID = STORE_LOTTERY_SALE_INDIVIDUAL.SALE_DAY_TRAN_ID
                                WHERE        (STORE_SALE_MASTER.STORE_ID = MAP_STORE_ID) AND (STORE_SALE_MASTER.SALE_DATE < 'MAP_DATE')

                            )
                            GROUP BY STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID, STORE_LOTTERY_SALE_INDIVIDUAL.SALE_DAY_TRAN_ID, 
                                                     STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_CLOSING_SERIAL, GROUP_LOTTERY_MASTER.LOTTERY_EACH_TICKET_VALUE, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_START, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_START - STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_CLOSING_SERIAL, 
                                                     (GROUP_LOTTERY_MASTER.LOTTERY_TICKET_NUMBER_START - STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_CLOSING_SERIAL) 
                                                     * GROUP_LOTTERY_MASTER.LOTTERY_EACH_TICKET_VALUE, SALE_LOTTERY_RETURN, LOTTERY_SERIAL_NO, LOTTERY_NUMBER_OF_BOOKS_ACTIVE
                            HAVING        (STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID = MAP_STORE_ID)  AND 
                                                     (STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_NUMBER_OF_BOOKS_ACTIVE = 0)";

                _sql = _sql.Replace("MAP_STORE_ID", obj.StoreID.ToString());
                _sql = _sql.Replace("MAP_DATE", obj.Date.ToString());

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                float fPrevDaySale = 0;
                float fPrevLotteryReturn = 0;

                while (dr.Read())
                {
                    fPrevDaySale += Convert.ToSingle(dr["AMOUNT_SOLD"]);
                    fPrevLotteryReturn = Convert.ToSingle(dr["SALE_LOTTERY_RETURN"]);
                }

                dr.Close();
                obj.PrevDaySale = fPrevDaySale - fPrevLotteryReturn;

                if (obj.PrevDaySale == 0)
                {
                    _sql = @"SELECT        TOP (200) PARAMETER_VALUE
                                FROM            STORE_PARAMETER
                                WHERE        (STORE_ID = " + obj.StoreID + ") AND (PARAMETER_ID = 2)";

                    sqlcmd = new SqlCommand(_sql, _conn);
                    obj.PrevDaySale = Convert.ToSingle(sqlcmd.ExecuteScalar());
                }

                return obj;
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

        public bool AddBooksReceive(List<LotteryReceive> objLotteryBooksReceipt)
        {
            bool bResult = false;
            SqlCommand cmd;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Lottery Receipt

                foreach (LotteryReceive obj in objLotteryBooksReceipt)
                {
                    _sql = "SELECT        COUNT(1) AS BooksActive FROM            STORE_LOTTERY_RECEIVE ";
                    _sql += " WHERE        (LTREC_STORE_ID = " + obj.StoreID + ") AND (LTREC_GAME_ID = " + obj.GameID + ") AND (LTREC_PACK_NO = '" + obj.PackNo + "')";
                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                        throw new Exception("The pack " + obj.PackNo + " is already received, so this cannot be received again");

                    _sql = "SELECT        COUNT(1) AS BooksActive FROM            STORE_LOTTERY_BOOKS_ACTIVE ";
                    _sql += " WHERE        (LTACT_STORE_ID = " + obj.StoreID + ") AND (LTACT_GAME_ID = " + obj.GameID + ") AND (LTACT_PACK_NO = '" + obj.PackNo + "')";
                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                        throw new Exception("The pack " + obj.PackNo + " is already activated, so this cannot be received again");
                }

                foreach (LotteryReceive obj in objLotteryBooksReceipt)
                {
                    dmlExecute.AddFields("LTREC_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("LTREC_GAME_ID", obj.GameID.ToString());
                    dmlExecute.AddFields("LTREC_PACK_NO", obj.PackNo.ToString());
                    dmlExecute.AddFields("LTREC_DATE", obj.Date.ToString());
                    dmlExecute.AddFields("LTREC_SHIFT_ID", obj.ShiftID.ToString());
                    dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                    dmlExecute.ExecuteInsert("STORE_LOTTERY_RECEIVE", _conn, _conTran);
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

        public bool UpdateCommission(LotteryCommission objLotteryCommission)
        {
            bool bResult = false;
            SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                DMLExecute dmlExecute = new DMLExecute();
                float fTotalCommission = 0;
                fTotalCommission = objLotteryCommission.LotteryInstantCommission + objLotteryCommission.LotteryOnlineCommission + objLotteryCommission.LotteryCashCommission;

                if (fTotalCommission <= 0)
                    throw new Exception("Lottery commission should not be 0");

                #region Adding Lottery Receipt

                int iDayTranID = _SaleSupportEntries.DayTranID(objLotteryCommission.BusinessEndidngDate, objLotteryCommission.StoreID, 1, _conn);

                _conTran = _conn.BeginTransaction();
                dmlExecute.AddFields("STORE_ID", objLotteryCommission.StoreID.ToString());
                dmlExecute.AddFields("SALE_DAY_TRAN_ID", iDayTranID.ToString());
                dmlExecute.AddFields("SALE_DATE", objLotteryCommission.BusinessEndidngDate.ToString());
                dmlExecute.AddFields("SALE_SHIFT_CODE", "1");

                dmlExecute.AddFields("SALE_LOTTERY_ONLINE_COMMISSION", objLotteryCommission.LotteryOnlineCommission.ToString());
                dmlExecute.AddFields("SALE_LOTTER_INSTANT_COMMISSION", objLotteryCommission.LotteryInstantCommission.ToString());
                dmlExecute.AddFields("SALE_LOTTERY_CASH_COMMISSION", objLotteryCommission.LotteryCashCommission.ToString());

                if (_SaleSupportEntries.IsEdit == true)
                {
                    string[] KeyFields = { "STORE_ID", "SALE_DAY_TRAN_ID" };
                    dmlExecute.ExecuteUpdate("STORE_SALE_MASTER", _conn, KeyFields, _conTran);
                }
                else
                {
                    throw new Exception("No Lottery entries for this date");
                }


                _sql = @"DELETE FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_RECORD_TYPE = 'LM') AND 
                            (ACTTRN_STORE_ID = " + objLotteryCommission.StoreID + ") AND (ACTTRN_SALE_DAY_TRAN_ID = " + iDayTranID + ")";
                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                if (fTotalCommission > 0)
                {
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objLotteryCommission.StoreID, "DR", 11, objLotteryCommission.BusinessEndidngDate, fTotalCommission, "Lottery Paidout Commission", iDayTranID, "LM");
                    UpdateAccountEntry(objLotteryCommission.StoreID, "CR", 12, objLotteryCommission.BusinessEndidngDate, fTotalCommission, "Lottery Paidout Commission", iDayTranID, "LM");
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

        public bool UpdateLotterySettle(List<LotterySettlePack> objSettlePacks)
        {
            try
            {
                SqlCommand cmd;
                SqlConnection con = new SqlConnection(DMLExecute.con);
                con.Open();

                foreach (LotterySettlePack obj in objSettlePacks)
                {
                    _sql = @"UPDATE STORE_LOTTERY_BOOKS_ACTIVE SET LTACT_AUTO_SETTLE_STATUS = 'Y', LTACT_AUTO_SETTLE_DATE = GETDATE()
                        WHERE LTACT_STORE_ID = PARMSTOREID AND LTACT_GAME_ID = 'PARMGAMEID' AND LTACT_PACK_NO = 'PARMPACKNO'";

                    _sql = _sql.Replace("PARMSTOREID", obj.StoreID.ToString());
                    _sql = _sql.Replace("PARMGAMEID", obj.GameID.ToString());
                    _sql = _sql.Replace("PARMPACKNO", obj.PackNo.ToString());

                    cmd = new SqlCommand(_sql, con);
                    cmd.ExecuteNonQuery();
                }

                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AddBooksActive(List<LotteryActive> objLotteryBooksActive)
        {
            bool bResult = false;
            SqlCommand cmd;
            SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                int DaySaleTranID = 0;
                List<LotteryGames> objGames = new List<LotteryGames>();

                #region Adding Lottery Book Active

                // Validating book is already activated or not
                foreach (LotteryActive obj in objLotteryBooksActive)
                {
                    if (objGames.Count == 0) 
                        objGames = GetLotteryGames(obj.StoreID);
                    
                    _sql = "SELECT        COUNT(1) AS BooksReceive FROM            STORE_LOTTERY_RECEIVE ";
                    _sql += " WHERE        (LTREC_STORE_ID = " + obj.StoreID + ") AND (LTREC_GAME_ID = " + obj.GameID + ") AND (LTREC_PACK_NO = '" + obj.PackNo + "')";
                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) == 0)
                        throw new Exception("The pack " + obj.PackNo + " is not received, so this cannot be activated");

                    _sql = "SELECT        COUNT(1) AS BooksActive FROM            STORE_LOTTERY_BOOKS_ACTIVE ";
                    _sql += " WHERE        (LTACT_STORE_ID = " + obj.StoreID + ") AND (LTACT_GAME_ID = " + obj.GameID + ") AND (LTACT_PACK_NO = '" + obj.PackNo + "')";
                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                        throw new Exception("The pack " + obj.PackNo + " is already activated, so this cannot be activated again");
                }

                foreach (LotteryActive obj in objLotteryBooksActive)
                {
                    obj.SlNo = GetSlNoFromBox(obj.StoreID, obj.BoxNo);
                    dmlExecute.AddFields("LTACT_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("LTACT_GAME_ID", obj.GameID.ToString());
                    dmlExecute.AddFields("LTACT_PACK_NO", obj.PackNo.ToString());
                    dmlExecute.AddFields("LTACT_DATE", obj.Date.ToString());
                    dmlExecute.AddFields("LTACT_SHIFT_ID", obj.ShiftID.ToString());
                    dmlExecute.AddFields("LTACT_BOX_ID", obj.BoxNo.ToString());
                    dmlExecute.AddFields("LTACT_SL_NO", obj.SlNo.ToString());
                    dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                    dmlExecute.ExecuteInsert("STORE_LOTTERY_BOOKS_ACTIVE", _conn, _conTran);
                    if (DaySaleTranID == 0)
                    {
                        SqlConnection conTemp = new SqlConnection();
                        conTemp = new SqlConnection(DMLExecute.con);
                        conTemp.Open();
                        DaySaleTranID = _SaleSupportEntries.DayTranID(obj.Date, obj.StoreID, obj.ShiftID, conTemp);
                        conTemp.Close();
                        conTemp.Dispose();

                        #region Adding Sales Master Fields
                        dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                        dmlExecute.AddFields("SALE_DAY_TRAN_ID", DaySaleTranID.ToString());
                        dmlExecute.AddFields("SALE_SHIFT_CODE", obj.ShiftID.ToString());
                        dmlExecute.AddFields("SALE_DATE", obj.Date.ToString());
                        dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                        dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                        dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                        dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                        dmlExecute.AddFields("LOTTERY_SHIFT_OPEN", "OPEN");

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
                    }
                }

                #endregion

                #region Adding to Lottery Daily Report
                LotteryGames objGame = new LotteryGames();


                foreach (LotteryActive obj in objLotteryBooksActive)
                {
                    #region Updating last serial number to the book that is already activated in same box
                    string GameID = "";
                    int LastSerialNo = 0;

                    _sql = @"SELECT REPL_GAME_ID FROM REPORT_LOTTERY_DAILY WHERE (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_SALE_DAY_TRAN_ID = " + DaySaleTranID + ")";
                                _sql +=  " AND (REPL_BOX_ID = " + obj.BoxNo + ")";
                                _sql +=  " AND REPL_SLNO IN";
                                _sql +=  " (";
                                    _sql +=  " SELECT      MAX(REPL_SLNO)";
                                    _sql +=  " FROM            REPORT_LOTTERY_DAILY ";
                                    _sql += " WHERE        (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_SALE_DAY_TRAN_ID = " + DaySaleTranID + ") AND (REPL_BOX_ID = " + obj.BoxNo + ")";
                                _sql +=  " )";

                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (cmd.ExecuteScalar() != null)
                    {
                        GameID = (string)cmd.ExecuteScalar();
                        objGame = objGames.Find(x => x.GameID == GameID);
                        LastSerialNo = objGame.TicketEndNumber;

                        _sql = @"SELECT  GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO
                                FROM            MAPPING_STORE_LOTTERY INNER JOIN
                                                         STORE_MASTER ON MAPPING_STORE_LOTTERY.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                         GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                         MAPPING_STORE_LOTTERY.GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                                WHERE        (MAPPING_STORE_LOTTERY.STORE_ID = " + obj.StoreID + ") AND (MAPPING_STORE_LOTTERY.GAME_ID = '" + GameID + "')";
                        cmd = new SqlCommand(_sql, _conn, _conTran);
                        LastSerialNo = (int)cmd.ExecuteScalar();

                        // Commented below code to support any box activation and any book scanning
                        //_sql = @"UPDATE REPORT_LOTTERY_DAILY SET REPL_END_TICKET = " + Convert.ToInt16(LastSerialNo + 1) + " WHERE (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_SALE_DAY_TRAN_ID = " + DaySaleTranID + ")";
                        //        _sql += " AND (REPL_BOX_ID = " + obj.BoxNo + ")";
                        //        _sql += " AND REPL_SLNO IN";
                        //        _sql += " (";
                        //                _sql += " SELECT      MAX(REPL_SLNO)";
                        //                _sql += " FROM            REPORT_LOTTERY_DAILY ";
                        //                _sql += " WHERE        (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_SALE_DAY_TRAN_ID = " + DaySaleTranID + ") AND (REPL_BOX_ID = " + obj.BoxNo + ")";
                        //        _sql += " )";

                        //dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                    }

                    #endregion
                    objGame = objGames.Find(x => x.GameID == obj.GameID.ToString());

                    obj.SlNo = GetSlNoFromDailyReport(obj.StoreID, obj.BoxNo);
                    dmlExecute.AddFields("REPL_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("REPL_SALE_DAY_TRAN_ID", DaySaleTranID.ToString());
                    dmlExecute.AddFields("REPL_GAME_ID", obj.GameID.ToString());
                    dmlExecute.AddFields("REPL_BOX_ID", obj.BoxNo.ToString());
                    dmlExecute.AddFields("REPL_PACK_NO", obj.PackNo.ToString());
                    dmlExecute.AddFields("REPL_SLNO", obj.SlNo.ToString());
                    dmlExecute.AddFields("REPL_START_TICKET", objGame.TicketStartNumber.ToString());
                    dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                    dmlExecute.ExecuteInsert("REPORT_LOTTERY_DAILY",_conn,_conTran);
                }
                #endregion

                _conTran.Commit();

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

        public bool AddLotteryReturn(List<LotteryReturn> objLotteryReturn)
        {
            bool bResult = false;
            SqlCommand cmd;
            List<LotteryGames> objGames = new List<LotteryGames>();
            LotteryGames objGame;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Lottery Return

                foreach (LotteryReturn obj in objLotteryReturn)
                {
                    if (objGames.Count == 0)
                        objGames = GetLotteryGames(obj.StoreID);

                    _sql = "SELECT        COUNT(1) AS BooksReceive FROM            STORE_LOTTERY_RECEIVE ";
                    _sql += " WHERE        (LTREC_STORE_ID = " + obj.StoreID + ") AND (LTREC_GAME_ID = " + obj.GameID + ") AND (LTREC_PACK_NO = '" + obj.PackNo + "')";
                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) <= 0)
                        throw new Exception("The pack " + obj.PackNo + " is not received, so this cannot be returned");

                    if (obj.ReturnFrom == 'A')
                    {
                        _sql = "SELECT        COUNT(1) AS BooksActive FROM            REPORT_LOTTERY_DAILY ";
                        _sql += " WHERE        (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_GAME_ID = " + obj.GameID + ") AND (REPL_PACK_NO = '" + obj.PackNo + "')";
                        _sql += " AND REPL_END_TICKET IS NULL";

                        cmd = new SqlCommand(_sql, _conn, _conTran);
                        if (Convert.ToInt16(cmd.ExecuteScalar()) <= 0)
                            throw new Exception("The pack " + obj.PackNo + " is not activated");
                    }
                    if (obj.ReturnFrom == 'I')
                    {
                        _sql = "SELECT        COUNT(1) AS BooksActive FROM            REPORT_LOTTERY_DAILY ";
                        _sql += " WHERE        (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_GAME_ID = " + obj.GameID + ") AND (REPL_PACK_NO = '" + obj.PackNo + "')";
                        _sql += " AND REPL_END_TICKET IS NULL";

                        cmd = new SqlCommand(_sql, _conn, _conTran);
                        if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                            throw new Exception("The pack " + obj.PackNo + " is activated, please scan at return from active section");
                    }

                    objGame = objGames.Find(x => x.GameID == obj.GameID.ToString());
                    _sql = "SELECT        COUNT(1) AS BooksActive FROM            REPORT_LOTTERY_DAILY ";
                    _sql += " WHERE        (REPL_STORE_ID = " + obj.StoreID + ") AND (REPL_GAME_ID = " + obj.GameID + ") AND (REPL_PACK_NO = '" + obj.PackNo + "')";
                    _sql += " AND REPL_END_TICKET = " + obj.LastTicketClosing;

                    cmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                        throw new Exception("The pack " + obj.PackNo + " is sold out completely, so this cannot be returned");
                }

                float CreditAmount = 0;
                string sRemarks = string.Empty ;

                foreach (LotteryReturn obj in objLotteryReturn)
                {
                    _sql = "DELETE FROM STORE_LOTTERY_RETURNS WHERE LTRET_STORE_ID = " + obj.StoreID + " AND LTRET_GAME_ID = '" + obj.GameID + "'";
                    _sql += " AND LTRET_PACK_NO = '" + obj.PackNo.ToString() + "'" ;
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                    CreditAmount = 0;

                    dmlExecute.AddFields("LTRET_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("LTRET_DATE", obj.Date.ToString());
                    dmlExecute.AddFields("LTRET_GAME_ID", obj.GameID.ToString());
                    dmlExecute.AddFields("LTRET_PACK_NO", obj.PackNo.ToString());
                    dmlExecute.AddFields("LTRET_SHIFT_ID", obj.ShiftID.ToString());
                    dmlExecute.AddFields("LTRET_RETURN_FROM", obj.ReturnFrom.ToString());
                    objGame = objGames.Find(x => x.GameID == obj.GameID.ToString());

                    if (obj.ReturnFrom == 'A')
                    {
                        dmlExecute.AddFields("LTRET_LAST_TICKET_NO", obj.LastTicketClosing.ToString());
                        CreditAmount = (obj.LastTicketClosing - objGame.TicketStartNumber) * objGame.TicketValue;
                        sRemarks = "GameID:" + obj.GameID + "/PackNo:" + obj.PackNo;
                    }
                    else
                    {
                        dmlExecute.AddFields("LTRET_LAST_TICKET_NO", objGame.TicketEndNumber.ToString());
                    }
                    
                    dmlExecute.AddFields("LTRET_CREDIT_AMOUNT", CreditAmount.ToString());
                    dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                    dmlExecute.ExecuteInsert("STORE_LOTTERY_RETURNS", _conn, _conTran);

                    if (obj.ReturnFrom == 'A')
                    {
                        _sql = "DELETE FROM ACTTRN_TRANS WHERE ACTTRN_STORE_ID = " + obj.StoreID + " AND ACTTRN_RECORD_TYPE = 'LR'";
                        _sql += " AND ACTTRN_REMARKS = '" + sRemarks + "'";
                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                        bAccountSameVoucher = false;
                        UpdateAccountEntry(obj.StoreID, "DR", 11, obj.Date, CreditAmount, sRemarks, 0, "LR");
                        UpdateAccountEntry(obj.StoreID, "CR", 27, obj.Date, CreditAmount, sRemarks, 0, "LR");

                        _sql = @"UPDATE REPORT_LOTTERY_DAILY SET REPL_END_TICKET = PARMLASTTICKET
                                WHERE REPL_STORE_ID = PARMSTOREID AND REPL_GAME_ID = 'PARMGAMEID'
                                AND REPL_PACK_NO = 'PARMPACKNO' AND REPL_SALE_DAY_TRAN_ID IN
                                (
	                                SELECT MAX(REPL_SALE_DAY_TRAN_ID) FROM REPORT_LOTTERY_DAILY 
	                                WHERE REPL_STORE_ID = PARMSTOREID AND REPL_GAME_ID = 'PARMGAMEID'
	                                AND REPL_PACK_NO = 'PARMPACKNO'
                                )";

                        _sql = _sql.Replace("PARMSTOREID", obj.StoreID.ToString());
                        _sql = _sql.Replace("PARMPACKNO", obj.PackNo);
                        _sql = _sql.Replace("PARMGAMEID", obj.GameID);
                        _sql = _sql.Replace("PARMLASTTICKET", obj.LastTicketClosing.ToString());

                        dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);


                    }
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

        public bool ActivateLotteryModule(LotteryMapping objLotteryMapping)
        {
            bool bResult = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Activate Lottery Module

                _sql = "UPDATE STORE_MASTER SET AVAIL_LOTTERY = 'Y', LOTTERY_AUTO_SETTLE_DAYS = " + objLotteryMapping.AutoSettleDays;
                _sql += " WHERE STORE_ID = " + objLotteryMapping.StoreID;

                dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                foreach (Games obj in objLotteryMapping.Games)
                {
                    dmlExecute.AddFields("STORE_ID", objLotteryMapping.StoreID.ToString());
                    dmlExecute.AddFields("GAME_ID", obj.GameID.ToString());
                    dmlExecute.AddFields("CREATED_BY", objLotteryMapping.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", objLotteryMapping.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", objLotteryMapping.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", objLotteryMapping.ModifiedTimeStamp);
                    dmlExecute.ExecuteInsert("MAPPING_STORE_LOTTERY", _conn, _conTran);
                }

                SqlCommand sqlcmd;

                for (int i = 1; i <= objLotteryMapping.NoOfSlots;i++ )
                {
                    _sql = @"SELECT COUNT(LTBOX_STORE_ID) FROM STORE_LOTTERY_BOX_MASTER WHERE LTBOX_STORE_ID = PARMSTOREID
                              AND LTBOX_BOX_ID = PARMBOXID"                        ;
                    _sql = _sql.Replace("PARMSTOREID", objLotteryMapping.StoreID.ToString());
                    _sql = _sql.Replace("PARMBOXID", i.ToString());
                    sqlcmd = new SqlCommand(_sql, _conn, _conTran);
                    if (Convert.ToInt16(sqlcmd.ExecuteScalar()) == 0)
                    {
                        dmlExecute.AddFields("LTBOX_STORE_ID", objLotteryMapping.StoreID.ToString());
                        dmlExecute.AddFields("LTBOX_BOX_ID", i.ToString());
                        dmlExecute.AddFields("LTBOX_BOX_DESCRIPTION", i.ToString());
                        dmlExecute.ExecuteInsert("STORE_LOTTERY_BOX_MASTER", _conn, _conTran);
                    }
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

        public List<LotteryGames> GetLotteryGames(int ID)
        {
            SqlConnection con = new SqlConnection();
            try
            {
                List<LotteryGames> objLotteryColl = new List<LotteryGames>();
                LotteryGames objLottery;

                con = new SqlConnection(DMLExecute.con);
                con.Open();

                _sql = @"SELECT    MAPPING_STORE_LOTTERY.STORE_ID,  MAPPING_STORE_LOTTERY.GAME_ID, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, 
                         GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_FIRST_TICKET_NO, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO
                            FROM            MAPPING_STORE_LOTTERY INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON MAPPING_STORE_LOTTERY.GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                        INNER JOIN STORE_MASTER ON STORE_MASTER.STORE_ID = MAPPING_STORE_LOTTERY.STORE_ID
                         AND STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID";
                _sql += " WHERE        (MAPPING_STORE_LOTTERY.ACTIVE = 'A') AND (MAPPING_STORE_LOTTERY.STORE_ID = " + ID + ")";
                _sql += " ORDER BY MAPPING_STORE_LOTTERY.GAME_ID";

                SqlCommand sqlcmd = new SqlCommand(_sql, con);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objLottery = new LotteryGames();
                    objLottery.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objLottery.GameID = dr["GAME_ID"].ToString();
                    objLottery.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    objLottery.TicketValue = Convert.ToInt16(dr["LOTTERY_TICKET_VALUE"].ToString());
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"]);
                    objLottery.BookValue = Convert.ToInt16(dr["LOTTERY_BOOK_VALUE"].ToString());
                    objLottery.TicketStartNumber = Convert.ToInt16(dr["LOTTERY_BOOK_FIRST_TICKET_NO"].ToString());
                    objLottery.TicketEndNumber = Convert.ToInt16(dr["LOTTERY_BOOK_LAST_TICKET_NO"].ToString());
                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        //This function is used to return games the specified state id that store belongs to
        public List<LotteryGames> GetApplicableLotteryGames(int ID)
        {
            SqlConnection con = new SqlConnection();
            try
            {
                List<LotteryGames> objLotteryColl = new List<LotteryGames>();
                LotteryGames objLottery;

                con = new SqlConnection(DMLExecute.con);
                con.Open();

                _sql = @"SELECT   STATE_LOTTERY_GAMES.LOTTERY_GAME_ID, STATE_LOTTERY_GAMES.LOTTERY_GAME_DESCRIPTION, 
                             STATE_LOTTERY_GAMES.LOTTERY_NO_OF_TICKETS, STATE_LOTTERY_GAMES.LOTTERY_TICKET_VALUE, 
                             STATE_LOTTERY_GAMES.LOTTERY_BOOK_FIRST_TICKET_NO, STATE_LOTTERY_GAMES.LOTTERY_BOOK_LAST_TICKET_NO, 
                             STATE_LOTTERY_GAMES.LOTTERY_BOOK_VALUE, STORE_MASTER.STORE_ID
                        FROM            STATE_LOTTERY_GAMES INNER JOIN
                                                    STORE_MASTER ON STATE_LOTTERY_GAMES.LOTTERY_STATE_ID = STORE_MASTER.STATE_ID
                        WHERE        (STORE_MASTER.STORE_ID = PARMSTOREID) AND LOTTERY_GAME_ID NOT IN 
                        (
                        SELECT    GAME_ID
                        FROM            MAPPING_STORE_LOTTERY
                        WHERE        (STORE_ID = PARMSTOREID)
                        )
                        ORDER BY STATE_LOTTERY_GAMES.LOTTERY_GAME_ID  
                        ";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, con);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objLottery = new LotteryGames();
                    objLottery.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objLottery.GameID = dr["LOTTERY_GAME_ID"].ToString();
                    objLottery.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    objLottery.TicketValue = Convert.ToInt16(dr["LOTTERY_TICKET_VALUE"].ToString());
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"]);
                    objLottery.BookValue = Convert.ToInt16(dr["LOTTERY_BOOK_VALUE"].ToString());
                    objLottery.TicketStartNumber = Convert.ToInt16(dr["LOTTERY_BOOK_FIRST_TICKET_NO"].ToString());
                    objLottery.TicketEndNumber = Convert.ToInt16(dr["LOTTERY_BOOK_LAST_TICKET_NO"].ToString());
                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public List<LotteryReceive> GetLotteryReceive(int StoreID, DateTime dDate)
        {
            try
            {
                List<LotteryReceive> objLotteryColl = new List<LotteryReceive>();
                LotteryReceive objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_LOTTERY_RECEIVE.LTREC_GAME_ID, STORE_LOTTERY_RECEIVE.LTREC_PACK_NO, STORE_LOTTERY_RECEIVE.LTREC_SHIFT_ID, 
                         STORE_LOTTERY_RECEIVE.CREATED_BY, STORE_LOTTERY_RECEIVE.CREATED_TIMESTAMP, STORE_LOTTERY_RECEIVE.MODIFIED_BY, 
                         STORE_LOTTERY_RECEIVE.MODIFIED_TIMESTAMP, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, STORE_LOTTERY_RECEIVE.LTREC_DATE,
                         GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS
                        FROM            STORE_LOTTERY_RECEIVE INNER JOIN
                                                 STORE_MASTER ON STORE_LOTTERY_RECEIVE.LTREC_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                 STORE_LOTTERY_RECEIVE.LTREC_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                        WHERE        (STORE_LOTTERY_RECEIVE.LTREC_STORE_ID = " + StoreID + ") AND (STORE_LOTTERY_RECEIVE.LTREC_DATE = '" + dDate + "')";
                _sql += " ORDER BY GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objLottery = new LotteryReceive();
                    objLottery.StoreID = StoreID;
                    objLottery.GameID = Convert.ToInt16(dr["LTREC_GAME_ID"]);
                    objLottery.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"].ToString());
                    objLottery.PackNo = dr["LTREC_PACK_NO"].ToString();
                    objLottery.Date = Convert.ToDateTime(dr["LTREC_DATE"].ToString());
                    objLottery.ShiftID = Convert.ToInt16(dr["LTREC_SHIFT_ID"].ToString());
                    objLottery.CreatedUserName = dr["CREATED_BY"].ToString();
                    objLottery.ModifiedUserName = dr["MODIFIED_BY"].ToString();

                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
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

        public List<LotteryActive> GetLotteryActive(int StoreID, DateTime dDate)
        {
            try
            {
                List<LotteryActive> objLotteryColl = new List<LotteryActive>();
                LotteryActive objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO, 
                         STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SHIFT_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_BOX_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SL_NO, 
                         STORE_LOTTERY_BOOKS_ACTIVE.CREATED_BY, STORE_LOTTERY_BOOKS_ACTIVE.CREATED_TIMESTAMP, STORE_LOTTERY_BOOKS_ACTIVE.MODIFIED_BY, 
                         STORE_LOTTERY_BOOKS_ACTIVE.MODIFIED_TIMESTAMP, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, 
                         GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE
                        FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                    STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                    GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                    STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                        WHERE        (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = " + StoreID + ") AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE = '" + dDate + "')";
                _sql += " ORDER BY GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SL_NO";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objLottery = new LotteryActive();
                    objLottery.StoreID = StoreID;
                    objLottery.GameID = Convert.ToInt16(dr["LTACT_GAME_ID"]);
                    objLottery.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"].ToString());
                    objLottery.PackNo = dr["LTACT_PACK_NO"].ToString();
                    objLottery.Date = Convert.ToDateTime(dr["LTACT_DATE"].ToString());
                    objLottery.ShiftID = Convert.ToInt16(dr["LTACT_SHIFT_ID"].ToString());
                    objLottery.BoxNo = Convert.ToInt16(dr["LTACT_BOX_ID"].ToString());
                    objLottery.SlNo = Convert.ToInt16(dr["LTACT_SL_NO"].ToString());
                    objLottery.CreatedUserName = dr["CREATED_BY"].ToString();
                    objLottery.ModifiedUserName = dr["MODIFIED_BY"].ToString();
                    
                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
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

        public List<LotteryReturn> GetLotteryReturn(int StoreID, DateTime dDate)
        {
            try
            {
                List<LotteryReturn> objLotteryColl = new List<LotteryReturn>();
                LotteryReturn objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   LTRET_GAME_ID, LTRET_PACK_NO, LTRET_SHIFT_ID, LTRET_RETURN_FROM, LTRET_LAST_TICKET_NO, MODIFIED_BY, MODIFIED_TIMESTAMP
                            FROM            STORE_LOTTERY_RETURNS
                            WHERE        (LTRET_STORE_ID = PARMSTOREID) AND (LTRET_DATE = 'PARMDATE')
                            ORDER BY LTRET_RETURN_FROM";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE",dDate.ToString());

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objLottery = new LotteryReturn();
                    objLottery.StoreID = StoreID;
                    objLottery.GameID = dr["LTRET_GAME_ID"].ToString();
                    objLottery.PackNo = dr["LTRET_PACK_NO"].ToString();
                    objLottery.CreatedUserName = dr["MODIFIED_BY"].ToString();
                    objLottery.ReturnFrom = Convert.ToChar(dr["LTRET_RETURN_FROM"].ToString());
                    objLottery.LastTicketClosing = Convert.ToInt16(dr["LTRET_LAST_TICKET_NO"].ToString());
                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
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

        public List<BoxNumbers> GetLotteryBoxes(int ID)
        {
            try
            {
                List<BoxNumbers> objBoxColl = new List<BoxNumbers>();
                BoxNumbers objBox;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   LTBOX_STORE_ID, LTBOX_BOX_ID, LTBOX_BOX_DESCRIPTION
                            FROM            STORE_LOTTERY_BOX_MASTER";
                _sql += " WHERE        (LTBOX_STORE_ID = " + ID + ")";
                _sql += " ORDER BY LTBOX_BOX_ID";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objBox = new BoxNumbers();
                    objBox.StoreID = Convert.ToInt16(dr["LTBOX_STORE_ID"]);
                    objBox.BoxNo = Convert.ToInt16(dr["LTBOX_BOX_ID"]);
                    objBox.BoxDescription = dr["LTBOX_BOX_DESCRIPTION"].ToString();
                    objBoxColl.Add(objBox);
                }

                dr.Close();
                return objBoxColl;
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

        public BookReceive ScanBookReceive(int ID, string PackNo)
        {
            try
            {
                BookReceive objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                string[] sPack = PackNo.Split('-');
                int GameID = Convert.ToInt16(sPack[0]);
                PackNo = sPack[1];

                _sql = @"SELECT    MAPPING_STORE_LOTTERY.STORE_ID,  MAPPING_STORE_LOTTERY.GAME_ID, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, 
                         GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_FIRST_TICKET_NO, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO
                            FROM            MAPPING_STORE_LOTTERY INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON MAPPING_STORE_LOTTERY.GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID";
                _sql += " WHERE        (MAPPING_STORE_LOTTERY.ACTIVE = 'A') AND (MAPPING_STORE_LOTTERY.STORE_ID = " + ID + ")";
                _sql += " AND (MAPPING_STORE_LOTTERY.GAME_ID = " + GameID + ")";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                if (dr.Read())
                {
                    objLottery = new BookReceive();
                    objLottery.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objLottery.GameID = dr["GAME_ID"].ToString();
                    objLottery.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    objLottery.TicketValue = Convert.ToInt16(dr["LOTTERY_TICKET_VALUE"].ToString());
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"]);
                    objLottery.BookValue = Convert.ToInt16(dr["LOTTERY_BOOK_VALUE"].ToString());
                    objLottery.TicketStartNumber = Convert.ToInt16(dr["LOTTERY_BOOK_FIRST_TICKET_NO"].ToString());
                    objLottery.TicketEndNumber = Convert.ToInt16(dr["LOTTERY_BOOK_LAST_TICKET_NO"].ToString());
                    objLottery.PackNo = PackNo;
                    dr.Close();
                }
                else
                {
                    dr.Close();
                    throw new Exception("Game Number not found in Game Master.  Please contact admin to add Game");
                }
                return objLottery;
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

        public BookActive ScanBookActive(int ID, string PackNo)
        {
            try
            {
                BookActive objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                string[] sPack = PackNo.Split('-');
                int GameID = Convert.ToInt16(sPack[0]);
                int TicketNo = Convert.ToInt16(sPack[2]);
                PackNo = sPack[1];

                _sql = @"SELECT    STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO, 
                         STORE_LOTTERY_BOOKS_ACTIVE.LTACT_BOX_ID, GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, 
                         GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO
                            FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                     STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID
                                            AND STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE        (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = " + ID + ") AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = '" + GameID + "') AND ";
                _sql += " (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO = '" + PackNo + "')";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                if (dr.Read())
                {
                    if (Convert.ToInt16(dr["LOTTERY_BOOK_LAST_TICKET_NO"].ToString()) < TicketNo)
                    {
                        dr.Close();
                        throw new Exception("Lottery ticket number should not be exceed than game ending ticket number");
                    }
                    objLottery = new BookActive();
                    objLottery.StoreID = ID;
                    objLottery.GameID = dr["LTACT_GAME_ID"].ToString();
                    objLottery.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    objLottery.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"]);
                    objLottery.BoxNo = dr["LTACT_BOX_ID"].ToString();
                    objLottery.PackNo = PackNo;
                    objLottery.TicketEndNumber = TicketNo;                      
                    dr.Close();
                    objLottery.PrevTicketNumber = PrevTicketNumber(_conn, objLottery.StoreID, objLottery.GameID, objLottery.PackNo);
                }
                else
                {
                    dr.Close();
                    throw new Exception("Game Number not found in Book Active.  Please activate the book.");
                }
                return objLottery;
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

        private string PrevTicketNumber(SqlConnection _con, int StoreID, string GameID, string PackNo)
        {
            string prevTicketNumber = "0";
            _sql = @"SELECT    REPL_START_TICKET
                        FROM            REPORT_LOTTERY_DAILY
                        WHERE        (REPL_STORE_ID = PARMSTOREID) AND (REPL_END_TICKET IS NULL) AND (REPL_GAME_ID = 'PARMGAMEID') 
                        AND (REPL_PACK_NO = 'PARMPACKNO')";

            _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
            _sql = _sql.Replace("PARMGAMEID",  GameID.ToString());
            _sql = _sql.Replace("PARMPACKNO",  PackNo.ToString());
            
            SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
            SqlDataReader dr = sqlcmd.ExecuteReader();

            if(dr.Read())
            {
                prevTicketNumber = dr["REPL_START_TICKET"].ToString();
            }
            dr.Close();
            return prevTicketNumber;
        }

        private Int32 GetSlNoFromBox(int iStoreID, int iBoxNo)
        {
            Int32 iSlNo = 0;
            _sql = "SELECT MAX(LTACT_SL_NO) FROM STORE_LOTTERY_BOOKS_ACTIVE WHERE LTACT_STORE_ID = " + iStoreID + " AND LTACT_BOX_ID = " + iBoxNo ;
            SqlCommand cmd = new SqlCommand(_sql, _conn, _conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                iSlNo = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            else
                iSlNo = 1;

            return iSlNo;
        }

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

        public LotteryRunningShift GetRunningShift(int ID)
        {
            try
            {

                LotteryRunningShift objShift ;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   MAX(SALE_DATE) AS SALE_DATE, MAX(SALE_SHIFT_CODE) AS SALE_SHIFT_CODE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + ID + ") AND (LOTTERY_SHIFT_OPEN = 'OPEN')";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                if (dr.Read())
                {
                    objShift = new LotteryRunningShift();
                    objShift.StoreID = ID;
                    if (dr[0].ToString().Length > 0)
                    {
                        objShift.CurrentDate = Convert.ToDateTime(dr["SALE_DATE"]);
                        objShift.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                        dr.Close();

                        //int prevshift = Convert.ToInt16(objShift.ShiftCode) -1;

                        _sql = @"SELECT   SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE
                                    FROM            STORE_SALE_MASTER
                                    WHERE        (STORE_ID = " + ID + ") AND (SALE_DATE = '" + objShift.CurrentDate + "')";
                        _sql += " AND SALE_SHIFT_CODE = " + objShift.ShiftCode;
                        sqlcmd = new SqlCommand(_sql,_conn);
                        dr = sqlcmd.ExecuteReader();
                        if (dr.Read())
                        {
                            objShift.SystemOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE"]);
                        }
                        dr.Close();
//                        else
//                        {
//                            dr.Close();
//                            _sql = @"SELECT   TOP 1 SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE
//                                        FROM            STORE_SALE_MASTER
//                                        WHERE        (STORE_ID = " + ID + ") AND (SALE_DATE <=  '" + objShift.CurrentDate + "')";
//                            _sql += " AND SALE_SHIFT_CODE IN (SELECT MAX(SHFT_SHIFT_CODE) FROM SHIFT_CODES WHERE SHFT_STORE_ID = " + ID + ")";
//                            _sql += " ORDER BY SALE_DATE DESC";
//                            sqlcmd = new SqlCommand(_sql, _conn);
//                            dr = sqlcmd.ExecuteReader();
//                            if (dr.Read())
//                            {
//                                objShift.SystemOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE"]);
//                            }
//                            dr.Close();
//                        }
                    }
                    else
                    {
                        objShift.CurrentDate = DateTime.Now.Date;
                        objShift.ShiftCode = 1;
                        dr.Close();
                    }
                }
                else
                {
                    dr.Close();
                    objShift = new LotteryRunningShift();
                    objShift.StoreID = ID;
                    objShift.CurrentDate = DateTime.Now.Date;
                    objShift.ShiftCode = 1;
                    dr.Close();
                    //throw new Exception("No running shift is found, please contact IT support");
                }
                return objShift;
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

        public bool AddLotteryGame(LotteryGames objGame)
        {
            bool bResult = false;
            int iGroupID = 0;
            SqlCommand cmd;
            bool canRollback = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                DMLExecute dmlExecute = new DMLExecute();

                #region Adding Lottery Game

                _sql = "SELECT  STORE_GROUP_ID FROM            STORE_MASTER WHERE        (STORE_ID = " + objGame.StoreID + ")";

                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                    iGroupID = Convert.ToInt16(cmd.ExecuteScalar());
                else
                        throw new Exception("The Store ID doesn't exists");


                _sql = "SELECT  COUNT(*)  FROM   GROUP_LOTTERY_MASTER WHERE        (LOTTERY_GROUP_ID = " + iGroupID.ToString() + ") AND LOTTERY_GAME_ID = '" + objGame.GameID.ToString() + "'";

                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                    throw new Exception("The Game is already added");

                _sql = @"SELECT STORE_ID FROM  STORE_MASTER WHERE        (STORE_GROUP_ID = " + iGroupID + ")";
                SqlDataAdapter sda = new SqlDataAdapter(_sql, _conn);
                DataSet ds = new DataSet();
                sda.Fill(ds, "STORES");

                _conTran = _conn.BeginTransaction();
                canRollback = true;

                dmlExecute.AddFields("LOTTERY_GROUP_ID", iGroupID.ToString());
                dmlExecute.AddFields("LOTTERY_GAME_ID", objGame.GameID.ToString());
                dmlExecute.AddFields("LOTTERY_GAME_DESCRIPTION", objGame.GameName.ToString());
                dmlExecute.AddFields("LOTTERY_NO_OF_TICKETS", objGame.NoOfTickets.ToString());
                dmlExecute.AddFields("LOTTERY_TICKET_VALUE", objGame.TicketValue.ToString());
                dmlExecute.AddFields("LOTTERY_BOOK_FIRST_TICKET_NO", objGame.TicketStartNumber.ToString());
                dmlExecute.AddFields("LOTTERY_BOOK_LAST_TICKET_NO", objGame.TicketEndNumber.ToString());
                dmlExecute.AddFields("LOTTERY_BOOK_VALUE", objGame.BookValue.ToString());

                dmlExecute.ExecuteInsert("GROUP_LOTTERY_MASTER", _conn, _conTran);

                for (int i = 0; i < ds.Tables["STORES"].Rows.Count; i++)
                {
                    dmlExecute.AddFields("STORE_ID", ds.Tables["STORES"].Rows[i]["STORE_ID"].ToString());
                    dmlExecute.AddFields("GAME_ID", objGame.GameID.ToString());
                    dmlExecute.AddFields("ACTIVE", "A");
                    dmlExecute.ExecuteInsert("MAPPING_STORE_LOTTERY", _conn, _conTran);
                }

                _sql = "SELECT  COUNT(*)  FROM   STATE_LOTTERY_GAMES WHERE   LOTTERY_GAME_ID = '" + objGame.GameID.ToString() + "'";
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (Convert.ToInt16(cmd.ExecuteScalar()) == 0)
                {
                    dmlExecute.AddFields("LOTTERY_STATE_ID", "TN");
                    dmlExecute.AddFields("LOTTERY_GAME_ID", objGame.GameID.ToString());
                    dmlExecute.AddFields("LOTTERY_GAME_DESCRIPTION", objGame.GameName.ToString());
                    dmlExecute.AddFields("LOTTERY_NO_OF_TICKETS", objGame.NoOfTickets.ToString());
                    dmlExecute.AddFields("LOTTERY_TICKET_VALUE", objGame.TicketValue.ToString());
                    dmlExecute.AddFields("LOTTERY_BOOK_FIRST_TICKET_NO", objGame.TicketStartNumber.ToString());
                    dmlExecute.AddFields("LOTTERY_BOOK_LAST_TICKET_NO", objGame.TicketEndNumber.ToString());
                    dmlExecute.AddFields("LOTTERY_BOOK_VALUE", objGame.BookValue.ToString());

                    dmlExecute.ExecuteInsert("STATE_LOTTERY_GAMES", _conn, _conTran);
                }

                _conTran.Commit();
                bResult = true;

                #endregion
                return bResult;
            }
            catch (Exception ex)
            {
                if (canRollback == true)
                    _conTran.Rollback();
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
        }

        public List<AutoSettle> SelectAutoSettle(int ID)
        {
            try
            {
                List<AutoSettle> objLotteryColl = new List<AutoSettle>();
                AutoSettle objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO, 
                         STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_DATE, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, 
                         STATE_CONFIGURATION.STATE_LOTTERY_INSTANT_COMMISSION, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE - GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE * (STATE_CONFIGURATION.STATE_LOTTERY_INSTANT_COMMISSION
                          / 100) AS SETTLE_AMOUNT
                        FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                 STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                 STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID INNER JOIN
                                                 STATE_CONFIGURATION ON STORE_MASTER.STATE_ID = STATE_CONFIGURATION.STATE_ID
                        WHERE        (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_STATUS = 'Y') AND 
                                                 (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SWEAP_STATUS = 'N')
                        ORDER BY STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_DATE";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                float fTotalBookAmount = 0;
                float fTotalSettleAmount = 0;
                while (dr.Read())
                {
                    objLottery = new AutoSettle();
                    objLottery.GameID = dr["LTACT_GAME_ID"].ToString();
                    objLottery.PackNo = dr["LTACT_PACK_NO"].ToString();
                    objLottery.AutoSettleDate = Convert.ToDateTime(dr["LTACT_AUTO_SETTLE_DATE"].ToString());
                    objLottery.BookAmount = Convert.ToSingle(dr["LOTTERY_BOOK_VALUE"].ToString());
                    objLottery.SettleAmount = Convert.ToSingle(dr["SETTLE_AMOUNT"]);

                    fTotalBookAmount += objLottery.BookAmount;
                    fTotalSettleAmount += objLottery.SettleAmount;
                    objLotteryColl.Add(objLottery);
                }

                objLottery = new AutoSettle();
                objLottery.PackNo = "Total";
                objLottery.BookAmount = fTotalBookAmount;
                objLottery.SettleAmount = fTotalSettleAmount;
                objLotteryColl.Add(objLottery);


                dr.Close();
                return objLotteryColl;
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

        public List<BooksActiveButNotSettle> SelectBooksActiveButNotSettled(int ID)
        {
            try
            {
                List<BooksActiveButNotSettle> objLotteryColl = new List<BooksActiveButNotSettle>();
                BooksActiveButNotSettle objLottery;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO, 
                                                     STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE - GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE * STATE_CONFIGURATION.STATE_LOTTERY_INSTANT_COMMISSION
                                                      / 100 AS SETTLE_AMOUNT
                            FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                     STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                     STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID INNER JOIN
                                                     STATE_CONFIGURATION ON STORE_MASTER.STATE_ID = STATE_CONFIGURATION.STATE_ID
                            WHERE        (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_STATUS = 'N')
                            ORDER BY STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    objLottery = new BooksActiveButNotSettle();
                    objLottery.GameID = dr["LTACT_GAME_ID"].ToString();
                    objLottery.PackNo = dr["LTACT_PACK_NO"].ToString();
                    objLottery.Date = Convert.ToDateTime(dr["LTACT_DATE"].ToString());
                    objLottery.BookAmount = Convert.ToSingle(dr["LOTTERY_BOOK_VALUE"].ToString());
                    objLottery.SettleAmount = Convert.ToSingle(dr["SETTLE_AMOUNT"]);

                    objLotteryColl.Add(objLottery);
                }

                dr.Close();
                return objLotteryColl;
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

        public bool UpdatePayment(AutoSettlePayment objLottery)
        {
            bool bResult = false;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Update Lottery Payments

                foreach (AutoSettlePaymentBooks obj in objLottery.AutoSettlePaymentBooks)
                {
                    dmlExecute.AddFields("LTACT_SWEAP_STATUS", "Y");
                    dmlExecute.AddFields("LTACT_SWEAP_DATE", Convert.ToDateTime(objLottery.PaymentSweapDate).ToString());
                    dmlExecute.AddFields("LT_ACT_BUSINESS_ENDING_DATE", Convert.ToDateTime(objLottery.BusinessEndingDate).ToString());
                    dmlExecute.AddFields("LTACT_PACK_NO", obj.PackNo.ToString());
                    dmlExecute.AddFields("LTACT_STORE_ID", objLottery.StoreID.ToString());
                    dmlExecute.AddFields("LTACT_GAME_ID", obj.GameID);

                    string[] KeyFields = { "LTACT_STORE_ID", "LTACT_GAME_ID","LTACT_PACK_NO" };
                    dmlExecute.ExecuteUpdate("STORE_LOTTERY_BOOKS_ACTIVE", _conn, KeyFields, _conTran);
                }

                if (objLottery.TotalPayment > 0)
                {
                    // Lottery Dealer A/c Dr To Lottery Bank A/c
                    bAccountSameVoucher = false;
                    UpdateAccountEntry(objLottery.StoreID, "DR", 11, objLottery.PaymentSweapDate, objLottery.TotalPayment, "Paid to Lottery Dealer", 0, "LP");
                    UpdateAccountEntry(objLottery.StoreID, "CR", 24, objLottery.PaymentSweapDate, objLottery.TotalPayment, "Paid to Lottery Dealer", 0, "LP");
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

        public PaymentDueModel GetLotteryPaymentDue(int ID)
        {
            PaymentDueModel objPaymentDueModel = new PaymentDueModel();
            List<AutoSettle> objAutoSettleColl = new List<AutoSettle>(); ;
            AutoSettle objAutoSettle;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                DateTime dtPreviousBusinessEnd;

                _sql = @"SELECT ISNULL(MAX(LT_ACT_BUSINESS_ENDING_DATE),'01-JAN-2000') FROM STORE_LOTTERY_BOOKS_ACTIVE 
                                WHERE LTACT_STORE_ID = PARMSTOREID";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                dtPreviousBusinessEnd = Convert.ToDateTime(sqlcmd.ExecuteScalar());

                _sql = @"SELECT STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO, 
                                                     STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_DATE, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE
                            FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                     STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                     STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE        (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_DATE > 'PARMPREVIOUSBUSINESSENDDATE') 
                            AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_STATUS = 'Y')
                            ORDER BY STORE_LOTTERY_BOOKS_ACTIVE.LTACT_AUTO_SETTLE_DATE";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                _sql = _sql.Replace("PARMPREVIOUSBUSINESSENDDATE", dtPreviousBusinessEnd.ToString());

                sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objAutoSettle = new AutoSettle();
                    objAutoSettle.GameID = dr["LTACT_GAME_ID"].ToString();
                    objAutoSettle.PackNo = dr["LTACT_PACK_NO"].ToString();
                    objAutoSettle.BookAmount = Convert.ToSingle(dr["LOTTERY_BOOK_VALUE"].ToString());
                    objAutoSettle.AutoSettleDate = Convert.ToDateTime(dr["LTACT_AUTO_SETTLE_DATE"].ToString());
                    objPaymentDueModel.TotalBooksActive += objAutoSettle.BookAmount;
                    objAutoSettleColl.Add(objAutoSettle);
                }
                dr.Close();
                objPaymentDueModel.SettledBooks = objAutoSettleColl;


                #region Adding Master Entries
                _sql = @"SELECT  SUM(SALE_LOTTERY_RETURN) AS SALE_LOTTERY_RETURN, SUM(SALE_LOTTERY_ONLINE) AS SALE_LOTTERY_ONLINE, 
                                                     SUM(SALE_LOTTERY_CASH_INSTANT_PAID) AS SALE_LOTTERY_CASH_INSTANT_PAID, SUM(SALE_LOTTERY_CASH_ONLINE_PAID) 
                                                     AS SALE_LOTTERY_CASH_ONLINE_PAID, SUM(SALE_LOTTERY_ONLINE_COMMISSION) AS SALE_LOTTERY_ONLINE_COMMISSION, 
                                                     SUM(SALE_LOTTER_INSTANT_COMMISSION) AS SALE_LOTTER_INSTANT_COMMISSION, SUM(SALE_LOTTERY_CASH_COMMISSION) 
                                                     AS SALE_LOTTERY_CASH_COMMISSION
                            FROM            STORE_SALE_MASTER
                            WHERE        (SALE_DATE > 'PARMPREVIOUSBUSINESSENDDATE') AND (STORE_ID = PARMSTOREID)";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                _sql = _sql.Replace("PARMPREVIOUSBUSINESSENDDATE", dtPreviousBusinessEnd.ToString());

                sqlcmd = new SqlCommand(_sql, _conn);
                dr = sqlcmd.ExecuteReader();
                if (dr.Read())
                {
                    objPaymentDueModel.StoreID = ID;
                    objPaymentDueModel.PreviousBusinessEndedDate = dtPreviousBusinessEnd;
                    objPaymentDueModel.OnlineSale = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"].ToString());
                    objPaymentDueModel.InstantCommission = Convert.ToSingle(dr["SALE_LOTTER_INSTANT_COMMISSION"].ToString());
                    objPaymentDueModel.SaleCommission = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE_COMMISSION"].ToString());
                    objPaymentDueModel.CashCommission = Convert.ToSingle(dr["SALE_LOTTERY_CASH_COMMISSION"].ToString());
                    objPaymentDueModel.InstantPaid = Convert.ToSingle(dr["SALE_LOTTERY_CASH_INSTANT_PAID"].ToString());
                    objPaymentDueModel.OnlinePaid = Convert.ToSingle(dr["SALE_LOTTERY_CASH_ONLINE_PAID"].ToString());
                    objPaymentDueModel.TotalDueAmount = objPaymentDueModel.OnlineSale + objPaymentDueModel.TotalBooksActive;
                    objPaymentDueModel.TotalDueAmount = objPaymentDueModel.TotalDueAmount - objPaymentDueModel.InstantCommission - objPaymentDueModel.SaleCommission;
                    objPaymentDueModel.TotalDueAmount = objPaymentDueModel.TotalDueAmount - objPaymentDueModel.CashCommission - objPaymentDueModel.InstantPaid;
                    objPaymentDueModel.TotalDueAmount = objPaymentDueModel.TotalDueAmount - objPaymentDueModel.OnlinePaid - objPaymentDueModel.LotteryReturn;
                }
                dr.Close();

                _sql = @"SELECT  SUM(LTRET_CREDIT_AMOUNT) AS LTRET_CREDIT_AMOUNT
                            FROM            STORE_LOTTERY_RETURNS
                            WHERE        (LTRET_STORE_ID = PARMSTOREID) AND (LTRET_DATE > 'PARMPREVIOUSBUSINESSENDDATE') AND (LTRET_RETURN_FROM = 'A')";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                _sql = _sql.Replace("PARMPREVIOUSBUSINESSENDDATE", dtPreviousBusinessEnd.ToString());
                sqlcmd = new SqlCommand(_sql, _conn);
                if (sqlcmd.ExecuteScalar().ToString().Length > 0)
                    objPaymentDueModel.LotteryReturn = Convert.ToSingle(sqlcmd.ExecuteScalar());


                #endregion

                return objPaymentDueModel;
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

    }
}
