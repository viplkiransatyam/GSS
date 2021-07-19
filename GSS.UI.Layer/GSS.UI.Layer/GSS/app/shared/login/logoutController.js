angular.module('GasApp').controller('LogoutController', ['$scope', '$state', '$cookies', '$http', 'loginService', function ($scope, $state, $cookies, $http, loginService) {
        $cookies.remove('auth');
        $cookies.remove('AccessType');
        $cookies.remove('GroupID');
        $cookies.remove('StoreID');
        $cookies.remove('UserName');
        $cookies.remove('Status');
        $cookies.remove('StoreType');
        $cookies.remove('StoreAdd1');
        $cookies.remove('StoreAdd2');
        $cookies.remove('AvailBusiness');
        $cookies.remove('AvailGas');
        $cookies.remove('AvailLottery');
        location.reload();
}]);