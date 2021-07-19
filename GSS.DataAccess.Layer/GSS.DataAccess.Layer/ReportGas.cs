using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class ReportGas
    {
        SqlConnection _conn;
        string _sql;

        public List<ReportGasItem> GetSaleReportIndividual(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReportGasItem> objReport = new List<ReportGasItem>();
            ReportGasItem obj;
            SqlCommand cmd;
            SqlDataReader dr;
            float fTotal = 0;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Adding Item wise sale report
                _sql = @"select * from 
                        (
                        SELECT        STORE_GAS_SALE_TRAN.SALE_DATE, STORE_GAS_SALE_TRAN.SALE_AMOUNT, GROUP_GASTYPE_MASTER.GASTYPE_NAME, 
                                                    STORE_SALE_MASTER.SALE_SHIFT_CODE
                        FROM            STORE_GAS_SALE_TRAN INNER JOIN
                                                    GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_TRAN.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                                    STORE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                    GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID INNER JOIN
                                                    STORE_SALE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                    STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID AND 
                                                    STORE_MASTER.STORE_ID = STORE_SALE_MASTER.STORE_ID
                        WHERE        (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND (STORE_GAS_SALE_TRAN.SALE_DATE >= 'PARMFROMDATE') AND 
                                                    (STORE_GAS_SALE_TRAN.SALE_DATE <= 'PARMTODATE')
                        ) tab
                        PIVOT(sum(sale_amount) FOR GASTYPE_NAME IN (UNLEAD,PREMIUM,MID_GRADE,DIESEL,KIROSENE,NON_ETHNOL)
                        )  pivotsales 
                        order by sale_date,SALE_SHIFT_CODE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());


                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    fTotal = 0;
                    obj = new ReportGasItem();
                    obj.Date            = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode       = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    if (dr["UNLEAD"].ToString().Length > 0)
                    {
                        obj.Unlead = Convert.ToSingle(dr["UNLEAD"].ToString());
                        fTotal = obj.Unlead;
                    }
                    if (dr["MID_GRADE"].ToString().Length > 0)
                    {
                        obj.MidGrade = Convert.ToSingle(dr["MID_GRADE"].ToString());
                        fTotal += obj.MidGrade;
                    }
                    if (dr["PREMIUM"].ToString().Length > 0)
                    {
                        obj.Premium = Convert.ToSingle(dr["PREMIUM"].ToString());
                        fTotal += obj.Premium;
                    }
                    if (dr["DIESEL"].ToString().Length > 0)
                    {
                        obj.Diesel = Convert.ToSingle(dr["DIESEL"].ToString());
                        fTotal += obj.Diesel;
                    }
                    if (dr["KIROSENE"].ToString().Length > 0)
                    {
                        obj.Kirosene = Convert.ToSingle(dr["KIROSENE"].ToString());
                        fTotal += obj.Kirosene;
                    }
                    if (dr["NON_ETHNOL"].ToString().Length > 0)
                    {
                        obj.NonEthnol = Convert.ToSingle(dr["NON_ETHNOL"].ToString());
                        fTotal += obj.NonEthnol;
                    }
                    obj.Total = fTotal;
                    
                    if (fTotal > 0) // Not adding if no sales are found
                        objReport.Add(obj);
                }
                dr.Close();

                #endregion
                return objReport;
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

        public List<ReportSaleTrendModel> GetSaleTrend(int StoreID, int MonthNo, int YearNo)
        {
            ReportSaleTrendModel obj;
            List<ReportSaleTrendModel> objColl = new List<ReportSaleTrendModel>();
            SqlCommand cmd;
            SqlDataReader dr;
            
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Adding Item wise sale report
                _sql = @"select sum(unlead) as unlead, sum(premium) as premium,sum(mid_grade) as mid_grade, 
                        sum(diesel) as diesel, sum(kirosene) as kirosene, sum(non_ethnol) as non_ethnol from 
                        (
                        SELECT        STORE_GAS_SALE_TRAN.SALE_DATE, STORE_GAS_SALE_TRAN.SALE_AMOUNT, GROUP_GASTYPE_MASTER.GASTYPE_NAME, 
                                                    STORE_SALE_MASTER.SALE_SHIFT_CODE
                        FROM            STORE_GAS_SALE_TRAN INNER JOIN
                                                    GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_TRAN.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                                    STORE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                    GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID INNER JOIN
                                                    STORE_SALE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                    STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID AND 
                                                    STORE_MASTER.STORE_ID = STORE_SALE_MASTER.STORE_ID
                        WHERE        (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND (MONTH(STORE_GAS_SALE_TRAN.SALE_DATE) = 'PARMMONTH') AND 
                                                    (YEAR(STORE_GAS_SALE_TRAN.SALE_DATE) <= 'PARMYEAR')
                        ) tab
                        PIVOT(sum(sale_amount) FOR GASTYPE_NAME IN (UNLEAD,PREMIUM,MID_GRADE,DIESEL,KIROSENE,NON_ETHNOL)
                        )  pivotsales ";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMMONTH", MonthNo.ToString());
                _sql = _sql.Replace("PARMYEAR", YearNo.ToString());


                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["UNLEAD"].ToString().Length > 0)
                    {
                        obj = new ReportSaleTrendModel();
                        obj.GasOilType = "Unlead";
                        obj.Sale = Convert.ToSingle(dr["UNLEAD"].ToString());
                        objColl.Add(obj);
                    }
                    if (dr["MID_GRADE"].ToString().Length > 0)
                    {
                        obj = new ReportSaleTrendModel();
                        obj.GasOilType = "MidGrade";
                        obj.Sale = Convert.ToSingle(dr["MID_GRADE"].ToString());
                        objColl.Add(obj);
                    }
                    if (dr["PREMIUM"].ToString().Length > 0)
                    {
                        obj = new ReportSaleTrendModel();
                        obj.GasOilType = "Premium";
                        obj.Sale = Convert.ToSingle(dr["PREMIUM"].ToString());
                        objColl.Add(obj);
                    }
                    if (dr["DIESEL"].ToString().Length > 0)
                    {
                        obj = new ReportSaleTrendModel();
                        obj.GasOilType = "Diesel";
                        obj.Sale = Convert.ToSingle(dr["DIESEL"].ToString());
                        objColl.Add(obj);
                    }
                    if (dr["KIROSENE"].ToString().Length > 0)
                    {
                        obj = new ReportSaleTrendModel();
                        obj.GasOilType = "Kirosene";
                        obj.Sale = Convert.ToSingle(dr["KIROSENE"].ToString());
                        objColl.Add(obj);
                    }
                    if (dr["NON_ETHNOL"].ToString().Length > 0)
                    {
                        obj = new ReportSaleTrendModel();
                        obj.GasOilType = "Non-Ethonol";
                        obj.Sale = Convert.ToSingle(dr["NON_ETHNOL"].ToString());
                        objColl.Add(obj);
                    }
                }
                dr.Close();

                #endregion
                return objColl;
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

        public ReportGasItem GasReportPreviousShiftSalesDashboard(int StoreID)
        {
            ReportGasItem obj = new ReportGasItem();
            SqlCommand cmd;
            SqlDataReader dr;
            DateTime PreviousDay = DateTime.Now.Date;
            int PreviousShiftCode = 0;
            float fTotal = 0;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Adding previous shift sale report

                _sql = @"SELECT        MAX(SALE_SHIFT_CODE) AS SHIFT_CODE, MAX(SALE_DATE) AS SALE_DATE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = PARMSTOREID) AND (SALE_GAS_TOTAL_SALE > 0)";
                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    PreviousDay = Convert.ToDateTime(dr["SALE_DATE"]);
                    PreviousShiftCode = Convert.ToInt16(dr["SHIFT_CODE"]);
                }
                dr.Close();

                _sql = @"select * from 
                        (
                        SELECT        STORE_GAS_SALE_TRAN.SALE_DATE, STORE_GAS_SALE_TRAN.SALE_AMOUNT, GROUP_GASTYPE_MASTER.GASTYPE_NAME, 
                                                    STORE_SALE_MASTER.SALE_SHIFT_CODE
                        FROM            STORE_GAS_SALE_TRAN INNER JOIN
                                                    GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_TRAN.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                                    STORE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                    GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID INNER JOIN
                                                    STORE_SALE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                    STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID AND 
                                                    STORE_MASTER.STORE_ID = STORE_SALE_MASTER.STORE_ID
                        WHERE        (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND (STORE_GAS_SALE_TRAN.SALE_DATE = 'PARMFROMDATE') AND 
                                                    (STORE_SALE_MASTER.SALE_SHIFT_CODE = 'PARMSHIFTCODE')
                        ) tab
                        PIVOT(sum(sale_amount) FOR GASTYPE_NAME IN (UNLEAD,PREMIUM,MID_GRADE,DIESEL,KIROSENE,NON_ETHNOL)
                        )  pivotsales 
                        order by sale_date,SALE_SHIFT_CODE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", PreviousDay.ToString());
                _sql = _sql.Replace("PARMSHIFTCODE", PreviousShiftCode.ToString());


                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    fTotal = 0;
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    if (dr["UNLEAD"].ToString().Length > 0)
                    {
                        obj.Unlead = Convert.ToSingle(dr["UNLEAD"].ToString());
                        fTotal = obj.Unlead;
                    }
                    if (dr["MID_GRADE"].ToString().Length > 0)
                    {
                        obj.MidGrade = Convert.ToSingle(dr["MID_GRADE"].ToString());
                        fTotal += obj.MidGrade;
                    }
                    if (dr["PREMIUM"].ToString().Length > 0)
                    {
                        obj.Premium = Convert.ToSingle(dr["PREMIUM"].ToString());
                        fTotal += obj.Premium;
                    }
                    if (dr["DIESEL"].ToString().Length > 0)
                    {
                        obj.Diesel = Convert.ToSingle(dr["DIESEL"].ToString());
                        fTotal += obj.Diesel;
                    }
                    if (dr["KIROSENE"].ToString().Length > 0)
                    {
                        obj.Kirosene = Convert.ToSingle(dr["KIROSENE"].ToString());
                        fTotal += obj.Kirosene;
                    }
                    if (dr["NON_ETHNOL"].ToString().Length > 0)
                    {
                        obj.NonEthnol = Convert.ToSingle(dr["NON_ETHNOL"].ToString());
                        fTotal += obj.NonEthnol;
                    }
                    obj.Total = fTotal;

                }
                dr.Close();

                #endregion
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

        public List<ReportInventory> GetInventoryReportForPreviousShift(int StoreID)
        {
            List<ReportInventory> objReport = new List<ReportInventory>();
            ReportInventory obj;
            SqlCommand cmd;
            SqlDataReader dr;
            DateTime PreviousDay = DateTime.Now.Date;
            int PreviousShiftCode = 0;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Query to get previous shift inventory

                _sql = @"SELECT        MAX(SALE_SHIFT_CODE) AS SHIFT_CODE, MAX(SALE_DATE) AS SALE_DATE
                            FROM            STORE_SALE_MASTER
                            WHERE        (STORE_ID = PARMSTOREID) AND (SALE_GAS_TOTAL_SALE > 0)";
                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    PreviousDay = Convert.ToDateTime(dr["SALE_DATE"]);
                    PreviousShiftCode = Convert.ToInt16(dr["SHIFT_CODE"]);
                }
                dr.Close();

                _sql = @"select * from 
                            (
                            SELECT   STORE_SALE_MASTER.SALE_SHIFT_CODE, STORE_SALE_MASTER.SALE_DATE, 'Opening Balance' as Description,
                                                     STORE_GAS_SALE_BALANCES.GAS_OP_BAL,
                                                     GROUP_GASTYPE_MASTER.GASTYPE_NAME
                            FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                                     STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_GASTYPE_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID AND 
                                                     STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                            WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = PARMSHIFTCODE) AND 
                                                     (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')
                             ) tab
                                                    PIVOT(max(GAS_OP_BAL) FOR GASTYPE_NAME IN (UNLEAD,PREMIUM,MID_GRADE,DIESEL,KIROSENE,NON_ETHNOL)
                                                    )  pivotsales 

                            UNION ALL 

                            select * from 
                            (
                            SELECT   STORE_SALE_MASTER.SALE_SHIFT_CODE, STORE_SALE_MASTER.SALE_DATE,'Received' as Description,
                                                     STORE_GAS_SALE_BALANCES.GAS_RECV,
                                                     GROUP_GASTYPE_MASTER.GASTYPE_NAME
                            FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                                     STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_GASTYPE_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID AND 
                                                     STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                            WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = PARMSHIFTCODE) AND 
                                                     (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')
                             ) tab
                                                    PIVOT(max(GAS_RECV) FOR GASTYPE_NAME IN (UNLEAD,PREMIUM,MID_GRADE,DIESEL,KIROSENE,NON_ETHNOL)
                                                    )  pivotsales 

                            UNION ALL
                            select * from 
                            (
                            SELECT   STORE_SALE_MASTER.SALE_SHIFT_CODE, STORE_SALE_MASTER.SALE_DATE,'Closing Balance' as Description,
                                                     STORE_GAS_SALE_BALANCES.GAS_ACT_CL_BAL,
                                                     GROUP_GASTYPE_MASTER.GASTYPE_NAME
                            FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                                     STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_GASTYPE_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID AND 
                                                     STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                            WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = PARMSHIFTCODE) AND 
                                                     (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')
                             ) tab
                                                    PIVOT(max(GAS_ACT_CL_BAL) FOR GASTYPE_NAME IN (UNLEAD,PREMIUM,MID_GRADE,DIESEL,KIROSENE,NON_ETHNOL)
                                                    )  pivotsales 


                            ";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMDATE", PreviousDay.ToString());
                _sql = _sql.Replace("PARMSHIFTCODE", PreviousShiftCode.ToString());
                #endregion

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new ReportInventory();
                    obj.SaleDate = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    obj.Description = dr["Description"].ToString();
                    if (dr["UNLEAD"].ToString().Length > 0)
                    {
                        obj.Unlead = Convert.ToSingle(dr["UNLEAD"].ToString());
                    }
                    if (dr["MID_GRADE"].ToString().Length > 0)
                    {
                        obj.MidGrade = Convert.ToSingle(dr["MID_GRADE"].ToString());
                    }
                    if (dr["PREMIUM"].ToString().Length > 0)
                    {
                        obj.Premium = Convert.ToSingle(dr["PREMIUM"].ToString());
                    }
                    if (dr["DIESEL"].ToString().Length > 0)
                    {
                        obj.Diesel = Convert.ToSingle(dr["DIESEL"].ToString());
                    }
                    if (dr["KIROSENE"].ToString().Length > 0)
                    {
                        obj.Kirosene = Convert.ToSingle(dr["KIROSENE"].ToString());
                    }
                    if (dr["NON_ETHNOL"].ToString().Length > 0)
                    {
                        obj.NonEthnol = Convert.ToSingle(dr["NON_ETHNOL"].ToString());
                    }
                    objReport.Add(obj);
                }
                dr.Close();


                return objReport;
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

        public List<SaleGraph> GetSaleReportGraph(int StoreID)
        {
            List<SaleGraph> objReport = new List<SaleGraph>();
            SaleGraph obj;
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Adding sale report for last 6 shifts
                _sql = @"SELECT * FROM
                        (
	                        SELECT  top 6 SALE_SHIFT_CODE, SALE_DATE, SALE_GAS_TOTAL_SALE
	                        FROM            STORE_SALE_MASTER
	                        WHERE        (STORE_ID = PARMSTOREID) AND (SALE_GAS_TOTAL_SALE > 0)
	                        ORDER BY SALE_DATE DESC
                        ) TAB
                            ORDER BY SALE_DATE, SALE_SHIFT_CODE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new SaleGraph();
                    obj.SaleDate = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    obj.SaleAmount = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"].ToString());

                    objReport.Add(obj);
                }
                dr.Close();

                #endregion
                return objReport;
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

        public List<ReconcilationStatement> ReconcilationStatement(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReconcilationStatement> objReport = new List<ReconcilationStatement>();
            ReconcilationStatement obj;
            SqlCommand cmd;
            SqlDataReader dr;
            float fCardAmount = 0;
            float fPaidAmount = 0;
            float fDiscount = 0;
            float fGSSInvoiceAmount = 0;
            float fCreditTransaction = 0;
            float fFee = 0;
            float fVendorInvoiceAmount = 0;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Adding Reconcilation statement between dates
                _sql = @"SELECT TAB1.SALE_DATE, TAB1.CARD_AMOUNT, ISNULL(TAB2.BUSINESS_AMOUNT,0) AS PAID_AMOUNT, ISNULL(TAB3.GR_INV_AMOUNT,0) AS INVOICE_AMOUNT,
	                    ISNULL(TAB6.CreditTransaction,0) as CreditTransaction, ISNULL(tab6.Discount,0) as Discount, ISNULL(tab6.Fee,0) as Fee, 
	                    ISNULL(tab6.InvoiceAmount,0) as InvoiceAmount
                    FROM 
		                    (
		                    SELECT        STORE_SALE_MASTER.SALE_DATE, SUM(STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT) AS CARD_AMOUNT
		                    FROM            STORE_SALE_MASTER INNER JOIN
								                     STORE_GAS_SALE_CARD_BREAKUP ON STORE_SALE_MASTER.STORE_ID = STORE_GAS_SALE_CARD_BREAKUP.STORE_ID AND 
								                     STORE_SALE_MASTER.SALE_DAY_TRAN_ID = STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID
		                    WHERE        (STORE_SALE_MASTER.STORE_ID = PARMSTOREID) AND (NOT (STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID IN (8, 9)))
		                    GROUP BY STORE_SALE_MASTER.SALE_DATE
		                    HAVING        (STORE_SALE_MASTER.SALE_DATE >= 'PARMFROMDATE' AND STORE_SALE_MASTER.SALE_DATE <= 'PARMTODATE')
		                    ) TAB1
                    LEFT OUTER JOIN 
	                    (
	                    SELECT        STORE_SALE_MASTER.SALE_DATE, SUM(STORE_BUSINESS_TRANS.BUSINESS_AMOUNT) AS BUSINESS_AMOUNT
	                    FROM            STORE_BUSINESS_TRANS INNER JOIN
							                     STORE_SALE_MASTER ON STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
							                     STORE_BUSINESS_TRANS.BUSINESS_SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
	                    WHERE        (STORE_BUSINESS_TRANS.BUSINESS_STORE_ID = PARMSTOREID) AND (STORE_BUSINESS_TRANS.BUSINESS_TRAN_TYPE IN ('BP', 'QP', 'CP')) AND (STORE_BUSINESS_TRANS.BUSINESS_ACTLED_ID = 38)
	                    GROUP BY STORE_SALE_MASTER.SALE_DATE
	                    HAVING        (STORE_SALE_MASTER.SALE_DATE >= 'PARMFROMDATE' AND STORE_SALE_MASTER.SALE_DATE <= 'PARMTODATE')
	                    ) TAB2
                    ON TAB1.SALE_DATE = TAB2.SALE_DATE

                    LEFT OUTER JOIN 
	                    (
	                    SELECT        GR_DATE, GR_INV_AMOUNT
	                    FROM            GAS_RECEIPT_MASTER
	                    WHERE        (GR_STORE_ID = PARMSTOREID) AND (GR_INV_AMOUNT > 0) AND (GR_INV_DATE >= 'PARMFROMDATE' AND 
							                     GR_INV_DATE <= 'PARMTODATE')
	                    ) TAB3
                    ON TAB1.SALE_DATE = TAB3.GR_DATE
                    LEFT OUTER JOIN
	                    (
	                    select tab5.GDTT_REF_TRAN_DATE, isnull(Sum(tab5.CreditTransaction),0) as CreditTransaction, isnull(sum(Tab5.Discount),0) as Discount, 
	                    isnull(Sum(Tab5.Invoice),0) As InvoiceAmount, isnull(Sum(Tab5.Fee),0) as Fee
	                    from 
		                    (
			                    select * from 
				                    (
				                    SELECT GAS_DEALER_TRAN_TRANS.GDTT_TRAN_TYPE, GAS_DEALER_TRAN_TRANS.GDTT_REF_TRAN_DATE, 
										                    sum(GAS_DEALER_TRAN_TRANS.GDTT_TRAN_AMOUNT) as GDTT_TRAN_AMOUNT , GLOBAL_GAS_TRAN_TYPES.GTP_TRAN_DESC
				                    FROM            GAS_DEALER_TRAN_MASTER INNER JOIN
										                     GAS_DEALER_TRAN_TRANS ON GAS_DEALER_TRAN_MASTER.GDT_STORE_ID = GAS_DEALER_TRAN_TRANS.GDTT_STORE_ID AND 
										                     GAS_DEALER_TRAN_MASTER.GDT_TRAN_ID = GAS_DEALER_TRAN_TRANS.GDTT_TRAN_ID INNER JOIN
										                     GLOBAL_GAS_TRAN_TYPES ON GAS_DEALER_TRAN_TRANS.GDTT_TRAN_TYPE = GLOBAL_GAS_TRAN_TYPES.GTP_TRAN_ID
				                    WHERE        (GAS_DEALER_TRAN_MASTER.GDT_STORE_ID = PARMSTOREID) AND (GAS_DEALER_TRAN_TRANS.GDTT_REF_TRAN_DATE >= 'PARMFROMDATE' 
										                     AND GAS_DEALER_TRAN_TRANS.GDTT_REF_TRAN_DATE <= 'PARMTODATE')
				                    group by GDTT_TRAN_TYPE, GDTT_REF_TRAN_DATE, GTP_TRAN_DESC
				                    ) tab
			                    pivot (sum(gdtt_tran_amount) for GTP_TRAN_DESC in (CreditTransaction, Discount, Invoice, Fee)
			                    ) pivottable
		                    ) tab5
	                    group by gdtt_ref_tran_date
	                    ) tab6
                    ON TAB1.SALE_DATE = TAB6.GDTT_REF_TRAN_DATE
                    Order by SALE_DATE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());


                #endregion

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new ReconcilationStatement();
                    obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString()).ToString();
                    obj.CardAmount = Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    obj.PaidAmount = Convert.ToSingle(dr["PAID_AMOUNT"].ToString());
                    obj.GSSInvoiceAmount = Convert.ToSingle(dr["INVOICE_AMOUNT"].ToString());
                    obj.CreditTransaction = Convert.ToSingle(dr["CreditTransaction"].ToString());
                    obj.Discount = Convert.ToSingle(dr["Discount"].ToString());
                    obj.Fee = Convert.ToSingle(dr["Fee"].ToString());
                    obj.VendorInvoiceAmount = Convert.ToSingle(dr["InvoiceAmount"].ToString());

                    fCardAmount += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    fPaidAmount += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    fDiscount += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    fGSSInvoiceAmount += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    fCreditTransaction += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    fFee += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                    fVendorInvoiceAmount += Convert.ToSingle(dr["CARD_AMOUNT"].ToString());

                    objReport.Add(obj);
                }
                dr.Close();

                obj = new ReconcilationStatement();
                obj.Date = "Total";
                obj.CardAmount = fCardAmount;
                obj.PaidAmount = fPaidAmount;
                obj.GSSInvoiceAmount = fDiscount;
                obj.CreditTransaction = fGSSInvoiceAmount;
                obj.Discount = fCreditTransaction;
                obj.Fee = fFee;
                obj.VendorInvoiceAmount = fVendorInvoiceAmount;
                objReport.Add(obj);
                
                return objReport;
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
        
        public List<ReportMonthlyStatement> GetSaleReportMonthly(int StoreID, DateTime FromDate, DateTime ToDate)
        {
            List<ReportMonthlyStatement> objReport = new List<ReportMonthlyStatement>();
            ReportMonthlyStatement obj;
            SqlCommand cmd;
            SqlDataReader dr;
            float fSale = 0;
            float fCardPayment = 0;
            float fPaidAmount = 0;
            float fDiscount = 0;
            float fPurchaseAmount = 0;
            float fBankCharges = 0;
            float fPaymentTotal = 0;
            float fPayableTotal = 0;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Adding sale report
                _sql = @" SELECT distinct SALE_DATE, G.GR_INV_DATE, GR.GDTT_REF_TRAN_DATE, SALE_SHIFT_CODE, SALE_GAS_TOTAL_SALE, SALE_GAS_CARD_TOTAL, BUSINESS_AMOUNT, 
                            G.GR_INV_AMOUNT, GR.GDTT_TRAN_TYPE, GR.GDTT_TRAN_AMOUNT
                            FROM            STORE_SALE_MASTER S
                            LEFT OUTER JOIN STORE_BUSINESS_TRANS B
                            ON S.STORE_ID = B.BUSINESS_STORE_ID AND S.SALE_DAY_TRAN_ID = B.BUSINESS_SALE_DAY_TRAN_ID AND B.BUSINESS_ACTLED_ID = 38 
                            FULL OUTER JOIN GAS_RECEIPT_MASTER G 
                            ON s.STORE_ID = G.GR_STORE_ID AND S.SALE_DATE = G.GR_INV_DATE AND S.SALE_SHIFT_CODE = G.GR_SHIFT_CODE
                            FULL OUTER JOIN GAS_DEALER_TRAN_TRANS GR 
                            ON S.STORE_ID = GR.GDTT_STORE_ID AND S.SALE_DATE = GR.GDTT_REF_TRAN_DATE 
                            WHERE       (	(s.STORE_ID = PARMSTOREID) 
				                            AND (s.SALE_DATE >= 'PARMFROMDATE' 
				                            AND s.SALE_DATE <= 'PARMTODATE')
                                            AND (SALE_GAS_TOTAL_SALE > 0 OR SALE_GAS_CARD_TOTAL > 0))
			                             OR
			                            (	(GR_STORE_ID = PARMSTOREID) 
				                            AND (GR_DATE >= 'PARMFROMDATE' 
				                            AND GR_DATE <= 'PARMTODATE')
				                            AND G.GR_INV_AMOUNT > 0)
			                            OR
			                            (
				                            (GDTT_STORE_ID = PARMSTOREID) 
					                            AND (GDTT_REF_TRAN_DATE >= 'PARMFROMDATE' 
					                            AND GDTT_REF_TRAN_DATE <= 'PARMTODATE')
					                            AND (GDTT_TRAN_TYPE IN (2, 4))
			                            )";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());


                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new ReportMonthlyStatement();
                    if (dr["SALE_DATE"].ToString().Length > 0)
                        obj.Date = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    else if (dr["GR_INV_DATE"].ToString().Length > 0)
                        obj.Date = Convert.ToDateTime(dr["GR_INV_DATE"].ToString());
                    else if (dr["GDTT_REF_TRAN_DATE"].ToString().Length > 0)
                        obj.Date = Convert.ToDateTime(dr["GDTT_REF_TRAN_DATE"].ToString());

                    if (dr["SALE_SHIFT_CODE"].ToString().Length > 0)
                        obj.ShiftCode = Convert.ToInt16(dr["SALE_SHIFT_CODE"].ToString());
                    else
                        obj.ShiftCode = 1;

                    if (dr["SALE_GAS_TOTAL_SALE"].ToString().Length > 0)
                    {
                        obj.Sales = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"].ToString());
                    }

                    if (dr["SALE_GAS_CARD_TOTAL"].ToString().Length > 0)
                    {
                        obj.CardPayment = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"].ToString());
                    }

                    if (dr["BUSINESS_AMOUNT"].ToString().Length > 0)
                    {
                        obj.PaidAmount = Convert.ToSingle(dr["BUSINESS_AMOUNT"].ToString());
                    }

                    if (dr["GR_INV_AMOUNT"].ToString().Length > 0)
                    {
                        obj.PurchaseValue = Convert.ToSingle(dr["GR_INV_AMOUNT"].ToString());
                    }

                    if (dr["GDTT_TRAN_TYPE"].ToString().Length > 0)
                    {
                        if (Convert.ToInt16(dr["GDTT_TRAN_TYPE"].ToString()) == 4)
                            obj.BankCharges = Convert.ToSingle(dr["GDTT_TRAN_AMOUNT"].ToString());
                        else if (Convert.ToInt16(dr["GDTT_TRAN_TYPE"].ToString()) == 2)
                            obj.Discount = Convert.ToSingle(dr["GDTT_TRAN_AMOUNT"].ToString());
                    }

                    objReport.Add(obj);
                }
                dr.Close();

                #endregion

                objReport.Sort(delegate(ReportMonthlyStatement x, ReportMonthlyStatement y)
                {
                    if (x.Date == null && y.Date == null) return 0;
                    else if (x.Date == null) return -1;
                    else if (y.Date == null) return 1;
                    else return x.Date.CompareTo(y.Date);
                });

                #region Adding Total
                DateTime dt = DateTime.Now.Date; ;
                foreach (ReportMonthlyStatement o in objReport)
                {
                    o.TotalPayments = o.PaidAmount + o.CardPayment + o.Discount;
                    o.TotalPayables = o.PurchaseValue + o.BankCharges;

                    fDiscount += o.Discount;
                    fPurchaseAmount += o.PurchaseValue;
                    fBankCharges += o.BankCharges;
                    fCardPayment += o.CardPayment;
                    fSale += o.Sales;
                    fPaidAmount += o.PaidAmount;

                    fPayableTotal += o.TotalPayables;
                    fPaymentTotal += o.TotalPayments;
                    dt = o.Date;
                }

                if (fPayableTotal > 0 || fPaymentTotal > 0)
                {
                    dt = dt.AddDays(1);
                    obj = new ReportMonthlyStatement();
                    obj.Date = dt;
                    obj.Discount = fDiscount;
                    obj.PurchaseValue = fPurchaseAmount;
                    obj.Sales = fSale;
                    obj.CardPayment = fCardPayment;
                    obj.PaidAmount = fPaidAmount;
                    obj.TotalPayments = fPaymentTotal;
                    obj.TotalPayables = fPayableTotal;
                    objReport.Add(obj);
                }
                #endregion

                return objReport;
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

        public List<SaleGraph> MonthlySaleReport(int StoreID, int MonthNo, int YearNo)
        {
            List<SaleGraph> SaleReport = new List<SaleGraph>();
            SaleGraph obj;
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
                List<SaleGraph> MonthlySale = new List<SaleGraph>();

                _sql = @"SELECT STORE_SALE_MASTER.SALE_DATE, SUM(STORE_GAS_SALE_TRAN.SALE_GALLONS) AS SALE_GALLONS, 
                            SUM(STORE_GAS_SALE_TRAN.SALE_AMOUNT) AS SALE_GAS_TOTAL_SALE
                            FROM            STORE_SALE_MASTER INNER JOIN
                            STORE_GAS_SALE_TRAN ON STORE_SALE_MASTER.STORE_ID = STORE_GAS_SALE_TRAN.STORE_ID AND STORE_SALE_MASTER.SALE_DAY_TRAN_ID = STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID
                            WHERE        (STORE_SALE_MASTER.STORE_ID = PARMSTOREID)
                            GROUP BY STORE_SALE_MASTER.SALE_DATE
                            HAVING        (STORE_SALE_MASTER.SALE_DATE >= 'PARMFROMDATE' AND STORE_SALE_MASTER.SALE_DATE <= 'PARMTODATE')
                            ORDER BY STORE_SALE_MASTER.SALE_DATE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMFROMDATE", FromDate.ToString());
                _sql = _sql.Replace("PARMTODATE", ToDate.ToString());

                cmd = new SqlCommand(_sql, _conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    obj = new SaleGraph();
                    obj.SaleDate = Convert.ToDateTime(dr["SALE_DATE"].ToString());
                    obj.SaleAmount = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"].ToString());
                    obj.SaleGallons = Convert.ToSingle(dr["SALE_GALLONS"].ToString());
                    MonthlySale.Add(obj);
                }
                dr.Close();
                #endregion

                foreach(SaleGraph s in MonthlySale)
                {

                }

                SaleGraph objTemp;

                for (DateTime i = FromDate; i <= ToDate; i = i.AddDays(1))
                {
                    obj = new SaleGraph();
                    obj.SaleDate = i;
                    objTemp = MonthlySale.Find(x => x.SaleDate == i);
                    if (objTemp != null)
                    {
                        obj.SaleAmount = objTemp.SaleAmount;
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

        public DayEndModel DayEndReport(int StoreID, DateTime dDate, int ShiftID)
        {
            DayEndModel dayEndModel = new DayEndModel();
            string _sql;
            float fTotal = 0;
            try
            {
                using (SqlConnection _conn = new SqlConnection(DMLExecute.con))
                {
                    List<DaySale> daySales = new List<DaySale>();
                    DaySale daySale;
                    List<DayCardReceipt> dayCardReceipts = new List<DayCardReceipt>();
                    DayCardReceipt dayCardReceipt;
                    List<DayStock> dayStocks = new List<DayStock>();
                    DayStock dayStock;
                    List<DayPurchase> dayPurchases = new List<DayPurchase>();
                    DayPurchase dayPurchase;


                    SqlCommand sqlcmd;
                    SqlDataReader sqldr;
                    _conn.Open();

                    #region Adding Day Sales
                    _sql = @"SELECT   GROUP_GASTYPE_MASTER.GASTYPE_NAME, STORE_GAS_SALE_TRAN.SALE_GALLONS, STORE_GAS_SALE_TRAN.SALE_PRICE, 
                                 STORE_GAS_SALE_TRAN.SALE_AMOUNT, STORE_GAS_SALE_TRAN.SET_PRICE
                                FROM    STORE_GAS_SALE_TRAN INNER JOIN
                                        STORE_SALE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                        STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                        GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_TRAN.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                        STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                        GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                                WHERE        (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')  ";

                    if (ShiftID != 0)
                        _sql += " AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = " + ShiftID + ")";

                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());
                    _sql += "ORDER BY GASTYPE_NAME";

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();

                    while (sqldr.Read())
                    {
                        daySale = new DaySale();
                        daySale.ShiftCode = ShiftID;
                        daySale.GasType = sqldr["GASTYPE_NAME"].ToString();
                        daySale.SaleQty = Convert.ToSingle(sqldr["SALE_GALLONS"].ToString());
                        daySale.SaleAmount = Convert.ToSingle(sqldr["SALE_AMOUNT"].ToString());
                        fTotal += daySale.SaleAmount;
                        daySale.UnitPrice = Convert.ToSingle(sqldr["SALE_PRICE"].ToString());
                        daySale.SetPrice = Convert.ToSingle(sqldr["SET_PRICE"].ToString());
                        daySales.Add(daySale);
                    }
                    sqldr.Close();

                    daySale = new DaySale();
                    daySale.GasType = "Total";
                    daySale.SaleAmount = fTotal;
                    daySales.Add(daySale);
                    #endregion

                    #region Adding Day Card Receipts
                    _sql = @"SELECT   GROUP_CARD_TYPE.GROU_CARD_NAME, STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT
                                FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                    STORE_SALE_MASTER ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                    STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                    GROUP_CARD_TYPE ON STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                                WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')  ";

                    if (ShiftID != 0)
                        _sql += " AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = " + ShiftID + ")";

                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();
                    fTotal = 0;
                    while (sqldr.Read())
                    {
                        dayCardReceipt = new DayCardReceipt();
                        dayCardReceipt.ShiftCode = ShiftID;
                        dayCardReceipt.CardName = sqldr["GROU_CARD_NAME"].ToString();
                        dayCardReceipt.CardAmount = Convert.ToSingle(sqldr["CARD_AMOUNT"].ToString());
                        fTotal += dayCardReceipt.CardAmount;
                        dayCardReceipts.Add(dayCardReceipt);
                    }
                    sqldr.Close();

                    dayCardReceipt = new DayCardReceipt();
                    dayCardReceipt.CardName = "Total";
                    dayCardReceipt.CardAmount = fTotal;
                    dayCardReceipts.Add(dayCardReceipt);
                    fTotal = 0;
                    #endregion

                    #region Adding Day Stock
                    _sql = @"SELECT  GROUP_GASTYPE_MASTER.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME, STORE_GAS_SALE_BALANCES.GAS_OP_BAL, 
                                                         STORE_GAS_SALE_BALANCES.GAS_RECV, STORE_GAS_SALE_BALANCES.GAS_ACT_CL_BAL,
                                                            STORE_GAS_SALE_BALANCES.GAS_SYSTEM_CL_BAL,
                                                         ISNULL(STORE_GAS_SALE_TRAN.SALE_GALLONS,0) AS SALE_GALLONS,
                                                        ISNULL(STORE_GAS_SALE_TRAN.SALE_AMOUNT,0) AS SALE_AMOUNT
                                FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                        STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                        STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                        GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                        STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                        GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID LEFT OUTER JOIN
                                        STORE_GAS_SALE_TRAN ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_GAS_SALE_TRAN.STORE_ID AND 
                                        STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID AND 
                                        STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = STORE_GAS_SALE_TRAN.GASTYPE_ID
                                WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')  ";

                    if (ShiftID != 0)
                        _sql += " AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = " + ShiftID + ")";

                    _sql += " ORDER BY GASTYPE_NAME";
                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();
                    fTotal = 0;
                    while (sqldr.Read())
                    {
                        dayStock = new DayStock();
                        dayStock.ShiftCode = ShiftID;
                        dayStock.GasTypeID = Convert.ToInt16(sqldr["GASTYPE_ID"].ToString()); 
                        dayStock.GasType = sqldr["GASTYPE_NAME"].ToString();
                        dayStock.OpenQty = Convert.ToSingle(sqldr["GAS_OP_BAL"].ToString());
                        dayStock.SaleQty = Convert.ToSingle(sqldr["SALE_GALLONS"].ToString());
                        dayStock.InwardQty = Convert.ToSingle(sqldr["GAS_RECV"].ToString());
                        dayStock.ClosingQty = Convert.ToSingle(sqldr["GAS_ACT_CL_BAL"].ToString());
                        dayStock.SalePrice = Convert.ToSingle(sqldr["SALE_AMOUNT"].ToString());
                        dayStock.SystemClosingQty = Convert.ToSingle(sqldr["GAS_SYSTEM_CL_BAL"].ToString());
                        dayStock.ShortOver = dayStock.SystemClosingQty - dayStock.ClosingQty;

                        dayStocks.Add(dayStock);
                    }
                    sqldr.Close();

                    #endregion

                    #region Adding Purchases
                    _sql = @"SELECT   GAS_RECEIPT_MASTER.GR_SHIFT_CODE, GAS_RECEIPT_MASTER.GR_INV_AMOUNT, GAS_RECEIPT_MASTER.GR_INV_NO, 
                                    GAS_RECEIPT_MASTER.GR_DUE_DATE, GROUP_GASTYPE_MASTER.GASTYPE_NAME, GAS_RECEIPT_DELIVERY.GRV_NET_GALLONS, 
                                    GAS_RECEIPT_DELIVERY.GRV_INV_NET_GALLONS
                                FROM            GAS_RECEIPT_MASTER INNER JOIN
                                    GAS_RECEIPT_DELIVERY ON GAS_RECEIPT_MASTER.GR_STORE_ID = GAS_RECEIPT_DELIVERY.GRV_STORE_ID AND 
                                    GAS_RECEIPT_MASTER.GR_TRAN_ID = GAS_RECEIPT_DELIVERY.GRV_TRAN_ID INNER JOIN
                                    STORE_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                    GROUP_GASTYPE_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID AND 
                                    GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                                WHERE        (GAS_RECEIPT_MASTER.GR_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_MASTER.GR_DATE = 'PARMDATE') ";

                    if (ShiftID != 0)
                        _sql += " AND (GAS_RECEIPT_MASTER.GR_SHIFT_CODE = " + ShiftID + ")";

                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();

                    while (sqldr.Read())
                    {
                        dayPurchase = new DayPurchase();
                        dayPurchase.ShiftID = Convert.ToInt16(sqldr["GR_SHIFT_CODE"].ToString());
                        dayPurchase.InvoiceNo = sqldr["GR_INV_NO"].ToString();
                        dayPurchase.GasType = sqldr["GASTYPE_NAME"].ToString();
                        dayPurchase.InwardQty = Convert.ToSingle(sqldr["GRV_NET_GALLONS"].ToString());
                        dayPurchase.InvoiceQty = Convert.ToSingle(sqldr["GRV_INV_NET_GALLONS"].ToString());
                        dayPurchase.InvoiceAmount = Convert.ToSingle(sqldr["GR_INV_AMOUNT"].ToString());
                        dayPurchase.DueDate = Convert.ToDateTime(sqldr["GR_DUE_DATE"].ToString());
                        dayPurchases.Add(dayPurchase);
                    }
                    sqldr.Close();

                    #endregion
                    
                    dayEndModel.DaySales = daySales;
                    dayEndModel.DayCardReceipts = dayCardReceipts;
                    dayEndModel.DayStocks = dayStocks;
                    dayEndModel.DayPurchases = dayPurchases;

                }
                return dayEndModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DayEndModel GasSystemClosingBalanance(int StoreID, DateTime dDate, int ShiftID)
        {
            DayEndModel dayEndModel = new DayEndModel();
            string _sql;
            float fTotal = 0;
            try
            {
                using (SqlConnection _conn = new SqlConnection(DMLExecute.con))
                {
                    List<DaySale> daySales = new List<DaySale>();
                    DaySale daySale;
                    List<DayCardReceipt> dayCardReceipts = new List<DayCardReceipt>();
                    List<DayStock> dayStocks = new List<DayStock>();
                    DayStock dayStock;
                    List<DayPurchase> dayPurchases = new List<DayPurchase>();
                    DayPurchase dayPurchase;


                    SqlCommand sqlcmd;
                    SqlDataReader sqldr;
                    _conn.Open();

                    #region Adding Day Sales
                    _sql = @"SELECT   GROUP_GASTYPE_MASTER.GASTYPE_NAME, STORE_GAS_SALE_TRAN.SALE_GALLONS, STORE_GAS_SALE_TRAN.SALE_PRICE, 
                                 STORE_GAS_SALE_TRAN.SALE_AMOUNT, STORE_GAS_SALE_TRAN.SET_PRICE
                                FROM    STORE_GAS_SALE_TRAN INNER JOIN
                                        STORE_SALE_MASTER ON STORE_GAS_SALE_TRAN.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                        STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                        GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_TRAN.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                        STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                        GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                                WHERE        (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')  ";

                    if (ShiftID != 0)
                        _sql += " AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = " + ShiftID + ")";

                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());
                    _sql += "ORDER BY GASTYPE_NAME";

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();

                    while (sqldr.Read())
                    {
                        daySale = new DaySale();
                        daySale.ShiftCode = ShiftID;
                        daySale.GasType = sqldr["GASTYPE_NAME"].ToString();
                        daySale.SaleQty = Convert.ToSingle(sqldr["SALE_GALLONS"].ToString());
                        daySale.SaleAmount = Convert.ToSingle(sqldr["SALE_AMOUNT"].ToString());
                        fTotal += daySale.SaleAmount;
                        daySale.UnitPrice = Convert.ToSingle(sqldr["SALE_PRICE"].ToString());
                        daySale.SetPrice = Convert.ToSingle(sqldr["SET_PRICE"].ToString());
                        daySales.Add(daySale);
                    }
                    sqldr.Close();

                    daySale = new DaySale();
                    daySale.GasType = "Total";
                    daySale.SaleAmount = fTotal;
                    daySales.Add(daySale);
                    #endregion

                    #region Adding Day Stock
                    _sql = @"SELECT  GROUP_GASTYPE_MASTER.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME, STORE_GAS_SALE_BALANCES.GAS_OP_BAL, 
                                                         STORE_GAS_SALE_BALANCES.GAS_RECV, STORE_GAS_SALE_BALANCES.GAS_ACT_CL_BAL, 
                                                         ISNULL(STORE_GAS_SALE_TRAN.SALE_GALLONS,0) AS SALE_GALLONS,
                                                        ISNULL(STORE_GAS_SALE_TRAN.SALE_AMOUNT,0) AS SALE_AMOUNT
                                FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                        STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                        STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                        GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                        STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                        GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID LEFT OUTER JOIN
                                        STORE_GAS_SALE_TRAN ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_GAS_SALE_TRAN.STORE_ID AND 
                                        STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID AND 
                                        STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = STORE_GAS_SALE_TRAN.GASTYPE_ID
                                WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')  ";

                    if (ShiftID != 0)
                        _sql += " AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = " + ShiftID + ")";

                    _sql += " ORDER BY GASTYPE_NAME";
                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();
                    fTotal = 0;
                    while (sqldr.Read())
                    {
                        dayStock = new DayStock();
                        dayStock.ShiftCode = ShiftID;
                        dayStock.GasTypeID = Convert.ToInt16(sqldr["GASTYPE_ID"].ToString());
                        dayStock.GasType = sqldr["GASTYPE_NAME"].ToString();
                        dayStock.OpenQty = Convert.ToSingle(sqldr["GAS_OP_BAL"].ToString());
                        dayStock.SaleQty = Convert.ToSingle(sqldr["SALE_GALLONS"].ToString());
                        dayStock.InwardQty = Convert.ToSingle(sqldr["GAS_RECV"].ToString());
                        dayStock.ClosingQty = Convert.ToSingle(sqldr["GAS_ACT_CL_BAL"].ToString());
                        dayStock.SalePrice = Convert.ToSingle(sqldr["SALE_AMOUNT"].ToString());
                        dayStocks.Add(dayStock);
                    }
                    sqldr.Close();

                    // Calculating sales and distributing for mid grade to unlead and premium
                    _sql = @"SELECT STORE_GAS_SALE_TRAN.SALE_GALLONS, MAPPING_STORE_GAS.GASTYPE_CONSUMPTION_FORMULA
                                FROM            STORE_SALE_MASTER INNER JOIN
                                    STORE_GAS_SALE_TRAN ON STORE_SALE_MASTER.STORE_ID = STORE_GAS_SALE_TRAN.STORE_ID AND 
                                    STORE_SALE_MASTER.SALE_DAY_TRAN_ID = STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID INNER JOIN
                                    MAPPING_STORE_GAS ON STORE_GAS_SALE_TRAN.STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                    STORE_GAS_SALE_TRAN.GASTYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID
                                WHERE        (STORE_GAS_SALE_TRAN.SALE_DATE = 'PARMDATE') AND (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND 
                                    (STORE_GAS_SALE_TRAN.GASTYPE_ID = 2)";

                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();
                    float fMidgradeSale = 0;
                    string fConsumptionFormula = string.Empty;
                    if (sqldr.Read())
                    {
                        fMidgradeSale = Convert.ToSingle(sqldr["SALE_GALLONS"]);
                        fConsumptionFormula = sqldr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                    }
                    sqldr.Close();

                    if (fMidgradeSale > 0)
                    {
                        List<GasOilFormula> gasOilFormula = CommonFunctions.ParseGasOilFormula(fConsumptionFormula);

                        foreach (GasOilFormula g in gasOilFormula)
                        {
                            foreach (DayStock o in dayStocks)
                            {
                                if (g.GasTypeID == o.GasTypeID)
                                {
                                    o.SaleQty = o.SaleQty + ((fMidgradeSale * g.GasOilConsmptPercent) / 100);
                                }
                            }
                        }

                    }

                    // Updating purchase price
                    float fPurchasePrice;
                    float fPurchaseUnitPrice;
                    foreach (DayStock o in dayStocks)
                    {
                        if (o.OpenQty > 0)
                        {
                            _sql = @"SELECT        TOP (1) GAS_RECEIPT_DELIVERY.GRV_INV_PRICE
                                        FROM            GAS_RECEIPT_DELIVERY INNER JOIN
                                            GAS_RECEIPT_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND
                                            GAS_RECEIPT_DELIVERY.GRV_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID
                                        WHERE        (GAS_RECEIPT_DELIVERY.GRV_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = PARMGASTYPEID) AND 
                                            (GAS_RECEIPT_MASTER.GR_DATE < 'PARMDATE')
                                            ORDER BY GR_DATE DESC";
                            _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                            _sql = _sql.Replace("PARMDATE", dDate.ToString());
                            _sql = _sql.Replace("PARMGASTYPEID", o.GasTypeID.ToString());

                            sqlcmd = new SqlCommand(_sql, _conn);
                            if (sqlcmd.ExecuteScalar() != null)
                            {
                                fPurchaseUnitPrice = Convert.ToSingle(sqlcmd.ExecuteScalar());
                                fPurchasePrice = o.SaleQty * fPurchaseUnitPrice;
                                if (o.OpenQty < o.SaleQty)
                                {
                                    fPurchasePrice = (o.OpenQty) * fPurchaseUnitPrice;
                                    // To calculate if has stock inward on that day to take stock inward price for remaining stock (sale qty - open qty)
                                    _sql = @"SELECT        TOP (1) GAS_RECEIPT_DELIVERY.GRV_INV_PRICE
                                        FROM            GAS_RECEIPT_DELIVERY INNER JOIN
                                            GAS_RECEIPT_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND
                                            GAS_RECEIPT_DELIVERY.GRV_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID
                                        WHERE        (GAS_RECEIPT_DELIVERY.GRV_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = PARMGASTYPEID) AND 
                                            (GAS_RECEIPT_MASTER.GR_DATE >= 'PARMDATE')
                                            ORDER BY GR_DATE";
                                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                                    _sql = _sql.Replace("PARMDATE", dDate.ToString());
                                    _sql = _sql.Replace("PARMGASTYPEID", o.GasTypeID.ToString());

                                    sqlcmd = new SqlCommand(_sql, _conn);
                                    if (sqlcmd.ExecuteScalar() != null)
                                    {
                                        fPurchaseUnitPrice = Convert.ToSingle(sqlcmd.ExecuteScalar());
                                        fPurchasePrice = fPurchasePrice + (o.SaleQty - o.OpenQty) * fPurchaseUnitPrice;
                                    }
                                }
                                o.PurchasePrice = fPurchasePrice;
                                o.ProfitLoss = o.SalePrice - o.PurchasePrice;
                            }
                        }

                    }


                    #endregion

                    foreach (DayStock o in dayStocks)
                    {
                        o.SystemClosingQty = o.InwardQty + o.OpenQty - o.SaleQty;
                        o.ShortOver = o.SystemClosingQty - o.ClosingQty;
                    }

                    #region Adding Purchases
                    _sql = @"SELECT   GAS_RECEIPT_MASTER.GR_SHIFT_CODE, GAS_RECEIPT_MASTER.GR_INV_AMOUNT, GAS_RECEIPT_MASTER.GR_INV_NO, 
                                    GAS_RECEIPT_MASTER.GR_DUE_DATE, GROUP_GASTYPE_MASTER.GASTYPE_NAME, GAS_RECEIPT_DELIVERY.GRV_NET_GALLONS, 
                                    GAS_RECEIPT_DELIVERY.GRV_INV_NET_GALLONS
                                FROM            GAS_RECEIPT_MASTER INNER JOIN
                                    GAS_RECEIPT_DELIVERY ON GAS_RECEIPT_MASTER.GR_STORE_ID = GAS_RECEIPT_DELIVERY.GRV_STORE_ID AND 
                                    GAS_RECEIPT_MASTER.GR_TRAN_ID = GAS_RECEIPT_DELIVERY.GRV_TRAN_ID INNER JOIN
                                    STORE_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                    GROUP_GASTYPE_MASTER ON STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID AND 
                                    GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                                WHERE        (GAS_RECEIPT_MASTER.GR_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_MASTER.GR_DATE = 'PARMDATE') ";

                    if (ShiftID != 0)
                        _sql += " AND (GAS_RECEIPT_MASTER.GR_SHIFT_CODE = " + ShiftID + ")";

                    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                    _sql = _sql.Replace("PARMDATE", dDate.ToString());

                    sqlcmd = new SqlCommand(_sql, _conn);
                    sqldr = sqlcmd.ExecuteReader();

                    while (sqldr.Read())
                    {
                        dayPurchase = new DayPurchase();
                        dayPurchase.ShiftID = Convert.ToInt16(sqldr["GR_SHIFT_CODE"].ToString());
                        dayPurchase.InvoiceNo = sqldr["GR_INV_NO"].ToString();
                        dayPurchase.GasType = sqldr["GASTYPE_NAME"].ToString();
                        dayPurchase.InwardQty = Convert.ToSingle(sqldr["GRV_NET_GALLONS"].ToString());
                        dayPurchase.InvoiceQty = Convert.ToSingle(sqldr["GRV_INV_NET_GALLONS"].ToString());
                        dayPurchase.InvoiceAmount = Convert.ToSingle(sqldr["GR_INV_AMOUNT"].ToString());
                        dayPurchase.DueDate = Convert.ToDateTime(sqldr["GR_DUE_DATE"].ToString());
                        dayPurchases.Add(dayPurchase);
                    }
                    sqldr.Close();

                    #endregion

                    dayEndModel.DaySales = daySales;
                    dayEndModel.DayCardReceipts = dayCardReceipts;
                    dayEndModel.DayStocks = dayStocks;
                    dayEndModel.DayPurchases = dayPurchases;

                }
                return dayEndModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<DayStock> ReturnProfitLoss(int StoreID,DateTime dFrom, DateTime dTo, int GasTypeID)
        {
            string _sql;
            List<DayStock> objDayStockColl = new List<DayStock>();
            DayStock dayStock;

            try
            {
                using (SqlConnection _conn = new SqlConnection(DMLExecute.con))
                {
                    SqlCommand sqlcmd;
                    SqlDataReader sqldr;
                    int ShiftID = 1;
                    _conn.Open();
                    while (dFrom <= dTo)
                    {
                        #region Adding Day Stock
                        _sql = @"SELECT  GROUP_GASTYPE_MASTER.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME, STORE_GAS_SALE_BALANCES.GAS_OP_BAL, 
                                                         STORE_GAS_SALE_BALANCES.GAS_RECV, STORE_GAS_SALE_BALANCES.GAS_SYSTEM_CL_BAL AS GAS_ACT_CL_BAL, 
                                                         ISNULL(STORE_GAS_SALE_TRAN.SALE_GALLONS,0) AS SALE_GALLONS,
                                                        ISNULL(STORE_GAS_SALE_TRAN.SALE_AMOUNT,0) AS SALE_AMOUNT
                                FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                        STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                        STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID INNER JOIN
                                        GROUP_GASTYPE_MASTER ON STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                        STORE_MASTER ON STORE_SALE_MASTER.STORE_ID = STORE_MASTER.STORE_ID AND 
                                        GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID LEFT OUTER JOIN
                                        STORE_GAS_SALE_TRAN ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_GAS_SALE_TRAN.STORE_ID AND 
                                        STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID AND 
                                        STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = STORE_GAS_SALE_TRAN.GASTYPE_ID
                                WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTOREID) AND (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')  
                                        AND STORE_GAS_SALE_TRAN.GASTYPE_ID = PARMGASTYPEID";

                        if (ShiftID != 0)
                            _sql += " AND (STORE_SALE_MASTER.SALE_SHIFT_CODE = " + ShiftID + ")";

                        _sql += " ORDER BY GASTYPE_ID";
                        _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                        _sql = _sql.Replace("PARMDATE", dFrom.ToString());
                        _sql = _sql.Replace("PARMGASTYPEID", GasTypeID.ToString());

                        sqlcmd = new SqlCommand(_sql, _conn);
                        sqldr = sqlcmd.ExecuteReader();

                        while (sqldr.Read())
                        {
                            dayStock = new DayStock();
                            dayStock.ShiftCode = ShiftID;
                            dayStock.Date = dFrom;
                            dayStock.GasTypeID = Convert.ToInt16(sqldr["GASTYPE_ID"].ToString());
                            dayStock.GasType = sqldr["GASTYPE_NAME"].ToString();
                            dayStock.OpenQty = Convert.ToSingle(sqldr["GAS_OP_BAL"].ToString());
                            dayStock.SaleQty = Convert.ToSingle(sqldr["SALE_GALLONS"].ToString());
                            dayStock.InwardQty = Convert.ToSingle(sqldr["GAS_RECV"].ToString());
                            dayStock.ClosingQty = Convert.ToSingle(sqldr["GAS_ACT_CL_BAL"].ToString());
                            dayStock.SalePrice = Convert.ToSingle(sqldr["SALE_AMOUNT"].ToString());
                            objDayStockColl.Add(dayStock);
                        }
                        sqldr.Close();
                        #endregion
                        dFrom = dFrom.AddDays(1);
                    } 

                        // Calculating sales and distributing for mid grade to unlead and premium
                        //_sql = @"SELECT STORE_GAS_SALE_TRAN.SALE_GALLONS, MAPPING_STORE_GAS.GASTYPE_CONSUMPTION_FORMULA
                        //        FROM            STORE_SALE_MASTER INNER JOIN
                        //            STORE_GAS_SALE_TRAN ON STORE_SALE_MASTER.STORE_ID = STORE_GAS_SALE_TRAN.STORE_ID AND 
                        //            STORE_SALE_MASTER.SALE_DAY_TRAN_ID = STORE_GAS_SALE_TRAN.SALE_DAY_TRAN_ID INNER JOIN
                        //            MAPPING_STORE_GAS ON STORE_GAS_SALE_TRAN.STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                        //            STORE_GAS_SALE_TRAN.GASTYPE_ID = MAPPING_STORE_GAS.GASTYPE_ID
                        //        WHERE        (STORE_GAS_SALE_TRAN.SALE_DATE = 'PARMDATE') AND (STORE_GAS_SALE_TRAN.STORE_ID = PARMSTOREID) AND 
                        //            (STORE_GAS_SALE_TRAN.GASTYPE_ID = 2)";

                        //_sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                        //_sql = _sql.Replace("PARMDATE", dFrom.ToString());

                        //sqlcmd = new SqlCommand(_sql, _conn);
                        //sqldr = sqlcmd.ExecuteReader();
                        //float fMidgradeSale = 0;
                        //string fConsumptionFormula = string.Empty;
                        //if (sqldr.Read())
                        //{
                        //    fMidgradeSale = Convert.ToSingle(sqldr["SALE_GALLONS"]);
                        //    fConsumptionFormula = sqldr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                        //}
                        //sqldr.Close();

                        //if (fMidgradeSale > 0)
                        //{
                        //    List<GasOilFormula> gasOilFormula = CommonFunctions.ParseGasOilFormula(fConsumptionFormula);

                        //    foreach (GasOilFormula g in gasOilFormula)
                        //    {
                        //        foreach (DayStock o in objDayStockColl)
                        //        {
                        //            if (g.GasTypeID == o.GasTypeID)
                        //            {
                        //                o.SaleQty = o.SaleQty + ((fMidgradeSale * g.GasOilConsmptPercent) / 100);
                        //            }
                        //        }
                        //    }

                        //}

                        // Updating purchase price
                        float fPurchasePrice = 0;
                        float fPurchaseUnitPrice = 0;
                        
                        foreach (DayStock o in objDayStockColl)
                        {
                            //if (o.OpenQty > 0)
                            //{
                            _sql = @"SELECT        TOP (1) GAS_RECEIPT_DELIVERY.GRV_INV_PRICE
                                        FROM            GAS_RECEIPT_DELIVERY INNER JOIN
                                            GAS_RECEIPT_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND
                                            GAS_RECEIPT_DELIVERY.GRV_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID
                                        WHERE        (GAS_RECEIPT_DELIVERY.GRV_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = PARMGASTYPEID) AND 
                                            (GAS_RECEIPT_MASTER.GR_DATE <= 'PARMDATE') AND GAS_RECEIPT_DELIVERY.GRV_INV_PRICE > 0
                                            ORDER BY GR_DATE DESC";
                            _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                            _sql = _sql.Replace("PARMDATE", dFrom.ToString());
                            _sql = _sql.Replace("PARMGASTYPEID", o.GasTypeID.ToString());

                            sqlcmd = new SqlCommand(_sql, _conn);
                            if (sqlcmd.ExecuteScalar() != null)
                            {
                                fPurchaseUnitPrice = Convert.ToSingle(sqlcmd.ExecuteScalar());
                            }
                            else
                            {
                                _sql = @"SELECT        TOP (1) GAS_RECEIPT_DELIVERY.GRV_INV_PRICE
                                        FROM            GAS_RECEIPT_DELIVERY INNER JOIN
                                            GAS_RECEIPT_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND
                                            GAS_RECEIPT_DELIVERY.GRV_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID
                                        WHERE        (GAS_RECEIPT_DELIVERY.GRV_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = PARMGASTYPEID) AND 
                                            (GAS_RECEIPT_MASTER.GR_DATE > 'PARMDATE')
                                            ORDER BY GR_DATE ";
                                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                                _sql = _sql.Replace("PARMDATE", dFrom.ToString());
                                _sql = _sql.Replace("PARMGASTYPEID", o.GasTypeID.ToString());
                                sqlcmd = new SqlCommand(_sql, _conn);


                                if (sqlcmd.ExecuteScalar() != null)
                                {
                                    fPurchaseUnitPrice = Convert.ToSingle(sqlcmd.ExecuteScalar());
                                }
                            }
                            if (fPurchaseUnitPrice >= 0)
                            {
                                fPurchasePrice = o.SaleQty * fPurchaseUnitPrice;
                                #region Previous logic to calculate on FIFO method but it will not accurate because of incorrect opening balances so commented

                                //if (o.OpenQty < o.SaleQty)
                                //{
                                //    fPurchasePrice = (o.OpenQty) * fPurchaseUnitPrice;
                                //    // To calculate if has stock inward on that day to take stock inward price for remaining stock (sale qty - open qty)
                                //    _sql = @"SELECT        TOP (1) GAS_RECEIPT_DELIVERY.GRV_INV_PRICE
                                //        FROM            GAS_RECEIPT_DELIVERY INNER JOIN
                                //            GAS_RECEIPT_MASTER ON GAS_RECEIPT_DELIVERY.GRV_STORE_ID = GAS_RECEIPT_MASTER.GR_STORE_ID AND
                                //            GAS_RECEIPT_DELIVERY.GRV_TRAN_ID = GAS_RECEIPT_MASTER.GR_TRAN_ID
                                //        WHERE        (GAS_RECEIPT_DELIVERY.GRV_STORE_ID = PARMSTOREID) AND (GAS_RECEIPT_DELIVERY.GRV_GAS_TYPE_ID = PARMGASTYPEID) AND 
                                //            (GAS_RECEIPT_MASTER.GR_DATE >= 'PARMDATE')
                                //            ORDER BY GR_DATE";
                                //    _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                                //    _sql = _sql.Replace("PARMDATE", dFrom.ToString());
                                //    _sql = _sql.Replace("PARMGASTYPEID", o.GasTypeID.ToString());

                                //    sqlcmd = new SqlCommand(_sql, _conn);
                                //    if (sqlcmd.ExecuteScalar() != null)
                                //    {
                                //        fPurchaseUnitPrice = Convert.ToSingle(sqlcmd.ExecuteScalar());
                                //        fPurchasePrice = fPurchasePrice + (o.SaleQty - o.OpenQty) * fPurchaseUnitPrice;
                                //    }
                                //}
                                //o.PurchasePrice = fPurchasePrice;
                                //o.ProfitLoss = o.SalePrice - o.PurchasePrice;
                            }
                            #endregion
                            o.PurchasePrice = fPurchasePrice;
                            o.ProfitLoss = o.SalePrice - o.PurchasePrice;
                        }

                        // }
                    _conn.Close();
                }
                return objDayStockColl;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
