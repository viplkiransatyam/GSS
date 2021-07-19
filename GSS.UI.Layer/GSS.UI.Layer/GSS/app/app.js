var app = angular.module('GasApp', ['ui.router', 'oc.lazyLoad', 'ngCookies', 'mwl.calendar','ui.bootstrap', 'angular-loading-bar', 'ngTable','chart.js','isteven-multi-select'], function ($httpProvider) {
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/json;charset=utf-8';
});

app.config(['$httpProvider', '$stateProvider', '$urlRouterProvider','cfpLoadingBarProvider','ChartJsProvider',
  	function($httpProvider, $stateProvider, $urlRouterProvider, cfpLoadingBarProvider, ChartJsProvider) {
  		//cfpLoadingBarProvider.includeSpinner = false;
  		cfpLoadingBarProvider.spinnerTemplate = '<div id="mydiv" class="ajax-loader text-center"><img src="../img/loader-animation.gif" /></div> ';    	
  	    ChartJsProvider.setOptions({ colors: ['#803690', '#00ADF9', '#DCDCDC', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'] });
    	$stateProvider
	      .state('template', {
	      	name:'template',
	        templateUrl: 'shared/template.html',
	        controller: 'TemplateController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['../assets/css/theme-default.css','../assets/css/gss-custom.css','shared/templateController.js']
	                }]);
	            }]
	        },	         	              
	        abstract: true,
	        authenticated:true
	      })	  
	      .state('dashboard', {
	      	name:'template',
	        url: '/dashboard',
	        parent: 'template',
	        templateUrl: 'shared/dashboard/dashboard.html',
	        controller: 'DashboardController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['shared/dashboard/dashboardService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'shared/dashboard/dashboardController.js']
	                }]);
	            }]
	        },
	        authenticated:true
	      })
	      .state('gas', {
	      	name:'gas',
	        url: '/gas',
	        parent: 'template',
	        templateUrl: 'components/gas/gas/gas.html',
	        controller: 'GasController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['../assets/css/select2.min.css','../assets/lib/select2.min.js',
	                    		'../assets/css/sweetalert.css','../assets/lib/sweetalert.min.js',
	                    		'components/gas/gasService.js','components/gas/gas/gasController.js']
	                }]);
	            }],
	             "check": function ($state, $rootScope) {
		            	if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }            
	        },       
	        authenticated:true
	      })
	      .state('purchases', {
	      	name:'purchases',
	        url: '/purchases',
	        parent: 'template',
	        templateUrl: 'components/gas/purchases/purchases.html',
	        controller: 'PurchasesController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js','components/gas/purchases/purchaseController.js',
                                '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            	if($rootScope.AvailGas == "N"){		            		             		
                         $state.go('dashboard')
	            	}
                }
	        },
	        authenticated:true
	      })
	      .state('receipts', {
	      	name:'receipts',
	        url: '/receipts',
	        parent: 'template',
	        templateUrl: 'components/gas/receipts/receipts.html',
	        controller: 'ReceiptsController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/receipts/receiptsController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('cardreceipts', {
	      	name:'cardreceipts',
	        url: '/cardreceipts',
	        parent: 'template',
	        templateUrl: 'components/common/cardreceipts/cardreceipts.html',
	        controller: 'CardReceiptsController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/common/cardreceipts/cardreceiptsController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('paymentvoucher', {
              url: '/paymentvoucher',
              parent: 'template',
              templateUrl: 'components/common/paymentvoucher/paymentvoucher.html',
              controller: 'PaymentVoucherController',
              resolve: {
                  lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                      return $ocLazyLoad.load([{
                          name: 'GasApp',
                          files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/common/paymentvoucher/paymentvoucher.js']
                      }])
                  }],
                  "check": function ($state, $rootScope) {
		            	if($rootScope.AvailBusiness == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
              },
              authenticated: true
          })
          .state('receiptvoucher', {
              url: '/receiptvoucher',
              parent: 'template',
              templateUrl: 'components/common/paymentvoucher/receiptvoucher.html',
              controller: 'PaymentVoucherController',
              resolve: {
                  lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                      return $ocLazyLoad.load([{
                          name: 'GasApp',
                          files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/common/paymentvoucher/paymentvoucher.js']
                      }])
                  }],
                  "check": function ($state, $rootScope) {
		            	if($rootScope.AvailBusiness == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
              },
              authenticated: true
          })
	      .state('stockinward', {
	      	name:'stockinward',
	        url: '/stockinward',
	        parent: 'template',
	        templateUrl: 'components/setup/stockinward/stockinward.html',
	        controller: 'StockInwardController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/stockinward/stockinwardController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('dailyclosing', {
	        url: '/dailyclosing',
	        parent: 'template',
	        templateUrl: 'components/lottery/dailyclosing/dailyclosing.html',
	        controller: 'DailyClosingController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/dailyclosing/dailyclosingController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('activebooks', {
	        url: '/activebooks',
	        parent: 'template',
	        templateUrl: 'components/lottery/activebooks/activebooks.html',
	        controller: 'ActivebooksController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/activebooks/activebooksController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('receivebooks', {
	        url: '/receivebooks',
	        parent: 'template',
	        templateUrl: 'components/lottery/receivebooks/receivebooks.html',
	        controller: 'ReceivebooksController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/receivebooks/receivebooksController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('lotterysalereport', {
	        url: '/lottery-sale-report',
	        parent: 'template',
	        templateUrl: 'components/lottery/reports/salereports/lotterysalereport.html',
	        controller: 'lotterySaleReportController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js',  '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js','components/lottery/reports/salereports/lotterySaleReportController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailLottery == "N"){		            		             		
	                          $state.go('dashboard');
		            	}	            			
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                          $state.go('dashboard');
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('salereportmonth', {
                url: '/salereportmonth',
                parent: 'template',
                templateUrl: 'components/lottery/reports/salereports/salereportmonth.html',
                controller: 'SaleReportMonthController',
                resolve: {
                    lazy: ['$ocLazyLoad', function($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/lottery/lotteryService.js',  '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js','components/lottery/reports/salereports/salereportmonth.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                    	if($rootScope.AvailLottery == "N"){		            		             		
	                          $state.go('dashboard');
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                          $state.go('dashboard');
		            	}
	                }
                },
                authenticated:true
           })
	      .state('detailedsalereport', {
	        url: '/detailedsalereport',
	        parent: 'template',
	        templateUrl: 'components/lottery/reports/detailedsalereports/detailedsalereport.html',
	        controller: 'DetailedSaleReportController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/detailedsalereports/detailedsalereportController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('activebooksreport', {
	        url: '/activebooksreport',
	        parent: 'template',
	        templateUrl: 'components/lottery/reports/activebookreports/activebooksreport.html',
	        controller: 'ActiveBooksReportController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js','components/lottery/reports/activebookreports/activebooksreportController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('receivebooksreport', {
	        url: '/receivebooksreport',
	        parent: 'template',
	        templateUrl: 'components/lottery/reports/receivebookreports/receivebooksreport.html',
	        controller: 'ReceiveBooksReportController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/receivebookreports/receivebooksreportController.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
	      .state('lotterypayments', {
	        url: '/lotterypayments',
	        parent: 'template',
	        templateUrl: 'components/lottery/lotterypayments/lotterypayments.html',
	        controller: 'lotteryPaymentsController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/lotterypayments/lotterypayments.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            	if($rootScope.AvailLottery == "N"){		            		             		
                         $state.go('dashboard')
	            	}
	            	if($rootScope.AccessType == "STORE_USER"){		            		             		
                         $state.go('dashboard')
	            	}
                }
	        },
	        authenticated:true
	      })          
	      .state('lotteryinventoryreport', {
             url: '/lotteryinventoryreport',
             parent: 'template',
             templateUrl: 'components/lottery/reports/inventoryreport/inventoryreport.html',
             controller: 'inventoryreportController',
             resolve: {
                 lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                     return $ocLazyLoad.load([{
                         name: 'GasApp',
                         files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/inventoryreport/inventoryreportController.js']
                     }]);
                 }],
                 "check": function ($state, $rootScope) {
                 		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
             },
             authenticated: true
         })
	      .state('gassalereport', {
	        url: '/gassalereport',
	        parent: 'template',
	        templateUrl: 'components/gas/reports/salereport/salereport.html',
	        controller: 'gasSaleReportController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/salereport/salereport.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
			.state('gassalereportmonth', {
                url: '/gassalereportmonth',
                parent: 'template',
                templateUrl: 'components/gas/reports/salereportmonth/salereportmonth.html',
                controller: 'GasSaleReportMonthController',
                resolve: {
                    lazy: ['$ocLazyLoad', function($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/gas/gasService.js',  '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js','components/gas/reports/salereportmonth/salereportmonth.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                    	if($rootScope.AvailGas == "N"){		            		             		
	                          $state.go('dashboard');
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                          $state.go('dashboard');
		            	}
	                }
                },
                authenticated:true
           })
			.state('shiftendreport', {
                url: '/shiftendreport',
                parent: 'template',
                templateUrl: 'components/gas/reports/shiftendreport/shiftendreport.html',
                controller: 'ShiftEndController',
                resolve: {
                    lazy: ['$ocLazyLoad', function($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/gas/gasService.js',  '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js','components/gas/reports/shiftendreport/shiftendreport.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                    	if($rootScope.AvailGas == "N"){		            		             		
	                          $state.go('dashboard');
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                          $state.go('dashboard');
		            	}
	                }
                },
                authenticated:true
           })
			.state('stockinwardreport', {
                url: '/stockinwardreport',
                parent: 'template',
                templateUrl: 'components/gas/reports/stockinward/stockinward.html',
                controller: 'StockInwardReportController',
                resolve: {
                    lazy: ['$ocLazyLoad', function($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/gas/gasService.js',  '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js','components/gas/reports/stockinward/stockinward.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                    	if($rootScope.AvailGas == "N"){		            		             		
	                          $state.go('dashboard');
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                          $state.go('dashboard');
		            	}
	                }
                },
                authenticated:true
           })
	      .state('gasreconcillationreport', {
              url: '/gasreconcillationreport',
              parent: 'template',
              templateUrl: 'components/gas/reports/reconcillationreport/reconcillationreport.html',
              controller: 'reconcillationreport',
              resolve: {
                  lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                      return $ocLazyLoad.load([{
                          name: 'GasApp',
                          files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/reconcillationreport/reconcillationreport.js']
                      }]);
                  }],
                  "check": function ($state, $rootScope) {
                  		if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
              },
              authenticated: true
          })
		  .state('purchaseregister', {
	        url: '/purchaseregister',
	        parent: 'template',
	        templateUrl: 'components/gas/reports/purchaseregister/purchaseregister.html',
	        controller: 'PurchaseRegisterController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/purchaseregister/purchaseregister.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
			.state('saletrend', {
	        url: '/saletrend',
	        parent: 'template',
	        templateUrl: 'components/gas/reports/saletrend/saletrend.html',
	        controller: 'SaleTrendController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/saletrend/saletrend.js']
	                }]);
	            }],
	            "check": function ($state, $rootScope) {
	            		if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	        },
	        authenticated:true
	      })
           .state('lotteryBookReturn', {
               url: '/lotteryBookReturn',
               parent: 'template',
               templateUrl: 'components/lottery/reports/returnbooks/returnbooks.html',
               controller: 'returnbooksController',
               resolve: {
                   lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([{
                           name: 'GasApp',
                           files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/returnbooks/returnbooks.js']
                       }]);
                   }],
                   "check": function ($state, $rootScope) {
                   		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
               },
               authenticated: true
           })
           .state('lotteryLedger', {
               url: '/lotteryLedger',
               parent: 'template',
               templateUrl: 'components/lottery/reports/ledger/lotteryledger.html',
               controller: 'lotteryledgerController',
               resolve: {
                   lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([{
                           name: 'GasApp',
                           files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/ledger/lotteryledgerController.js']
                       }]);
                   }],
                   "check": function ($state, $rootScope) {
                   		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
               },
               authenticated: true
           })
           .state('lotterycashtransfer', {
               url: '/lotterycashtransfer',
               parent: 'template',
               templateUrl: 'components/lottery/cashtransfer/cashtransfer.html',
               controller: 'lotterycashtransferController',
               resolve: {
                   lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([{
                           name: 'GasApp',
                           files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/cashtransfer/lotterycashtransferController.js']
                       }]);
                   }],
                   "check": function ($state, $rootScope) {
		            	if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
               },
               authenticated: true
           })
           .state('dayendreport', {
			   url: '/dayendreport',
			   parent: 'template',
			   templateUrl: 'components/lottery/reports/dayendreport/dayendreport.html',
			   controller: 'dayendreportController',
			   resolve: {
			       lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
			           return $ocLazyLoad.load([{
			               name: 'GasApp',
			               files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/dayendreport/dayendreportController.js']
			           }]);
			       }],
			       "check": function ($state, $rootScope) {
			       		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
			   },
			   authenticated: true
			})
			.state('saletrendgamewise', {
			   url: '/saletrendgamewise',
			   parent: 'template',
			   templateUrl: 'components/lottery/reports/saletrendgamewise/saletrendgamewise.html',
			   controller: 'saletrendgamewiseController',
			   resolve: {
			       lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
			           return $ocLazyLoad.load([{
			               name: 'GasApp',
			               files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/saletrendgamewise/saletrendgamewiseController.js']
			           }]);
			       }],
			       "check": function ($state, $rootScope) {
			       		if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
			   },
			   authenticated: true
			})			
            .state('gasmonthlyreport', {
                url: '/gasmonthlyreport',
                parent: 'template',
                templateUrl: 'components/gas/reports/monthlyreport/monthlyreport.html',
                controller: 'MonthlyStatementController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/monthlyreport/monthlyreport.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                    	if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
                },
                authenticated: true
            })
			.state('profitloss', {
			   url: '/profitloss',
			   parent: 'template',
			   templateUrl: 'components/gas/reports/profitloss/profitloss.html',
			   controller: 'profitlossController',
			   resolve: {
			       lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
			           return $ocLazyLoad.load([{
			               name: 'GasApp',
			               files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/profitloss/profitlossController.js']
			           }]);
			       }],
			       "check": function ($state, $rootScope) {
			       		if($rootScope.AvailGas == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
			   },
			   authenticated: true
			})
			.state('accountcreation', {
	            url: '/accountcreation',
	            parent: 'template',
	            templateUrl: 'components/setup/accountcreation/accountcreation.html',
	            controller: 'AccountCreationController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/accountcreation/accountcreation.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
			.state('businesspurchase', {
	            url: '/businesspurchase',
	            parent: 'template',
	            templateUrl: 'components/setup/purchases/purchases.html',
	            controller: 'BusinessPurchasesController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/purchases/purchasesController.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
            .state('newgame', {
	            url: '/newgame',
	            parent: 'template',
	            templateUrl: 'components/setup/newgame/newgame.html',
	            controller: 'newgameController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/lottery/lotteryService.js', 'components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/newgame/newgameController.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
	        .state('manualsettlepack', {
                url: '/manualsettlepack',
                parent: 'template',
                templateUrl: 'components/setup/manualsettlepack/manualsettlepack.html',
                controller: 'ManualSettlePackController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/lottery/lotteryService.js', 'components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/manualsettlepack/manualsettlepack.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
                },
                authenticated: true
            })
            .state('returnbooks', {
                url: '/returnbooks',
                parent: 'template',
                templateUrl: 'components/setup/returnbooks/returnbooks.html',
                controller: 'ReturnBooksSetupController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/lottery/lotteryService.js', 'components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/returnbooks/returnbooks.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
                },
                authenticated: true
            })
            .state('updatecomm', {
                url: '/updatecomm',
                parent: 'template',
                templateUrl: 'components/setup/updatecomm/updatecomm.html',
                controller: 'UpdateCommController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/lottery/lotteryService.js', 'components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/updatecomm/updatecomm.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
                },
                authenticated: true
            })
 			.state('reportconfig', {
                url: '/reportconfig',
                parent: 'template',
                templateUrl: 'components/setup/reportconfig/reportconfig.html',
                controller: 'ReportConfigController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/setup/setupService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/setup/reportconfig/reportconfig.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
                },
                authenticated: true
            }) 			
            .state('createstore', {
	            url: '/createstore',
	            parent: 'template',
	            templateUrl: 'components/admin/storecreation/store.html',
	            controller: 'storeController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/admin/adminService.js','../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/admin/storecreation/store.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
	        .state('createuser', {
	            url: '/createuser',
	            parent: 'template',
	            templateUrl: 'components/admin/usercreation/user.html',
	            controller: 'userController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/admin/adminService.js','../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/admin/usercreation/user.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
	        .state('addlottery', {
	            url: '/addlottery',
	            parent: 'template',
	            templateUrl: 'components/admin/addlottery/addlottery.html',
	            controller: 'addlotteryController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/admin/adminService.js','../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/admin/addlottery/addlottery.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
	        .state('addgas', {
	            url: '/addgas',
	            parent: 'template',
	            templateUrl: 'components/admin/addgas/addgas.html',
	            controller: 'addgasController',
	            resolve: {
	                lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
	                    return $ocLazyLoad.load([{
	                        name: 'GasApp',
	                        files: ['components/admin/adminService.js','../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/admin/addgas/addgas.js']
	                    }]);
	                }],
	                "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
	            },
	            authenticated: true
	        })
	        .state('storechange', {
		        url: '/storechange',
		        parent: 'template',
		        templateUrl: 'components/admin/storechange/storechange.html',
		        controller: 'StoreChangeController',
		        resolve: {
		            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
		                return $ocLazyLoad.load([{
		                    name: 'GasApp',
		                    files: ['components/admin/adminService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/admin/storechange/storechange.js']
		                }]);
		            }],
		            "check": function ($state, $rootScope) {
		            	if($rootScope.AccessType != "SUPER_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
		        },
		        authenticated:true
		     })     
	        .state('paymentdue', {
		        url: '/paymentdue',
		        parent: 'template',
		        templateUrl: 'components/lottery/reports/paymentdue/paymentdue.html',
		        controller: 'PaymentDueController',
		        resolve: {
		            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
		                return $ocLazyLoad.load([{
		                    name: 'GasApp',
		                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/paymentdue/paymentdue.js']
		                }]);
		            }],
		            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
		        },
		        authenticated:true
		     })
		     .state('booksactivenotsettled', {
		        url: '/booksactivenotsettled',
		        parent: 'template',
		        templateUrl: 'components/lottery/reports/booksactivenotsettled/booksactivenotsettled.html',
		        controller: 'BooksActiveNotSettledController',
		        resolve: {
		            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
		                return $ocLazyLoad.load([{
		                    name: 'GasApp',
		                    files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/booksactivenotsettled/booksactivenotsettled.js']
		                }]);
		            }],
		            "check": function ($state, $rootScope) {
		            	if($rootScope.AvailLottery == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
		        },
		        authenticated:true
		     })
	        .state('logout', {
              name: 'logout',
              url: '/logout',
              controller: 'LogoutController',
               resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['../assets/css/sweetalert.css','../css/theme.css','../assets/lib/sweetalert.min.js','shared/login/logoutController.js']
	                }]);
	            }]
	        },
	        authenticated:false
          })
		  .state('gasLedger', {
                url: '/gasLedger',
                parent: 'template',
                templateUrl: 'components/gas/reports/ledger/gasledger.html',
                controller: 'gasledgerController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/ledger/gasledgerController.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                        if ($rootScope.AvailLottery == "N") {
                            $state.go('dashboard')
                        }
                        if ($rootScope.AccessType == "STORE_USER") {
                            $state.go('dashboard')
                        }
                    }
                },
                authenticated: true
            })
             .state('addcreditcard', {
                 url: '/addcreditcard',
                 parent: 'template',
                 templateUrl: 'components/admin/addcreditcard/addcreditcard.html',
                 controller: 'AddCreditCardController',
                 resolve: {
                     lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                         return $ocLazyLoad.load([{
                             name: 'GasApp',
                             files: ['components/admin/adminService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/admin/addcreditcard/addcreditcard.js']
                         }]);
                     }],
                     "check": function ($state, $rootScope) {
                         if ($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN") {
                             $state.go('dashboard')
                         }
                     }
                 },
                 authenticated: true
             })
            .state('lotterygames', {
                url: '/lotterygames',
                parent: 'template',
                templateUrl: 'components/lottery/reports/lotterygames/lotterygames.html',
                controller: 'lotterygamesController',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/lottery/reports/lotterygames/lotterygamesController.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                        if ($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN") {
                            $state.go('dashboard')
                        }
                    }
                },
                authenticated: true
            })
            .state('recgasdealeraccount', {
                url: '/recgasdealeraccount',
                parent: 'template',
                templateUrl: 'components/gas/reports/recgasdealeraccount/recgasdealeraccount.html',
                controller: 'recgasdealeraccountContoller',
                resolve: {
                    lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([{
                            name: 'GasApp',
                            files: ['components/gas/gasService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/gas/reports/recgasdealeraccount/recgasdealeraccountContoller.js']
                        }]);
                    }],
                    "check": function ($state, $rootScope) {
                        if ($rootScope.AccessType != "SUPER_USER" || $rootScope.AccessType != "STORE_ADMIN") {
                            $state.go('dashboard')
                        }
                    }
                },
                authenticated: true
            })
			.state('saleentry', {
              url: '/saleentry',
              parent: 'template',
              templateUrl: 'components/business/saleentry/saleentry.html',
              controller: 'SaleEntryController',
              resolve: {
                  lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                      return $ocLazyLoad.load([{
                          name: 'GasApp',
                          files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/business/saleentry/saleentry.js']
                      }])
                  }],
                  "check": function ($state, $rootScope) {
		            	if($rootScope.AvailBusiness == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
              },
              authenticated: true
          })
          .state('bank', {
          url: '/bank',
          parent: 'template',
          templateUrl: 'components/business/bank/bank.html',
          controller: 'BankController',
          resolve: {
              lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                  return $ocLazyLoad.load([{
                      name: 'GasApp',
                      files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/business/bank/bank.js']
                  }])
              }],
              "check": function ($state, $rootScope) {
	            	if($rootScope.AvailBusiness == "N"){		            		             		
                         $state.go('dashboard')
	            	}
                }
          },
          authenticated: true
          })
          .state('cashentry', {
          url: '/cashentry',
          parent: 'template',
          templateUrl: 'components/business/cashentry/cashentry.html',
          controller: 'CashEntryController',
          resolve: {
              lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                  return $ocLazyLoad.load([{
                      name: 'GasApp',
                      files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/business/cashentry/cashentry.js']
                  }])
              }],
              "check": function ($state, $rootScope) {
	            	if($rootScope.AvailBusiness == "N"){		            		             		
                         $state.go('dashboard')
	            	}
                }
          },
          authenticated: true
          })
          .state('businessledgerreport', {
               url: '/businessledgerreport',
               parent: 'template',
               templateUrl: 'components/business/reports/ledger/ledger.html',
               controller: 'BusinessLedgerController',
               resolve: {
                   lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([{
                           name: 'GasApp',
                           files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/business/reports/ledger/ledger.js']
                       }]);
                   }],
                   "check": function ($state, $rootScope) {
                   		if($rootScope.AvailBusiness == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
               },
               authenticated: true
           })
			.state('businessshiftendreport', {
               url: '/businessshiftendreport',
               parent: 'template',
               templateUrl: 'components/business/reports/shiftendreport/shiftendreport.html',
               controller: 'ShiftEndController',
               resolve: {
                   lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([{
                           name: 'GasApp',
                           files: ['components/business/businessService.js','components/gas/gasService.js','components/lottery/lotteryService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/business/reports/shiftendreport/shiftendreport.js']
                       }]);
                   }],
                   "check": function ($state, $rootScope) {
                   		if($rootScope.AvailBusiness == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
               },
               authenticated: true
           })
			.state('businessmonthlyreport', {
               url: '/businessmonthlyreport',
               parent: 'template',
               templateUrl: 'components/business/reports/monthlystmtreport/businessmonthlyreport.html',
               controller: 'MonthlyStmtController',
               resolve: {
                   lazy: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([{
                           name: 'GasApp',
                           files: ['components/business/businessService.js', '../assets/css/sweetalert.css', '../assets/lib/sweetalert.min.js', 'components/business/reports/monthlystmtreport/businessmonthlyreport.js']
                       }]);
                   }],
                   "check": function ($state, $rootScope) {
                   		if($rootScope.AvailBusiness == "N"){		            		             		
	                         $state.go('dashboard')
		            	}
		            	if($rootScope.AccessType == "STORE_USER"){		            		             		
	                         $state.go('dashboard')
		            	}
	                }
               },
               authenticated: true
           })
	      .state('login', {
	      	name:'login',
	        url: '/login',
	        templateUrl: 'shared/login/login.html',
	        controller: 'LoginController',
	        resolve: {
	            lazy: ['$ocLazyLoad', function($ocLazyLoad) {
	                return $ocLazyLoad.load([{
	                    name: 'GasApp',
	                    files: ['../assets/css/sweetalert.css','../css/theme.css','../assets/lib/sweetalert.min.js','shared/login/loginController.js']
	                }]);
	            }]
	        },
	        authenticated:false
	      });
	      

   	 	$urlRouterProvider.otherwise('/login');
  	}
]);

