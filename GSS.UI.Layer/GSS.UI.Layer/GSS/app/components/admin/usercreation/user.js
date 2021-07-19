angular.module('GasApp').controller("userController", ['$scope', '$rootScope', '$filter', 'adminService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, adminService, NgTableParams, Excel, $timeout, httpPreConfig) {
    

    $scope.GroupID = $rootScope.GroupID;
    $scope.save = true;
    $scope.update = false;
    $scope.user = {
         "GroupID" :  $scope.GroupID,
         "StoreID" : "",
         "UserName" : "",
         "Password" : "",
         "AccessType" : "",
         "CreatedUserName" : $rootScope.UserName,
         "ModifiedUserName" : $rootScope.UserName       
    };
   
    $scope.userData = [];
    $scope.stores = [];
    $scope.user.ActiveStatus = 'A';
    $scope.clear = function () {
        $scope.user = {};
        $scope.adduserForm.$submitted = false;
    };

    var searchData = {};
    searchData.SearchKey = '0';
    searchData.Searchvalue = $rootScope.GroupID;  


    var getStoreUsers = function(){
        adminService.getUsers(searchData).success(function (response) {
            $scope.userData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { StoreName: "asc", UserName: "asc" }
            }, {
                dataset: $scope.userData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }

    adminService.getStores(searchData).success(function (response) {
        $scope.stores = response;
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
    });

    $scope.userreset = function () {
        $scope.user = {};
        $scope.adduserForm.$submitted = false;
    };

     $scope.getSelectedRow = function (row) {
        console.log(row);
        if(row){
            $scope.save = false;
            $scope.update = true; 
            $("#UserName").attr("readonly",true);
            //$scope.user.StoreID = row.StoreID;            
            $scope.user.StoreID = row.StoreID;
            $scope.currentStore = row.StoreID;
            $scope.user.UserName = row.UserName;
            $scope.user.AccessType = row.AccessType;
            $scope.user.ActiveStatus = row.ActiveStatus;  
        }
    };


    $scope.addNewUser = function () {
        if ($scope.adduserForm.$invalid) {
             sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            var UserPostData = angular.toJson($scope.user);
            UserPostData = JSON.parse(UserPostData);
            adminService.saveUser(UserPostData).success(function (response) {  
                sweetAlert("Success", "User added successfully", "success");
                getStoreUsers();
            }).error(function (response) {
                sweetAlert("Error",response, "error");
            });
        }       
        
    };

    $scope.updateUser = function () {
        if ($scope.adduserForm.$invalid) {
             sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            var UserPostData = angular.toJson($scope.user);
            UserPostData = JSON.parse(UserPostData);
            adminService.updateUser(UserPostData).success(function (response) {
                sweetAlert("Success", "User Updated successfully", "success");
                getStoreUsers();
            }).error(function (response) {
                sweetAlert("Error",response, "error");
            });
        }       
        
    };
    getStoreUsers();
}]);