angular.module('GasApp').controller("DetailedSaleReportController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout) {
    $scope.loading = true;
    $scope.lotteryDetailedSalesReportData = []; //array for detailed sale report data


    var storeID = $rootScope.StoreID;
    $scope.date = new Date();

    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);

    $scope.lotteryDetailedSalesReport = function (repSalefromDate,repSaleToDate) {
        var postData = {};
        //var saleReportData = [];

        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');

        postData.StoreID = storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        lotteryService.getLotteryDetailedSalesReport(postData).success(function (response) {    
            $scope.lotteryDetailedSalesReportData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { Date: "asc", ShiftCode: "asc" }
            }, {
                dataset: $scope.lotteryDetailedSalesReportData
            });
            $scope.grandTotal = $scope.sum($scope.lotteryDetailedSalesReportData, 'AmountSold');         
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.sum = function (items, prop) {
        return items.reduce(function (a, b) {
            return a + b[prop];
        }, 0);
    };


    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'LotteryDetailedSaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'LotteryDetailedSaleReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();

    }

}]);