angular.module('GasApp').controller("AddCreditCardController", ['$scope', '$state', '$rootScope', '$filter', 'adminService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $state, $rootScope, $filter, adminService, NgTableParams, Excel, $timeout, httpPreConfig) {

    var searchData = {};
    searchData.SearchKey = '0';
    searchData.Searchvalue =  $rootScope.GroupID;
    $scope.addedCards = [];
    $scope.creditTo = [];

    $scope.allCards = [];


    var creditToObj = new Object();
    creditToObj.text = "Gas Dealer Account";
    creditToObj.value = "G";

    $scope.creditTo.push(creditToObj);

    creditToObj = new Object();
    creditToObj.text = "Vendor Account";
    creditToObj.value = "V";

    $scope.creditTo.push(creditToObj);

    $scope.init = function () {

        adminService.getStores(searchData).success(function (response) {

            $scope.stores = response;
            debugger;
            var currentStore = {};
            currentStore.StoreID = $rootScope.StoreID;
            currentStore.StoreName = $rootScope.StoreName;
            $scope.cboStore = currentStore;

            adminService.GetGroupCardTypes($rootScope.GroupID).success(function (response) {
                $scope.allCards = response;

                $scope.tableParams = new NgTableParams({
                    sorting: { StoreName: "asc", State: "asc" }
                }, {
                    dataset: $scope.allCards
                });

            }).error(function (response) {
                sweetAlert("Error!!", response, "error");
            });


        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    }

    $scope.init();

    
    $scope.StoreIDChange = function ()
    {
        adminService.GetGroupCardTypes($rootScope.GroupID).success(function (response) {
            $scope.allCards = response;

            $scope.tableParams = new NgTableParams({
                sorting: { StoreName: "asc", State: "asc" }
            }, {
                dataset: $scope.allCards
            });

        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
        });
    };

    
    adminService.getRemainingStoreCardTypes($rootScope.StoreID).success(function (response) {
        $scope.cards = response;
    }).error(function (response) {
        sweetAlert("Error!!", response, "error");
    });

    

    $scope.AddCard = function () {
        if ($scope.addCreditCardToStore.$invalid) {
            $scope.submitted = true;
            return;
        }

        var addedCardsObjArr = $scope.addedCards.filter($scope.filterCardType);
        var cardObj = new Object();
        var filterArray = false;
        if (addedCardsObjArr.length > 0) {
            filterArray = true;
            cardObj = addedCardsObjArr[0];
        }

        console.log($scope.user.AcceptCard.CardType);


        var index = $scope.addedCards.length;

        cardObj.CardName = $scope.user.AcceptCard.CardTypeName;

        cardObj.CardType = $scope.user.AcceptCard.CardTypeID;

        cardObj.StoreID = $scope.cboStore.StoreID;

        cardObj.CreditTo = $scope.user.CreditTo.text;
        cardObj.CreditToID = $scope.user.CreditTo.value;
        debugger;
        cardObj.CardCreditType= $scope.user.CreditTo.value;

        cardObj.Status='A';

        cardObj.CrudType = 'N';
        cardObj.index = index + 1;
        debugger;
        
        if (filterArray == false) {
            $scope.addedCards.push(cardObj);
        }
        else {
            $scope.addedCards = $scope.addedCards.filter($scope.filterCashTypeExcept);
            $scope.addedCards.push(cardObj);
        }
        $scope.clear();

    }

    $scope.clear = function () {
        //$scope.model = {
        //    paidFor: '',
        //    amount: ''
        //};
        //$scope.submitted = false;
        $scope.addCreditCardToStore.$invalid = false;

    }

    $scope.filterCardType = function (addedCardsEntry, cardtype) {
        debugger;
        return addedCardsEntry.CardType == $scope.user.AcceptCard.CardTypeID;
    }

    $scope.filterCardTypeExcept = function (addedCardsEntry, cardtype) {
        return addedCardsEntry.CardType != $scope.user.AcceptCard.CardTypeID;
    }

    $scope.deleteRow = function (item) {
        for (var i = $scope.addedCards.length - 1; i >= 0; i--) {
            if ($scope.addedCards[i].index === item.index) {
                $scope.addedCards.splice(i, 1);
            }
        }
    };

    $scope.savecards = function () {
        var postData = $scope.addedCards;

        
        adminService.saveCards(postData).success(function (response) {
            $scope.StoreIDChange();

            $scope.user.CreditTo.value = "";
            $scope.user.AcceptCard.CardTypeID = "";

            sweetAlert("Success", "Card added Successfully", "success");
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });


    }

    
}]);