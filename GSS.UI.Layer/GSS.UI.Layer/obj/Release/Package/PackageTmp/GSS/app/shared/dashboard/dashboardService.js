angular.module("GasApp").service('dashboardService', ['$http', function ($http) {
    const getPreviousShiftSalesURL = "/Api/Lottery/GetDashboard/";
    const getGasDashboardDataURL = "/Api/GasOil/DashboardPreviousShift/";
    const DashboardGasProfitLossURL = "/Api/Report/GetDashboardProfitAndLoss";
    
    var dashboardService = [];
    dashboardService.getPreviousShiftSales = function (storeID) {
        return $http({
            method: 'GET',
            url: getPreviousShiftSalesURL + storeID
        });
    }

    dashboardService.getGasDashboardDataSales = function (storeID) {
        return $http({
            method: 'GET',
            url: getGasDashboardDataURL + storeID
        });
    }

    dashboardService.getGasDashboardProfitLossReport = function (requestData) {
        return $http({
            method: 'POST',
            url: DashboardGasProfitLossURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: requestData
        });
    }


    return dashboardService;
}]);


