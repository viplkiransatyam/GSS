using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GSS.DataAccess.Layer
{
    public class ReportLottery
    {
        SqlConnection _conn;
        string _sql;

        public List<ReportLotteryModel> DetailedSaleReport(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReportLotteryModel> SaleReport = new List<ReportLotteryModel>();
            ReportLotteryModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Preparing Detailed Sale Report

                _sql = @"SELECT  STORE_SALE_MASTER.SALE_DATE, STORE_SALE_MASTER.SALE_SHIFT_CODE, REPORT_LOTTERY_DAILY.REPL_PACK_NO, REPORT_LOTTERY_DAILY.REPL_GAME_ID, 
                             GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE, REPORT_LOTTERY_DAILY.REPL_BOX_ID, 
                             REPORT_LOTTERY_DAILY.REPL_START_TICKET, REPORT_LOTTERY_DAILY.REPL_END_TICKET, 
                             (REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET)  AS NoOfTicketsSold, 
                             GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE * ((REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET))
                             AS AmountSold,
                                case REPORT_LOTTERY_DAILY.REPL_END_TICKET 
                                when GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO then 'Sold Out' else 'Continuing' End as PackStatus,
                                REPL_SLNO
                        FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                                 STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                                 STORE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                 REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                        WHERE   REPL_ENTRY_TYPE = 'S' AND     (REPORT_LOTTERY_DAILY.REPL_STORE_ID = " + StoreID + ") AND (STORE_SALE_MASTER.SALE_DATE >= '" + FromDate + "' AND ";
                _sql += " STORE_SALE_MASTER.SALE_DATE <= '" + ToDate + "') ";
                _sql += " UNION ";
                _sql += "SELECT        SALE_DATE, SALE_SHIFT_CODE, '','','Online Sale','','','','','',SALE_LOTTERY_ONLINE,'',0 as REPL_SLNO";
                _sql += " FROM            STORE_SALE_MASTER ";
                _sql += " WHERE        (STORE_ID = " + StoreID + ") AND SALE_DATE >= '" + FromDate + "'" ;
                _sql += " AND SALE_DATE <= '" + ToDate + "' AND SALE_LOTTERY_ONLINE > 0 ";
                _sql += " ORDER BY SALE_DATE, SALE_SHIFT_CODE, REPL_SLNO ";


                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    obj = new ReportLotteryModel();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = dr["SALE_SHIFT_CODE"].ToString();
                    obj.Game = dr["REPL_GAME_ID"].ToString();
                    obj.Pack = dr["REPL_PACK_NO"].ToString();
                    if (dr["REPL_BOX_ID"].ToString() != "0")
                        obj.BoxNo = dr["REPL_BOX_ID"].ToString();
                    obj.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();

                    if (dr["LOTTERY_TICKET_VALUE"].ToString() != "0")
                        obj.TicketValue = dr["LOTTERY_TICKET_VALUE"].ToString();

                    //if (dr["REPL_START_TICKET"].ToString() != "0")
                    if (dr["REPL_START_TICKET"].ToString().Length > 0)
                        obj.StartTicket = dr["REPL_START_TICKET"].ToString();

                    if (dr["REPL_END_TICKET"].ToString().Length > 0)
                    {
                        if (obj.GameName != "Online Sale")
                        {
                            obj.EndTicket = dr["REPL_END_TICKET"].ToString();
                            obj.NoOfTicketSold = dr["NoOfTicketsSold"].ToString();
                        }
                        obj.AmountSold = Convert.ToSingle(dr["AmountSold"].ToString());
                    }
                    obj.PackStatus = dr["PackStatus"].ToString();
                    SaleReport.Add(obj);
                }

                

                #endregion

                return SaleReport;
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

        public List<ReportLotteryModel> BookActive(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReportLotteryModel> SaleReport = new List<ReportLotteryModel>();
            ReportLotteryModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Book Active
                _sql = @"SELECT      STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SHIFT_ID, GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID, 
                                 GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO, 
                                 STORE_LOTTERY_BOOKS_ACTIVE.LTACT_BOX_ID, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE
                        FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                 STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                 STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID 
                        WHERE     (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE >= 'PARMFROMDATE')
                                                  AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE <= 'PARMTODATE')
                        ORDER BY STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE, STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SHIFT_ID";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new ReportLotteryModel();
                    obj.Date = Convert.ToDateTime(dr["LTACT_DATE"].ToString());
                    obj.ShiftCode = dr["LTACT_SHIFT_ID"].ToString();
                    obj.Game = dr["LOTTERY_GAME_ID"].ToString();
                    obj.Pack = dr["LTACT_PACK_NO"].ToString();
                    if (dr["LTACT_BOX_ID"].ToString() != "0")
                        obj.BoxNo = dr["LTACT_BOX_ID"].ToString();
                    obj.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.AmountSold = Convert.ToSingle(dr["LOTTERY_BOOK_VALUE"].ToString());

                    //if (dr["REPL_END_TICKET"].ToString().Length > 0)
                    //    obj.PackStatus = "SOLD OUT";
                    //else
                    //    obj.PackStatus = "RUNNING";

                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion
                return SaleReport;
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

        public List<ReportLotteryModel> BookReceive(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReportLotteryModel> SaleReport = new List<ReportLotteryModel>();
            ReportLotteryModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Book Receive
                _sql = @"SELECT  STORE_LOTTERY_RECEIVE.LTREC_DATE, STORE_LOTTERY_RECEIVE.LTREC_SHIFT_ID, STORE_LOTTERY_RECEIVE.LTREC_GAME_ID, 
                             GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, STORE_LOTTERY_RECEIVE.LTREC_PACK_NO, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, 
                             STORE_LOTTERY_BOOKS_ACTIVE.LTACT_BOX_ID
                        FROM            STORE_LOTTERY_RECEIVE INNER JOIN
                                                    STORE_MASTER ON STORE_LOTTERY_RECEIVE.LTREC_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                    GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                    STORE_LOTTERY_RECEIVE.LTREC_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID LEFT OUTER JOIN
                                                    STORE_LOTTERY_BOOKS_ACTIVE ON STORE_LOTTERY_RECEIVE.LTREC_STORE_ID = STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID AND 
                                                    STORE_LOTTERY_RECEIVE.LTREC_GAME_ID = STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID AND 
                                                    STORE_LOTTERY_RECEIVE.LTREC_PACK_NO = STORE_LOTTERY_BOOKS_ACTIVE.LTACT_PACK_NO
                        WHERE        (STORE_LOTTERY_RECEIVE.LTREC_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_RECEIVE.LTREC_DATE >= 'PARMFROMDATE' AND 
                                                    STORE_LOTTERY_RECEIVE.LTREC_DATE <= 'PARMTODATE')
                        ORDER BY STORE_LOTTERY_RECEIVE.LTREC_DATE, STORE_LOTTERY_RECEIVE.LTREC_SHIFT_ID";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new ReportLotteryModel();
                    obj.Date = Convert.ToDateTime(dr["LTREC_DATE"].ToString());
                    obj.ShiftCode = dr["LTREC_SHIFT_ID"].ToString();
                    obj.Game = dr["LTREC_GAME_ID"].ToString();
                    obj.Pack = dr["LTREC_PACK_NO"].ToString();
                    obj.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.AmountSold = Convert.ToSingle(dr["LOTTERY_BOOK_VALUE"].ToString());

                    if (dr["LTACT_BOX_ID"].ToString().Length > 0)
                        obj.PackStatus = "ACTIVATED";
                    else
                        obj.PackStatus = "";

                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion
                return SaleReport;
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

        public List<ReportLotteryModel> BookReturn(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReportLotteryModel> SaleReport = new List<ReportLotteryModel>();
            ReportLotteryModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Book Receive
                _sql = @"SELECT    STORE_LOTTERY_RETURNS.LTRET_DATE, STORE_LOTTERY_RETURNS.LTRET_SHIFT_ID, STORE_LOTTERY_RETURNS.LTRET_GAME_ID, 
                                                     STORE_LOTTERY_RETURNS.LTRET_PACK_NO, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, 
                                                     STORE_LOTTERY_RETURNS.LTRET_LAST_TICKET_NO, 
                                CASE STORE_LOTTERY_RETURNS.LTRET_RETURN_FROM WHEN 'I' THEN 'FROM STOCK' ELSE 'FROM ACTIVATION' END AS LTRET_RETURN_FROM
                            FROM            STORE_LOTTERY_RETURNS INNER JOIN
                                                     STORE_MASTER ON STORE_LOTTERY_RETURNS.LTRET_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID
                                                    AND STORE_LOTTERY_RETURNS.LTRET_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE        (STORE_LOTTERY_RETURNS.LTRET_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_RETURNS.LTRET_DATE >= 'PARMFROMDATE' AND 
                                                     STORE_LOTTERY_RETURNS.LTRET_DATE <= 'PARMTODATE')
                            ORDER BY LTRET_DATE, STORE_LOTTERY_RETURNS.LTRET_SHIFT_ID";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new ReportLotteryModel();
                    obj.Date = Convert.ToDateTime(dr["LTRET_DATE"].ToString());
                    obj.ShiftCode = dr["LTRET_SHIFT_ID"].ToString();
                    obj.Game = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.Pack = dr["LTRET_PACK_NO"].ToString();
                    obj.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.EndTicket = dr["LTRET_LAST_TICKET_NO"].ToString();
                    obj.PackStatus = dr["LTRET_RETURN_FROM"].ToString();

                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion
                return SaleReport;
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

        public List<SaleReport> SaleReport(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<SaleReport> SaleReport = new List<SaleReport>();
            SaleReport obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Sale Report
                _sql = @"SELECT  SALE_DATE, SALE_SHIFT_CODE, SALE_LOTTERY_SALE, SALE_LOTTERY_ONLINE, SALE_LOTTERY_ONLINE + SALE_LOTTERY_SALE AS TOTAL_SALE, 
                             SALE_LOTTERY_BOOKS_ACTIVE, SALE_LOTTERY_CASH_INSTANT_PAID + SALE_LOTTERY_CASH_ONLINE_PAID AS CASH_PAID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DATE >= 'PARMFROMDATE') AND (SALE_DATE <= 'PARMTODATE')
                        ORDER BY SALE_DATE, SALE_SHIFT_CODE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new SaleReport();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    obj.InstantSale = Convert.ToSingle(dr["SALE_LOTTERY_SALE"].ToString());
                    obj.OnlineSale = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"].ToString());
                    obj.TotalSale = Convert.ToSingle(dr["TOTAL_SALE"].ToString());
                    obj.BookActive = Convert.ToSingle(dr["SALE_LOTTERY_BOOKS_ACTIVE"].ToString());
                    obj.CashPaid = Convert.ToSingle(dr["CASH_PAID"].ToString());

                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion
                return SaleReport;
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

        public List<BaseSale> MonthlySaleReport(int StoreID, int MonthNo, int YearNo)
        {
            List<BaseSale> SaleReport = new List<BaseSale>();
            BaseSale obj;
            BaseSale objTemp;
            SqlCommand cmd;
            SqlDataReader dr;
            DateTime FromDate;
            DateTime ToDate;

            FromDate = Convert.ToDateTime(MonthNo.ToString() + "-01" + "-" + YearNo.ToString());
            ToDate = FromDate.AddMonths(1);
            ToDate = ToDate.AddDays(-1);

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Sale Report
                List<BaseSale> MonthlySale = new List<BaseSale>();
                _sql = @"SELECT  SALE_DATE, SUM(SALE_LOTTERY_SALE) AS SALE_LOTTERY_SALE, SUM(SALE_LOTTERY_ONLINE) AS SALE_LOTTERY_ONLINE,
                            sum(SALE_LOTTERY_ONLINE_COMMISSION) as SALE_LOTTERY_ONLINE_COMMISSION, 
                            sum(SALE_LOTTER_INSTANT_COMMISSION) as SALE_LOTTER_INSTANT_COMMISSION, 
                            sum(SALE_LOTTERY_CASH_COMMISSION) as SALE_LOTTERY_CASH_COMMISSION 
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DATE >= 'PARMFROMDATE') AND (SALE_DATE <= 'PARMTODATE')
	                        GROUP BY SALE_DATE
                            ORDER BY SALE_DATE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new BaseSale();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.InstantSale = Convert.ToSingle(dr["SALE_LOTTERY_SALE"].ToString());
                    obj.OnlineSale = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"].ToString());
                    obj.InstantCommission = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE_COMMISSION"].ToString());
                    obj.OnlineCommission = Convert.ToSingle(dr["SALE_LOTTER_INSTANT_COMMISSION"].ToString());
                    obj.CashCommission = Convert.ToSingle(dr["SALE_LOTTERY_CASH_COMMISSION"].ToString());

                    MonthlySale.Add(obj);
                }
                dr.Close();
                #endregion

                for (DateTime i = FromDate; i <= ToDate; i = i.AddDays(1) )
                {
                    obj = new BaseSale();
                    obj.Date = i;
                    objTemp = MonthlySale.Find(x => x.Date == i);
                    if (objTemp != null)
                    {
                        obj.InstantSale = objTemp.InstantSale;
                        obj.OnlineSale = objTemp.OnlineSale;
                        obj.TotalSale = objTemp.InstantSale + objTemp.OnlineSale;
                    }
                    SaleReport.Add(obj);

                }

                return SaleReport;
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

        private List<SaleReport> DashboardSaleReport(int StoreID)
        {
            List<SaleReport> SaleReport = new List<SaleReport>();
            SaleReport obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Sale Report
                _sql = @"SELECT  TOP 6 SALE_DATE, SALE_SHIFT_CODE, SALE_LOTTERY_ONLINE + SALE_LOTTERY_SALE AS TOTAL_SALE
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = PARMSTOREID) 
                        ORDER BY SALE_DATE DESC, SALE_SHIFT_CODE DESC";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new SaleReport();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    obj.TotalSale = Convert.ToSingle(dr["TOTAL_SALE"].ToString());

                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion
                return SaleReport;
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

        private List<SaleReport> DashboardPreviousDaySales(int StoreID)
        {
            List<SaleReport> SaleReport = new List<SaleReport>();
            SaleReport obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Previous Day Sale Report
                _sql = @"SELECT        TOP (1) SALE_DATE, SALE_SHIFT_CODE, SALE_LOTTERY_SALE, SALE_LOTTERY_ONLINE, SALE_LOTTERY_ONLINE + SALE_LOTTERY_SALE AS TOTAL_SALE, 
                            SALE_LOTTERY_BOOKS_ACTIVE, SALE_LOTTERY_CASH_INSTANT_PAID + SALE_LOTTERY_CASH_ONLINE_PAID AS CASH_PAID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = PARMSTOREID) AND (SALE_LOTTERY_ONLINE + SALE_LOTTERY_SALE > 0)
                        ORDER BY SALE_DATE DESC, SALE_SHIFT_CODE DESC";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new SaleReport();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    obj.InstantSale = Convert.ToSingle(dr["SALE_LOTTERY_SALE"].ToString());
                    obj.OnlineSale = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"].ToString());
                    obj.TotalSale = Convert.ToSingle(dr["TOTAL_SALE"].ToString());
                    obj.BookActive = Convert.ToSingle(dr["SALE_LOTTERY_BOOKS_ACTIVE"].ToString());
                    obj.CashPaid = Convert.ToSingle(dr["CASH_PAID"].ToString());

                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion
                return SaleReport;
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

        private List<ReportLotteryModel> PreviousShiftBalance(int StoreID)
        {
            List<ReportLotteryModel> SaleReport = new List<ReportLotteryModel>();
            ReportLotteryModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Previous Shift Balance

                _sql = @"SELECT        STORE_SALE_MASTER.SALE_DATE, STORE_SALE_MASTER.SALE_SHIFT_CODE, REP.REPL_SALE_DAY_TRAN_ID, REP.REPL_BOX_ID, REP.REPL_GAME_ID, 
                             REP.REPL_PACK_NO, REP.REPL_SLNO, REP.REPL_START_TICKET, REP.REPL_END_TICKET, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, 
                             STORE_SALE_MASTER.SALE_LOTTERY_SALE, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE
                        FROM            REPORT_LOTTERY_DAILY AS REP INNER JOIN
                             (SELECT        REPL_BOX_ID, MAX(REPL_SLNO) AS SLNO
                               FROM            REPORT_LOTTERY_DAILY
                               WHERE        (REPL_SALE_DAY_TRAN_ID IN
                                                             (SELECT        max(REPL_SALE_DAY_TRAN_ID)
                                                               FROM            REPORT_LOTTERY_DAILY AS REPORT_LOTTERY_DAILY_1
															   INNER JOIN STORE_SALE_MASTER S1 
															   ON S1.STORE_ID = REPORT_LOTTERY_DAILY_1.REPL_STORE_ID
															   AND S1.SALE_DAY_TRAN_ID = REPORT_LOTTERY_DAILY_1.REPL_SALE_DAY_TRAN_ID
															   AND S1.SALE_LOTTERY_SALE > 0 
                                                               WHERE        (REPL_STORE_ID = PARMSTOREID))) AND (REPL_STORE_ID = PARMSTOREID)
                               GROUP BY REPL_BOX_ID) AS TAB ON REP.REPL_BOX_ID = TAB.REPL_BOX_ID AND REP.REPL_SLNO = TAB.SLNO INNER JOIN
                         STORE_SALE_MASTER ON STORE_SALE_MASTER.STORE_ID = REP.REPL_STORE_ID AND 
                         STORE_SALE_MASTER.SALE_DAY_TRAN_ID = REP.REPL_SALE_DAY_TRAN_ID INNER JOIN
                         STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND REP.REPL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                         GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                         REP.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID  AND  STORE_MASTER.STORE_ID = PARMSTOREID";


                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    obj = new ReportLotteryModel();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = dr["SALE_SHIFT_CODE"].ToString();
                    obj.Game = dr["REPL_GAME_ID"].ToString();
                    obj.Pack = dr["REPL_PACK_NO"].ToString();
                    if (dr["REPL_BOX_ID"].ToString() != "0")
                        obj.BoxNo = dr["REPL_BOX_ID"].ToString();
                    obj.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.TicketValue = dr["LOTTERY_TICKET_VALUE"].ToString();
                    if (dr["REPL_START_TICKET"].ToString() != "0")
                        obj.StartTicket = dr["REPL_START_TICKET"].ToString();

                    if (dr["REPL_END_TICKET"].ToString().Length > 0)
                    {
                        obj.EndTicket = dr["REPL_END_TICKET"].ToString();
                    }
                    SaleReport.Add(obj);
                }



                #endregion

                return SaleReport;
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

        public DashBoard GetDashBoard(int storeID)
        {
            DashBoard dashBoard = new DashBoard();
            dashBoard.DashboardSaleReport = DashboardSaleReport(storeID);
            dashBoard.PreviousShiftBalance = PreviousShiftBalance(storeID);
            dashBoard.DashboardPreviousDaySales = DashboardPreviousDaySales(storeID);
            return dashBoard;
        }

        public List<InventoryReportModel> InventoryReport(int StoreID)
        {
            List<InventoryReportModel> InventoryReport = new List<InventoryReportModel>();
            InventoryReportModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Inventory Report
                _sql = @"SELECT       GRP.LOTTERY_GAME_ID, GRP.LOTTERY_GAME_DESCRIPTION, ISNULL(REC.CNT,0) AS RECEIVED,
                            ISNULL(ACT.ACTIVATED,0) AS ACTIVATED,  ISNULL(REC.CNT,0) + ISNULL(ACT.ACTIVATED,0) AS TOTAL
                            FROM            GROUP_LOTTERY_MASTER GRP INNER JOIN
                            STORE_MASTER ON GRP.LOTTERY_GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                            LEFT OUTER JOIN 
	                            (
		                            SELECT  LTREC_GAME_ID AS GAME_ID, COUNT(ISNULL(ACT.LTACT_GAME_ID,0)) AS CNT
		                            FROM            STORE_LOTTERY_RECEIVE REC
		                            LEFT OUTER JOIN STORE_LOTTERY_BOOKS_ACTIVE ACT
		                            ON  REC.LTREC_STORE_ID = ACT.LTACT_STORE_ID AND REC.LTREC_GAME_ID = ACT.LTACT_GAME_ID
		                            AND REC.LTREC_PACK_NO = ACT.LTACT_PACK_NO
		                            WHERE        (LTREC_STORE_ID = PARMSTOREID) AND ACT.LTACT_GAME_ID IS NULL
		                            GROUP BY LTREC_GAME_ID 
	                            ) REC

                            ON GRP.LOTTERY_GAME_ID = REC.GAME_ID
                            LEFT OUTER JOIN 
	                            (
		                            SELECT   REPL_GAME_ID, COUNT(ISNULL(REPL_END_TICKET,0)) AS ACTIVATED
		                            FROM            REPORT_LOTTERY_DAILY
		                            WHERE        (REPL_STORE_ID = PARMSTOREID) AND (REPL_END_TICKET IS NULL)
		                            GROUP BY REPL_GAME_ID
	                            ) ACT
                            ON GRP.LOTTERY_GAME_ID = ACT.REPL_GAME_ID
                            WHERE        (STORE_MASTER.STORE_ID = PARMSTOREID) AND  ISNULL(REC.CNT,0) + ISNULL(ACT.ACTIVATED,0)  > 0
                            ORDER BY GRP.LOTTERY_GAME_ID";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new InventoryReportModel();
                    obj.GameID = Convert.ToString(dr["LOTTERY_GAME_ID"].ToString());
                    obj.GameDescription = Convert.ToString(dr["LOTTERY_GAME_DESCRIPTION"].ToString());
                    obj.Received = Convert.ToInt16(dr["RECEIVED"].ToString());
                    obj.Activated = Convert.ToInt16(dr["ACTIVATED"].ToString());
                    obj.Total = Convert.ToInt16(dr["TOTAL"].ToString());

                    InventoryReport.Add(obj);
                }
                dr.Close();
                #endregion
                return InventoryReport;
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

        public DayReport DayEndReport(int StoreID, DateTime Date, int ShiftCode)
        {
            DayReport DayReport = new DayReport();
            SqlCommand cmd;
            SqlDataReader dr;
            float fOnlineSale = 0;
            float fTotalSale = 0;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Sale Report



                _sql = @"SELECT   ISNULL(SUM(SALE_LOTTERY_SALE),0) AS SALE_LOTTERY_SALE, ISNULL(SUM(SALE_LOTTERY_ONLINE),0) AS SALE_LOTTERY_ONLINE, 
                                                 ISNULL(SUM(SALE_LOTTERY_CASH_INSTANT_PAID),0) AS SALE_LOTTERY_CASH_INSTANT_PAID, 
                                                ISNULL(SUM(SALE_LOTTERY_CASH_ONLINE_PAID),0) AS SALE_LOTTERY_CASH_ONLINE_PAID, 
                                                ISNULL(SUM(SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE),0) AS SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE, 
                                                 ISNULL(SUM(SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE),0) AS SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE, 
                                                 ISNULL(SUM(SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE),0) AS SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE, 
                                                 ISNULL(SUM(SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE),0) AS SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE, 
                                                 ISNULL(SUM(SALE_LOTTERY_CASH_TOTAL_TRANSFER),0) AS SALE_LOTTERY_CASH_TOTAL_TRANSFER
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DATE = 'PARMDATE')";

                if (ShiftCode != 0)
                         _sql += " AND SALE_SHIFT_CODE = " + ShiftCode;

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", Date.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.HasRows)
                    {
                        throw new Exception("No lottery sale is found");
                    }
                    
                    #region Adding Day Abstract
                    DayAbstractSale objAbstract = new DayAbstractSale();
                    objAbstract.OpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE"].ToString());
                    objAbstract.Sales = Convert.ToSingle(dr["SALE_LOTTERY_SALE"].ToString()) + Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"].ToString());
                    objAbstract.CashTransfer = Convert.ToSingle(dr["SALE_LOTTERY_CASH_TOTAL_TRANSFER"].ToString());
                    objAbstract.Paidout = Convert.ToSingle(dr["SALE_LOTTERY_CASH_INSTANT_PAID"].ToString()) + Convert.ToSingle(dr["SALE_LOTTERY_CASH_ONLINE_PAID"].ToString());
                    objAbstract.InsystemClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE"].ToString());
                    objAbstract.PhysicalClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE"].ToString());
                    fOnlineSale = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"].ToString());
                    DayReport.AbstractSale = objAbstract;
                    #endregion
                    if (objAbstract.Sales == 0)
                    {
                        throw new Exception("No lottery sale is found");
                    }

                    #region Adding Day transactions
                    DetailedDayTrans objDayTrans = new DetailedDayTrans();
                    objDayTrans.InsystemOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE"].ToString());
                    objDayTrans.PhysicalOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE"].ToString());
                    objDayTrans.InsystemClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE"].ToString());
                    objDayTrans.PhysicalClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE"].ToString());

                    objDayTrans.OpeningBalanceShort = objDayTrans.PhysicalOpeningBalance - objDayTrans.InsystemOpeningBalance;
                    objDayTrans.ClosingBalanceShort = objDayTrans.PhysicalClosingBalance - objDayTrans.InsystemClosingBalance;

                    objDayTrans.InsystemCashPaidOut = Convert.ToSingle(dr["SALE_LOTTERY_CASH_INSTANT_PAID"].ToString());
                    objDayTrans.PhysicalCashPaidOut = Convert.ToSingle(dr["SALE_LOTTERY_CASH_ONLINE_PAID"].ToString());
                    objDayTrans.TotalCashPaidOut = objDayTrans.InsystemCashPaidOut + objDayTrans.PhysicalCashPaidOut;
                    dr.Close();

                    _sql = @"SELECT   STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE, SUM(STORE_BUSINESS_TRANS.BUSINESS_AMOUNT) AS BUSINESS_AMOUNT
                                FROM            STORE_SALE_MASTER INNER JOIN
                                                         STORE_BUSINESS_TRANS ON STORE_SALE_MASTER.STORE_ID = STORE_BUSINESS_TRANS.BUSINESS_STORE_ID AND 
                                                         STORE_SALE_MASTER.SALE_DAY_TRAN_ID = STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID
                                WHERE        (STORE_SALE_MASTER.STORE_ID = PARMSTOREID) AND (STORE_BUSINESS_TRANS.BUSINESS_TRAN_FROM = 'LC') AND 
                                                         (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')";
                    if (ShiftCode != 0)
                        _sql += " AND SALE_SHIFT_CODE = " + ShiftCode;

                    _sql += " GROUP BY STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE";
                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", Date.ToString());
                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["BUSINESS_TRAN_TYPE"].ToString().Equals("LD"))
                            objDayTrans.BankDeposit = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());
                        else if (dr["BUSINESS_TRAN_TYPE"].ToString().Equals("BC"))
                            objDayTrans.BusinessCash = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());
                        else if (dr["BUSINESS_TRAN_TYPE"].ToString().Equals("GC"))
                            objDayTrans.GasCash = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());
                    }
                    DayReport.Balance = objDayTrans;
                    #endregion
                }
                dr.Close();

                #region Adding Instant Sale
                SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

                int iDayTranID = _SaleSupportEntries.DayTranID(Date, StoreID, ShiftCode, _conn);

                _sql = @"SELECT CREATED_BY, max(CREATED_TIMESTAMP) AS TIME_STAMP
                            FROM   STORE_SALE_LOTTERY_CLOSING
                            WHERE  (LTRSAL_STORE_ID = PARMSTOREID) AND LTRSAL_SALE_DAY_TRAN_ID = PARMDAYTRANID
                            group by CREATED_BY";
                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID", iDayTranID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        DayReport.DayClosedBy = dr["CREATED_BY"].ToString();
                        DayReport.DayClosedOn = Convert.ToDateTime(dr["TIME_STAMP"].ToString());
                    }
                }
                dr.Close();

                _sql = @"SELECT REPORT_LOTTERY_DAILY.REPL_GAME_ID, REPORT_LOTTERY_DAILY.REPL_PACK_NO, 
                                                        (REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET) AS NO_OF_TICKETS, 
                                                        GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE, ((REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET))
                                                        * GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE AS AMOUNT_SOLD
                            FROM            STORE_SALE_MASTER INNER JOIN
                                                        REPORT_LOTTERY_DAILY ON STORE_SALE_MASTER.STORE_ID = REPORT_LOTTERY_DAILY.REPL_STORE_ID AND 
                                                        STORE_SALE_MASTER.SALE_DAY_TRAN_ID = REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID INNER JOIN
                                                        STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                        GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                        REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE      REPL_ENTRY_TYPE = 'S' AND     (STORE_SALE_MASTER.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')";

                if (ShiftCode != 0)
                         _sql += " AND SALE_SHIFT_CODE = " + ShiftCode;

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", Date.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                List<DetailedDayInstantSale> objInstantSaleColl = new List<DetailedDayInstantSale>();
                DetailedDayInstantSale objInstantSale;
                while (dr.Read())
                {
                    objInstantSale = new DetailedDayInstantSale();
                    objInstantSale.GameID = dr["REPL_GAME_ID"].ToString();
                    objInstantSale.PackNo = dr["REPL_PACK_NO"].ToString();
                    objInstantSale.TicketValue = Convert.ToSingle(dr["LOTTERY_TICKET_VALUE"].ToString());
                    objInstantSale.NoOfTickets = Convert.ToInt16(dr["NO_OF_TICKETS"].ToString());
                    objInstantSale.AmountSold = Convert.ToSingle(dr["AMOUNT_SOLD"].ToString());
                    fTotalSale += objInstantSale.AmountSold;
                    objInstantSaleColl.Add(objInstantSale);
                }
                dr.Close();
                objInstantSale = new DetailedDayInstantSale();
                objInstantSale.PackNo = "Online Sale";
                objInstantSale.AmountSold = fOnlineSale;
                fTotalSale += objInstantSale.AmountSold;
                objInstantSaleColl.Add(objInstantSale);

                objInstantSale = new DetailedDayInstantSale();
                objInstantSale.PackNo = "Total";
                objInstantSale.AmountSold = fTotalSale;
                objInstantSaleColl.Add(objInstantSale);

                DayReport.InstantSale = objInstantSaleColl;

                #endregion

                #region Adding Inventory
                List<GameCount> objGameCountReceived = new List<GameCount>();
                List<GameCount> objGameCountActivated = new List<GameCount>();
                List<GameCount> objGameCountReturnedStock = new List<GameCount>();
                List<GameCount> objGameCountReturnedActivated = new List<GameCount>();

                GameCount objCount;

                //Adding Received Books
                _sql = @"SELECT LTREC_GAME_ID, COUNT(LTREC_STORE_ID) AS GAME_RECEIVED
                        FROM            STORE_LOTTERY_RECEIVE
                        WHERE        (LTREC_DATE = 'PARMDATE')";
                if (ShiftCode != 0)
                    _sql += " AND  LTREC_SHIFT_ID = " + ShiftCode;

                _sql += " GROUP BY LTREC_GAME_ID, LTREC_STORE_ID ";
                _sql += " HAVING        (LTREC_STORE_ID = PARMSTOREID)";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", Date.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    objCount = new GameCount();
                    objCount.GameID =  dr["LTREC_GAME_ID"].ToString();
                    objCount.Count = Convert.ToInt16(dr["GAME_RECEIVED"].ToString());
                    objGameCountReceived.Add(objCount);
                }
                dr.Close();

                //Adding Activated Books
                _sql = @"SELECT COUNT(LTACT_STORE_ID) AS GAME_ACTIVATED, LTACT_GAME_ID
                        FROM             STORE_LOTTERY_BOOKS_ACTIVE
                        WHERE        (LTACT_DATE = 'PARMDATE')";
                if (ShiftCode != 0)
                    _sql += " AND   LTACT_SHIFT_ID = " + ShiftCode;

                _sql += " GROUP BY LTACT_GAME_ID, LTACT_STORE_ID ";
                _sql += " HAVING        (LTACT_STORE_ID = PARMSTOREID)";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", Date.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    objCount = new GameCount();
                    objCount.GameID = dr["LTACT_GAME_ID"].ToString();
                    objCount.Count = Convert.ToInt16(dr["GAME_ACTIVATED"].ToString());
                    objGameCountActivated.Add(objCount);
                }
                dr.Close();

                //Adding Returned Books from Inventory
                _sql = @"SELECT COUNT(LTRET_STORE_ID) AS GAME_RETURNED, LTRET_GAME_ID
                        FROM               STORE_LOTTERY_RETURNS
                        WHERE        (LTRET_DATE = 'PARMDATE') AND (LTRET_RETURN_FROM = 'I')";
                if (ShiftCode != 0)
                    _sql += " AND   LTRET_SHIFT_ID = " + ShiftCode;

                _sql += " GROUP BY LTRET_GAME_ID, LTRET_STORE_ID ";
                _sql += " HAVING        (LTRET_STORE_ID = PARMSTOREID)";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", Date.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    objCount = new GameCount();
                    objCount.GameID = dr["LTRET_GAME_ID"].ToString();
                    objCount.Count = Convert.ToInt16(dr["GAME_RETURNED"].ToString());
                    objGameCountReturnedStock.Add(objCount);
                }
                dr.Close();

                //Adding Returned Books from Activated
                _sql = @"SELECT COUNT(LTRET_STORE_ID) AS GAME_RETURNED, LTRET_GAME_ID
                        FROM               STORE_LOTTERY_RETURNS
                        WHERE        (LTRET_DATE = 'PARMDATE') AND (LTRET_RETURN_FROM = 'A')";
                if (ShiftCode != 0)
                    _sql += " AND   LTRET_SHIFT_ID = " + ShiftCode;

                _sql += " GROUP BY LTRET_GAME_ID, LTRET_STORE_ID ";
                _sql += " HAVING        (LTRET_STORE_ID = PARMSTOREID)";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", Date.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    objCount = new GameCount();
                    objCount.GameID = dr["LTRET_GAME_ID"].ToString();
                    objCount.Count = Convert.ToInt16(dr["GAME_RETURNED"].ToString());
                    objGameCountReturnedActivated.Add(objCount);
                }
                dr.Close();


                List<DayInventory> objDayInvColl = new List<DayInventory>();
                List<LotteryGames>  objGameModel = new List<LotteryGames>();
                DayInventory objDayInv ;
                LotteryMaster objMaster = new LotteryMaster();
                objGameModel = objMaster.GetLotteryGames(StoreID);
                foreach(LotteryGames o in objGameModel)
                {
                    objDayInv = new DayInventory();
                    objDayInv.GameID = o.GameID;
                    objDayInvColl.Add(objDayInv);
                }


                foreach (DayInventory o in objDayInvColl)
                {
                    objCount = objGameCountReceived.Find(x => x.GameID == o.GameID);
                    if (objCount != null)
                        o.NoOfBooksReceived = objCount.Count;

                    objCount = objGameCountActivated.Find(x => x.GameID == o.GameID);
                    if (objCount != null)
                        o.NoOfBooksActivated = objCount.Count;

                    objCount = objGameCountReturnedStock.Find(x => x.GameID == o.GameID);
                    if (objCount != null)
                        o.NoOfBooksReturnFromInventory = objCount.Count;

                    objCount = objGameCountReturnedActivated.Find(x => x.GameID == o.GameID);
                    if (objCount != null)
                        o.NoOfBooksReturnFromActive = objCount.Count;
                }

                objDayInvColl.RemoveAll(o => o.NoOfBooksReturnFromActive == 0 && o.NoOfBooksActivated == 0 && o.NoOfBooksReceived == 0 && o.NoOfBooksReturnFromInventory == 0);

                //foreach (DayInventory o in objDayInvColl)
                //{
                //    if (o.NoOfBooksReturnFromActive == 0 && o.NoOfBooksActivated == 0 && o.NoOfBooksReceived == 0 && o.NoOfBooksReturnFromInventory == 0)
                //        objDayInvColl.Remove(o);
                //}

                DayReport.Inventory = objDayInvColl;
                #endregion

                #endregion
                return DayReport;
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

        public List<GameSaleTrend> GameSaleTrend(int StoreID, int MonthNo, int YearNo)
        {
            List<GameSaleTrend> SaleReport = new List<GameSaleTrend>();
            GameSaleTrend obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Sale Report
                List<BaseSale> MonthlySale = new List<BaseSale>();
                _sql = @"SELECT   REPORT_LOTTERY_DAILY.REPL_GAME_ID, SUM((REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET) 
                                                     * GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE) AS TOTAL_SOLD
                            FROM            REPORT_LOTTERY_DAILY INNER JOIN
                                                     STORE_SALE_MASTER ON REPORT_LOTTERY_DAILY.REPL_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     REPORT_LOTTERY_DAILY.REPL_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                                     STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                     REPORT_LOTTERY_DAILY.REPL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                            WHERE        (REPORT_LOTTERY_DAILY.REPL_STORE_ID = PARMSTOREID) AND (MONTH(STORE_SALE_MASTER.SALE_DATE) =  PARMMONTH) AND (YEAR(STORE_SALE_MASTER.SALE_DATE) = PARMYEAR) 
                                                     AND (REPORT_LOTTERY_DAILY.REPL_ENTRY_TYPE = 'S')
                            GROUP BY REPORT_LOTTERY_DAILY.REPL_GAME_ID
                            HAVING        (NOT (SUM((REPORT_LOTTERY_DAILY.REPL_END_TICKET - REPORT_LOTTERY_DAILY.REPL_START_TICKET) 
                                                     * GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE) IS NULL))
                            ORDER BY REPORT_LOTTERY_DAILY.REPL_GAME_ID";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMMONTH", MonthNo.ToString());
                _sql = _sql.Replace("PARMYEAR", YearNo.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new GameSaleTrend();
                    obj.GameID = dr["REPL_GAME_ID"].ToString();
                    obj.SaleAmount = Convert.ToSingle(dr["TOTAL_SOLD"].ToString());
                    SaleReport.Add(obj);
                }
                dr.Close();
                #endregion

                return SaleReport;
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

        public List<GameModel> StoreGames(int StoreID)
        {
            List<GameModel> StoreGame = new List<GameModel>();
            GameModel obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Store Games
                _sql = @"SELECT   MAPPING_STORE_LOTTERY.GAME_ID, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, 
                         GROUP_LOTTERY_MASTER.LOTTERY_NO_OF_TICKETS, GROUP_LOTTERY_MASTER.LOTTERY_TICKET_VALUE, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE, GROUP_LOTTERY_MASTER.LOTTERY_BOOK_FIRST_TICKET_NO, 
                         GROUP_LOTTERY_MASTER.LOTTERY_BOOK_LAST_TICKET_NO
                        FROM            MAPPING_STORE_LOTTERY INNER JOIN
                                                 STORE_MASTER ON MAPPING_STORE_LOTTERY.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID
                        WHERE        (MAPPING_STORE_LOTTERY.STORE_ID = PARMSTOREID) AND (MAPPING_STORE_LOTTERY.ACTIVE = 'A')";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new GameModel();
                    obj.Game = dr["GAME_ID"].ToString();
                    obj.GameName = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.NoOfTicket = Convert.ToInt16(dr["LOTTERY_NO_OF_TICKETS"].ToString());
                    obj.TicketValue = Convert.ToSingle(dr["LOTTERY_TICKET_VALUE"].ToString());
                    obj.BookValue = Convert.ToSingle(dr["LOTTERY_BOOK_VALUE"].ToString());
                    obj.StartTicket = dr["LOTTERY_BOOK_FIRST_TICKET_NO"].ToString();
                    obj.EndTicket = dr["LOTTERY_BOOK_LAST_TICKET_NO"].ToString();

                    StoreGame.Add(obj);
                }
                dr.Close();
                #endregion
                return StoreGame;
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

        public List<LotterySale> UnsavedClosingReading(int StoreID)
        {
            List<LotterySale> unsavedGames = new List<LotterySale>();
            LotterySale obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Store Games
                _sql = @"SELECT LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_SLNO, LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_GAME_ID, 
                                                     LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_PACK_NO, LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_NO_TICKETS, 
                                                     LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_BOX_NO, LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_LAST_TICKET_NO, 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION
                            FROM            LOTTERY_TEMPORARY_CLOSING_READING INNER JOIN
                                                     GROUP_LOTTERY_MASTER ON LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID INNER JOIN
                                                     STORE_MASTER ON LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_STORE_ID = STORE_MASTER.STORE_ID AND 
                                                     GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                            WHERE        (LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_STORE_ID = PARMSTOREID)
                            ORDER BY LOTTERY_TEMPORARY_CLOSING_READING.TMPCLS_SLNO";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new LotterySale();
                    obj.GameID = dr["TMPCLS_GAME_ID"].ToString();
                    obj.GameDescription = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                    obj.NoOfTickets = Convert.ToInt16(dr["TMPCLS_NO_TICKETS"].ToString());
                    obj.PackNo = dr["TMPCLS_PACK_NO"].ToString();
                    obj.LastTicketClosing = Convert.ToInt16(dr["TMPCLS_LAST_TICKET_NO"].ToString());
                    obj.BoxID = Convert.ToInt16(dr["TMPCLS_BOX_NO"].ToString());

                    unsavedGames.Add(obj);
                }
                dr.Close();
                #endregion
                return unsavedGames;
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

    public class GameCount
    {
        public string GameID { get; set; }
        public int Count { get; set; }
    }
}

