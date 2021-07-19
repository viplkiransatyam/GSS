angular.module('GasApp').controller('LoginController',['$scope','$state','$cookies','$http','loginService',function ($scope,$state,$cookies,$http,loginService) { 
    $scope.login = {
        UserName: "",
        Password: ""
    };
    $scope.init = function () {        
        $scope.login.UserName = null;
        $scope.login.Password = null;
    }
    $scope.validateLogin = function (login) {
        $scope.loading = true; 
        loginService.login(login).success(function (response) {
            if (response.Status == "VALID"){
                //adding cookies
                $cookies.put('auth', response);
                $cookies.put('AccessType', response.AccessType);
                $cookies.put('GroupID', response.GroupID);
                $cookies.put('StoreID', response.StoreID);
                $cookies.put('UserName', response.UserName);
				$cookies.put('StoreType', response.StoreType);
				$cookies.put('Status', response.Status);
				$cookies.put('StoreAdd1', response.StoreAdd1);
				$cookies.put('StoreAdd2', response.StoreAdd2);                
                $cookies.put('AvailBusiness', response.AvailBusiness);
                $cookies.put('AvailGas', response.AvailGas);
                $cookies.put('AvailLottery', response.AvailLottery);
                $cookies.put('StoreName', response.StoreName);
                $scope.loading = false;                
                $state.go('dashboard');
            } else if (response.Status == "INVALID") {
                sweetAlert("Error!!", "Invalid User Details", "error");
                $scope.loading = false;
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }
}]);