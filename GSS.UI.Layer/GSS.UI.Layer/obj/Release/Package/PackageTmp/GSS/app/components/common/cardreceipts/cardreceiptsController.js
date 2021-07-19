angular.module('GasApp').controller('CardReceiptsController',['$scope', '$http', '$filter', '$rootScope', 'gasService', function ($scope, $http, $filter, $rootScope, gasService) {

	$scope.HasKey = null;
   	$scope.date = new Date();
   	var storeID = $rootScope.StoreID;
   	$scope.shifts = [];
   	$scope.GasReceipt = [];
    $scope.GasReceipt1 = [];
    $scope.cardInput = {}; 
    $scope.checkValidation = true;
    $scope.GassaveOnlineAmount = false;
    $scope.TotalSale = null;
	 //GasCards Object array sample
    $scope.gasCards = {
        "StoreID": $rootScope.StoreID,
        "Date": "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "ShiftCode":"",
        "GasReceipt": [
            {
                "CardTypeID": "",
                "CardAmount": "",
            }
        ]
    };

     //Getting Date value from tabs. 
    if (!angular.isUndefined($rootScope.changedDate)) {
        $scope.myDate = $rootScope.changedDate;
    } else {
        $scope.myDate = new Date();
        $scope.myDate.setDate($scope.myDate.getDate() - 1);
    }
	
    //Checking Store Type
	var getType = function(){
		if($rootScope.StoreType=="GS"){
			$scope.storeType = true;
		}else if($rootScope.StoreType=="LS"){
			$scope.storeType = false;
		}
	}
	
	getType();
    
    //Add Date & shift details to the Header bar
    gasService.getRunningShift(storeID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.ShiftCode = result.ShiftCode;
        $scope.SystemOpeningBalance = result.SystemOpeningBalance;
        
    }).error(function () {
        sweetAlert("Error!!", "Error in Getting Running Shift Details", "error");
    });

    //Getting Shifts Details for the store
    gasService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
    });

    //Getting as Cards Information pushing into GasReceipt Array
    gasService.getCardsPerStore(storeID).success(function (result) {
        for (var i = 0; i < result.length; i++) {
            $scope.GasReceipt.push({
                'CardTypeID': result[i].CardTypeID,
                'CardTypeName': result[i].CardTypeName
            });
        }
        //Assigning the GasReceipt array to  $scope.gasCards.GasReceipt object array
        $scope.gasCards.GasReceipt = $scope.GasReceipt;

    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        
    });

     //Get Day Wise Details
    $scope.getDaySaleDetails = function (selectedDate, ShiftCode) {
        var postData = {};
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        postData.RequestType = 'GAS-STORE-SALE';
        postData.ShiftCode = ShiftCode;        
        //Calling getPreviousDayTranscations function to get Day sale data.
        gasService.getPreviousDayTranscations(postData).success(function (response) {
        	console.log(response);
        	 if (response == "No Data Found") {
        	 	 for (i = 0; i < $scope.GasReceipt.length; i++) {
                    $scope.GasReceipt[i].CardAmount = "";
                }
                $scope.TotalSale = 0;
             	$scope.gasCards.CardTotal = 0;
                $scope.gasCards.Cash = 0;
                $scope.checkValidation = true;
        	 }else {
                //Making All Save buttons disable after clicking finaltransaction button in Bankdeposits form 
                if (response.EntryLockStatus == "Y") {
                    $scope.GassaveOnlineAmount = true;
                } else {
                    $scope.GassaveOnlineAmount = false;
                }
                //checking GasReceipt array to fill GasReceipt Section in Form
                if (response.GasReceipt.length > 0) {
                    $scope.GasReceipt1 = [];
                    for (j = 0; j < response.GasReceipt.length; j++) {
                        $scope.GasReceipt1.push({
                            CardTypeID: response.GasReceipt[j].CardTypeID,
                            CardTypeName: response.GasReceipt[j].CardName,
                            CardAmount: parseFloat(response.GasReceipt[j].CardAmount).toFixed(2),
                        });
                    }
                      //Card Section
                    $scope.gasCards.CardTotal = parseFloat(response.CardTotal).toFixed(2);
                    $scope.checkValidation = false;
                    $scope.GetTotalAmount($scope.GasReceipt1);
                } else {
                    $scope.GasReceipt1 = [];
                }               
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            
        });
    }

            //Calculating CardsTotal by calling getgasCardsTotal on onchange of Gasreciept text fields
    $scope.getgasCardsTotal = function (GasReceipt) {
        var total = 0;
        //Looping GasReceipt for card amounts
        for (var i = 0; i < GasReceipt.length; i++) {
            if (GasReceipt[i].CardAmount != "" && !angular.isUndefined(GasReceipt[i].CardAmount)) {
                $scope.gasCards.GasReceipt[i].CardAmount = parseFloat(GasReceipt[i].CardAmount).toFixed(2);
                total += parseFloat(GasReceipt[i].CardAmount);
            } else {
                $scope.gasCards.GasReceipt[i].CardAmount = "";
            }
        }
        //Filling Cards Total Amount
        $scope.gasCards.CardTotal = total.toFixed(2);
        $scope.gasCards.Cash = ($scope.SystemOpeningBalance - $scope.gasCards.CardTotal).toFixed(2);
        // //Checking for Total Sale Amount and calculating Cash amount
        // if ($scope.gasStation.TotalSale) {
        //     $scope.gasCards.Cash = ($scope.gasStation.TotalSale - $scope.gasCards.CardTotal).toFixed(2);
        // } else {
        //     $scope.gasStation.TotalSale = 0;
        //     sweetAlert('Warning!!', 'Please check the Sale section Amounts', 'warning');
        //     $scope.gasCards.Cash = ($scope.gasStation.TotalSale - $scope.gasCards.CardTotal).toFixed(2);
        // }
    }

        //Save BankCards BY POST
    $scope.saveOnlineAmount = function () {
        $scope.gasCards.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
        $scope.gasCards.ShiftCode = $scope.ShiftCode;
        console.log($scope.gasCards);
        if($scope.gasCards.ShiftCode != "" && $scope.gasCards.ShiftCode != null && !angular.isUndefined($scope.gasCards.ShiftCode)){
            $scope.loading = true;
            var GasCardsPostData = angular.toJson($scope.gasCards);
            GasCardsPostData = JSON.parse(GasCardsPostData);
            gasService.saveGasCardsData(GasCardsPostData).success(function (response) {                
                sweetAlert("Success", "Gas Cards Amount Saved Successfully", "success");
            }).error(function (response) {
                
                sweetAlert("Error!!", response, "error");
            });
        }else{
            
            sweetAlert("Error!!", 'Please Select Shift Code', "error");                        
        }
       
    }

     //Amount Decimals
    $scope.getDecimals = function () {
        if ($scope.cardInput.CardAmount != null && $scope.cardInput.CardAmount != "" && $scope.cardInput.CardAmount != "undefined") {
            $scope.cardInput.CardAmount = parseFloat($scope.cardInput.CardAmount).toFixed(2);
        }
    };

     //Adding Cards
    $scope.addCard = function (event) {
        console.log(event);
        if (angular.isUndefined(event.CardTypeName) || event.CardTypeName == null) {
            sweetAlert("Error!!", "Please Select Card Name.", "error");
        } else if (angular.isUndefined(event.CardAmount) || event.CardAmount == null) {
            sweetAlert("Error!!", "Please Enter Amount", "error");
        }else{
            if ($scope.HasKey === null) {
                $scope.GasReceipt1.push({                   
                    CardTypeID: event.CardTypeName,                   
                    CardTypeName: $("#CardTypeName option:selected").text(),                   
                    CardAmount: parseFloat(event.CardAmount).toFixed(2),
                }); 
               $scope.GetTotalAmount($scope.GasReceipt1);
            } else {
                for (i = 0; $scope.GasReceipt1.length > i; i++) {
                    if ($scope.HasKey === $scope.GasReceipt1[i].$$hashKey) {                       
                        $scope.GasReceipt1[i].CardTypeID = $scope.cardInput.CardTypeName;
                        $scope.GasReceipt1[i].CardTypeName = $("#CardTypeName option:selected").text();
                        $scope.GasReceipt1[i].CardAmount = parseFloat($scope.cardInput.CardAmount).toFixed(2);
                        $scope.HasKey = null;
                        break;
                    }
                }
                $scope.GetTotalAmount($scope.GasReceipt1);
                
            }
             //Assigning the GasReceipt array to  $scope.gasCards.GasReceipt object array
            $scope.gasCards.GasReceipt = $scope.GasReceipt1;
            $scope.clearInputs1();
        }
    };

 
     //Remove Cards
     $scope.RemoveCard = function (items) {
        for (i = 0; $scope.GasReceipt1.length > i; i++) {
            if (items.$$hashKey === $scope.GasReceipt1[i].$$hashKey) {
                $scope.GasReceipt1.splice(i, 1);
                sweetAlert("Deleted", 'Item deleted from the list', "success");
                break;
            }
        }
        $scope.clearInputs1();
        $scope.HasKey = null;
    }
    //Clear Fields
    $scope.clearInputs1 = function () {
        $scope.cardInput.CardTypeID = null;
        $scope.cardInput.CardTypeName = null;
        $scope.cardInput.CardAmount = null;
        $scope.GetTotalAmount($scope.GasReceipt1);
    };

     //Calclating Totals
    $scope.GetTotalAmount = function (GasReceipt1) {
        var total = 0;
        for (var i = 0; i < GasReceipt1.length; i++) {
            total += parseFloat(GasReceipt1[i].CardAmount);
        }
        $scope.gasCards.CardTotal = parseFloat(total).toFixed(2);
        $scope.gasCards.Cash = ($scope.SystemOpeningBalance - $scope.gasCards.CardTotal).toFixed(2);
        //  //Checking for Total Sale Amount and calculating Cash amount
        // if ($scope.TotalSale !=null) {
        //     $scope.gasCards.Cash = ($scope.TotalSale - $scope.gasCards.CardTotal).toFixed(2);
        // } else {
        //     sweetAlert('Warning!!', 'Please Select ShiftCode and click Go', 'warning');
        //     $scope.GasReceipt1 = [];
        //     $scope.gasCards.CardTotal = 0;
        //     $scope.gasCards.Cash = 0;
        // }
    }

}]);