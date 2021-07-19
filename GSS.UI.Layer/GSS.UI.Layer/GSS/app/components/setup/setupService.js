angular.module('GasApp').factory('setupService', function ($http, $state, $cookies, $location) {
    const saveAddGame = "/Api/Lottery/AddGame";
    const getLotteryInventoryReportURL = "/Api/Lottery/GetLotteryGames";
    const settlePacksURL = "/Api/Lottery/UpdateLotterySettle";
    const AddLotteryReturnURL = "/Api/Lottery/AddLotteryReturn";
    const UpdateCommURL = "/Api/Lottery/UpdateCommission";
    const getAccountGroupsURL = "/Api/AccoutMaster/GetAccountGroups";
    const getBusinessAccountGroupsURL = "/Api/AccoutMaster/GetBusinessAccountGroups";
    const getAccountLedgersURL = "/Api/AccoutMaster/GetAccounts";
    const getSaleGroupsURL = "/Api/AccoutMaster/GetSalesGroups";
    const getSaleIndividualsURL = "/Api/AccoutMaster/GetSalesLedgers";
    const getSaveAccountLedgerURL = "/Api/AccoutMaster/PostAccountMaster";
    const GetDaySaleTransURL = "/Api/Sale/GetDaySale";
    const SavePurchasesURL = "/Api/Sale/SavePurchase";
    const SavePurchaseRetunsURL = "/Api/Sale/SavePurchaseReturn";
    const getShiftsPerStoreURL = "/Api/StoreMaster/GetShifts";
    const GetRunningShiftURL = "/Api/Lottery/GetRunningShift";
    const GetCustomGroups1URL = "/Api/Report/GetCustomGroups";
    const AddReportGroupURL = "/Api/Report/AddReportGroup";
    const GetGroupLedgersURL = "/Api/Report/GetGroupLedgers";
    const GetCustomGroupsURL = "/Api/Report/GetCustomGroups";
    const getAccountperStore = "/Api/AccoutMaster/GetAccounts";
    const AddAccountsToReportGroupURL = "/Api/Report/AddAccountsToReportGroup";
    const deleteAccountURL = "/Api/AccoutMaster/DeleteAccountMaster";

    var setupService = [];
    //Add New Game
    setupService.addGame = function (postJson) {
        return $http({
            method: 'POST',
            url: saveAddGame,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

    setupService.GameList = function (postJson) {
        return $http({
            method: 'GET',
            url: getLotteryInventoryReportURL + "/" + postJson,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

    setupService.saveSettelPacks = function (postJson) {
        return $http({
            method: 'POST',
            url: settlePacksURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

    setupService.AddLotteryReturn = function (postJson) {
        return $http({
            method: 'POST',
            url: AddLotteryReturnURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

     setupService.updateComm = function (postJson) {
        return $http({
            method: 'POST',
            url: UpdateCommURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }
    

    setupService.getGroups = function () {
        return $http({
            method: 'GET',
            url: getAccountGroupsURL,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    setupService.getBusinessGroupAccounts = function () {
        return $http({
            method: 'GET',
            url: getBusinessAccountGroupsURL,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    setupService.getAccountLedgers = function (StoreId) {
        return $http({
            method: 'GET',
            url: getAccountLedgersURL+"/"+StoreId,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    setupService.getSaleGroups = function (StoreId) {
        return $http({
            method: 'GET',
            url: getSaleGroupsURL+"/"+StoreId,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    setupService.getSaleIndividuals = function (StoreId) {
        return $http({
            method: 'GET',
            url: getSaleIndividualsURL+"/"+StoreId,
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }


    setupService.addAccountLedger = function (postJson) {
        return $http({
            method: 'POST',
            url: getSaveAccountLedgerURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

    setupService.addSalesGroup = function (postJson) {
        return $http({
            method: 'POST',
            url: getSaveAccountLedgerURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }
    
    setupService.addSalesIndividuals = function (postJson) {
        return $http({
            method: 'POST',
            url: getSaveAccountLedgerURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

     //Get Day Sale Transactions.
    setupService.getPreviousDayTranscations = function (postData) {
        return $http({
            method: 'POST',
            url: GetDaySaleTransURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    setupService.savePurchases = function (postData) {
         return $http({
             method: 'POST',
             url: SavePurchasesURL,
             data: postData
         });
     }

     setupService.savePurchaseReturns = function (postData) {
         return $http({
             method: 'POST',
             url: SavePurchaseRetunsURL,
             data: postData
         });
     }

      //Get Shifts
    setupService.getShifts = function (storeID) {
        return $http({
            method: 'GET',
            url: getShiftsPerStoreURL + '/' + storeID,
        });
    }

    //Get Shifts
    setupService.getRunningShift = function (storeID) {
        return $http({
            method: 'GET',
            url: GetRunningShiftURL + '/' + storeID,
        });
    }
    

    setupService.getCustomGroups1 = function (storeID) {
         return $http({
             method: 'GET',
             url: GetCustomGroups1URL + '/' + storeID,
         });
     }

    setupService.addReportGroup = function (postData) {
         return $http({
             method: 'POST',
             url: AddReportGroupURL,
             headers: {
                 'Content-Type': 'application/json'
             },
             data: postData
         });
     }

    setupService.getGroupLedgers = function (postData) {
         return $http({
             method: 'POST',
             url: GetGroupLedgersURL,
             data: postData
         });
     }

    setupService.getCustomGroups = function (storeID) {
     return $http({
         method: 'GET',
         url: GetCustomGroupsURL + '/' + storeID,
     });
    }

    setupService.getAccounts = function (storeID) {
        return $http({
            method: 'GET',
            url: getAccountperStore + '/' + storeID,
        });
    }

    setupService.addAccountsToReportGroup = function (postData) {
         return $http({
             method: 'POST',
             url: AddAccountsToReportGroupURL,
             headers: {
                 'Content-Type': 'application/json'
             },
             data: postData
         });
     } 

     setupService.deleteAccount = function (postData) {
         return $http({
             method: 'POST',
             url: deleteAccountURL,
             headers: {
                 'Content-Type': 'application/json'
             },
             data: postData
         });
     }

    return setupService;
});