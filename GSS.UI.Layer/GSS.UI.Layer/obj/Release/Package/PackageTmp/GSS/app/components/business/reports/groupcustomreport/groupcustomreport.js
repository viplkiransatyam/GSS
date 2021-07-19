angular.module('GasApp').controller('GroupCustomController', ['$scope', '$http', '$filter', '$rootScope', 'businessService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, businessService, NgTableParams, Excel, $timeout, httpPreConfig) {
    
    //$scope.lotterySaleReportData = []; //array for sale report data
    $scope.ledgerData = []; 
    $scope.LedgerData1 = []; 

    $scope.ledger = "";

    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);



    $scope.storeID = $rootScope.StoreID;

   
    $scope.getLedger = function (fromdate, todate) {
     
        var postData = {};
        //var saleReportData = [];

        repSalefromDate = $filter('date')(new Date(fromdate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(todate), 'dd-MMM-yyyy');
        postData.StoreID = $scope.storeID;
        postData.LedgerID = $scope.ledger.LedgerID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        businessService.getCustomReport(postData).success(function (response) {  
            if (response.length <= 0) {
                $scope.ledgerData = [];
            } else {
                $scope.ledgerData = response;
            }

            $scope.tableParams = new NgTableParams({
            }, {
                dataset: $scope.ledgerData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            
        });
    }  
    $scope.getLedger($scope.fromDate,$scope.toDate);

     $scope.getDetailedReport = function (selectedID) {
                
        $scope.Titlename = selectedID.GroupName;
        var postData = {};
        postData.StoreID = $scope.storeID;
        postData.LedgerID = parseInt(selectedID.GroupID);
        postData.FromDate = $filter('date')($scope.fromDate, 'dd-MMM-yyyy');
        postData.ToDate = $filter('date')($scope.toDate, 'dd-MMM-yyyy');
        $scope.Ledgerdata1 = [];
        businessService.getGroupCustomDetailedReport(postData).success(function (response) {   
            if (response.length <= 0) {
                $scope.Ledgerdata1 = [];
            } else {
                $scope.Ledgerdata1 = response;
            }

            $scope.tableParams = new NgTableParams({
            }, {
                dataset: $scope.Ledgerdata1
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            
        });       
    }
      

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'tableToExport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'GroupCustomReport' + $filter('date')(new Date($scope.fromDate), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

     $scope.exportToExcel1 = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'tableToExport1');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'GroupCustomLedgerReport' + $filter('date')(new Date($scope.fromDate), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }


    //$scope.getStoreAccounts();
    //$scope.grandTotal = $scope.sum($scope.lotteryActiveBooksReportData, 'AmountSold');
}]);