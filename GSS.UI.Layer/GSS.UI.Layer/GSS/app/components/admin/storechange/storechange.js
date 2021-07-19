angular.module('GasApp').controller("StoreChangeController", ['$scope', '$state', '$rootScope', '$filter', 'adminService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $state, $rootScope, $filter, adminService, NgTableParams, Excel, $timeout, httpPreConfig) {
    
    var searchData = {};
    searchData.SearchKey = '0';
    searchData.Searchvalue = $rootScope.GroupID;  

 
    adminService.getStores(searchData).success(function (response) {
        $scope.stores = response;
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
    });  
  

    $scope.changeStore = function () {
        if ($scope.changeStoreForm.$invalid) {
             sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            $scope.currentStore = $scope.user.StoreID;
            adminService.getStoreName($scope.currentStore).success(function (response) {
                $rootScope.StoreID = $scope.user.StoreID;
                $rootScope.StoreName = response.StoreName;
                $rootScope.AvailBusiness = response.AvailBusiness;
                $rootScope.AvailGas = response.AvailGas;
                $rootScope.AvailLottery = response.AvailLottery;
                sweetAlert('Info!!',"Store is Changed", 'info');
                $state.go('dashboard');
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }       
        
    };   
}]);