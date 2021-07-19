angular.module('GasApp').controller('MonthlyStatementController', ['$scope', '$http', '$filter', '$rootScope', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, gasService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.monthlyReportData = [];
    $scope.date = new Date();
    var storeID = $rootScope.StoreID;
    //Getting Shifts Details for the store
    gasService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;

    });

    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);


    $scope.getSaleReportMonth = function (repSalefromDate, repSaleToDate) {
        var postData = {};
        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        gasService.getSaleReportMonth(postData).success(function (response) {
            console.log(response);
            if (response.length <= 0) {
                $scope.monthlyReportData = [];
            } else {
                $scope.monthlyReportData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { Date: "asc", ShiftCode: "asc", Sales: "asc", CardPayment: "asc", Discount: "asc", TotalPayments: "asc", PaidAmount: "asc", PurchaseValue: "asc", BankCharges: "asc", TotalPayables: "asc" }
                }, {
                    dataset: $scope.monthlyReportData
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'GasMnthlyStmtReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'GasMnthlyStmtReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);

