angular.module('GasApp').factory('loginService', function ($http,$state,$cookies) {
    var loginService = [];    
    loginService.login = function (login) {        
        return $http({
            method: 'POST',
            url: '/Api/StoreMaster/ValidateUser',
            headers: {
                'Content-Type': 'application/json'
            },
            data: login
        });
    };

    loginService.getAuthStatus = function () {
        var status = $cookies.get('auth');       
        if (status) {
            return true;
        } else {
            return false;
        }
    };



    loginService.IsAdmin = function () {       
        return $cookies.get('AccessType') == "ADMIN";
    }

    loginService.IsStoreManager = function () {
        return $cookies.get('AccessType') == "STORE";
    }

    loginService.IsSuperUser = function () {
        return $cookies.get('AccessType') == "SUPERUSER";
    }

    return loginService;
    
});