angular.module('GasApp').controller("lotterygamesController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.lotteryGamesReportData = [];




    lotteryService.getLotteryGames($rootScope.StoreID).success(function (response) {



        if (response.length <= 0) {
            $scope.lotteryGamesReportData = [];
        } else {
            $scope.lotteryGamesReportData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { GameID: "asc", GameName: "asc", NoOfTickets: "asc", TicketValue: "asc", BookValue: "asc",TicketStartNumber:"asc",TicketEndNumber:"asc" }
            }, {
                dataset: $scope.lotteryGamesReportData
            });
        }
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
        $scope.loading = false;
    });



    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'lotterygamesReports');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'lotterygamesReports' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);