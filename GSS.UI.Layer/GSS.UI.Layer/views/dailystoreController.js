app.controller('DailyStoreController', ['$scope', '$http', '$filter', '$rootScope', 'loginService', 'authService', function ($scope, $http, $filter, $rootScope, loginService, authService) {
    $scope.date = new Date();
    //$scope.myDate = new Date();
	if (!angular.isUndefined($rootScope.changedDate)) {
        $scope.myDate= $rootScope.changedDate;
    } else {
        $scope.myDate = new Date();
    }
    $scope.username = $rootScope.UserName;
    var storeID = $rootScope.StoreID;
    $scope.checkValidation = true;
    $scope.GasSale = [];
    $scope.GasSaleData = [];
    $scope.TankDetail = [];   
    $scope.GasInventory = [];
    $scope.GasReceipt = [];
    $scope.SystemClosingBal = [];
    $scope.saleSectionSave = true;
    $scope.gasInventory = false;
    $scope.GassaveOnlineAmount = false;
    $scope.gasStation = {
        "StoreID": $rootScope.StoreID,
        "Date":"",
        "TotalSaleGallons": "",
        "TotalTotalizer": "",
        "TotalSale": "",
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
                "SalePrice": ""
            }            
        ]
    };
    
    $scope.inventory = {
        "StoreID": $rootScope.StoreID,
        "Date": "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "GasInventory": [
            {
                "TankID": "",
                "GasTypeID": "",
                "OpeningBalance": "",
                "GasReceived": "",
                "ActualClosingBalance": "",
                "StickInches": "",
                "StickGallons": ""
            }            
        ]
    }

    $scope.gasCards = {
        "StoreID": $rootScope.StoreID,
        "Date": "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "GasReceipt": [
            {
                "CardTypeID": "",
                "CardAmount": "",
            }
        ]
    };
    $scope.IsAdmin = function () {
        return loginService.IsAdmin();
    }
    $scope.IsStoreManager = function () {
        return loginService.IsStoreManager();
    }
    $scope.logout = function () {
        $scope.loading = true;
        loginService.logout();
    }

    // Getting data from Day Sale while loading the Form
    var postData = {};
    storeID = $rootScope.StoreID;
    selectedDate = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
    postData.StoreID = storeID;
    postData.Date = selectedDate;
    $scope.loading = true;
    var getDaySale = function (postData) {
        authService.getPreviousDayTranscations(postData).success(function (response) {
           // console.log(response);  
            $scope.SystemClosingBal = [];
            $scope.inventory.SystemClosingBal = [];
            if (response == "No Data Found") {
                //clearing the data 
                for (i = 0; i < $scope.GasSale.length; i++) {                    
                    $scope.GasSale[i].SaleAmount = "";
                    $scope.GasSale[i].SaleGallons = "";
                    $scope.GasSale[i].SalePrice = "";
                    $scope.GasSale[i].Totalizer = "";
                }
                for (i = 0; i < $scope.GasInventory.length; i++) {                    
                    $scope.GasInventory[i].OpeningBalance = "";
                    $scope.GasInventory[i].GasReceived = "";
                    $scope.GasInventory[i].ActualClosingBalance = "";
                    $scope.GasInventory[i].StickInches = "";
                    $scope.GasInventory[i].StickGallons = "";
                }
                for (i = 0; i < $scope.GasReceipt.length; i++) {                   
                    $scope.GasReceipt[i].CardAmount = "";                   
                }
                //Sale Section
                $scope.gasStation.TotalTotalizer = "";
                $scope.gasStation.TotalSaleGallons = "";
                $scope.gasStation.TotalSale = "";
                //Card Section
                $scope.gasCards.CardTotal = "";
                $scope.gasCards.Cash = "";
                $scope.checkValidation = true;               
                $scope.saleSectionSave = true;
                $scope.gasInventory = false;
                $scope.GassaveOnlineAmount = false;
            } else {
                if (response.EntryLockStatus == "Y") {
                    $scope.saleSectionSave = true;
                    $scope.gasInventory = true;
                    $scope.GassaveOnlineAmount = true;
                } else {
                    $scope.saleSectionSave = false;
                    $scope.gasInventory = false;
                    $scope.GassaveOnlineAmount = false;
                }
                if (response.GasSale.length > 0) {               
                    for (i = 0; i < response.GasSale.length; i++) {
                        for (j = 0; j < $scope.GasSale.length; j++) {
                            if ($scope.GasSale[j].GasTypeID == response.GasSale[i].GasTypeID) {
                                $scope.GasSale[j].GasTypeID = response.GasSale[i].GasTypeID;
                                $scope.GasSale[j].GasTypeName = response.GasSale[i].GasTypeName;
                                $scope.GasSale[j].SaleAmount = response.GasSale[i].SaleAmount;
                                $scope.GasSale[j].SaleGallons = response.GasSale[i].SaleGallons;
                                $scope.GasSale[j].SalePrice = response.GasSale[i].SalePrice;
                                $scope.GasSale[j].Totalizer = response.GasSale[i].Totalizer;
                            }
                        }                                
                        }
                } else {
                    for (i = 0; i < $scope.GasSale.length; i++) {
                        $scope.GasSale[i].SaleAmount = "";
                        $scope.GasSale[i].SaleGallons = "";
                        $scope.GasSale[i].SalePrice = "";
                        $scope.GasSale[i].Totalizer = "";
                    }                    
                }
                    if (response.GasInventory.length > 0) {
                        for (i = 0; i < response.GasInventory.length; i++) {
                            for (j = 0; j < $scope.GasInventory.length; j++) {
                                if ($scope.GasInventory[j].GasTypeID != 2) {
                                    if ($scope.GasInventory[j].GasTypeID == response.GasInventory[i].GasTypeID) {
                                        $scope.GasInventory[j].TankID = response.GasInventory[i].TankID;
                                        $scope.GasInventory[j].GasTypeID = response.GasInventory[i].GasTypeID;
                                        $scope.GasInventory[j].OpeningBalance = response.GasInventory[i].OpeningBalance;
                                        $scope.GasInventory[j].GasReceived = response.GasInventory[i].GasReceived;
                                        $scope.GasInventory[j].ActualClosingBalance = response.GasInventory[i].ActualClosingBalance;
                                        $scope.GasInventory[j].StickInches = response.GasInventory[i].StickInches;
                                        $scope.GasInventory[j].StickGallons = response.GasInventory[i].StickGallons;
                                        $scope.GasInventory[j].GasPrice = response.GasInventory[i].GasPrice;
                                    }
                                }
                            }
                        }
                    } else {
                        for (i = 0; i < $scope.GasInventory.length; i++) {                            
                            $scope.GasInventory[i].GasReceived = "";
                            $scope.GasInventory[i].ActualClosingBalance = "";
                            $scope.GasInventory[i].StickInches = "";
                            $scope.GasInventory[i].StickGallons = "";
                        }
                    }
                    if (response.GasReceipt.length > 0) {
                        for (i = 0; i < response.GasReceipt.length; i++) {
                            for (j = 0; j < $scope.GasReceipt.length; j++) {
                                if ($scope.GasReceipt[j].CardTypeID == response.GasReceipt[i].CardTypeID) {
                                    $scope.GasReceipt[j].CardTypeID = response.GasReceipt[i].CardTypeID;
                                    $scope.GasReceipt[j].CardAmount = response.GasReceipt[i].CardAmount;
                                    $scope.GasReceipt[j].CardName = response.GasReceipt[i].CardName;
                                }
                            }
                        }
                        
                    } else {
                        for (i = 0; i < $scope.GasReceipt.length; i++) {
                            $scope.GasReceipt[i].CardAmount = "";
                        }
                    }
                    //getting data into the total values
                    //Sale Section
                    $scope.gasStation.TotalTotalizer = response.TotalTotalizer;
                    $scope.gasStation.TotalSaleGallons = response.TotalSaleGallons;
                    $scope.gasStation.TotalSale = response.TotalSale;
                    //Card Section
                    $scope.gasCards.CardTotal = response.CardTotal;
                    $scope.gasCards.Cash = ($scope.gasStation.TotalSale - $scope.gasCards.CardTotal).toFixed(3);
                    $scope.checkValidation = false;
                    $scope.getClosingBalance();  
                    
            }
            $scope.loading = false;
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }
    //calling the below method on page load
    getDaySale(postData);
   
    // Getting data from Day Sale after changing the data from datepicker
    var getSaleData = {};
    $scope.getDaySaleDetails = function (selectedDate) {
        $scope.loading = true;
		$rootScope.changedDate = $scope.myDate;
        storeID = $rootScope.StoreID;
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');       
        getSaleData.StoreID = storeID;
        getSaleData.Date = selectedDate;
        //calling it again after date selection
        getDaySale(getSaleData);
    }

    
    var gasData = {};
    var getStoresAndGasOpeningBalance = authService.getStoreName(storeID).success(function (result) {        
        $scope.StoreName = result;       
        for(var i=0; i<result.TankDetail.length;i++){
            $scope.TankDetail.push({
                "TankID": result.TankDetail[i].TankID,
                "TankName": result.TankDetail[i].TankName
            });
            //To get GasOpeningBalance
            gasData.StoreId = $rootScope.StoreID;
            gasData.TankID = result.TankDetail[i].TankID;
            gasData.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
            //console.log(gasData);
;            authService.getGasopeningBalance(gasData).success(function (result) {
                //console.log(result);
                for (var i = 0; i < result.length; i++) {
                    //if (result[i].GasTypeID == 2) {
                    //    //Harded the gastype ID need to change at runtime
                    //} else {
                        $scope.GasInventory.push({
                            "TankID": result[i].TankID,//tankid is getting 0 from the server
                            "GasTypeID": result[i].GasTypeID,
                            "GasTypeName": result[i].GasTypeName,
                            "OpeningBalance": result[i].OpeningBalance
                        });
                    //}                    
                }
                $scope.inventory.GasInventory = $scope.GasInventory;
                //console.log($scope.inventory.GasInventory);
               // $scope.loading = false;
            }).error(function (result) {
                sweetAlert('Error!!', result, 'error');
                $scope.loading = false;
               // location.reload();
            });
        }        
    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        $scope.loading = false;
        //location.reload();
    });
	
	getStoresAndGasOpeningBalance();
    $scope.loading = true;
    authService.getStorebasedGas(storeID).success(function (result) {        
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
        $scope.gasStation.GasSale = $scope.GasSale;
       // $scope.loading = false;
    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        $scope.loading = false;
        //location.reload();
    });
    $scope.loading = true;
    authService.getCardsPerStore(storeID).success(function (result) {        
        for (var i = 0; i < result.length; i++) {
            $scope.GasReceipt.push({
                'CardTypeID': result[i].CardTypeID,
                'CardTypeName': result[i].CardTypeName
            });
        }        
        $scope.gasCards.GasReceipt = $scope.GasReceipt;
        //$scope.loading = false;
    }).error(function (result) {
        sweetAlert('Error!!', result, 'error');
        $scope.loading = false;
        //location.reload();
    });

    $scope.getgasCardsTotal = function (GasReceipt) {        
        var total = 0;       
        $scope.GasReceipt.forEach(function (cards) {
            if (cards.CardAmount) {
                total += cards.CardAmount;
            }
        })   
        $scope.gasCards.CardTotal = total.toFixed(3);
        if ($scope.gasStation.TotalSale) {
            $scope.gasCards.Cash = ($scope.gasStation.TotalSale - $scope.gasCards.CardTotal).toFixed(3);
        } else {
            $scope.gasStation.TotalSale = 0;
            sweetAlert('Warning!!', 'Please check the Sale section Amounts', 'warning');
            $scope.gasCards.Cash = ($scope.gasStation.TotalSale - $scope.gasCards.CardTotal).toFixed(3);           
        }
    }

    $scope.calculatePerGallon = function (GasSale) {       
        var sum = 0;
        for (var i = 0; i < GasSale.length; i++) {            
            if (GasSale[i].SaleAmount) {
                if (GasSale[i].Totalizer == null) {
                    sweetAlert('Warning!!', 'Please Fill Totalizer', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                } else if (GasSale[i].SaleGallons == null) {
                    sweetAlert('Warning!!', 'Please Fill Gallons', 'warning');
                    GasSale[i].SaleAmount = '';
                    break;
                } else {
                    sum += GasSale[i].SaleAmount;
                    var SalePrice = GasSale[i].SaleAmount / GasSale[i].SaleGallons;
                    GasSale[i].SalePrice = parseFloat(SalePrice.toFixed(3));
                }
            }
         } 
        $scope.gasStation.TotalSale = sum.toFixed(3);
        $scope.saleSectionSave = false;
    }
    $scope.validateTotalizer = function (GasSale) {        
        var sum = 0;
        for (var i = 0; i < GasSale.length; i++) {
            if (GasSale[i].SaleGallons) {
                if (GasSale[i].Totalizer == null) {
                    sweetAlert('Warning!!', 'Please Fill Totalizer', 'warning');
                    GasSale[i].SaleGallons = '';
                    break;
                } else {
                    sum += GasSale[i].SaleGallons;
                }
            }
        }
        $scope.gasStation.TotalSaleGallons = sum.toFixed(3);
        $scope.saleSectionSave = true;
    }    
    $scope.grandtotalTotalizer = function (GasSale) {
        var sum = 0;
        $scope.GasSale.forEach(function (gas) {
            if (gas.Totalizer) {
                sum += gas.Totalizer;
            }
        })        
        $scope.gasStation.TotalTotalizer = sum.toFixed(3);
        $scope.saleSectionSave = true;
    }    
    

    //Save Selection BY POST
    $scope.saleSection = function (gasSale) {        
            $scope.gasStation.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
            $scope.loading = true;
            var GasStationPostData = angular.toJson($scope.gasStation);
            GasStationPostData = JSON.parse(GasStationPostData);
            authService.saveGasStationData(GasStationPostData).success(function (response) {
                sweetAlert("Success", "Store Sale Created Successfully", "success");
                $scope.loading = false;
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
                $scope.loading = false;
            });
    }

    var result;
    $scope.enableClosingBalButton = function (GasInventory) {        
        for (var j = 0; j < $scope.GasSale.length; j++) {
            if ($scope.GasSale[j].Totalizer) {                
                result = false;
                break;
            }
            else {
                result = true;               
            }
        }       
        //for (var i = 0; i < GasInventory.length; i++) {            
        //    if (result == true) {
        //        result = true;
        //    } else if(result == false){
        //        if ($scope.GasInventory[i].GasReceived && $scope.GasInventory[i].ActualClosingBalance) {
        //            result = false;
        //        } else {
        //            result = true;
        //        }
        //    }           
        //}        
        $scope.checkValidation = result;
    }
    
    //To get System Gas Account Balance
    //StoreId,GasTypeID,SaleQty,GasopeningBalance,Recieved,Actualgasclosingbalance    
    $scope.getClosingBalance = function () {       
        $scope.SystemClosingBalance = [];         
        for (var i = 0; i < $scope.GasSale.length; i++) {
           // if ($scope.GasSale[i].GasTypeID != 2) {                
                for (var j = 0; $scope.GasInventory.length; j++) {                  
                    if ($scope.GasSale[i].GasTypeID == $scope.inventory.GasInventory[j].GasTypeID) {
                        $scope.SystemClosingBalance.push({
                            'StoreID': $rootScope.StoreID,
                            'GasTypeID': $scope.GasSale[i].GasTypeID,
                            'SaleQty': $scope.GasSale[i].SaleGallons,
                            'GasOpeningBalance': $scope.inventory.GasInventory[j].OpeningBalance,
                            'Received': $scope.inventory.GasInventory[j].GasReceived,
                            'ActualClosingBalance': $scope.inventory.GasInventory[j].ActualClosingBalance
                        });
                        break;
                    }
                }
           // }
        }
      
        var SystemGasBalance = angular.toJson($scope.SystemClosingBalance);        
        SystemGasBalance = JSON.parse(SystemGasBalance);       
        $scope.loading = true;
        authService.getSystemClosingBalance(SystemGasBalance).success(function (result) {           
            if ($scope.SystemClosingBal.length == 0) {               
            } else if ($scope.SystemClosingBal.length != 0) {
                $scope.SystemClosingBal = [];
                $scope.inventory.SystemClosingBal = [];
            }            
            for (var i = 0; i < result.length; i++) {              
                $scope.SystemClosingBal.push({
                    "SystemClosingBalance": result[i].SystemClosingBalance.toFixed(3),
                    "ShortOrExcess": result[i].ShortOrExcess.toFixed(3),
                    "GasTypeID": result[i].GasTypeID,
                });
            }           
            $scope.inventory.SystemClosingBal = $scope.SystemClosingBal;            
            $scope.loading = false;
        }).error(function (result) {
            sweetAlert("Error!!", result, "error");
            $scope.loading = false;           
        });
    }


    //Save Inventory BY POST    
    $scope.saveInventory = function () {
        $scope.inventory.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
        $scope.loading = true;
        var GasInventoryPostData = angular.toJson($scope.inventory);
        GasInventoryPostData = JSON.parse(GasInventoryPostData);
        authService.saveGasInventoryData(GasInventoryPostData).success(function (response) {            
            sweetAlert("Success", "Store Inventory Created Successfully", "success");
            $scope.loading = false;
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }

    //Save BankCards BY POST
    $scope.saveOnlineAmount = function () {
        $scope.gasCards.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
        $scope.loading = true;
        var GasCardsPostData = angular.toJson($scope.gasCards);        
        GasCardsPostData = JSON.parse(GasCardsPostData);        
        authService.saveGasCardsData(GasCardsPostData).success(function (response) {            
            sweetAlert("Success", "Gas Cards Amount Saved Successfully", "success");
            $scope.loading = false;
        }).error(function (response) {
            sweetAlert("Error!!",response, "error");
            $scope.loading = false;
        });
    }

}]);