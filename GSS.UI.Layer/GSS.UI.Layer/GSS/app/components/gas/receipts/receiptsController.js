angular.module('GasApp').controller('ReceiptsController', ['$scope', '$http', '$filter', '$rootScope', 'gasService', function ($scope, $http, $filter, $rootScope, gasService) {
    $scope.date = new Date();
    var storeID = $rootScope.StoreID;
    $scope.receiptsSave = true;
    $scope.TransactionTypes = [];
    $scope.GasDealerTransRows = [];
    $scope.receiptsInput = {};    
    $scope.HasKey = null;
    $scope.receipts = {
        "StoreID": storeID,
        "TranID": "",
        "ReferenceNo": "",
        "ReferenceDate": $filter('date')($scope.date, 'dd-MMM-yyyy'),
        "CreatedUserName": $rootScope.UserName,
        "ModifiedUserName": $rootScope.UserName,
        "GasDealerTransRows": [{
            "ReferenceTranNo": "",
            "ReferenceTranDate": "",
            "TranTypeIndicator": "",
            "TranAmount": null
        }], 
    };

    //$scope.loading = true;
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

    //Getting TransactionTypes  
    gasService.getTransactionTypes().success(function (result) {
        $scope.TransactionTypes = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "Transaction Types Error", "error");
        $scope.loading = false;
    });

    //Getting Gas Purchases for the Day
    $scope.getReceiptDetails = function () {
        if ($scope.receipts.ReferenceNo != null) {
            var postData = {};
            postData.StoreID = storeID;
            postData.ReferenceNo = $scope.receipts.ReferenceNo;
            gasService.getGasReceipts(postData).success(function (result) {
                $scope.receiptsSave = false;
                //if (result.GasDealerTransRows.length > 0) {
                    //$scope.receipts.StoreID = result.StoreID;
                    $scope.receipts.TranID = result.TranID;
                    //$scope.receipts.ReferenceNo = result.ReferenceNo;
                    $scope.receipts.ReferenceDate = $filter('date')(result.ReferenceDate, 'dd-MMM-yyyy');
                    $scope.GasDealerTransRows = [];
                    for (var i = 0; i < result.GasDealerTransRows.length; i++) {
                        $scope.GasDealerTransRows.push({
                            SlNo: result.GasDealerTransRows[i].SlNo,
                            ReferenceTranNo: result.GasDealerTransRows[i].ReferenceTranNo,
                            ReferenceTranDate: $filter('date')(result.GasDealerTransRows[i].ReferenceTranDate, 'dd-MMM-yyyy'),
                            TranTypeIndicator: result.GasDealerTransRows[i].TranTypeIndicator,
                            TranDescription: result.GasDealerTransRows[i].TranDescription,
                            TranAmount: parseFloat(result.GasDealerTransRows[i].TranAmount).toFixed(2),
                        });
                    }
                    $scope.GetTotalAmount($scope.GasDealerTransRows);
                /*} else {
                    sweetAlert("Error!!", "No data for this Draft No", "error");
                    $scope.GasDealerTransRows = [];
                    $scope.receipts.ReferenceNo = null;
                }*/
            }).error(function () {
                sweetAlert("Error!!", "Error in Getting Receipts", "error");
                $scope.GasDealerTransRows = [];
                $scope.receipts.ReferenceNo = null;
            });
        } else {
            sweetAlert("Warning!!", "Please Select Draft No.", "warning");
            $scope.GasDealerTransRows = [];
            $scope.receipts.ReferenceNo = null;
        }
    };

    //Adding Receipts
    $scope.addReceipts = function (event) {
        if (angular.isUndefined(event.ReferenceTranNo) || event.ReferenceTranNo == null) {
            sweetAlert("Error!!", "Please Select Reference No.", "error");
            $scope.loading = false;
        } else if (event.ReferenceTranDate == null) {
            sweetAlert("Error!!", "Please Choose Date", "error");
            $scope.loading = false;
        } else if (angular.isUndefined(event.TranTypeIndicator) || event.TranTypeIndicator == null) {
            sweetAlert("Error!!", "Please Select Transaction Type", "error");
            $scope.loading = false;
        } else if (angular.isUndefined(event.TranAmount) || event.TranAmount == null) {
            sweetAlert("Error!!", "Please Enter Amount", "error");
            $scope.loading = false;
            } else {
               var postData ={};
            postData.StoreID=$rootScope.StoreID;
            postData.TypeID = $scope.receiptsInput.TranTypeIndicator;
            postData.RefNumber=$scope.receiptsInput.ReferenceTranNo;
            postData.DateOfTransaction=$filter('date')($scope.receiptsInput.ReferenceTranDate, 'dd-MMM-yyyy');
         
            gasService.getTransAmount(postData).success(function (response) {
                $scope.loading = true;
                if ($scope.HasKey === null) {
                    $scope.GasDealerTransRows.push({
                        SlNo: parseInt($scope.GasDealerTransRows.length) + 1,
                        ReferenceTranNo: event.ReferenceTranNo,
                        ReferenceTranDate: $filter('date')(event.ReferenceTranDate, 'dd-MMM-yyyy'),
                        TranTypeIndicator: event.TranTypeIndicator,
                        TranDescription: $("#TranTypeIndicator option:selected").text(),
                        TranAmount: parseFloat(event.TranAmount).toFixed(2),
                        VendorAmount: response
                    });
                    $scope.loading = false;
                    $scope.GetTotalAmount($scope.GasDealerTransRows);
                } else {
                    for (i = 0; $scope.GasDealerTransRows.length > i; i++) {
                        if ($scope.HasKey === $scope.GasDealerTransRows[i].$$hashKey) {
                            $scope.GasDealerTransRows[i].SlNo = parseInt($scope.GasDealerTransRows.length) + 1;
                            $scope.GasDealerTransRows[i].ReferenceTranNo = $scope.receiptsInput.ReferenceTranNo;
                            $scope.GasDealerTransRows[i].ReferenceTranDate = $filter('date')($scope.receiptsInput.ReferenceTranDate, 'dd-MMM-yyyy');
                            $scope.GasDealerTransRows[i].TranTypeIndicator = $scope.receiptsInput.TranTypeIndicator;
                            $scope.GasDealerTransRows[i].TranDescription = $("#TranTypeIndicator option:selected").text();
                            $scope.GasDealerTransRows[i].VendorAmount = response;
                            $scope.GasDealerTransRows[i].TranAmount = parseFloat($scope.receiptsInput.TranAmount).toFixed(2);
                            $scope.HasKey = null;
                            break;
                        }
                    }
                    $scope.GetTotalAmount($scope.GasDealerTransRows);
                    $scope.loading = false;
                }
                $scope.clearInputs();
            }).error(function (response) {
                debugger;
                $scope.loading = false;
                sweetAlert("Error!!", response, "error");
            });

         
        }
    }
    //Edit Receipts
    $scope.EditReceipts = function (items) {
        //console.log(items.$$hashKey);
        $scope.HasKey = items.$$hashKey;
        $scope.receiptsInput.ReferenceTranNo = items.ReferenceTranNo;
        $scope.receiptsInput.ReferenceTranDate = new Date(items.ReferenceTranDate);
        $scope.receiptsInput.TranTypeIndicator = items.TranTypeIndicator;
        $scope.receiptsInput.TranAmount = parseFloat(items.TranAmount).toFixed(2);
    };
    //Clear Fields
    $scope.clearInputs = function () {
        $scope.receiptsInput.ReferenceTranNo = null;
        $scope.receiptsInput.ReferenceTranDate = null;
        $scope.receiptsInput.TranTypeIndicator = null;
        $scope.receiptsInput.TranAmount = null;
    };
    //Amount Decimals
    $scope.getDecimals = function () {
        if ($scope.receiptsInput.TranAmount != null && $scope.receiptsInput.TranAmount != "" && $scope.receiptsInput.TranAmount != "undefined") {
            $scope.receiptsInput.TranAmount = parseFloat($scope.receiptsInput.TranAmount).toFixed(2);
        }
    };
    //Calclating Totals
    $scope.GetTotalAmount = function (GasDealerTransRows) {
        var total = 0;
        for (var i = 0; i < GasDealerTransRows.length; i++) {
            total += parseFloat(GasDealerTransRows[i].TranAmount);
        }
        $scope.receipts.total = parseFloat(total).toFixed(2);
    }

    //Saving Receipts
    $scope.saveReceipts = function () {
        $scope.loading = true;
        $scope.receipts.ReferenceNo = $scope.receipts.ReferenceNo;
        $scope.receipts.GasDealerTransRows = $scope.GasDealerTransRows;      
        var ReceiptData = angular.toJson($scope.receipts);
        ReceiptData = JSON.parse(ReceiptData);

        gasService.saveReceiptsData(ReceiptData).success(function (response) {
            $scope.loading = false;
            sweetAlert("Success", "Receipts Saved Successfully", "success");
        }).error(function (response) {
            $scope.loading = false;
            sweetAlert("Error!!", response, "error");
        });    
    };



   


}]);