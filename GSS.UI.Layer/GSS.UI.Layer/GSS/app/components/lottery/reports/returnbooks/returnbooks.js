/// <reference path="../salereports/lotterySaleReportController.js" />
angular.module('GasApp').controller('returnbooksController', ['$scope', '$http', '$filter', '$rootScope', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, lotteryService, NgTableParams, Excel, $timeout, httpPreConfig) {

    $scope.loading = true;


    $scope.retrunBookList = [];


    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);



    $scope.storeID = $rootScope.StoreID;

    $scope.getReturnBooks = function (fromdate, todate) {
        var postData = {};
        repSalefromDate = $filter('date')(new Date(fromdate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(todate), 'dd-MMM-yyyy');
        postData.StoreID = $scope.storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;

        lotteryService.getReturnBooks(postData).success(function (response) {

            if (response.length <= 0) {
                $scope.retrunBookList = [];
            } else {
                $scope.retrunBookList = response;
            }

            $scope.tableParams = new NgTableParams({
                sorting: { Date: "asc", ShiftCode: "asc", Game: "asc", GameName: "asc", Pack: "asc", EndTicket: "asc" }
            }, {
                dataset: $scope.retrunBookList
            });


        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'tableToExport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'tableToExport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    //$scope.grandTotal = $scope.sum($scope.lotteryActiveBooksReportData, 'AmountSold');
}]);