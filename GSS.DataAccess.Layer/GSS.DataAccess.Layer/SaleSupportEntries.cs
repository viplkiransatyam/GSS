using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class SaleSupportEntries
    {
        string _sql;
        public bool IsEdit { get; set; }

        public int DayTranID(DateTime Date, int iStoreID, int ShiftCode, SqlConnection con)
        {
            int iDayTranID = 0;
            IsEdit = false;

            _sql = @"SELECT        SALE_DAY_TRAN_ID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + Date + "') AND (SALE_SHIFT_CODE = " + ShiftCode + ")";

            
            SqlCommand cmd = new SqlCommand(_sql, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                IsEdit = true;
                iDayTranID = Convert.ToInt16(dr["SALE_DAY_TRAN_ID"]);
            }
            dr.Close();

            if (iDayTranID == 0)
            {
                _sql = @"SELECT      MAX(SALE_DAY_TRAN_ID) + 1
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ")";

                cmd = new SqlCommand(_sql, con);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr[0] == null)
                        iDayTranID = 1;
                    else if (dr[0].ToString().Length == 0)
                        iDayTranID = 1;
                    else
                        iDayTranID = Convert.ToInt16(dr[0]);
                }
                else
                    iDayTranID = 1;

                dr.Close();
            }

            return iDayTranID;

        }

        public float GetBookActivePerShift(DateTime Date, int iStoreID, int ShiftCode, SqlConnection con, SqlTransaction conTran)
        {
            float fBookActive = 0;


            _sql = @"SELECT    SUM(GROUP_LOTTERY_MASTER.LOTTERY_BOOK_VALUE) AS TotalBookActive
                        FROM            STORE_LOTTERY_BOOKS_ACTIVE INNER JOIN
                                                 STORE_MASTER ON STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                 GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                                 STORE_LOTTERY_BOOKS_ACTIVE.LTACT_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                        WHERE        (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_STORE_ID = PARMSTOREID) AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_DATE = 'PARMDATE') 
                            AND (STORE_LOTTERY_BOOKS_ACTIVE.LTACT_SHIFT_ID = PARMSHIFTCODE)";

            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMDATE", Date.ToString());
            _sql = _sql.Replace("PARMSHIFTCODE", ShiftCode.ToString());

            SqlCommand cmd = new SqlCommand(_sql, con, conTran);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                if (dr["TotalBookActive"].ToString().Length > 0)
                    fBookActive = Convert.ToSingle(dr["TotalBookActive"]);
                else
                    fBookActive = 0;
            }
            dr.Close();

            return fBookActive;

        }

        public bool GetLotteryShiftStatus(Int32 DayTranID, int iStoreID, SqlConnection con, SqlTransaction conTran)
        {

            bool bReturn = false;

            _sql = @"SELECT    LOTTERY_SHIFT_OPEN FROM STORE_SALE_MASTER WHERE STORE_ID = PARMSTOREID AND SALE_DAY_TRAN_ID = PARMDAYTRANID";
                         
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMDAYTRANID", DayTranID.ToString());

            SqlCommand cmd = new SqlCommand(_sql, con, conTran);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                if (dr["LOTTERY_SHIFT_OPEN"].ToString() == "OPEN")
                    bReturn = true;
                else
                    bReturn = false;
            }
            dr.Close();
            return bReturn;
        }

        public OnlyMaster GetSaleMasterEntries(Int32 DayTranID, int iStoreID, SqlConnection con, SqlTransaction conTran)
        {
            OnlyMaster onlyMaster = new OnlyMaster();

            _sql = @"SELECT    STORE_ID,SALE_DAY_TRAN_ID,SALE_SHIFT_CODE,SALE_DATE,SALE_GAS_TOTAL_GALLONS,SALE_GAS_TOTAL_TOTALIER,SALE_GAS_TOTAL_SALE,
                            SALE_GAS_CARD_TOTAL,SALE_LOTTERY_RETURN,SALE_LOTTERY_SALE,SALE_LOTTERY_BOOKS_ACTIVE,SALE_LOTTERY_ONLINE,SALE_LOTTERY_CASH_INSTANT_PAID,
                            SALE_LOTTERY_CASH_ONLINE_PAID,SALE_LOTTERY_ONLINE_COMMISSION,SALE_LOTTER_INSTANT_COMMISSION,SALE_LOTTERY_CASH_COMMISSION,
                            SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE,SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE,SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE,
                            SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE,SALE_LOTTERY_CASH_TOTAL_TRANSFER,CASH_OPENING_BALANCE,CASH_PHYSICAL_AT_STORE,CASH_DEPOSITED_IN_BANK,
                            CASH_CLOSING_BALANCE,SALE_ENTRY_LOCKED,LOTTERY_SHIFT_OPEN
                            FROM STORE_SALE_MASTER WHERE STORE_ID = PARMSTOREID AND SALE_DAY_TRAN_ID = PARMDAYTRANID";

            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMDAYTRANID", DayTranID.ToString());

            SqlCommand cmd = new SqlCommand(_sql, con, conTran);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                onlyMaster.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                onlyMaster.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                onlyMaster.TotalSaleGallons = Convert.ToSingle(dr["SALE_GAS_TOTAL_GALLONS"]);
                onlyMaster.TotalTotalizer = Convert.ToSingle(dr["SALE_GAS_TOTAL_TOTALIER"]);
                onlyMaster.TotalSale = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"]);
                onlyMaster.CardTotal = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"]);
                onlyMaster.LotteryReturn = Convert.ToSingle(dr["SALE_LOTTERY_RETURN"]);
                onlyMaster.LotterySale = Convert.ToSingle(dr["SALE_LOTTERY_SALE"]);
                onlyMaster.LotteryBooksActive = Convert.ToSingle(dr["SALE_LOTTERY_BOOKS_ACTIVE"]);
                onlyMaster.LotteryOnline = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE"]);
                onlyMaster.LotteryCashInstantPaid = Convert.ToSingle(dr["SALE_LOTTERY_CASH_INSTANT_PAID"]);
                onlyMaster.LotteryCashOnlinePaid = Convert.ToSingle(dr["SALE_LOTTERY_CASH_ONLINE_PAID"]);
                onlyMaster.LotteryOnlineCommission = Convert.ToSingle(dr["SALE_LOTTERY_ONLINE_COMMISSION"]);
                onlyMaster.LotteryInstantCommission = Convert.ToSingle(dr["SALE_LOTTER_INSTANT_COMMISSION"]);
                onlyMaster.LotteryCashCommission = Convert.ToSingle(dr["SALE_LOTTERY_CASH_COMMISSION"]);
                onlyMaster.LotteryCashSystemOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_OPENING_BALANCE"]);
                onlyMaster.LotteryCashPhysicalOpeningBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_OPENING_BALANCE"]);
                onlyMaster.LotteryCashSystemClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_SYSTEM_CASH_CLOSING_BALANCE"]);
                onlyMaster.LotteryCashPhysicalClosingBalance = Convert.ToSingle(dr["SALE_LOTTERY_PHYSICAL_CASH_CLOSING_BALANCE"]);
                onlyMaster.LotteryCashTransfer = Convert.ToSingle(dr["SALE_LOTTERY_CASH_TOTAL_TRANSFER"]);
                onlyMaster.CashOpeningBalance = Convert.ToSingle(dr["CASH_OPENING_BALANCE"]);
                onlyMaster.CashPhysicalAtStore = Convert.ToSingle(dr["CASH_PHYSICAL_AT_STORE"]);
                onlyMaster.CashDeposited = Convert.ToSingle(dr["CASH_DEPOSITED_IN_BANK"]);
                onlyMaster.CashClosingBalance = Convert.ToSingle(dr["CASH_CLOSING_BALANCE"]);
            }
            dr.Close();
            return onlyMaster;
        }

        public NextShift NextShift(DateTime CurrentDate, int iStoreID, int CurrentShiftCode, SqlConnection con, SqlTransaction conTran)
        {
            NextShift nextShift = new NextShift();
            int iDayTranID = 0;
            SqlCommand cmd;
            SqlDataReader dr;

            //Calculation Next Shift
            _sql = @"SELECT    MIN(SHFT_SHIFT_CODE)  FROM            SHIFT_CODES
                        WHERE        (SHFT_STORE_ID = " + iStoreID + ") AND SHFT_SHIFT_CODE > " + CurrentShiftCode;

            cmd = new SqlCommand(_sql, con, conTran);

            if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().ToString().Length > 0) // Next Shift exists
            {
                nextShift.NextShiftID = (int)cmd.ExecuteScalar();
                nextShift.NextDate = CurrentDate;

                _sql = @"SELECT        SALE_DAY_TRAN_ID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + CurrentDate + "') AND (SALE_SHIFT_CODE = " + nextShift.NextShiftID + ")";


                cmd = new SqlCommand(_sql, con, conTran);
                dr = cmd.ExecuteReader(); // Getting day tran id for next shift
                if (dr.Read())
                {
                    iDayTranID = Convert.ToInt16(dr["SALE_DAY_TRAN_ID"]);
                    nextShift.NextDayTranID = iDayTranID;
                }
                dr.Close();
            }
            else
            {
                nextShift.NextShiftID = 1;
                nextShift.NextDate = CurrentDate.AddDays(1);
            }

            if (iDayTranID == 0) // If no next shift exists
            {
                IsEdit = false;
                _sql = @"SELECT        SALE_DAY_TRAN_ID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + nextShift.NextDate + "') AND (SALE_SHIFT_CODE = " + nextShift.NextShiftID + ")";


                cmd = new SqlCommand(_sql, con, conTran);
                dr = cmd.ExecuteReader(); // Getting day tran id for next shift
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        IsEdit = true;
                        iDayTranID = Convert.ToInt16(dr["SALE_DAY_TRAN_ID"]);
                    }
                }

                dr.Close();

                if (IsEdit == false)
                {
                    _sql = @"SELECT      MAX(SALE_DAY_TRAN_ID) + 1
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ")";

                    cmd = new SqlCommand(_sql, con, conTran);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr[0] == null)
                            iDayTranID = 1;
                        else if (dr[0].ToString().Length == 0)
                            iDayTranID = 1;
                        else
                            iDayTranID = Convert.ToInt16(dr[0]);
                    }
                    else
                        iDayTranID = 1;

                    dr.Close();
                }
                nextShift.NextDayTranID = iDayTranID;
            }

            return nextShift;

        }

        public NextShift NextShift(DateTime CurrentDate, int iStoreID, int CurrentShiftCode, SqlConnection con)
        {
            NextShift nextShift = new NextShift();
            int iDayTranID = 0;
            SqlCommand cmd;
            SqlDataReader dr;

            //Calculation Next Shift
            _sql = @"SELECT    MIN(SHFT_SHIFT_CODE)  FROM            SHIFT_CODES
                        WHERE        (SHFT_STORE_ID = " + iStoreID + ") AND SHFT_SHIFT_CODE > " + CurrentShiftCode;

            cmd = new SqlCommand(_sql, con);

            if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().ToString().Length > 0) // Next Shift exists
            {
                nextShift.NextShiftID = (int)cmd.ExecuteScalar();
                nextShift.NextDate = CurrentDate;

                _sql = @"SELECT        SALE_DAY_TRAN_ID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + CurrentDate + "') AND (SALE_SHIFT_CODE = " + nextShift.NextShiftID + ")";


                cmd = new SqlCommand(_sql, con);
                dr = cmd.ExecuteReader(); // Getting day tran id for next shift
                if (dr.Read())
                {
                    iDayTranID = Convert.ToInt16(dr["SALE_DAY_TRAN_ID"]);
                    nextShift.NextDayTranID = iDayTranID;
                }
                dr.Close();
            }
            else
            {
                nextShift.NextShiftID = 1;
                nextShift.NextDate = CurrentDate.AddDays(1);
            }

            if (iDayTranID == 0) // If no next shift exists
            {
                IsEdit = false;
                _sql = @"SELECT      MAX(SALE_DAY_TRAN_ID) + 1
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ")";

                cmd = new SqlCommand(_sql, con);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr[0] == null)
                        iDayTranID = 1;
                    else if (dr[0].ToString().Length == 0)
                        iDayTranID = 1;
                    else
                        iDayTranID = Convert.ToInt16(dr[0]);
                }
                else
                    iDayTranID = 1;

                dr.Close();
                nextShift.NextDayTranID = iDayTranID;
            }

            return nextShift;

        }

        public int DayTranIDWithTransaction(DateTime Date, int iStoreID, int ShiftCode, SqlConnection con, SqlTransaction _conTran)
        {
            int iDayTranID = 0;
            IsEdit = false;

            _sql = @"SELECT        SALE_DAY_TRAN_ID
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + Date + "') AND (SALE_SHIFT_CODE = " + ShiftCode + ")";


            SqlCommand cmd = new SqlCommand(_sql, con, _conTran);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                IsEdit = true;
                iDayTranID = Convert.ToInt16(dr["SALE_DAY_TRAN_ID"]);
            }
            dr.Close();

            if (iDayTranID == 0)
            {
                _sql = @"SELECT      MAX(SALE_DAY_TRAN_ID) + 1
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ")";

                cmd = new SqlCommand(_sql, con, _conTran);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr[0] == null)
                        iDayTranID = 1;
                    else if (dr[0].ToString().Length == 0)
                        iDayTranID = 1;
                    else
                        iDayTranID = Convert.ToInt16(dr[0]);
                }
                else
                    iDayTranID = 1;

                dr.Close();
            }

            return iDayTranID;

        }

        public List<GasSaleModel> GasSale(int iStoreID, int iDayTranID, DateTime dDate, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<GasSaleModel> GasSaleColl = new List<GasSaleModel>();
            GasSaleModel GasSale;

            _sql = @"SELECT     STORE_GAS_SALE_TRAN.GASTYPE_ID, STORE_GAS_SALE_TRAN.SALE_TOTALIZER, STORE_GAS_SALE_TRAN.SALE_GALLONS, 
                         STORE_GAS_SALE_TRAN.SALE_AMOUNT, STORE_GAS_SALE_TRAN.SALE_PRICE, STORE_GAS_SALE_TRAN.SET_PRICE, GROUP_GASTYPE_MASTER.GASTYPE_NAME
                        FROM            STORE_GAS_SALE_TRAN INNER JOIN
                                                 MAPPING_STORE_GAS ON STORE_GAS_SALE_TRAN.STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                                 STORE_GAS_SALE_TRAN.GASTYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                        WHERE        (STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID = " + iDayTranID + ") AND (STORE_GAS_SALE_TRAN.STORE_ID = " + iStoreID + ")";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            bool IsFound = false;
            while (dr.Read())
            {
                IsFound = true;
                GasSale = new GasSaleModel();
                GasSale.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                GasSale.GasTypeName  = dr["GASTYPE_NAME"].ToString();
                GasSale.Totalizer = Convert.ToSingle(dr["SALE_TOTALIZER"]);
                GasSale.SaleGallons  = Convert.ToSingle(dr["SALE_GALLONS"]);
                GasSale.SaleAmount  = Convert.ToSingle(dr["SALE_AMOUNT"]);
                GasSale.SalePrice = Convert.ToSingle(dr["SALE_PRICE"]);
                GasSale.SetPrice = Convert.ToSingle(dr["SET_PRICE"]);
                GasSaleColl.Add(GasSale); 
            }
            dr.Close();

            if (IsFound == false)
            {
                List<GasOil> GasOilForStore = new List<GasOil>();
                GasOilDal _GasOilDal = new GasOilDal();
                GasOilForStore = _GasOilDal.SelectRecords(iStoreID);
                foreach(GasOil obj in GasOilForStore)
                {
                    GasSale = new GasSaleModel();
                    GasSale.GasTypeID = obj.GasTypeID;
                    GasSale.GasTypeName = obj.GasTypeName;
                    _sql = "SELECT SET_PRICE FROM STORE_GAS_SALE_TRAN WHERE STORE_ID = " + iStoreID + " AND GASTYPE_ID = " + obj.GasTypeID + " AND SALE_DATE " ;
                    _sql +=   "IN ";
                    _sql += "(";
                    _sql += "SELECT MIN(SALE_DATE) FROM STORE_GAS_SALE_TRAN WHERE STORE_ID = " + iStoreID + " AND GASTYPE_ID = " + obj.GasTypeID;
                    _sql += " AND SALE_DATE <= '" + dDate + "'";
                    _sql += ")";
                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                        GasSale.SetPrice = Convert.ToSingle(dr["SET_PRICE"]);
                    dr.Close();
                    GasSaleColl.Add(GasSale);
                }

            }

            return GasSaleColl;
        }

        public List<LotterySale> LotteryClosingReading(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<LotterySale> LotteryClosing = new List<LotterySale>();
            LotterySale objLottery;

////            _sql = @"SELECT        T1.REPL_BOX_ID, GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, T1.REPL_GAME_ID, T1.REPL_PACK_NO, T1.REPL_END_TICKET
////                        FROM            GROUP_LOTTERY_MASTER INNER JOIN
////                                                 STORE_MASTER ON GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID = STORE_MASTER.STORE_GROUP_ID INNER JOIN
////                                                 REPORT_LOTTERY_DAILY AS T1 INNER JOIN
////                                                     (SELECT        REPL_BOX_ID, MAX(REPL_SLNO) AS REPL_SLNO
////                                                       FROM            REPORT_LOTTERY_DAILY
////                                                       WHERE        (REPL_STORE_ID = PARMSTOREID) AND (REPL_SALE_DAY_TRAN_ID = PARMTRANID)
////                                                       GROUP BY REPL_BOX_ID) AS T2 ON T1.REPL_BOX_ID = T2.REPL_BOX_ID AND T1.REPL_SLNO = T2.REPL_SLNO ON 
////                                                 STORE_MASTER.STORE_ID = T1.REPL_STORE_ID AND GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID = T1.REPL_GAME_ID
////                        WHERE        (T1.REPL_STORE_ID = PARMSTOREID) AND (T1.REPL_SALE_DAY_TRAN_ID = PARMTRANID)
////                        ORDER BY T1.REPL_BOX_ID";

            _sql = @"SELECT  STORE_SALE_LOTTERY_CLOSING.LTRSAL_GAME_ID, STORE_SALE_LOTTERY_CLOSING.LTRSAL_PACK_NO, 
                         GROUP_LOTTERY_MASTER.LOTTERY_GAME_DESCRIPTION, STORE_SALE_LOTTERY_CLOSING.LTRSAL_BOX_ID, 
                         STORE_SALE_LOTTERY_CLOSING.LTRSAL_LAST_TICKET_NO
                    FROM            STORE_SALE_LOTTERY_CLOSING INNER JOIN
                                             STORE_MASTER ON STORE_SALE_LOTTERY_CLOSING.LTRSAL_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                             GROUP_LOTTERY_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_LOTTERY_MASTER.LOTTERY_GROUP_ID AND 
                                             STORE_SALE_LOTTERY_CLOSING.LTRSAL_GAME_ID = GROUP_LOTTERY_MASTER.LOTTERY_GAME_ID
                    WHERE        (STORE_SALE_LOTTERY_CLOSING.LTRSAL_STORE_ID = PARMSTOREID) AND (STORE_SALE_LOTTERY_CLOSING.LTRSAL_SALE_DAY_TRAN_ID = PARMTRANID)
                    ORDER BY STORE_SALE_LOTTERY_CLOSING.LTRSAL_BOX_ID";

            _sql = _sql.Replace("PARMTRANID", iDayTranID.ToString());
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objLottery = new LotterySale();
                objLottery.BoxID = Convert.ToInt16(dr["LTRSAL_BOX_ID"]);
                objLottery.GameDescription = dr["LOTTERY_GAME_DESCRIPTION"].ToString();
                objLottery.GameID = dr["LTRSAL_GAME_ID"].ToString();
                objLottery.PackNo = dr["LTRSAL_PACK_NO"].ToString();
                if (dr["LTRSAL_LAST_TICKET_NO"].ToString().Length > 0)
                    objLottery.LastTicketClosing = Convert.ToInt16(dr["LTRSAL_LAST_TICKET_NO"]);
                LotteryClosing.Add(objLottery);
            }
            dr.Close();

            return LotteryClosing;
        }

        public List<LotteryTransfer> LotteryCashTransfer(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<LotteryTransfer> LotteryTransfer = new List<LotteryTransfer>();
            LotteryTransfer objLottery;

            _sql = @"SELECT   STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE, STORE_BUSINESS_TRANS.BUSINESS_AMOUNT, 
                                                 STORE_BUSINESS_TRANS.BUSINESS_DISPLAY_NAME
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_TRAN_FROM = 'LC') AND (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = PARMSTOREID) 
                            AND (STORE_SALE_MASTER.SALE_DAY_TRAN_ID = PARMTRANID)";

            _sql = _sql.Replace("PARMTRANID", iDayTranID.ToString());
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objLottery = new LotteryTransfer();
                objLottery.TranType = dr["BUSINESS_TRAN_TYPE"].ToString();
                objLottery.TranAmount = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());
                LotteryTransfer.Add(objLottery);
            }
            dr.Close();

            return LotteryTransfer;
        }


        public List<GasReceipt> GasInvReceipt(int iStoreID, DateTime dDate, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<GasReceipt> GasInvReceiptColl = new List<GasReceipt>();
            GasReceipt GasInvReceipt;

            _sql = @"SELECT GAS_RECEIPT_MASTER.GR_DUE_DATE, GAS_RECEIPT_MASTER.GR_INV_RECT_STATUS, GAS_RECEIPT_MASTER.GR_BOL, 
                         GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID, GAS_RECEIPT_DELIVERY.GRV_SLNO, GAS_RECEIPT_DELIVERY.GRV_GROSS_GALLONS, 
                         GAS_RECEIPT_DELIVERY.GRV_NET_GALLONS, GROUP_GASTYPE_MASTER.GASTYPE_NAME,  GAS_RECEIPT_MASTER.GR_SHIFT_CODE
                    FROM            GAS_RECEIPT_MASTER INNER JOIN
                                             GAS_RECEIPT_DELIVERY ON GAS_RECEIPT_MASTER.GR_STORE_ID = GAS_RECEIPT_DELIVERY.GRV_STORE_ID AND 
                                             GAS_RECEIPT_MASTER.GR_TRAN_ID = GAS_RECEIPT_DELIVERY.GRV_TRAN_ID INNER JOIN
                                             MAPPING_STORE_GAS ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                             GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID INNER JOIN
                                             GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                             STORE_MASTER ON MAPPING_STORE_GAS.STORE_ID = STORE_MASTER.STORE_ID AND 
                                             GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                    WHERE        (GAS_RECEIPT_MASTER.GR_DATE = '" + dDate + " ') AND (GAS_RECEIPT_MASTER.GR_STORE_ID = " + iStoreID + ")";
            _sql += " ORDER BY GAS_RECEIPT_DELIVERY.GRV_SLNO";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                GasInvReceipt = new GasReceipt();
                GasInvReceipt.StoreID = iStoreID;
                GasInvReceipt.ShiftCode = Convert.ToInt16(dr["GR_SHIFT_CODE"].ToString());
                GasInvReceipt.BillOfLading = dr["GR_BOL"].ToString();
                GasInvReceipt.DueDate = Convert.ToDateTime(dr["GR_DUE_DATE"]);
                GasInvReceipt.GasTypeID = Convert.ToInt16(dr["GRV_GAS_TYPE_ID"]);
                GasInvReceipt.GasTypeName = dr["GASTYPE_NAME"].ToString(); ;
                GasInvReceipt.SlNo = Convert.ToInt16(dr["GRV_SLNO"]) ;
                GasInvReceipt.GrossGallons = Convert.ToSingle(dr["GRV_GROSS_GALLONS"]); ;
                GasInvReceipt.NetGallons = Convert.ToSingle(dr["GRV_NET_GALLONS"]);
                GasInvReceiptColl.Add(GasInvReceipt);
            }
            dr.Close();
            return GasInvReceiptColl;
        }

        public List<GasSaleReceipt> GasSaleReceipts(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<GasSaleReceipt> GasSaleRctColl = new List<GasSaleReceipt>();
            GasSaleReceipt GasSaleRct;

            _sql = @"SELECT        STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID, STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT, GROUP_CARD_TYPE.GROU_CARD_NAME
                        FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                 MAPPING_STORE_CARD ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = MAPPING_STORE_CARD.STORE_ID AND 
                                                 STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID = MAPPING_STORE_CARD.CARD_TYPE_ID INNER JOIN
                                                 GROUP_CARD_TYPE ON MAPPING_STORE_CARD.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                        WHERE        (STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = " + iDayTranID + ") AND (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = " + iStoreID + ")";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                GasSaleRct = new GasSaleReceipt();
                GasSaleRct.CardTypeID = Convert.ToInt16(dr["CARD_TYPE_ID"]);
                GasSaleRct.CardName = dr["GROU_CARD_NAME"].ToString();
                GasSaleRct.CardAmount = Convert.ToInt16(dr["CARD_AMOUNT"]);
                GasSaleRctColl.Add(GasSaleRct);
            }
            dr.Close();
            return GasSaleRctColl;
        }

        public List<GasInventoryUpdate> GasSaleInvetory(int iStoreID, int iDayTranID, SqlConnection _conn, SaleMaster objSaleMaster)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<GasInventoryUpdate> GasSaleInvColl = new List<GasInventoryUpdate>();
            GasInventoryUpdate GasSaleInv;

            _sql = @"SELECT  STORE_GAS_SALE_BALANCES.STORE_GAS_TANK_ID, STORE_GAS_SALE_BALANCES.GAS_RECV_PRICE,  STORE_GAS_SALE_BALANCES.GAS_TYPE_ID, STORE_GAS_SALE_BALANCES.GAS_OP_BAL, 
                             STORE_GAS_SALE_BALANCES.GAS_RECV, STORE_GAS_SALE_BALANCES.GAS_ACT_CL_BAL, STORE_GAS_SALE_BALANCES.GAS_SYSTEM_CL_BAL, STORE_GAS_SALE_BALANCES.GAS_STICK_INCHES, 
                             STORE_GAS_SALE_BALANCES.GAS_STICK_GALLONS, STORE_GAS_TANKS_MASTER.GAS_TANK_NAME, GROUP_GASTYPE_MASTER.GASTYPE_NAME, GAS_CAPACITY 
                        FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                 MAPPING_STORE_GAS ON STORE_GAS_SALE_BALANCES.STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                                 STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID INNER JOIN
                                                 STORE_GAS_TANKS_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_GAS_TANKS_MASTER.STORE_ID AND 
                                                 STORE_GAS_SALE_BALANCES.STORE_GAS_TANK_ID = STORE_GAS_TANKS_MASTER.GAS_TANK_ID INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                        WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = " + iStoreID + ") AND (STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = " + iDayTranID + ")";
             _sql +=      " ORDER BY STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                GasSaleInv = new GasInventoryUpdate();
                GasSaleInv.TankID = Convert.ToInt16(dr["STORE_GAS_TANK_ID"]);
                GasSaleInv.TankName = dr["GAS_TANK_NAME"].ToString();
                GasSaleInv.GasTypeID = Convert.ToInt16(dr["GAS_TYPE_ID"]);
                GasSaleInv.GasTypeName = dr["GASTYPE_NAME"].ToString();
                GasSaleInv.OpeningBalance = Convert.ToSingle(dr["GAS_OP_BAL"]);
                GasSaleInv.GasReceived = Convert.ToSingle(dr["GAS_RECV"]);
                GasSaleInv.GasPrice = Convert.ToSingle(dr["GAS_RECV_PRICE"]);
                GasSaleInv.ActualClosingBalance = Convert.ToSingle(dr["GAS_ACT_CL_BAL"]);
                GasSaleInv.SystemClosingBalance = Convert.ToSingle(dr["GAS_SYSTEM_CL_BAL"]);
                GasSaleInv.StickInches = Convert.ToSingle(dr["GAS_STICK_INCHES"]);
                GasSaleInv.StickGallons = Convert.ToSingle(dr["GAS_STICK_GALLONS"]);
                GasSaleInv.TankCapacity = Convert.ToSingle(dr["GAS_CAPACITY"]);

                GasSaleInvColl.Add(GasSaleInv);
            }
            dr.Close();
            
            if (GasSaleInvColl.Count > 0)
            {
                foreach (GasInventoryUpdate o in GasSaleInvColl)
                {
                    if (o.OpeningBalance == 0)
                        o.OpeningBalance = GetOpeningBalance(iStoreID, iDayTranID, o.GasTypeID, _conn);
                }
            }
            else
            {
                foreach (GasOil o in objSaleMaster.GasTypes)
                {
                    GasSaleInv = new GasInventoryUpdate();
                    GasSaleInv.GasTypeID = o.GasTypeID;
                    GasSaleInv.GasTypeName = o.GasTypeName.ToString();
                    GasSaleInv.OpeningBalance = GetOpeningBalance(iStoreID, iDayTranID, o.GasTypeID, _conn);
                    GasSaleInvColl.Add(GasSaleInv);
                }

            }


            return GasSaleInvColl;
        }

        private float GetOpeningBalance(int iStoreID, int iDayTranID,int GasTypeID, SqlConnection _conn)
        {
            _sql = @"SELECT SALE_DAY_TRAN_ID FROM STORE_SALE_MASTER 
                                WHERE SALE_DATE IN
                                (
                                SELECT DATEADD(DAY,-1, MAX(SALE_DATE) ) FROM
                                STORE_SALE_MASTER WHERE STORE_ID = PARMSTOREID AND SALE_DATE IN 
                                (
                                SELECT        SALE_DATE
                                FROM            STORE_SALE_MASTER
                                WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DAY_TRAN_ID = PARMTRANID)
                                )
                                )  AND STORE_ID = PARMSTOREID";

            _sql = _sql.Replace("PARMTRANID", iDayTranID.ToString());
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            int iPrevDayTranID = 0;
            float fGasOpeningBalance = 0;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            if (cmd.ExecuteScalar() != null)
                iPrevDayTranID = Convert.ToInt16(cmd.ExecuteScalar());

            _sql = @"SELECT   STORE_GAS_SALE_BALANCES.GAS_SYSTEM_CL_BAL AS GAS_ACT_CL_BAL
                                FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                    MAPPING_STORE_GAS ON STORE_GAS_SALE_BALANCES.STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                    STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID INNER JOIN
                                    STORE_GAS_TANKS_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_GAS_TANKS_MASTER.STORE_ID AND 
                                    STORE_GAS_SALE_BALANCES.STORE_GAS_TANK_ID = STORE_GAS_TANKS_MASTER.GAS_TANK_ID INNER JOIN
                                    GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                                WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = PARMTRANID) 
						        AND STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = PARMGASTYPEID";

            _sql = _sql.Replace("PARMTRANID", iPrevDayTranID.ToString());
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMGASTYPEID", GasTypeID.ToString());
            cmd = new SqlCommand(_sql, _conn);
            if (cmd.ExecuteScalar() != null)
                fGasOpeningBalance = Convert.ToInt64(cmd.ExecuteScalar());

            return fGasOpeningBalance;
        }

        public List<LotterySale> LotterySale(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<LotterySale> GasSaleLotteryColl = new List<LotterySale>();
            LotterySale GasSaleLottery;

            _sql = @"SELECT  STORE_LOTTERY_SALE_INDIVIDUAL.LOTTER_ID, STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_SERIAL_NO, 
                         STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_CLOSING_SERIAL, STORE_LOTTERY_SALE_INDIVIDUAL.LOTTERY_NUMBER_OF_BOOKS_ACTIVE, 
                         MAPPING_STORE_LOTTERY.LOTTERY_NAME
                    FROM            STORE_LOTTERY_SALE_INDIVIDUAL INNER JOIN
                                                MAPPING_STORE_LOTTERY ON STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID = MAPPING_STORE_LOTTERY.STORE_ID AND 
                                                STORE_LOTTERY_SALE_INDIVIDUAL.LOTTER_ID = MAPPING_STORE_LOTTERY.LOTTER_ID
                    WHERE        (STORE_LOTTERY_SALE_INDIVIDUAL.STORE_ID = " + iStoreID + ") AND (STORE_LOTTERY_SALE_INDIVIDUAL.SALE_DAY_TRAN_ID = " + iDayTranID + ")";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                GasSaleLottery = new LotterySale();
                //GasSaleLottery.LotteryID = Convert.ToInt16(dr["LOTTER_ID"]);
                //GasSaleLottery.LotterName = dr["LOTTERY_NAME"].ToString();
                //GasSaleLottery.LotterySerialNo = Convert.ToInt16(dr["LOTTERY_SERIAL_NO"]);
                //GasSaleLottery.LotteryClosingSerial = Convert.ToInt16(dr["LOTTERY_CLOSING_SERIAL"]);
                //GasSaleLottery.NumberOfBooksActive = Convert.ToInt16(dr["LOTTERY_NUMBER_OF_BOOKS_ACTIVE"]);
                GasSaleLotteryColl.Add(GasSaleLottery);
            }
            dr.Close();
            return GasSaleLotteryColl;
        }

        public List<AccountPaidReceivables> ListOfPayments(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<AccountPaidReceivables> objAccountReceivableColl = new List<AccountPaidReceivables>();
            AccountPaidReceivables objAccountReceivable;

            _sql = @"SELECT   CASE BUSINESS_TRAN_TYPE WHEN 'CP' THEN 'CASH' ELSE 'CHEQUE' END AS PAYMENT_MODE, 
                         BUSINESS_ACTLED_ID, BUSINESS_AMOUNT, BUSINESS_DISPLAY_NAME, BUSINESS_REMARKS
                        FROM            STORE_BUSINESS_TRANS
                        WHERE        (BUSINESS_STORE_ID = " + iStoreID + ") AND (BUSINESS_SALE_DAY_TRAN_ID = " + iDayTranID + ") AND (BUSINESS_TRAN_TYPE IN ('CP', 'QP'))";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objAccountReceivable = new AccountPaidReceivables();
                objAccountReceivable.AccountLedgerID = Convert.ToInt16(dr["BUSINESS_ACTLED_ID"]);
                objAccountReceivable.DisplayName = dr["BUSINESS_DISPLAY_NAME"].ToString();
                objAccountReceivable.AccountTranType = dr["PAYMENT_MODE"].ToString();
                objAccountReceivable.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"]);
                objAccountReceivable.PaymentRemarks = dr["BUSINESS_REMARKS"].ToString();


                objAccountReceivableColl.Add(objAccountReceivable);
            }
            dr.Close();
            return objAccountReceivableColl;
        }

        public List<AccountPaidReceivables> ListOfReceipts(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<AccountPaidReceivables> objAccountReceivableColl = new List<AccountPaidReceivables>();
            AccountPaidReceivables objAccountReceivable;

            _sql = @"SELECT   CASE BUSINESS_TRAN_TYPE WHEN 'CR' THEN 'CASH' ELSE 'CHEQUE' END AS RECEIPT_MODE, 
                         BUSINESS_ACTLED_ID, BUSINESS_AMOUNT, BUSINESS_DISPLAY_NAME, BUSINESS_REMARKS
                        FROM            STORE_BUSINESS_TRANS
                        WHERE        (BUSINESS_STORE_ID = " + iStoreID + ") AND (BUSINESS_SALE_DAY_TRAN_ID = " + iDayTranID + ") AND (BUSINESS_TRAN_TYPE IN ('CR', 'QR'))";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objAccountReceivable = new AccountPaidReceivables();
                objAccountReceivable.AccountLedgerID = Convert.ToInt16(dr["BUSINESS_ACTLED_ID"]);
                objAccountReceivable.DisplayName = dr["BUSINESS_DISPLAY_NAME"].ToString();
                objAccountReceivable.AccountTranType = dr["RECEIPT_MODE"].ToString();
                objAccountReceivable.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"]);
                objAccountReceivable.PaymentRemarks = dr["BUSINESS_REMARKS"].ToString();


                objAccountReceivableColl.Add(objAccountReceivable);
            }
            dr.Close();
            return objAccountReceivableColl;
        }

        public List<Purchase> ListOfPurchasesReturns(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<Purchase> objPurchaseColl = new List<Purchase>();
            Purchase objPurchase;

            _sql = @"SELECT      PURCHASE_MASTER.PRC_SUPPLIER_ID, ACCOUNTS_ACTLED_STORE.ACTLED_NAME, PURCHASE_MASTER.PRC_INV_CRD_DT, 
                             PURCHASE_MASTER.PRC_INV_CRD_NO, PURCHASE_MASTER.PRC_INV_CRD_AMOUNT, PURCHASE_MASTER.PRC_DUE_DATE, 
                             PURCHASE_MASTER.PRC_INVCRD_REMARKS
                        FROM            PURCHASE_MASTER INNER JOIN
                                                 ACCOUNTS_ACTLED_STORE ON PURCHASE_MASTER.PRC_STORE_ID = ACCOUNTS_ACTLED_STORE.STORE_ID AND 
                                                 PURCHASE_MASTER.PRC_SUPPLIER_ID = ACCOUNTS_ACTLED_STORE.ACTLED_ID
                        WHERE        (PURCHASE_MASTER.PRC_STORE_ID = STOREID) AND (PURCHASE_MASTER.PRC_SALE_TRAN_ID = DAYTRANID) AND (PURCHASE_MASTER.PRC_INV_CRD_TYPE = 'C')
                        ORDER BY PURCHASE_MASTER.PRC_TRAN_ID";

            _sql = _sql.Replace("STOREID", iStoreID.ToString());
            _sql = _sql.Replace("DAYTRANID", iDayTranID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objPurchase = new Purchase();
                objPurchase.SupplierNo = Convert.ToInt32( dr["PRC_SUPPLIER_ID"].ToString());
                objPurchase.SupplierName = dr["ACTLED_NAME"].ToString();
                objPurchase.InvCrdDate = Convert.ToDateTime( dr["PRC_INV_CRD_DT"].ToString());
                objPurchase.InvCrdNumber = dr["PRC_INV_CRD_NO"].ToString();
                objPurchase.InvCrdAmount = Convert.ToSingle( dr["PRC_INV_CRD_AMOUNT"].ToString());
                if (dr["PRC_DUE_DATE"].ToString().Length > 0)
                    objPurchase.DueDate = Convert.ToDateTime(dr["PRC_DUE_DATE"].ToString());

                if (dr["PRC_INVCRD_REMARKS"].ToString().Length > 0)
                    objPurchase.Remarks = dr["PRC_INVCRD_REMARKS"].ToString();

                objPurchaseColl.Add(objPurchase);
            }
            dr.Close();
            return objPurchaseColl;
        }

        public List<ChequeCashingTran> ListOfChequeCashingEntries(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<ChequeCashingTran> objChequeCashingColl = new List<ChequeCashingTran>();
            ChequeCashingTran objChequeCashing;

            _sql = @"SELECT  CHQCS_CHQNO, CHQCS_BANK_NAME, CHQCS_CHQ_AMOUNT, CHQCS_PAID_AMOUNT, CHQCS_COMMISSION, CHQCS_REMARKS
                        FROM            CHEQUE_CASHING_TRAN
                        WHERE        (CHQCS_STORE_ID = STOREID) AND (CHQCS_SALE_TRAN_ID = DAYTRANID) ORDER BY CHQCS_TRAN_ID";

            _sql = _sql.Replace("STOREID", iStoreID.ToString());
            _sql = _sql.Replace("DAYTRANID", iDayTranID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objChequeCashing = new ChequeCashingTran();
                objChequeCashing.ChequeNo = dr["CHQCS_CHQNO"].ToString();
                objChequeCashing.BankName = dr["CHQCS_BANK_NAME"].ToString();
                objChequeCashing.ChequeAmount = Convert.ToSingle(dr["CHQCS_CHQ_AMOUNT"].ToString());
                objChequeCashing.PaidAmount = Convert.ToSingle(dr["CHQCS_PAID_AMOUNT"].ToString());
                objChequeCashing.Commission = Convert.ToSingle(dr["CHQCS_COMMISSION"].ToString());
                objChequeCashing.Remarks = dr["CHQCS_REMARKS"].ToString();

                objChequeCashingColl.Add(objChequeCashing);
            }
            dr.Close();
            return objChequeCashingColl;
        }

        public List<ChequeCashingTran> ListOfChequeToBeDeposited(int iStoreID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<ChequeCashingTran> objChequeCashingColl = new List<ChequeCashingTran>();
            ChequeCashingTran objChequeCashing;

            _sql = @"SELECT  CHQCS_TRAN_ID, CHQCS_CHQNO, CHQCS_BANK_NAME, CHQCS_CHQ_AMOUNT, CHQCS_PAID_AMOUNT, CHQCS_COMMISSION, CHQCS_REMARKS
                        FROM            CHEQUE_CASHING_TRAN
                        WHERE        (CHQCS_STORE_ID = STOREID) AND (CHQCS_DEPOSIT_STATUS = 'N') ORDER BY CHQCS_TRAN_ID";

            _sql = _sql.Replace("STOREID", iStoreID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objChequeCashing = new ChequeCashingTran();
                objChequeCashing.ChequeNo = dr["CHQCS_CHQNO"].ToString();
                objChequeCashing.BankName = dr["CHQCS_BANK_NAME"].ToString();
                objChequeCashing.ChequeAmount = Convert.ToSingle(dr["CHQCS_CHQ_AMOUNT"].ToString());
                objChequeCashing.PaidAmount = Convert.ToSingle(dr["CHQCS_PAID_AMOUNT"].ToString());
                objChequeCashing.Commission = Convert.ToSingle(dr["CHQCS_COMMISSION"].ToString());
                objChequeCashing.Remarks = dr["CHQCS_REMARKS"].ToString();
                objChequeCashing.ChequeTranID = Convert.ToInt64(dr["CHQCS_TRAN_ID"].ToString());

                objChequeCashingColl.Add(objChequeCashing);
            }
            dr.Close();
            return objChequeCashingColl;
        }

        public List<Purchase> ListOfPurchases(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            SqlCommand cmd;
            SqlDataReader dr;
            List<Purchase> objPurchaseColl = new List<Purchase>();
            Purchase objPurchase;

            _sql = @"SELECT      PURCHASE_MASTER.PRC_SUPPLIER_ID, ACCOUNTS_ACTLED_STORE.ACTLED_NAME, PURCHASE_MASTER.PRC_INV_CRD_DT, 
                             PURCHASE_MASTER.PRC_INV_CRD_NO, PURCHASE_MASTER.PRC_INV_CRD_AMOUNT, PURCHASE_MASTER.PRC_DUE_DATE, 
                             PURCHASE_MASTER.PRC_INVCRD_REMARKS
                        FROM            PURCHASE_MASTER INNER JOIN
                                                 ACCOUNTS_ACTLED_STORE ON PURCHASE_MASTER.PRC_STORE_ID = ACCOUNTS_ACTLED_STORE.STORE_ID AND 
                                                 PURCHASE_MASTER.PRC_SUPPLIER_ID = ACCOUNTS_ACTLED_STORE.ACTLED_ID
                        WHERE        (PURCHASE_MASTER.PRC_STORE_ID = STOREID) AND (PURCHASE_MASTER.PRC_SALE_TRAN_ID = DAYTRANID) AND (PURCHASE_MASTER.PRC_INV_CRD_TYPE = 'I')
                        ORDER BY PURCHASE_MASTER.PRC_TRAN_ID";

            _sql = _sql.Replace("STOREID", iStoreID.ToString());
            _sql = _sql.Replace("DAYTRANID", iDayTranID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objPurchase = new Purchase();
                objPurchase.SupplierNo = Convert.ToInt32(dr["PRC_SUPPLIER_ID"].ToString());
                objPurchase.SupplierName = dr["ACTLED_NAME"].ToString();
                objPurchase.InvCrdDate = Convert.ToDateTime(dr["PRC_INV_CRD_DT"].ToString());
                objPurchase.InvCrdNumber = dr["PRC_INV_CRD_NO"].ToString();
                objPurchase.InvCrdAmount = Convert.ToSingle(dr["PRC_INV_CRD_AMOUNT"].ToString());
                if (dr["PRC_DUE_DATE"].ToString().Length > 0)
                    objPurchase.DueDate = Convert.ToDateTime(dr["PRC_DUE_DATE"].ToString());

                if (dr["PRC_INVCRD_REMARKS"].ToString().Length > 0) 
                    objPurchase.Remarks = dr["PRC_INVCRD_REMARKS"].ToString();

                objPurchaseColl.Add(objPurchase);
            }
            dr.Close();
            return objPurchaseColl;
        }
        
        public List<AccountModel> BusinessTran(int iStoreID, DateTime dDate)
        {
            List<AccountModel> BusienssTranColl = new List<AccountModel>();
            List<AccountModel> BusienssTranCollTemp = new List<AccountModel>();
            AccountMasterDal _dalAccountMaster = new AccountMasterDal();

            BusienssTranCollTemp = _dalAccountMaster.SelectRecords(iStoreID, "BS", dDate);
            foreach(AccountModel obj in BusienssTranCollTemp)
            {
                BusienssTranColl.Add(obj);
            }
            BusienssTranCollTemp.Clear();

            BusienssTranCollTemp = _dalAccountMaster.SelectRecords(iStoreID, "BP", dDate);
            foreach (AccountModel obj in BusienssTranCollTemp)
            {
                BusienssTranColl.Add(obj);
            }
            BusienssTranCollTemp.Clear();
            return BusienssTranColl;
        }

        public List<AccountModel> BusinessTran(int iStoreID, int iDayTranID, SqlConnection _conn)
        {
            List<AccountModel> BusienssTranColl = new List<AccountModel>();
            AccountModel obj;

            _sql = @"SELECT  BUSINESS_STORE_ID, BUSINESS_SALE_DAY_TRAN_ID, BUSINESS_TRAN_TYPE, BUSINESS_TRAN_FROM, BUSINESS_ACTLED_ID, 
                         BUSINESS_AMOUNT, BUSINESS_DISPLAY_NAME, BUSINESS_REMARKS, BUSINESS_PMT_TYPE
                        FROM            STORE_BUSINESS_TRANS
                        WHERE        (BUSINESS_STORE_ID = PARMSTOREID) AND (BUSINESS_TRAN_TYPE IN ('BP', 'BS')) AND (BUSINESS_SALE_DAY_TRAN_ID = PARMDAYTRANID)";

            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMDAYTRANID", iDayTranID.ToString());

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                obj = new AccountModel();
                obj.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"]);
                obj.DisplaySide = dr["BUSINESS_TRAN_TYPE"].ToString();
                obj.StoreID = Convert.ToInt16(dr["BUSINESS_STORE_ID"]);
                obj.LedgerID = Convert.ToInt16(dr["BUSINESS_ACTLED_ID"]);
                obj.LedgerName = dr["BUSINESS_DISPLAY_NAME"].ToString();
                if (dr["BUSINESS_PMT_TYPE"] == null)
                    obj.PaymentType = "CS";
                else
                {
                    if (dr["BUSINESS_PMT_TYPE"].ToString().Length == 0)
                        obj.PaymentType = "CS";
                    else
                        obj.PaymentType = dr["BUSINESS_PMT_TYPE"].ToString();
                }
                BusienssTranColl.Add(obj);
            }
            dr.Close();

            return BusienssTranColl;
        }

        public List<AccountModel> CashInOutFlow(int iStoreID, DateTime dDate, int ShiftCode)
        {
            int iDayTranID;

            List<AccountModel> BusienssTranColl = new List<AccountModel>();
            AccountModel obj;
            SqlConnection _conn;
            SqlDataReader dr;
            SqlCommand cmd;
            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            try
            {
                iDayTranID = new SaleSupportEntries().DayTranID(dDate, iStoreID, ShiftCode, _conn);                

                #region Opening Balance
                _sql = @"SELECT        TOP (1) CASH_PHYSICAL_AT_STORE, SALE_DATE, SALE_SHIFT_CODE
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DAY_TRAN_ID < PARMDAYTRANID)
                        ORDER BY SALE_DAY_TRAN_ID DESC";
                _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID", iDayTranID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    obj = new AccountModel();
                    obj.Amount = Convert.ToSingle(dr["CASH_PHYSICAL_AT_STORE"]);
                    obj.DisplaySide = "BS";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.LedgerName = "Opening balance on date " + dr["SALE_DATE"].ToString() + ", shift " + dr["SALE_SHIFT_CODE"].ToString();
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);
                }
                dr.Close();
                #endregion

                #region Gas and Lottery entries
                _sql = @"SELECT SALE_GAS_TOTAL_SALE, SALE_GAS_CARD_TOTAL, SALE_LOTTERY_SALE + SALE_LOTTERY_ONLINE AS LOTTERY_SALE,
                          SALE_LOTTERY_CASH_INSTANT_PAID + SALE_LOTTERY_CASH_ONLINE_PAID AS LOTTERY_PAIDOUT, CASH_DEPOSITED_IN_BANK, 
                          CASH_PHYSICAL_AT_STORE
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DAY_TRAN_ID = PARMDAYTRANID)";

                _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID", iDayTranID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    obj = new AccountModel();
                    obj.LedgerName = "Gas Sale";
                    obj.Amount = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"]);
                    obj.DisplaySide = "BS";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);

                    obj = new AccountModel();
                    obj.LedgerName = "Lottery Sale";
                    obj.Amount = Convert.ToSingle(dr["LOTTERY_SALE"]);
                    obj.DisplaySide = "BS";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);

                    obj = new AccountModel();
                    obj.LedgerName = "Card Receipt";
                    obj.Amount = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"]);
                    obj.DisplaySide = "BP";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);

                    obj = new AccountModel();
                    obj.LedgerName = "Lottery Paid-Out";
                    obj.Amount = Convert.ToSingle(dr["LOTTERY_PAIDOUT"]);
                    obj.DisplaySide = "BP";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);

                    obj = new AccountModel();
                    obj.LedgerName = "Total Bank Deposit";
                    obj.Amount = Convert.ToSingle(dr["CASH_DEPOSITED_IN_BANK"]);
                    obj.DisplaySide = "BP";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);

                    obj = new AccountModel();
                    obj.LedgerName = "Cash at Store";
                    obj.Amount = Convert.ToSingle(dr["CASH_PHYSICAL_AT_STORE"]);
                    obj.DisplaySide = "CB";
                    obj.StoreID = iStoreID;
                    obj.LedgerID = 0;
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);
                }
                dr.Close();
                #endregion

                #region Business Transactions
                _sql = @"SELECT  BUSINESS_STORE_ID, BUSINESS_SALE_DAY_TRAN_ID, BUSINESS_TRAN_TYPE, BUSINESS_TRAN_FROM, BUSINESS_ACTLED_ID,BUSINESS_PMT_TYPE, 
                         BUSINESS_AMOUNT, BUSINESS_DISPLAY_NAME, BUSINESS_REMARKS
                        FROM            STORE_BUSINESS_TRANS
                        WHERE        (BUSINESS_STORE_ID = PARMSTOREID) AND (BUSINESS_TRAN_TYPE IN ('BP', 'BS','CP','CR')) AND (BUSINESS_SALE_DAY_TRAN_ID = PARMDAYTRANID)";

                _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID", iDayTranID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new AccountModel();
                    obj.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"]);
                    if (dr["BUSINESS_PMT_TYPE"].ToString() == "CQ")
                        obj.DisplaySide = dr["BUSINESS_PMT_TYPE"].ToString();
                    else
                        obj.DisplaySide = dr["BUSINESS_TRAN_TYPE"].ToString();

                    obj.StoreID = Convert.ToInt16(dr["BUSINESS_STORE_ID"]);
                    obj.LedgerID = Convert.ToInt16(dr["BUSINESS_ACTLED_ID"]);
                    obj.LedgerName = dr["BUSINESS_DISPLAY_NAME"].ToString();
                    obj.Date = dDate;
                    BusienssTranColl.Add(obj);
                }
                dr.Close();
                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _conn.Close();
            }
            return BusienssTranColl;
        }


        public BankDepositForm BankDepositDetails(int iStoreID, DateTime dDate, int iShiftCode)
        {
            BankDepositForm objDepositColl = new BankDepositForm();
            BankDeposit objDeposit;
            List<BankDeposit> objDeposits = new List<BankDeposit>();
            SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

            SqlConnection _conn;
            SqlCommand cmd;
            SqlDataReader dr;
            float fSale = 0;
            float fPaid = 0;
            float fDeposit = 0;
            int iDayTranID;

            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            try
            {

                #region Adding Business Deposit
                iDayTranID = _SaleSupportEntries.DayTranID(dDate, iStoreID, iShiftCode, _conn);

                _sql = @"SELECT        SUM(ACTCASH_AMOUNT) AS TOTAL_AMOUNT 
                        FROM            ACTCASH_TRAN
                        WHERE        (ACTCASH_TRAN_TYPE = 'BS')  AND (ACTCASH_INWARD_OUTWARD_TYPE = 'I') AND 
                        (ACTCASH_STORE_ID = " + iStoreID + ") AND (ACTCASH_DATE = '" + dDate + "') AND (ACTCAST_SUB_TRAN_TYPE = 'N') AND (ACTCASH_SALE_PMT_DAY_TRAN_ID = " + iDayTranID.ToString() + ")";

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["TOTAL_AMOUNT"].ToString().Length > 0)
                        fSale = Convert.ToSingle(dr[0]);
                }
                dr.Close();

                _sql = @"SELECT SUM(BUSINESS_AMOUNT) AS TAX_COLLECTED
                            FROM            STORE_BUSINESS_TRANS
                            WHERE       ((BUSINESS_ACTLED_ID = 8) OR (BUSINESS_STORE_ID = 1 AND BUSINESS_ACTLED_ID = 79)
                            OR (BUSINESS_STORE_ID = 2 AND BUSINESS_ACTLED_ID = 85)) AND BUSINESS_STORE_ID = PARMSTOREID
                            AND BUSINESS_SALE_DAY_TRAN_ID = PARMDAYTRANID";

                _sql = _sql.Replace("PARMSTOREID",iStoreID.ToString());
                _sql = _sql.Replace("PARMDAYTRANID", iDayTranID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["TAX_COLLECTED"].ToString().Length > 0)
                        fSale = fSale - Convert.ToSingle(dr["TAX_COLLECTED"]);
                }
                dr.Close();



                _sql = @"SELECT        SUM(ACTCASH_AMOUNT) AS TOTAL_AMOUNT 
                        FROM            ACTCASH_TRAN
                        WHERE        (ACTCASH_TRAN_TYPE IN ('BP','CP'))  AND (ACTCASH_INWARD_OUTWARD_TYPE = 'O')  AND (ACTCAST_SUB_TRAN_TYPE = 'N') AND 
                        (ACTCASH_STORE_ID = " + iStoreID + ") AND (ACTCASH_DATE = '" + dDate + "')  AND (ACTCASH_SALE_PMT_DAY_TRAN_ID = " + iDayTranID.ToString() + ")";

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["TOTAL_AMOUNT"].ToString().Length > 0)
                        fPaid = Convert.ToSingle(dr[0]);
                }
                dr.Close();

                fDeposit = GetDepositPerAccount(iStoreID, dDate, 26, iShiftCode);

                //Adding Business Sale
                objDeposit = new BankDeposit();
                objDeposit.LedgerID = 26;
                objDeposit.LedgerName = "TOTAL BUSINESS COLLECTION";
                objDeposit.LedgerSale = fSale;
                objDeposit.LedgerPaid = fPaid;
                objDeposit.Balance = fSale - fPaid;
                objDeposit.Deposit = fDeposit;
                objDeposits.Add(objDeposit);
                #endregion

                #region Adding Gas and Lotter Deposit
                _sql = @"SELECT   SALE_GAS_TOTAL_SALE, SALE_GAS_CARD_TOTAL, 
                                                 SALE_LOTTERY_SALE + SALE_LOTTERY_ONLINE - SALE_LOTTERY_RETURN AS TOTAL_LOTTERY_SALE, 
                                                 SALE_LOTTERY_CASH_INSTANT_PAID + SALE_LOTTERY_CASH_ONLINE_PAID AS TOTAL_LOTTERY_PAID, CASH_OPENING_BALANCE, 
                                                 CASH_PHYSICAL_AT_STORE, CASH_DEPOSITED_IN_BANK, CASH_CLOSING_BALANCE, SALE_ENTRY_LOCKED
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + dDate + "')";

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    // Adding Gas sale
                    fSale = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"]);
                    fPaid = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"]);
                    fDeposit = GetDepositPerAccount(iStoreID, dDate, 25,iShiftCode);

                    objDeposit = new BankDeposit();
                    objDeposit.LedgerID = 25;
                    objDeposit.LedgerName = "GAS SALE";
                    objDeposit.LedgerSale = fSale;
                    objDeposit.LedgerPaid = fPaid;
                    objDeposit.Balance = fSale - fPaid;
                    objDeposit.Deposit = fDeposit;
                    objDeposits.Add(objDeposit);

                    // Adding Lottery sale
                    fSale = Convert.ToSingle(dr["TOTAL_LOTTERY_SALE"]);
                    fPaid = Convert.ToSingle(dr["TOTAL_LOTTERY_PAID"]);
                    fDeposit = GetDepositPerAccount(iStoreID, dDate, 24, iShiftCode);

                    objDeposit = new BankDeposit();
                    objDeposit.LedgerID = 24;
                    objDeposit.LedgerName = "LOTTERY SALE";
                    objDeposit.LedgerSale = fSale;
                    objDeposit.LedgerPaid = fPaid;
                    objDeposit.Balance = fSale - fPaid;
                    objDeposit.Deposit = fDeposit;
                    objDeposits.Add(objDeposit);

                    objDepositColl.CashInHand = Convert.ToSingle(dr["CASH_PHYSICAL_AT_STORE"]);
                    objDepositColl.DepositedInBank = Convert.ToSingle(dr["CASH_DEPOSITED_IN_BANK"]);
                    objDepositColl.CashClosingBalance = Convert.ToSingle(dr["CASH_CLOSING_BALANCE"]);
                }
                objDepositColl.LedgerDetail = objDeposits;

                dr.Close();

                #endregion

                #region Adding Money Order Deposit
                fSale = 0;
                fPaid = 0;

                // Money order collection
                _sql = @"SELECT        SUM(ACTCASH_AMOUNT) AS AMOUNT
                            FROM            ACTCASH_TRAN
                            WHERE        (ACTCASH_STORE_ID = MAPSTOREID) AND (ACTCASH_INWARD_OUTWARD_TYPE = 'I') AND (ACTCAST_SUB_TRAN_TYPE = 'MO') 
                                AND (ACTCASH_TRAN_TYPE = 'BS') AND 
                                (ACTCASH_DATE >= '" + dDate + "') AND (ACTCASH_DATE <= '" + dDate + "')";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["AMOUNT"].ToString().Length > 0)
                    {
                        fSale = Convert.ToSingle(dr["AMOUNT"]);
                    }
                }
                dr.Close();

                // Money order paidout
                _sql = @"SELECT        SUM(ACTCASH_AMOUNT) AS AMOUNT
                            FROM            ACTCASH_TRAN
                            WHERE        (ACTCASH_STORE_ID = MAPSTOREID) AND (ACTCASH_INWARD_OUTWARD_TYPE = 'O') AND (ACTCAST_SUB_TRAN_TYPE = 'MO') 
                                AND (ACTCASH_TRAN_TYPE = 'BP') AND 
                                (ACTCASH_DATE >= '" + dDate + "') AND (ACTCASH_DATE <= '" + dDate + "')";

                _sql = _sql.Replace("MAPSTOREID", iStoreID.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["AMOUNT"].ToString().Length > 0)
                    {
                        fPaid = Convert.ToSingle(dr["AMOUNT"]);
                    }
                }
                dr.Close();

                fDeposit = GetDepositPerAccount(iStoreID, dDate, 34, iShiftCode);

                objDeposit = new BankDeposit();
                objDeposit.LedgerID = 34;
                objDeposit.LedgerName = "MONEY ORDER ";
                objDeposit.LedgerSale = fSale;
                objDeposit.LedgerPaid = fPaid;
                objDeposit.Balance = fSale - fPaid;
                objDeposit.Deposit = fDeposit;
                objDeposits.Add(objDeposit);
                #endregion

                // Previous day cash opening balance
                fDeposit = 0;
