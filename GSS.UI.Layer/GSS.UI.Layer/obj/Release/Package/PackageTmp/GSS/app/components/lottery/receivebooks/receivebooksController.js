angular.module('GasApp').controller("ReceivebooksController", ['$scope', '$rootScope', '$filter', 'lotteryService', function ($scope, $rootScope, $filter, lotteryService) {   
    $scope.date = new Date();
    var allGames = [];
    $scope.gamesAdded = [];
    $scope.todayAddedGames = [];
    $scope.shifts = [];
    var storeID = $rootScope.StoreID;

    $scope.loading = true;
    $scope.hideBelowTable = true;    
    ////Getting Date value from tabs. 
    //if (!angular.isUndefined($rootScope.changedDate)) {
    //    $scope.myDate = $rootScope.changedDate;
    //} else {
    //    $scope.myDate = new Date();
    //    $scope.myDate.setDate($scope.date.getDate() - 1);
    //}

    //Add Date & shift details to the Header bar
    lotteryService.getRunningShift(storeID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "Error in Getting Running Shift Details", "error");
        $scope.loading = false;
    });


    //Checking Store Type
    var getType = function () {
        if ($rootScope.StoreType == "GS") {
            $scope.storeType = true;
        } else if ($rootScope.StoreType == "LS") {
            $scope.storeType = false;
        }
    }

    getType();
    //Getting Shifts Details for the store
    lotteryService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "Error in getting Shifts", "error");
        $scope.loading = false;
    });   

    $scope.getAllGames = function () {
        var getGamesData = lotteryService.getAllGames(storeID);
        getGamesData.then(function (game) {
            allGames = game.data;
        }, function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
    $scope.getAllGames();

    //Get Day Wise Details
    $scope.getDaySaleDetails = function (selectedDate, ShiftCode) {
        var postData = {};
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        // postData.RequestType = 'GAS-STORE-SALE';
        // postData.ShiftCode = ShiftCode;
        //Calling getPreviousDayTranscations function to get Day sale data.
        lotteryService.getLotteryReceive(postData).success(function (response) {
            if (response.length <=0) {
                $scope.todayAddedGames = [];
            } else {
                if (response.length > 0) {
                    $scope.todayAddedGames = [];
                    $scope.hideBelowTable = false;
                    for (i = 0; i < response.length; i++) {
                        $scope.todayAddedGames.push({
                            GameID: response[i].GameID,
                            PackNo: response[i].PackNo,
                            NoOfTickets: response[i].NoOfTickets,
                            GameName: response[i].GameName,
                        });
                    }
                } else {
                    $scope.todayAddedGames = [];
                }
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };

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
                                result.NoOfTickets = allGames[i].NoOfTickets;
                                result.GameName = allGames[i].GameName;
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
                            result.NoOfTickets = allGames[i].NoOfTickets;
                            result.GameName = allGames[i].GameName;
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

    $scope.checkAddedObject = function(gameID,packNo){
        var found = $scope.gamesAdded.some(function (el) {            
            return el.GameID === gameID && el.PackNo === packNo;
        });        
        return found;
    }

    $scope.removeReceivedBook = function (game) {
        var index = $scope.gamesAdded.indexOf(game);
        $scope.gamesAdded.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
    }
    
    $scope.saveReceivedBooks = function () {
        $scope.receiveBooks = [];
        if($scope.myDate != null){
             $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                if ($scope.gamesAdded.length > 0) {
                    for (var i = 0; i < $scope.gamesAdded.length; i++) {
                        $scope.receiveBooks.push({
                            "StoreID": storeID,
                            "GameID": $scope.gamesAdded[i].GameID,
                            "PackNo": $scope.gamesAdded[i].PackNo,
                            "Date": $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                            "ShiftID": $scope.ShiftCode,
                            "CreatedUserName": $rootScope.UserName,
                            "ModifiedUserName": $rootScope.UserName
                        });
                    }
                    var ReceiveBooksPostData = angular.toJson($scope.receiveBooks);
                    ReceiveBooksPostData = JSON.parse(ReceiveBooksPostData);
                    lotteryService.saveReceivedBooks(ReceiveBooksPostData).success(function (response) {
                        sweetAlert("Success", "Recevie Books Saved Successfully", "success");
                        $scope.loading = false;
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                        $scope.receiveBooks = [];
                        $scope.loading = false;
                    });
                }
            } else {
                $scope.loading = false;
                sweetAlert("Error!!", 'Please Select Shift Code', "error");
            }
        } else {
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Date', "error");
        }
    }   
}]);