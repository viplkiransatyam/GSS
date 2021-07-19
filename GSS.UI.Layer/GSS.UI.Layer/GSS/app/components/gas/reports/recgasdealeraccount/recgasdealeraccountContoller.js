angular.module('GasApp').controller("recgasdealeraccountContoller", ['$scope', '$rootScope', '$filter', 'gasService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, gasService, NgTableParams, Excel, $timeout, httpPreConfig) {

    $scope.reconcilationData = [];
    $scope.transactionType = 1;

    $scope.GetReconcillationDetails = function () {
        var postData = {};
        postData.StoreID = $rootScope.StoreID;
        postData.ReferenceNo = $scope.transactionType;
        
        
        gasService.getReconcillationDetails(postData).success(function (response) {
            console.log(response);
            if (response.length <= 0) {
                $scope.reconcilationData = [];
            } else {
                $scope.reconcilationData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { DealerRefNo: "asc", DealerDate: "asc", DealerAmount: "asc", StoreRefNo: "asc", StoreDate: "asc", StoreAmount: "asc"}
                }, {
                    dataset: $scope.reconcilationData
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };

    $scope.addRefNo = [];
    $scope.checkedItems = [];
    $scope.statechanged = function (refNo) {
        
        if ($scope.addRefNo[refNo]) {
            $scope.checkedItems.push(refNo);
        }
        else {
            if ($scope.checkedItems.indexOf(refNo)>=0)
            {
                $scope.checkedItems.splice($scope.checkedItems.indexOf(refNo), 1);
            }
        }
    };

    $scope.Save = function () {
        if ($scope.checkedItems.length <= 0)
        {
            sweetAlert("Information", "Please choose reference numbers for reconcillation!!", "error");
            return false;
        }

        var postdata = {};
        postdata.StoreID = $rootScope.StoreID;
        postdata.TransactionType = $scope.transactionType;
        postdata.ModifiedUserName = $rootScope.UserName;
        postdata.Reference = [];
        for (var i = 0; i < $scope.checkedItems.length; i++)
        {
            var ref = {};
            ref.ReferenceNo = $scope.checkedItems[i];
            postdata.Reference.push(ref);
        }
        gasService.UpdateReconcillation(postdata).success(function (response) {
            sweetAlert("Success", "Refernce numbers reconciled", "success");
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });
    };


    $scope.exportToExcel = function (tableId) { // ex: '#my-table'
        var exportHref = Excel.tableToExcel(tableId, 'gasrecReports');
        var a = document.createElement('a');
        a.href = exportHref;
        a.download = 'gasrecReports' + $filter('date')(new Date($scope.date), 'dd-MMM-yyyy') + '.xls';
        a.click();
    }

}]);