angular.module('GasApp').controller("PaymentDueController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout) {
    var storeID = $rootScope.StoreID;
    $scope.loading = true;
    $scope.lotteryPaymentsData = []; //array for sale report data
    $scope.date = new Date();

    $scope.getPaymentDues = function () { 
        lotteryService.getPaymentDues(storeID).success(function (response) {
            //console.log(response);
            if (response.length <= 0) {
                $scope.lotteryPaymentsData = [];
            } else {
                    $scope.PreviousBusinessEndedDate = response.PreviousBusinessEndedDate;
                    $scope.TotalBooksActive = response.TotalBooksActive;
                    $scope.OnlineSale = response.OnlineSale;
                    $scope.InstantCommission = response.InstantCommission;
                    $scope.SaleCommission = response.SaleCommission;
                    $scope.CashCommission = response.CashCommission;
                    $scope.InstantPaid = response.InstantPaid;
                    $scope.OnlinePaid = response.OnlinePaid;
                    $scope.LotteryReturn = response.LotteryReturn;
                    $scope.TotalDueAmount = response.TotalDueAmount;
                    
                    $scope.lotteryPaymentsData = response.SettledBooks;
                    $scope.tableParams = new NgTableParams({
                        sorting: { GameID: "asc", PackNo: "asc", AutoSettleDate: "asc", BookAmount: "asc"}
                    }, {                        
                        dataset: $scope.lotteryPaymentsData
                    });
                    $scope.getTotal();
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.getTotal = function(){
        var BookAmountTotal = 0;
        for(var i = 0; i < $scope.lotteryPaymentsData.length; i++){
            var product = $scope.lotteryPaymentsData[i];
            BookAmountTotal += (product.BookAmount);
        }
        $scope.BookAmountTotal =  parseFloat(BookAmountTotal).toFixed(2);
    }
    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'LotterySaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'LotteryPayments' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.getPaymentDues();

}]);