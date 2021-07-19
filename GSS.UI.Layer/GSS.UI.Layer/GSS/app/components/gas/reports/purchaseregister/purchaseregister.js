angular.module('GasApp').controller("PurchaseRegisterController", ['$scope', '$rootScope', '$filter', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, gasService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.loading = true;
    var storeID = $rootScope.StoreID;
    $scope.shifts = [];
    $scope.GasInventory = [];
    $scope.GasTax = [];
    $scope.GasDefaultTax = [];
    $scope.date = new Date();

    //Getting Shifts Details for the store
    gasService.getShifts(storeID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;

    });
    $scope.getPurchaseRegisterData = [];
    $scope.fromDate = new Date();
    $scope.toDate = new Date();
    $scope.fromDate.setDate($scope.fromDate.getDate() - 1);
    $scope.toDate.setDate($scope.toDate.getDate() - 1);

    $scope.getPurchaseRegisterReport = function (repSalefromDate, repSaleToDate) {
        var postData = {};
        repSalefromDate = $filter('date')(new Date(repSalefromDate), 'dd-MMM-yyyy');
        repSaleToDate = $filter('date')(new Date(repSaleToDate), 'dd-MMM-yyyy');
        postData.StoreID = storeID;
        postData.FromDate = repSalefromDate;
        postData.ToDate = repSaleToDate;
        gasService.getPurchaseRegister(postData).success(function (response) {
            if (response.length <= 0) {
                $scope.getPurchaseRegisterData = [];
            } else {
                $scope.getPurchaseRegisterData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { InvDate: "asc", InvNo: "asc", DueDate: "asc", Amount: "asc" }
                }, {
                    dataset: $scope.getPurchaseRegisterData
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }


    $scope.getListDetails = function(tranId){        
        var postData = {};
        postData.StoreID = storeID;
        postData.BOL = tranId;
        gasService.getGasPurchases(postData).success(function (result) {
            if (result.BillOfLading != null) {                        
                $scope.dueDate = $filter('date')(result.DueDate, 'dd-MMM-yyyy');
                $scope.receiptDate = $filter('date')(result.ReceiptDate, 'dd-MMM-yyyy');
                $scope.TransactionID = result.TransactionID;
               
                if (result.InvDate == "0001-01-01T00:00:00") {
                    $scope.InvDate = $filter('date')(result.InvDate, 'dd-MMM-yyyy');
                } else {
                    $scope.Day = $filter('date')(result.InvDate, 'dd');
                    $scope.Month = $filter('date')(result.InvDate, 'MM');
                    $scope.Year = $filter('date')(result.InvDate, 'yyyy');
                    $scope.InvDate = new Date($scope.Year, $scope.Month-1, $scope.Day);
                }
                
                $scope.InvAmount = parseFloat(result.InvAmount).toFixed(2);
                $scope.InvNo = result.InvNo;
                $scope.GasInvoiceReceiptType = result.GasInvoiceReceiptType;
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
                $("#myModal").modal();
            } else {
                sweetAlert("Error!!", "No data for this Bill of Ladding ", "error");
                $scope.dueDate = null;
                $scope.receiptDate = null;
                $scope.BillOfLading = null;
                $scope.TransactionID = null;
                $scope.InvDate = null;
                $scope.InvAmount = null;
                $scope.InvNo = null;
                $scope.GasInvoiceReceiptType = null;
                $scope.GasInventory = [];
                $scope.GasDefaultTax = [];
            }

        }).error(function () {
            sweetAlert("Error!!", "Error in Getting Gas Purchases", "error");
            $scope.dueDate = null;
            $scope.receiptDate = null;
            $scope.BillOfLading = null;
            $scope.TransactionID = null;
            $scope.InvDate = null;
            $scope.InvAmount = null;
            $scope.InvNo = null;
            $scope.GasInvoiceReceiptType = null;
            $scope.GasInventory = [];
            $scope.GasDefaultTax = [];
            $scope.loading = false;
        });

    }
    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'GasSaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'GasSaleReport' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);