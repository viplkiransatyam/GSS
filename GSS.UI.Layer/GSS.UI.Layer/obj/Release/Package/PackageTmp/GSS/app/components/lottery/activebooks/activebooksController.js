angular.module('GasApp').controller("ActivebooksController", ['$scope', '$rootScope', '$filter', 'lotteryService', function ($scope, $rootScope, $filter, lotteryService) {   
    $scope.date = new Date();
    $scope.shifts = [];
    $scope.gamesAdded = [];
    $scope.todayActivatedBooks = [];

    var allGames = [];
    var storeID = $rootScope.StoreID;


    $scope.activeBooks = {};

    $scope.loading = true;
    $scope.hideBelowTable = true;
    ////Getting Date value from tabs. 
    //if (!angular.isUndefined($rootScope.changedDate)) {
    //    $scope.myDate = $rootScope.changedDate;
    //} else {
    //    $scope.myDate = new Date();
    //    $scope.myDate.setDate($scope.myDate.getDate() - 1);
    //}

    //Add Date & shift details to the Header bar
    lotteryService.getRunningShift(storeID).success(function (result) {
        console.log(result);
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
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });   

    //Getting Box Details for the store
    lotteryService.getBoxes(storeID).success(function (result) {
        $scope.Boxes = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });
    //Getting All Games
    $scope.getAllGames = function () {
        var getGamesData = lotteryService.getAllGames(storeID);
        getGamesData.then(function (game) {
            allGames = game.data;
        }, function () {
            alert("ERROR");
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
        lotteryService.getLotteryActive(postData).success(function (response) {
            if (response <=0) {
                $scope.todayActivatedBooks = [];
            } else {
                if (response.length > 0) {
                    $scope.todayActivatedBooks = [];
                    $scope.hideBelowTable = false;
                    for (i = 0; i < response.length; i++) {
                        $scope.todayActivatedBooks.push({
                            GameID: response[i].GameID,
                            PackNo: response[i].PackNo,
                            NoOfTickets: response[i].NoOfTickets,
                            GameName: response[i].GameName,
                            BoxNo: response[i].BoxNo,
                        });
                    }
                } else {
                    $scope.todayActivatedBooks = [];
                }
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };

    $scope.addBox = function () {
        if (angular.isUndefined($scope.activeBooks.gameNumber) || $scope.activeBooks.gameNumber == "" || $scope.activeBooks.gameNumber == null) {
            sweetAlert("Error!!", "Please Enter Book Serial No.", "error");
        } else if (angular.isUndefined($scope.activeBooks.BoxNo) || $scope.activeBooks.BoxNo == null) {
            sweetAlert("Error!!", "Please Select Box No.", "error");
        } else {
            var gameId = [];
                var fullGame = $scope.activeBooks.gameNumber;
                var boxNo = $scope.activeBooks.BoxNo;
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
                            $scope.activeBooks.gameNumber = "";
                            $scope.activeBooks.BoxNo = null;
                        }else{
                            var result = {};
                            for (var i = 0; i < allGames.length; i++) {
                                if(allGames[i].GameID == gameId[0]){
                                    result.GameID = gameId[0];
                                    result.PackNo = gameId[1];
                                    result.NoOfTickets = allGames[i].NoOfTickets;
                                    result.GameName = allGames[i].GameName;
                                    result.BoxNo = $scope.activeBooks.BoxNo;
                                    $scope.gamesAdded.push(result);
                                    $scope.activeBooks.gameNumber = "";
                                    $scope.activeBooks.BoxNo = null;
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
                                result.BoxNo = $scope.activeBooks.BoxNo;
                                $scope.gamesAdded.push(result);
                                $scope.activeBooks.gameNumber = "";
                                $scope.activeBooks.BoxNo = null;
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
   
    $scope.removeActiveBook = function (game) {
        var index = $scope.gamesAdded.indexOf(game);
        $scope.gamesAdded.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
    }
    
    $scope.saveActiveBooks = function () {
        $scope.activeBooks = [];
        if ($scope.myDate != null) {
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                if ($scope.gamesAdded.length > 0) {
                    for (var i = 0; i < $scope.gamesAdded.length; i++) {
                        $scope.activeBooks.push({
                            "StoreID": storeID,
                            "GameID": $scope.gamesAdded[i].GameID,
                            "PackNo": $scope.gamesAdded[i].PackNo,
                            "Date": $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                            "ShiftID": $scope.ShiftCode,
                            "BoxNo": $scope.gamesAdded[i].BoxNo,
                            "CreatedUserName": $rootScope.UserName,
                            "ModifiedUserName": $rootScope.UserName
                        });
                    }                   
                    var ActiveBooksPostData = angular.toJson($scope.activeBooks);
                    ActiveBooksPostData = JSON.parse(ActiveBooksPostData);
                    lotteryService.saveActiveBooks(ActiveBooksPostData).success(function (response) {
                        sweetAlert("Success", "Active Books Saved Successfully", "success");
                        $scope.loading = false;
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                        $scope.activeBooks = [];
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