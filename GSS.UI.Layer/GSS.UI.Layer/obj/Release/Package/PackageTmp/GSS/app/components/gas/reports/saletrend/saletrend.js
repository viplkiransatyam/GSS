angular.module('GasApp').controller("SaleTrendController", ['$scope', '$rootScope', '$filter', 'gasService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, gasService, NgTableParams, Excel, $timeout) {

    $scope.month = new Date();
    $scope.reportData = [];
    //Month Year Based Days
    function daysInMonth(month, year) {
        return new Date(year, month, 0).getDate();
    }
    var dayscount = daysInMonth(($scope.month.getMonth() + 1), $scope.month.getFullYear());
    var monthNames = ["January", "February", "March", "April", "May", "June",
                      "July", "August", "September", "October", "November", "December"
    ];
    var days = [];
    var endday = null;
    for (var i = 1; i <= dayscount; i++) {
        days.push(i);
        endday = i;
    }
 

    var ctx1 = document.getElementById("myBarChart").getContext("2d");

    $scope.GetGasSaleTrend = function (month)
    {
        var postData={StoreID : $rootScope.StoreID,
            Date:$filter('date')($scope.month, 'dd-MMM-yyyy')
        };
        gasService.getGasSaleTrend(postData).success(function (response) {  

            console.log(response);
            var xAxis = [];
            var yAxis = [];
            for (var i = 0; i < response.length; i++)
            {
                xAxis.push(response[i].GasOilType);
                yAxis.push(response[i].Sale);                        
            }
            var data = {
                labels: xAxis,
                datasets: [
                    {
                        label: "Total Sale",
                        fill: false,
                        backgroundColor: "rgb(0, 204, 204)",
                        borderColor: "rgb(0, 204, 204)",
                        borderCapStyle: 'butt',
                        borderJoinStyle: 'miter',
                        pointBorderColor: "rgb(0, 204, 204)",
                        pointBackgroundColor: "#fff",
                        pointBorderWidth: 1,
                        pointHoverRadius: 5,
                        pointHoverBackgroundColor: "rgb(0, 204, 204)",
                        pointHoverBorderColor: "rgb(0, 204, 204)",
                        pointHoverBorderWidth: 2,
                        pointRadius: 2,
                        pointHitRadius: 10,
                        data: yAxis,
                    },                           
                ]
            };
            var myBarChart = new Chart(ctx1, {
                type: 'bar',
                data: data,
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:true
                            }
                        }]
                    },
                    responsive: true,
                    legend: {
                        display: true,
                        labels: {
                            fontColor: 'rgb(255, 99, 132)'
                        }
                    },
                }
            });
            myBarChart.update(); 
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });      
    };
    
    $scope.GetGasSaleTrend($scope.month);   

}]);