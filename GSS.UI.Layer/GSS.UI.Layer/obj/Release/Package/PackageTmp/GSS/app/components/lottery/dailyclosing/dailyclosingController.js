angular.module('GasApp').controller("DailyClosingController", ['$scope', '$rootScope', '$filter', 'lotteryService', function ($scope, $rootScope, $filter, lotteryService) {   
    $scope.date = new Date();
    var storeID = $rootScope.StoreID;
    //Getting Date value from tabs. 
    //if (!angular.isUndefined($rootScope.changedDate)) {
    //    $scope.myDate = $rootScope.changedDate;
    //} else {
    //    $scope.myDate = new Date();
    //    $scope.myDate.setDate($scope.myDate.getDate() - 1);
    //}
    $scope.booksactive = true;
    $scope.hidetable = true;
    $scope.boxes = [];
    $scope.dailyClosing = {
        "StoreID": "",
        "Date": "",
        "ShiftCode": "",
        "CreatedUserName": $rootScope.UserName,
        "ModifiedUserName": $rootScope.UserName,
        "LotteryReturn" : "",
        "LotterySale" : "",
        "LotteryBooksActive" :"",
        "LotteryOnline" : "",
        "LotteryCashInstantPaid" : "",
        "LotteryCashOnlinePaid" : "",
        "LotteryCashPhysicalOpeningBalance" : "",
        "LotteryCashPhysicalClosingBalance" : "",
        "LotteryClosingCount": [{
            "BoxID": "",
            "GameID": "",
            "PackNo": "",
            "LastTicketClosing": "",
        }]
    }



    //Add Date & shift details to the Header bar
    lotteryService.getRunningShift(storeID).success(function (result) {
       //console.log(result);
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.dailyClosing.LotteryCashPhysicalOpeningBalance = (result.SystemOpeningBalance != 0)?parseFloat(result.SystemOpeningBalance).toFixed(2):result.SystemOpeningBalance;
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

      

    $scope.shifts = [];
    $scope.LotteryClosingCount = [];
    $scope.dailyClosedBooks = [];
    $scope.dailyClosing.LotteryClosingCount = [];    
    $scope.loading = true;
    $scope.hideBelowTable = true;
    

    //Getting Shifts Details for the store
    lotteryService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });



    //Get Day Wise Details
    $scope.getDaySaleDetails = function (selectedDate, ShiftCode) {
        var postData = {};
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        postData.ShiftCode = $("#repeatSelect").val();
        //Calling getPreviousDayTranscations function to get Day sale data.
        lotteryService.getPreviousDayTranscations(postData).success(function (response) {
            if (response == "No Data Found") {
                $scope.dailyClosedBooks = [];                
            } else {
                 //Making All Save buttons disable after clicking finaltransaction button in Bankdeposits form 
                if (response.EntryLockStatus == "Y") {
                    $scope.dailyclosingsave = true;
                } else {
                    $scope.dailyclosingsave = false;
                }
                if (response.LotteryClosingCount.length > 0) {
                    $scope.dailyClosedBooks = [];
                    $scope.hideBelowTable = false;
                    for (i = 0; i < response.LotteryClosingCount.length; i++) {
                        $scope.dailyClosedBooks.push({
                            GameID: response.LotteryClosingCount[i].GameID,
                            PackNo: response.LotteryClosingCount[i].PackNo,
                            GameName: response.LotteryClosingCount[i].GameDescription,
                            BoxID: response.LotteryClosingCount[i].BoxID,
                        });
                    }                       
                } else {
                    $scope.dailyClosedBooks = [];
                }
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };

    $scope.addBook = function () {
        $scope.boxes = [];
        $scope.AllLotteryClosingCount = [];
        $scope.booksactive = true;
        if (angular.isUndefined($scope.dailyClosing.gameNumber)) {
            sweetAlert("Error!!", "Please Enter Book Serial No.", "error");
        }else {
            var LastTicketNo = [];
            var postData = {};
            var fullGame = $scope.dailyClosing.gameNumber;
            postData.StoreID = storeID;
            LastTicketNo[0] = fullGame.substring(0, 3);
            LastTicketNo[1] = fullGame.substring(3, 9);
            LastTicketNo[2] = fullGame.substring(9, 12);
            postData.ScanSerial = LastTicketNo[0]+"-"+LastTicketNo[1]+"-"+LastTicketNo[2];
           
            if($scope.LotteryClosingCount.length>0){
                var hasAddObject = $scope.checkAddedObject(LastTicketNo[0],LastTicketNo[1]);
                if(hasAddObject){
                    sweetAlert("Error!!", "Game ID is Already added to the list!! Please add another GameID", "error");
                    $scope.dailyClosing.gameNumber = "";
                }else{
                    lotteryService.getClosingTickets(postData).success(function (response) {
                        console.log(response);
                        if(LastTicketNo[2]<=response.NoOfTickets){
                            $scope.LotteryClosingCount.push({
                                GameID: response.GameID,
                                PackNo: response.PackNo,
                                NoOfTickets: response.NoOfTickets,
                                GameName: response.GameName,
                                BoxID: response.BoxNo,
                                PrevTicketNumber: response.PrevTicketNumber,
                                TicketValue: response.TicketValue,
                                BookValue: response.BookValue,
                                TicketStartNumber: response.TicketStartNumber,
                                TicketEndNumber: response.TicketEndNumber,
                                LastTicketClosing: LastTicketNo[2],
                            });
                        }else{
                            sweetAlert("Error!!", "Please Check Last Ticket No. with No. of Tickets", "error");
                            $scope.dailyClosing.gameNumber = "";
                        }
                        $scope.loading = false;
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                        $scope.loading = false;
                    });
                }
            }else{
                lotteryService.getClosingTickets(postData).success(function (response) {
                    //console.log(response);
                    if(LastTicketNo[2]<=response.NoOfTickets){
                        $scope.LotteryClosingCount.push({
                            GameID: response.GameID,
                            PackNo: response.PackNo,
                            NoOfTickets: response.NoOfTickets,
                            GameName: response.GameName,
                            BoxID: response.BoxNo,
                            PrevTicketNumber: response.PrevTicketNumber,
                            TicketValue: response.TicketValue,
                            BookValue: response.BookValue,
                            TicketStartNumber: response.TicketStartNumber,
                            TicketEndNumber: response.TicketEndNumber,
                            LastTicketClosing: LastTicketNo[2],
                        });
                    }else{
                        sweetAlert("Error!!", "Please Check Last Ticket No. with No. of Tickets", "error");
                        $scope.dailyClosing.gameNumber = "";
                    }
                    $scope.loading = false;
                }).error(function (response) {
                    sweetAlert("Error!!", response, "error");
                    $scope.loading = false;
                });
            }
        }
        $scope.dailyClosing.gameNumber = "";
    }

    $scope.checkAddedObject = function(gameID,packNo){
        var found = $scope.LotteryClosingCount.some(function (el) {            
            return el.GameID === gameID && el.PackNo === packNo;
        });        
        return found;
    }

    $scope.removeDailyClosing = function (game) {
        var index = $scope.LotteryClosingCount.indexOf(game);
        $scope.LotteryClosingCount.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }        
    }

    // $scope.getTicketsSold = function (LastTicketClosing) {        
    //         if ($scope.LotteryClosingCount.length > 0) {
    //             for (var i = 0; i < $scope.LotteryClosingCount.length; i++) {
    //                 if (LastTicketClosing != null && !angular.isUndefined(LastTicketClosing)) {
    //                     if (LastTicketClosing <= $scope.LotteryClosingCount[i].NoOfTickets && LastTicketClosing >=0) {
    //                         $scope.LotteryClosingCount[i].TotalTicketssold = $scope.LotteryClosingCount[i].NoOfTickets - $scope.LotteryClosingCount[i].LastTicketClosing;
    //                         $scope.dailyClosing.LotteryInstantSale = $scope.LotteryClosingCount[i].TotalTicketssold * $scope.LotteryClosingCount[i].TicketValue;
    //                         $scope.dailyClosing.LotteryInstantSale = parseFloat($scope.dailyClosing.LotteryInstantSale).toFixed(2);
    //                         $scope.dailyClosing.LotterySale = parseFloat(($scope.dailyClosing.LotteryOnline) ? parseFloat($scope.dailyClosing.LotteryOnline) : parseFloat(0) + parseFloat($scope.dailyClosing.LotteryInstantSale)).toFixed(2);                            
    //                         if ($scope.dailyClosing.LotteryReturn != null && $scope.dailyClosing.LotteryReturn != ""  && !angular.isUndefined($scope.dailyClosing.LotteryReturn)) { parseFloat($scope.dailyClosing.LotteryReturn); } else { $scope.dailyClosing.LotteryReturn = parseFloat(0).toFixed(2); }                            
    //                         $scope.dailyClosing.TotalCashAtCounter = parseFloat(parseFloat($scope.dailyClosing.LotterySale) - parseFloat($scope.dailyClosing.LotteryReturn)).toFixed(2);
    //                     } else {
    //                         sweetAlert("Error!!", "Please Enter above 0 and below Number of Tickets", "error");
    //                         $scope.LotteryClosingCount[i].LastTicketClosing = null;
    //                     }
    //                 } else {
    //                     $scope.LotteryClosingCount[i].TotalTicketssold = null;
    //                     $scope.LotteryClosingCount[i].LastTicketClosing = null;
    //                     $scope.dailyClosing.LotteryInstantSale = parseFloat(0).toFixed(2);
    //                     $scope.dailyClosing.LotterySale = parseFloat(0).toFixed(2);
    //                 }
    //             }
    //         }        
    // }

    // $scope.getTotalSale = function () {
    //     if ($scope.dailyClosing.LotteryOnline != null && !angular.isUndefined($scope.dailyClosing.LotteryOnline)) {
    //         for (var i = 0; i < $scope.LotteryClosingCount.length; i++) {                
    //             if ($scope.LotteryClosingCount[i].LastTicketClosing != null && !angular.isUndefined($scope.LotteryClosingCount[i].LastTicketClosing)) { $scope.LotteryClosingCount[i].TotalTicketssold = $scope.LotteryClosingCount[i].NoOfTickets - $scope.LotteryClosingCount[i].LastTicketClosing; } else { $scope.LotteryClosingCount[i].TotalTicketssold = 0;}
    //             $scope.dailyClosing.LotteryInstantSale = $scope.LotteryClosingCount[i].TotalTicketssold * $scope.LotteryClosingCount[i].TicketValue;
    //             $scope.dailyClosing.LotteryInstantSale = parseFloat($scope.dailyClosing.LotteryInstantSale).toFixed(2);
    //             $scope.dailyClosing.LotteryOnline = parseFloat($scope.dailyClosing.LotteryOnline).toFixed(2);
    //             $scope.dailyClosing.LotterySale = parseFloat(parseFloat($scope.dailyClosing.LotteryOnline) + parseFloat($scope.dailyClosing.LotteryInstantSale)).toFixed(2);
    //             if ($scope.dailyClosing.LotteryReturn != null && !angular.isUndefined($scope.dailyClosing.LotteryReturn)) { parseFloat($scope.dailyClosing.LotteryReturn); } else { $scope.dailyClosing.LotteryReturn = parseFloat(0).toFixed(2); }
    //             $scope.dailyClosing.TotalCashAtCounter = parseFloat(parseFloat($scope.dailyClosing.LotterySale) - parseFloat($scope.dailyClosing.LotteryReturn)).toFixed(2);
    //         }
    //     }
    // }

    // $scope.lotteryReturned = function () {
    //     if ($scope.dailyClosing.LotteryReturn != null && !angular.isUndefined($scope.dailyClosing.LotteryReturn)) {
    //         for (var i = 0; i < $scope.LotteryClosingCount.length; i++) {
    //             if ($scope.LotteryClosingCount[i].LastTicketClosing != null && !angular.isUndefined($scope.LotteryClosingCount[i].LastTicketClosing)) { $scope.LotteryClosingCount[i].TotalTicketssold = $scope.LotteryClosingCount[i].NoOfTickets - $scope.LotteryClosingCount[i].LastTicketClosing; } else { $scope.LotteryClosingCount[i].TotalTicketssold = 0; }
    //             $scope.dailyClosing.LotteryInstantSale = $scope.LotteryClosingCount[i].TotalTicketssold * $scope.LotteryClosingCount[i].TicketValue;
    //             $scope.dailyClosing.LotteryInstantSale = parseFloat($scope.dailyClosing.LotteryInstantSale).toFixed(2);
    //             if ($scope.dailyClosing.LotteryOnline != null && !angular.isUndefined($scope.dailyClosing.LotteryOnline)) { parseFloat($scope.dailyClosing.LotteryOnline); } else { $scope.dailyClosing.LotteryOnline = parseFloat(0).toFixed(2); }
    //             $scope.dailyClosing.LotteryOnline = parseFloat($scope.dailyClosing.LotteryOnline).toFixed(2);
    //             $scope.dailyClosing.LotteryReturn = parseFloat($scope.dailyClosing.LotteryReturn).toFixed(2);
    //             $scope.dailyClosing.LotterySale = parseFloat(parseFloat($scope.dailyClosing.LotteryOnline) + parseFloat($scope.dailyClosing.LotteryInstantSale)).toFixed(2);                
    //             $scope.dailyClosing.TotalCashAtCounter = parseFloat(parseFloat($scope.dailyClosing.LotterySale) - parseFloat($scope.dailyClosing.LotteryReturn)).toFixed(2);
    //         }           
    //     }
    // }
   
    $scope.cashInstantPaid = function () {
        if ($scope.dailyClosing.LotteryCashInstantPaid != null && !angular.isUndefined($scope.dailyClosing.LotteryCashInstantPaid) && $scope.dailyClosing.LotteryCashInstantPaid != "") {
            $scope.dailyClosing.LotteryCashInstantPaid = parseFloat($scope.dailyClosing.LotteryCashInstantPaid).toFixed(2); 
           getCalculations();
        }else{
        	getCalculations();
        }
    }
    $scope.cashOnlinePaid = function () {
        if ($scope.dailyClosing.LotteryCashOnlinePaid != null && !angular.isUndefined($scope.dailyClosing.LotteryCashOnlinePaid) && $scope.dailyClosing.LotteryCashOnlinePaid != "") {
            $scope.dailyClosing.LotteryCashOnlinePaid = parseFloat($scope.dailyClosing.LotteryCashOnlinePaid).toFixed(2);
            getCalculations();
        }else{
        	getCalculations();
        }
    }
    $scope.InstantComm = function () {        
        if ($scope.dailyClosing.LotteryCashPhysicalOpeningBalance != null && !angular.isUndefined($scope.dailyClosing.LotteryCashPhysicalOpeningBalance) && $scope.dailyClosing.LotteryCashPhysicalOpeningBalance != "") {
            $scope.dailyClosing.LotteryCashPhysicalOpeningBalance = parseFloat($scope.dailyClosing.LotteryCashPhysicalOpeningBalance).toFixed(2);
        }
    }
    $scope.OnlineComm = function () {
        if ($scope.dailyClosing.LotteryCashPhysicalClosingBalance != null && !angular.isUndefined($scope.dailyClosing.LotteryCashPhysicalClosingBalance) && $scope.dailyClosing.LotteryCashPhysicalClosingBalance != "") {
            $scope.dailyClosing.LotteryCashPhysicalClosingBalance = parseFloat($scope.dailyClosing.LotteryCashPhysicalClosingBalance).toFixed(2);
        }
    }
    $scope.LotteryOnline = function () {
        if ($scope.dailyClosing.LotteryOnline != null && !angular.isUndefined($scope.dailyClosing.LotteryOnline) && $scope.dailyClosing.LotteryOnline != "") {
            $scope.dailyClosing.LotteryOnline = parseFloat($scope.dailyClosing.LotteryOnline).toFixed(2); 
        	getCalculations();
        }else{
        	getCalculations();
        }
    }
    $scope.LotteryReturn = function () {
        if ($scope.dailyClosing.LotteryReturn != null && !angular.isUndefined($scope.dailyClosing.LotteryReturn) && $scope.dailyClosing.LotteryReturn != "") {
            $scope.dailyClosing.LotteryReturn = parseFloat($scope.dailyClosing.LotteryReturn).toFixed(2);
        }
    }

    $scope.cashAtCounter = function () {
        if ($scope.dailyClosing.LotteryCashPhysicalClosingBalance != null && !angular.isUndefined($scope.dailyClosing.LotteryCashPhysicalClosingBalance) && $scope.dailyClosing.LotteryCashPhysicalClosingBalance != "") {
            $scope.dailyClosing.LotteryCashPhysicalClosingBalance = parseFloat($scope.dailyClosing.LotteryCashPhysicalClosingBalance).toFixed(2);
        }
    }
    $scope.AllLotteryClosingCount = [];
    $scope.confirmInstantSale = function(){
    	$scope.ShiftCode = $("#repeatSelect").val();
        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
            // count=0;
            // if($scope.boxes.length>0){
            //     angular.forEach( $scope.boxes, function( value, idx ) {
            //          if ( value[ 'ticked' ] === true ) {
            //              count++;
            //          }
            //     });
            // }
            // if($scope.boxes.length == count){
                if($scope.boxes.length>0){
                    if(document.getElementById('boxcheck').checked) {
                        $scope.dailyClosing.ShiftCode = $scope.ShiftCode;
                        $scope.dailyClosing.StoreID = storeID;
                        $scope.dailyClosing.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');

                        if($scope.AllLotteryClosingCount.length>0)
                            $scope.dailyClosing.LotteryClosingCount = $scope.AllLotteryClosingCount;
                        else                
                            $scope.dailyClosing.LotteryClosingCount = $scope.LotteryClosingCount;

                        var DailyClosingPostData = angular.toJson($scope.dailyClosing);
                        DailyClosingPostData = JSON.parse(DailyClosingPostData);
                        lotteryService.confirmInstantSale(DailyClosingPostData).success(function (response) {
                            $scope.InstantSale = response.InstantSale;
                            $scope.CashTransfer = response.CashTransfer;
                            if(response.BoxNumbers.length>0){
                                $scope.boxes = response.BoxNumbers;                    
                                $scope.AllLotteryClosingCount = $scope.LotteryClosingCount.concat($scope.boxes);
                                $scope.booksactive = false;
                                $scope.hidetable = true;
                            }else{
                                $scope.booksactive = false;
                                $scope.hidetable = false;
                            }
                            getCalculations();                
                    
                        }).error(function (response) {
                            sweetAlert("Error!!", response, "error");
                            $scope.AllLotteryClosingCount = [];
                        });
                    } else {
                        sweetAlert("Error!!", "Please tick the Checkbox to select all empty boxes", "error");
                    }
                }else{
                        $scope.dailyClosing.ShiftCode = $scope.ShiftCode;
                        $scope.dailyClosing.StoreID = storeID;
                        $scope.dailyClosing.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');

                        if($scope.AllLotteryClosingCount.length>0)
                            $scope.dailyClosing.LotteryClosingCount = $scope.AllLotteryClosingCount;
                        else                
                            $scope.dailyClosing.LotteryClosingCount = $scope.LotteryClosingCount;

                        var DailyClosingPostData = angular.toJson($scope.dailyClosing);
                        DailyClosingPostData = JSON.parse(DailyClosingPostData);
                        lotteryService.confirmInstantSale(DailyClosingPostData).success(function (response) {
                            $scope.InstantSale = response.InstantSale;
                            $scope.CashTransfer = response.CashTransfer;
                            if(response.BoxNumbers.length>0){
                                $scope.boxes = response.BoxNumbers;                    
                                $scope.AllLotteryClosingCount = $scope.LotteryClosingCount.concat($scope.boxes);
                                $scope.booksactive = false;
                                $scope.hidetable = true;
                            }else{
                                $scope.booksactive = false;
                                $scope.hidetable = false;
                            }
                            getCalculations();                
                    
                        }).error(function (response) {
                            sweetAlert("Error!!", response, "error");
                            $scope.AllLotteryClosingCount = [];
                        });
                }                
            // }else{
            //      sweetAlert("Error!!", "Please Select all Boxes in the dropdown", "error");
            // }
        }else {
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        } 
    }
     var getCalculations = function(){
    	var lotteryonline = ($scope.dailyClosing.LotteryOnline)?$scope.dailyClosing.LotteryOnline:0;
    	var instantpaid = ($scope.dailyClosing.LotteryCashInstantPaid)?$scope.dailyClosing.LotteryCashInstantPaid:0;
    	var onlinepaid = ($scope.dailyClosing.LotteryCashOnlinePaid)?$scope.dailyClosing.LotteryCashOnlinePaid:0;
    	var instantSale = ($scope.InstantSale)?$scope.InstantSale:0;
    	var cashtransfer = ($scope.CashTransfer)?$scope.CashTransfer:0;

    	$scope.TotalSale = parseFloat(parseFloat(instantSale)+parseFloat(lotteryonline)).toFixed(2);
    	$scope.TotalPaid = parseFloat(parseFloat(cashtransfer)+parseFloat(instantpaid)+parseFloat(onlinepaid)).toFixed(2);

        $scope.LotteryCashPhysicalClosingBalance = parseFloat(parseFloat(parseFloat($scope.dailyClosing.LotteryCashPhysicalOpeningBalance)+parseFloat($scope.TotalSale))-parseFloat($scope.TotalPaid)).toFixed(2);
    	$scope.dailyClosing.LotteryCashPhysicalClosingBalance = parseFloat(parseFloat(parseFloat($scope.dailyClosing.LotteryCashPhysicalOpeningBalance)+parseFloat($scope.TotalSale))-parseFloat($scope.TotalPaid)).toFixed(2);

    }

    $scope.FinishDayClosing = function () {
        $scope.ShiftCode = $("#repeatSelect").val();
        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
            $scope.loading = true;
            $scope.dailyClosing.ShiftCode = $scope.ShiftCode;
            $scope.dailyClosing.StoreID = storeID;
            $scope.dailyClosing.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');  

            if($scope.AllLotteryClosingCount.length>0)
                $scope.dailyClosing.LotteryClosingCount = $scope.AllLotteryClosingCount;
            else                
                $scope.dailyClosing.LotteryClosingCount = $scope.LotteryClosingCount;

            
            var DailyClosingPostData = angular.toJson($scope.dailyClosing);
            DailyClosingPostData = JSON.parse(DailyClosingPostData);
            lotteryService.saveDailyClosing(DailyClosingPostData).success(function (response) {
                sweetAlert("Success", "Day Finalized Successfully", "success"); 
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        } else {
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        }        
    }

    $scope.fillUnsavedData = function(){
        lotteryService.getUnsaveddata(storeID).success(function (response) {
            if (response.length>0) {
                $scope.LotteryClosingCount = [];
                for(var i=0;i<response.length;i++){
                     $scope.LotteryClosingCount.push({
                        GameID: response[i].GameID,
                        PackNo: response[i].PackNo,
                        NoOfTickets: response[i].NoOfTickets,
                        GameName: response[i].GameDescription,
                        BoxID: response[i].BoxID,
                        PrevTicketNumber: response[i].PrevTicketNumber,
                        TicketValue: response[i].EachTicketValue,
                        LastTicketClosing: response[i].LastTicketClosing,
                    }); 
                 }                           
            }
            //console.log($scope.LotteryClosingCount);
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }
    getCalculations();
}]);
