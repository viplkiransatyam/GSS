angular.module('GasApp').controller("PaymentVoucherController", ['$scope', '$rootScope', '$filter', 'businessService', function ($scope, $rootScope, $filter, businessService) {   
    $scope.date = new Date();
    $scope.shifts = [];
    $scope.accounts = [];
    $scope.paymentsTable = [];
    $scope.receiptsTable = [];
    var storeID = $rootScope.StoreID;
   

    //Add Date & shift details to the Header bar
    businessService.getRunningShift(storeID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.ShiftCode = $scope.currentShift;
    }).error(function () {
        sweetAlert("Error!!", "Error in Getting Running Shift Details", "error");
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
    businessService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });  


    //Getting  Business account groups
    businessService.getAccountLedgers(storeID).success(function (response) {
        $scope.accounts = response;
    }).error(function (response) {
        sweetAlert("Error!!",response, "error");
    }); 
    

    $scope.addPayments = function () {
        if (angular.isUndefined($scope.payments.AccountLedgerID) || $scope.payments.AccountLedgerID == null) {
            sweetAlert("Warning!!", "Please Select Account Name.", "warning");
        }else if (angular.isUndefined($scope.payments.AccountTranType) || $scope.payments.AccountTranType == null) {
            sweetAlert("Warning!!", "Please Select Account Transaction type.", "warning");
        }else if (angular.isUndefined($scope.payments.Amount) || $scope.payments.Amount == "" || $scope.payments.Amount == null) {
            sweetAlert("Warning!!", "Please Enter Amount", "warning");
        }else if(angular.isUndefined($scope.payments.PaymentRemarks) || $scope.payments.PaymentRemarks == "" || $scope.payments.PaymentRemarks == null){
            sweetAlert("Warning!!", "Please Enter Remarks", "warning");
        }else{
            if ($scope.myDate != null) {
                $scope.ShiftCode = $("#repeatSelect").val();
                if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                    var result = {};                  
                    result.StoreID = storeID;
                    result.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    result.ShiftCode =  $scope.ShiftCode;
                    result.VoucherType = "PAYMENT";
                    result.CreatedUserName = $rootScope.UserName;
                    result.AccountLedgerID = $scope.payments.AccountLedgerID;
                    result.AccountTranType = $scope.payments.AccountTranType;
                    result.Amount = $scope.payments.Amount;
                    result.PaymentRemarks = $scope.payments.PaymentRemarks;
                    result.DisplayName = $("#AccountLedgerID option:selected").text();
                    var PaymentsPostData = angular.toJson(result);
                    PaymentsPostData = JSON.parse(PaymentsPostData);
                    businessService.saveRecords(PaymentsPostData).success(function (response) {
                        sweetAlert("Success", "Payments Saved Successfully", "success");
                        $scope.payments = [];
                        getPaymentsData($filter('date')($scope.myDate, 'dd-MMM-yyyy'),$scope.ShiftCode);
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                    });
                }else {
                    sweetAlert("Error!!", 'Please Select Shift Code', "error");
                }
            } else {
                sweetAlert("Error!!", 'Please Select Date', "error");
            }  
        }
        
    }


    $scope.addReceipts = function () {
        if (angular.isUndefined($scope.receipts.AccountLedgerID) || $scope.receipts.AccountLedgerID == null) {
            sweetAlert("Warning!!", "Please Select Account Name.", "warning");
        }else if (angular.isUndefined($scope.receipts.AccountTranType) || $scope.receipts.AccountTranType == null) {
            sweetAlert("Warning!!", "Please Select Account Transaction type.", "warning");
        }else if (angular.isUndefined($scope.receipts.Amount) || $scope.receipts.Amount == "" || $scope.receipts.Amount == null) {
            sweetAlert("Warning!!", "Please Enter Amount", "warning");
        }else if(angular.isUndefined($scope.receipts.PaymentRemarks) || $scope.receipts.PaymentRemarks == "" || $scope.receipts.PaymentRemarks == null){
            sweetAlert("Warning!!", "Please Enter Remarks", "warning");
        }else{
            if ($scope.myDate != null) {
                $scope.ShiftCode = $("#repeatSelect").val();
                if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                    var result = {};
                    result.StoreID = storeID;
                    result.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    result.ShiftCode =  $scope.ShiftCode;
                    result.VoucherType = "RECEIPT";
                    result.CreatedUserName = $rootScope.UserName;                  
                    result.AccountLedgerID = $scope.receipts.AccountLedgerID;
                    result.AccountTranType = $scope.receipts.AccountTranType;
                    result.Amount = $scope.receipts.Amount;
                    result.PaymentRemarks = $scope.receipts.PaymentRemarks;
                    result.DisplayName = $("#AccountLedgerID option:selected").text();

                    var ReceiptsPostData = angular.toJson(result);
                    ReceiptsPostData = JSON.parse(ReceiptsPostData);
                    businessService.saveRecords(ReceiptsPostData).success(function (response) {
                        sweetAlert("Success", "Receipts Saved Successfully", "success");
                        $scope.receipts = [];
                        getReceiptsData($filter('date')($scope.myDate, 'dd-MMM-yyyy'),$scope.ShiftCode);
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                    });

                }else {
                    sweetAlert("Error!!", 'Please Select Shift Code', "error");
                }
            } else {
                sweetAlert("Error!!", 'Please Select Date', "error");
            }
        }        
    }
    $scope.removePayments = function (index) {
        //var index = $scope.paymentsTable.indexOf(index);
        
        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
            var result = {};
            result.StoreID = storeID;
            result.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
            result.ShiftCode = $scope.ShiftCode;
            result.VoucherType = "PAYMENT";
            result.CreatedUserName = $rootScope.UserName;
            result.AccountLedgerID = $scope.paymentsTable[index].AccountLedgerID;
            result.AccountTranType = $scope.paymentsTable[index].AccountTranType;
            result.Amount = $scope.paymentsTable[index].Amount;

            var PaymentsPostData = angular.toJson(result);
            PaymentsPostData = JSON.parse(PaymentsPostData);
            businessService.deleteRecords(PaymentsPostData).success(function (response) {
                sweetAlert("Success", "Payment Voucher deleted Successfully", "success");
                $scope.payments = [];
                getPaymentsData($filter('date')($scope.myDate, 'dd-MMM-yyyy'), $scope.ShiftCode);
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }

        $scope.paymentsTable.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
    }

    $scope.removeReceipts = function (index) {

        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
            var result = {};
            result.StoreID = storeID;
            result.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
            result.ShiftCode = $scope.ShiftCode;
            result.VoucherType = "RECEIPT";
            result.CreatedUserName = $rootScope.UserName;
            result.AccountLedgerID = $scope.receiptsTable[index].AccountLedgerID;
            result.AccountTranType = $scope.receiptsTable[index].AccountTranType;
            result.Amount = $scope.receiptsTable[index].Amount;

            var PaymentsPostData = angular.toJson(result);
            PaymentsPostData = JSON.parse(PaymentsPostData);
            businessService.deleteRecords(PaymentsPostData).success(function (response) {
                sweetAlert("Success", "Payment Voucher deleted Successfully", "success");
                $scope.payments = [];
                getPaymentsData($filter('date')($scope.myDate, 'dd-MMM-yyyy'), $scope.ShiftCode);
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }

        $scope.receiptsTable.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
    }

     //Get Day Wise Details
    $scope.getDaySaleDetails = function () {

    	getPaymentsData($filter('date')($scope.myDate, 'dd-MMM-yyyy'),$scope.ShiftCode);
    	getReceiptsData($filter('date')($scope.myDate, 'dd-MMM-yyyy'),$scope.ShiftCode);
    }
    

    var getPaymentsData = function(selectedDate,ShiftCode){
    	//For payments
        var postData = {};        
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        postData.RequestType = 'PAYMENTS';
        postData.ShiftCode = ShiftCode; 
         //Calling getPreviousDayTranscations function to get Day sale data.
        businessService.getPreviousDayTranscations(postData).success(function (response) {
        	 if (response == "No Data Found") {
        	 	 for (i = 0; i < $scope.paymentsTable.length; i++) {
                    $scope.paymentsTable[i].AccountTranType = "";
                    $scope.paymentsTable[i].AccountLedgerID = "";
                    $scope.paymentsTable[i].Amount = "";
                    $scope.paymentsTable[i].DisplayName = "";
                    $scope.paymentsTable[i].PaymentType = "";
                    $scope.paymentsTable[i].PaymentRemarks = "";
                }
        	 }else {               
                //checking GasReceipt array to fill GasReceipt Section in Form
                if (response.PaymentAccounts.length > 0) {
                    $scope.paymentsTable = [];
                    for (j = 0; j < response.PaymentAccounts.length; j++) {
                        $scope.paymentsTable.push({
                            AccountTranType: response.PaymentAccounts[j].AccountTranType,
                            AccountLedgerID: response.PaymentAccounts[j].AccountLedgerID,
                            Amount: parseFloat(response.PaymentAccounts[j].Amount).toFixed(2),
                            DisplayName: response.PaymentAccounts[j].DisplayName,
                            PaymentType: response.PaymentAccounts[j].PaymentType,
                            PaymentRemarks: response.PaymentAccounts[j].PaymentRemarks,
                        });
                    }
                } else {
                    $scope.paymentsTable = [];
                }               
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            
        });
    }

    var getReceiptsData = function(selectedDate,ShiftCode){
    	 //For receipts
        var postData1 = {};
        postData1.StoreID = storeID;
        postData1.Date = selectedDate;
        postData1.RequestType = 'RECEIPTS';
        postData1.ShiftCode = ShiftCode; 
    	businessService.getPreviousDayTranscations(postData1).success(function (response) {
        	 if (response == "No Data Found") {
        	 	 for (i = 0; i < $scope.receiptsTable.length; i++) {
                    $scope.receiptsTable[i].AccountTranType = "";
                    $scope.receiptsTable[i].AccountLedgerID = "";
                    $scope.receiptsTable[i].Amount = "";
                    $scope.receiptsTable[i].DisplayName = "";
                    $scope.receiptsTable[i].PaymentType = "";
                    $scope.receiptsTable[i].PaymentRemarks = "";
                }
        	 }else {               
                //checking GasReceipt array to fill GasReceipt Section in Form
                if (response.ReceiptAccounts.length > 0) {
                    $scope.receiptsTable = [];
                    for (j = 0; j < response.ReceiptAccounts.length; j++) {
                        $scope.receiptsTable.push({
                            AccountTranType: response.ReceiptAccounts[j].AccountTranType,
                            AccountLedgerID: response.ReceiptAccounts[j].AccountLedgerID,
                            Amount: parseFloat(response.ReceiptAccounts[j].Amount).toFixed(2),
                            DisplayName: response.ReceiptAccounts[j].DisplayName,
                            PaymentType: response.ReceiptAccounts[j].PaymentType,
                            PaymentRemarks: response.ReceiptAccounts[j].PaymentRemarks,
                        });
                    }
                } else {
                    $scope.receiptsTable = [];
                }               
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            
        });
    }

     $scope.savePayments = function () {
         $scope.allPayments = [];
         if ($scope.myDate != null) {
             $scope.ShiftCode = $("#repeatSelect").val();
             if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                 if ($scope.paymentsTable.length > 0) {
                     for (var i = 0; i < $scope.paymentsTable.length; i++) {
                         $scope.allPayments.push({
                             "StoreID": storeID,
                             "DisplayName": $scope.paymentsTable[i].DisplayName,
                             "PaymentRemarks": $scope.paymentsTable[i].PaymentRemarks,
                             "Date": $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                             "ShiftCode": $scope.ShiftCode,
                             "VoucherType":"PAYMENT",
                             "AccountLedgerID": $scope.paymentsTable[i].AccountLedgerID,
                             "AccountTranType": $scope.paymentsTable[i].AccountTranType,
                             "CreatedUserName": $rootScope.UserName,
                             "ModifiedUserName": $rootScope.UserName
                         });
                     }                   
                     var PaymentsPostData = angular.toJson($scope.allPayments);
                     PaymentsPostData = JSON.parse(PaymentsPostData);
                     console.log(PaymentsPostData);
                     businessService.saveRecords(PaymentsPostData).success(function (response) {
                         sweetAlert("Success", "Payments Saved Successfully", "success");
                     }).error(function (response) {
                         sweetAlert("Error!!", response, "error");
                         $scope.allPayments = [];
                     });
                 }
             } else {
                 sweetAlert("Error!!", 'Please Select Shift Code', "error");
             }
         } else {
             sweetAlert("Error!!", 'Please Select Date', "error");
         }
     }


     $scope.saveReceipts = function () {
         $scope.allReceipts = [];
         if ($scope.myDate != null) {
             $scope.ShiftCode = $("#repeatSelect").val();
             if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                 if ($scope.receiptsTable.length > 0) {
                     for (var i = 0; i < $scope.receiptsTable.length; i++) {
                         $scope.allReceipts.push({
                             "StoreID": storeID,
                             "DisplayName": $scope.receiptsTable[i].DisplayName,
                             "PaymentRemarks": $scope.receiptsTable[i].PaymentRemarks,
                             "Date": $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                             "ShiftCode": $scope.ShiftCode,
                             "VoucherType":"RECEIPT",
                             "AccountLedgerID": $scope.receiptsTable[i].AccountLedgerID,
                             "AccountTranType": $scope.receiptsTable[i].AccountTranType,
                             "CreatedUserName": $rootScope.UserName,
                             "ModifiedUserName": $rootScope.UserName
                         });
                     }                   
                     var ReceiptsPostData = angular.toJson($scope.allReceipts);
                     ReceiptsPostData = JSON.parse(ReceiptsPostData);
                     businessService.saveRecords(ReceiptsPostData).success(function (response) {
                         sweetAlert("Success", "Receipts Saved Successfully", "success");
                     }).error(function (response) {
                         sweetAlert("Error!!", response, "error");
                         $scope.allReceipts = [];
                     });
                 }
             } else {
                 sweetAlert("Error!!", 'Please Select Shift Code', "error");
             }
         } else {
             sweetAlert("Error!!", 'Please Select Date', "error");
         }
     }

}]);