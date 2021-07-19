'use strict';

var app = angular.module('gasApp', ['ngRoute', 'ngMaterial', 'ngMessages', 'ngCookies', 'isteven-multi-select'], function ($httpProvider) {
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/json;charset=utf-8';
});
app.config(['$routeProvider', '$locationProvider',
function ($routeProvider, $locationProvider) {
        $routeProvider
        .when('/stores', {
            templateUrl: 'views/StoreView.html',
            controller: 'StoreController',            
            authenticated: true,
            resolve: {
            "check": function ($location, $rootScope) {
                        if ($rootScope.AccessType == "STORE") {
                            sweetAlert("Info", "Access denied", "info");
                            $location.path('/dashboard');
                        }
                    }
            }
        })
        .when('/applygas', {
            templateUrl: 'views/applygasoiltypeview.html',
            controller: 'GasOilTypeController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "STORE") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/assigncard', {
            templateUrl: 'views/assignCardtype.html',
            controller: 'AssignCardTypeController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "STORE") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/accountcreation', {
            templateUrl: 'views/accountcreation.html',
            controller: 'AccountController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "STORE") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/accountcreationstore', {
            templateUrl: 'views/accountcreationstore.html',
            controller: 'AccountStoreController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/', {
            templateUrl: 'loginsample.html',
            controller: 'LoginController',
            authenticated: false,
        })       
        .when('/dashboard', {
            templateUrl: 'views/dashboard.html',
            controller: 'defaultController',
            authenticated: true
        })  
		.when('/changepassword', {
            templateUrl: 'views/changepassword.html',
            controller: 'ChangePasswordController',
            authenticated: true,            
        })		
        .when('/dailystorebasis', {
            templateUrl: 'views/dailystorebasis.html',
            controller: 'DailyStoreController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {                   
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
					if($rootScope.StoreType == "LS"){						
                        $location.path('/lottery');
					}
                }
            }
        })
        .when('/lottery', {
            templateUrl: 'views/lottery.html',
            controller: 'LotteryController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/storepaymetns', {
            templateUrl: 'views/instorepayments.html',
            controller: 'InStorePaymentsController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/storesales', {
            templateUrl: 'views/instoresale.html',
            controller: 'InStoreSalesController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/receipts', {
            templateUrl: 'views/receipts.html',
            controller: 'ReceiptsController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/bankdeposits', {
            templateUrl: 'views/bankdeposit.html',
            controller: 'BankDepositsController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/openingbalanceupdate', {
            templateUrl: 'views/openingbalanceupdate.html',
            controller: 'OpeningBalanceController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
					if($rootScope.StoreType == "LS"){
						sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
					}
                }
            }
        })
        .when('/ReportLedger', {
            templateUrl: 'views/ReportLedger.html',
            controller: 'ReportLedgerController',
            authenticated: true
        })
        .when('/BalanceSheet', {
            templateUrl: 'views/BalanceSheetGas.html',
            controller: 'BalanceSheetGasController',
            authenticated: true
        })

        .when('/unlockDay', {
            templateUrl: 'views/unlockDay.html',
            controller: 'UnlockDayController',
            authenticated: true
        })
        .when('/gaspriceupdate', {
            templateUrl: 'views/GasPriceUpdate.html',
            controller: 'updategaspriceController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })

		.when('/monthlyreport', {
		    templateUrl: 'views/monthlyreportview.html',
		    controller: 'MonthlyReportController',
		    authenticated: true,
		})
        .when('/gasmonthlyreport', {
            templateUrl: 'views/gasmonthlyreportview.html',
            controller: 'GasMonthlyReportController',
            authenticated: true,
        })
        .when('/groupcustomreport', {
            templateUrl: 'views/groupcustomreport.html',
            controller: 'GroupCustomReportController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/creategroups', {
            templateUrl: 'views/creategroups.html',
            controller: 'CreateGroupsController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/mapledgers', {
            templateUrl: 'views/mapledgers.html',
            controller: 'MapLedgersController',
            authenticated: true,
            resolve: {
                "check": function ($location, $rootScope) {
                    if ($rootScope.AccessType == "ADMIN") {
                        sweetAlert("Info", "Access denied", "info");
                        $location.path('/dashboard');
                    }
                }
            }
        })
        .when('/purchases', {
            templateUrl: 'views/purchases.html',
            controller: 'PurchasesController',
            authenticated: true
        })
        .when('/chequecashing', {
            templateUrl: 'views/chequeCashing.html',
            controller: 'ChequeCashingController',
            authenticated: true
        })
        .when('/chequedeposit', {
            templateUrl: 'views/chequedeposits.html',
            controller: 'ChequeDepositController',
            authenticated: true
        })

        .otherwise({ redirectTo: '/' });
    }
]);

app.run(['$rootScope', '$location', 'loginService','$cookies',
    function ($rootScope, $location, loginService, $cookies) {     
        //settign gloabal values to rootscope        
        $rootScope.AccessType = $cookies.get('AccessType');
        $rootScope.GroupID = $cookies.get('GroupID');
        $rootScope.StoreID = $cookies.get('StoreID');
        $rootScope.UserName = $cookies.get('UserName');
		$rootScope.StoreType = $cookies.get('StoreType');

        $rootScope.$on("$routeChangeStart",
            function (event, next, current) {
                if (next.$$route.authenticated) {
                    if (!loginService.getAuthStatus()) {
                        $location.path('/');
                    }
                }
                if (next.$$route.originalPath == "/") {
                    if (loginService.getAuthStatus()) {
                        $location.path(current.$$route.originalPath);
                    }
                }
            });
    }]);
