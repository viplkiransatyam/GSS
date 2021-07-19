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
    public class ReportMonthlyTotal
    {
        SqlConnection _conn;
        string _sql;
        private int StoreID;
        private string MonthName;
        private int Year;
        List<ReportTranModel> objTransModel = new List<ReportTranModel>();
        List<ReportLotterySection> objLotteryTrans = new List<ReportLotterySection>();
        string sFromDate;
        string sEndDate;
        DateTime dFromDate;
        DateTime dEndDate;

        public RepMonth GetMonthYear(string iStoreID)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                RepMonth _objRepMonth = new RepMonth();
                List<string> MonthName = new List<string>();
                List<int> Year = new List<int>();

                MonthName.Add("Jan");
                MonthName.Add("Feb");
                MonthName.Add("Mar");
                MonthName.Add("Apr");
                MonthName.Add("Jun");
                MonthName.Add("Jul");
                MonthName.Add("Aug");
                MonthName.Add("Sep");
                MonthName.Add("Oct");
                MonthName.Add("Nov");
                MonthName.Add("Dec");
                _objRepMonth.MonthName = MonthName;

                _sql = @"SELECT DISTINCT YEAR(SALE_DATE) AS YEAR
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID IN (" + iStoreID + "))";

                SqlCommand cmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Year.Add(Convert.ToInt16(dr["YEAR"]));
                }
                dr.Close();
                _objRepMonth.YearNo = Year;
                return _objRepMonth;
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

        public ReportMonthlyTotalNormal GetMonthlyStatementNormal(int iStoreID, string sMonth, int iYear)
        {
            ReportMonthlyTotalNormal objMonStmt = new ReportMonthlyTotalNormal();
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


                objMonStmt.Business = BusinessTable();
                objMonStmt.Gas = GasTable();
                objMonStmt.MoneyOrder = MoneyOrderTable();
                objMonStmt.Lottery = LotteryTable();

                return objMonStmt;
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

        private DataTable BusinessTable()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;


            _sql = @"SELECT   ACTLED_ID, ACTLED_NAME, ACTLED_DISP_SIDE
                        FROM            ACCOUNTS_ACTLED_STORE
                        WHERE        (STORE_ID = MAPSTOREID) AND (ACTLED_DISP_SIDE IN (N'BS', N'BP')) AND (ACTLED_ACTIVE = 'A') AND (NOT (ACTLED_ID IN (4, 5))) AND (ACTLED_ID > 40 OR
                                                 ACTLED_ID < 31)
                        ORDER BY ACTLED_DISP_SIDE DESC";
            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());
            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            while (dr.Read())
            {
                dtblTemp.Columns.Add(dr["ACTLED_DISP_SIDE"].ToString() + "_" + dr["ACTLED_ID"].ToString() + "_" + dr["ACTLED_NAME"].ToString());
            }
            dtblTemp.Columns.Add("CP_0_CASH PAID");
            dtblTemp.Columns.Add("QP_0_CHEQUE PAID");
            dtblTemp.Columns.Add("BD_9_DEPOSIT"); // TO BE DEPOSITED AGAINST BUSINESS (BUSINESS SALE - BUSINESS PAID)
            dtblTemp.Columns.Add("BD_9_DEPOSIT-IN-BANK"); // BUSINESS DEPOSIT
            dr.Close();

            FillTransaction();

            while (dFromDate <= dEndDate)
            {
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    if (dtblTemp.Columns[i].ColumnName == "BD_9_DEPOSIT_IN_BANK")
                        drRow[dtblTemp.Columns[i].ColumnName] = GetToBeDeposited(dFromDate,"TBD");
                    else
                        drRow[dtblTemp.Columns[i].ColumnName] = GetTranAmount(Convert.ToInt16(sTemp[1]), dFromDate, sTemp[0]);
                }
                
                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] sTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++ )
            {
                for (int j=1; j < dtblTemp.Columns.Count; j++)
                {
                    sTotal[j] = sTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count -1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = sTotal[k].ToString();
                    }
                    dtblTemp.Rows.Add(drRow);
                    break;
                }
            }
                
            for (int i = 1; i < dtblTemp.Columns.Count; i++)
            {
                string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                if (sTemp.Length == 3)
                {
                    dtblTemp.Columns[i].ColumnName = sTemp[2].Trim();
                }
            }
            return dtblTemp;
        }

        private void FillTransaction()
        {
            ReportTranModel objTranModel;

            #region Business Sales, Receipt, Business / Gas / Lottery Deposit
            _sql = @"SELECT  STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE, STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID, 
                                                 STORE_BUSINESS_TRANS.BUSINESS_AMOUNT, STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = " + StoreID + ") AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND ";
            _sql += "(convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = Convert.ToInt16(dr["BUSINESS_ACTLED_ID"].ToString());
                objTranModel.TranType = dr["BUSINESS_TRAN_TYPE"].ToString();
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());

                objTransModel.Add(objTranModel);
            }
            dr.Close();
            #endregion

            #region Cash Paid and Cheque Paid
            _sql = @"SELECT  STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE, SUM(STORE_BUSINESS_TRANS.BUSINESS_AMOUNT) AS BUSINESS_AMOUNT, 
                                                 STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = MAPSTOREID) AND (STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE IN ('CP', 'QP'))
                        GROUP BY STORE_SALE_MASTER.SALE_DATE, STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE
                        HAVING        (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";
            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = 0;
                objTranModel.TranType = dr["BUSINESS_TRAN_TYPE"].ToString();
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());

                objTransModel.Add(objTranModel);
            }
            dr.Close();

            #endregion

            #region Total Business sale (TBS) - total business paid (recorded with TBP (BP + CP) )
            _sql = @"SELECT      'TBS' AS TRAN_TYPE,  ACTCASH_DATE, SUM(ACTCASH_AMOUNT) AS AMOUNT
                        FROM            ACTCASH_TRAN
                        WHERE        (ACTCASH_STORE_ID = MAPSTOREID) AND (ACTCASH_INWARD_OUTWARD_TYPE = 'I') AND (ACTCASH_TRAN_TYPE = 'BS') AND (ACTCAST_SUB_TRAN_TYPE <> 'MO')
                        GROUP BY ACTCASH_DATE
                        HAVING        (ACTCASH_DATE >= 'FROMDATE') AND (ACTCASH_DATE <='TODATE')
                        UNION ALL 
                        SELECT     'TBP' AS TRAN_TYPE, ACTCASH_DATE, SUM(ACTCASH_AMOUNT) AS AMOUNT
                        FROM            ACTCASH_TRAN
                        WHERE        (ACTCASH_STORE_ID = MAPSTOREID) AND (ACTCASH_INWARD_OUTWARD_TYPE = 'O') AND (ACTCASH_TRAN_TYPE IN ('CP','BP')) AND (ACTCAST_SUB_TRAN_TYPE <> 'MO')
                        GROUP BY ACTCASH_DATE
                        HAVING        (ACTCASH_DATE >= 'FROMDATE') AND (ACTCASH_DATE <= 'TODATE')";


            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());
            _sql = _sql.Replace("FROMDATE", sFromDate);
            _sql = _sql.Replace("TODATE", sEndDate);

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = 0;
                objTranModel.TranType = dr["TRAN_TYPE"].ToString();
                objTranModel.Date = Convert.ToDateTime(dr["ACTCASH_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["AMOUNT"].ToString());
                
                objTransModel.Add(objTranModel);
            }
            dr.Close();


            #endregion
        }

        private float GetToBeDeposited(DateTime Date, string sTranType)
        {
            float fAmount = 0;
            ReportTranModel objTranModel = new ReportTranModel();

            #region TBD - TOTAL BUSINESS DEPOSIT

            if (sTranType == "TBD")
                objTranModel = objTransModel.Find(x => x.Date == Date && x.TranType == "TBS");

            if (objTranModel != null)
            {
                if (objTranModel.Amount > 0)
                    fAmount = objTranModel.Amount;
            }

            if (sTranType == "TBD")
                objTranModel = objTransModel.Find(x => x.Date == Date && x.TranType == "TBP");

            if (objTranModel != null)
            {
                if (objTranModel.Amount > 0)
                    fAmount = fAmount - objTranModel.Amount;
            }
            #endregion

            return fAmount;
        }

        private float GetTranAmount(int LedgerID, DateTime Date, string sTranType)
        {
            float fAmount = 0;
            ReportTranModel objTranModel = new ReportTranModel();

            if ((sTranType == "BS") || (sTranType == "BP") || (sTranType == "BD") || (sTranType == "LD") || (sTranType == "GD"))
                objTranModel = objTransModel.Find(x => x.LedgerID == LedgerID && x.Date == Date && x.TranType == sTranType);
            else if ((sTranType == "CP") || (sTranType == "QP"))
                objTranModel = objTransModel.Find(x => x.Date == Date && x.TranType == sTranType);

            if (objTranModel != null)
            {
                if (objTranModel.Amount > 0)
                    fAmount = objTranModel.Amount;
            }

            return fAmount;
        }

        private DataTable GasTable()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;
            dFromDate = Convert.ToDateTime(sFromDate);

            objTransModel.Clear();

            _sql = @"SELECT    MAPPING_STORE_CARD.STORE_ID, MAPPING_STORE_CARD.CARD_TYPE_ID, GROUP_CARD_TYPE.GROU_CARD_NAME
                    FROM            MAPPING_STORE_CARD INNER JOIN
                                             GROUP_CARD_TYPE ON MAPPING_STORE_CARD.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                    WHERE        (MAPPING_STORE_CARD.ACTIVE = 'A') AND (MAPPING_STORE_CARD.STORE_ID = MAPSTOREID)";

            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());
            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            dtblTemp.Columns.Add("GAS_SALE");
            while (dr.Read())
            {
                dtblTemp.Columns.Add("CD_" + dr["CARD_TYPE_ID"].ToString() + "_" + dr["GROU_CARD_NAME"].ToString());
            }
            dtblTemp.Columns.Add("GD_4_DEPOSIT"); // TO BE DEPOSITED AGAINST BUSINESS (BUSINESS SALE - BUSINESS PAID)
            dtblTemp.Columns.Add("GD_5_DEPOSIT-IN-BANK"); // BUSINESS DEPOSIT
            dr.Close();

            FillGasCardTrans();

            while (dFromDate <= dEndDate)
            {
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    if (dtblTemp.Columns[i].ColumnName == "GAS_SALE")
                        drRow[dtblTemp.Columns[i].ColumnName] = GetGasTranAmount(0, dFromDate, "GSS");
                    else if (dtblTemp.Columns[i].ColumnName == "GD_4_DEPOSIT")
                        drRow[dtblTemp.Columns[i].ColumnName] = GetGasTranAmount(0,dFromDate, "TTD");
                    else if (dtblTemp.Columns[i].ColumnName == "GD_5_DEPOSIT-IN-BANK")
                        drRow[dtblTemp.Columns[i].ColumnName] = GetGasTranAmount(0, dFromDate, "TGD");
                    else
                        drRow[dtblTemp.Columns[i].ColumnName] = GetGasTranAmount(Convert.ToInt16(sTemp[1]), dFromDate, "CD");
                }

                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }


            float[] iTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++)
            {
                for (int j = 1; j < dtblTemp.Columns.Count; j++)
                {
                    iTotal[j] = iTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count - 1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = iTotal[k].ToString();
                    }
                    dtblTemp.Rows.Add(drRow);
                    break;
                }
            }

            for (int i = 1; i < dtblTemp.Columns.Count; i++)
            {
                string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                if (sTemp.Length == 3)
                {
                    dtblTemp.Columns[i].ColumnName = sTemp[2].Trim();
                }
            }
            return dtblTemp;
        }

        private void FillGasCardTrans()
        {
            ReportTranModel objTranModel;

            #region Gas Card Receipt
            _sql = @"SELECT      STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID, STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT, STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = MAPSTOREID) AND 
                        (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '"+ sFromDate + "') AND  (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";
            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = Convert.ToInt16(dr["CARD_TYPE_ID"].ToString());
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.TranType = "GCD"; // Gas Card Amount
                objTranModel.Amount = Convert.ToSingle(dr["CARD_AMOUNT"].ToString());

                objTransModel.Add(objTranModel);
            }
            dr.Close();
            #endregion

            #region Gas Sale and Gas amount to be deposited
            _sql = @"SELECT   STORE_ID, SALE_DATE, SALE_GAS_TOTAL_SALE, SALE_GAS_CARD_TOTAL
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = MAPSTOREID) AND 
                        (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";
            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());


            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = 0;
                objTranModel.TranType = "GSS";  // GAS SALE
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"].ToString());
                objTransModel.Add(objTranModel);

                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = 0;
                objTranModel.TranType = "TTD";  // TOTAL GAS DEPOSIT
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"].ToString()) - Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"].ToString());
                objTransModel.Add(objTranModel);
            }
            dr.Close();

            #endregion

            #region GAS DEPOSITS
            _sql = @"SELECT      'TGD' AS TRAN_TYPE,  ACTCASH_DATE, SUM(ACTCASH_AMOUNT) AS AMOUNT
                        FROM            ACTCASH_TRAN
                        WHERE        (ACTCASH_STORE_ID = MAPSTOREID) AND (ACTCASH_INWARD_OUTWARD_TYPE = 'O') AND (ACTCASH_TRAN_TYPE = 'GD')
                        GROUP BY ACTCASH_DATE
                        HAVING        (ACTCASH_DATE >= 'FROMDATE') AND (ACTCASH_DATE <='TODATE')";

            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());
            _sql = _sql.Replace("FROMDATE", sFromDate);
            _sql = _sql.Replace("TODATE", sEndDate);

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = 0;
                objTranModel.TranType = dr["TRAN_TYPE"].ToString();
                objTranModel.Date = Convert.ToDateTime(dr["ACTCASH_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["AMOUNT"].ToString());

                objTransModel.Add(objTranModel);
            }
            dr.Close();





            #endregion
        }

        private float GetGasTranAmount(int CardID, DateTime Date, string sTranType)
        {
            float fAmount = 0;
            ReportTranModel objTranModel = new ReportTranModel();

            if (sTranType == "CD")
                objTranModel = objTransModel.Find(x => x.LedgerID == CardID && x.Date == Date);
            else 
                objTranModel = objTransModel.Find(x => x.Date == Date && x.TranType == sTranType);

            if (objTranModel != null)
            {
                if (objTranModel.Amount > 0)
                    fAmount = objTranModel.Amount;
            }

            return fAmount;
        }

        private DataTable MoneyOrderTable()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;
            dFromDate = Convert.ToDateTime(sFromDate);

            _sql = @"SELECT   ACTLED_ID, ACTLED_NAME, ACTLED_DISP_SIDE
                        FROM            ACCOUNTS_ACTLED_STORE
                        WHERE        (STORE_ID = MAPSTOREID) AND (ACTLED_DISP_SIDE IN (N'BS', N'BP')) AND (ACTLED_ACTIVE = 'A') AND (ACTLED_ID IN (34, 35, 36,37))
                        ORDER BY ACTLED_DISP_SIDE DESC";
            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());
            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            while (dr.Read())
            {
                dtblTemp.Columns.Add(dr["ACTLED_DISP_SIDE"].ToString() + "_" + dr["ACTLED_ID"].ToString() + "_" + dr["ACTLED_NAME"].ToString());
            }
            dtblTemp.Columns.Add("BP_37_DEPOSIT"); // TO BE DEPOSITED AGAINST MO (MO SALE - MO PAID)
            dtblTemp.Columns.Add("MD_34_DEPOSIT-IN-BANK"); // BUSINESS DEPOSIT
            dr.Close();

            FillMOTransaction();
            float fDeposit = 0;

            while (dFromDate <= dEndDate)
            {
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    if (dtblTemp.Columns[i].ColumnName == "BP_37_DEPOSIT")
                    {
                        fDeposit -= GetTranAmount(Convert.ToInt16(sTemp[1]), dFromDate, sTemp[0]);
                        drRow[dtblTemp.Columns[i].ColumnName] = fDeposit.ToString();
                    }
                    else if (dtblTemp.Columns[i].ColumnName == "MD_34_DEPOSIT-IN-BANK")
                        drRow[dtblTemp.Columns[i].ColumnName] = GetTranAmount(Convert.ToInt16(sTemp[1]), dFromDate, sTemp[0]);
                    else
                    {
                        drRow[dtblTemp.Columns[i].ColumnName] = GetTranAmount(Convert.ToInt16(sTemp[1]), dFromDate, sTemp[0]);
                        fDeposit += Convert.ToSingle(drRow[dtblTemp.Columns[i].ColumnName]);
                    }
                }
                
                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] iTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++ )
            {
                for (int j=1; j < dtblTemp.Columns.Count; j++)
                {
                    iTotal[j] = iTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count -1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = iTotal[k].ToString();
                    }
                    dtblTemp.Rows.Add(drRow);
                    break;
                }
            }
                
            for (int i = 1; i < dtblTemp.Columns.Count; i++)
            {
                string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                if (sTemp.Length == 3)
                {
                    dtblTemp.Columns[i].ColumnName = sTemp[2].Trim();
                }
            }
            return dtblTemp;
        }
        
        private void FillMOTransaction()
        {
            ReportTranModel objTranModel;
            objTransModel.Clear();

            #region Money Order Sales, Paid out, Deposit
            _sql = @"SELECT  STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE, STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID, 
                                                 STORE_BUSINESS_TRANS.BUSINESS_AMOUNT, STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_BUSINESS_TRANS INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID IN (34,35,36,37)) AND (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = " + StoreID + ") AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND ";
            _sql += "(convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "') ";

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportTranModel();
                objTranModel.LedgerID = Convert.ToInt16(dr["BUSINESS_ACTLED_ID"].ToString());
                objTranModel.TranType = dr["BUSINESS_TRAN_TYPE"].ToString();
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.Amount = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());

                objTransModel.Add(objTranModel);
            }
            dr.Close();
            #endregion

        }

        private DataTable LotteryTable()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            dFromDate = Convert.ToDateTime(sFromDate);

            dtblTemp.Columns.Add("Date");
            dtblTemp.Columns.Add("OnlineSale");
            dtblTemp.Columns.Add("InstantSale");
            dtblTemp.Columns.Add("OnlinePaid"); 
            dtblTemp.Columns.Add("InstantPaid"); 
            dtblTemp.Columns.Add("Deposit"); 
            dtblTemp.Columns.Add("DepositInBank");

            FillLotteryTransaction();
            ReportLotterySection objTranModel ;

            while (dFromDate <= dEndDate)
            {
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                objTranModel = GetLotteryRecord(dFromDate);

                drRow["OnlineSale"] = 0;
                drRow["InstantSale"] = 0;
                drRow["OnlinePaid"] = 0;
                drRow["InstantPaid"] = 0;
                drRow["Deposit"] = 0;
                drRow["DepositInBank"] = 0;
                
                if (objTranModel != null)
                {
                    drRow["OnlineSale"] = objTranModel.OnlineSale;
                    drRow["InstantSale"] = objTranModel.InstantSale;
                    drRow["OnlinePaid"] = objTranModel.OnlinePaid;
                    drRow["InstantPaid"] = objTranModel.InstantPaid;
                    drRow["Deposit"] = objTranModel.Deposit;
                    drRow["DepositInBank"] = objTranModel.DepositInBank;
                }
        
                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] iTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++)
            {
                for (int j = 1; j < dtblTemp.Columns.Count; j++)
                {
                    iTotal[j] = iTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count - 1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = iTotal[k].ToString();
                    }
                    dtblTemp.Rows.Add(drRow);
                    break;
                }
            }

            return dtblTemp;
        }

        private void FillLotteryTransaction()
        {
            ReportLotterySection objTranModel;
            objTransModel.Clear();

            #region Lottery Transactions
            _sql = @"SELECT   STORE_SALE_MASTER.STORE_ID, STORE_SALE_MASTER.SALE_DATE, STORE_SALE_MASTER.SALE_LOTTERY_SALE AS INSTANT_SALE, 
                                                 STORE_SALE_MASTER.SALE_LOTTERY_ONLINE AS ONLINE_SALE, STORE_SALE_MASTER.SALE_LOTTERY_CASH_INSTANT_PAID AS INSTANT_PAID, 
                                                 STORE_SALE_MASTER.SALE_LOTTERY_CASH_ONLINE_PAID AS ONLINE_PAID, 
                                                 STORE_SALE_MASTER.SALE_LOTTERY_SALE + STORE_SALE_MASTER.SALE_LOTTERY_ONLINE - STORE_SALE_MASTER.SALE_LOTTERY_CASH_INSTANT_PAID - STORE_SALE_MASTER.SALE_LOTTERY_CASH_ONLINE_PAID
                                                  AS DEPOSIT, ACTCASH_TRAN.ACTCASH_TRAN_TYPE, ACTCASH_TRAN.ACTCASH_AMOUNT
                        FROM            STORE_SALE_MASTER LEFT OUTER JOIN
                                                 ACTCASH_TRAN ON STORE_SALE_MASTER.STORE_ID = ACTCASH_TRAN.ACTCASH_STORE_ID AND 
                                                 STORE_SALE_MASTER.SALE_DAY_TRAN_ID = ACTCASH_TRAN.ACTCASH_SALE_PMT_DAY_TRAN_ID
						                         AND (ACTCASH_TRAN.ACTCASH_TRAN_TYPE = 'LD')
                        WHERE        (STORE_SALE_MASTER.STORE_ID = MAPSTOREID) AND 
                        (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";

            _sql = _sql.Replace("MAPSTOREID", StoreID.ToString());
            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new ReportLotterySection();

                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objTranModel.InstantPaid = Convert.ToSingle(dr["INSTANT_PAID"].ToString());
                objTranModel.InstantSale = Convert.ToSingle(dr["INSTANT_SALE"].ToString());
                objTranModel.OnlinePaid = Convert.ToSingle(dr["ONLINE_PAID"].ToString());
                objTranModel.OnlineSale = Convert.ToSingle(dr["ONLINE_SALE"].ToString());
                objTranModel.Deposit = Convert.ToSingle(dr["DEPOSIT"].ToString());

                if (dr["ACTCASH_AMOUNT"].ToString().Length > 0)
                    objTranModel.DepositInBank = Convert.ToSingle(dr["ACTCASH_AMOUNT"].ToString());

                objLotteryTrans.Add(objTranModel);
            }
            dr.Close();
            #endregion

        }

        private ReportLotterySection GetLotteryRecord(DateTime Date)
        {
            ReportLotterySection objTranModel = new ReportLotterySection();

             objTranModel = objLotteryTrans.Find(x => x.Date == Date);

             return objTranModel;
        }

    }

}
