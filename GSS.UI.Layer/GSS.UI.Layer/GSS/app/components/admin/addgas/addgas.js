angular.module('GasApp').controller("addgasController", ['$scope', '$rootScope', '$filter', 'adminService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, adminService, NgTableParams, Excel, $timeout, httpPreConfig) {
    

    $scope.StoreID = $rootScope.StoreID;
    $scope.GroupID = $rootScope.GroupID;

     $scope.gas = {
        "GroupID": $scope.GroupID,
        "StoreID": "",
        "GasTypeID": "",
        "GasTypeName": "",
        "GasOilFormula": "",
        "StockPrice": 0,
        "GasTankCapacity":"",
        "GasOilConsumption": [          
        ],
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": ""
    };
    $scope.stores = [];
    $scope.gases = [];
    $scope.gasData = [];
    $scope.midgrade = true;
    var searchData = {};
    searchData.SearchKey = '0';
    searchData.Searchvalue = $rootScope.GroupID;  

    adminService.getStores(searchData).success(function (response) {
        $scope.stores = response;
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
    });


    var getGases = function(){
        adminService.getGases().success(function (result) {           
            $scope.gases = result;            
        }).error(function () {
            sweetAlert("Error!!", result, "error");
            $scope.gases =[];
        }); 
    }

    $scope.clear = function () {
        $scope.gas = {};
        $scope.addgasForm.$submitted = false;
    };

     $scope.userreset = function () {
        $scope.gas = {};
        $scope.premium = null;
        $scope.unlead = null;
        $scope.addgasForm.$submitted = false;
    };

    $scope.getGasType = function(gasTypeID){
        if(gasTypeID == 2)
            $scope.midgrade = false;
        else
            $scope.midgrade = true;
    }



   var getGroupGases = function(){
        adminService.getGroupGases($rootScope.GroupID).success(function (response) {
            $scope.gasData = response;
            $scope.licenses = response.length;
            $scope.tableParams = new NgTableParams({
                sorting: { StoreName: "asc",GasTypeName:"asc",GasTankCapacity:"asc"}
            }, {
                dataset: $scope.gasData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
   $scope.getPercentage1 = function(premium){
        if(premium == "" || angular.isUndefined(premium) || premium == null)
            $scope.premium = 0;
        else
            $scope.unlead = 100 - premium;
   }

   $scope.getPercentage2 = function(unlead){
        if(unlead == "" || angular.isUndefined(unlead) || unlead == null)
            $scope.unlead = 0;
        else
            $scope.premium = 100 - unlead;
   }

   $scope.addGas = function(){
        if ($scope.addgasForm.$invalid) {
             sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            if($scope.gas.GasTypeID == 2){
                $scope.gas.GasOilConsumption = [
                    {
                    "GasTypeID": 3,
                    "GasOilConsmptPercent": $scope.premium
                    },
                    {
                    "GasTypeID": 1,
                    "GasOilConsmptPercent": $scope.unlead
                    }
                ]; 
            }else{
                 $scope.gas.GasOilConsumption = [
                    {
                        "GasTypeID": $scope.gas.GasTypeID,
                        "GasOilConsmptPercent": 100
                    },
                 ];
            }
            var GasPostData = angular.toJson($scope.gas);
            GasPostData = JSON.parse(GasPostData);
            adminService.saveGasType(GasPostData).success(function (response) {  
                sweetAlert("Success", "Gas Type added successfully", "success");
                getGroupGases();
            }).error(function (response) {
                sweetAlert("Error",response,"error");
            });         
        }  
   }

   $scope.editItem = function(item){
        if(item.GasTypeID == 2){
            $scope.midgrade = false;
           $scope.premium = item.GasOilConsumption[0].GasOilConsmptPercent;
           $scope.unlead = item.GasOilConsumption[1].GasOilConsmptPercent;
        }else{
            $scope.midgrade = true;
        }

        $('#StoreID').val(item.StoreID);
        $('#GasTypeID').val(item.GasTypeID);
        $scope.gas.StoreID = $('#StoreID').val();
        $scope.gas.GasTypeID = $('#GasTypeID').val();
        $scope.gas.GasTankCapacity = item.GasTankCapacity;
    }
      
    getGases();
    getGroupGases();


}]);