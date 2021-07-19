angular.module('GasApp').controller("profitlossController", ['$scope', '$rootScope', '$filter', 'gasService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, gasService, NgTableParams, Excel, $timeout) {
    
    $scope.gasProfitLossData = []; //array for detailed sale report data

    $scope.gastype = '';

    $scope.GasTypeList = [];

    var storeID = $rootScope.StoreID;
    $scope.date = new Date();

    $scope.fromDate = new Date($scope.date.getFullYear(), $scope.date.getMonth(), 2);
    $scope.toDate = new Date();

    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);

    gasService.getStorebasedGas(storeID).success(function (result) {        
        $scope.GasTypeList = result;
    }).error(function () {
        sweetAlert("Error!!", result, "error");
    });
    

    $scope.gasProfitLossReport = function (gasttype,repSalefromDate,repSaleToDate) {
        var postData = {};
        //var saleReportData = [];

        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.LedgerID = gasttype.GasTypeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        gasService.getGasProfitLossReport(postData).success(function (response) {
            $scope.gasProfitLossData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { Date : "asc", OpenQty: "asc", InwardQty: "asc" ,SaleQty: "asc", ClosingQty: "asc" ,SystemClosingQty: "asc", ShortOver: "asc" ,SalePrice: "asc",PurchasePrice: "asc",ProfitLoss: "asc"}
            }, {
                dataset: $scope.gasProfitLossData
            });        
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'ProfitLossReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'ProfitLossReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();

    }

}]);