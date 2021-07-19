angular.module('GasApp').controller('storeBusinessController', ['$scope', '$http', '$filter', '$rootScope', 'storeService', function ($scope, $http, $filter, $rootScope, storeService) {


    $scope.showPanelsOne = function(){
        $("#accOneColOne").css("display","block");
        $("#accOneColTwo").css("display","none");
        $("#accOneColThree").css("display","none");
        $("#accOneColFour").css("display","none");
    };
    $scope.showPanelsTwo = function(){
        $("#accOneColOne").css("display","none");
        $("#accOneColTwo").css("display","block");
        $("#accOneColThree").css("display","none");
        $("#accOneColFour").css("display","none");
    };
    $scope.showPanelsThree = function(){
        $("#accOneColOne").css("display","none");
        $("#accOneColTwo").css("display","none");
        $("#accOneColThree").css("display","block");
        $("#accOneColFour").css("display","none");
    };
    $scope.showPanelsFour = function(){
        $("#accOneColOne").css("display","none");
        $("#accOneColTwo").css("display","none");
        $("#accOneColThree").css("display","none");
        $("#accOneColFour").css("display","block");
    };
}]);