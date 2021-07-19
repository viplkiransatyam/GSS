angular.module('GasApp').controller("gasSaleReportController", ['$scope', '$rootScope', '$filter', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, gasService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.loading = true;
    var storeID = $rootScope.StoreID;

    $scope.date = new Date();

    //Getting Shifts Details for the store
    gasService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;

    });
    $scope.gasSaleReportData = [];
    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);

    $scope.getgasSaleReport = function (repSalefromDate, repSaleToDate) {
        var postData = {};
        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        gasService.getGasSaleReport(postData).success(function (response) {
            if (response.length <= 0) {
                $scope.gasSaleReportData = [];
            } else {
                $scope.gasSaleReportData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { Date: "asc", ShiftCode: "asc", Unlead: "asc", Premium: "asc", MidGrade: "asc", Kirosene: "asc", Diesel: "asc", NonEthnol: "asc", Total: "asc" }
                }, {
                    dataset: $scope.gasSaleReportData
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'GasSaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'GasSaleReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);