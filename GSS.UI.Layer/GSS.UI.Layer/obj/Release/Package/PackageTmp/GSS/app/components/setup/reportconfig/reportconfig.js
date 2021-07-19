angular.module('GasApp').controller('ReportConfigController', ['$scope', '$http', '$filter', '$rootScope', 'setupService', function ($scope, $http, $filter, $rootScope, setupService) {

    $scope.date = new Date();
    $scope.myDate = new Date();
    $scope.username = $rootScope.UserName;

    var getType = function () {
        if ($rootScope.StoreType == "GS") {
            $scope.storeType = true;
        } else if ($rootScope.StoreType == "LS") {
            $scope.storeType = false;
        }
    }

    getType();


    var storeID = $rootScope.StoreID;
    var GroupID = $rootScope.GroupID;

    //Create Groups

    var getGroups = function (storeID) {
        $scope.groups = [];
        setupService.getCustomGroups1(storeID).success(function (result) {
            for (var j = 0; j < result.length; j++) {
                $scope.groups.push({
                    "GroupName": result[j].GroupName,
                });
            }
            $scope.tanks = $scope.groups;
        }).error(function () {
            sweetAlert("Error!!", result, "error");
            
        });
    }

   
   
    
    $scope.addGroup = function (GroupName) {
        
        var postData = {};       
        postData.StoreID = storeID;
        postData.GroupName = GroupName;

        setupService.addReportGroup(postData).success(function (response) {
            
            sweetAlert("Success!!", 'Report Group Created Successfully', "success");
            getGroups(storeID)
        }).error(function (response) {
            sweetAlert("Error!!", response.Message, "error");
            
        });

    }

     getGroups(storeID);

    //mapledger


    $scope.getLedgers = function (selectedId) {       
        $scope.ledgers = [];
        var postData = {};
        postData.StoreID = storeID;
        postData.GroupID = selectedId;
        setupService.getGroupLedgers(postData).success(function (result) {
            for (var j = 0; j < result.length; j++) {
                $scope.ledgers.push({
                    "LedgerName": result[j].LedgerName,
                });
            }
            $scope.tanks = $scope.ledgers;
        }).error(function () {
            sweetAlert("Error!!", result, "error");
            
        });
    }

    $scope.loading = true;
    //Getting Groups information from API For Reports Groups
    setupService.getCustomGroups(storeID).success(function (result) {       
        $scope.groups = result;
        
    }).error(function () {
        sweetAlert("Error!!", result, "error");
        
    });
    //Getting LedgerAccounts information from API
    setupService.getAccounts(storeID).success(function (result) {
        $scope.accounts = result;
        
    }).error(function () {
        sweetAlert("Error!!", result, "error");
        
    });   
   
    $scope.mapLedger = function (mapLedgers) {
        $scope.loading = true;
        var postData = [];
        for (var i = 0; i < $scope.LedgerIDs.length; i++) {
            postData.push({
                'StoreID':parseInt(storeID),
                'GroupID':mapLedgers.GroupID,
                'LedgerID': $scope.LedgerIDs[i].LedgerID

            });            
        }
       
        setupService.addAccountsToReportGroup(postData).success(function (response) {
            
            sweetAlert("Success!!", 'Ledger Mapped to Group Successfully', "success");
           
        }).error(function (response) {
            sweetAlert("Error!!", response.Message, "error");
            
        });

    }

}]);