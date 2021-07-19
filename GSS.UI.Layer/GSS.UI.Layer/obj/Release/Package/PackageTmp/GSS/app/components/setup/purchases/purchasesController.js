angular.module('GasApp').controller('BusinessPurchasesController', ['$scope', '$http', '$filter', '$rootScope', 'setupService', function ($scope, $http, $filter, $rootScope, setupService) {

    $scope.date = new Date();
    
    //Getting Date value from tabs. 
    if (!angular.isUndefined($rootScope.changedDate)) {
        $scope.myDate = $rootScope.changedDate;
    } else {
        $scope.myDate = new Date();        
        $scope.myDate.setDate($scope.myDate.getDate() - 1);
    }

    var getType = function () {
        if ($rootScope.StoreType == "GS") {
            $scope.storeType = true;
        } else if ($rootScope.StoreType == "LS") {
            $scope.storeType = false;
        }
    }
    getType();

    $scope.username = $rootScope.UserName;
    var storeID = $rootScope.StoreID;
    $scope.shifts = [];
    $scope.HasKey = null;
    $scope.PurchasesRepeat = {
        "StoreID": storeID,
        "Date" : "",
        "ShiftCode" : "",
        "CreatedUserName": $scope.username,
        "CreateTimeStamp" : "",
        "ModifiedUserName": $scope.username,
        "ModifiedTimeStamp" : "",
		"Purchase":[{
			"SupplierName": "",
			"SupplierNo": null,
			"InvCrdDate": "",
			"InvCrdNumber": "",
			"InvCrdAmount": null,
			"DueDate": "",
			"Remarks": "",
		}]
    };
    $scope.PurchaseReturnsRepeat = {
        "StoreID": storeID,
        "Date": "",
        "ShiftCode" : "",
        "CreatedUserName": $scope.username,
        "CreateTimeStamp": "",
        "ModifiedUserName": $scope.username,
        "ModifiedTimeStamp": "",
		"Purchase":[{
			"SupplierName": "",
			"SupplierNo": null,
			"InvCrdDate": "",
			"InvCrdNumber": "",
			"InvCrdAmount": null,
			"DueDate": "",
			"Remarks": "",
		}]
    };
    $scope.PurchasesRepeatInput = {};
    $scope.PurchaseReturnRepeatInput = {};
    $scope.PurchasesRepeat.Purchase = [];
    $scope.PurchaseReturnsRepeat.Purchase = [];

    
    //Add Date & shift details to the Header bar
    setupService.getRunningShift(storeID).success(function (result) {
       //console.log(result);
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "Error in Getting Running Shift Details", "error");
    });

    //Getting Shifts Details for the store
    setupService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });

    setupService.getAccountLedgers(storeID).success(function (result) {
        $scope.accounts = result;
    }).error(function () {
        sweetAlert("Error!!", result, "error");
        
    });
	
	
    var postData = {};
    storeID = $rootScope.StoreID;
    selectedDate = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
    postData.StoreID = storeID;
    postData.Date = selectedDate;

    var getdaysale = function (postData) {	
        var purchasesTotal = 0;
        var purchaseRetrunsTotal = 0;
    
        //calling getpreviousdaytranscations function to get day sale data.
        setupService.getPreviousDayTranscations(postData).success(function (response) {	
            if (response == "No Data Found") {
                $scope.PurchasesRepeat.Purchase = [];
                $scope.PurchaseReturnsRepeat.Purchase = [];
            }
            else {
				//$scope.PurchaseReturnsRepeat.Purchase = [];
				//$scope.PurchasesRepeat.Purchase = [];
                //making all save buttons disable after clicking finaltransaction button in bankdeposits form 
                if (response.entrylockstatus == "y") {
                    $scope.savepurchasesreturns = true;
                    $scope.savepurchases = true;               
                } else {
                    $scope.savepurchasesreturns = false;
                    $scope.savepurchases = false;               
                }
                //checking purchase array to fill gassale section in form
                if (response.Purchase.length > 0) {	
                 $scope.PurchasesRepeat.Purchase =[];				
                    for (i = 0; i < response.Purchase.length; i++) {                      
						$scope.PurchasesRepeat.Purchase.push({
							SupplierName: response.Purchase[i].SupplierName,
							SupplierNo: response.Purchase[i].SupplierNo,
							InvCrdDate: response.Purchase[i].InvCrdDate,
							InvCrdNumber: response.Purchase[i].InvCrdNumber,
							InvCrdAmount: response.Purchase[i].InvCrdAmount,
							DueDate: response.Purchase[i].DueDate,
							Remarks: response.Purchase[i].Remarks,
						});
						purchasesTotal = parseFloat(parseFloat(purchasesTotal) + parseFloat(response.Purchase[i].InvCrdAmount)).toFixed(2);
                    }
                } else {
                    $scope.PurchasesRepeat.Purchase = [];
                }
                //checking purchasereturn array to fill gassale section in form
                if (response.PurchaseReturn.length > 0) {
                    $scope.PurchaseReturnsRepeat.Purchase =[];
                    for (i = 0; i < response.PurchaseReturn.length; i++) {                        
						$scope.PurchaseReturnsRepeat.Purchase.push({
							SupplierName: response.PurchaseReturn[i].SupplierName,
							SupplierNo: response.PurchaseReturn[i].SupplierNo,
							InvCrdDate: response.PurchaseReturn[i].InvCrdDate,
							InvCrdNumber: response.PurchaseReturn[i].InvCrdNumber,
							InvCrdAmount: response.PurchaseReturn[i].InvCrdAmount,							
							Remarks: response.PurchaseReturn[i].Remarks,
						});
                        purchaseRetrunsTotal = parseFloat(parseFloat(purchaseRetrunsTotal) + parseFloat(response.PurchaseReturn[i].InvCrdAmount)).toFixed(2);
                    }					
                } else {
                    $scope.PurchaseReturnsRepeat.Purchase = [];
                }
				
				$scope.purchasesinvoiceTotalAmount = purchasesTotal;
				$scope.purchasereturnsinvoiceTotalAmount = purchaseRetrunsTotal;
            }
        }).error(function (response) {
            sweetAlert("error!!", response, "error");
            
        });
    }

    //getdaysale(postData);

  // getting day sale data after changing the date from datepicker
    var getsaledata = {};
    $scope.getDaySaleDetails = function (selecteddate) {        
        $rootScope.changedDate = $scope.myDate;
        StoreID = $rootScope.StoreID;
        selecteddate = $filter('date')(selecteddate, 'dd-MMM-yyyy');
        getsaledata.StoreID = StoreID;
        getsaledata.Date = selecteddate;
        //calling the below method to get day sale details for all the sections
        getdaysale(getsaledata);
    }


    $scope.amountChange = function (value) {        
        var numberformat = /^[0-9]+([,.][0-9]{0,2})?$/;
		if (!angular.isUndefined(value) && value != null && value != "") {
			if (!numberformat.test(String(value))) {
				sweetAlert("Invoice Amount!!", "Not a valid number", "error");
				$scope.PurchasesRepeatInput.InvCrdAmount = null;           
			} else {
				$scope.PurchasesRepeatInput.InvCrdAmount = parseFloat($scope.PurchasesRepeatInput.InvCrdAmount).toFixed(2);           
			}
		}
    };

    $scope.amountChange1 = function (value) {
        var numberformat = /^[0-9]+([,.][0-9]{0,2})?$/;
		if (!angular.isUndefined(value) && value != null && value != "") {
			if (!numberformat.test(String(value))) {
				sweetAlert("Invoice Amount!!", "Not a valid number", "error");           
				$scope.PurchaseReturnRepeatInput.InvCrdAmount = null;
			} else {            
				$scope.PurchaseReturnRepeatInput.InvCrdAmount = parseFloat($scope.PurchaseReturnRepeatInput.InvCrdAmount).toFixed(2);
			}			
		}
    };

    //Adding Purchases	
    $scope.addPurchases = function (event) {
        if (angular.isUndefined(event.SupplierNo) || event.SupplierNo == null) {
            sweetAlert("Error!!", "Please Select Vendor", "error");
            
        } else if (event.InvCrdDate == null) {
            sweetAlert("Error!!", "Please Choose Invoice Date", "error");
            
        } else if (angular.isUndefined(event.InvCrdNumber) || event.InvCrdNumber == null) {
            sweetAlert("Error!!", "Please fill Invoice Number", "error");
            
        } else if (angular.isUndefined(event.InvCrdAmount) || event.InvCrdAmount == null) {            
            sweetAlert("Error!!", "Please fill Invoice Amount", "error");
            
        } else {
                var purchasesTotal = 0;     
            if ($scope.HasKey === null) {
				if(angular.isUndefined($scope.purchasesinvoiceTotalAmount)){
					purchasesTotal = parseFloat(parseFloat(purchasesTotal) + parseFloat(event.InvCrdAmount)).toFixed(2);
				}else{
					purchasesTotal = parseFloat(parseFloat($scope.purchasesinvoiceTotalAmount) + parseFloat(event.InvCrdAmount)).toFixed(2);
				}				
                //if ($scope.hasDuplicate(event.SupplierNo) === false) {
                $scope.PurchasesRepeat.Purchase.push({
                    SupplierName: $("#SupplierNo option:selected").text(),
                    SupplierNo: event.SupplierNo,
                    InvCrdDate: $filter('date')(event.InvCrdDate, 'dd-MMM-yyyy'),
                    InvCrdNumber: event.InvCrdNumber,
                    InvCrdAmount: event.InvCrdAmount,
                    DueDate: $filter('date')(event.DueDate, 'dd-MMM-yyyy'),
                    Remarks: event.Remarks,
                });
                
				$scope.purchasesinvoiceTotalAmount = purchasesTotal;
                //    } else {
                //        sweetAlert("Message", 'This Item already in list', "warning");
                //    }
            } else {
				var purchasesTotal = 0;	
                for (i = 0; $scope.PurchasesRepeat.Purchase.length > i; i++) {
                    if ($scope.HasKey === $scope.PurchasesRepeat.Purchase[i].$$hashKey) {						
                        $scope.PurchasesRepeat.Purchase[i].SupplierName = $("#SupplierNo option:selected").text();
                        $scope.PurchasesRepeat.Purchase[i].SupplierNo = $scope.PurchasesRepeatInput.SupplierNo;
                        $scope.PurchasesRepeat.Purchase[i].InvCrdDate = $filter('date')($scope.PurchasesRepeatInput.InvCrdDate, 'dd-MMM-yyyy');
                        $scope.PurchasesRepeat.Purchase[i].InvCrdNumber = $scope.PurchasesRepeatInput.InvCrdNumber;
                        $scope.PurchasesRepeat.Purchase[i].InvCrdAmount = $scope.PurchasesRepeatInput.InvCrdAmount;
                        $scope.PurchasesRepeat.Purchase[i].DueDate = $filter('date')($scope.PurchasesRepeatInput.DueDate, 'dd-MMM-yyyy');
                        $scope.PurchasesRepeat.Purchase[i].Remarks = $scope.PurchasesRepeatInput.Remarks;
                        $scope.HasKey = null;
                        break;
                    }
                }
				for (i = 0; $scope.PurchasesRepeat.Purchase.length>i; i++) {
					purchasesTotal = parseFloat(parseFloat(purchasesTotal) + parseFloat($scope.PurchasesRepeat.Purchase[i].InvCrdAmount)).toFixed(2);						
				}
                
				$scope.purchasesinvoiceTotalAmount = purchasesTotal;
            }			
            $scope.clearInputs();
        }       
    }

    //Adding Returns	
    $scope.addReturns = function (event) { 		
        if (angular.isUndefined(event.SupplierNo) || event.SupplierNo == null) {
            sweetAlert("Error!!", "Please Select Vendor", "error");
            
        } else if (event.InvCrdDate == null) {
            sweetAlert("Error!!", "Please Choose Creditnote Date", "error");
            
        } else if (angular.isUndefined(event.InvCrdNumber) || event.InvCrdNumber == null) {
            sweetAlert("Error!!", "Please fill Credit Number", "error");
            
        } else if (angular.isUndefined(event.InvCrdAmount) || event.InvCrdAmount == null) {
            sweetAlert("Error!!", "Please fill Credit Amount", "error");
            
        } else {
        	var purchaseRetrunsTotal = 0; 
            if ($scope.HasKey === null) {
				if(angular.isUndefined($scope.purchasereturnsinvoiceTotalAmount)){
					purchaseRetrunsTotal = parseFloat(parseFloat(purchaseRetrunsTotal) + parseFloat(event.InvCrdAmount)).toFixed(2);
				}else{
					purchaseRetrunsTotal = parseFloat(parseFloat($scope.purchasereturnsinvoiceTotalAmount) + parseFloat(event.InvCrdAmount)).toFixed(2);
				}				
                //      if ($scope.hasDuplicate(event.SupplierNo) === false) {
                $scope.PurchaseReturnsRepeat.Purchase.push({
                    SupplierName: $("#SupplierNo1 option:selected").text(),
                    SupplierNo: event.SupplierNo,
                    InvCrdDate: $filter('date')(event.InvCrdDate, 'dd-MMM-yyyy'),
                    InvCrdNumber: event.InvCrdNumber,
                    InvCrdAmount: event.InvCrdAmount,
                   // DueDate: $filter('date')(event.DueDate, 'dd-MMM-yyyy'),
                    Remarks: event.Remarks,
                });
				$scope.purchasereturnsinvoiceTotalAmount = purchaseRetrunsTotal;
                
                //    } else {
                //        sweetAlert("Message", 'This Item already in list', "warning");
                //    }
            } else {
				var purchaseRetrunsTotal = 0;
                for (i = 0; $scope.PurchaseReturnsRepeat.Purchase.length > i; i++) {
                    if ($scope.HasKey === $scope.PurchaseReturnsRepeat.Purchase[i].$$hashKey) {					
                        $scope.PurchaseReturnsRepeat.Purchase[i].SupplierName = $("#SupplierNo1 option:selected").text();
                        $scope.PurchaseReturnsRepeat.Purchase[i].SupplierNo = $scope.PurchaseReturnRepeatInput.SupplierNo;
                        $scope.PurchaseReturnsRepeat.Purchase[i].InvCrdDate = $filter('date')($scope.PurchaseReturnRepeatInput.InvCrdDate, 'dd-MMM-yyyy');
                        $scope.PurchaseReturnsRepeat.Purchase[i].InvCrdNumber = $scope.PurchaseReturnRepeatInput.InvCrdNumber;
                        $scope.PurchaseReturnsRepeat.Purchase[i].InvCrdAmount = $scope.PurchaseReturnRepeatInput.InvCrdAmount;
                       // $scope.PurchasesRepeat.Purchase[i].DueDate = $filter('date')($scope.PurchasesRepeatInput.DueDate, 'dd-MMM-yyyy');
                        $scope.PurchaseReturnsRepeat.Purchase[i].Remarks = $scope.PurchaseReturnRepeatInput.Remarks;
                        $scope.HasKey = null;
                        break;
                    }
                }
				for (i = 0; $scope.PurchaseReturnsRepeat.Purchase.length>i; i++) {
					purchaseRetrunsTotal = parseFloat(parseFloat(purchaseRetrunsTotal) + parseFloat($scope.PurchaseReturnsRepeat.Purchase[i].InvCrdAmount)).toFixed(2);						
				}
				$scope.purchasereturnsinvoiceTotalAmount = purchaseRetrunsTotal;
                
            }			
            $scope.clearInputs1();
        }

    }

    $scope.clearInputs = function () {       
        $scope.PurchasesRepeatInput.SupplierNo = null;
        $scope.PurchasesRepeatInput.InvCrdDate = null;
        $scope.PurchasesRepeatInput.InvCrdNumber = null;
        $scope.PurchasesRepeatInput.InvCrdAmount = null;
        $scope.PurchasesRepeatInput.DueDate = null;
        $scope.PurchasesRepeatInput.Remarks = null;
    };

    $scope.clearInputs1 = function () {
        $scope.PurchaseReturnRepeatInput.SupplierNo = null;
        $scope.PurchaseReturnRepeatInput.InvCrdDate = null;
        $scope.PurchaseReturnRepeatInput.InvCrdNumber = null;
        $scope.PurchaseReturnRepeatInput.InvCrdAmount = null;
       // $scope.PurchasesRepeatInput.DueDate = null;
        $scope.PurchaseReturnRepeatInput.Remarks = null;
    };

    $scope.EditPurchases = function (items) {        
        //console.log(items.$$hashKey);
        $scope.HasKey = items.$$hashKey;
        $scope.PurchasesRepeatInput.SupplierNo = parseInt(items.SupplierNo);
        $scope.PurchasesRepeatInput.InvCrdDate = new Date(items.InvCrdDate);
        $scope.PurchasesRepeatInput.InvCrdNumber = items.InvCrdNumber;
        $scope.PurchasesRepeatInput.InvCrdAmount = parseFloat(items.InvCrdAmount);
        $scope.PurchasesRepeatInput.DueDate = new Date(items.DueDate);
        $scope.PurchasesRepeatInput.Remarks = items.Remarks;
    };

    $scope.EditReturns = function (items) {
        //console.log(items.$$hashKey);
        $scope.HasKey = items.$$hashKey;
        $scope.PurchaseReturnRepeatInput.SupplierNo = parseInt(items.SupplierNo);
        $scope.PurchaseReturnRepeatInput.InvCrdDate = new Date(items.InvCrdDate);
        $scope.PurchaseReturnRepeatInput.InvCrdNumber = items.InvCrdNumber;
        $scope.PurchaseReturnRepeatInput.InvCrdAmount = parseFloat(items.InvCrdAmount);
        //$scope.PurchaseReturnRepeatInput.DueDate = $filter('date')(items.DueDate, 'dd-MMM-yyyy');
        $scope.PurchaseReturnRepeatInput.Remarks = items.Remarks;
    };

    $scope.RemovePurchases = function (items) {  
		for (i = 0; $scope.PurchasesRepeat.Purchase.length > i; i++) {
			if (items.$$hashKey === $scope.PurchasesRepeat.Purchase[i].$$hashKey) {   
				$scope.purchasesinvoiceTotalAmount = parseFloat($scope.purchasesinvoiceTotalAmount)- parseFloat(items.InvCrdAmount);
				$scope.PurchasesRepeat.Purchase.splice(i,1);
				sweetAlert("Deleted", 'Item deleted from the list', "success");
				break;
			}
		}
		
          
        $scope.clearInputs1();
        $scope.HasKey = null;
              
    };

    $scope.RemoveReturns = function (items) {                      
			 
		for (i = 0; $scope.PurchaseReturnsRepeat.Purchase.length > i; i++) {
			if (items.$$hashKey === $scope.PurchaseReturnsRepeat.Purchase[i].$$hashKey) { 
			   $scope.purchasereturnsinvoiceTotalAmount = parseFloat($scope.purchasereturnsinvoiceTotalAmount)- parseFloat(items.InvCrdAmount);
			   $scope.PurchaseReturnsRepeat.Purchase.splice(i,1);
			   sweetAlert("Deleted", 'Item deleted from the list', "success");
			   break;
			}
		}
        $scope.clearInputs1();
        $scope.HasKey = null;           
    };


    $scope.savePurchases = function () {
        
        if ($scope.PurchasesRepeat.Purchase.length <= 0) {
            sweetAlert("Warning!!", "Please fill all fields", "warning");            
        } else {
			$scope.PurchasesRepeat.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
			$scope.PurchasesRepeat.ShiftCode = $('#repeatSelect').val();
            var PurchasesRepeat = angular.toJson($scope.PurchasesRepeat);
            PurchasesRepeat = JSON.parse(PurchasesRepeat);
            console.log(PurchasesRepeat);
            // setupService.savePurchases(PurchasesRepeat).success(function (response) {
            //     sweetAlert("Success", "Purchases Saved Successfully", "success");
            // }).error(function (response) {
            //     sweetAlert("Error!!", response, "error");
            //     $scope.PurchasesRepeat.Purchase = [];                
            // });
        }             
    };

    $scope.savePurchasesReturns = function () {        
        if ($scope.PurchaseReturnsRepeat.Purchase.length <= 0) {
            sweetAlert("Warning!!", "Please fill all fields", "warning");            
        } else {
			$scope.PurchaseReturnsRepeat.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
			$scope.PurchaseReturnsRepeat.ShiftCode = $('#repeatSelect').val();
            var PurchaseReturnsRepeat = angular.toJson($scope.PurchaseReturnsRepeat);
            PurchaseReturnsRepeat = JSON.parse(PurchaseReturnsRepeat);
            console.log(PurchaseReturnsRepeat);
            // setupService.savePurchaseReturns(PurchaseReturnsRepeat).success(function (response) {
            //     sweetAlert("Success", "Purchase Returns Saved Successfully", "success");
                
            // }).error(function (response) {
            //     sweetAlert("Error!!", response, "error");
            //     $scope.PurchaseReturnsRepeat.Purchase = [];
                
            // });
        }
    };

}]);