angular.module('GasApp').controller('GasController',['$scope', '$http', '$filter', '$rootScope', 'gasService', function ($scope, $http, $filter, $rootScope, gasService) {

    //getting shifts information    
    $scope.HasKey = null;
   	$scope.date = new Date();
   	var storeID = $rootScope.StoreID;
	$scope.shifts = [];
    $scope.GasSale = [];
    $scope.GasSaleData = [];
    $scope.TankDetail = [];
    $scope.GasInventory = [];
    $scope.GasInventory1 = [];   
    $scope.deliveryRepeat = [];
    $scope.SystemClosingBal = [];
    $scope.SystemClosingBal1 = [];   
    $scope.GasTypes = [];
    $scope.deliveryRepeatInput = {};    
    $scope.saleSectionSave = true;    
    $scope.gasInventory = false;  
    $scope.tanksstatus = false;
    $scope.checkValidation = true;

    //GasSale Object array sample
    $scope.gasStation = {
        "StoreID": $rootScope.StoreID,
        "Date": "",
        "TotalSaleGallons": "",
        "TotalTotalizer": "",
        "TotalSale": "",
        "ShiftCode": "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "GasSale": [
            {
                "GasTypeID": "",
                "Totalizer": "",
                "SaleGallons": "",
                "SaleAmount": "",
                "SalePrice": "",
                "SetPrice": ""
            }
        ]
    };

    //GasInventory Object array sample
    $scope.inventory = {
        "StoreID": $rootScope.StoreID,
        "Date": "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "ShiftCode":"",
        "GasInventory": [
            {
                "TankID": "",
                "GasTypeID": "",
                "OpeningBalance": "",                
                "ActualClosingBalance": "",
                "StickInches": "",
                "StickGallons": "",
            }
        ]
    }

   

    $scope.loading = true;
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

    $scope.showPanelsOne = function(){
        $("#accOneColOne").css("display","block");
        $("#accOneColThree").css("display","none");
        $("#accOneColFour").css("display","none");
    };  
    $scope.showPanelsThree = function(){
        $("#accOneColOne").css("display","none");
        $("#accOneColThree").css("display","block");
        $("#accOneColFour").css("display","none");
    }; 
    $scope.showPanelsFour = function(){
        $("#accOneColOne").css("display","none");
        $("#accOneColThree").css("display","none");
        $("#accOneColFour").css("display","block");
    };   

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
        //Assigning the GasSale array to $scope.gasStation.GasSale object array
        $scope.gasStation.GasSale = $scope.GasSale;
    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        $scope.loading = false;
    });

    //Getting as Sale Information pushing into GasSale Array    
    gasService.getGasTypeExcludeDuplicate(storeID).success(function (result) {
        $scope.GasTypes = result;
    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        $scope.loading = false;
    });



    //Getting Store Name and Tank Details
    var getTanksAndOpeningBalance = function(storeID){
            gasService.getStoreName(storeID).success(function (result1) {
            $scope.StoreName = result1.StoreName;

            for (var j = 0; j < result1.TankDetail.length; j++) {
                $scope.TankDetail.push({
                    "TankID": result1.TankDetail[j].TankID,
                    "TankName": result1.TankDetail[j].TankName
                });
            }
            if ($scope.TankDetail.length > 0) {
                $scope.tank1 = 0;
                $scope.tank2 = 0;
                var t = 0;
                for (var tanks = 0; tanks < $scope.TankDetail.length; tanks++) {
                    //Getting Tank Details and pushing array into GasInventory 
                    if (t != $scope.TankDetail[tanks].TankID) {
                        if ($scope.tank1 == 0) {
                            $scope.TankName = $scope.TankDetail[tanks].TankName;
                            $scope.tank1 = $scope.TankDetail[tanks].TankID;
                        }
                        else {
                            $scope.TankName1 = $scope.TankDetail[tanks].TankName;
                            $scope.tank2 = $scope.TankDetail[tanks].TankID;
                            $scope.tanksstatus = true;
                        }

                    }
                    t = $scope.TankDetail[tanks].TankID;

                    var gasData = {};
                    gasData.StoreId = $rootScope.StoreID;
                    gasData.TankID = $scope.TankDetail[tanks].TankID;
                    gasData.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    gasService.getGasopeningBalance(gasData).success(function (result) {
                        for (var i = 0; i < result.length; i++) {
                            if ($scope.tank2 == 0) {
                                $scope.GasInventory.push({
                                    "TankID": result[i].TankID,//tankid is getting 0 from the server
                                    "GasTypeID": result[i].GasTypeID,
                                    "GasTypeName": result[i].GasTypeName,
                                    "OpeningBalance": result[i].OpeningBalance.toFixed(3),
                                    "GasReceived": "",
                                    "ActualClosingBalance": "",
                                    "StickInches": "",
                                    "StickGallons": "",
                                    "GasPrice": "",
                                    "TankCapacity": result[i].TankCapacity.toFixed(3),
                                    "EmptySpace":"",
                                });
                            } else {
                                if ($scope.tank1 == result[i].TankID) {
                                    $scope.GasInventory.push({
                                        "TankID": result[i].TankID,//tankid is getting 0 from the server
                                        "GasTypeID": result[i].GasTypeID,
                                        "GasTypeName": result[i].GasTypeName,
                                        "OpeningBalance": result[i].OpeningBalance.toFixed(3),
                                        "GasReceived": "",
                                        "ActualClosingBalance": "",
                                        "StickInches": "",
                                        "StickGallons": "",
                                        "GasPrice": "",
                                        "TankCapacity": result[i].TankCapacity.toFixed(3),
                                        "EmptySpace":"",
                                    });
                                } else if ($scope.tank2 == result[i].TankID) {
                                    $scope.GasInventory1.push({
                                        "TankID": result[i].TankID,//tankid is getting 0 from the server
                                        "GasTypeID": result[i].GasTypeID,
                                        "GasTypeName": result[i].GasTypeName,
                                        "OpeningBalance": result[i].OpeningBalance.toFixed(3),
                                        "GasReceived": "",
                                        "ActualClosingBalance": "",
                                        "StickInches": "",
                                        "StickGallons": "",
                                        "GasPrice": "",
                                        "TankCapacity": result[i].TankCapacity.toFixed(3),
                                        "EmptySpace":"",
                                    });
                                }
                            }
                        }
                        //Assigning the GasInventory array to $scope.inventory.GasInventory object array
                        // $scope.inventory.GasInventory = $scope.GasInventory;                    
                    }).error(function (result) {
                        sweetAlert('Error!!', result, 'error');
                        $scope.loading = false;
                    });
                }
            }
        }).error(function (result1) {
            sweetAlert('Error!!', result1, 'error');
            $scope.loading = false;
        });
    }    
    getTanksAndOpeningBalance(storeID); 

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
            $scope.SystemClosingBal = [];
            $scope.inventory.SystemClosingBal = [];
            $scope.SystemClosingBal1 = [];
            $scope.inventory.SystemClosingBal1 = [];
            if (response == "No Data Found") {
                for (i = 0; i < $scope.GasSale.length; i++) {
                    $scope.GasSale[i].SaleAmount = "";
                    $scope.GasSale[i].SaleGallons = "";
                    $scope.GasSale[i].SalePrice = "";
                    $scope.GasSale[i].Totalizer = "";
                    $scope.GasSale[i].SetPrice = "";
                }
              
                
                for (i = 0; i < $scope.GasInventory.length; i++) {
                    $scope.GasInventory[i].GasReceived = "";
                    $scope.GasInventory[i].ActualClosingBalance = "";
                    $scope.GasInventory[i].OpeningBalance = "";
                    $scope.GasInventory[i].StickInches = "";
                    $scope.GasInventory[i].StickGallons = "";
                    $scope.GasInventory[i].TankCapacity = "";
                    $scope.GasInventory[i].GasPrice = "";
                    $scope.TankDetail = [];
                    $scope.GasInventory = [];
                    $scope.GasInventory1 = [];
                    getTanksAndOpeningBalance(storeID);
                }
                for (i = 0; i < $scope.GasInventory1.length; i++) {
                    $scope.GasInventory1[i].GasReceived = "";
                    $scope.GasInventory1[i].ActualClosingBalance = "";
                    $scope.GasInventory1[i].StickInches = "";
                    $scope.GasInventory1[i].StickGallons = "";
                    $scope.GasInventory1[i].GasPrice = "";
                    $scope.GasInventory1[i].TankCapacity = "";
                }
                //Sale Section
                $scope.gasStation.TotalTotalizer = 0;
                $scope.gasStation.TotalSaleGallons = 0;
                $scope.gasStation.TotalSale = 0;
             
                $scope.checkValidation = true;
                $scope.saleSectionSave = true;
                $scope.gasInventory = false;
            } else {
                //Making All Save buttons disable after clicking finaltransaction button in Bankdeposits form 
                if (response.EntryLockStatus == "Y") {
                    $scope.saleSectionSave = true;
                    $scope.gasInventory = true;
                    $scope.saleDeliverySave = true;
                } else {
                    $scope.saleSectionSave = false;
                    $scope.gasInventory = false;
                    $scope.saleDeliverySave = false;
                }
                //checking GasSale array to fill GasSale Section in Form
                if (response.GasSale.length > 0) {
                    for (i = 0; i < response.GasSale.length; i++) {
                        for (j = 0; j < $scope.GasSale.length; j++) {
                            if ($scope.GasSale[j].GasTypeID == response.GasSale[i].GasTypeID) {
                                $scope.GasSale[j].GasTypeID = response.GasSale[i].GasTypeID;
                                $scope.GasSale[j].GasTypeName = response.GasSale[i].GasTypeName;
                                $scope.GasSale[j].SaleAmount = parseFloat(response.GasSale[i].SaleAmount).toFixed(2);
                                $scope.GasSale[j].SaleGallons = parseFloat(response.GasSale[i].SaleGallons).toFixed(3);
                                $scope.GasSale[j].SalePrice = parseFloat(response.GasSale[i].SalePrice).toFixed(2);
                                $scope.GasSale[j].Totalizer = parseFloat(response.GasSale[i].Totalizer).toFixed(3);
                                $scope.GasSale[j].SetPrice = parseFloat(response.GasSale[i].SetPrice).toFixed(2);
                            }
                        }
                    }
                } else {
                    for (i = 0; i < $scope.GasSale.length; i++) {
                        $scope.GasSale[i].SaleAmount = "";
                        $scope.GasSale[i].SaleGallons = "";
                        $scope.GasSale[i].SalePrice = "";
                        $scope.GasSale[i].Totalizer = "";
                        $scope.GasSale[i].SetPrice = "";
                    }
                }
                //checking GasInventory array to fill GasInventory Section in Form
                if (response.GasInventory.length > 0) {
                    for (i = 0; i < response.GasInventory.length; i++) {
                        if ($scope.tank2 == 0) {
                            for (j = 0; j < $scope.GasInventory.length; j++) {
                                if ($scope.GasInventory[j].GasTypeID != 2) {
                                    if (response.GasInventory[i].GasTypeID == $scope.GasInventory[j].GasTypeID) {
                                        $scope.GasInventory[j].TankID = response.GasInventory[i].TankID;
                                        $scope.GasInventory[j].GasTypeID = response.GasInventory[i].GasTypeID;
                                        $scope.GasInventory[j].OpeningBalance = parseFloat(response.GasInventory[i].OpeningBalance).toFixed(3);
                                        $scope.GasInventory[j].SystemClosingBalance = parseFloat(response.GasInventory[i].SystemClosingBalance).toFixed(3);
                                        $scope.GasInventory[j].GasReceived = parseFloat(response.GasInventory[i].GasReceived).toFixed(3);
                                        $scope.GasInventory[j].ActualClosingBalance = parseFloat(response.GasInventory[i].ActualClosingBalance).toFixed(3);
                                        $scope.GasInventory[j].StickInches = parseFloat(response.GasInventory[i].StickInches).toFixed(3);
                                        $scope.GasInventory[j].StickGallons = parseFloat(response.GasInventory[i].StickGallons).toFixed(3);
                                        $scope.GasInventory[j].GasPrice = parseFloat(response.GasInventory[i].GasPrice).toFixed(3);
                                        $scope.GasInventory[j].TankCapacity = parseFloat(response.GasInventory[i].TankCapacity).toFixed(3);
                                        $scope.GasInventory[j].EmptySpace = (response.GasInventory[i].ActualClosingBalance>0)?parseFloat(response.GasInventory[i].TankCapacity - response.GasInventory[i].ActualClosingBalance).toFixed(3):0;
                                    }
                                }
                            }                            
                        } else {
                            if ($scope.tank1 == response.GasInventory[i].TankID) {
                                for (j = 0; j < $scope.GasInventory.length; j++) {
                                    if ($scope.GasInventory[j].GasTypeID != 2) {
                                        if (response.GasInventory[i].GasTypeID == $scope.GasInventory[j].GasTypeID) {
                                            $scope.GasInventory[j].TankID = response.GasInventory[i].TankID;
                                            $scope.GasInventory[j].GasTypeID = response.GasInventory[i].GasTypeID;
                                            $scope.GasInventory[j].OpeningBalance = parseFloat(response.GasInventory[i].OpeningBalance).toFixed(3);
                                            $scope.GasInventory[j].SystemClosingBalance = parseFloat(response.GasInventory[i].SystemClosingBalance).toFixed(3);
                                            $scope.GasInventory[j].GasReceived = parseFloat(response.GasInventory[i].GasReceived).toFixed(3);
                                            $scope.GasInventory[j].ActualClosingBalance = parseFloat(response.GasInventory[i].ActualClosingBalance).toFixed(3);
                                            $scope.GasInventory[j].StickInches = parseFloat(response.GasInventory[i].StickInches).toFixed(3);
                                            $scope.GasInventory[j].StickGallons = parseFloat(response.GasInventory[i].StickGallons).toFixed(3);
                                            $scope.GasInventory[j].GasPrice = parseFloat(response.GasInventory[i].GasPrice).toFixed(3);
                                            $scope.GasInventory[j].TankCapacity = parseFloat(response.GasInventory[i].TankCapacity).toFixed(3);
                                            $scope.GasInventory[j].EmptySpace = (response.GasInventory[i].ActualClosingBalance>0)?parseFloat(response.GasInventory[i].TankCapacity - response.GasInventory[i].ActualClosingBalance).toFixed(3):0;
                                        }
                                    }
                                }
                            } else if ($scope.tank2 == response.GasInventory[i].TankID) {
                                for (j = 0; j < $scope.GasInventory1.length; j++) {
                                    if ($scope.GasInventory1[j].GasTypeID != 2) {
                                        if (response.GasInventory[i].GasTypeID == $scope.GasInventory[j].GasTypeID) {
                                            $scope.GasInventory1[j].TankID = response.GasInventory[i].TankID;
                                            $scope.GasInventory1[j].GasTypeID = response.GasInventory[i].GasTypeID;
                                            $scope.GasInventory1[j].OpeningBalance = parseFloat(response.GasInventory[i].OpeningBalance).toFixed(3);
                                            $scope.GasInventory1[j].GasReceived = parseFloat(response.GasInventory[i].GasReceived).toFixed(3);
                                            $scope.GasInventory1[j].ActualClosingBalance = parseFloat(response.GasInventory[i].ActualClosingBalance).toFixed(3);
                                            $scope.GasInventory1[j].StickInches = parseFloat(response.GasInventory[i].StickInches).toFixed(3);
                                            $scope.GasInventory1[j].StickGallons = parseFloat(response.GasInventory[i].StickGallons).toFixed(3);
                                            $scope.GasInventory1[j].GasPrice = parseFloat(response.GasInventory[i].GasPrice).toFixed(3);
                                            $scope.GasInventory1[j].TankCapacity = parseFloat(response.GasInventory[i].TankCapacity).toFixed(3);
                                            $scope.GasInventory[j].EmptySpace = (response.GasInventory[i].ActualClosingBalance>0)?parseFloat(response.GasInventory[i].TankCapacity - response.GasInventory[i].ActualClosingBalance).toFixed(3):0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    for (i = 0; i < $scope.GasInventory.length; i++) {
                        $scope.GasInventory[i].GasReceived = "";
                        $scope.GasInventory[i].OpeningBalance = "";
                        $scope.GasInventory[i].ActualClosingBalance = "";
                        $scope.GasInventory[i].StickInches = "";
                        $scope.GasInventory[i].StickGallons = "";
                        $scope.GasInventory[i].GasPrice = "";
                        $scope.GasInventory[i].TankCapacity = "";
                    }
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
                
                //Filling data into the all Total values

                //Sale Section
                $scope.gasStation.TotalTotalizer = response.TotalTotalizer.toFixed(3);
                $scope.gasStation.TotalSaleGallons = response.TotalSaleGallons.toFixed(3);
                $scope.gasStation.TotalSale = response.TotalSale.toFixed(2);

                $scope.checkValidation = false;
            }
            $scope.loading = false;
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });

    }



    //Calculating Price of per Gallon in this calculatePerGallon function
    $scope.calculatePerGallon = function (GasSale) {
        var sum = 0;
        for (var i = 0; i < GasSale.length; i++) {
            if (GasSale[i].SaleAmount && !angular.isUndefined(GasSale[i].SaleAmount)) {
                if (GasSale[i].Totalizer == null) {
                    sweetAlert('Warning!!', 'Please Fill Totalizer', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                } else if (GasSale[i].SaleGallons == null) {
                    sweetAlert('Warning!!', 'Please Fill Gallons', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                } else {
                    $scope.gasStation.GasSale[i].SaleAmount = parseFloat(GasSale[i].SaleAmount).toFixed(2);
                    sum += parseFloat(GasSale[i].SaleAmount);
                    var SalePrice = GasSale[i].SaleAmount / GasSale[i].SaleGallons;
                    GasSale[i].SalePrice = parseFloat(SalePrice.toFixed(2));
                    $scope.saleSectionSave = false;
                }
            } else {
                $scope.gasStation.GasSale[i].SaleAmount = "";
                $scope.saleSectionSave = true;
            }
        }
        $scope.gasStation.TotalSale = parseFloat(sum).toFixed(2);
    }

    //Totalizer validation
    $scope.validateTotalizer = function (GasSale) {
        var sum = 0;
        for (var i = 0; i < GasSale.length; i++) {
            if (GasSale[i].SaleGallons && !angular.isUndefined(GasSale[i].SaleGallons)) {
                if (GasSale[i].Totalizer == null) {
                    sweetAlert('Warning!!', 'Please Fill Totalizer', 'warning');
                    GasSale[i].SaleGallons = '';
                    break;
                } else {
                    $scope.gasStation.GasSale[i].SaleGallons = parseFloat(GasSale[i].SaleGallons).toFixed(3);
                    sum += parseFloat(GasSale[i].SaleGallons);
                    $scope.saleSectionSave = false;
                }
            } else {
                $scope.gasStation.GasSale[i].SaleGallons = "";
                $scope.saleSectionSave = true;
            }
        }
        $scope.gasStation.TotalSaleGallons = parseFloat(sum).toFixed(3);

    }

    //Calculating Totalizer values to get Total Totalizer
    $scope.grandtotalTotalizer = function (GasSale) {
        var sum = 0;
        for (var i = 0; i < GasSale.length; i++) {
            if (GasSale[i].Totalizer != "" && !angular.isUndefined(GasSale[i].Totalizer)) {
                $scope.gasStation.GasSale[i].Totalizer = parseFloat(GasSale[i].Totalizer).toFixed(3);
                sum += parseFloat(GasSale[i].Totalizer);
                $scope.saleSectionSave = false;
            } else {
                $scope.saleSectionSave = true;
                $scope.gasStation.GasSale[i].Totalizer = "";
            }
        }
        $scope.gasStation.TotalTotalizer = parseFloat(sum).toFixed(3);

    }

    $scope.getSetprice = function (GasSale) {
        var sum = 0;
        for (var i = 0; i < GasSale.length; i++) {
            if (GasSale[i].SetPrice != "" && !angular.isUndefined(GasSale[i].SetPrice)) {
            	 if (GasSale[i].Totalizer == null) {
                    sweetAlert('Warning!!', 'Please Fill Totalizer', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                } else if (GasSale[i].SaleGallons == null) {
                    sweetAlert('Warning!!', 'Please Fill Gallons', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                } else if(GasSale[i].SaleAmount == null){
                	sweetAlert('Warning!!', 'Please Fill Amount', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                }else{
                	$scope.gasStation.GasSale[i].SetPrice = parseFloat(GasSale[i].SetPrice).toFixed(2);                
                	$scope.saleSectionSave = false;
                }
            } else {
                $scope.saleSectionSave = true;
                $scope.gasStation.GasSale[i].SetPrice = "";
            }
        }      
    }
    

   
    //Three decimals to Stock
    $scope.checkBalance = function (GasInventory) {
        for (var i = 0; i < GasInventory.length; i++) {
            if (GasInventory[i].OpeningBalance && !angular.isUndefined(GasInventory[i].OpeningBalance)) {
                $scope.GasInventory[i].OpeningBalance = parseFloat(GasInventory[i].OpeningBalance).toFixed(3);
            } else {
                $scope.GasInventory[i].OpeningBalance = "";
            }
            if (GasInventory[i].GasReceived && !angular.isUndefined(GasInventory[i].GasReceived)) {
                $scope.GasInventory[i].GasReceived = parseFloat(GasInventory[i].GasReceived).toFixed(3);
            } else {
                $scope.GasInventory[i].GasReceived = "";
            }
            if (GasInventory[i].GasPrice && !angular.isUndefined(GasInventory[i].GasPrice)) {
                $scope.GasInventory[i].GasPrice = parseFloat(GasInventory[i].GasPrice).toFixed(3);
            } else {
                $scope.GasInventory[i].GasPrice = "";
            }
            if (GasInventory[i].StickInches && !angular.isUndefined(GasInventory[i].StickInches)) {
                $scope.GasInventory[i].StickInches = parseFloat(GasInventory[i].StickInches).toFixed(3);
            } else {
                $scope.GasInventory[i].StickInches = "";
            }
            if (GasInventory[i].StickGallons && !angular.isUndefined(GasInventory[i].StickGallons)) {
                $scope.GasInventory[i].StickGallons = parseFloat(GasInventory[i].StickGallons).toFixed(3);
            } else {
                $scope.GasInventory[i].StickGallons = "";
            }
            if (GasInventory[i].ActualClosingBalance && !angular.isUndefined(GasInventory[i].ActualClosingBalance)) {
                $scope.GasInventory[i].ActualClosingBalance = parseFloat(GasInventory[i].ActualClosingBalance).toFixed(3);
            } else {
                $scope.GasInventory[i].ActualClosingBalance = "";
            }
        }
    }
    //Sale Section Post
    $scope.saleSection = function (gasSale) {
        $scope.gasStation.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
        $scope.gasStation.ShiftCode = $("#repeatSelect").val();
        if($scope.gasStation.ShiftCode != "" && $scope.gasStation.ShiftCode != null && !angular.isUndefined($scope.gasStation.ShiftCode)){
            $scope.loading = true;
            var GasStationPostData = angular.toJson($scope.gasStation);
            GasStationPostData = JSON.parse(GasStationPostData);
            gasService.saveGasStationData(GasStationPostData).success(function (response) {
                $scope.saveInventory();
                sweetAlert("Success", "Store Sale Created Successfully", "success");
                $scope.loading = false;
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
                $scope.loading = false;
            });
        }else{
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        }
    }

    //Save Inventory BY POST    
    $scope.saveInventory = function () {
        $scope.inventory.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
        $scope.inventory.ShiftCode = $("#repeatSelect").val();
        if($scope.inventory.ShiftCode != "" && $scope.inventory.ShiftCode != null && !angular.isUndefined($scope.inventory.ShiftCode)){
            $scope.loading = true;
            if ($scope.GasInventory1.length > 0) {
                for (var i = 0; i < $scope.GasInventory1.length; i++) {
                    $scope.GasInventory.push($scope.GasInventory1[i]);
                }
                $scope.inventory.GasInventory = $scope.GasInventory;
            } else {
                $scope.inventory.GasInventory = $scope.GasInventory;
            }
            var GasInventoryPostData = angular.toJson($scope.inventory);
            GasInventoryPostData = JSON.parse(GasInventoryPostData);
            //console.log(GasInventoryPostData);
            gasService.saveGasInventoryData(GasInventoryPostData).success(function (response) {
                sweetAlert("Success", "Store Inventory Created Successfully", "success");
                $scope.loading = false;
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
                $scope.loading = false;
            });
        }else{
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        }
    }


    $scope.filterData = function (item) {
        return (item.GasTypeID != 7 && item.GasTypeID != 2);
    }
   

}]);

// $('.select2').select2({
//     allowClear: false,
// });