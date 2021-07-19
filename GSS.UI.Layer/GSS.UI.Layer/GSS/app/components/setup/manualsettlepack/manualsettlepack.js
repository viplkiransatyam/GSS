angular.module('GasApp').controller("ManualSettlePackController", ['$scope', '$rootScope', '$filter', 'setupService','lotteryService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, setupService, lotteryService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.date = new Date();
    var storeID = $rootScope.StoreID;
    var allGames = [];
    $scope.gamesAdded = [];
    $scope.getAllGames = function () {
        var getGamesData = lotteryService.getAllGames(storeID);
        getGamesData.then(function (game) {
            allGames = game.data;
        }, function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
    $scope.getAllGames();

    $scope.addBook = function () {
        if (angular.isUndefined($scope.gameNumber) || $scope.gameNumber == "") {
            sweetAlert("Error!!", "Please Enter Book Serial No.", "error");
        } else {
            var fullGame = $scope.gameNumber;
            var gameId = [];
            gameId[0] = fullGame.substring(0, 3);
            gameId[1] = fullGame.substring(3, 9);
            gameId[2] = fullGame.substring(9, 12);
            var hasObject = $scope.checkGamesObject(gameId[0]);
            if(!hasObject){
                sweetAlert("Error!!", "Game ID not Available", "error");
                $scope.gameNumber = "";               
            }else{
                if($scope.gamesAdded.length>0){
                    var hasAddObject = $scope.checkAddedObject(gameId[0],gameId[1]);
                    if(hasAddObject){
                        sweetAlert("Error!!", "Game ID is Already added to the list!! Please add another GameID", "error");
                        $scope.gameNumber = "";
                    }else{
                        var result = {};
                        for (var i = 0; i < allGames.length; i++) {
                            if(allGames[i].GameID == gameId[0]){
                                result.GameID = gameId[0];
                                result.PackNo = gameId[1];
                                result.BookAmount = allGames[i].BookValue;
                                $scope.gamesAdded.push(result);
                                $scope.gameNumber = "";
                                return;
                            }                        
                        }          
                    }
                }else{
                    var result = {};
                    for (var i = 0; i < allGames.length; i++) {
                        if(allGames[i].GameID == gameId[0]){
                            result.GameID = gameId[0];
                            result.PackNo = gameId[1];
                            result.BookAmount = allGames[i].BookValue;
                            $scope.gamesAdded.push(result);
                            $scope.gameNumber = "";
                            return;
                        }                        
                    }                    
                }                              
            }        
        } 
    }

    $scope.checkGamesObject = function(gameID){
        var found = allGames.some(function (el) {
            return el.GameID === gameID;
        });        
        return found;
    }

   $scope.SaveSettlePacks = function () {
        
        if ($scope.gamesAdded.length <= 0) {
            sweetAlert("Error!!", 'Please Enter atleast one book serial', "error");
        } else {            
            var SettlePacksData = angular.toJson($scope.gamesAdded);
            SettlePacksData = JSON.parse(SettlePacksData);
            for (var i = 0; i < SettlePacksData.length; i++)
            {
                SettlePacksData[i].StoreID = $rootScope.StoreID;
            }
            
            console.log(SettlePacksData);
            setupService.saveSettelPacks(SettlePacksData).success(function (response) {
                sweetAlert("Success", "Manual Settle Packs Saved Successfully", "success");
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }
    }
}]);