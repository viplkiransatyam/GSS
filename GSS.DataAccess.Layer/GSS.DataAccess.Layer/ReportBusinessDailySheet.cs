using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class ReportBusinessDailySheet
    {
        SqlConnection _conn;
        string _sql;
        private int StoreID;
        private string MonthName;
        private int Year;
        string sFromDate;
        string sEndDate;
        DateTime dFromDate;
        DateTime dEndDate;

        public List<BusinessDailySheet> GetBusinessDailySheet(int iStoreID, string sMonth, int iYear)
        {
            List<BusinessDailySheet> objRepColl = new List<BusinessDailySheet>();
            BusinessDailySheet objMonStmt;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                StoreID = iStoreID;
                MonthName = sMonth;
                Year = iYear;

                sFromDate = "01-" + MonthName + "-" + Year.ToString();
                dFromDate = Convert.ToDateTime(sFromDate);
                dEndDate = dFromDate.AddMonths(1).AddDays(-1);
                sEndDate = String.Format("{0:dd-MMM-yyyy}", dEndDate);

                List<BusinessDailySheet> objTrans1 = FillBusinessSheet(iStoreID, dFromDate, dEndDate);
                List<BusinessDailySheet> objTrans2 = FillTransactionSheet(iStoreID, dFromDate, dEndDate);
                List<BusinessDailySheet> objTrans3 = FillTransactionLedgers(iStoreID, dFromDate, dEndDate);
                BusinessDailySheet objTemp;

                while (dFromDate <= dEndDate)
                {
                    objMonStmt = new BusinessDailySheet();

                    objMonStmt.Date = dFromDate;

                    objTemp = new BusinessDailySheet();
                    objTemp = objTrans1.Find(x => x.Date == dFromDate);

                    if (objTemp != null)
                    {
                        objMonStmt.Gas = objTemp.Gas;
                        objMonStmt.CreditCard = objTemp.CreditCard;
                        objMonStmt.Lottery = objTemp.Lottery;
                        objMonStmt.LotteryPaidout = objTemp.LotteryPaidout;
                        objMonStmt.CashOnHand = objTemp.CashOnHand;
                        objMonStmt.BankDeposit = objTemp.BankDeposit;  
                    }

                    objTemp = objTrans2.Find(x => x.Date == dFromDate);

                    if (objTemp != null)
                    {
                        objMonStmt.BorrowFromOthers = objTemp.BorrowFromOthers;
                        objMonStmt.BankDeposit = objTemp.BankDeposit;
                        objMonStmt.CashPaid = objTemp.CashPaid;
                        objMonStmt.CreditCard = objTemp.CreditCard;
                    }

                    objTemp = objTrans3.Find(x => x.Date == dFromDate);

                    if (objTemp != null)
                    {
                        objMonStmt.UPAD = objTemp.UPAD;
                        objMonStmt.BusinessSavings = objTemp.BusinessSavings;
                    }
                    objRepColl.Add(objMonStmt);
                    dFromDate = dFromDate.AddDays(1);
                }

                foreach (BusinessDailySheet o in objRepColl)
                {
                    o.CashBalance = o.Business + o.Lottery + o.Gas + o.BorrowFromOthers - o.LotteryPaidout - o.CreditCard - o.CashPaid - o.LoanToOthers - o.UPAD - o.BusinessSavings - o.BankDeposit;
                    o.OverShort = o.CashBalance - o.CashOnHand;
                }
                
                return objRepColl;
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

        private List<BusinessDailySheet> FillBusinessSheet(int storeID, DateTime fromDate, DateTime toDate)
        {
            List<BusinessDailySheet> objRepColl = new List<BusinessDailySheet>();
            BusinessDailySheet objMonStmt;
            _sql = @"SELECT  SALE_DATE, SUM(SALE_GAS_TOTAL_SALE) AS SALE_GAS_TOTAL_SALE, SUM(SALE_GAS_CARD_TOTAL) AS SALE_GAS_CARD_TOTAL, 
                         SUM(SALE_LOTTERY_SALE + SALE_LOTTERY_ONLINE) AS LOTTERY_SALE, 
                         SUM(SALE_LOTTERY_CASH_INSTANT_PAID + SALE_LOTTERY_CASH_ONLINE_PAID) AS LOTTERY_PAIDOUT, SUM(CASH_PHYSICAL_AT_STORE) AS CASH_PHYSICAL_AT_STORE, 
                         SUM(CASH_DEPOSITED_IN_BANK) AS CASH_DEPOSITED_IN_BANK
                    FROM            STORE_SALE_MASTER
                    WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DATE >= 'PARMFROMDATE') AND (SALE_DATE <= 'PARMTODATE') GROUP BY SALE_DATE";

            _sql = _sql.Replace("PARMSTOREID", storeID.ToString());
            _sql = _sql.Replace("PARMFROMDATE", fromDate.ToString());
            _sql = _sql.Replace("PARMTODATE", toDate.ToString());

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                objMonStmt = new BusinessDailySheet();
                objMonStmt.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objMonStmt.Gas = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"]);
                objMonStmt.CreditCard = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"]);
                objMonStmt.Lottery = Convert.ToSingle(dr["LOTTERY_SALE"]);
                objMonStmt.LotteryPaidout = Convert.ToSingle(dr["LOTTERY_PAIDOUT"]);
                objMonStmt.CashOnHand = Convert.ToSingle(dr["CASH_PHYSICAL_AT_STORE"]);
                objMonStmt.BankDeposit = Convert.ToSingle(dr["CASH_DEPOSITED_IN_BANK"]);

                objRepColl.Add(objMonStmt);
            }
            dr.Close();

            return objRepColl;
        }

        private List<BusinessDailySheet> FillTransactionSheet(int storeID, DateTime fromDate, DateTime toDate)
        {
            List<BusinessDailySheet> objRepColl = new List<BusinessDailySheet>();
            BusinessDailySheet objMonStmt;
            _sql = @"SELECT
                        SUM(CASE STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE WHEN 'BD' THEN STORE_BUSINESS_TRANS.BUSINESS_AMOUNT ELSE 0 END) AS BANK_DEPOSIT, 
                        SUM(CASE STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE WHEN 'CP' THEN STORE_BUSINESS_TRANS.BUSINESS_AMOUNT ELSE 0 END) AS CASH_PAID, 
                        SUM(CASE STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE WHEN 'CR' THEN STORE_BUSINESS_TRANS.BUSINESS_AMOUNT ELSE 0 END) AS CASH_RECEIPT, 
                        SUM(CASE STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE WHEN 'BP' THEN STORE_BUSINESS_TRANS.BUSINESS_AMOUNT ELSE 0 END) AS BUSINESS_PAID, 
                        STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE >= 'PARMFROMDATE') AND 
                                                 (STORE_SALE_MASTER.SALE_DATE <= 'PARMTODATE')
						                         AND STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID NOT IN (10,28)
                        GROUP BY STORE_SALE_MASTER.SALE_DATE";

            _sql = _sql.Replace("PARMSTOREID", storeID.ToString());
            _sql = _sql.Replace("PARMFROMDATE", fromDate.ToString());
            _sql = _sql.Replace("PARMTODATE", toDate.ToString());

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                objMonStmt = new BusinessDailySheet();
                objMonStmt.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objMonStmt.BankDeposit = Convert.ToSingle(dr["BANK_DEPOSIT"]);
                objMonStmt.CashPaid = Convert.ToSingle(dr["CASH_PAID"]);
                objMonStmt.BorrowFromOthers = Convert.ToSingle(dr["CASH_RECEIPT"]);
                objMonStmt.CreditCard = Convert.ToSingle(dr["CASH_RECEIPT"]);

                objRepColl.Add(objMonStmt);
            }
            dr.Close();

            return objRepColl;
        }

        private List<BusinessDailySheet> FillTransactionLedgers(int storeID, DateTime fromDate, DateTime toDate)
        {
            List<BusinessDailySheet> objRepColl = new List<BusinessDailySheet>();
            BusinessDailySheet objMonStmt;
            _sql = @"SELECT
                        SUM(CASE STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID WHEN 10 THEN STORE_BUSINESS_TRANS.BUSINESS_AMOUNT ELSE 0 END) AS BUSINESS_SAVINGS, 
                        SUM(CASE STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID WHEN 28 THEN STORE_BUSINESS_TRANS.BUSINESS_AMOUNT ELSE 0 END) AS UPAD, 
                        STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE >= 'PARMFROMDATE') AND 
                                                 (STORE_SALE_MASTER.SALE_DATE <= 'PARMTODATE')
						                         AND STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID NOT IN (10,28)
                        GROUP BY STORE_SALE_MASTER.SALE_DATE";

            _sql = _sql.Replace("PARMSTOREID", storeID.ToString());
            _sql = _sql.Replace("PARMFROMDATE", fromDate.ToString());
            _sql = _sql.Replace("PARMTODATE", toDate.ToString());

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                objMonStmt = new BusinessDailySheet();
                objMonStmt.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objMonStmt.UPAD = Convert.ToSingle(dr["UPAD"]);
                objMonStmt.BusinessSavings = Convert.ToSingle(dr["BUSINESS_SAVINGS"]);

                objRepColl.Add(objMonStmt);
            }
            dr.Close();

            return objRepColl;
        }

    }

    

}
