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
    public class ReportDashBoard
    {
        SqlConnection _conn;
        private int StoreID;
        string MonthName;
        private int Year;
        string sFromDate;
        string sEndDate;
        DateTime dFromDate;
        DateTime dEndDate;

        public GroupDashBoardModel GetGroupSaleDashboard()
        {
            GroupDashBoardModel objGroupModel = new GroupDashBoardModel();
            StoreDashBoardModel objStoreModel ;
            List<StoreDashBoardModel> objStoreModels = new List<StoreDashBoardModel>();

            List<StoreMaster> objStore = new List<StoreMaster>();
            StoreMasterDal dalStoreMaster = new StoreMasterDal();
            try
            {
                objStore = dalStoreMaster.SelectRecords(1);  // Hardcode group id to 1.  Expecting one database can have one group only with multiple store.  This helps on security also.
                foreach (StoreMaster obj in objStore)
                {
                    objStoreModel = new StoreDashBoardModel();
                    
                    objStoreModel = GetStoreSaleDashboard(obj.StoreID);
                    objStoreModel.StoreID = obj.StoreID;
                    objStoreModel.StoreName = obj.StoreName;
                    objStoreModels.Add(objStoreModel);
                }
                
                objGroupModel.GroupDashboard = objStoreModels;
            }
            catch(Exception ex)
            { 
                throw ex;
            }
            return objGroupModel;
        }


        public StoreDashBoardModel GetStoreSaleDashboard(int iStoreID)
        {
            StoreDashBoardModel objSales = new StoreDashBoardModel();
            
            List<GraphData> objGraphDataGasColl = new List<GraphData>();
            List<GraphData> objGraphDataLotteryColl = new List<GraphData>();
            List<GraphData> objGraphDataBusinessColl = new List<GraphData>();
            List<GraphData> objGraphDataMoneyOrderColl = new List<GraphData>();

            GraphData objGraphDataGas ;
            GraphData objGraphDataLottery ;
            GraphData objGraphDataBusiness ;
            GraphData objGraphDataMoneyOrder ;


            try
            {
                _conn = new SqlConnection(DMLExecute.con);
                _conn.Open();
                StoreID = iStoreID;
                MonthName = DateTime.Now.ToString("MMM");
                Year = DateTime.Now.Year;

                sFromDate = "01-" + MonthName + "-" + Year.ToString();
                dFromDate = Convert.ToDateTime(sFromDate);
                dEndDate = dFromDate.AddMonths(1).AddDays(-1);
                sEndDate = String.Format("{0:dd-MMM-yyyy}", dEndDate);
                BankDepositForm _bankDepositForm;
                SaleSupportEntries _SaleSupportEntries = new SaleSupportEntries();

                while (dFromDate <= dEndDate)
                {
                    objGraphDataBusiness = new GraphData();
                    objGraphDataBusiness.Date = dFromDate.ToShortDateString();
                    objGraphDataLottery = new GraphData();
                    objGraphDataLottery.Date = dFromDate.ToShortDateString();
                    objGraphDataGas = new GraphData();
                    objGraphDataGas.Date = dFromDate.ToShortDateString();
                    objGraphDataMoneyOrder = new GraphData();
                    objGraphDataMoneyOrder.Date = dFromDate.ToShortDateString();

                    _bankDepositForm = _SaleSupportEntries.BankDepositDetails(iStoreID, dFromDate, 1);
                    foreach(BankDeposit obj in _bankDepositForm.LedgerDetail)
                    {
                        if (obj.LedgerID == 9) // Business Sale
                            objGraphDataBusiness.Sale = obj.LedgerSale;
                        else if (obj.LedgerID == 5) // Gas Sale
                            objGraphDataGas.Sale = obj.LedgerSale;
                        else if (obj.LedgerID == 4) // Lottery Sale
                            objGraphDataLottery.Sale = obj.LedgerSale;
                        else if (obj.LedgerID == 34) // Money Order Sale
                            objGraphDataMoneyOrder.Sale = obj.LedgerSale;
                    }

                    objGraphDataBusinessColl.Add(objGraphDataBusiness);
                    objGraphDataGasColl.Add(objGraphDataGas);
                    objGraphDataLotteryColl.Add(objGraphDataLottery);
                    objGraphDataMoneyOrderColl.Add(objGraphDataMoneyOrder);
                    dFromDate = dFromDate.AddDays(1);
                }

                objSales.BusinessSale = objGraphDataBusinessColl;
                objSales.LotterySale = objGraphDataLotteryColl;
                objSales.GasSale = objGraphDataGasColl;
                objSales.MoneyOrderSale = objGraphDataMoneyOrderColl;

                return objSales;
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
