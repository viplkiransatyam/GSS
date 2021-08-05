angular.module('GasApp').factory('businessService', function ($http, $state, $cookies, $location) {
    const getShiftsPerStoreURL = "/Api/StoreMaster/GetShifts";
    const GetRunningShiftURL = "/Api/GasOil/GetRunningShift";
    const getAccountLedgersURL = "/Api/AccoutMaster/GetAccounts";   
    const getAccountCommonURL = "/Api/AccoutMaster/GetCommonAccounts";   
    const saveRecordsURL = "/Api/Sale/SaveJournalVoucher";
    const deleteRecordsURL = "/Api/Sale/DeletePaymentOrReceipt";
    const GetDaySaleTransURL = "/Api/Sale/GetDaySale";
    const GetGroupSaleURL = "/Api/AccoutMaster/GetBusinessSalesGroups";
    const GetGropuPaidURL = "/Api/AccoutMaster/GetBusinessPaidGroups";
    const GetSaleLedgersURL = "/Api/AccoutMaster/GetSalesLedgers";
    const GetBusinessPaidLedgersURL = "/Api/AccoutMaster/GetSalesLedgers";    
    const BusinessSaveURL = "/Api/Sale/SaveBusinessSale";
    const GetCashDepositURL = "/Api/Sale/GetCashDeposit";
    const SaveCashDepositURL = "/Api/Sale/SaveCashDeposit";
    const GetCashFlowURL = "/Api/Sale/GetCashFlow";
    const SaveCashEntryURL = "/Api/Sale/FinalizeTransaction";
    const GetStoreAccountsURL = "/Api/AccoutMaster/GetStoreAccounts";
    const GetLedgersURL = "/Api/Report/GetLedger";
    const GetMonthlyBusinessSheetURL = "/Api/Report/GetMonthlyBusinessSheet";
    const GetNewBusinessPaidLedgersURL = "/Api/AccoutMaster/GetBusinessPaidLedgers";

    var businessService = [];
    
    //Get Shifts
    businessService.getShifts = function (storeID) {
        return $http({
            method: 'GET',
            url: getShiftsPerStoreURL + '/' + storeID,
        });
    }

    //Get Shifts
    businessService.getRunningShift = function (storeID) {
        return $http({
            method: 'GET',
            url: GetRunningShiftURL + '/' + storeID,
        });
    }

    businessService.getAccountLedgers = function (StoreId) {
        return $http({
            method: 'GET',
            url: getAccountLedgersURL+"/"+StoreId,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    businessService.getAccountCommonLedgers = function (StoreId) {
        return $http({
            method: 'GET',
            url: getAccountCommonURL + "/" + StoreId,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    businessService.saveRecords = function (postData) {
        return $http({
            method: 'POST',
            url: saveRecordsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    businessService.deleteRecords = function (postData) {
        return $http({
            method: 'POST',
            url: deleteRecordsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

     //Get Day Sale Transactions.
    businessService.getPreviousDayTranscations = function (postData) {
        return $http({
            method: 'POST',
            url: GetDaySaleTransURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    businessService.getAccounts = function (storeID) {
        return $http({
            method: 'GET',
            url: getAccountperStore + '/' + storeID,
        });
    }

    businessService.groupSales = function (storeID) {
        return $http({
            method: 'GET',
            url: GetGroupSaleURL + '/' + storeID,
        });
    }
    businessService.groupPaid = function (storeID) {
        return $http({
            method: 'GET',
            url: GetGropuPaidURL + '/' + storeID,
        });
    }
    businessService.individualsaleorpaid = function (storeID) {
        return $http({
            method: 'GET',
            url: GetSaleLedgersURL + '/' + storeID,
        });
    }
    
    businessService.saveBusiness = function (postData) {
        return $http({
            method: 'POST',
            url: BusinessSaveURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

     //Get Day Sale Transactions.
    businessService.getCashDeposit = function (postData) {
        return $http({
            method: 'POST',
            url: GetCashDepositURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    businessService.saveCashDeposit = function (postData) {
        return $http({
            method: 'POST',
            url: SaveCashDepositURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

       //Get Day Sale Transactions.
    businessService.getCashFlow = function (postData) {
        return $http({
            method: 'POST',
            url: GetCashFlowURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    businessService.saveCashEntry = function (postData) {
        return $http({
            method: 'POST',
            url: SaveCashEntryURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    businessService.getStoreAccounts = function (storeID) {
        return $http({
            method: 'GET',
            url: GetStoreAccountsURL + '/' + storeID,
        });
    }

    businessService.getLedgers = function (postData) {
        return $http({
            method: 'POST',
            url: GetLedgersURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    businessService.getMonthlyReport= function (postData) {
        return $http({
            method: 'POST',
            url: GetMonthlyBusinessSheetURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //newely added code
    businessService.getPaidLedgers = function (storeID) {
        return $http({
            method: 'GET',
            url: GetNewBusinessPaidLedgersURL + '/' + storeID,
        });
    }

    return businessService;
});