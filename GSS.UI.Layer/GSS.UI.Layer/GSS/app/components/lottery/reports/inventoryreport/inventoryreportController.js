angular.module('GasApp').controller("inventoryreportController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.inventoryReportData = [];

    

    
    lotteryService.getLotteryInventoryReportData($rootScope.StoreID).success(function (response) { 

        
        
            if (response.length <= 0) {
                $scope.inventoryReportData = [];
            } else {
                $scope.inventoryReportData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { GameID: "asc", GameDescription: "asc", Received: "asc", Activated: "asc", Total: "asc"}
                }, {
                    dataset: $scope.inventoryReportData
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });

    

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'lotteryInventoryReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'lotteryInventoryReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);