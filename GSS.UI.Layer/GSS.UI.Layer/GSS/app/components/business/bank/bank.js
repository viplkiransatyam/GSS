angular.module('GasApp').controller('BankController',['$scope', '$http', '$filter', '$rootScope', 'businessService', function ($scope, $http, $filter, $rootScope, businessService) {
	
	$scope.date = new Date();
    $scope.shifts = [];
    $scope.BankDetails = [];
    $scope.bankSave = true;
    var storeID = $rootScope.StoreID;
   
    $scope.bank = {
        "StoreID": storeID,
        "Date" : "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "BankDepositDetail" :[],
      }

    //Add Date & shift details to the Header bar
    businessService.getRunningShift(storeID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.getCashDeposit($filter('date')($scope.myDate, 'dd-MMM-yyyy'),$scope.currentShift);
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

    //Get Day Wise Details
    $scope.getCashDeposit = function (selectedDate, ShiftCode) {
        var postData = {};
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        postData.ShiftCode = $("#repeatSelect").val();        
        //Calling getPreviousDayTranscations function to get Day sale data.
        businessService.getCashDeposit(postData).success(function (response) {   
           //Get SaleDelivery Section
            if (response.LedgerDetail.length > 0) {
                $scope.BankDetails = [];
                for (j = 0; j < response.LedgerDetail.length; j++) {                        
                    $scope.BankDetails.push({
                        "LedgerID":response.LedgerDetail[j].LedgerID,
                        "LedgerName":response.LedgerDetail[j].LedgerName,
                        "LedgerSale":response.LedgerDetail[j].LedgerSale,
                        "LedgerPaid":response.LedgerDetail[j].LedgerPaid,
                        "Balance":response.LedgerDetail[j].Balance,
                        "Deposit":response.LedgerDetail[j].Deposit,
                   });                   
                }                    
            }else{
                $scope.BankDetails = [];
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
  

      //Calculating Totalizer values to get Total Totalizer
    $scope.fixDecimals = function (BankDetails) {
        for (var i = 0; i < BankDetails.length; i++) {
            if (BankDetails[i].Deposit != "" && !angular.isUndefined(BankDetails[i].Deposit)) {
                $scope.BankDetails[i].Deposit = parseFloat(BankDetails[i].Deposit).toFixed(2);
                $scope.bankSave = false;
            } else {
                $scope.bankSave = true;
                $scope.BankDetails[i].Deposit = "";
            }
        }
    }

    $scope.saveCashDeposit = function () {
        if ($scope.myDate != null) {
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                $scope.bank.BankDepositDetail = [];
                if ($scope.BankDetails.length > 0) {
                    $scope.bank.ShiftCode = $scope.ShiftCode;
                    $scope.bank.BankDepositDetail = $scope.BankDetails;
                    $scope.bank.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    var BankCashDepositPostData = angular.toJson($scope.bank);
                    BankCashDepositPostData = JSON.parse(BankCashDepositPostData);
                    console.log(BankCashDepositPostData);
                    businessService.saveCashDeposit(BankCashDepositPostData).success(function (response) {
                        sweetAlert("Success", "Bank Deposit Saved Successfully", "success");
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                        $scope.business.BankDepositDetail = [];
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