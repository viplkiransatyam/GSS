angular.module('GasApp').factory('adminService', function ($http, $state, $cookies, $location) {
    const saveStore = "/Api/StoreMaster/PostStoreMaster";
    const selectStores = "/Api/GroupMaster/SelectStores";    
    const getStoreName = "/Api/StoreMaster/GetStoreMaster";
    const SelectUsers = "/Api/GroupMaster/SelectUsers";
    const saveUser = "/Api/StoreMaster/AddUser";
    const updateUser = "/Api/StoreMaster/UpdateUser";
    const getLotteryGamesURL = "/Api/Lottery/GetApplicableLotteryGames";
    const saveLotteryURL = "/Api/Lottery/ActivateLotteryModule";
    const getLotteriesURL = "/Api/GroupMaster/GetLotteryStores";
    const getGasesURL = "/Api/GasOil/SelectAllGasOil";
    const getGroupGasesURL = "/Api/GasOil/GetGroupGasType";
    const saveGasTypeURL = "/Api/GasOil/PostGasOil";
    const SelectRemainingStoreCardTypesURL= "/Api/GroupMaster/SelectRemainingStoreCardTypes";
    const AddCardToStoreURL = "/Api/StoreMaster/AddCardToStore";
    const GetGroupCardTypesURL = "/Api/StoreMaster/GetGroupCardTypes";

    var adminService = [];
    //Add New Store
    adminService.saveStore = function (postJson) {
        return $http({
            method: 'POST',
            url: saveStore,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }
    adminService.saveUser = function (postJson) {
        return $http({
            method: 'POST',
            url: saveUser,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }
    adminService.updateUser = function (postJson) {
        return $http({
            method: 'POST',
            url: updateUser,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

    adminService.getStores = function (searchData) {
       return $http({
            method: 'POST',
            url: selectStores,
            data: searchData
        });
    } 
     //Get StoreName
    adminService.getStoreName = function (storeID) {
        return $http({
            method:'GET',
            url: getStoreName+'/'+storeID,           
        });
    }
    adminService.getUsers = function (searchData) {
       return $http({
            method: 'POST',
            url: SelectUsers,
            data: searchData
        });
    }

    adminService.getLotteryGames = function (storeID) {
       return $http({
            method: 'GET',
            url: getLotteryGamesURL+"/"+storeID,
        });
    }

    adminService.saveLottery = function (searchData) {
       return $http({
            method: 'POST',
            url: saveLotteryURL,
            data: searchData
        });
    }

    adminService.getLotteries = function (storeID) {
       return $http({
            method: 'GET',
            url: getLotteriesURL+"/"+storeID,
        });
    }

    adminService.getGases = function () {
       return $http({
            method: 'POST',
            url: getGasesURL,
        });
    }

    adminService.getGroupGases = function (groupID) {
       return $http({
            method: 'GET',
            url: getGroupGasesURL+"/"+groupID,
        });
    }

    adminService.saveGasType = function (searchData) {
       return $http({
            method: 'POST',
            url: saveGasTypeURL,
            data: searchData
        });
    }

       adminService.getRemainingStoreCardTypes = function (storeID) {
        debugger;
        return $http({
            method: 'GET',
            url: SelectRemainingStoreCardTypesURL + "/" + storeID,
        });
    }


    adminService.saveCards = function (postJson) {
        return $http({
            method: 'POST',
            url: AddCardToStoreURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postJson
        });
    }

    adminService.GetGroupCardTypes = function (groupID) {
        return $http({
            method: 'GET',
            url: GetGroupCardTypesURL + "/" + groupID,
        });
    }

    return adminService;
});