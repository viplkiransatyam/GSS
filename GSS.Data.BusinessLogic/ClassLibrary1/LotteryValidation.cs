using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSS.DataAccess.Layer;

namespace GSS.BusinessLogic.Layer
{
    public class LotteryValidation    
    {
        public static string con = System.Configuration.ConfigurationManager.ConnectionStrings["StoreSoftConnection"].ToString();
        string _sql = "";

        public bool LotteryClosing(SaleMaster objLotterySale)
        {
            List<LotterySale> objLotteryShiftExists = new List<LotterySale>();
            LotterySale objLotteryShift ;

            try
            {
                using (SqlConnection _conn = new SqlConnection(LotteryValidation.con))
                {
                    _conn.Open();
                    SqlCommand cmd;
                    SqlDataReader dr;

                    if (!GetLotteryShiftStatus(objLotterySale.Date, objLotterySale.ShiftCode, objLotterySale.StoreID, _conn))
                    {
                        throw new Exception("Lottery Shift is closed and the entries cannot be edited");
                    }

                    #region The lottery book should exists in current shift

                    _sql = @"SELECT REPORT_LOTTERY_DAILY.REPL_BOX_ID, REPORT_LOTTERY_DAILY.REPL_GAME_ID, REPORT_LOTTERY_DAILY.REPL_PACK_NO
                                    FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                    STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                    REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                                    WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = " + objLotterySale.StoreID + ") AND ";
                    _sql += " (STORE_SALE_MASTER.SALE_DATE = '" + objLotterySale.Date + "') AND (SALE_SHIFT_CODE = " + objLotterySale.ShiftCode + ")  AND ";
                    _sql += " (REPORT_LOTTERY_DAILY.REPL_END_TICKET IS NULL)";
                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        objLotteryShift = new LotterySale();
                        objLotteryShift.BoxID = (int) dr["REPL_BOX_ID"];
                        objLotteryShift.PackNo = dr["REPL_PACK_NO"].ToString();
                        objLotteryShift.GameID = dr["REPL_GAME_ID"].ToString();
                        objLotteryShiftExists.Add(objLotteryShift);
                    }
                    dr.Close();

                    #endregion

                    #region Lottery Reading Validation

                    foreach (LotterySale objLottery in objLotterySale.LotteryClosingCount)
                    {
                        if (objLottery.GameID == null)
                            objLottery.GameID = "";

                        if (objLottery.GameID.Trim().Length > 0)
                        {
                            _sql = @"SELECT  COUNT(1) 
                            FROM            STORE_LOTTERY_BOOKS_ACTIVE 
                            WHERE      LTACT_STORE_ID = '" + objLotterySale.StoreID.ToString() + "'  AND ";
                            _sql += " (LTACT_GAME_ID = '" + objLottery.GameID.ToString() + "') AND (LTACT_BOX_ID = " + objLottery.BoxID + ") AND LTACT_PACK_NO = '" + objLottery.PackNo + "'";

                            cmd = new SqlCommand(_sql, _conn);
                            dr = cmd.ExecuteReader();
                            dr.Read();
                            if (Convert.ToInt16(dr[0]) == 0)
                            {
                                dr.Close();
                                throw new Exception("This book " + objLottery.PackNo + " is not activated at Box " + objLottery.BoxID);
                            }
                            dr.Close();


                            // Commeting below code to support any book can be scanned any box
                            //_sql = @"SELECT        TOP (1) LTACT_GAME_ID, LTACT_PACK_NO
                            //    FROM            STORE_LOTTERY_BOOKS_ACTIVE
                            //    WHERE        (LTACT_STORE_ID = " + objLotterySale.StoreID + ") AND (LTACT_BOX_ID = " + objLottery.BoxID + ")";
                            //_sql += " ORDER BY LTACT_SL_NO DESC";
                            //cmd = new SqlCommand(_sql, _conn);
                            //dr = cmd.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    if ((dr[0].ToString() != objLottery.GameID) || (dr[1].ToString() != objLottery.PackNo))
                            //    {
                            //        dr.Close();
                            //        throw new Exception("This lottery book " + objLottery.PackNo + " is not last book that is activated for Game " + objLottery.GameID + " at Box " + objLottery.BoxID);
                            //    }
                            //}
                            //dr.Close();

                            objLotteryShift = objLotteryShiftExists.Find(x => x.PackNo == objLottery.PackNo && x.GameID == objLottery.GameID);
                            if (objLotteryShift == null)
                            {
                                throw new Exception("This Closing reading has not taken for this lottery book " + objLottery.PackNo + " Game " + objLottery.GameID + " at Box " + objLottery.BoxID );
                            }

                            _sql = @"SELECT MAX(R.REPL_END_TICKET)
                                    FROM            REPORT_LOTTERY_DAILY R 
                                    WHERE        (R.REPL_STORE_ID = PARMSTOREID) AND 
									R.REPL_GAME_ID = PARMGAMENO AND R.REPL_PACK_NO = 'PARMPACKNO'";

                            _sql = _sql.Replace("PARMSTOREID", objLotterySale.StoreID.ToString());
                            _sql = _sql.Replace("PARMPACKNO", objLottery.PackNo);
                            _sql = _sql.Replace("PARMGAMENO", objLottery.GameID);

                            cmd = new SqlCommand(_sql, _conn);
                            int iTemp;
                            if (cmd.ExecuteScalar().ToString().Length > 0)
                            {
                                iTemp = Convert.ToInt16(cmd.ExecuteScalar().ToString());
                                if (objLottery.LastTicketClosing != 0)
                                {
                                    if (objLottery.LastTicketClosing < iTemp)
                                    {
                                        throw new Exception("Previous scanned ticket is " + iTemp + " and the current scanned ticket should be later previous ticket for Box No." + objLottery.BoxID + " and Game ID " + objLottery.GameID);
                                    }
                                }
                            }
                        }
                    }


                    #endregion

                    #region Check whether all books scanned 

                    foreach (LotterySale objLottery in objLotteryShiftExists)
                    {
                        objLotteryShift = objLotterySale.LotteryClosingCount.Find(x => x.GameID == objLottery.GameID && x.PackNo == objLottery.PackNo);
                        
                        if (objLotteryShift == null)
                        {
                            throw new Exception("This Closing reading has not taken for this lottery book " + objLottery.PackNo + " Game " + objLottery.GameID);
                        }
                    }


                    #endregion

                    _conn.Dispose();
                    _conn.Close();
                    
                    SqlConnection.ClearPool(_conn);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LotteryClosingInstantSale GetEmptyBoxes(SaleMaster objLotterySale)
        {
            LotteryClosingInstantSale objClosingInstantSale = new LotteryClosingInstantSale();
            BoxNumbersEmptyBoxes objBox;
            List<BoxNumbersEmptyBoxes> objBoxNumbers = new List<BoxNumbersEmptyBoxes>();
            
            List<LotterySale> objLotteryShiftExists = new List<LotterySale>();
            LotterySale objLotteryShift;

            try
            {
                using (SqlConnection _conn = new SqlConnection(LotteryValidation.con))
                {
                    _conn.Open();
                    SqlCommand cmd;
                    SqlDataReader dr;
                    SqlTransaction _conTran;
                    int i = 0;
                    _conTran = _conn.BeginTransaction();
                    DMLExecute dmlExecute = new DMLExecute();
                    _sql = "DELETE FROM LOTTERY_TEMPORARY_CLOSING_READING WHERE TMPCLS_STORE_ID = " + objLotterySale.StoreID;
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);

                    foreach (LotterySale objLottery in objLotterySale.LotteryClosingCount)
                    {
                        if (objLottery.GameID != null)
                        {
                            if ((objLottery.GameID.Trim().Length > 0) && (objLottery.NoOfTickets > 0))
                            {
                                i++;
                                dmlExecute.AddFields("TMPCLS_STORE_ID", objLotterySale.StoreID.ToString());
                                dmlExecute.AddFields("TMPCLS_GAME_ID", objLottery.GameID.ToString());
                                dmlExecute.AddFields("TMPCLS_BOX_NO", objLottery.BoxID.ToString());
                                dmlExecute.AddFields("TMPCLS_PACK_NO", objLottery.PackNo.ToString());
                                dmlExecute.AddFields("TMPCLS_SLNO", i.ToString());
                                dmlExecute.AddFields("TMPCLS_NO_TICKETS", objLottery.NoOfTickets.ToString());
                                dmlExecute.AddFields("TMPCLS_LAST_TICKET_NO", objLottery.LastTicketClosing.ToString());
                                dmlExecute.ExecuteInsert("LOTTERY_TEMPORARY_CLOSING_READING", _conn, _conTran);
                            }
                        }
                    }
                    _conTran.Commit();


                    #region The lottery book should exists in current shift

                    _sql = @"SELECT REPORT_LOTTERY_DAILY.REPL_BOX_ID, REPORT_LOTTERY_DAILY.REPL_GAME_ID, REPORT_LOTTERY_DAILY.REPL_PACK_NO,
                                    STORE_SALE_MASTER.SALE_LOTTERY_CASH_TOTAL_TRANSFER
                                    FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                    STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                    REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                                    WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = " + objLotterySale.StoreID + ") AND ";
                    _sql += " (STORE_SALE_MASTER.SALE_DATE = '" + objLotterySale.Date + "') AND (SALE_SHIFT_CODE = " + objLotterySale.ShiftCode + ")  AND ";
                    _sql += " (REPORT_LOTTERY_DAILY.REPL_END_TICKET IS NULL)";
                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        if (objClosingInstantSale.CashTransfer == 0)
                            objClosingInstantSale.CashTransfer = Convert.ToSingle(dr["SALE_LOTTERY_CASH_TOTAL_TRANSFER"].ToString());
                        objLotteryShift = new LotterySale();
                        objLotteryShift.BoxID = (int)dr["REPL_BOX_ID"];
                        objLotteryShift.PackNo = dr["REPL_PACK_NO"].ToString();
                        objLotteryShift.GameID = dr["REPL_GAME_ID"].ToString();
                        objLotteryShiftExists.Add(objLotteryShift);
                    }
                    dr.Close();

                    #endregion

                    #region Lottery Reading Validation

                    foreach (LotterySale objLottery in objLotterySale.LotteryClosingCount)
                    {
                        if (objLottery.GameID == null)
                            objLottery.GameID = "";

                        if (objLottery.GameID.Trim().Length > 0)
                        {
                            _sql = @"SELECT MAX(R.REPL_END_TICKET)
                                FROM            REPORT_LOTTERY_DAILY R 
                                WHERE        (R.REPL_STORE_ID = PARMSTOREID) AND 
								R.REPL_GAME_ID = PARMGAMENO AND R.REPL_PACK_NO = 'PARMPACKNO'";

                            _sql = _sql.Replace("PARMSTOREID", objLotterySale.StoreID.ToString());
                            _sql = _sql.Replace("PARMPACKNO", objLottery.PackNo);
                            _sql = _sql.Replace("PARMGAMENO", objLottery.GameID);

                            cmd = new SqlCommand(_sql, _conn);
                            int iTemp;
                            if (cmd.ExecuteScalar().ToString().Length > 0)
                            {
                                iTemp = Convert.ToInt16(cmd.ExecuteScalar().ToString());
                                if (objLottery.LastTicketClosing != 0)
                                {
                                    if (objLottery.LastTicketClosing < iTemp)
                                    {
                                        throw new Exception("Previous scanned ticket is " + iTemp + " and the current scanned ticket should be later previous ticket for Box No." + objLottery.BoxID + " and Game ID " + objLottery.GameID + " and pack no " + objLottery.PackNo);
                                    }
                                }
                            }
                        }
                    }


                    #endregion

                    #region Check whether all books scanned

                    foreach (LotterySale objLottery in objLotteryShiftExists)
                    {
                        objLotteryShift = objLotterySale.LotteryClosingCount.Find(x => x.GameID == objLottery.GameID && x.PackNo == objLottery.PackNo);
                        if (objLotteryShift == null)
                        {
                            objBox = new BoxNumbersEmptyBoxes();
                            objBox.BoxID = objLottery.BoxID;
                            objBox.StoreID = objLotterySale.StoreID;
                            objBox.GameID = objLottery.GameID;
                            objBox.PackNo = objLottery.PackNo;
                            objBoxNumbers.Add(objBox);
                        }
                    }
                    objClosingInstantSale.BoxNumbers = objBoxNumbers;

                    #endregion

                    #region Calculate Instant Sale
                    float fInstantSale = 0;
                    SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();
                    objLotterySale.DayTranID = _SaleSupportEntries.DayTranID(objLotterySale.Date, objLotterySale.StoreID, objLotterySale.ShiftCode, _conn);

                    // Reading existing sale excluding the closing reading books
                    _sql = @"SELECT   SUM(((REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET)) 
                               * GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE) AS Sold
                            FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                                     STORE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                     REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = " + objLotterySale.StoreID + ") AND REPL_END_TICKET IS NOT NULL AND ";
                    _sql += " (REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = " + objLotterySale.DayTranID + ")";

