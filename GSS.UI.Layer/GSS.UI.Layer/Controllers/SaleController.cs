using GSS.BusinessLogic.Layer;
using GSS.Data.Model;
using GSS.DataAccess.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GSS.UI.Layer.Controllers
{
    public class SaleController : ApiController
    {
        #region Gas Sales

        [HttpPost]
        public HttpResponseMessage SaveGasSales(SaleMaster objGasSale)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objGasSale, "GAS_SALES");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Sale"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveGasCardBreakup(SaleMaster objGasSale)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                GasValidation _validateGas = new GasValidation();

                if (_validateGas.ValidateGasEntryMadeForDay(objGasSale))
                {
                    _dalSaleEntries.AddOrUpdateSale(objGasSale, "GAS_CARD_BREAKUP");
                }
                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Sale"
                };
                throw new HttpResponseException(resp);
            }

        }
       
        [HttpPost]
        public HttpResponseMessage SaveGasInventory(SaleMaster objGasSale)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objGasSale, "GAS_INVENTORY_UPDATE");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveGasReceipt(List<GasReceipt> objGasReceipt)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateGasReceipt(objGasReceipt);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Gas Sale"
                };
                throw new HttpResponseException(resp);
            }
        }

        #endregion

        [HttpPost]
        public HttpResponseMessage SaveChequeCashing(SaleMaster objChequeCashing)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objChequeCashing, "CHEQUE_CASHING");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Cheque Cashing"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveChequeDeposit(SaleMaster objChequeCashing)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objChequeCashing, "CHEQUE_DEPOSIT");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Cheque Cashing"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpGet]
        public IHttpActionResult GetRunningShift(int ID)
        {
            try
            {
                SaleSupportEntries storeShift = new SaleSupportEntries();
                LotteryRunningShift runningShift = storeShift.GetRunningShift(ID);

                return Ok(runningShift);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }


        #region Purchases and Purchase Return

        [HttpPost]
        public HttpResponseMessage SavePurchase(SaleMaster objPurchase)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objPurchase, "PURCHASE");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Purchases"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage SavePurchaseReturn(SaleMaster objPurchaseReturn)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objPurchaseReturn, "PURCHASE_RETURN");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Purchase Return"
                };
                throw new HttpResponseException(resp);
            }

        }

        #endregion
        
        #region Lottery Sales

        [HttpPost]
        public HttpResponseMessage SaveLotterySales(SaleMaster objLotterySale)
        {
            try
            {
                LotteryValidation _valLottery = new LotteryValidation();
                SaleEntries _dalSaleEntries = new SaleEntries();

                if (_valLottery.LotteryClosing(objLotterySale))
                {
                    _dalSaleEntries.AddOrUpdateSale(objLotterySale, "LOTTERY_SALE");
                }

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetEmptyBoxes(SaleMaster objLotterySale)
        {
            try
            {
                LotteryClosingInstantSale objClosingInstantSale = new LotteryClosingInstantSale();
                LotteryValidation _valLottery = new LotteryValidation();

                objClosingInstantSale = _valLottery.GetEmptyBoxes(objLotterySale);
                return Ok(objClosingInstantSale);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }

        public HttpResponseMessage SaveLotteryTransfer(SaleMaster objLotterySale)
        {
            try
            {
                LotteryValidation _valLottery = new LotteryValidation();
                SaleEntries _dalSaleEntries = new SaleEntries();


                _dalSaleEntries.AddOrUpdateSale(objLotterySale, "LOTTERY_TRANSFER");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Sale"
                };
                throw new HttpResponseException(resp);
            }

        }
        #endregion

        #region Account Transaction
        [HttpPost]
        public HttpResponseMessage SaveAccountTransaction(SaleMaster objAccount)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "ACCOUNT_TRAN");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }

        public HttpResponseMessage SaveJournalVoucher(JournalVoucher objEntry)
        {
            try
            {
                SaleMaster objAccount = new SaleMaster();
                AccountPaidReceivables objVoucher = new AccountPaidReceivables();
                objVoucher.AccountLedgerID = objEntry.AccountLedgerID;
                objEntry.AccountTranType = objEntry.AccountTranType.ToUpper().Trim();

                if (objEntry.VoucherType == "PAYMENT")
                {
                    if (objEntry.AccountTranType == "MO" || objEntry.AccountTranType == "CASH")
                        objVoucher.AccountTranType = "CP";

                    else if (objEntry.AccountTranType == "CHEQUE" || objEntry.AccountTranType == "CASHIER CHEQUE" || objEntry.AccountTranType == "AUTOMATIC BANK PAYMENTS(EFT)" || objEntry.AccountTranType == "AUTOMATIC BANK RECEIPTS")
                        objVoucher.AccountTranType = "QP";
                }

                else if (objEntry.VoucherType == "RECEIPT")
                {
                    if (objEntry.AccountTranType == "MO" || objEntry.AccountTranType == "CASH")
                        objVoucher.AccountTranType = "CR";

                    else if (objEntry.AccountTranType == "CHEQUE" || objEntry.AccountTranType == "CASHIER CHEQUE" || objEntry.AccountTranType == "AUTOMATIC BANK PAYMENTS(EFT)" || objEntry.AccountTranType == "AUTOMATIC BANK RECEIPTS")
                        objVoucher.AccountTranType = "QR";
                }

                objVoucher.Amount = objEntry.Amount;
                objVoucher.DisplayName = objEntry.DisplayName;
                objVoucher.PaymentRemarks = objEntry.DisplayName + " - " + objEntry.AccountTranType + " - " + objEntry.PaymentRemarks;

                objAccount.StoreID = objEntry.StoreID;
                objAccount.ShiftCode = objEntry.ShiftCode;
                objAccount.Date = objEntry.Date;
                objAccount.CreatedUserName = objEntry.CreatedUserName;
                objAccount.ModifiedUserName = objEntry.CreatedUserName;
                
                List<AccountPaidReceivables> objActColl = new List<AccountPaidReceivables>();
                objActColl.Add(objVoucher);

                objAccount.PaymentAccounts = objActColl;

                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "ACCOUNT_TRAN");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }


        #endregion

        #region Business Sale Transaction
        [HttpPost]
        public HttpResponseMessage SaveBusinessSale(SaleMaster objAccount)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "BUSINESS_SALE");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Transaction"
                };
                throw new HttpResponseException(resp);
            }
        }

        // Option is added by Kiran but to be tested for delete payment
        [HttpPost]
        public HttpResponseMessage DeletePaymentOrReceipt(SaleMaster objAccount)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.DeletePayment(objAccount);

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Account Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }
        #endregion

        #region Cash Deposit Transaction
        [HttpPost]
        public HttpResponseMessage SaveCashDeposit(SaleMaster objAccount)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "CASH_DEPOSIT");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Cash Deposit Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }

        #endregion

        #region Finalize Transaction
        [HttpPost]
        public HttpResponseMessage FinalizeTransaction(SaleMaster objAccount)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "FINALIZE_TRAN");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Finalize Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }

        #endregion

        #region Finalize Transaction
        [HttpPost]
        public HttpResponseMessage UnlockDay(SaleMaster objAccount)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "UNLOCK_DAY");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Finalize Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }

        #endregion

        [HttpPost]
        public HttpResponseMessage Test()
        {
            try
            {
                SaleMaster objAccount = new SaleMaster();
                objAccount.StoreID = 3;
                objAccount.Date = Convert.ToDateTime("01-Sep-2016");
                objAccount.CreatedUserName = "store616";
                objAccount.ModifiedUserName = "store616";
                SaleEntries _dalSaleEntries = new SaleEntries();
                _dalSaleEntries.AddOrUpdateSale(objAccount, "FINALIZE_TRAN");

                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Finalize Transaction"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetDaySale(SelectDay objSaleDay)
        {
            try
            {
                SaleEntries _dalSaleEntries = new SaleEntries();
                SaleMaster objSaleMaster = new SaleMaster();
                if (objSaleDay.ShiftCode == 0)
                    objSaleDay.ShiftCode = 1;

                objSaleMaster = _dalSaleEntries.GetSaleTransaction(objSaleDay.StoreID,objSaleDay.Date, objSaleDay.ShiftCode,  objSaleDay.RequestType);

                if (objSaleMaster.StoreID == 0)
                {
                    return Ok("No Data Found");
                }
                else
                {
                    return Ok(objSaleMaster);
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Sale Day Report"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetCashDeposit(SelectDay objSaleDay)
        {
            try
            {
                SaleSupportEntries _dalSaleEntries = new SaleSupportEntries();
                BankDepositForm objDeposit = new BankDepositForm();

                objDeposit = _dalSaleEntries.BankDepositDetails(objSaleDay.StoreID, objSaleDay.Date, objSaleDay.ShiftCode);
                return Ok(objDeposit);
                
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Sale Day Report"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public IHttpActionResult GetCashFlow(SelectDay objSaleDay)
        {
            try
            {
                List<AccountModel> BusienssTranColl = new List<AccountModel>();
                BusienssTranColl = new SaleSupportEntries().CashInOutFlow(objSaleDay.StoreID, objSaleDay.Date, objSaleDay.ShiftCode);
                return Ok(BusienssTranColl);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Sale Day Report"
                };
                throw new HttpResponseException(resp);
            }
        }



        [HttpGet]
        public IHttpActionResult GetStudentDetails()
        {
            try
            {
                List<TestStudent> TestStudent = new List<TestStudent>();
                TestStudent objEmploy;
                Marks mark;
                List<Marks> marks;
                
                objEmploy = new TestStudent();
                objEmploy.StudentID = 1;
                objEmploy.StudentName = "Test 1";
                marks = new List<Marks>();
                mark = new Marks();
                mark.Mark = 80;
                mark.Subject = "English";
                marks.Add(mark);
                
                mark = new Marks();
                mark.Mark = 90;
                mark.Subject = "Telugu";
                marks.Add(mark);

                objEmploy.Marks = marks;
                TestStudent.Add(objEmploy);

                objEmploy = new TestStudent();
                marks = new List<Marks>();
                objEmploy.StudentID = 2;
                objEmploy.StudentName = "Test 2";

                mark = new Marks();
                mark.Mark = 50;
                mark.Subject = "English";
                marks.Add(mark);

                mark = new Marks();
                mark.Mark = 60;
                mark.Subject = "Telugu";
                marks.Add(mark);
                objEmploy.Marks = marks;

                TestStudent.Add(objEmploy);

                return Ok(TestStudent);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Lottery Controller"
                };
                throw new HttpResponseException(resp);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveStudents(List<TestStudent> objStudents)
        {
            try
            {
                var tmp = new HttpResponseMessage(HttpStatusCode.Created);
                return tmp;

            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Error {0}", ex.Message)),
                    ReasonPhrase = "Error in Student Transaction"
                };
                throw new HttpResponseException(resp);
            }

        }

    }


    public class SelectDay
    {
        public int StoreID { get; set; }
        public DateTime Date { get; set; }
        public int ShiftCode { get; set; }
        public string RequestType { get; set; }
    }
}
