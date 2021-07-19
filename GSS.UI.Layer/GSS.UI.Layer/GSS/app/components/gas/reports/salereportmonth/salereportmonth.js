 function clickMe(a){
    var data = a.getAttribute('data-currentDate');  
    var scope = angular.element($("#outer")).scope();
    scope.$apply(function(){
        scope.getSaleData(data);
    })        
    $("#myModal").modal();
}

angular.module('GasApp').controller("GasSaleReportMonthController", ['$scope', '$rootScope', '$filter', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'moment', 'calendarConfig', function ($scope, $rootScope, $filter, gasService, NgTableParams, Excel, $timeout, moment, calendarConfig) {

    var storeID = $rootScope.StoreID;
    $scope.lotteryDetailedSalesReportData = []; 
        //These variables MUST be set as a minimum for the calendar to work
    $scope.calendarView = 'month';
    $scope.viewDate = new Date();


    //Month Year Based Days
    function daysInMonth(month, year) {
        return new Date(year, month, 0).getDate();
    }
    var dayscount = daysInMonth(($scope.viewDate.getMonth() + 1), $scope.viewDate.getFullYear());
    var monthNames = ["January", "February", "March", "April", "May", "June",
                      "July", "August", "September", "October", "November", "December"
    ];
    var days = [];
    var endday = null;
    for (var i = 1; i <= dayscount; i++) {
        days.push(i);
        endday = i;
    } 

    $scope.toggle = function ($event, field, event) {
        $event.preventDefault();
        $event.stopPropagation();
        event[field] = !event[field];
    };
   $scope.timespanClicked = function (date, cell) {

       if ($scope.calendarView === 'month') {
           if (($scope.cellIsOpen && moment(date).startOf('day').isSame(moment($scope.viewDate).startOf('day'))) || cell.events.length === 0 || !cell.inMonth) {
               $scope.cellIsOpen = false;
            } else {
               $scope.cellIsOpen = true;
               $scope.viewDate = date;
            }
        } else if ($scope.calendarView === 'year') {
            if (($scope.cellIsOpen && moment(date).startOf('month').isSame(moment($scope.viewDate).startOf('month'))) || cell.events.length === 0) {
                $scope.cellIsOpen = false;
            } else {
                $scope.cellIsOpen = true;
                $scope.viewDate = date;
            }
        }

    };

    

    //getting Sale report month data
    $scope.getLotterySalesMonth = function(){
         var postData = {};
            postData.StoreID = storeID;
            postData.Date = "1-" + monthNames[$scope.viewDate.getMonth()] + "-" + $scope.viewDate.getFullYear();
            gasService.getGasSalesMonth(postData).success(function (response) {
            var rdata = [];          
            $scope.events = [];
            rdata = response;
            for (var i = 0; i < response.length; i++) {              
                $scope.events.push({SaleAmount:response[i].SaleAmount,SaleGallons:response[i].SaleGallons,Profit:response[i].Profit,startsAt:$filter('date')(response[i].SaleDate, 'dd-MMM-yyyy')});
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    $scope.getSaleData = function(saledate){
        var postData = {};
        //var saleReportData = [];
        repSalefromDate = $filter('date')(new Date(saledate), 'dd-MMM-yyyy');

        postData.StoreID = storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSalefromDate;
        console.log(postData);
        gasService.getGasDailySalesMonth(postData).success(function (response) {
            console.log(response);
            if (response.length <= 0) {
                $scope.lotteryDetailedSalesReportData = [];
            } else {
                $scope.lotteryDetailedSalesReportData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { Date: "asc", ShiftCode: "asc", Unlead: "asc", Pack: "asc", MidGrade: "asc", Premium: "asc", Diesel: "asc", Kirosene: "asc", NonEthnol: "asc", Total: "asc", AmountSold: "asc"}
                }, {
                    dataset: $scope.lotteryDetailedSalesReportData
                });
                $scope.grandTotal = $scope.sum($scope.lotteryDetailedSalesReportData, 'AmountSold');
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }
    
    
   

   $scope.sum = function (items, prop) {
        return items.reduce(function (a, b) {
            return a + b[prop];
        }, 0);
    };


    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'LotteryDetailedSaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'LotteryDetailedSaleReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();

    }
   
    $scope.getLotterySalesMonth();
}]);




