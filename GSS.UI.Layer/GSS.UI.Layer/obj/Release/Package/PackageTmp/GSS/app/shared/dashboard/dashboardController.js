angular.module('GasApp').controller('DashboardController', ["$scope", "$rootScope", "$filter", "dashboardService", function ($scope, $rootScope, $filter, dashboardService) {
    $scope.loading = true;  
    $scope.dashboardData = [];
    $scope.gasdashboardData = [];
    $scope.reportData = [];
    $scope.gasreportData = [];

    $scope.gasDashboardProfitLossData = []; // to hold dashboard data for each gas type with Weekly and Daily Profit and Loss.

    // if ($rootScope.AvailBusiness == "Y") { $scope.AvailBusiness = true; } else if ($rootScope.AvailBusiness == "N") { $scope.AvailBusiness = false; };
    // if ($rootScope.AvailGas == "Y") { $scope.AvailGas = true; } else if ($rootScope.AvailGas == "N") { $scope.AvailGas = true; };
    // if ($rootScope.AvailLottery == "Y") { $scope.AvailLottery = true; } else if ($rootScope.AvailLottery == "N") { $scope.AvailLottery = false; };
    

    $scope.getDasboardData = function () {
        var getDashboardDataAPI = dashboardService.getPreviousShiftSales(storeID);
        getDashboardDataAPI.then(function (result) {
            $scope.dashboardData = result.data;
            $scope.init();
        }, function () {
            location.reload();
        });
    };

    $scope.getGasDasboardData = function () {
        var getGasDashboardDataAPI = dashboardService.getGasDashboardDataSales(storeID);
        getGasDashboardDataAPI.then(function (result) {
            $scope.gasdashboardData = result.data;
            $scope.gasGraph();
        }, function () {
            //location.reload();
        });
    };

    $scope.init = function(){
        if($scope.dashboardData.DashboardSaleReport.length>0){
            for (var i = $scope.dashboardData.DashboardSaleReport.length; i --> 0; ){
                $scope.reportData.push({
                    y: $filter('date')($scope.dashboardData.DashboardSaleReport[i].Date, 'MM/dd')+'\n Shift '+$scope.dashboardData.DashboardSaleReport[i].ShiftCode,
                    a: $scope.dashboardData.DashboardSaleReport[i].TotalSale,
                    b: $scope.dashboardData.DashboardSaleReport[i].CashPaid,
                });
            }
            // for(var i=$scope.dashboardData.DashboardSaleReport.length; i<$scope.dashboardData.DashboardSaleReport.length;i--){
            //     $scope.reportData.push({
            //         y: $filter('date')($scope.dashboardData.DashboardSaleReport[i].Date, 'MM/dd')+'\n Shift '+$scope.dashboardData.DashboardSaleReport[i].ShiftCode,
            //         a: $scope.dashboardData.DashboardSaleReport[i].TotalSale,
            //         b: $scope.dashboardData.DashboardSaleReport[i].CashPaid,
            //     });
            // }
        }
        

        Morris.Bar({
            element: 'dashboard-bar-1',
            data: $scope.reportData,
            xkey: 'y',
            ykeys: ['a', 'b'],
            labels: ['Total Sale', 'CashPaid'],
            barColors: ['#33414E', '#1caf9a'],
            gridTextSize: '10px',
            hideHover: true,
            resize: true,
            gridLineColor: '#E5E5E5'
        });

       
    }
    $scope.gasGraph = function(){
        if($scope.gasdashboardData.SaleGraph.length>0){
            for(var i=0; i<$scope.gasdashboardData.SaleGraph.length;i++){
                $scope.gasreportData.push({
                    y: $filter('date')($scope.gasdashboardData.SaleGraph[i].SaleDate, 'MM/dd')+'\n Shift '+$scope.gasdashboardData.SaleGraph[i].ShiftCode,
                    a: $scope.gasdashboardData.SaleGraph[i].SaleAmount,                   
                });
            }
        }

         Morris.Bar({
            element: 'dashboard-bar-2',
            data: $scope.gasreportData,
            xkey: 'y',
            ykeys: ['a'],
            labels: ['Sale Amount'],
            barColors: ['#33414E'],
            gridTextSize: '10px',
            hideHover: true,
            resize: true,
            gridLineColor: '#E5E5E5'
        });

    };

    // Prasad
    $scope.getGasDashboardProfitLossData = function () {
        var postData = {};
        postData.StoreID = $rootScope.StoreID;
        dashboardService.getGasDashboardProfitLossReport(postData).success(function (response) {
            $scope.gasDashboardProfitLossData = response;
            /*
            $scope.tableParams = new NgTableParams({
                sorting: { Date: "asc", OpenQty: "asc", InwardQty: "asc", SaleQty: "asc", ClosingQty: "asc", SystemClosingQty: "asc", ShortOver: "asc", SalePrice: "asc", PurchasePrice: "asc", ProfitLoss: "asc" }
            },{
              dataset: $scope.gasProfitLossData
            });
            */
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }

    var storeID = $rootScope.StoreID;

    $scope.getDasboardData();
    $scope.getGasDasboardData();
    $scope.getGasDashboardProfitLossData();
}]);