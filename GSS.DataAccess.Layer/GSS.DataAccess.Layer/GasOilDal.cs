using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class GasOilDal : IDal<GasOil>
    {
        SqlConnection _conn;
        SqlTransaction _conTran;
        string _sql;

        public bool AddRecord(GasOil obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                SqlCommand cmd;

                #region Add Gas Oil Types to Store
                _sql = "SELECT COUNT(*) FROM MAPPING_STORE_GAS WHERE STORE_ID = PARMSTOREID AND GASTYPE_ID = PARMGASTYPEID";
                _sql = _sql.Replace("PARMSTOREID", obj.StoreID.ToString());
                _sql = _sql.Replace("PARMGASTYPEID", obj.GasTypeID.ToString());
                cmd = new SqlCommand(_sql, _conn, _conTran);

                if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                {
                    UpdateRecord(obj);
                    return true;
                    //throw new Exception(obj.GasTypeName + " is already added to store " + obj.StoreName);
                }

                _sql = "SELECT COUNT(*) FROM GROUP_GASTYPE_MASTER WHERE GROUP_ID = " + obj.GroupID;
                cmd = new SqlCommand(_sql, _conn, _conTran);
                if (Convert.ToInt16(cmd.ExecuteScalar()) == 0)
                {
                    _sql = @"INSERT INTO GROUP_GASTYPE_MASTER
	                            SELECT     PARMGROUPID, GASTYPE_ID, GASTYPE_NAME, GASTYPE_CONSUMPTION_FORMULA, CREATED_BY, CREATED_TIMESTAMP, MODIFIED_BY, 
							                             MODIFIED_TIMESTAMP
	                            FROM            GROUP_GASTYPE_MASTER
	                            WHERE        (GROUP_ID = 1)";

                    _sql = _sql.Replace("PARMGROUPID", obj.GroupID.ToString());
                    dmlExecute.ExecuteDMLSQL(_sql, _conn, _conTran);
                }

                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("GASTYPE_ID", obj.GasTypeID.ToString());
                if (obj.GasOilConsumption.Count > 0)
                    dmlExecute.AddFields("GASTYPE_CONSUMPTION_FORMULA", CommonFunctions.BuildGasOilFormula(obj.GasOilConsumption));
                else
                    dmlExecute.AddFields("GASTYPE_CONSUMPTION_FORMULA", "(" + obj.GasTypeID.ToString() + "X" + "100)");

                dmlExecute.AddFields("GAS_STOCK_PRICE", obj.StockPrice.ToString());

                dmlExecute.AddFields("CREATED_BY", obj.CreatedUserName);
                dmlExecute.AddFields("CREATED_TIMESTAMP", obj.CreateTimeStamp);
                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                dmlExecute.AddFields("GAS_CAPACITY", obj.GasTankCapacity.ToString());

                dmlExecute.ExecuteInsert("MAPPING_STORE_GAS", _conn, _conTran);
                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Oil already added");
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

        public bool UpdateRecord(GasOil obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Add Gas Oil Types to Store

                dmlExecute.AddFields("STORE_ID", obj.StoreID.ToString());
                dmlExecute.AddFields("GASTYPE_ID", obj.GasTypeID.ToString());
                dmlExecute.AddFields("GAS_STOCK_PRICE", obj.StockPrice.ToString());
                dmlExecute.AddFields("MODIFIED_BY", obj.ModifiedUserName);
                dmlExecute.AddFields("MODIFIED_TIMESTAMP", obj.ModifiedTimeStamp);
                dmlExecute.AddFields("GAS_CAPACITY", obj.GasTankCapacity.ToString());

                string[] _keyfields = { "GASTYPE_ID", "STORE_ID" };

                dmlExecute.ExecuteUpdate("MAPPING_STORE_GAS", _conn,_keyfields, _conTran);
                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Oil already added");
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

        public bool DeleteRecord(int ID)
        {
            throw new NotImplementedException();
        }

        public GasOil SelectRecord(int StoreID, int GasTypeID)
        {
            GasOil objGas = new GasOil();
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   *
                            FROM            MAPPING_STORE_GAS 
                            WHERE        (MAPPING_STORE_GAS.ACTIVE = 'A') AND (MAPPING_STORE_GAS.STORE_ID = " + StoreID + ")  AND (MAPPING_STORE_GAS.GASTYPE_ID = " + GasTypeID + ") ";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                if (dr.Read())
                {
                    objGas.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                    objGas.StockPrice = Convert.ToSingle(dr["GAS_STOCK_PRICE"]);
                    objGas.GasOilFormula = dr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                    objGas.GasTankCapacity = Convert.ToSingle(dr["GAS_CAPACITY"]);
                    objGas.GasOilConsumption = CommonFunctions.ParseGasOilFormula(dr["GASTYPE_CONSUMPTION_FORMULA"].ToString());
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _conn.Close();
            }
            return objGas;

        }

        public GasOil SelectRecord(int ID)
        {
            throw new NotImplementedException(); 
        }

        public List<GasOil> SelectGasOilPerGroupByUserName(string sUserName)
        {
            try
            {
                List<GasOil> objGasColl = new List<GasOil>();
                GasOil objGas;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT  GROUP_GASTYPE_MASTER.GROUP_ID, GROUP_GASTYPE_MASTER.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME, 
                            GROUP_GASTYPE_MASTER.GASTYPE_CONSUMPTION_FORMULA
                            FROM            GROUP_GASTYPE_MASTER INNER JOIN
                            USER_LOGIN ON GROUP_GASTYPE_MASTER.GROUP_ID = USER_LOGIN.GROUP_ID
                            WHERE        (USER_LOGIN.USER_ACCESS_TYPE = 0) AND 
                            (USER_LOGIN.USER_ACTIVE_STATUS = 'A') AND (USER_LOGIN.USER_ID = N'" + sUserName + "') ORDER BY GROUP_GASTYPE_MASTER.GASTYPE_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objGas = new GasOil();
                    objGas.GroupID = Convert.ToInt16(dr["GROUP_ID"]);
                    objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                    objGas.GasTypeName = dr["GASTYPE_NAME"].ToString();
                    objGas.GasOilFormula = dr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                    objGas.GasOilConsumption = CommonFunctions.ParseGasOilFormula(dr["GASTYPE_CONSUMPTION_FORMULA"].ToString());
                    
                    objGasColl.Add(objGas);
                }

                dr.Close();
                return objGasColl;
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

        public List<GasOil> SelectGasOilPerGroupByGroupID(int iGroupID)
        {
            try
            {
                List<GasOil> objGasColl = new List<GasOil>();
                GasOil objGas ;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   GROUP_ID, GASTYPE_ID, GASTYPE_NAME, GASTYPE_CONSUMPTION_FORMULA
                            FROM            GROUP_GASTYPE_MASTER
                            WHERE        (GROUP_ID = " + iGroupID + ") ORDER BY GASTYPE_NAME";
                            

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objGas = new GasOil();
                    objGas.GroupID = Convert.ToInt16(dr["GROUP_ID"]);
                    objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                    objGas.GasTypeName = dr["GASTYPE_NAME"].ToString();
                    objGas.GasOilFormula = dr["GASTYPE_CONSUMPTION_FORMULA"].ToString();

                    objGas.GasOilConsumption = CommonFunctions.ParseGasOilFormula(dr["GASTYPE_CONSUMPTION_FORMULA"].ToString());
                    objGasColl.Add(objGas);
                }

                dr.Close();
                return objGasColl;
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

        public List<GasOil> SelectAllGasOil()
        {
            try
            {
                int iGroupID = 1;
                List<GasOil> objGasColl = new List<GasOil>();
                GasOil objGas;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT   GASTYPE_ID, GASTYPE_NAME
                            FROM            GROUP_GASTYPE_MASTER
                            WHERE        (GROUP_ID = " + iGroupID + ") ORDER BY GASTYPE_ID";


                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objGas = new GasOil();
                    objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                    objGas.GasTypeName = dr["GASTYPE_NAME"].ToString();
                    objGasColl.Add(objGas);
                }

                dr.Close();
                return objGasColl;
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
        
        public List<GasOil> SelectRecords(int ID)
        {
            try
            {
                List<GasOil> objGasColl = new List<GasOil>();
                GasOil objGas;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT        MAPPING_STORE_GAS.STORE_ID, MAPPING_STORE_GAS.GASTYPE_ID, MAPPING_STORE_GAS.GASTYPE_CONSUMPTION_FORMULA, 
                             MAPPING_STORE_GAS.GAS_STOCK_PRICE, GROUP_GASTYPE_MASTER.GASTYPE_NAME, GROUP_GASTYPE_MASTER.GROUP_ID, GAS_CAPACITY
                        FROM            MAPPING_STORE_GAS INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                                 STORE_MASTER ON MAPPING_STORE_GAS.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                 GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                        WHERE        (MAPPING_STORE_GAS.ACTIVE = 'A') AND (MAPPING_STORE_GAS.STORE_ID = " + ID + ") AND MAPPING_STORE_GAS.GASTYPE_ID <> 7 ORDER BY GROUP_GASTYPE_MASTER.GASTYPE_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objGas = new GasOil();
                    objGas.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objGas.GroupID = Convert.ToInt16(dr["GROUP_ID"]);
                    objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                    objGas.GasTypeName = dr["GASTYPE_NAME"].ToString();
                    objGas.StockPrice = Convert.ToSingle(dr["GAS_STOCK_PRICE"]);
                    objGas.GasOilFormula = dr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                    objGas.StockPrice = Convert.ToSingle(dr["GAS_CAPACITY"]);
                    objGas.GasOilConsumption = CommonFunctions.ParseGasOilFormula(dr["GASTYPE_CONSUMPTION_FORMULA"].ToString());
                    objGasColl.Add(objGas);
                }

                dr.Close();
                return objGasColl;
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

        public List<GasOil> SelectRecordsExcludeOil(int iStoreID)
        {
            try
            {
                List<GasOil> objGasColl = new List<GasOil>();
                GasOil objGas;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT        MAPPING_STORE_GAS.STORE_ID, MAPPING_STORE_GAS.GASTYPE_ID, MAPPING_STORE_GAS.GASTYPE_CONSUMPTION_FORMULA, 
                             MAPPING_STORE_GAS.GAS_STOCK_PRICE, GROUP_GASTYPE_MASTER.GASTYPE_NAME, GROUP_GASTYPE_MASTER.GROUP_ID, GAS_CAPACITY
                        FROM            MAPPING_STORE_GAS INNER JOIN
                                                 GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                                 STORE_MASTER ON MAPPING_STORE_GAS.STORE_ID = STORE_MASTER.STORE_ID AND 
                                                 GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                        WHERE        (MAPPING_STORE_GAS.ACTIVE = 'A') AND (MAPPING_STORE_GAS.STORE_ID = " + iStoreID + ") AND MAPPING_STORE_GAS.GASTYPE_ID <> 7 ORDER BY GROUP_GASTYPE_MASTER.GASTYPE_NAME";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    if (Convert.ToInt16(dr["GASTYPE_ID"]) != 7)
                    {
                        objGas = new GasOil();
                        objGas.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                        objGas.GroupID = Convert.ToInt16(dr["GROUP_ID"]);
                        objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                        objGas.GasTypeName = dr["GASTYPE_NAME"].ToString();
                        objGas.StockPrice = Convert.ToSingle(dr["GAS_STOCK_PRICE"]);
                        objGas.GasOilFormula = dr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                        objGas.GasTankCapacity = Convert.ToSingle(dr["GAS_CAPACITY"]);

                        objGas.GasOilConsumption = CommonFunctions.ParseGasOilFormula(dr["GASTYPE_CONSUMPTION_FORMULA"].ToString());
                        objGasColl.Add(objGas);
                    }
                }

                dr.Close();
                return objGasColl;
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

        public List<GasOil> GetGroupGasType(int ID)
        {
            try
            {
                List<GasOil> objGasColl = new List<GasOil>();
                GasOil objGas;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT        MAPPING_STORE_GAS.STORE_ID, MAPPING_STORE_GAS.GASTYPE_ID, MAPPING_STORE_GAS.GASTYPE_CONSUMPTION_FORMULA, 
                                                     MAPPING_STORE_GAS.GAS_STOCK_PRICE, GROUP_GASTYPE_MASTER.GASTYPE_NAME, GROUP_GASTYPE_MASTER.GROUP_ID,
                                                    STORE_MASTER.STORE_NAME, GAS_CAPACITY
                            FROM            MAPPING_STORE_GAS INNER JOIN
                                                     GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID INNER JOIN
                                                     STORE_MASTER ON STORE_MASTER.STORE_ID = MAPPING_STORE_GAS.STORE_ID AND 
                                                     GROUP_GASTYPE_MASTER.GROUP_ID = STORE_MASTER.STORE_GROUP_ID
                            WHERE        (MAPPING_STORE_GAS.ACTIVE = 'A') AND (STORE_MASTER.STORE_GROUP_ID = " + ID + ") ORDER BY GROUP_GASTYPE_MASTER.GASTYPE_NAME";


                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objGas = new GasOil();
                    objGas.StoreID = Convert.ToInt16(dr["STORE_ID"]);
                    objGas.GroupID = Convert.ToInt16(dr["GROUP_ID"]);
                    objGas.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"]);
                    objGas.GasTypeName = dr["GASTYPE_NAME"].ToString();
                    objGas.StoreName = dr["STORE_NAME"].ToString();
                    objGas.StockPrice = Convert.ToSingle(dr["GAS_STOCK_PRICE"]);
                    objGas.GasOilFormula = dr["GASTYPE_CONSUMPTION_FORMULA"].ToString();
                    objGas.GasTankCapacity = Convert.ToSingle(dr["GAS_CAPACITY"]);

                    objGas.GasOilConsumption = CommonFunctions.ParseGasOilFormula(dr["GASTYPE_CONSUMPTION_FORMULA"].ToString());
                    objGasColl.Add(objGas);
                }

                dr.Close();

                foreach (GasOil g in objGasColl)
                {

                }

                return objGasColl;
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
            throw new NotImplementedException();
        }

        public List<GasOpeningBalance> GetGasOpeningBalance(int iStoreID, int iTankID, DateTime dDate)
        {
            List<GasOpeningBalance> objlstGasOpeningBalance = new List<GasOpeningBalance>();
            GasOpeningBalance objGasOpeningBalance ;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                #region Read Gas types and add to collection
                _sql = @"SELECT        MAPPING_STORE_GAS.STORE_ID, MAPPING_STORE_GAS.GASTYPE_ID, GROUP_GASTYPE_MASTER.GASTYPE_NAME,
                        GAS_CAPACITY
                            FROM            MAPPING_STORE_GAS INNER JOIN
                                STORE_MASTER ON MAPPING_STORE_GAS.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                GROUP_GASTYPE_MASTER ON MAPPING_STORE_GAS.GASTYPE_ID = GROUP_GASTYPE_MASTER.GASTYPE_ID AND 
                                STORE_MASTER.STORE_GROUP_ID = GROUP_GASTYPE_MASTER.GROUP_ID
                            WHERE        (MAPPING_STORE_GAS.STORE_ID = " + iStoreID + ") ORDER BY GASTYPE_NAME";

                SqlCommand cmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    objGasOpeningBalance = new GasOpeningBalance();
                    objGasOpeningBalance.StoreID = Convert.ToInt16(dr["STORE_ID"].ToString());
                    objGasOpeningBalance.GasTypeID = Convert.ToInt16(dr["GASTYPE_ID"].ToString());
                    objGasOpeningBalance.GasTypeName = dr["GASTYPE_NAME"].ToString();
                    objGasOpeningBalance.TankCapacity = Convert.ToSingle(dr["GAS_CAPACITY"].ToString());
                    objGasOpeningBalance.TankID = iTankID;
                    objGasOpeningBalance.Date = dDate;
                    
                    //objGasOpeningBalance.OpeningBalance = GetOpeningBalance(iStoreID,iTankID,dDate,Convert.ToInt16(dr["GASTYPE_ID"].ToString()));
                    objlstGasOpeningBalance.Add(objGasOpeningBalance);
                }
                dr.Close();

                foreach (GasOpeningBalance obj in objlstGasOpeningBalance)
                {
                    obj.OpeningBalance = GetOpeningBalance(obj.StoreID, obj.TankID, obj.Date, obj.GasTypeID);
                }

                #endregion

                return objlstGasOpeningBalance;
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

        private float GetOpeningBalance(int iStoreID, int iTankID, DateTime dDate, int iGasTypeID)
        {
            _sql = @"SELECT        TOP (1) STORE_GAS_SALE_BALANCES.GAS_SYSTEM_CL_BAL as GAS_ACT_CL_BAL
                        FROM            STORE_GAS_SALE_BALANCES INNER JOIN
                                                 STORE_SALE_MASTER ON STORE_GAS_SALE_BALANCES.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                 STORE_GAS_SALE_BALANCES.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                        WHERE        (STORE_GAS_SALE_BALANCES.STORE_ID = PARMSTORE) AND (STORE_GAS_SALE_BALANCES.STORE_GAS_TANK_ID = PARMTANKID) AND 
                                                 (STORE_GAS_SALE_BALANCES.GAS_TYPE_ID = PARMGASTYPEID) AND (STORE_SALE_MASTER.SALE_DATE < 'PARMDATE') 
                        ORDER BY STORE_SALE_MASTER.SALE_DATE DESC";

            // I modified SALE_DATE <= to < to calculate opening balance on 10/3

            _sql = _sql.Replace("PARMSTORE",iStoreID.ToString());
            _sql = _sql.Replace("PARMTANKID", iTankID.ToString());
            _sql = _sql.Replace("PARMGASTYPEID",iGasTypeID.ToString());
            _sql = _sql.Replace("PARMDATE",dDate.ToString());

             SqlCommand cmd = new SqlCommand(_sql, _conn);
            SqlDataReader dr = cmd.ExecuteReader();
            float fBalance = 0;

            if (dr.Read())
                fBalance = Convert.ToInt64(dr["GAS_ACT_CL_BAL"]);
            dr.Close();
            return fBalance;
        }

        public List<GasSystemClosingBalance> GetGasSystemClosingBalance(List<GasSystemClosingBalance> objGasClBalColl)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                SqlCommand cmd ;
                List<GasSystemClosingBalance> objCollTemp = new List<GasSystemClosingBalance>();

                #region Update consumption formula

                foreach (GasSystemClosingBalance obj in objGasClBalColl)
                {
                    _sql = @"SELECT        GASTYPE_CONSUMPTION_FORMULA
                            FROM            MAPPING_STORE_GAS  
                            WHERE        MAPPING_STORE_GAS.STORE_ID = " + obj.StoreID + " AND GASTYPE_ID = " + obj.GasTypeID;
                    
                    cmd = new SqlCommand(_sql, _conn);
                    obj.ConsumedPercent = CommonFunctions.ParseGasOilFormula(cmd.ExecuteScalar().ToString());

                    obj.SystemClosingBalance = obj.GasOpeningBalance + obj.Received;
                    objCollTemp.Add(obj);
                }
                #endregion

                #region Updating system closing balance
                foreach (GasSystemClosingBalance obj in objCollTemp)
                {
                    GasSystemClosingBalance objTemp = new GasSystemClosingBalance();
                    foreach (GasOilFormula objGasFormula in obj.ConsumedPercent)
                    {
                            foreach (GasSystemClosingBalance _obj in objGasClBalColl)
                            {
                                if (objGasFormula.GasTypeID == _obj.GasTypeID)
                                {
                                    objTemp = _obj;
                                    break;
                                }
                        }

                        float fClBal = objTemp.SystemClosingBalance;

                        if (obj.SaleQty > 0)
                            fClBal = fClBal - ((obj.SaleQty * objGasFormula.GasOilConsmptPercent) / 100);
                        objTemp.SystemClosingBalance = fClBal;
                        objTemp.ShortOrExcess = objTemp.ActualClosingBalance - objTemp.SystemClosingBalance;
                    }
                }
                #endregion

                return objGasClBalColl;
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

        public LotteryRunningShift GetRunningShift(int ID)
        {
            try
            {

                LotteryRunningShift objShift;
                NextShift nextShift = new NextShift();
                SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

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

                        _sql = @"SELECT   SALE_GAS_TOTAL_SALE
                                    FROM            STORE_SALE_MASTER
                                    WHERE        (STORE_ID = PARMSTOREID) AND (SALE_DATE = 'PARMDATE') AND (SALE_SHIFT_CODE = PARMSHIFTCODE)";
                        
                        _sql = _sql.Replace("PARMSTOREID",ID.ToString());
                        _sql = _sql.Replace("PARMDATE",nextShift.NextDate.ToString());
                        _sql = _sql.Replace("PARMSHIFTCODE",nextShift.NextShiftID.ToString());
                        sqlcmd = new SqlCommand(_sql, _conn);
                        dr = sqlcmd.ExecuteReader();
                        if (dr.Read())
                        {
                            objShift.SystemOpeningBalance = Convert.ToSingle(dr["SALE_GAS_TOTAL_SALE"]);
                        }
                        dr.Close();
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

        public List<ReconcillationModel> ReconcillationDetails(int StoreID, int TransactionType)
        {
            try
            {
                List<ReconcillationModel> objRecDetails = new List<ReconcillationModel>();
                ReconcillationModel objRec;

                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();

                _sql = @"SELECT        STORE_RECONCILLATION_ENTRIES.RECON_REF_NO, STORE_RECONCILLATION_ENTRIES.RECON_REF_DATE, 
                                                     STORE_RECONCILLATION_ENTRIES.RECON_AMOUNT, GLOBAL_GAS_TRAN_TYPES.GTP_TRAN_DESC
                            FROM            STORE_RECONCILLATION_ENTRIES INNER JOIN
                                                     GLOBAL_GAS_TRAN_TYPES ON STORE_RECONCILLATION_ENTRIES.RECON_TYPE_ID = GLOBAL_GAS_TRAN_TYPES.GTP_TRAN_ID
                            WHERE        (STORE_RECONCILLATION_ENTRIES.RECON_STORE_ID = PARMSTOREID) AND ";

                if (TransactionType != 0)
                {
                    _sql += "(STORE_RECONCILLATION_ENTRIES.RECON_TYPE_ID = PARMTYPEID) AND " ;
                }

                _sql += "(STORE_RECONCILLATION_ENTRIES.RECON_STATUS = 'N') ORDER BY STORE_RECONCILLATION_ENTRIES.RECON_REF_DATE";

                _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                _sql = _sql.Replace("PARMTYPEID", TransactionType.ToString());


                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);
                SqlDataReader dr = sqlcmd.ExecuteReader();

                while (dr.Read())
                {
                    objRec = new ReconcillationModel();
                    objRec.DealerRefNo = dr["RECON_REF_NO"].ToString();
                    objRec.TransactionType = dr["GTP_TRAN_DESC"].ToString();
                    objRec.DealerDate = Convert.ToDateTime(dr["RECON_REF_DATE"].ToString());
                    objRec.DealerAmount = Convert.ToSingle(dr["RECON_AMOUNT"].ToString());
                    objRecDetails.Add(objRec);
                }
                dr.Close();

                foreach (ReconcillationModel o in objRecDetails)
                {
                    if ((TransactionType == 3) || (TransactionType == 0)) // Getting Invoice details
                    {
                        _sql = @"SELECT   GR_INV_NO, GR_INV_DATE, GR_INV_AMOUNT
                                    FROM            GAS_RECEIPT_MASTER
                                    WHERE        (GR_STORE_ID = PARMSTOREID) AND (GR_INV_NO = 'PARMREFNO')";

                        _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                        _sql = _sql.Replace("PARMREFNO", o.DealerRefNo);

                        sqlcmd = new SqlCommand(_sql, _conn);
                        dr = sqlcmd.ExecuteReader();
                        if (dr.Read())
                        {
                            o.StoreRefNo = dr["GR_INV_NO"].ToString();
                            o.StoreDate = Convert.ToDateTime(dr["GR_INV_DATE"].ToString());
                            o.StoreAmount = Convert.ToSingle(dr["GR_INV_AMOUNT"].ToString());
                        }
                        else
                        {
                            o.StoreRefNo = "Not Found";
                        }
                        dr.Close();
                    }
                    
                    if ((TransactionType == 1) || (TransactionType == 0))  // Getting Transaction Details
                    {
                        _sql = @"SELECT STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID, SUM(STORE_GAS_SALE_CARD_BREAKUP.CARD_AMOUNT) AS CARD_AMOUNT
                                    FROM            STORE_GAS_SALE_CARD_BREAKUP INNER JOIN
                                                             MAPPING_STORE_CARD ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = MAPPING_STORE_CARD.STORE_ID AND 
                                                             STORE_GAS_SALE_CARD_BREAKUP.CARD_TYPE_ID = MAPPING_STORE_CARD.CARD_TYPE_ID INNER JOIN
                                                             STORE_SALE_MASTER ON STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = STORE_SALE_MASTER.STORE_ID AND 
                                                             STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID = STORE_SALE_MASTER.SALE_DAY_TRAN_ID
                                    WHERE        (STORE_GAS_SALE_CARD_BREAKUP.STORE_ID = PARMSTOREID) AND (MAPPING_STORE_CARD.CARD_CREDIT_TYPE = 'G') AND 
                                                             (STORE_SALE_MASTER.SALE_DATE = 'PARMDATE')
                                    GROUP BY STORE_GAS_SALE_CARD_BREAKUP.SALE_DAY_TRAN_ID";

                        _sql = _sql.Replace("PARMSTOREID", StoreID.ToString());
                        _sql = _sql.Replace("PARMDATE", o.DealerDate.ToString());

                        sqlcmd = new SqlCommand(_sql, _conn);
                        dr = sqlcmd.ExecuteReader();
                        if (dr.Read())
                        {
                            o.StoreRefNo = dr["SALE_DAY_TRAN_ID"].ToString();
                            o.StoreDate = o.DealerDate;
                            o.StoreAmount = Convert.ToSingle(dr["CARD_AMOUNT"].ToString());
                        }
                        else
                        {
                            o.StoreRefNo = "Not Found";
                        }
                        dr.Close();
                    }
                }

                return objRecDetails;
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

        public bool UpdateReconcillation(UpdateReconcillation obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();

                #region Update Reconcillation

                foreach (UpdateReconcillationTran refNo in obj.Reference)
                {
                    dmlExecute.AddFields("RECON_STATUS", "Y");
                    dmlExecute.AddFields("RECON_DONE_BY", obj.ModifiedUserName);
                    dmlExecute.AddFields("RECON_DONE_DATE", obj.ModifiedTimeStamp);

                    dmlExecute.AddFields("RECON_STORE_ID", obj.StoreID.ToString());
                    dmlExecute.AddFields("RECON_TYPE_ID", obj.TransactionType.ToString());
                    dmlExecute.AddFields("RECON_REF_NO", refNo.ReferenceNo);


                    string[] _keyfields = { "RECON_STORE_ID", "RECON_TYPE_ID", "RECON_REF_NO" };

                    dmlExecute.ExecuteUpdate("STORE_RECONCILLATION_ENTRIES", _conn, _keyfields, _conTran);
                }

                #endregion

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
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
