angular.module('GasApp').controller("ReturnBooksSetupController", ['$scope', '$rootScope', '$filter', 'lotteryService','setupService', function ($scope, $rootScope, $filter, lotteryService, setupService) {
    $scope.date = new Date();
    var allGames = [];
    $scope.gamesAdded = [];
    $scope.returnGamesAdded = [];
    $scope.todayAddedGames = [];
    var storeID = $rootScope.StoreID;

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

    $scope.loading = true;
    $scope.hideBelowTable = true;

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
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
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
    

  
    $scope.addActivatedBook = function () {
        if (angular.isUndefined($scope.activatedGameNumber) || $scope.activatedGameNumber == "") {
            sweetAlert("Error!!", "Please Enter Book Serial No.", "error");
        } else {
            var fullGame = $scope.activatedGameNumber;
            var gameId = [];
            var postData = {};
            postData.StoreID = storeID;
            gameId[0] = fullGame.substring(0, 3);
            gameId[1] = fullGame.substring(3, 9);
            gameId[2] = fullGame.substring(9, 12);
            postData.ScanSerial = gameId[0] + "-" + gameId[1] + "-" + gameId[2];
            var hasObject = $scope.checkGamesObject(gameId[0]);
            if (!hasObject) {
                sweetAlert("Error!!", "Game ID not Available", "error");

            } else {
                if ($scope.returnGamesAdded.length > 0) {
                    var hasAddObject = $scope.checkAddedObject1(gameId[0], gameId[1]);
                    if (hasAddObject) {
                        sweetAlert("Error!!", "Game ID is Already added to the list!! Please add another GameID", "error");
                        $scope.activatedGameNumber = "";
                    } else {
                        lotteryService.getClosingTickets(postData).success(function (response) {
                            if (gameId[2] <= response.NoOfTickets) {
                                $scope.returnGamesAdded.push({
                                    GameID: response.GameID,
                                    PackNo: response.PackNo,
                                    NoOfTickets: response.NoOfTickets,
                                    GameName: response.GameName,
                                    BoxID: response.BoxNo,
                                    TicketValue: response.TicketValue,
                                    BookValue: response.BookValue,
                                    TicketStartNumber: response.TicketStartNumber,
                                    TicketEndNumber: response.TicketEndNumber,
                                    LastTicketClosing: gameId[2],
                                    ReturnFrom: 'A',
                                });
                            } else {
                                sweetAlert("Error!!", "Please Check Last Ticket No. with No. of Tickets", "error");
                                $scope.activatedGameNumber = "";
                            }
                        }).error(function (response) {
                            sweetAlert("Error!!", response, "error");
                        });
                    }
                } else {
                    lotteryService.getClosingTickets(postData).success(function (response) {
                        if (gameId[2] <= response.NoOfTickets) {
                            $scope.returnGamesAdded.push({
                                GameID: response.GameID,
                                PackNo: response.PackNo,
                                NoOfTickets: response.NoOfTickets,
                                GameName: response.GameName,
                                BoxID: response.BoxNo,
                                TicketValue: response.TicketValue,
                                BookValue: response.BookValue,
                                TicketStartNumber: response.TicketStartNumber,
                                TicketEndNumber: response.TicketEndNumber,
                                LastTicketClosing: gameId[2],
                                ReturnFrom: 'A',
                            });
                        } else {
                            sweetAlert("Error!!", "Please Check Last Ticket No. with No. of Tickets", "error");
                            $scope.activatedGameNumber = "";
                        }
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                    });
                }
            }
        }
    }

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
            if (!hasObject) {
                sweetAlert("Error!!", "Game ID not Available", "error");

            } else {
                if ($scope.gamesAdded.length > 0) {
                    var hasAddObject = $scope.checkAddedObject(gameId[0], gameId[1]);
                    if (hasAddObject) {
                        sweetAlert("Error!!", "Game ID is Already added to the list!! Please add another GameID", "error");
                        $scope.gameNumber = "";
                    } else {
                        var result = {};
                        for (var i = 0; i < allGames.length; i++) {
                            if (allGames[i].GameID == gameId[0]) {
                                result.GameID = gameId[0];
                                result.PackNo = gameId[1];
                                result.NoOfTickets = allGames[i].NoOfTickets;
                                result.GameName = allGames[i].GameName;
                                result.ReturnFrom = 'I';
                                $scope.gamesAdded.push(result);
                                $scope.gameNumber = "";
                                return;
                            }
                        }
                    }
                } else {
                    var result = {};
                    for (var i = 0; i < allGames.length; i++) {
                        if (allGames[i].GameID == gameId[0]) {
                            result.GameID = gameId[0];
                            result.PackNo = gameId[1];
                            result.NoOfTickets = allGames[i].NoOfTickets;
                            result.ReturnFrom = 'I';
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

    $scope.checkGamesObject = function (gameID) {
        var found = allGames.some(function (el) {
            return el.GameID === gameID;
        });
        return found;
    }

    $scope.checkAddedObject = function (gameID, packNo) {
        var found = $scope.gamesAdded.some(function (el) {
            return el.GameID === gameID && el.PackNo === packNo;
        });
        return found;
    }
    $scope.checkAddedObject1 = function (gameID, packNo) {
        var found = $scope.returnGamesAdded.some(function (el) {
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

    $scope.removeReceivedBook1 = function (game) {
        var index = $scope.returnGamesAdded.indexOf(game);
        $scope.returnGamesAdded.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
    }

    $scope.getLotteryReturns = function (myDate) {
        var postData = {};
        postData.StoreID = storeID;
        postData.Date = $filter('date')(myDate, 'dd-MMM-yyyy');
        lotteryService.getLotteryReturns(postData).success(function (response) {
            console.log(response);
            $scope.gamesAdded = response;
            $scope.returnGamesAdded = response;
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }

    $scope.saveReturnedBooks = function () {
        $scope.returnBooks = [];
        if ($scope.myDate != null) {
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                if ($scope.returnGamesAdded.length > 0) {
                    for (var i = 0; i < $scope.returnGamesAdded.length; i++) {
                        $scope.returnBooks.push({
                            "StoreID": storeID,
                            "Date": $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                            "ShiftID": $scope.ShiftCode,
                            "ReturnFrom": 'A',
                            "LastTicketClosing": $scope.returnGamesAdded[i].LastTicketClosing,
                            "GameID": $scope.returnGamesAdded[i].GameID,
                            "PackNo": $scope.returnGamesAdded[i].PackNo,
                            "NoOfTickets": $scope.returnGamesAdded[i].NoOfTickets,
                            "BoxID": $scope.returnGamesAdded[i].BoxNo,
                            "CreatedUserName": $rootScope.UserName,
                            "ModifiedUserName": $rootScope.UserName
                        });
                    }                    
                }
                if ($scope.gamesAdded.length > 0) {
                    for (var i = 0; i < $scope.gamesAdded.length; i++) {
                        $scope.returnBooks.push({
                            "StoreID": storeID,
                            "Date": $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                            "ShiftID": $scope.ShiftCode,
                            "ReturnFrom": 'I',
                            "GameID": $scope.gamesAdded[i].GameID,
                            "PackNo": $scope.gamesAdded[i].PackNo,
                            "NoOfTickets": $scope.gamesAdded[i].NoOfTickets,
                            "CreatedUserName": $rootScope.UserName,
                            "ModifiedUserName": $rootScope.UserName,
                        });
                    }
                }
                if ($scope.returnBooks.length > 0) {
                    var ReceiveBooksPostData = angular.toJson($scope.returnBooks);
                    ReceiveBooksPostData = JSON.parse(ReceiveBooksPostData);
                    console.log(ReceiveBooksPostData);
                    setupService.AddLotteryReturn(ReceiveBooksPostData).success(function (response) {
                        sweetAlert("Success", "Return Books Saved Successfully", "success");
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                        $scope.returnBooks = [];
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