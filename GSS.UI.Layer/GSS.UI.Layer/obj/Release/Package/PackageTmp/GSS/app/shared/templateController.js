angular.module('GasApp').controller('TemplateController', function ($scope,$rootScope,loginService,$location) {
	$scope.loading = false;
	//$scope.Storename= $rootScope.UserName;
	$scope.address = ($rootScope.StoreAdd1 != "null") ? $rootScope.StoreAdd1 : '';
	$scope.address += ($rootScope.StoreAdd2 != "null") ? $rootScope.StoreAdd2 : '';
	//$scope.StoreName1 = $rootScope.StoreName;
	$scope.oneAtATime = true;

	// if ($rootScope.AvailBusiness == "Y") { 
	// 	$scope.AvailBusiness = true; 
	// } else if ($rootScope.AvailBusiness == "N") { 
	// 	$scope.AvailBusiness = false; 
	// };
	// if ($rootScope.AvailGas == "Y") {
	// 	$scope.AvailGas = true; 
	// } else if ($rootScope.AvailGas == "N") { 
	// 	$scope.AvailGas = false; 
	// };
	// if ($rootScope.AvailLottery == "Y") { 
	// 	$scope.AvailLottery = true; 
	// } else if ($rootScope.AvailLottery == "N") {
	// 	$scope.AvailLottery = false; 
	// };

	// if($rootScope.AccessType == "SUPER_USER"){
	// 	$scope.AvailAdmin = true;
	// 	$scope.AvailSetup = true;
	// 	$scope.AvailReports = true;
	// }else if($rootScope.AccessType == "STORE_ADMIN"){
	// 	$scope.AvailAdmin = false;
	// 	$scope.AvailSetup = true;
	// 	$scope.AvailReports = true;
	// }else if($rootScope.AccessType == "STORE_USER"){
	// 	$scope.AvailAdmin = false;
	// 	$scope.AvailSetup = false;
	// 	$scope.AvailReports = true;
	// }
	
	// $scope.getShiftCode = function(code){
	// 	$rootScope.ShiftCode = code;
	// }

	// $scope.getDaySaleDetails = function(myDate,ShiftCode){
	// 	console.log(myDate);
	// 	console.log(ShiftCode);

	// }
	$scope.status = {
	    isCustomHeaderOpen: false,
	    isFirstOpen: true,
	    isFirstDisabled: false
	  };
	 $scope.logout = function () {
        loginService.logout();
    }

})