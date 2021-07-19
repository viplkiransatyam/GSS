angular.module('GasApp').controller("BooksActiveNotSettledController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout) {
    var storeID = $rootScope.StoreID;
    $scope.loading = true;
    $scope.lotteryPaymentsData = []; //array for sale report data
    $scope.date = new Date();

    $scope.notSettledBooksList = function () { 
        lotteryService.notSettledBooksList(storeID).success(function (response) {
            //console.log(response);
            if (response.length <= 0) {
                $scope.lotteryPaymentsData = [];
            } else {
                    $scope.lotteryPaymentsData = response;
                    $scope.tableParams = new NgTableParams({
                        sorting: { Date: "asc",GameID: "asc", PackNo: "asc", BookAmount: "asc", SettleAmount: "asc"}
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
        var SettleAmountTotal = 0;
        for(var i = 0; i < $scope.lotteryPaymentsData.length; i++){
            var product = $scope.lotteryPaymentsData[i];
            BookAmountTotal += (product.BookAmount);
            SettleAmountTotal += (product.SettleAmount);
        }
       $scope.BookAmountTotal = parseFloat(BookAmountTotal).toFixed(2);
       $scope.SettleAmountTotal = parseFloat(SettleAmountTotal).toFixed(2);
    }

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'LotterySaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'LotteryPayments' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.notSettledBooksList();

}]);