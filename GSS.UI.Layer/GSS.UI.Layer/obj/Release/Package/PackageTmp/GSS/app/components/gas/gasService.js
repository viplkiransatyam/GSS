angular.module('GasApp').factory('gasService', function ($http, $state, $cookies, $location) {
    const getShiftsPerStoreURL = "/Api/StoreMaster/GetShifts";
    const getStorebasedGas = "/Api/GasOil/GetGasType";
    const getStoreName = "/Api/StoreMaster/GetStoreMaster";
    const getGasbalance = "/Api/GasOil/GetGasOpeningBalance";
    const getCardperStore = "/Api/GroupMaster/GetCardPerStore";
    const PostGasStationURL = "/Api/Sale/SaveGasSales";
    const PostGasInventoryURL = "/Api/Sale/SaveGasInventory";
    const PostGasCardsURL = "/Api/Sale/SaveGasCardBreakup";
    const PostDeliveryURL = "/Api/Sale/SaveGasReceipt";
    const GetDaySaleTransURL = "/Api/Sale/GetDaySale";
    const GetPurchasesURL = "/Api/GasPurchase/GetGasPurchase";
    const SavePurchasesURL = "/Api/GasPurchase/SavePurchase";
    const getTransactionTypesURL = "/Api/GasPurchase/GetTranTypes";
    const GetReceiptsURL = "/Api/GasPurchase/GetGasStatement";
    const SaveReceiptsURL = "/Api/GasPurchase/AddOrUpdateGasDealerStatement";
    const getGasSaleReportURL = "/Api/GasOil/GetSaleReportIndividual";
    const GasMonthlyStatementURL = "/Api/GasOil/GetSaleReportMonth";
    const GetReconcillationReportURL = "/Api/GasOil/GetReconcillationStatement"
    const GetGasLedger = "/Api/Report/GetLedger";
    const GetGasAccount = "/Api/AccoutMaster/GetGasAccounts";
    const GetGasTransAmount = "/Api/GasPurchase/GetTranAmount";
    const GetPurchaseRegisterURL = "/Api/GasPurchase/GetPurchaseRegister";
    const GetRunningShiftURL = "/Api/GasOil/GetRunningShift";
    const GetSaleTrendURL = "/Api/Report/GetGasSaleTrend";
    const GetSaleReportMonthURL = "/Api/Report/GetGasMonthlySaleAbstractReport";
    const GetSaleReportIndividualURL = "/Api/GasOil/GetSaleReportIndividual";
    const GetStockInwardReportURL = "/Api/GasPurchase/GetStockInward";
    const GetReconcillationDetailsURL = "/Api/GasPurchase/ReconcillationDetails";
    const UpdateReconcillationURL = "/Api/GasPurchase/UpdateReconcillation";
    const ShifitendURL = "/Api/GasOil/DayEndReport";
    const ProfitLossURL = "/Api/Report/GetGasProfitLoss";
    const getGasTypeExcludeDuplicate = "/Api/GasOil/GetGasTypeExcludeDuplicate";

    var gasService = [];
    //Get Shifts
    gasService.getRunningShift = function (storeID) {
        return $http({
            method: 'GET',
            url: GetRunningShiftURL + '/' + storeID,
        });
    }

    //Get Shifts
    gasService.getShifts = function (storeID) {
        return $http({
            method: 'GET',
            url: getShiftsPerStoreURL + '/' + storeID,
        });
    }
    //Get Gas's based on store
    gasService.getStorebasedGas = function (storeID) {
        return $http({
            method: 'GET',
            url: getStorebasedGas + '/' + storeID,
        });
    }

    //Get Gas's based on store with out duplicates
    gasService.getGasTypeExcludeDuplicate = function (storeID) {
        return $http({
            method: 'GET',
            url: getGasTypeExcludeDuplicate + '/' + storeID,
        });
    }
    //Get StoreName
    gasService.getStoreName = function (storeID) {
        return $http({
            method: 'GET',
            url: getStoreName + '/' + storeID,
        });
    }
    //Get Gasopening Balance for the store
    gasService.getGasopeningBalance = function (gasData) {
        return $http({
            method: 'POST',
            url: getGasbalance,
            data: gasData
        });
    }
    //Get Store based Cards
    gasService.getCardsPerStore = function (StoreID) {
        return $http({
            method: 'GET',
            url: getCardperStore + '/' + StoreID,
        });
    }

    //Post Sale Section
    gasService.saveGasStationData = function (GasStationPostData) {
        return $http({
            method: 'POST',
            url: PostGasStationURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: GasStationPostData
        });
    }
    //Save Inventory Data
    gasService.saveGasInventoryData = function (GasInventoryPostData) {
        return $http({
            method: 'POST',
            url: PostGasInventoryURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: GasInventoryPostData
        });
    }
    //Save Cards Data
    gasService.saveGasCardsData = function (GasCardsPostData) {
        return $http({
            method: 'POST',
            url: PostGasCardsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: GasCardsPostData
        });
    }

    //Save Delivery Data
    gasService.saveDeliveries = function (deliveryRepeatdata) {
        return $http({
            method: 'POST',
            url: PostDeliveryURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: deliveryRepeatdata
        });

    }
    //Get Day Sale Transactions.
    gasService.getPreviousDayTranscations = function (postData) {
        return $http({
            method: 'POST',
            url: GetDaySaleTransURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //Get Gas Purchases.
    gasService.getGasPurchases = function (postData) {
        return $http({
            method: 'POST',
            url: GetPurchasesURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //Save Purchase Invoice Data.
    gasService.savePurchaseInvoiceData = function (postData) {
        return $http({
            method: 'POST',
            url: SavePurchasesURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //Transaction Types
    gasService.getTransactionTypes = function () {
        return $http({
            method: 'GET',
            url: getTransactionTypesURL,
        });
    }

    //Getting Receipts based on Ref NO;
    gasService.getGasReceipts = function (postData) {
        return $http({
            method: 'POST',
            url: GetReceiptsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //Saving Receipts Data
    gasService.saveReceiptsData = function (postData) {
        return $http({
            method: 'POST',
            url: SaveReceiptsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //Gas SaleReport
    gasService.getGasSaleReport = function (postData) {
        return $http({
            method: 'POST',
            url: getGasSaleReportURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    };
    //Gas Sale Report Month
    gasService.getSaleReportMonth = function (postData) {
        return $http({
            method: 'POST',
            url: GasMonthlyStatementURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    };

    gasService.getLedger = function (postData) {
        return $http({
            method: 'POST',
            url: GetGasLedger,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.getGasAccount = function (storeID) {
        return $http({
            method: 'GET',
            url: GetGasAccount + '/' + storeID,
        });
    }

    gasService.getTransAmount = function (postData) {
        return $http({
            method: 'POST',
            url: GetGasTransAmount,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.getReconcillationReport = function (postData) {
        return $http({
            method: 'POST',
            url: GetReconcillationReportURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    };

    gasService.getPurchaseRegister = function (postData) {
        return $http({
            method: 'POST',
            url: GetPurchaseRegisterURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.getGasSaleTrend = function (postData) {
        return $http({
            method: 'POST',
            url: GetSaleTrendURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.getGasSalesMonth = function (postData) {
        return $http({
            method: 'POST',
            url: GetSaleReportMonthURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }
    gasService.getGasDailySalesMonth = function (postData) {
        return $http({
            method: 'POST',
            url: GetSaleReportIndividualURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.GetStockInwardReport = function (postData) {
        return $http({
            method: 'POST',
            url: GetStockInwardReportURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.getReconcillationDetails = function (postData) {
        return $http({
            method: 'POST',
            url: GetReconcillationDetailsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.UpdateReconcillation = function (postData) {
        return $http({
            method: 'POST',
            url: UpdateReconcillationURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }


    gasService.getShiftendReport = function (postData) {
        return $http({
            method: 'POST',
            url: ShifitendURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    gasService.getGasProfitLossReport = function (gasData) {
        return $http({
            method: 'POST',
            url: ProfitLossURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: gasData
        });
    }

    return gasService;

});