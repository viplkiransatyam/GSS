angular.module('GasApp').controller('dayendreportController', ['$scope', '$http', '$filter', '$rootScope', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, lotteryService, NgTableParams, Excel, $timeout, httpPreConfig) {

    $scope.shift = '';

    $scope.shiftList = [];

    $scope.AbstractSaleModel = {};
    $scope.BalanceModel = {};
    
    $scope.InstantSaleModel = [];
    $scope.InventoryModel = [];

    $scope.showAbstDetails = true;
    $scope.showDetailed= false;
    $scope.showInventory= false;


    $scope.InventoryContent = function () {
        $scope.showInventory= true;
        $scope.showDetailed= false;
        $scope.showAbstDetails = false;
    };
    $scope.DetailedTabContent = function () {
        $scope.showInventory= false;
        $scope.showDetailed= true;
        $scope.showAbstDetails = false;
    };
    $scope.ShowAbstractDetailsTabContent = function () {
        $scope.showInventory= false;
        $scope.showDetailed= false;
        $scope.showAbstDetails = true;
    };


    lotteryService.getRunningShift($rootScope.StoreID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.dateOf = new Date($scope.Year, $scope.Month - 1, $scope.Day);
    }).error(function () {
        sweetAlert("Error!!", result, "error");
        $scope.loading = false;
    });

    lotteryService.getShifts($rootScope.StoreID).success(function (result) {        
        $scope.shiftList = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", result, "error");
        $scope.loading = false;
    });


    $scope.GetDayEndReport = function () {

        var postData = {};
        postData.StoreID = $rootScope.StoreID;
        postData.Date = $filter('date')($scope.dateOf);
        //postData.Date ="02-Feb-2017";
        if ($scope.shift == '') {
            postData.ShiftID = 0;
        }
        else {
            postData.ShiftID = $scope.shift.ShiftCode;
        }
        console.log(postData);

        lotteryService.getLotteryDayEndReport(postData).success(function (result) {
            console.log(result);
            $scope.AbstractSaleModel = result.AbstractSale;
            $scope.BalanceModel = result.Balance;
            $scope.InstantSaleModel = result.InstantSale;
            $scope.InventoryModel = result.Inventory;
            var total = 0;
            for(var i = 0; i < $scope.InstantSale.length; i++){
                var sale = $scope.InstantSale[i];
                total += (sale.AmountSold);
            }
            $scope.saleTotal = total;
        }).error(function (result) {
            sweetAlert("Error!!", result, "error");
            $scope.loading = false;
        });

    };

    $scope.saleFilter = function(instsale){
        return instsale.TicketValue != 0 || instsale.NoOfTickets != 0;
    }

    //$scope.GetDayEndReport();

}]);