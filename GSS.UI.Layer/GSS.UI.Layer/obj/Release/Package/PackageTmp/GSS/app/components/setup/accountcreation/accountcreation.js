angular.module('GasApp').controller('AccountCreationController', ['$scope', '$http', '$filter', '$rootScope', 'setupService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, setupService, NgTableParams, Excel, $timeout, httpPreConfig) {

    var storeID = $rootScope.StoreID;
    $scope.groups = [];
    $scope.businessaccountgroups = [];

    $scope.account = {
        "StoreID": storeID,
        "LedgerID": 0,
        "LedgerName": "",
        "GroupID": "",
        "Remarks": "",
        "ActiveStatus": "A",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "DisplayStatus":""
    };

    $scope.salesgroup = {
        "StoreID": storeID,
        "LedgerID": 0,
        "LedgerName": "",
        "GroupID": "",
        "Remarks": "",
        "ActiveStatus": "A",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": ""
    };

    $scope.saleindividual = {
        "StoreID": storeID,
        "LedgerID": 0,
        "LedgerName": "",
        "GroupID": "",
        "Remarks": "",
        "SalesGroupID" : "",
        "ActiveStatus": "A",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": ""
    };



    //Getting Groups 
    setupService.getGroups().success(function (result) {
        $scope.groups = result;
    }).error(function (result) {
        sweetAlert("Error!!",result, "error");
    });

    //Getting  Business account groups
    setupService.getBusinessGroupAccounts().success(function (result) {
        $scope.businessaccountgroups = result;
    }).error(function (result) {
        sweetAlert("Error!!",result, "error");
    });


    $scope.showLedgerDetails = true;
    $scope.showSaleGroupDetails= false;
    $scope.showIndividualDetails= false;


    $scope.accountLedger = function () {
        $scope.showIndividualDetails= false;
        $scope.showSaleGroupDetails= false;
        $scope.showLedgerDetails = true;
        $scope.getAccountCommonURL();
        $scope.accountreset();
    };
    $scope.saleGroupCreation = function () {
        $scope.showIndividualDetails= false;
        $scope.showSaleGroupDetails= true;
        $scope.showLedgerDetails = false;
        $scope.getSaleGroups();
        $scope.salesgroupreset();
    };

    $scope.saleIndividual = function () {
        $scope.showIndividualDetails= true;
        $scope.showSaleGroupDetails= false;
        $scope.showLedgerDetails = false;
        $scope.getSaleGroups();
        $scope.getSaleIndividuals();
        $scope.saleindividualreset();
    };

    $scope.LedgersData = [];
    $scope.SaleGroupsData = [];
    $scope.SaleIndividualsData = [];

    $scope.getAccountLedgers = function(){
        setupService.getAccountLedgers($rootScope.StoreID).success(function (response) {        
            $scope.LedgersData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { LedgerName: "asc",SalesGroupName:"asc"}
            }, {
                dataset: $scope.LedgersData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }


    $scope.getSaleGroups = function(){
        setupService.getSaleGroups($rootScope.StoreID).success(function (response) {
            $scope.SaleGroupsData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { SalesGroupName: "asc"}
            }, {
                dataset: $scope.SaleGroupsData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }


    $scope.getSaleIndividuals = function(){
        setupService.getSaleIndividuals($rootScope.StoreID).success(function (response) {
        	console.log(response);
            $scope.SaleIndividualsData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { LedgerName: "asc",SalesGroupName:"asc"}
            }, {
                dataset: $scope.SaleIndividualsData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }

    $scope.accountreset = function () {
        $scope.account = {
        	"StoreID": storeID,
	        "LedgerID": 0,
	        "LedgerName": "",
	        "GroupID": "",
	        "Remarks": "",
	        "ActiveStatus": "A",
	        "CreatedUserName": $rootScope.UserName,
	        "CreateTimeStamp": "",
	        "ModifiedUserName": $rootScope.UserName,
	        "ModifiedTimeStamp": ""
        };
    };

    $scope.salesgroupreset = function () {
        $scope.salesgroup = {
        	"StoreID": storeID,
	        "LedgerID": 0,
	        "LedgerName": "",
	        "GroupID": "",
	        "Remarks": "",
	        "ActiveStatus": "A",
	        "CreatedUserName": $rootScope.UserName,
	        "CreateTimeStamp": "",
	        "ModifiedUserName": $rootScope.UserName,
	        "ModifiedTimeStamp": ""
        };
    };

    $scope.saleindividualreset = function () {
        $scope.saleindividual = {
	        "StoreID": storeID,
	        "LedgerID": 0,
	        "LedgerName": "",
	        "GroupID": "",
	        "Remarks": "",
	        "SalesGroupID" : "",
	        "ActiveStatus": "A",
	        "CreatedUserName": $rootScope.UserName,
	        "CreateTimeStamp": "",
	        "ModifiedUserName": $rootScope.UserName,
	        "ModifiedTimeStamp": ""
        };
    };

    $scope.addAccountLedger = function () {
            var AccountLedgerPostData = angular.toJson($scope.account);
            AccountLedgerPostData = JSON.parse(AccountLedgerPostData);
            setupService.addAccountLedger(AccountLedgerPostData).success(function (response) {  
                sweetAlert("Success", "Account Ledger added successfully", "success");
                $scope.getAccountLedgers();
            }).error(function (response) {
                sweetAlert("Error",response,"error");
            });
    }; 

    $scope.addSalesGroup = function () {
            var SaleGroupPostData = angular.toJson($scope.salesgroup);
            SaleGroupPostData = JSON.parse(SaleGroupPostData);
            setupService.addSalesGroup(SaleGroupPostData).success(function (response) {  
                sweetAlert("Success", "Sale Group added successfully", "success");
                $scope.getSaleGroups();
            }).error(function (response) {
                sweetAlert("Error",response,"error");
            });
    };   

    $scope.addSaleIndividual = function () {
            var SalesIndividualPostData = angular.toJson($scope.saleindividual);
            SalesIndividualPostData = JSON.parse(SalesIndividualPostData);
            setupService.addSalesIndividuals(SalesIndividualPostData).success(function (response) {  
                sweetAlert("Success", "Sales Individual added successfully", "success");
                $scope.getSaleIndividuals();
            }).error(function (response) {
                sweetAlert("Error",response,"error");
            });
    };


    $scope.editItem = function(item){
        console.log(item);
        $scope.account.LedgerName = item.LedgerName;
        $('#GroupID').val(item.GroupID);
        $scope.account.GroupID = $('#GroupID').val();
        $scope.account.Remarks = item.Remarks;
        $scope.account.LedgerID = item.LedgerID;
    }

    $scope.removeItem = function(item){
        setupService.deleteAccount(item).success(function (response) {  
            sweetAlert("Success", "Account Deleted Successfully", "success");
        }).error(function (response) {
            sweetAlert("Error",response,"error");
        }); 
    }

    $scope.display = function(item){
        $scope.account.LedgerID = item.LedgerID;
        $scope.account.DisplayStatus = 'N';
        $scope.account.GroupID = item.GroupID;
        $scope.account.LedgerName = item.LedgerName;

        var AccountLedgerPostData = angular.toJson($scope.account);
        AccountLedgerPostData = JSON.parse(AccountLedgerPostData);
        setupService.addAccountLedger(AccountLedgerPostData).success(function (response) {  
            sweetAlert("Success", "DisplayStatus Updated Successfully", "success");
        }).error(function (response) {
            sweetAlert("Error",response,"error");
        });

    }

    $scope.getAccountLedgers();   

}]);