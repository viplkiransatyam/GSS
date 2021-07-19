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
    public class ReportGasMonthlyStatement
    {
        SqlConnection _conn;
        string _sql;
        private int StoreID;
        string MonthName;
        private int Year;
        string sFromDate;
        string sEndDate;
        DateTime dFromDate;
        DateTime dEndDate;
        List<GasSaleCollection> objGasSale = new List<GasSaleCollection>();
        List<GasCardCollection> objGasCard = new List<GasCardCollection>();
        List<GasMasterRecord> objGasMasterRecord = new List<GasMasterRecord>();
        List<GasReceived> objGasReceived = new List<GasReceived>();

        public ReportGasMonthlyStatementModel GetMonthlyStatementNormal(int iStoreID, string sMonth, int iYear)
        {
            ReportGasMonthlyStatementModel objMonStmt = new ReportGasMonthlyStatementModel();
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

                objMonStmt.GasSale = GasSaleTable();
                objMonStmt.GasReceipt = CardTable();
                objMonStmt.ReceivedGallon = GasReceivedQuantity();
                objMonStmt.PurchasePrice = GasReceivedPrice();

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

        private DataTable GasSaleTable()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;

            _sql = @"SELECT        MAPPING_STORE_GAS.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME
                        FROM            MAPPING_STORE_GAS INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                        WHERE        (MAPPING_STORE_GAS.STORE_ID = " + StoreID + ") AND (MAPPING_STORE_GAS.ACTIVE = 'A')";
                        
            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            while (dr.Read())
            {
                dtblTemp.Columns.Add(dr["GASTYPE_ID"].ToString() + "_" + dr["GASTYPE_NAME"].ToString() + "_TEST");
            }
            dtblTemp.Columns.Add("TOTAL_GALLON");
            dtblTemp.Columns.Add("TOTAL_SALE");
            dr.Close();

            FillGasSale();
           
            float fTotalGallon = 0;

            while (dFromDate <= dEndDate)
            {
                fTotalGallon = 0;
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count - 2; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    drRow[dtblTemp.Columns[i].ColumnName] = GetSaleAmount(Convert.ToInt16(sTemp[0]), dFromDate, "GS")[0];
                    fTotalGallon += Convert.ToSingle(drRow[dtblTemp.Columns[i].ColumnName]);
                }

                drRow["TOTAL_GALLON"] = fTotalGallon;
                drRow["TOTAL_SALE"] = GetSaleAmount(0, dFromDate, "MS")[0];

                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] fTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++)
            {
                for (int j = 1; j < dtblTemp.Columns.Count; j++)
                {
                    fTotal[j] = fTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count - 1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = fTotal[k].ToString();
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
                    dtblTemp.Columns[i].ColumnName = sTemp[1].Trim();
                }
            }
            return dtblTemp;
        }

        private DataTable CardTable()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;
            dFromDate = Convert.ToDateTime(sFromDate);


            _sql = @"SELECT  MAPPING_STORE_CARD.CARD_TYPE_ID, GROUP_CARD_TYPE.GROU_CARD_NAME
                        FROM            MAPPING_STORE_CARD INNER JOIN
                                                 GROUP_CARD_TYPE ON MAPPING_STORE_CARD.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID
                        WHERE        (MAPPING_STORE_CARD.STORE_ID = " + StoreID + ") AND (MAPPING_STORE_CARD.ACTIVE = 'A')";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            while (dr.Read())
            {
                dtblTemp.Columns.Add(dr["CARD_TYPE_ID"].ToString() + "_" + dr["GROU_CARD_NAME"].ToString() + "_TEST");
            }
            dtblTemp.Columns.Add("TOTAL_CARD");
            dtblTemp.Columns.Add("TOTAL_DEPOSIT");
            dr.Close();

            float fTotalCard = 0;
            while (dFromDate <= dEndDate)
            {
                fTotalCard = 0;
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count - 2; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    drRow[dtblTemp.Columns[i].ColumnName] = GetSaleAmount(Convert.ToInt16(sTemp[0]), dFromDate, "CA")[0];
                    fTotalCard += Convert.ToSingle(drRow[dtblTemp.Columns[i].ColumnName]);
                }
                drRow["TOTAL_CARD"] = fTotalCard;
                drRow["TOTAL_DEPOSIT"] = GetSaleAmount(0, dFromDate, "MS")[1];

                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] fTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++)
            {
                for (int j = 1; j < dtblTemp.Columns.Count; j++)
                {
                    fTotal[j] = fTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count - 1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = fTotal[k].ToString();
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
                    dtblTemp.Columns[i].ColumnName = sTemp[1].Trim();
                }
            }
            return dtblTemp;
        }

        private DataTable GasReceivedQuantity()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;
            dFromDate = Convert.ToDateTime(sFromDate);


            _sql = @"SELECT        MAPPING_STORE_GAS.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME
                        FROM            MAPPING_STORE_GAS INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                        WHERE        (MAPPING_STORE_GAS.STORE_ID = " + StoreID + ") AND (MAPPING_STORE_GAS.ACTIVE = 'A')";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            while (dr.Read())
            {
                dtblTemp.Columns.Add(dr["GASTYPE_ID"].ToString() + "_" + dr["GASTYPE_NAME"].ToString() + "_TEST");
            }
            dr.Close();

            while (dFromDate <= dEndDate)
            {
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count ; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    drRow[dtblTemp.Columns[i].ColumnName] = GetSaleAmount(Convert.ToInt16(sTemp[0]), dFromDate, "GR")[0];
                }

                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] fTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++)
            {
                for (int j = 1; j < dtblTemp.Columns.Count; j++)
                {
                    fTotal[j] = fTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count - 1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = fTotal[k].ToString();
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
                    dtblTemp.Columns[i].ColumnName = sTemp[1].Trim();
                }
            }
            return dtblTemp;
        }

        private DataTable GasReceivedPrice()
        {
            DataTable dtblTemp = new DataTable();
            DataRow drRow;
            SqlCommand cmd;
            SqlDataReader dr;
            dFromDate = Convert.ToDateTime(sFromDate);


            _sql = @"SELECT        MAPPING_STORE_GAS.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME
                        FROM            MAPPING_STORE_GAS INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID
                        WHERE        (MAPPING_STORE_GAS.STORE_ID = " + StoreID + ") AND (MAPPING_STORE_GAS.ACTIVE = 'A')";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            dtblTemp.Columns.Add("Date");
            while (dr.Read())
            {
                dtblTemp.Columns.Add(dr["GASTYPE_ID"].ToString() + "_" + dr["GASTYPE_NAME"].ToString() + "_TEST");
            }
            dtblTemp.Columns.Add("PURCHASE_TOTAL");
            dr.Close();

            float fTotalPurchase = 0;

            while (dFromDate <= dEndDate)
            {
                fTotalPurchase = 0;
                drRow = dtblTemp.NewRow();
                drRow["Date"] = dFromDate.ToShortDateString();
                for (int i = 1; i < dtblTemp.Columns.Count -1; i++)
                {
                    string[] sTemp = dtblTemp.Columns[i].ColumnName.Split('_');
                    drRow[dtblTemp.Columns[i].ColumnName] = GetSaleAmount(Convert.ToInt16(sTemp[0]), dFromDate, "GR")[1];
                    fTotalPurchase += Convert.ToSingle(drRow[dtblTemp.Columns[i].ColumnName]);
                }
                drRow["PURCHASE_TOTAL"] = fTotalPurchase;
                dtblTemp.Rows.Add(drRow);
                dFromDate = dFromDate.AddDays(1);
            }

            float[] fTotal = new float[dtblTemp.Columns.Count];
            for (int i = 0; i < dtblTemp.Rows.Count; i++)
            {
                for (int j = 1; j < dtblTemp.Columns.Count; j++)
                {
                    fTotal[j] = fTotal[j] + Convert.ToSingle(dtblTemp.Rows[i][j]);
                }

                if (i == dtblTemp.Rows.Count - 1) // last row
                {
                    drRow = dtblTemp.NewRow();
                    drRow["Date"] = "Total ";
                    for (int k = 1; k < dtblTemp.Columns.Count; k++)
                    {
                        drRow[dtblTemp.Columns[k].ColumnName] = fTotal[k].ToString();
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
                    dtblTemp.Columns[i].ColumnName = sTemp[1].Trim();
                }
            }
            return dtblTemp;
        }

        private float[] GetSaleAmount(int ID, DateTime Date, string sTranType)
        {
            float[] fAmount = new float[2];

            if (sTranType == "GS") // GAS SALE RECORD
            {
                GasSaleCollection objTranModel = new GasSaleCollection();
                objTranModel = objGasSale.Find(x => x.GasTypeID == ID && x.Date == Date);
                if (objTranModel != null)
                {
                    if (objTranModel.SaleGallons > 0)
                        fAmount[0] = objTranModel.SaleGallons;
                }
            }
            else if (sTranType == "MS") // MASTER SALE RECORD
            {
                GasMasterRecord objMasterRecord = new GasMasterRecord();
                objMasterRecord = objGasMasterRecord.Find(x => x.Date == Date);
                if (objMasterRecord != null)
                {
                    if (objMasterRecord.SaleAmount > 0)
                    {
                        fAmount[0] = objMasterRecord.SaleAmount;
                        fAmount[1] = objMasterRecord.Balance;
                    }
                }
            }
            else if (sTranType == "CA") // CARD AMOUNT
            {
                GasCardCollection objCard = new GasCardCollection();
                objCard = objGasCard.Find(x => x.GasCardID == ID && x.Date == Date);
                if (objCard != null)
                {
                    if (objCard.CardAmount > 0)
                        fAmount[0] = objCard.CardAmount;
                }
            }
            else if (sTranType == "GR") // Gas Received quantity and price
            {
                GasReceived objGasRecv = new GasReceived();
                objGasRecv = objGasReceived.Find(x => x.GasTypeID == ID && x.Date == Date);
                if (objGasRecv != null)
                {
                    if (objGasRecv.GasReceivedQty > 0)
                    {
                        fAmount[0] = objGasRecv.GasReceivedQty;
                        fAmount[1] = objGasRecv.GasReceivedPrice;
                    }
                }
            }

            return fAmount;
        }

        private void FillGasSale()
        {
            GasSaleCollection objTranModel;
            GasMasterRecord objMasterRecord;
            GasCardCollection objCard;
            GasReceived objGasRecv;

            #region Gas Sales
            _sql = @"SELECT       GASTYPE_ID, SALE_DATE, SALE_GALLONS
                        FROM            STORE_GAS_SALE_TRAN
                        WHERE        (STORE_ID = " + StoreID + ") AND (convert(datetime,SALE_DATE,105) >= '" + sFromDate + "') AND ";
            _sql += "(convert(datetime,SALE_DATE,105) <= '" + sEndDate + "')";

            SqlDataReader dr;
            SqlCommand cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objTranModel = new GasSaleCollection();
                objTranModel.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"].ToString());
                objTranModel.SaleGallons = Convert.ToSingle(dr["SALE_GALLONS"].ToString());
                objTranModel.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objGasSale.Add(objTranModel);
            }
            dr.Close();
            #endregion

            #region Gas sales total, card total
            _sql = @"SELECT        STORE_ID, SALE_DATE, SALE_GAS_TOTAL_SALE, SALE_GAS_CARD_TOTAL, SALE_GAS_TOTAL_SALE - SALE_GAS_CARD_TOTAL AS DEPOSIT
                        FROM            STORE_SALE_MASTER
                        WHERE        (STORE_ID = " + StoreID + ") AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND ";
            _sql += "(convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objMasterRecord = new GasMasterRecord();
                objMasterRecord.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objMasterRecord.CardTotal = Convert.ToSingle(dr["SALE_GAS_CARD_TOTAL"].ToString());
                objMasterRecord.SaleAmount = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"].ToString());
                objMasterRecord.Balance = Convert.ToSingle(dr["DEPOSIT"].ToString());
                objGasMasterRecord.Add(objMasterRecord);
            }
            dr.Close();

            #endregion

            #region Card Breakup
            _sql = @"SELECT    STORE_GAS_SALE_CARD_BREAKUP.STORE_ID, STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID, 
                                                    STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT, STORE_SALE_MASTER.SALE_DATE
                        FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                    STORE_SALE_MASTER ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                    STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_SALE_MASTER.STORE_ID = " + StoreID + ") AND (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND ";
            _sql += "(convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objCard = new GasCardCollection();
                objCard.GasCardID = Convert.ToInt16(dr["CARD_TYPE_ID"].ToString());
                objCard.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objCard.CardAmount = Convert.ToSingle(dr["CARD_AMOUNT"].ToString());

                objGasCard.Add(objCard);
            }
            dr.Close();


            #endregion

            #region Gas Received quantity and price
            _sql = @"SELECT  STORE_GAS_SALE_BALANCES.GAS_TYPE_ID, SUM(STORE_GAS_SALE_BALANCES.GAS_RECV) AS GAS_RECV, 
                                                     SUM(STORE_GAS_SALE_BALANCES.GAS_RECV_PRICE) AS GAS_RECV_PRICE, STORE_SALE_MASTER.SALE_DATE
                            FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                     STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                     STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                            WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = MAPSTOREID)
                            GROUP BY STORE_GAS_SALE_BALANCES.GAS_TYPE_ID, STORE_SALE_MASTER.SALE_DATE
                            HAVING        (convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) >= '" + sFromDate + "') AND ";
            _sql += "(convert(datetime,STORE_SALE_MASTER.SALE_DATE,105) <= '" + sEndDate + "')";

            _sql = _sql.Replace("MAPSTOREID",StoreID.ToString());

            cmd = new SqlCommand(_sql, _conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objGasRecv = new GasReceived();
                objGasRecv.Date = Convert.ToDateTime(dr["SALE_DATE"]);
                objGasRecv.GasTypeID = Convert.ToInt16(dr["GAS_TYPE_ID"].ToString());
                objGasRecv.GasReceivedQty = Convert.ToSingle(dr["GAS_RECV"].ToString());
                objGasRecv.GasReceivedPrice = Convert.ToSingle(dr["GAS_RECV_PRICE"].ToString());
                objGasReceived.Add(objGasRecv);
            }
            dr.Close();

            #endregion
        }
    }
}
