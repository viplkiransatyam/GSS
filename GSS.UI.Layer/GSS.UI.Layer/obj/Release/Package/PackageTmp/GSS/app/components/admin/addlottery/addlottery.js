angular.module('GasApp').controller("addlotteryController", ['$scope', '$rootScope', '$filter', 'adminService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, adminService, NgTableParams, Excel, $timeout, httpPreConfig) {
    

    $scope.StoreID = $rootScope.StoreID;
    $scope.lottery = {
        "StoreID" : "",
         "NoOfSlots" : null,
         "AutoSettleDays" : null,
         "Games" : [                        
          ] 
    };
   
    $scope.userData = [];
    $scope.stores = [];
    $scope.clear = function () {
        $scope.lottery = {};
        $scope.addlotteryForm.$submitted = false;
    };
     $scope.games = [];
    $scope.getLotteryGames = function (selectedId) { 
        if(selectedId){
            $scope.games = [];
            adminService.getLotteryGames(selectedId).success(function (result) {
                $scope.games = result;
                
            }).error(function () {
                sweetAlert("Error!!", result, "error");
            }); 
        }else{
           sweetAlert("Warning!!", "Please Select Store", "warning");
           $scope.games = [];
        }      
        
    }


    

    var getLotteries = function(){
        adminService.getLotteries($rootScope.GroupID).success(function (response) {
            $scope.userData = response;
            $scope.licenses = response.length;
            $scope.tableParams = new NgTableParams({
                sorting: { StoreName: "asc"}
            }, {
                dataset: $scope.userData
            });
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
   
    var searchData = {};
    searchData.SearchKey = '0';
    searchData.Searchvalue = $rootScope.GroupID;  

    adminService.getStores(searchData).success(function (response) {
        $scope.stores = response;
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
    });


    $scope.userreset = function () {
        $scope.lottery = {};
        $scope.addlotteryForm.$submitted = false;
    };

    // $scope.getSelectedRow = function (row) {
    //     console.log(row);
    //     if(row){
    //         $scope.save = false;
    //         $scope.update = true; 
    //         $("#UserName").attr("readonly",true);
    //         //$scope.user.StoreID = row.StoreID;            
    //         $scope.user.StoreID = row.StoreID;
    //         $scope.currentStore = row.StoreID;
    //         $scope.user.UserName = row.UserName;
    //         $scope.user.AccessType = row.AccessType;
    //         $scope.user.ActiveStatus = row.ActiveStatus;  
    //     }
    // };


    $scope.addLottery = function () {
        if ($scope.addlotteryForm.$invalid) {
             sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
        }else{
            if($scope.games.length<=0){
                sweetAlert('Warning!!',"Please select atleast on Lottery Game", 'warning');
            }else{
                 $scope.lottery.Games = [];
                for (var j = 0; j < $scope.games.length; j++) {
                    if($scope.games[j].ticked){
                        $scope.lottery.Games.push({"GameID":$scope.games[j].GameID});
                    }
                  
                }
                var LotteryPostData = angular.toJson($scope.lottery);
                LotteryPostData = JSON.parse(LotteryPostData);
                adminService.saveLottery(LotteryPostData).success(function (response) {  
                    sweetAlert("Success", "Lottery added successfully", "success");
                    getLotteries();
                }).error(function (response) {
                    sweetAlert("Error",response,"error");
                });
            }
            
        }       
        
    };

    // $scope.updateUser = function () {
    //     if ($scope.adduserForm.$invalid) {
    //          sweetAlert('Warning!!',"Please Enter all required fileds", 'warning');
    //     }else{
    //         var UserPostData = angular.toJson($scope.user);
    //         UserPostData = JSON.parse(UserPostData);
    //         adminService.updateUser(UserPostData).success(function (response) {
    //             sweetAlert("Success", "User Updated successfully", "success");
    //             location.reload();
    //         }).error(function (errorRes) {
    //             sweetAlert("Error",errorRes, "Something gone wrong.Please Try Again!!");
    //         });
    //     }       
        
    // };
    getLotteries();
}]);