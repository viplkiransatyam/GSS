angular.module('GasApp').controller('PurchasesController', ['$scope', '$http', '$filter', '$rootScope', 'gasService', function ($scope, $http, $filter, $rootScope, gasService) {
    $scope.date = new Date();
    var storeID = $rootScope.StoreID;
    $scope.purchasesSave = false;
    $scope.shifts = [];
    $scope.GasInventory = [];
    $scope.GasTax = [];
    $scope.GasDefaultTax = [];

    //GasSale Object array sample
    $scope.purchaseInvoice = {
        "StoreID": $rootScope.StoreID,
        "Date": "",        
        "ShiftCode": "",
        "CreatedUserName": $rootScope.UserName,
        "CreateTimeStamp": "",
        "ModifiedUserName": $rootScope.UserName,
        "ModifiedTimeStamp": "",
        "TransactionID":"",
        "GasInventory": [
            {
                "SlNo": "",
                "GasTypeID": "",                
                "GrossInvGallons": "",
                "NetInvGallons": "",
                "Price": "",
                "Amount": ""
            }
        ],
        "GasTax": [
           {
               "TaxId": "",
               "TaxAmount": ""
           }
        ]
    };

    $scope.loading = true;
    //Getting Date value from tabs. 
    if (!angular.isUndefined($rootScope.changedDate)) {
        $scope.myDate = $rootScope.changedDate;
    } else {
        $scope.myDate = new Date();
        $scope.myDate.setDate($scope.myDate.getDate() - 1);
    }

    //Checking Store Type
    var getType = function () {
        if ($rootScope.StoreType == "GS") {
            $scope.storeType = true;
        } else if ($rootScope.StoreType == "LS") {
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


   


    //Getting Gas Purchases for the Day
    $scope.getBillDetails = function () {
        $scope.ShiftCode = $("#repeatSelect").val();
        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
            if ($scope.purchaseInvoice.BillOfLading != null) {
                var postData = {};
                postData.StoreID = storeID;
                postData.BOL = $scope.purchaseInvoice.BillOfLading;
                gasService.getGasPurchases(postData).success(function (result) {
                    console.log(result);
                    if (result.BillOfLading != null) {                        
                        $scope.dueDate = $filter('date')(result.DueDate, 'MM/dd/yyyy');
                        $scope.receiptDate = $filter('date')(result.ReceiptDate, 'MM/dd/yyyy');
                        $scope.purchaseInvoice.TransactionID = result.TransactionID;
                       
                        if (result.InvDate == "0001-01-01T00:00:00") {
                            $scope.purchaseInvoice.InvDate = $filter('date')(result.InvDate, 'dd-MMM-yyyy');
                        } else {
                            $scope.Day = $filter('date')(result.InvDate, 'dd');
                            $scope.Month = $filter('date')(result.InvDate, 'MM');
                            $scope.Year = $filter('date')(result.InvDate, 'yyyy');
                            $scope.purchaseInvoice.InvDate = new Date($scope.Year, $scope.Month-1, $scope.Day);
                        }
                        
                        $scope.purchaseInvoice.InvAmount = parseFloat(result.InvAmount).toFixed(2);
                        $scope.purchaseInvoice.InvNo = result.InvNo;
                        $scope.GasInventory = [];
                        $scope.GasDefaultTax = [];
                        if (result.GasInventory.length > 0) {
                            for (var i = 0; i < result.GasInventory.length; i++) {
                                $scope.GasInventory.push({
                                    SlNo: result.GasInventory[i].SlNo,
                                    GasTypeID: result.GasInventory[i].GasTypeID,
                                    GasTypeName: result.GasInventory[i].GasTypeName,
                                    /*GrossGallons: parseFloat(result.GasInventory[i].GrossGallons).toFixed(3),
                                    NetGallons: parseFloat(result.GasInventory[i].NetGallons).toFixed(3),*/
                                    Price: parseFloat(result.GasInventory[i].Price).toFixed(8),
                                    Amount: parseFloat(result.GasInventory[i].Amount).toFixed(2),
                                    GrossInvGallons: (result.GasInventory[i].GrossInvGallons) ? parseFloat(result.GasInventory[i].GrossInvGallons).toFixed(3) : parseFloat(result.GasInventory[i].GrossGallons).toFixed(3),
                                    NetInvGallons: (result.GasInventory[i].NetInvGallons) ? parseFloat(result.GasInventory[i].NetInvGallons).toFixed(3) : parseFloat(result.GasInventory[i].NetGallons).toFixed(3)
                                });
                            }                           
                        }else{
                            $scope.GasInventory = [];
                        }

                        if (result.GasDefaultTax.length > 0) {
                            if (result.GasTax.length > 0) {
                                for (var i = 0; i < result.GasDefaultTax.length; i++) {
                                    for (var j = 0; j < result.GasTax.length; j++) {
                                        if (result.GasDefaultTax[i].TaxId == result.GasTax[j].TaxId) {
                                            $scope.GasDefaultTax.push({
                                                TaxId: result.GasDefaultTax[i].TaxId,
                                                TaxName: result.GasDefaultTax[i].TaxName,
                                                TaxAmount: result.GasTax[j].TaxAmount,
                                            });
                                        }                                       
                                    }                                    
                                }

                            }else{
                                for (var i = 0; i < result.GasDefaultTax.length; i++) {
                                    $scope.GasDefaultTax.push({
                                        TaxId: result.GasDefaultTax[i].TaxId,
                                        TaxName: result.GasDefaultTax[i].TaxName,
                                    });
                                }
                            }                            
                        }else{
                            $scope.GasDefaultTax = [];
                        }                       
                        $scope.loading = false;
                    } else {
                        sweetAlert("Error!!", "No data for this Bill of Ladding ", "error");
                        $scope.dueDate = null;
                        $scope.receiptDate = null;
                        $scope.purchaseInvoice.BillOfLading = null;
                        $scope.purchaseInvoice.TransactionID = null;
                        $scope.purchaseInvoice.InvDate = null;
                        $scope.purchaseInvoice.InvAmount = null;
                        $scope.purchaseInvoice.InvNo = null;
                        $scope.GasInventory = [];
                        $scope.GasDefaultTax = [];
                    }

                }).error(function () {
                    sweetAlert("Error!!", "Error in Getting Gas Purchases", "error");
                    $scope.dueDate = null;
                    $scope.receiptDate = null;
                    $scope.purchaseInvoice.BillOfLading = null;
                    $scope.purchaseInvoice.TransactionID = null;
                    $scope.purchaseInvoice.InvDate = null;
                    $scope.purchaseInvoice.InvAmount = null;
                    $scope.purchaseInvoice.InvNo = null;
                    $scope.GasInventory = [];
                    $scope.GasDefaultTax = [];
                    $scope.loading = false;
                });
            } else {
                $scope.loading = false;
                sweetAlert("Error!!", 'Please Enter Bill of Lading', "error");
            }
            
        } else {
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        }
    }

    //putting 8 decimals to Price
    $scope.getPriceDecimals = function (GasInventory) {
        console.log(GasInventory);
        if (GasInventory.length > 0) {
            for (var i = 0; i < GasInventory.length; i++) {                
                if (GasInventory[i].Price != "" && !angular.isUndefined(GasInventory[i].Price)) {
                    if (GasInventory[i].NetInvGallons != "" && !angular.isUndefined(GasInventory[i].NetInvGallons)) {
                        $scope.GasInventory[i].Amount = parseFloat(parseFloat(GasInventory[i].NetInvGallons) * parseFloat(GasInventory[i].Price)).toFixed(2);
                        $scope.GasInventory[i].Price = parseFloat(GasInventory[i].Price).toFixed(8);
                        $scope.getAmountCalculations(GasInventory);
                    } else {
                        sweetAlert("Error!!", 'Please Enter Net Gallons', "error");
                        $scope.GasInventory[i].Price = "";
                        $scope.GasInventory[i].Amount = "";
                    }
                    
                } else {
                    $scope.GasInventory[i].Price = "";
                }
            }
        }
    }
    
    //Amount Decimals and Total calculations
    $scope.getAmountCalculations = function (GasInventory) {
        var inventorytotal = 0;
        var taxtotal = 0;
        if (GasInventory.length > 0) {
            for (var i = 0; i < GasInventory.length; i++) {
                if (GasInventory[i].Amount != "" && !angular.isUndefined(GasInventory[i].Amount)) {
                    $scope.GasInventory[i].Amount = parseFloat(GasInventory[i].Amount).toFixed(2);
                    inventorytotal += parseFloat(GasInventory[i].Amount);
                } else {
                    $scope.GasInventory[i].Amount = "";
                    inventorytotal += parseFloat(0);
                }
            }
        }
        if ($scope.GasDefaultTax.length > 0) {
            for (var i = 0; i < $scope.GasDefaultTax.length; i++) {
                if ($scope.GasDefaultTax[i].TaxAmount != "" && !angular.isUndefined($scope.GasDefaultTax[i].TaxAmount)) {
                    $scope.GasDefaultTax[i].TaxAmount = parseFloat($scope.GasDefaultTax[i].TaxAmount).toFixed(2);
                    taxtotal += parseFloat($scope.GasDefaultTax[i].TaxAmount);
                } else {
                    $scope.GasDefaultTax[i].TaxAmount = "";
                    taxtotal += parseFloat(0);
                }
            }
        }
        $scope.purchaseInvoice.InvAmount = parseFloat(parseFloat(inventorytotal) + parseFloat(taxtotal)).toFixed(2);
    }

    //Amount Decimals and Total calculations
    $scope.getTaxesTotal = function (GasDefaultTax) {
        var inventorytotal = 0;
        var taxtotal = 0;
        if ($scope.GasDefaultTax.length > 0) {
            for (var i = 0; i < GasDefaultTax.length; i++) {
                if (GasDefaultTax[i].TaxAmount != "" && !angular.isUndefined(GasDefaultTax[i].TaxAmount)) {
                    $scope.GasDefaultTax[i].TaxAmount = parseFloat(GasDefaultTax[i].TaxAmount).toFixed(2);                   
                    taxtotal += parseFloat($scope.GasDefaultTax[i].TaxAmount);
                } else {
                    $scope.GasDefaultTax[i].TaxAmount = "";
                    taxtotal += parseFloat(0);
                }
            }
        }

        if ($scope.GasInventory.length > 0) {
            for (var i = 0; i < $scope.GasInventory.length; i++) {
                if ($scope.GasInventory[i].Amount != "" && !angular.isUndefined($scope.GasInventory[i].Amount)) {
                    $scope.GasInventory[i].Amount = parseFloat($scope.GasInventory[i].Amount).toFixed(2);
                    inventorytotal += parseFloat($scope.GasInventory[i].Amount);
                } else {
                    $scope.GasInventory[i].TaxAmount = "";
                    inventorytotal += parseFloat(0);
                }
            }
        }
        $scope.purchaseInvoice.InvAmount = parseFloat(parseFloat(inventorytotal)+ parseFloat(taxtotal)).toFixed(2);
    }

    $scope.savePurchases = function () {
        $scope.ShiftCode = $("#repeatSelect").val();
        if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
        $scope.loading = true;
            $scope.purchaseInvoice.GasInventory = $scope.GasInventory;
            if ($scope.GasDefaultTax.length > 0) {
                for (var i = 0; i < $scope.GasDefaultTax.length; i++) {
                    if ($scope.GasDefaultTax[i].TaxAmount != "" && !angular.isUndefined($scope.GasDefaultTax[i].TaxAmount)) {                   
                        $scope.GasTax.push({
                            "TaxId": $scope.GasDefaultTax[i].TaxId,
                            "TaxAmount": $scope.GasDefaultTax[i].TaxAmount
                        });
                    }
                }
            }
            $scope.purchaseInvoice.GasTax = $scope.GasTax;        
            var PurchaseInvoiceData = angular.toJson($scope.purchaseInvoice);
            PurchaseInvoiceData = JSON.parse(PurchaseInvoiceData);
            console.log(PurchaseInvoiceData);
            gasService.savePurchaseInvoiceData(PurchaseInvoiceData).success(function (response) {
                $scope.loading = false;
                sweetAlert("Success", "Purhcase Invoice Saved Successfully", "success");
            }).error(function (response) {
                $scope.loading = false;
                sweetAlert("Error!!", response, "error");
            });
        } else {
            $scope.loading = false;
            sweetAlert("Error!!", 'Please Select Shift Code', "error");
        }
    }
   

}]);