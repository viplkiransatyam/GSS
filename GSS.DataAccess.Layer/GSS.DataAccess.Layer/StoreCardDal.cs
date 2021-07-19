using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSS.Data.Model;
using System.Data.SqlClient;

namespace GSS.DataAccess.Layer
{
    public class StoreCardDal : IDal<StoreCard>
    {
        SqlConnection _conn;
        SqlTransaction _conTran;

        public bool AddRecord(List<StoreCard> obj)
        {
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                _conTran = _conn.BeginTransaction();
                DMLExecute dmlExecute = new DMLExecute();
                string _sql = string.Empty;
                SqlCommand cmd;

                foreach (StoreCard o in obj)
                {
                    _sql = "SELECT COUNT(*) FROM MAPPING_STORE_CARD WHERE STORE_ID = PARMSTOREID AND CARD_TYPE_ID = PARMCARDID";
                    _sql = _sql.Replace("PARMSTOREID", o.StoreID.ToString());
                    _sql = _sql.Replace("PARMCARDID", o.CardType.ToString());
                    cmd = new SqlCommand(_sql, _conn, _conTran);

                    if (Convert.ToInt16(cmd.ExecuteScalar()) > 0)
                    {
                        throw new Exception(o.CardName + " is already added to the store");
                    }

                    #region Add Store Cards

                    dmlExecute.AddFields("STORE_ID", o.StoreID.ToString());
                    dmlExecute.AddFields("CARD_TYPE_ID", o.CardType.ToString());
                    dmlExecute.AddFields("ACTIVE", "A");
                    dmlExecute.AddFields("CARD_CREDIT_TYPE", o.CardCreditType.ToString());
                    dmlExecute.AddFields("CREATED_BY", o.CreatedUserName);
                    dmlExecute.AddFields("CREATED_TIMESTAMP", o.CreateTimeStamp);
                    dmlExecute.AddFields("MODIFIED_BY", o.ModifiedUserName);
                    dmlExecute.AddFields("MODIFIED_TIMESTAMP", o.ModifiedTimeStamp);

                    dmlExecute.ExecuteInsert("MAPPING_STORE_CARD", _conn, _conTran);
                    #endregion
                }

                _conTran.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // <-- but this will
                    throw new Exception("Card already exists");
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

        public bool AddRecord(StoreCard obj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRecord(StoreCard obj)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRecord(int ID)
        {
            throw new NotImplementedException();
        }

        public StoreCard SelectRecord(int ID)
        {
            throw new NotImplementedException();
        }

        public List<StoreCard> SelectRecords(int ID)
        {
            List<StoreCard> objColl = new List<StoreCard>();
            StoreCard _storeCard;

            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                string _sql = string.Empty;

                _sql = @"SELECT   MAPPING_STORE_CARD.CARD_TYPE_ID, GROUP_CARD_TYPE.GROU_CARD_NAME
                            FROM            MAPPING_STORE_CARD INNER JOIN
                                                     STORE_MASTER ON MAPPING_STORE_CARD.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_CARD_TYPE ON MAPPING_STORE_CARD.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID AND 
                                                     STORE_MASTER.STORE_GROUP_ID = GROUP_CARD_TYPE.GAS_GROUP_ID
                            WHERE        (MAPPING_STORE_CARD.STORE_ID = PARMSTOREID) AND (MAPPING_STORE_CARD.ACTIVE = 'A')
                            ORDER BY GROUP_CARD_TYPE.GROU_CARD_NAME";

                _sql = _sql.Replace("PARMSTOREID", ID.ToString());
                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _storeCard = new StoreCard();
                    _storeCard.CardType = (int)dr["CARD_TYPE_ID"];
                    _storeCard.CardName = dr["CARD_TYPE_ID"].ToString();
                    objColl.Add(_storeCard);
                }
                dr.Close();
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

        public List<ReportStoreCard> SelectGroupCardTypes(int iGroupID)
        {
            List<ReportStoreCard> objStoreCards = new List<ReportStoreCard>();
            ReportStoreCard _storeCard;
            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                string _sql = string.Empty;

                _sql = @"SELECT    STORE_MASTER.STORE_ID, STORE_MASTER.STORE_NAME, MAPPING_STORE_CARD.CARD_TYPE_ID, GROUP_CARD_TYPE.GROU_CARD_NAME, 
                             CASE MAPPING_STORE_CARD.CARD_CREDIT_TYPE WHEN 'G' THEN 'GAS DEALER A/C' ELSE 'VENDOR A/C' END AS CREDITED_TO
                            FROM            MAPPING_STORE_CARD INNER JOIN
                                                     STORE_MASTER ON MAPPING_STORE_CARD.STORE_ID = STORE_MASTER.STORE_ID INNER JOIN
                                                     GROUP_CARD_TYPE ON MAPPING_STORE_CARD.CARD_TYPE_ID = GROUP_CARD_TYPE.GAS_CARD_TYPE_ID AND 
                                                     STORE_MASTER.STORE_GROUP_ID = GROUP_CARD_TYPE.GAS_GROUP_ID
                            WHERE        (STORE_MASTER.STORE_GROUP_ID = " + iGroupID + ")";

                SqlCommand sqlcmd = new SqlCommand(_sql, _conn);

                SqlDataReader dr = sqlcmd.ExecuteReader();
                while (dr.Read())
                {
                    _storeCard = new ReportStoreCard();
                    _storeCard.StoreID = (int)dr["STORE_ID"];
                    _storeCard.StoreName = dr["STORE_NAME"].ToString();
                    _storeCard.CardType = (int) dr["CARD_TYPE_ID"];
                    _storeCard.CardName = dr["GROU_CARD_NAME"].ToString();
                    _storeCard.CardCreditType = dr["CREDITED_TO"].ToString();

                    objStoreCards.Add(_storeCard);
                }
                dr.Close();
                return objStoreCards;
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
    }
}
