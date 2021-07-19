angular.module('GasApp').controller('lotterycashtransferController', ['$scope', '$http', '$filter', '$rootScope', 'lotteryService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $http, $filter, $rootScope, lotteryService, NgTableParams, Excel, $timeout, httpPreConfig) {

    var Day = "";
    var Month = "";
    var Year = "";  
    $scope.shifts = [];
    $scope.PaidForList = [];
    $scope.LotteryTransferList = [];

    $scope.model={paidFor : "",
        amount : ''};

    //Getting Shifts Details for the store
    lotteryService.getShifts($rootScope.StoreID).success(function (result) {
        $scope.shifts = result;
        $scope.loading = false;
    }).error(function () {
        sweetAlert("Error!!", "shifts", "error");
        $scope.loading = false;
    });

    lotteryService.getRunningShift($rootScope.StoreID).success(function (result) {
        $scope.Day = $filter('date')(result.CurrentDate, 'dd');
        $scope.Month = $filter('date')(result.CurrentDate, 'MM');
        $scope.Year = $filter('date')(result.CurrentDate, 'yyyy');
        $scope.todaysDate = new Date($scope.Year, $scope.Month - 1, $scope.Day);       
        $scope.currentShift = result.ShiftCode;
        var postData = {
            storeID: $rootScope.StoreID,
            Date: $scope.todaysDate,
            ShiftCode: $scope.currentShift,
            RequestType: "LOTTERY-TRANSFER"
        };
        lotteryService.GetDaySale(postData).success(function (response) {
            if (response != "No Data Found") {

                $scope.LotteryTransferList = response.LotteryTransferList;

                $scope.LotteryTransferList.forEach(function (item, index) {

                    if (item.TranType == "GC")
                    {
                        item.TranTypeText = "Gas Cash";
                    }
                    else if (item.TranType == "BC")
                    {
                        item.TranTypeText = "Business Cash";
                    }
                    else if (item.TranType == "LD")
                    {
                        item.TranTypeText = "Bank Deposit";
                    }
                    item.CrudType = 'S';
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", "Error in getting day sale", "error");
        });

    }).error(function () {
        sweetAlert("Error!!", "Error in getting running shift", "error");
        $scope.loading = false;
    });




    var paidForObj = new Object();
    paidForObj.text = "Bank Deposite";
    paidForObj.value = "LD";

    $scope.PaidForList.push(paidForObj);

    paidForObj = new Object();
    paidForObj.text = "Transfer to gas cash";
    paidForObj.value = "GC";

    $scope.PaidForList.push(paidForObj);


    paidForObj = new Object();
    paidForObj.text = "Transfer to bussiness cash";
    paidForObj.value = "BC";

    $scope.PaidForList.push(paidForObj);

    $scope.addtoCashTransfer = function ()
    {
        
        if ($scope.lottercashtransferfrm.$invalid) {
            $scope.submitted = true;
            return;
        }

        var cashTransferObjArr = $scope.LotteryTransferList.filter($scope.filterCashType);
        var cashTransferObj = new Object();
        var filterArray = false;
        if (cashTransferObjArr.length > 0)
        {
            filterArray = true;
            cashTransferObj = cashTransferObjArr[0];
        }
        
        console.log($scope.model.paidFor);
        

        var  index= $scope.LotteryTransferList.length;
        cashTransferObj.TranType = $scope.model.paidFor.value;
        cashTransferObj.TranTypeText = $scope.model.paidFor.text;
        if (cashTransferObj.TranAmount == "" || cashTransferObj.TranAmount==undefined)
            cashTransferObj.TranAmount = 0;
        cashTransferObj.TranAmount = (Number( cashTransferObj.TranAmount) +Number(  $scope.model.amount)).toString();
        cashTransferObj.CrudType = 'N';
        cashTransferObj.index = index + 1;
        if (filterArray == false) {
            $scope.LotteryTransferList.push(cashTransferObj);
        }
        else {
            $scope.LotteryTransferList = $scope.LotteryTransferList.filter($scope.filterCashTypeExcept);
            $scope.LotteryTransferList.push(cashTransferObj);
        }
        $scope.clear();
        
    }
    $scope.submitted = false;
    $scope.clear = function ()
    {
        $scope.model = {
            paidFor: '',
            amount: ''
        };
        $scope.submitted = false;
        $scope.lottercashtransferfrm.$invalid = false;

    }

    $scope.deleteRow = function (item)
    {
        for (var i = $scope.LotteryTransferList.length - 1; i >= 0; i--) {
            if ($scope.LotteryTransferList[i].index === item.index) {
                $scope.LotteryTransferList.splice(i, 1);
            }
        }
    };
    console.log($scope.PaidForList);

    $scope.savelotterycashtrans = function ()
    {
        var postData=
        {
            StoreID: $rootScope.StoreID,
            Date: $scope.todaysDate,
            ShiftCode: $scope.currentShift,
            CreatedUserName: $rootScope.UserName,
            ModifiedUserName: $rootScope.UserName,
            //LotteryTransferList: $scope.LotteryTransferList.filter($scope.filterNewRecord)
            LotteryTransferList: $scope.LotteryTransferList
        }
        lotteryService.savelotterycashtrans(postData).success(function (response) {
            sweetAlert("Success", "Lottery Cash Transfer saved Successfully", "success");
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
        
       
    }

    $scope.filterCashType = function (cashtransferentry, cashtype) {
        return cashtransferentry.TranType == $scope.model.paidFor.value;
    }

    $scope.filterCashTypeExcept = function (cashtransferentry, cashtype) {
        return cashtransferentry.TranType != $scope.model.paidFor.value;
    }
    
}]);