using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class LoginDal
    {
        string _sql;
        
        public LoginResult ValidateUser(Login objLogin)
        {
            LoginResult objLoginResult = new LoginResult();
            try
            {
                using (SqlConnection _conn = new SqlConnection(DMLExecute.con))
                {
                    _conn.Open();
                    _sql = @"SELECT        USER_ACCESS_TYPE, GROUP_ID, STORE_ID,USER_ID
                            FROM            USER_LOGIN
                            WHERE     USER_ACTIVE_STATUS = 'A'  AND
                            (USER_ID = '" + objLogin.UserName + "') AND (USER_PWD = '" + objLogin.Password + "')";

                    SqlCommand cmd = new SqlCommand(_sql, _conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        objLoginResult.Status = "VALID";
                        objLoginResult.UserName = dr["USER_ID"].ToString();
                        if (dr["USER_ACCESS_TYPE"].ToString() == "G")
                            objLoginResult.AccessType = "SUPER_USER";
                        else if (dr["USER_ACCESS_TYPE"].ToString() == "S")
                            objLoginResult.AccessType = "STORE_ADMIN";
                        else if (dr["USER_ACCESS_TYPE"].ToString() == "U")
                            objLoginResult.AccessType = "STORE_USER";

                        objLoginResult.GroupID = Convert.ToInt16(dr["GROUP_ID"]);
                        objLoginResult.StoreID = (dr["USER_ACCESS_TYPE"].ToString() == "G") ? 0 : Convert.ToInt16(dr["STORE_ID"]);

                        StoreMasterDal dalStoreMaster = new StoreMasterDal();
                        StoreMaster objStoreMaster = new StoreMaster();
                        objStoreMaster = dalStoreMaster.SelectRecord(objLoginResult.StoreID);
                        objLoginResult.StoreType = objStoreMaster.StoreType;
                        objLoginResult.StoreName = objStoreMaster.StoreName;
                        objLoginResult.StoreAdd1 = objStoreMaster.StoreAddress1;
                        objLoginResult.StoreAdd2 = objStoreMaster.StoreAddress2;

                        objLoginResult.AvailGas = objStoreMaster.AvailGas;
                        objLoginResult.AvailLottery = objStoreMaster.AvailLottery;
                        objLoginResult.AvailBusiness = objStoreMaster.AvailBusiness;

                    }
                    else
                    {
                        objLoginResult.Status = "INVALID";
                    }

                    dr.Close();
                    if (objLoginResult.StoreID > 0)
                        //UpdateLotteryAutoSettle(objLoginResult.StoreID);
                    _conn.Dispose();
                    _conn.Close();
                    
                    SqlConnection.ClearPool(_conn);
                    return objLoginResult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void UpdateLotteryAutoSettle(int iStoreID, DateTime dSettleDate, SqlConnection con, SqlTransaction conTran)
        {
            _sql = "SELECT LOTTERY_AUTO_SETTLE_DAYS FROM STORE_MASTER WHERE STORE_ID = " + iStoreID;
            int autoSettle = 0;
            SqlCommand cmd = new SqlCommand(_sql, con, conTran);
            if (cmd.ExecuteScalar().ToString().Length > 0)
                autoSettle = Convert.ToInt16(cmd.ExecuteScalar());

            _sql = @"UPDATE [STORE_LOTTERY_BOOKS_ACTIVE] SET LTACT_AUTO_SETTLE_STATUS = 'Y', LTACT_AUTO_SETTLE_DATE = 'PARMSETTLEDATE'
                        WHERE DATEDIFF(day,LTACT_DATE,  'PARMSETTLEDATE') >= PARMSETTLEDAYS AND LTACT_STORE_ID = PARMSTOREID AND LTACT_AUTO_SETTLE_STATUS = 'N'";
            _sql = _sql.Replace("PARMSETTLEDAYS", autoSettle.ToString());
            _sql = _sql.Replace("PARMSTOREID", iStoreID.ToString());
            _sql = _sql.Replace("PARMSETTLEDATE", dSettleDate.ToString());

            if (autoSettle > 0)
            {
                cmd = new SqlCommand(_sql, con, conTran);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
