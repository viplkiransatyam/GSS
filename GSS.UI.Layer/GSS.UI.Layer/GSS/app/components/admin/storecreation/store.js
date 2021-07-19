angular.module('GasApp').controller("storeController", ['$scope', '$rootScope', '$filter', 'adminService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, adminService, NgTableParams, Excel, $timeout, httpPreConfig) {
    

    $scope.GroupID = $rootScope.GroupID;

    $scope.store = {
         "GroupID" :  $scope.GroupID,
         "StoreID" : null,
         "StoreName" : "",
         "StoreAddress1" : "",
         "StoreAddress2" : "",
         "State" : "",
         "NoOfShifts" : "",
         "CreatedUserName" : $rootScope.UserName,
         "ModifiedUserName" : $rootScope.UserName
    };
   
    $scope.storeData = [];

    $scope.clear = function () {
        $scope.store = {};
        $scope.addstoreForm.$submitted = false;
    };

    var searchData = {};
    searchData.SearchKey = '0';
    searchData.Searchvalue = $rootScope.GroupID;   
    adminService.getStores(searchData).success(function (response) {
        $scope.storeData = response;
        console.log($scope.storeData);
        $scope.tableParams = new NgTableParams({
            sorting: { StoreName: "asc", State: "asc" }
        }, {
            dataset: $scope.storeData
        });
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
    });

    $scope.storereset = function () {
        $scope.store = {};
        $scope.addstoreForm.$submitted = false;
    };

    $scope.addNewStore = function () {

        if ($scope.addstoreForm.$invalid) {
             sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            var StorePostData = angular.toJson($scope.store);
            StorePostData = JSON.parse(StorePostData);

            adminService.saveStore(StorePostData).success(function (response) {
                var pushdata = {};
                pushdata.StoreName = $scope.store.StoreName;
                pushdata.State = $scope.store.State;

                $scope.storeData.push(pushdata);
                $scope.tableParams.reload();
                $scope.clear();
                sweetAlert("Success", "Store added successfully", "success");
            }).error(function (response) {
                sweetAlert("Error",response, "error");
            });
        }       
        
    };

}]);