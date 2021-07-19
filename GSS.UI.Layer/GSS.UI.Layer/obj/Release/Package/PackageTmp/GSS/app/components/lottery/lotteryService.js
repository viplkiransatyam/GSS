angular.module('GasApp').service('lotteryService', ['$http', function ($http) {
    const getShiftsPerStoreURL = "/Api/StoreMaster/GetShifts";
    const GetRunningShiftURL = "/Api/Lottery/GetRunningShift";
    const saveReceivedBooksURL = "/Api/Lottery/AddBooksReceive";
    const getBoxesStoreURL = "/Api/Lottery/GetLotteryBoxes";
    const saveActiveBooksURL = "/Api/Lottery/AddBooksActive";
    const getClosingTicketsURL = "/Api/Lottery/ScanTicketForClosingRead";
    const saveDailyClosingURL = "/Api/Sale/SaveLotterySales";
    const getLotteryReceiveURL = "/Api/Lottery/GetLotteryReceive";
    const getLotteryActiveURL = "/Api/Lottery/GetLotteryActive";
    const GetDaySaleTransURL = "/Api/Sale/GetDaySale";
    const getLotterySaleReportURL = "/Api/Lottery/GetLotterySaleReport";
    const getLotteryActiveBooksReportURL = "/Api/Lottery/GetLotteryBookActiveReport";
    const getLotteryReceiveBooksReportURL = "/Api/Lottery/GetLotteryBookReceiveReport";
    const getLotteryDetailedSalesReportURL = "/Api/Lottery/GetLotteryDailyReport";
    const getLotteryInventoryReportURL = "/Api/Lottery/GetInventoryReport";
    const getLotteryPaymentsURL = "/Api/Lottery/GetAutoSettle";
    const getUpdateLotteryPaymentURL = "/Api/Lottery/UpdateLotteryPayment";    
    const getLotterySalesMonth = "/Api/Lottery/GetLotteryMonthlySaleReport";
    const GetLotteryReturnURL = "/Api/Lottery/GetLotteryReturn";
    const getLotteryLedger = "/Api/Report/GetLedger";
    const getLotteryAccount = "/Api/AccoutMaster/GetLotteryAccounts";
    const getDailySale = "/Api/Sale/GetDaySale";
    const putlotterycashtransfer = "/Api/Sale/SaveLotteryTransfer";
    const getReturnBooks = "/Api/Lottery/GetLotteryBookReturnReport";
    const notSettledBooksListURL = "/Api/Lottery/SelectBooksActiveButNotSettled";
    const PaymentDueURL = "/Api/Lottery/GetPaymentDue";
    const getLotteryGameSaleTrend = "/Api/Lottery/GetLotteryGameSaleTrend";
    const getLotteryDayEndReport = "/Api/Lottery/GetLotteryDayEndReport";
    const getEmptyBoxesURL = "/Api/Sale/GetEmptyBoxes";
    const getUnsaveddataURL = "/Api/Lottery/UnsavedClosingReading";
    const getLotteryGamesURL = "/Api/Lottery/GetLotteryGames";



    var lotteryService = [];
    lotteryService.getAllGames = function (storeID) {
        return $http.get('/Api/Lottery/GetLotteryGames/' + storeID);
    }
    //Get Shifts
    lotteryService.getShifts = function (storeID) {
        return $http({
            method: 'GET',
            url: getShiftsPerStoreURL + '/' + storeID,
        });
    }

    //Get Shifts
    lotteryService.getRunningShift = function (storeID) {
        return $http({
            method: 'GET',
            url: GetRunningShiftURL + '/' + storeID,
        });
    }
    
    //Get Boxes
    lotteryService.getBoxes = function (storeID) {
        return $http({
            method: 'GET',
            url: getBoxesStoreURL + '/' + storeID,
        });
    }

    // Report :: Lottery Deetailed Sale Report
    lotteryService.getLotteryDetailedSalesReport = function (postData) {
        return $http({
            method: 'POST',
            url: getLotteryDetailedSalesReportURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    };

    // Report :: Lottery Receive Books Report
    lotteryService.getLotteryReceiveBooksReport = function (postData) {
        return $http({
            method: 'POST',
            url: getLotteryReceiveBooksReportURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    };

    // Report :: Lottery Active Books Report
    lotteryService.getLotteryActiveBooksReport = function (postData) {
        return $http({
            method: 'POST',
            url: getLotteryActiveBooksReportURL,
            headers: {
                'Content-Type':'application/json'
            },
            data: postData
        });
    };

    // Report :: Lottery Sale Report
    lotteryService.getLotterySaleReport = function (postData) {
        return $http({
            method: 'POST',
            url: getLotterySaleReportURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    };

     //Get Day Sale Transactions.
    lotteryService.getPreviousDayTranscations = function (postData) {
        return $http({
            method: 'POST',
            url: GetDaySaleTransURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //save receive books
    lotteryService.saveReceivedBooks = function (postData) {
        return $http({
            method: 'POST',
            url: saveReceivedBooksURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    //getLotteryReceived
     lotteryService.getLotteryReceive = function (postData) {
        return $http({
            method: 'POST',
            url: getLotteryReceiveURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }    


    lotteryService.saveActiveBooks = function (postData) {
        return $http({
            method: 'POST',
            url: saveActiveBooksURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

     //getLotteryActive
     lotteryService.getLotteryActive = function (postData) {
        return $http({
            method: 'POST',
            url: getLotteryActiveURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    lotteryService.getClosingTickets = function (postData) {
        return $http({
            method: 'POST',
            url: getClosingTicketsURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    lotteryService.saveDailyClosing = function (postData) {
        return $http({
            method: 'POST',
            url: saveDailyClosingURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

     //Inventory Service
    lotteryService.getLotteryInventoryReportData = function (postData) {
        return $http({
            method: 'GET',
            url: getLotteryInventoryReportURL + "/" + postData,
            headers: {
                'Content-Type': 'application/json'
            }
            
        });
    }

     lotteryService.getLotteryPayments = function (storeID) {
        return $http({
            method: 'GET',
            url: getLotteryPaymentsURL + '/' + storeID,
        });
    }

    lotteryService.updateLotteryPayment = function (postData) {
        return $http({
            method: 'POST',
            url: getUpdateLotteryPaymentURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    lotteryService.getLotterySalesMonth = function (postData) {
         return $http({
             method: 'POST',
             url: getLotterySalesMonth,
             headers: {
                 'Content-Type': 'application/json'
             },
             data: postData
         });
     }
     lotteryService.getLotteryReturns = function (postData) {
         return $http({
             method: 'POST',
             url: GetLotteryReturnURL,
             headers: {
                 'Content-Type': 'application/json'
             },
             data: postData
         });
     }

     lotteryService.getLedger = function (postData) {
       return $http({
           method: 'POST',
           url: getLotteryLedger,
           headers: {
               'Content-Type': 'application/json'
           },
           data: postData
       });
   }

lotteryService.GetDaySale = function (postData) {
       return $http({
           method: 'POST',
           url: getDailySale,
           headers: {
               'Content-Type': 'application/json'
           },
           data: postData
       });
   }

   lotteryService.savelotterycashtrans = function (postData) {
       return $http({
           method: 'POST',
           url: putlotterycashtransfer,
           headers: {
               'Content-Type': 'application/json'
           },
           data: postData
       });
   }

   lotteryService.getReturnBooks = function (postData)
   {
       return $http({
           method: 'POST',
           url: getReturnBooks,
           headers: {
               'Content-Type': 'application/json'
           },
           data: postData
       });

   }

   lotteryService.getLotteryAccount = function (storeID) {
       return $http({
           method: 'GET',
           url: getLotteryAccount + '/' + storeID,
       });
   }

   lotteryService.notSettledBooksList = function (storeID) {
        return $http({
            method: 'GET',
            url: notSettledBooksListURL + '/' + storeID,
        });
    }
    lotteryService.getPaymentDues = function (storeID) {
        return $http({
            method: 'GET',
            url: PaymentDueURL + '/' + storeID,
        });
    }
    lotteryService.getLotteryDayEndReport = function (postData) {
      return $http({
          method: 'POST',
          url: getLotteryDayEndReport,
          headers: {
              'Content-Type': 'application/json'
          },
          data: postData
      });
    }
    lotteryService.getLotteryGameSaleTrend = function (postData) {
      return $http({
          method: 'POST',
          url: getLotteryGameSaleTrend,
          headers: {
              'Content-Type': 'application/json'
          },
          data: postData
      });
    }


    lotteryService.confirmInstantSale = function (postData) {
        return $http({
            method: 'POST',
            url: getEmptyBoxesURL,
            headers: {
                'Content-Type': 'application/json'
            },
            data: postData
        });
    }

    lotteryService.getUnsaveddata = function (storeID) {
        return $http({
            method: 'GET',
            url: getUnsaveddataURL + '/' + storeID,
        });
    }
    lotteryService.getLotteryGames = function (storeID) {
        return $http({
            method: 'GET',
            url: getLotteryGamesURL + '/' + storeID,
        });
    }

    return lotteryService;
}]);    