app.run(['$rootScope', '$location','$state','loginService','$http','$cookies',
    function ($rootScope, $location,$state,loginService,$http, $cookies) {  
        //settign gloabal values to rootscope        
        
		$rootScope.Status = $cookies.get('Status');
        $rootScope.UserName = $cookies.get('UserName');
        $rootScope.AccessType = $cookies.get('AccessType');
        $rootScope.GroupID = $cookies.get('GroupID');
        $rootScope.StoreID = $cookies.get('StoreID');       	
        $rootScope.StoreType = $cookies.get('StoreType');
        $rootScope.StoreAdd1 = $cookies.get('StoreAdd1');
        $rootScope.StoreAdd2 = $cookies.get('StoreAdd2');        
        $rootScope.AvailBusiness = $cookies.get('AvailBusiness');
        $rootScope.AvailGas = $cookies.get('AvailGas');
        $rootScope.AvailLottery = $cookies.get('AvailLottery');
        $rootScope.StoreName = $cookies.get('StoreName');

		$rootScope.$on("$stateChangeStart", function(event, toState, toParams, fromState, fromParams) {
		var shouldLogin = toState.authenticated !== undefined
            && toState.authenticated;
        // NOT authenticated - wants any private stuff
        if(shouldLogin || fromState.name === "") {
            var token = $cookies.get('auth') == null ? null : $cookies.get('auth');
            if (token == null) {
                if(toState.name === 'login')
                    return;
                $state.go('login');
                event.preventDefault();
            } else {
                if(toState.name === toState.name)
                {
                    return;
                    $state.go(toState.name);
                	event.preventDefault();
                }else{
                	return;
                    $state.go('dashboard');
                	event.preventDefault();
                }
                //TODO: Check token
               
            }
        }

        if(!loginService.getAuthStatus()){
        	$state.go('login');
            event.preventDefault();
        }  
    });
}]);

app.factory('Excel', function ($window) {
    var uri = 'data:application/vnd.ms-excel;base64,',
        template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
        base64 = function (s) { return $window.btoa(unescape(encodeURIComponent(s))); },
        format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };
    return {
        tableToExcel: function (tableId, worksheetName) {
            var table = $(tableId),
                ctx = { worksheet: worksheetName, table: table.html() },
                href = uri + base64(format(template, ctx));
            return href;
        }
    };
});

app.factory('httpPreConfig', ['$http', '$rootScope', function ($http, $rootScope) {
    $http.defaults.transformRequest.push(function (data) {
        //document.getElementById("mydiv").style.display = "block";
        $("#loadingImg").show();
        return data;
    });
    $http.defaults.transformResponse.push(function (data) {
        //document.getElementById("mydiv").style.display = "none";
        $("#loadingImg").hide();
        return data;
    })
    return $http;
}]);