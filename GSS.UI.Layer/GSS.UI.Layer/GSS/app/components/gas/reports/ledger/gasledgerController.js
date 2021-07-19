angular.module('GasApp').controller('gasledgerController', ['$scope', '$http', '$filter', '$rootScope', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, gasService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.loading = true;
    //$scope.lotterySaleReportData = []; //array for sale report data
    $scope.ledgerData = []; //array for sale report data
    $scope.ledgeformisvalid =true ;
    $scope.gasAccounts = [];

    $scope.ledger = "";

    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);



    $scope.storeID = $rootScope.StoreID;

    $scope.getLedger = function (fromdate, todate) {

        if ($scope.ledger.LedgerID==undefined)
        {
            sweetAlert("Please select account", "", "error");
            return;
        }
        var postData = {};
        //var saleReportData = [];

        repSalefromDate = $filter('date')(new Date(fromdate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(todate), 'dd-MMM-yyyy');
        postData.StoreID = $scope.storeID;
        postData.LedgerID = $scope.ledger.LedgerID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;

        gasService.getLedger(postData).success(function (response) {
            if (response.length <= 0) {
                $scope.ledgerData = [];
            } else {
                $scope.ledgerData = response;
                console.log(response);
            }

            $scope.tableParams = new NgTableParams({
            }, {
                dataset: $scope.ledgerData
            });


        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.getgasAcct = function () {

        gasService.getGasAccount($rootScope.StoreID).success(function (response) {
            
            $scope.gasAccounts = response;

        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.getgasAcct();

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'tableToExport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'tableToExport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    //$scope.grandTotal = $scope.sum($scope.lotteryActiveBooksReportData, 'AmountSold');
}]);