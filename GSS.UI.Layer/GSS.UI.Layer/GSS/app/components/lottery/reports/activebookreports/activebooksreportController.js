angular.module('GasApp').controller("ActiveBooksReportController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout) {
    $scope.loading = true;
    //$scope.lotterySaleReportData = []; //array for sale report data
    $scope.lotteryActiveBooksReportData = []; //array for sale report data

    var storeID = $rootScope.StoreID;
    $scope.date = new Date();

    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);

    $scope.lotteryActiveBooksReportData = function (repSalefromDate, repSaleToDate) {
        var postData = {};
        //var saleReportData = [];

        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');

        postData.StoreID = storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;

        lotteryService.getLotteryActiveBooksReport(postData).success(function (response) {
            if (response.length <= 0) {
                $scope.lotteryActiveBooksReportData = [];
            } else {
                $scope.lotteryActiveBooksReportData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { Date: "asc", ShiftCode: "asc", GameNo: "asc", GameName: "asc", Pack: "asc", BoxNo: "asc", AmountSold: "asc", PackStatus: "asc" }
                }, {
                    dataset: $scope.lotteryActiveBooksReportData
                });
                $scope.grandTotal = $scope.sum($scope.lotteryActiveBooksReportData, 'AmountSold');
            }
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
        var exportHref = Excel.tableToExcel(tableId, 'ActiveBooksReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'ActiveBooksReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }
}]);