angular.module('GasApp').controller("UpdateCommController", ['$scope', '$rootScope', '$filter', 'setupService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, setupService, NgTableParams, Excel, $timeout, httpPreConfig) {
    

    $scope.StoreID = $rootScope.StoreID;

    $scope.comm = {
         "StoreID" : $scope.StoreID,
         "BusinessEndidngDate" : "",
         "LotteryOnlineCommission" : "",
         "LotteryInstantCommission" : "",
         "LotteryCashCommission" : "",
    };

    $scope.clear = function () {
        $scope.comm = {};
        $scope.updateCommForm.$submitted = false;
    };
   

    $scope.commreset = function () {
        $scope.store = {};
        $scope.updateCommForm.$submitted = false;
    };

    $scope.addCommission = function () {
        if ($scope.updateCommForm.$invalid) {
             swal('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            var CommPostData = angular.toJson($scope.comm);
            CommPostData = JSON.parse(CommPostData);
            setupService.updateComm(CommPostData).success(function (response) {            
                $scope.clear();
                swal("Success", "Commissions Updated successfully", "success");
            }).error(function (response) {
                $scope.clear();
                swal("Error",response, "error");
            });
        }       
        
    };

}]);