angular.module('GasApp').controller('ShiftEndController',['$scope', '$http', '$filter', '$rootScope', 'NgTableParams', 'Excel', 'businessService','gasService','lotteryService', function ($scope, $http, $filter, $rootScope, NgTableParams, Excel, businessService, gasService, lotteryService) {
    
    $scope.date = new Date();
    $scope.shifts = [];
    $scope.CashEntryDetails = [];
    $scope.lotterySaleReportData = [];
    $scope.GasReceipt1 = [];
    $scope.BankDetails = [];  
    $scope.PaymentAccounts = [];
    $scope.ledgerData = [];
    $scope.bankSave = true;
    var storeID = $rootScope.StoreID;
   
    //InSaleAccounts Object array sample
    $scope.cashentry = {
        "StoreID": $rootScope.StoreID,
        "Date": "",
        "ShiftCode": "",
        "CashOpeningBalance": "",
        "CashPhysicalAtStore": "",
        "CashClosingBalance": "",
        "ModifiedUserName": $rootScope.UserName
    };

    //Add Date & shift details to the Header bar
    businessService.getRunningShift(storeID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.myDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);
        $scope.currentShift = result.ShiftCode;
        $scope.ShiftCode = result.ShiftCode;
        $scope.getCashFlow($filter('date')($scope.myDate, 'dd-MMM-yyyy'),$scope.ShiftCode);
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
    $scope.getCashFlow = function (selectedDate,ShiftCode) {
        var postData = {};
        selectedDate = $filter('date')(selectedDate, 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.Date = selectedDate;
        postData.ShiftCode = ShiftCode;  
        //Calling getPreviousDayTranscations function to get Day sale data.
        businessService.getCashFlow(postData).success(function (response) {           
            if(response.length>0){
                $scope.CashEntryDetails = response;
                $scope.init($scope.CashEntryDetails);
            }else{
                sweetAlert("Error!!", response, "error");
                 $scope.CashEntryDetails = [];
            }
          
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }
    
    //Initializing a functino init() to calculate all loaded values
    $scope.init = function (BusinessSaleCollection) {
        $scope.BSaleSum = 0;
        $scope.BPaidSum = 0;
        $scope.Cash = 0;

        for (var i = 0; i < BusinessSaleCollection.length; i++) {
            if (BusinessSaleCollection[i].Amount && !angular.isUndefined(BusinessSaleCollection[i].Amount)) {
                if (BusinessSaleCollection[i].LedgerID != '40') {
                    if (BusinessSaleCollection[i].DisplaySide == "BS") {
                        $scope.BSaleSum += parseFloat(BusinessSaleCollection[i].Amount);
                    } else if (BusinessSaleCollection[i].DisplaySide == "BP") {
                        $scope.BPaidSum += parseFloat(BusinessSaleCollection[i].Amount);
                    }else if (BusinessSaleCollection[i].DisplaySide == "CB") {
                        $scope.Cash += parseFloat(BusinessSaleCollection[i].Amount);
                    }
                }

            }
        }

        $scope.bsTotal = $scope.BSaleSum.toFixed(2);
        $scope.bsGrandTotal = $scope.BSaleSum.toFixed(2);
        $scope.bpTotal = $scope.BPaidSum.toFixed(2);
        $scope.cashentry.CashClosingBalance = parseFloat(parseFloat($scope.bsTotal) - parseFloat($scope.bpTotal)).toFixed(2);
        $scope.getShortOrOver($scope.Cash);
    }

     //Getting short/over values
    $scope.getShortOrOver = function (amount) {
        if (!angular.isUndefined(amount)) {
            $scope.cashentry.CashPhysicalAtStore = parseFloat(amount).toFixed(2);
            $scope.shortorover = ($scope.cashentry.CashClosingBalance - amount).toFixed(2);
        } else {
            $scope.shortorover = (0 - $scope.cashentry.CashClosingBalance).toFixed(2);
        }
        $scope.bpGrandTotal = (parseFloat($scope.bpTotal) + parseFloat(($scope.bsTotal - $scope.bpTotal))).toFixed(2);
    }

    $scope.saveCashEntry = function () {
        if ($scope.myDate != null) {
            $scope.ShiftCode = $("#repeatSelect").val();
            if ($scope.ShiftCode != "" && $scope.ShiftCode != null && !angular.isUndefined($scope.ShiftCode)) {
                if ($scope.CashEntryDetails.length > 0) {
                    $scope.cashentry.Date = $filter('date')($scope.myDate, 'dd-MMM-yyyy');
                    $scope.cashentry.ShiftCode = $scope.ShiftCode;
                    $scope.cashentry.CashOpeningBalance = $scope.CashEntryDetails[0].Amount;
                    var CashEntryPostData = angular.toJson($scope.cashentry);
                    CashEntryPostData = JSON.parse(CashEntryPostData);
                    businessService.saveCashEntry(CashEntryPostData).success(function (response) {
                        sweetAlert("Success", "Cash Entry Saved Successfully", "success");
                    }).error(function (response) {
                        sweetAlert("Error!!", response, "error");
                    });
                }
            } else {
                sweetAlert("Error!!", 'Please Select Shift Code', "error");
            }
        } else {
            sweetAlert("Error!!", 'Please Select Date', "error");
        }
    }    

    $scope.getDetailedReport = function(LedgerName){
    	$scope.headerTitle = LedgerName;
        var postData = {};
        repSalefromDate = $filter('date')(new Date($scope.myDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date($scope.myDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        console.log(postData);
        if($scope.headerTitle == "Gas Sale"){
            postData.FromDate = repSalefromDate;
            postData.ToDate = repSaleToDate;
            gasService.getGasSaleReport(postData).success(function (response) {
                console.log(response);
                if (response.length <= 0) {
                    $scope.ledgerData = [];
                } else {
                    $scope.ledgerData = response;
                }

                $scope.tableParams = new NgTableParams({
                },{
                    dataset: $scope.ledgerData
                });
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }else if($scope.headerTitle == "Lottery Sale"){
            postData.FromDate = repSalefromDate;
            postData.ToDate = repSaleToDate;
            lotteryService.getLotterySaleReport(postData).success(function (response) {
                if (response.length <= 0) {
                    $scope.lotterySaleReportData = [];
                } else {
                    $scope.lotterySaleReportData = response;
                    $scope.tableParams = new NgTableParams({
                        sorting: { Date: "asc", ShiftCode: "asc", InstantSale: "asc", OnlineSale: "asc", BookActive: "asc", CashPaid: "asc", TotalSale: "asc" }
                    }, {
                        dataset: $scope.lotterySaleReportData
                    });
                    $scope.grandTotal = $scope.sum($scope.lotterySaleReportData, 'TotalSale');
                }
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }else if($scope.headerTitle == "Card Receipt"){
            postData.Date = repSalefromDate;
            postData.ShiftCode = $scope.ShiftCode; 
            postData.RequestType = 'GAS-STORE-SALE';
            gasService.getPreviousDayTranscations(postData).success(function (response) {
                 if (response == "No Data Found") {
                        $scope.GasReceipt1 = [];
                 }else {
                    if (response.GasReceipt.length <= 0) {
                        $scope.GasReceipt1 = [];
                    }else{
                        $scope.GasReceipt1 = response.GasReceipt;
                        $scope.tableParams = new NgTableParams({
                            sorting: { CardName: "asc", CardAmount: "asc"}
                        }, {
                            dataset: $scope.GasReceipt1
                        });
                        //Card Section
                        $scope.CardTotal = parseFloat(response.CardTotal).toFixed(2);
                    }
                            
                }
                   
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
                
            });
        }else if($scope.headerTitle == "Total Bank Deposit"){
            postData.FromDate = repSalefromDate;
            postData.ToDate = repSaleToDate;
            postData.ShiftCode = $scope.ShiftCode;        
            //Calling getPreviousDayTranscations function to get Day sale data.
            businessService.getCashDeposit(postData).success(function (response) {
               //Get SaleDelivery Section
                if (response.LedgerDetail.length <= 0) {
                    $scope.BankDetails = [];                             
                }else{
                    $scope.BankDetails = response.LedgerDetail;
                    $scope.tableParams = new NgTableParams({
                        sorting: { LedgerName: "asc", LedgerSale: "asc", LedgerPaid: "asc", Balance: "asc", Deposit: "asc"}
                    }, {
                        dataset: $scope.BankDetails
                    });
                }

            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }else if($scope.headerTitle == "Cash Paid"){
            postData.Date = repSalefromDate;
            postData.RequestType = 'PAYMENTS';
            postData.ShiftCode = $scope.ShiftCode;        
            //Calling getPreviousDayTranscations function to get Day sale data.
             gasService.getPreviousDayTranscations(postData).success(function (response) {
                 if (response == "No Data Found") {
                        $scope.PaymentAccounts = [];
                 }else {
                    if (response.PaymentAccounts.length <= 0) {
                        $scope.PaymentAccounts = [];
                    }else{
                        $scope.PaymentAccounts = response.PaymentAccounts;
                        $scope.tableParams = new NgTableParams({
                            sorting: { PaymentRemarks: "asc", AccountTranType: "asc", Amount: "asc"}
                        }, {
                            dataset: $scope.PaymentAccounts
                        });
                    }        
                }
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }
    }    


    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'GasSaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'GasSaleReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.sum = function (items, prop) {
        return items.reduce(function (a, b) {
            return a + b[prop];
        }, 0);
    };

    $scope.exportToExcel1 = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'LotterySaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'LotterySaleReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.exportToExcel2 = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'BankDepositReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'BankDepositReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.exportToExcel3 = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'CardReceiptReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'CardReceiptReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

     $scope.exportToExcel4 = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'CashPaidReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'CashPaidReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);