//                _sql = @"SELECT       CASH_OPENING_BALANCE
//                        FROM            STORE_SALE_MASTER
//                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE = '" + dDate + "') ORDER BY SALE_DATE DESC";

//                cmd = new SqlCommand(_sql, _conn);
//                dr = cmd.ExecuteReader();
//                if (dr.Read())
//                {
//                    fDeposit = Convert.ToSingle(dr["CASH_OPENING_BALANCE"]);
//                }
//                dr.Close();


                if (fDeposit == 0)
                {
                    _sql = @"SELECT        TOP (1) CASH_CLOSING_BALANCE
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + iStoreID + ") AND (SALE_DATE < '" + dDate + "') ORDER BY SALE_DATE DESC";

                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        objDepositColl.CashOpeningBalance = Convert.ToSingle(dr["CASH_CLOSING_BALANCE"]);
                        fDeposit = objDepositColl.CashOpeningBalance;
                    }
                    dr.Close();
                }

                if (fDeposit == 0)
                {
                    _sql = @"SELECT        ACTTRN_AMOUNT
                            FROM            ACTTRN_TRANS
                            WHERE        (ACTTRN_STORE_ID = " + iStoreID + ") AND (ACTTRN_TYPE = 'DR') AND (ACTTRN_ACTLED_ID = 1) AND  ACTTRN_RECORD_TYPE = 'OP'";
                    cmd = new SqlCommand(_sql, _conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        objDepositColl.CashOpeningBalance = Convert.ToSingle(dr["ACTTRN_AMOUNT"]);
                    }
                    dr.Close();
                }

                return objDepositColl;
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

        private float GetDepositPerAccount(int iStoreID, DateTime dDate, int iLedgerID, int iShiftCode)
        {
            SqlConnection conn;
            float fDeposit = 0;

            conn = new SqlConnection(DMLExecute.con);
            conn.Open();

            try
            {
                string maptrantype = "BD";
                if (iLedgerID == 24)
                    maptrantype = "LD";
                else if (iLedgerID == 25)
                    maptrantype = "GD";
                else if (iLedgerID == 34)
                    maptrantype = "MD";

                _sql = @"SELECT        STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID, STORE_BUSINESS_TRANS.BUSINESS_AMOUNT, 
                                                 STORE_BUSINESS_TRANS.BUSINESS_DISPLAY_NAME
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID = LEDGERID) AND (STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE = 'MAPTRANTYPE') AND 
                                    (STORE_SALE_MASTER.SALE_SHIFT_CODE = SHIFTCODE) AND 
                                     (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = " + iStoreID + ")  AND (STORE_SALE_MASTER.SALE_DATE = '" + dDate + "')";

                _sql = _sql.Replace("LEDGERID", iLedgerID.ToString());
                _sql = _sql.Replace("MAPTRANTYPE", maptrantype);
                _sql = _sql.Replace("SHIFTCODE", iShiftCode.ToString());

                SqlDataReader dr;
                SqlCommand cmd;
                

                cmd = new SqlCommand(_sql, conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["BUSINESS_AMOUNT"].ToString().Length > 0)
                        fDeposit = Convert.ToSingle(dr["BUSINESS_AMOUNT"]);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
                conn.Close();
            }
            return fDeposit;
        }

        public LotteryRunningShift GetRunningShift(int ID)
        {
            SqlConnection _conn;
            _conn = new SqlConnection(DMLExecute.con);
            _conn.Open();

            try
            {
                LotteryRunningShift objShift;
                NextShift nextShift = new NextShift();
                SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

                _sql = @"SELECT   MAX(SALE_DATE) AS SALE_DATE, MAX(SALE_SHIFT_CODE) AS SALE_SHIFT_CODE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = " + ID + ") AND ( BUSINESS_SHIFT_OPEN = 'CLOSE')";

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
                        nextShift = _SaleSupportEntries.NextShift(objShift.CurrentDate, ID, objShift.ShiftCode, _conn);
                        objShift.CurrentDate = nextShift.NextDate;
                        objShift.ShiftCode = nextShift.NextShiftID;
                    }

                    else
                    {
                        dr.Close();
                        objShift.CurrentDate = DateTime.Now.Date;
                        objShift.ShiftCode = 1;
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

    }
}
