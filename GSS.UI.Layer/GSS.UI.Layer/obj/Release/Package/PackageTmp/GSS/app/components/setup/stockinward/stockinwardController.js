angular.module('GasApp').controller('StockInwardController',['$scope', '$http', '$filter', '$rootScope', 'gasService', function ($scope, $http, $filter, $rootScope, gasService) {
	
	$scope.HasKey = null;
   	$scope.date = new Date();
   	var storeID = $rootScope.StoreID;
   	$scope.shifts = [];
   	$scope.GasSale = [];
    $scope.GasSaleData = [];
	$scope.deliveryRepeat = [];
	$scope.deliveryRepeatInput = {}; 

	$scope.saleDeliverySave = true;
	$scope.checkValidation = true;

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
    gasService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });
	 //Getting Shifts Details for the store
   	gasService.getShifts(storeID).success(function (result) { 
   	    $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });

      //Getting as Sale Information pushing into GasSale Array    
    gasService.getStorebasedGas(storeID).success(function (result) {
        for (var i = 0; i < result.length; i++) {
            $scope.GasSale.push({
                "GasTypeName": result[i].GasTypeName,
                "GasTypeID": result[i].GasTypeID,
            });
            if (result[i].GasTypeID != 2) {
                $scope.GasSaleData.push({
                    "GasTypeName": result[i].GasTypeName,
                    "GasTypeID": result[i].GasTypeID,
                });
            }
        }
    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        $scope.loading = false;
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
        	   	$scope.deliveryRepeat = [];
                $scope.deliveryRepeatInput.BillOfLading = null;
                $scope.deliveryRepeatInput.DueDate = null;
                $scope.deliveryRepeatInput.GasTypeName = null;
                $scope.deliveryRepeatInput.GasTypeID = null;
                $scope.deliveryRepeatInput.GrossGallons = null;
                $scope.deliveryRepeatInput.NetGallons = null;
                $scope.saleDeliverySave = false;
        	 }else {
                //Making All Save buttons disable after clicking finaltransaction button in Bankdeposits form 
                if (response.EntryLockStatus == "Y") {                    
                    $scope.saleDeliverySave = true;
                } else {                    
                    $scope.saleDeliverySave = false;
                }
               //Get SaleDelivery Section
                if (response.GasInvReceipt.length > 0) {
                    $scope.deliveryRepeat = [];
                    for (j = 0; j < response.GasInvReceipt.length; j++) {
                        $scope.deliveryRepeat.push({
                            StoreID: response.GasInvReceipt[j].storeID,
                            Date: $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                            BillOfLading: response.GasInvReceipt[j].BillOfLading,
                            DueDate: $filter('date')(response.GasInvReceipt[j].DueDate, 'dd-MMM-yyyy'),
                            GasTypeName: response.GasInvReceipt[j].GasTypeName,
                            GasTypeID: response.GasInvReceipt[j].GasTypeID,
                            GrossGallons: parseFloat(response.GasInvReceipt[j].GrossGallons).toFixed(3),
                            NetGallons: parseFloat(response.GasInvReceipt[j].NetGallons).toFixed(3),
                            ShiftCode: response.GasInvReceipt[j].NetGallons,
                            SlNo: response.GasInvReceipt[j].SlNo,
                            CreatedUserName: response.GasInvReceipt[j].UserName,
                            CreateTimeStamp: response.GasInvReceipt[j].CreateTimeStamp,
                            ModifiedUserName: response.GasInvReceipt[j].UserName,
                            ModifiedTimeStamp: response.GasInvReceipt[j].ModifiedTimeStamp
                        });                            
                    }                    
                }else{
                    $scope.deliveryRepeat = [];
                }
               
                $scope.checkValidation = false;
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

     //Adding Delivery Receipt	
    $scope.addDelivery = function (event) {
        if (angular.isUndefined(event.BillOfLading) || event.BillOfLading == null) {
            sweetAlert("Error!!", "Please Enter Bill Of Lading", "error");
            $scope.loading = false;
        } else if (event.DueDate == null) {
            sweetAlert("Error!!", "Please Choose Due Date", "error");
            $scope.loading = false;
        } else if (angular.isUndefined(event.GasTypeID) || event.GasTypeID == null || event.GasTypeID == "") {
            sweetAlert("Error!!", "Please Select Gas Name", "error");
            $scope.loading = false;
        } else if (angular.isUndefined(event.GrossGallons) || event.GrossGallons == null) {
            sweetAlert("Error!!", "Please fill Gross Gallons", "error");
            $scope.loading = false;
        } else if (angular.isUndefined(event.NetGallons) || event.NetGallons == null) {
            sweetAlert("Error!!", "Please fill Net Gallons", "error");
            $scope.loading = false;
        } else {
            $scope.loading = true;
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.HasKey === null) {
                //if ($scope.hasDuplicate(event.SupplierNo) === false) {
                if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                    $scope.deliveryRepeat.push({
                        StoreID:storeID,
                        Date: $filter('date')($scope.myDate, 'dd-MMM-yyyy'),
                        BillOfLading: event.BillOfLading,
                        DueDate: $filter('date')(event.DueDate, 'dd-MMM-yyyy'),
                        GasTypeName: $("#GasTypeName option:selected").text(),
                        GasTypeID: event.GasTypeID,
                        GrossGallons: parseFloat(event.GrossGallons).toFixed(3),
                        NetGallons: parseFloat(event.NetGallons).toFixed(3),
                        ShiftCode: $scope.ShiftCode,
                        SlNo: parseInt($scope.deliveryRepeat.length) + 1,
                        CreatedUserName: $rootScope.UserName,
                        CreateTimeStamp: "",
                        ModifiedUserName: $rootScope.UserName,
                        ModifiedTimeStamp: ""
                    });
                    $scope.loading = false;
                } else {
                    $scope.loading = false;
                    sweetAlert("Error!!", 'Please Select Shift Code', "error");
                }
                //    } else {
                //        sweetAlert("Message", 'This Item already in list', "warning");
                //    }
            } else {
                if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                    for (i = 0; $scope.deliveryRepeat.length > i; i++) {
                        if ($scope.HasKey === $scope.deliveryRepeat[i].$$hashKey) {
                            $scope.deliveryRepeat[i].StoreID = storeID;
                            $scope.deliveryRepeat[i].Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                            $scope.deliveryRepeat[i].BillOfLading = $scope.deliveryRepeatInput.BillOfLading;
                            $scope.deliveryRepeat[i].DueDate = $filter('date')($scope.deliveryRepeatInput.DueDate, 'dd-MMM-yyyy');
                            $scope.deliveryRepeat[i].GasTypeName = $("#GasTypeName option:selected").text();
                            $scope.deliveryRepeat[i].GasTypeID = $scope.deliveryRepeatInput.GasTypeID;
                            $scope.deliveryRepeat[i].GrossGallons = parseFloat($scope.deliveryRepeatInput.GrossGallons).toFixed(3);
                            $scope.deliveryRepeat[i].NetGallons = parseFloat($scope.deliveryRepeatInput.NetGallons).toFixed(3);
                            $scope.deliveryRepeat[i].SlNo = parseInt($scope.deliveryRepeat.length) + 1;
                            $scope.deliveryRepeat[i].CreatedUserName = $rootScope.UserName;
                            $scope.deliveryRepeat[i].CreateTimeStamp = "";
                            $scope.deliveryRepeat[i].ModifiedUserName = $rootScope.UserName;
                            $scope.deliveryRepeat[i].ModifiedTimeStamp = "";
                            $scope.HasKey = null;
                            break;
                        }
                    }
                } else {
                    $scope.loading = false;
                    sweetAlert("Error!!", 'Please Select Shift Code', "error");
                }
                $scope.loading = false;                
            }
            $scope.clearInputs();
        }
    }

    $scope.clearInputs = function () {
        // $scope.deliveryRepeatInput.BillOfLading = null;
        // $scope.deliveryRepeatInput.DueDate = null;
        $scope.deliveryRepeatInput.GasTypeName = null;
        $scope.deliveryRepeatInput.GasTypeID = null;
        $scope.deliveryRepeatInput.GrossGallons = null;
        $scope.deliveryRepeatInput.NetGallons = null;
    };

    $scope.RemoveDelivery = function (items) {
        for (i = 0; $scope.deliveryRepeat.length > i; i++) {
            if (items.$$hashKey === $scope.deliveryRepeat[i].$$hashKey) {
                $scope.deliveryRepeat.splice(i, 1);
                sweetAlert("Deleted", 'Item deleted from the list', "success");
                break;
            }
        }
        $scope.clearInputs();
        $scope.HasKey = null;
    }

    //Three decimals to Gross gallons
    $scope.GrossGallonsDecimals = function (value) {
        if (angular.isUndefined(value) || value=="" || value == null) {
           	$scope.deliveryRepeatInput.GrossGallons = null;
        }else{
        	 $scope.deliveryRepeatInput.GrossGallons = parseFloat(value).toFixed(3);
        }
    }

    //Three decimals to Net gallons
    $scope.NetGallonsDecimals = function (value) {
        if (angular.isUndefined(value) || value=="" || value == null) {
            $scope.deliveryRepeatInput.NetGallons = null;
        }else{
        	$scope.deliveryRepeatInput.NetGallons = parseFloat(value).toFixed(3);
        }       
    }

    //Save Delivery Receipts
    $scope.saveDelivery = function () {
        $scope.loading = true;
        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
            if ($scope.deliveryRepeat.length <= 0) {
                sweetAlert("Warning!!", "Please fill all fields", "warning");
                $scope.loading = false;
            } else {
                var deliveryRepeatdata = angular.toJson($scope.deliveryRepeat);
                deliveryRepeatdata = JSON.parse(deliveryRepeatdata);
                gasService.saveDeliveries(deliveryRepeatdata).success(function (response) {
                    sweetAlert("Success", "Delivery Receipt Saved Successfully", "success");
                    $scope.loading = false;
                }).error(function (response) {
                    sweetAlert("Error!!", response, "error");
                    $scope.deliveryRepeat = [];
                    $scope.loading = false;
                });
            }
        } else {
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        }
    }

}]);