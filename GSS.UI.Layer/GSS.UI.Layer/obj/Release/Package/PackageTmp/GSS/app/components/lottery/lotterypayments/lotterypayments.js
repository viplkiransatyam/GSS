angular.module('GasApp').controller("lotteryPaymentsController", ['$scope', '$rootScope', '$filter', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', function ($scope, $rootScope, $filter, lotteryService, NgTableParams, Excel, $timeout) {
    var storeID = $rootScope.StoreID;
    $scope.loading = true;
    $scope.lotteryPaymentsData = []; //array for sale report data
    $scope.date = new Date();
    $scope.Payments = {
    	"StoreID":$rootScope.StoreID,
    	"PaymentSweapDate":null,
    	"BusinessEndingDate":null,
    	"TotalPayment":null,
    	"AutoSettlePaymentBooks":[
    		{
    			"GameID":null,
    			"PackNo":null
    		}
    	]
    }

    $scope.getLotteryPayments = function () { 
        lotteryService.getLotteryPayments(storeID).success(function (response) {
            console.log(response);
            var bookamounttotal = 0; 
            var settleamounttotal = 0; 
            $scope.lotteryPaymentsData = response;
            $scope.tableParams = new NgTableParams({
                sorting: { GameID: "asc", PackNo: "asc", AutoSettleDate: "asc", BookAmount: "asc", SettleAmount: "asc"}
            }, {                        
                dataset: $scope.lotteryPaymentsData
            });  

            for(var i=0;i<$scope.lotteryPaymentsData.length-1;i++){
                var paymentdata = $scope.lotteryPaymentsData[i];
                bookamounttotal += (paymentdata.BookAmount);
                settleamounttotal += (paymentdata.SettleAmount);
            }
           
            $scope.bookamounttotal = parseFloat(bookamounttotal).toFixed(2);
            $scope.settleamounttotal = parseFloat(settleamounttotal).toFixed(2);

        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    }



    $scope.updateLotteryPayment = function(){
        $scope.Payments.AutoSettlePaymentBooks = [];
        angular.forEach($scope.lotteryPaymentsData, function(row){
        	if (!!row.selected) $scope.Payments.AutoSettlePaymentBooks.push({'GameID':row.GameID,'PackNo':row.PackNo});
        })

		if($scope.Payments.AutoSettlePaymentBooks.length<=0){
             sweetAlert("Error!!", 'Please Check atleast one payment', "error");
        }else if($scope.Payments.BusinessEndingDate == null){
            sweetAlert("Error!!", 'Please Select Business Ending Date', "error");
        }else if($scope.Payments.PaymentSweapDate == null){
             sweetAlert("Error!!", 'Please Select Sweep Date', "error");
        }else if($scope.Payments.TotalPayment == null){
             sweetAlert("Error!!", 'Please Enter Total Payment', "error");
        }else{
            var UpdatePaymentData = angular.toJson($scope.Payments);
            UpdatePaymentData = JSON.parse(UpdatePaymentData);
            //console.log(UpdatePaymentData);
            lotteryService.updateLotteryPayment(UpdatePaymentData).success(function (response) {
                sweetAlert("Success", "Lottery Payment Updated Successfully", "success");
            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });
        }
    }

    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'LotterySaleReport');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'LotteryPayments' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

    $scope.removeTotalFilter = function(row){
        return row.GameID != null;
    }

    $scope.getLotteryPayments();

   
}]);