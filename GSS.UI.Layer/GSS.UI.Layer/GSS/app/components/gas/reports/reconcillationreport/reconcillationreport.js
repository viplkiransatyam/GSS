angular.module('GasApp').controller('reconcillationreport', ['$scope', '$http', '$filter', '$rootScope', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, gasService, NgTableParams, Excel, $timeout, httpPreConfig) { 
    $scope.reconcillationReportData= [];
    $scope.date = new Date();
    $scope.AllStore = [];
    

    var storeID = $rootScope.StoreID;
    //Getting Shifts Details for the store
    

    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);


    $scope.getReconcillationReport = function (repSalefromDate, repSaleToDate) {
        var postData = {};
        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.StoreID = 3;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        gasService.getReconcillationReport(postData).success(function (response) {
            console.log(response);
            if (response.length <= 0) {
                $scope.reconcillationReportData = [];
            } else {

                var summaryrow = response[response.length-1];

                $scope.reconcillationReportData = response;
                //$scope.reconcillationReportData = response;


                $scope.tableParams = new NgTableParams({
                    sorting: { Date: "asc", CardAmount: "asc", PaidAmount: "asc", GSSInvoiceAmount: "asc", CreditTransaction: "asc", Discount: "asc", Fee: "asc", VendorInvoiceAmount: "asc"}
                }, {
                    dataset: $scope.reconcillationReportData
                });
                
                $scope.cardAmountTotalGSS = $scope.sum($scope.reconcillationReportData, 'CardAmount');
                $scope.CreditTransactionTotalVendor = $scope.sum($scope.reconcillationReportData, 'CreditTransaction');
                //$scope.DiffcardAmount = $scope.cardAmountTotalGSS - $scope.CreditTransactionTotalVendor;

                $scope.DiffcardAmount = summaryrow.CreditTransaction - summaryrow.CardAmount;

                $scope.purchaseInvoiceGSS = $scope.sum($scope.reconcillationReportData, 'GSSInvoiceAmount');
                $scope.purchaseInvoiceVendor = $scope.sum($scope.reconcillationReportData, 'VendorInvoiceAmount');
                $scope.bankFeeChargeVendor = $scope.sum($scope.reconcillationReportData, 'Fee');

                //$scope.purchaseDif = $scope.purchaseInvoiceGSS - $scope.purchaseInvoiceVendor;

                $scope.purchaseDif = summaryrow.GSSInvoiceAmount - summaryrow.VendorInvoiceAmount;


            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };

    $scope.sum = function (items, prop) {
        return items.reduce(function (a, b) {
            return a + b[prop];
        }, 0);
    };
    
    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'reconcillationstatement');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'reconcillationstatement' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);


