angular.module('GasApp').controller("newgameController", ['$scope', '$rootScope', '$filter', 'setupService', 'NgTableParams', 'Excel', '$timeout', 'httpPreConfig', function ($scope, $rootScope, $filter, setupService, NgTableParams, Excel, $timeout, httpPreConfig) {
    $scope.game = {};
    $scope.game.BookValue = "";
    $scope.game.TicketEndNumber = "";
    $scope.game.TicketStartNumber = "";
    $scope.game.TicketValue = "";
    $scope.game.NoOfTickets = "";
    $scope.game.GameName = "";
    $scope.game.GameID = "";
    $scope.game.StoreID = $rootScope.StoreID;
    $scope.gamesData = [];

    $scope.clear = function () {
        $scope.game.BookValue = "";
        $scope.game.TicketEndNumber = "";
        $scope.game.TicketStartNumber = "";
        $scope.game.TicketValue = "";
        $scope.game.NoOfTickets = "";
        $scope.game.GameName = "";
        $scope.game.GameID = "";
        $scope.game.StoreID = "";
        $scope.addGameForm.$submitted = false;
    };

    $scope.getList = function () {
        setupService.GameList($rootScope.StoreID).success(function (response) {

            if (response.length <= 0) {
                $scope.gamesData = [];
            } else {
                $scope.gamesData = response;
                $scope.tableParams = new NgTableParams({
                    sorting: { GameID: "asc", GameName: "asc" }
                }, {
                    dataset: $scope.gamesData
                });
            }
        }).error(function (response) {
            sweetAlert("Error!!", response, "error");
            $scope.loading = false;
        });

    };

    $scope.getList();

    $scope.gamereset = function () {
        $scope.game.BookValue = "";
        $scope.game.TicketEndNumber = "";
        $scope.game.TicketStartNumber = "";
        $scope.game.TicketValue = "";
        $scope.game.NoOfTickets = "";
        $scope.game.GameName = "";
        $scope.game.GameID = "";
        $scope.addGameForm.$submitted = false;
    };

    $scope.addNewGame = function (gameObj)
    {
        
        if ($scope.addGameForm.$invalid) {
            return;
        }
        
        var postData = {};
        postData = gameObj;
        setupService.addGame(postData).success(function (response) {
            var pushdata = {};
            pushdata.GameName = $scope.game.GameName;
            pushdata.GameID = $scope.game.GameID;

            $scope.gamesData.push(pushdata);
            $scope.tableParams.reload();



            $scope.clear();
            sweetAlert("Success", "New game added successfully", "success");
        }).error(function (errorRes) {
            sweetAlert("Error",errorRes, "danger");
            $scope.loading = false;
        });
    };

}]);