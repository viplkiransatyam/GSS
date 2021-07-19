angular.module('GasApp').controller('MonthlyStmtController', ['$scope', '$http', '$filter', '$rootScope', 'businessService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, businessService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.ledgerData = []; //array for sale report data
    $scope.Date = new Date();
    $scope.storeID = $rootScope.StoreID;

    $scope.fromDate = $filter('date')(new Date($scope.Date), 'dd-MMM-yyyy');
    var monthNames = ["January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
    ];
    $scope.getLedger = function (fromdate) {        
        if (fromdate==undefined || angular.isUndefined(fromdate) || fromdate =='')
        {
            sweetAlert("Please Select Date", "", "error");
            return;
        }
        var postData = {};
        postData.StoreID = $scope.storeID;
        postData.Month = monthNames[fromdate.getMonth()];
        postData.Year = fromdate.getFullYear();
        console.log(postData);
        businessService.getMonthlyReport(postData).success(function (response) {
            console.log(response);
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
            $scope.loading = false;
        });
    }    

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'tableToExport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'MonthlyReport' + $filter('date')(new Date($scope.fromDate), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.getLedger($scope.Date);
}]);