                    cmd = new SqlCommand(_sql, _conn);
                    if (cmd.ExecuteScalar().ToString().Length > 0)
                    {
                        fInstantSale = Convert.ToSingle(cmd.ExecuteScalar());
                    }

                    _sql = @"SELECT   REPORT_LOTTERY_DAILY.REPL_BOX_ID, REPORT_LOTTERY_DAILY.REPL_GAME_ID, REPORT_LOTTERY_DAILY.REPL_PACK_NO, 
                                 REPORT_LOTTERY_DAILY.REPL_START_TICKET, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE, 
                                 GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS
                            FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                                     STORE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID AND 
                                                     STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID
                            WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = PARMSTOREID) AND (REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = PARMDAYTRANID) AND 
                                                     (REPORT_LOTTERY_DAILY.REPL_END_TICKET IS NULL)";
                    _sql = _sql.Replace("PARMSTOREID", objLotterySale.StoreID.ToString());
                    _sql = _sql.Replace("PARMDAYTRANID", objLotterySale.DayTranID.ToString());
                    objLotteryShiftExists.Clear();
                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        objLotteryShift = new LotterySale();
                        objLotteryShift.BoxID = (int)dr["REPL_BOX_ID"];
                        objLotteryShift.PackNo = dr["REPL_PACK_NO"].ToString();
                        objLotteryShift.GameID = dr["REPL_GAME_ID"].ToString();
                        objLotteryShift.LastTicketClosing = Convert.ToInt16(dr["REPL_START_TICKET"].ToString());
                        objLotteryShift.EachTicketValue = Convert.ToInt16(dr["LOTTERY_TICKET_VALUE"].ToString());
                        objLotteryShift.NoOfTickets = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"].ToString());
                        objLotteryShiftExists.Add(objLotteryShift);
                    }
                    dr.Close();

                    //Calculating instant sale from scanned books 
                    foreach (LotterySale objLottery in objLotterySale.LotteryClosingCount)
                    {
                        if (objLottery.GameID.Trim().Length > 0)
                        {
                            objLotteryShift = objLotteryShiftExists.Find(x => x.GameID == objLottery.GameID && x.PackNo == objLottery.PackNo);
                            if (objLotteryShift != null)
                            {
                                if ((objLottery.LastTicketClosing == 0) && (objLottery.NoOfTickets == 0)) //Lottery books that are not scanned were treated as sold fully
                                    fInstantSale += (objLotteryShift.NoOfTickets - objLotteryShift.LastTicketClosing) * objLotteryShift.EachTicketValue;
                                else
                                    fInstantSale += (objLottery.LastTicketClosing - objLotteryShift.LastTicketClosing) * objLotteryShift.EachTicketValue;
                            }
                        }
                        else
                        {
                            throw new Exception("No Game found");
                            objLotteryShift = objLotteryShiftExists.Find(x => x.BoxID == objLottery.BoxID);
                            fInstantSale += (objLotteryShift.NoOfTickets - objLottery.LastTicketClosing) * objLotteryShift.EachTicketValue;
                        }
                    }

                    objClosingInstantSale.InstantSale = fInstantSale;
                    #endregion

                    _conn.Dispose();
                    _conn.Close();

                    SqlConnection.ClearPool(_conn);
                    return objClosingInstantSale;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool BookActive(List<LotteryActive> objLotteryBooksActive)
        {
            try
            {
                bool alreadyCheck = false;
                using (SqlConnection _conn = new SqlConnection(LotteryValidation.con))
                {
                    _conn.Open();
                    SqlCommand cmd;
                    SqlDataReader dr;

                    string sReturnCheckGameID = string.Empty;
                    string sReturnCheckPackNo = string.Empty;

                    foreach(LotteryActive obj in objLotteryBooksActive)
                    {
                        if (alreadyCheck == false)
                        {
                            alreadyCheck = true;
                            if (!GetLotteryShiftStatus(obj.Date, obj.ShiftID, obj.StoreID, _conn))
                            {
                                throw new Exception("Lottery Shift is closed or Previous shift is not closed and the entries cannot be edited");
                            }
                        }

                        #region Previous shift should be closed



                        #endregion


                        #region Expecting the box to be in current shift.  The below code is commented to allow to maintain empty boxes
                        // Checking whether shift exists or not
//                        _sql = @"SELECT  COUNT(STORE_SALE_MASTER.SALE_DATE)
//                                    FROM            REPORT_LOTTERY_DAILY INNER JOIN
//                                                             STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
//                                                             REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
//                                    WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = PARMSTORE) AND (REPORT_LOTTERY_DAILY.REPL_BOX_ID = PARMBOXID) AND 
//                                                             (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE') 
//                                                    AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = PARMSHIFTCODE) AND 
//                                                    (REPORT_LOTTERY_DAILY.REPL_END_TICKET IS NULL)";

//                        _sql = _sql.Replace("PARMSTORE", obj.StoreID.ToString());
//                        _sql = _sql.Replace("PARMBOXID", obj.BoxNo.ToString());
//                        _sql = _sql.Replace("PARMDATE", obj.Date.ToString());
//                        _sql = _sql.Replace("PARMSHIFTCODE", obj.ShiftID.ToString());
//                        cmd = new SqlCommand(_sql, _conn);

//                        if (Convert.ToInt16(cmd.ExecuteScalar()) <= 0)
//                        {
//                            // Checking whether the latest is book is returned.  If it is retunred, the system should allow to active another book
//                            _sql = @"SELECT  TOP 1 REPL_GAME_ID, REPL_PACK_NO
//	                                    FROM            REPORT_LOTTERY_DAILY INNER JOIN
//								                                    STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
//								                                    REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
//	                                    WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = PARMSTORE) AND (REPORT_LOTTERY_DAILY.REPL_BOX_ID = PARMBOXID) 
//                                    ORDER BY STORE_SALE_MASTER.SALE_DATE DESC, STORE_SALE_MASTER.SALE_SHIFT_CODE DESC";

//                            _sql = _sql.Replace("PARMSTORE", obj.StoreID.ToString());
//                            _sql = _sql.Replace("PARMBOXID", obj.BoxNo.ToString());
//                            cmd = new SqlCommand(_sql, _conn);
//                            dr = cmd.ExecuteReader();
//                            if (dr.Read())
//                            {
//                                sReturnCheckGameID = dr["REPL_GAME_ID"].ToString();
//                                sReturnCheckPackNo = dr["REPL_PACK_NO"].ToString();
//                            }
//                            dr.Close();

//                            _sql = @"SELECT        COUNT(1) AS Expr1
//                                        FROM            STORE_LOTTERY_RETURNS
//                                        WHERE        (LTRET_STORE_ID = PARMSTORE) AND (LTRET_GAME_ID = 'PARMGAMEID') AND (LTRET_PACK_NO = 'PARMPACKNO')";

//                            _sql = _sql.Replace("PARMSTORE", obj.StoreID.ToString());
//                            _sql = _sql.Replace("PARMGAMEID", sReturnCheckGameID);
//                            _sql = _sql.Replace("PARMPACKNO", sReturnCheckPackNo);

//                            cmd = new SqlCommand(_sql, _conn);
//                            if (Convert.ToInt16(cmd.ExecuteScalar()) <= 0)
//                            {
//                                // Checking is this first transaction for this box
//                                _sql = @"SELECT   count( STORE_SALE_MASTER.SALE_DATE)
//                                    FROM            REPORT_LOTTERY_DAILY INNER JOIN
//                                                             STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
//                                                             REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
//                                    WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = PARMSTORE) AND (REPORT_LOTTERY_DAILY.REPL_BOX_ID = PARMBOXID)";

//                                _sql = _sql.Replace("PARMSTORE", obj.StoreID.ToString());
//                                _sql = _sql.Replace("PARMBOXID", obj.BoxNo.ToString());
//                                cmd = new SqlCommand(_sql, _conn);
//                                if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
//                                {
//                                    throw new Exception("Previous shift closing reading is required before book is activated for next shift");
//                                }
//                            }
//                        }
                        #endregion
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool GetLotteryShiftStatus(DateTime dDate, int shiftCode, int iStoreID, SqlConnection con)
        {

            bool bReturn = false;
            SqlCommand cmd ;
            SqlDataReader dr;

            _sql = @"SELECT    count(1)  FROM REPORT_LOTTERY_DAILY WHERE REPL_STORE_ID = PARMSTOREID";
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            cmd = new SqlCommand(_sql, con);
            if (Convert.ToInt64(cmd.ExecuteScalar()) <= 0)
            {
                bReturn = true;
            }

            if (bReturn == false)
            {
                _sql = @"SELECT    LOTTERY_SHIFT_OPEN FROM STORE_SALE_MASTER WHERE STORE_ID = PARMSTOREID AND SALE_DATE = 'PARMDATE' AND SALE_SHIFT_CODE = PARMSHIFTCODE";

                _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("PARMDATE", dDate.ToString());
                _sql = _sql.Replace("PARMSHIFTCODE", shiftCode.ToString());

                cmd = new SqlCommand(_sql, con);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["LOTTERY_SHIFT_OPEN"].ToString() == "OPEN")
                        bReturn = true;
                    else
                        bReturn = false;
                }
                else
                {
                    bReturn = false;
                }
                dr.Close();
            }

            return bReturn;

        }

    }
}
