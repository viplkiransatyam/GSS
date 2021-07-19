angular.module('GasApp').controller('ShiftEndController', ['$scope', '$http', '$filter', '$rootScope', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, gasService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.monthlyReportData = [];
    $scope.date = new Date();
    var storeID = $rootScope.StoreID;
    $scope.shifts = [];

    $scope.showAbstDetails = true;
    $scope.showDetailed= false;

    $scope.DaySales = [];
    $scope.DayCardReceipts = [];
    $scope.DayStocks = [];
    $scope.DayPurchases = [];
  
    $scope.DetailedTabContent = function () {
        $scope.showDetailed= true;
        $scope.showAbstDetails = false;
    };
    $scope.ShowAbstractDetailsTabContent = function () {
        $scope.showDetailed= false;
        $scope.showAbstDetails = true;
    };


   //Add Date & shift details to the Header bar
    gasService.getRunningShift(storeID).success(function (result) {
       //console.log(result);
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
    }).error(function () {
        sweetAlert("Error!!", "Error in Getting Running Shift Details", "error");
    });

    //Getting Shifts Details for the store
    gasService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
    });

    $scope.fromDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);

    $scope.getShiftendReport = function (repSalefromDate, shift) {
        var postData = {};
        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = repSalefromDate;
        postData.ShiftID = shift;
        console.log(postData);
        gasService.getShiftendReport(postData).success(function (response) {
            console.log(response);
            $scope.DaySales = response.DaySales;
            $scope.DayCardReceipts = response.DayCardReceipts;
            $scope.DayStocks = response.DayStocks;
            $scope.DayPurchases = response.DayPurchases;
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    };

    $scope.getShiftendReport($scope.fromDate,$scope.ShiftCode);

}]);

