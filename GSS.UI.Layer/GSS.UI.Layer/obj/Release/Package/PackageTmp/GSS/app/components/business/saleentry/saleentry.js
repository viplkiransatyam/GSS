angular.module('GasApp').controller('SaleEntryController',['$scope', '$http', '$filter', '$rootScope', 'businessService', function ($scope, $http, $filter, $rootScope, businessService) {
	
	$scope.date = new Date();
    $scope.shifts = [];
    $scope.accounts = [];
    $scope.accounts1 = [];
    $scope.BusinessSales = [];
    $scope.BusinessPaids = [];
    var storeID = $rootScope.StoreID;
   
    $scope.business = {
        "StoreID": storeID,
        "Date" : "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "BusinessSaleCollection" :[],
      }

    //Add Date & shift details to the Header bar
    businessService.getRunningShift(storeID).success(function (result) {
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
    businessService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });  
    //Get Business paid ledgers
    businessService.getPaidLedgers(storeID).success(function (response) {
        $scope.accounts1 = response;
    }).error(function (response) {
        sweetAlert("Error!!",response, "error");
    });

    //Get Day Wise Details
    $scope.getDaySaleDetails = function (selectedDate, ShiftCode) {
        var postData = {};
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        postData.ShiftCode = $("#repeatSelect").val();        
        postData.RequestType = "IN-STORE-SALE";  
        //Calling getPreviousDayTranscations function to get Day sale data.
        businessService.getPreviousDayTranscations(postData).success(function (response) {
            console.log(response);
             if (response == "No Data Found") {
                $scope.BusinessSales = [];
                $scope.BusinessPaids = [];
             }else {               
               //Get SaleDelivery Section
                if (response.BusinessSaleCollection.length > 0) {
                    $scope.BusinessSales = [];
                    $scope.BusinessPaids = [];
                    for (j = 0; j < response.BusinessSaleCollection.length; j++) {
                        if(response.BusinessSaleCollection[j].DisplaySide == "BS"){
                            $scope.BusinessSales.push({
                                "LedgerID":response.BusinessSaleCollection[j].LedgerID,
                                "DisplaySide":response.BusinessSaleCollection[j].DisplaySide,
                                "LedgerName":response.BusinessSaleCollection[j].LedgerName,
                                "Amount":response.BusinessSaleCollection[j].Amount,
                           });
                        }else if(response.BusinessSaleCollection[j].DisplaySide == "BP"){
                            $scope.BusinessPaids.push({
                                "LedgerID":response.BusinessSaleCollection[j].LedgerID,
                                "DisplaySide":response.BusinessSaleCollection[j].DisplaySide,
                                "LedgerName":response.BusinessSaleCollection[j].LedgerName,
                                "Amount":response.BusinessSaleCollection[j].Amount,
                                "PaymentType":response.BusinessSaleCollection[j].PaymentType,
                           });
                        }                 
                    }                    
                }else{
                    $scope.BusinessSales = [];
                    $scope.BusinessPaids = [];
                }               
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
   

    $scope.getSaleHeads = function(selectedItem){
        if(selectedItem == "GroupSale"){
            //Getting  Business account groups
            businessService.groupSales(storeID).success(function (response) {
                $scope.accounts = response;
            }).error(function (response) {
                sweetAlert("Error!!",response, "error");
            });
        }else if(selectedItem == "IndividualSale"){
            //Getting  Business account groups
            businessService.individualsaleorpaid(storeID).success(function (response) {
                $scope.accounts = response;
            }).error(function (response) {
                sweetAlert("Error!!",response, "error");
            });
        }
       
    }

    $scope.getSalePaids = function(selectedItem){
        if(selectedItem == "GroupPaid"){
            //Getting  Business account groups
            businessService.groupPaid(storeID).success(function (response) {
                $scope.accounts1 = response;
            }).error(function (response) {
                sweetAlert("Error!!",response, "error");
            });
        }else if(selectedItem == "IndividualPaid"){
            //Getting  Business account groups
            businessService.individualsaleorpaid(storeID).success(function (response) {
                $scope.accounts1 = response;
            }).error(function (response) {
                sweetAlert("Error!!",response, "error");
            });
        }
    }
    

    $scope.addBusinessSale = function () {
        if (angular.isUndefined($scope.businesssale.ItemName) || $scope.businesssale.ItemName == null) {
            sweetAlert("Warning!!", "Please Select Sale.", "warning");
        }else if (angular.isUndefined($scope.businesssale.LedgerID) || $scope.businesssale.LedgerID == null) {
            sweetAlert("Warning!!", "Please Select Sale Head.", "warning");
        }else if (angular.isUndefined($scope.businesssale.Amount) || $scope.businesssale.Amount == "" || $scope.businesssale.Amount == null) {
            sweetAlert("Warning!!", "Please Enter Amount", "warning");
        }else{
        	$scope.saleTotal = 0;
           $scope.BusinessSales.push({
                "LedgerID":$scope.businesssale.LedgerID,
                "DisplaySide":"BS",
                "LedgerName":$("#LedgerID option:selected").text(),
                "Amount":$scope.businesssale.Amount,
           });

           if($scope.BusinessSales.length>0){
           	for (j = 0; j < $scope.BusinessSales.length; j++) {
           		$scope.saleTotal = parseFloat(parseFloat($scope.BusinessSales[j].Amount)+parseFloat($scope.saleTotal)).toFixed(2);
           	}
           }
           
           $scope.businesssale.Amount = null;
           $scope.businesssale.LedgerID = null;
        }
    }


    $scope.addBusinessPaid = function () {
        if (angular.isUndefined($scope.businesspaid.PaymentType) || $scope.businesspaid.PaymentType == null) {
            sweetAlert("Warning!!", "Please Select PaymentType.", "warning");
        }else if (angular.isUndefined($scope.businesspaid.LedgerID) || $scope.businesspaid.LedgerID == null) {
            sweetAlert("Warning!!", "Please Select Paid for.", "warning");
        }else if (angular.isUndefined($scope.businesspaid.Amount) || $scope.businesspaid.Amount == "" || $scope.businesspaid.Amount == null) {
            sweetAlert("Warning!!", "Please Enter Amount", "warning");
        }else{
        	$scope.paidTotal = 0;
           $scope.BusinessPaids.push({
                "LedgerID":$scope.businesspaid.LedgerID,
                "DisplaySide":"BP",
                "LedgerName":$("#LedgerID1 option:selected").text(),
                "Amount":$scope.businesspaid.Amount,
                "PaymentType":$scope.businesspaid.PaymentType,
           });
           if($scope.BusinessPaids.length>0){
           	for (j = 0; j < $scope.BusinessPaids.length; j++) {
           		$scope.paidTotal = parseFloat(parseFloat($scope.BusinessPaids[j].Amount)+parseFloat($scope.paidTotal)).toFixed(2);
           	}
           }
           
           $scope.businesspaid = {};
        }
    }

    $scope.removeSale = function (game) {
        var index = $scope.BusinessSales.indexOf(game);
        $scope.BusinessSales.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
        if($scope.BusinessSales.length>0){
           	for (j = 0; j < $scope.BusinessSales.length; j++) {
           		$scope.saleTotal = parseFloat(parseFloat($scope.BusinessSales[j].Amount)+parseFloat($scope.saleTotal)).toFixed(2);
           	}
        }
    }

    $scope.removePaid = function (game) {
        var index = $scope.BusinessPaids.indexOf(game);
        $scope.BusinessPaids.splice(game, 1);
        if (index >= 0) {
            array.splice(index, 1);
        }
        if($scope.BusinessPaids.length>0){
           	for (j = 0; j < $scope.BusinessPaids.length; j++) {
           		$scope.paidTotal = parseFloat(parseFloat($scope.BusinessPaids[j].Amount)+parseFloat($scope.paidTotal)).toFixed(2);
           	}
        }
    }
  

    $scope.saveBusinessSale = function () {
        if ($scope.myDate != null) {
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                $scope.business.BusinessSaleCollection = [];
                if ($scope.BusinessSales.length > 0) {
                    $scope.business.BusinessSaleCollection = $scope.BusinessSales;
                    $scope.business.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    $scope.business.ShiftCode = $scope.ShiftCode;
                    var BusinessSalePostData = angular.toJson($scope.business);
                    BusinessSalePostData = JSON.parse(BusinessSalePostData);
                    //console.log(BusinessSalePostData);
                    businessService.saveBusiness(BusinessSalePostData).success(function (response) {
                        sweetAlert("Success", "Business Sale Saved Successfully", "success");
                        $scope.businesssale = [];
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                        $scope.business.BusinessSaleCollection = [];
                    });
                }
            } else {
                sweetAlert("Error!!", 'Please Select Shift Code', "error");
            }
        } else {
            sweetAlert("Error!!", 'Please Select Date', "error");
        }
    }


    $scope.saveBusinessPaid = function () {
        if ($scope.myDate != null) {
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                $scope.business.BusinessSaleCollection = [];
                if ($scope.BusinessPaids.length > 0) {
                    $scope.business.BusinessSaleCollection = $scope.BusinessPaids;
                    $scope.business.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    $scope.business.ShiftCode = $scope.ShiftCode;
                    var BusinessPaidsPostData = angular.toJson($scope.business);
                    BusinessPaidsPostData = JSON.parse(BusinessPaidsPostData);
                    businessService.saveBusiness(BusinessPaidsPostData).success(function (response) {
                        sweetAlert("Success", "Business Paid Saved Successfully", "success");
                        $scope.businesspaid = [];
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                         $scope.business.BusinessSaleCollection = [];
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