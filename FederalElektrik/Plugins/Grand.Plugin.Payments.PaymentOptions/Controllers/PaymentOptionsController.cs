using Grand.Core;
using Grand.Core.Domain.Configuration;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Domain.Points;
using Grand.Framework.Mvc.Filters;
using Grand.Framework.UI;
using Grand.Plugin.Payments.PaymentOptions.Models;
using Grand.Plugin.Payments.PaymentOptions.Utilities;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Messages;
using Grand.Services.Orders;
using Grand.Services.Points;
using Grand.Services.Stores;
using Grand.Web.Controllers;
using Grand.Web.Extensions;
using IPara.DeveloperPortal.Core;
using IPara.DeveloperPortal.Core.Entity;
using IPara.DeveloperPortal.Core.Request;
using IPara.DeveloperPortal.Core.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Grand.Plugin.Payments.PaymentOptions.Controllers
{
    public class PaymentOptionsController : BasePublicController
    {
        #region Fields
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IRewardPointsService _rewardPointsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly OrderSettings _orderSettings;
        private readonly IPdfService _pdfService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IProcessService _processService;
        #endregion

        #region Constructors

        public PaymentOptionsController(IOrderService orderService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductService productService,
            ILocalizationService localizationService,
            LocalizationSettings localizationSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreService storeService,
            IRewardPointsService rewardPointsService,
            IHttpContextAccessor httpContextAccessor,
            IWebHelper webHelper,
            IOrderPaymentService orderPaymentService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            OrderSettings orderSettings,
            IPdfService pdfService,
            IOrderProcessingService orderProcessingService,
            IProcessService processService, ICategoryService categoryService)
        {
            this._orderService = orderService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._productService = productService;
            this._localizationService = localizationService;
            this._localizationSettings = localizationSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeService = storeService;
            this._rewardPointsService = rewardPointsService;
            this._httpContextAccessor = httpContextAccessor;
            this._webHelper = webHelper;
            this._orderPaymentService = orderPaymentService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._workflowMessageService = workflowMessageService;
            this._orderSettings = orderSettings;
            this._pdfService = pdfService;
            this._orderProcessingService = orderProcessingService;
            this._processService = processService;
            this._categoryService = categoryService;

        }

        #endregion

        #region Methods

        public virtual IActionResult PaymentOptions()
        {
            PaymentOptionModel model = new PaymentOptionModel();

            //Kullanıcının seçtiği ödeyeceği miktar getiriliyor...
            decimal selectedPointPaidVal = decimal.Parse(_workContext.CurrentCustomer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));

            //Kaydedilen order getiriliyor...
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();

            if (order != null)
            {
                Core.Domain.Customers.Customer customer = _customerService.GetCustomerById(order.CustomerId);

                order.PaymentStatus = PaymentStatus.Pending;
                order.OrderStatus = OrderStatus.Pending;
                order.AuthorizationTransactionId = order.OrderGuid.ToString();
                _orderService.UpdateOrder(order);
                //Puan ile ödeme sistemi aktif mi
                Setting _havePuanPaymentSett = _settingService.GetSetting("paymentoptionsettings.puanactive");
                bool _havePuanPayment = Convert.ToBoolean(_havePuanPaymentSett.Value);
                if (_havePuanPayment)
                {
                    //Güncel harcanabilir puanı
                    decimal availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

                    //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
                    //decimal debitPointValue = (availeblePoint - decimal.Parse(MoneyToPoint.CalculatePoint((int)order.OrderTotal)));

                    decimal debitPointValue = (availeblePoint - order.OrderTotal);
                    string orderId = "";
                    if (debitPointValue >= 0)
                    {
                        Process _proc = new Process()
                        {
                            Description = order.OrderNumber.ToString() + " numaralı sipariş için düşülen puan.",
                            OrderGuid = order.OrderGuid,
                            OrderNumber = order.OrderNumber,
                            CreatedOnUtc = DateTime.UtcNow,
                            Point = (selectedPointPaidVal * -1),
                            TypeId = (int)ProcessType.Spend,
                            Username = _workContext.CurrentCustomer.Username,
                        };
                        orderId = _processService.Insert(_proc).Id;
                        if (!string.IsNullOrEmpty(orderId))
                        {
                            order.PaymentStatus = PaymentStatus.Paid;
                            order.OrderStatus = OrderStatus.Processing;
                            order.AuthorizationTransactionId = order.OrderGuid.ToString();
                            _orderService.UpdateOrder(order);

                            //Set new UserPoint
                            var pointBalanceTotal = _processService.GetCustomerActualPoint(customer.Username);
                            if (pointBalanceTotal >= 0)
                                _rewardPointsService.RemoveOldPointAndAddNewPoints(_workContext.CurrentCustomer.Id, pointBalanceTotal, _storeContext.CurrentStore.Id, string.Empty);

                            _orderService.InsertOrderNote(new OrderNote
                            {
                                Note = order.OrderNumber.ToString() + " numaralı siparişin tamamı " + selectedPointPaidVal.ToString() + " puan ile ödendi",
                                DisplayToCustomer = false,
                                CreatedOnUtc = DateTime.UtcNow,
                                OrderId = order.Id,
                            });

                            SendEmailNotification(order);

                            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                        }
                        else
                        {
                            _orderService.InsertOrderNote(new OrderNote
                            {
                                Note = order.OrderNumber.ToString() + " numaralı siparişin puan bakiyesi yetersizliğinden" + selectedPointPaidVal.ToString() + " puanı düşülemedi!",
                                DisplayToCustomer = false,
                                CreatedOnUtc = DateTime.UtcNow,
                                OrderId = order.Id,
                            });
                            ErrorNotification(JsonConvert.SerializeObject(order.OrderNumber.ToString() + " numaralı siparişin puan bakiyesi yetersizliğinden puanı düşülemedi!"));
                            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                        }
                    }
                    else
                    {
                        // puan yetersizse, 
                        decimal debitMoneyValue = 0;
                        var setting = _settingService.GetSetting("payment.moneypointfactor");
                        if (selectedPointPaidVal == 0)
                        {
                            debitMoneyValue = (int)order.OrderTotal / decimal.Parse(setting.Value);
                            model.TotalDebit = debitMoneyValue;
                        }
                        else
                        {
                            model.TotalDebit = ((int)debitPointValue * -1) / decimal.Parse(setting.Value);
                        } 
                    }
                }
                else
                {
                    model.TotalDebit = (int)order.OrderTotal;
                }
            }

            var thisUserRole = _workContext.CurrentCustomer.CustomerRoles.Where(x => x.SystemName == "GenelMerkez").FirstOrDefault();
            if (thisUserRole != null)
                model.IsRoleGenelMudurluk = true;

            //Havale İndirim Oranının Settingsden getirilmesi
            Setting _havaleDiscountRate = _settingService.GetSetting("paymentoptionsettings.havalediscountrate");
            if (_havaleDiscountRate != null)
                model.HavaleDiscountRate = decimal.Parse(_havaleDiscountRate.Value);
            else
                model.HavaleDiscountRate = 0;

            //Banka Bilgilerinin Settingsden getirilmesi
            Setting _bankInfo = _settingService.GetSetting("paymentoptionsettings.BankInfo");
            if (_bankInfo != null)
                model.BanksInfo = _bankInfo.Value;

            model.OrderNumber = order.OrderNumber.ToString();
            //model.OrderGuide = order.OrderGuid.ToString();
            return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
        }


        #region 3D'siz ödeme

        [HttpPost]
        public virtual IActionResult PaymentOptions(PaymentOptionModel model)
        {
            ApiPaymentResponse response = GetPamentWithIPara(model);
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();

            if (response.Result == "1")
            {
                if (order != null)
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                    order.OrderStatus = OrderStatus.Processing;
                    order.AuthorizationTransactionId = response.ResponseMessage;
                    _orderService.UpdateOrder(order);

                    _orderService.InsertOrderNote(new OrderNote
                    {
                        Note = order.OrderNumber.ToString() + " numaralı sipariş " + response.ResponseMessage + " bilgisi ile alındı...",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                    });

                    SendEmailNotification(order);
                }
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            else
            {
                CancelTransaction(order);

                model.PaymentStatus = (int)PaymentStatusTypeEnum.Odeme_Basarisiz;
                model.Msg += "Ödeme Başarısız : <strong>" + response.ErrorMessage + "</strong><br/>";

                //ErrorNotification(JsonConvert.SerializeObject(model.Msg));

                return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
            }
        }

        private ApiPaymentResponse GetPamentWithIPara(PaymentOptionModel model)
        {
            Settings iParaSet = new Settings();

            iParaSet.BaseUrl = "https://api.ipara.com/";
            iParaSet.Mode = "T";
            iParaSet.PrivateKey = _settingService.GetSetting("paymentoption.iparasettings.privatekey").Value;
            iParaSet.PublicKey = _settingService.GetSetting("paymentoption.iparasettings.publickey").Value;
            iParaSet.Version = "1.0";
            iParaSet.TransactionDate = DateTime.Now.ToString();

            #region Request New

            var request = new ApiPaymentRequest();

            request.OrderId = model.OrderNumber;
            request.Echo = "Echo";
            request.Mode = "T";
            request.Amount = String.Format("{0:n}", Math.Round(model.TotalDebit, 0)).Replace(",", "."); //10000// 100.00 tL
            request.CardOwnerName = model.CardNameSurname;
            request.CardNumber = model.CardNumber;
            request.CardExpireMonth = model.CardMonth.ToString();
            request.CardExpireYear = model.CardYear.ToString();
            request.Installment = "1";
            request.Cvc = model.CardCvv.ToString();
            request.ThreeD = "false";
            request.CardId = "";
            request.UserId = "";

            #endregion

            #region Sipariş veren bilgileri
            request.Purchaser = new Purchaser();
            request.Purchaser.Name = "Murat";
            request.Purchaser.SurName = "Kaya";
            request.Purchaser.BirthDate = "1986-07-11";
            request.Purchaser.Email = "murat@kaya.com";
            request.Purchaser.GsmPhone = "5881231212";
            request.Purchaser.IdentityNumber = "1234567890";
            request.Purchaser.ClientIp = "127.0.0.1";
            #endregion

            #region Fatura bilgileri

            request.Purchaser.InvoiceAddress = new PurchaserAddress();
            request.Purchaser.InvoiceAddress.Name = "Murat";
            request.Purchaser.InvoiceAddress.SurName = "Kaya";
            request.Purchaser.InvoiceAddress.Address = "Mevlüt Pehlivan Mah. Multinet Plaza Şişli";
            request.Purchaser.InvoiceAddress.ZipCode = "34782";
            request.Purchaser.InvoiceAddress.CityCode = "34";
            request.Purchaser.InvoiceAddress.IdentityNumber = "1234567890";
            request.Purchaser.InvoiceAddress.CountryCode = "TR";
            request.Purchaser.InvoiceAddress.TaxNumber = "123456";
            request.Purchaser.InvoiceAddress.TaxOffice = "Kozyatağı";
            request.Purchaser.InvoiceAddress.CompanyName = "iPara";
            request.Purchaser.InvoiceAddress.PhoneNumber = "2122222222";

            #endregion

            #region Kargo Adresi bilgileri

            request.Purchaser.ShippingAddress = new PurchaserAddress();
            request.Purchaser.ShippingAddress.Name = "Murat";
            request.Purchaser.ShippingAddress.SurName = "Kaya";
            request.Purchaser.ShippingAddress.Address = "Mevlüt Pehlivan Mah. Multinet Plaza Şişli";
            request.Purchaser.ShippingAddress.ZipCode = "34782";
            request.Purchaser.ShippingAddress.CityCode = "34";
            request.Purchaser.ShippingAddress.IdentityNumber = "1234567890";
            request.Purchaser.ShippingAddress.CountryCode = "TR";
            request.Purchaser.ShippingAddress.PhoneNumber = "2122222222";

            #endregion

            #region Ürün bilgileri

            request.Products = new List<Product>();
            Product p = new Product();
            p.Title = "Telefon";
            p.Code = "TLF0001";
            p.Price = "5000"; //50.00 TL 
            p.Quantity = 1;
            request.Products.Add(p);
            p = new Product();
            p.Title = "Bilgisayar";
            p.Code = "BLG0001";
            p.Price = "5000"; //50.00 TL 
            p.Quantity = 1;
            request.Products.Add(p);

            ApiPaymentResponse response = ApiPaymentRequest.Execute(request, iParaSet);
            return response;

            #endregion
        }

        #endregion


        [HttpPost]
        public virtual IActionResult ThreeDPayment(PaymentOptionModel model)
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            if (order != null)
            {
                decimal selectedPointPaidVal = decimal.Parse(_workContext.CurrentCustomer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));

                Core.Domain.Customers.Customer customer = _customerService.GetCustomerById(order.CustomerId);

                decimal availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

                //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
                decimal debitPointValue = (availeblePoint - (int)order.OrderTotal);

                var setting = _settingService.GetSetting("payment.moneypointfactor");
                decimal debitMoneyValue = ((debitPointValue * -1) / decimal.Parse(setting.Value));

                Settings iParaSet = new Settings();
                iParaSet.BaseUrl = "https://api.ipara.com/";
                iParaSet.Mode = "P";
                iParaSet.PrivateKey = _settingService.GetSetting("paymentoption.iparasettings.privatekey").Value;
                iParaSet.PublicKey = _settingService.GetSetting("paymentoption.iparasettings.publickey").Value;
                iParaSet.ThreeDInquiryUrl = "https://www.ipara.com/3dgate";
                iParaSet.HashString = string.Empty;
                iParaSet.Version = "1.0";
                iParaSet.TransactionDate = DateTime.Now.ToString();

                var request = new ThreeDPaymentInitRequest();
                request.OrderId = model.OrderNumber.ToString();
                request.Echo = "Echo";
                request.Mode = iParaSet.Mode;
                request.Version = iParaSet.Version;
                request.Amount = debitMoneyValue.ToString("F").Replace(",", "").Replace(".", ""); //String.Format("{0:n}", Math.Round(model.TotalDebit, 0)).Replace(",00", "").Replace(",", "").Replace(".", ""); // 100 tL
                request.CardOwnerName = model.CardNameSurname;
                request.CardNumber = model.CardNumber;
                request.CardExpireMonth = model.CardMonth;
                request.CardExpireYear = model.CardYear;
                request.Installment = "1";
                request.Cvc = model.CardCvv;

                request.PurchaserName = order.ShippingAddress.FirstName;
                request.PurchaserSurname = order.ShippingAddress.LastName;
                request.PurchaserEmail = order.CustomerEmail;

                string cardInfo = model.CardNameSurname + "_" + model.CardNumber + "_" + model.CardMonth + "_" + model.CardYear + "_" + model.CardCvv;
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "cardInfo", cardInfo,
                    _storeContext.CurrentStore.Id);

                //Ssl olsun mu
                var storeLocation = _webHelper.GetStoreLocation(true);

                request.SuccessUrl = $"{storeLocation}Plugins/PaymentOptions/IParaSuccess";
                request.FailUrl = $"{storeLocation}Plugins/PaymentOptions/CancelOrder";
                //request.SuccessUrl = "http://localhost:16592/Plugins/PaymentOptions/IParaSuccess";
                //request.FailUrl = "http://localhost:16592/Plugins/PaymentOptions/CancelOrder";

                var form = ThreeDPaymentInitRequest.Execute(request, iParaSet);
                ViewBag.Source = form;

                _orderService.InsertOrderNote(new OrderNote
                {
                    Note = order.OrderNumber.ToString() + " numaralı sipariş, " + model.TotalDebit.ToString() + " tutarı için bankaya yönlendirildi.",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                });

                return View("~/Plugins/Payments.PaymentOptions/Views/ThreeDPayment.cshtml");
                //}
                //else
                //{
                //    _orderService.InsertOrderNote(new OrderNote
                //    {
                //        Note = order.OrderNumber.ToString() + " numaralı sipariş, " + selectedPointPaidVal.ToString() + " puanı ile alınamadı : " + _localizationService.GetResource("rewardpoints.insufficient.points"),
                //        DisplayToCustomer = false,
                //        CreatedOnUtc = DateTime.UtcNow,
                //        OrderId = order.Id,
                //    });

                //    ErrorNotification(JsonConvert.SerializeObject(_localizationService.GetResource("rewardpoints.insufficient.points")));
                //    return RedirectToRoute("Plugin.Payments.PaymentOptions");
                //}

            }
            return RedirectToRoute("HomePage");
        }



        [HttpPost]
        public virtual IActionResult HavalePayment(PaymentOptionModel model)
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
             customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            if (order != null)
            {
                //Havale İndirim Oranının Settingsden getirilmesi
                decimal havaleDiscountRate = 0;
                Setting _havaleDiscountRateSett = _settingService.GetSetting("paymentoptionsettings.havalediscountrate");
                if (_havaleDiscountRateSett != null)
                    havaleDiscountRate = decimal.Parse(_havaleDiscountRateSett.Value);

                decimal tax = 0;
                foreach (var item in order.OrderItems)
                {
                    item.PriceExclTax = item.PriceExclTax - ((item.PriceExclTax * havaleDiscountRate) / 100);
                    item.PriceInclTax = item.PriceInclTax - ((item.PriceInclTax * havaleDiscountRate) / 100);
                    item.UnitPriceExclTax = item.UnitPriceExclTax - ((item.UnitPriceExclTax * havaleDiscountRate) / 100);
                    item.UnitPriceInclTax = item.UnitPriceInclTax - ((item.UnitPriceInclTax * havaleDiscountRate) / 100);
                    tax += item.PriceInclTax - item.PriceExclTax;
                }

                //Total fiyata indirim uygulanması
                order.OrderTotal = (int)order.OrderTotal - (((int)order.OrderTotal * havaleDiscountRate) / 100);

                //KDV rakamı
                order.OrderTax = tax;

                order.PaymentMethodSystemName = "Payments.PaymentOptions.Havale";
                order.PaymentStatus = PaymentStatus.Pending;
                order.OrderStatus = OrderStatus.Pending;
                order.AuthorizationTransactionId = "Havale seçildi.";
                _orderService.UpdateOrder(order);

                _orderService.InsertOrderNote(new OrderNote
                {
                    Note = order.OrderNumber.ToString() + " numaralı sipariş havale ile alındı...",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                });

                SendEmailNotification(order);
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            return RedirectToRoute("HomePage");
        }

        [CheckAccessPublicStore(true)]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public IActionResult CancelOrder(IFormCollection form)
        {
            string returnRouteName = "";
            if (form != null)
            {
                int pageType = 0;//1 = 3Dden gelen, 2 = Sayfa iptalinden Gelen
                string[] orderNumber = new string[2];
                if (!string.IsNullOrEmpty(form["errorMessage"]))
                {
                    orderNumber = form["orderId"].ToString().Split("_");
                    pageType = 1;
                }
                else
                {
                    string[] tempGuide = form["OrderNumber"].ToString().Split("_");
                    orderNumber[0] = tempGuide[0];
                    pageType = 2;
                }

                Order order = _orderService.GetOrderByNumber(int.Parse(orderNumber[0]));
                if (order != null)
                {
                    if (pageType == 1)
                    {
                        Core.Domain.Customers.Customer customer = _customerService.GetCustomerById(order.CustomerId);

                        decimal selectedPointPaidVal = decimal.Parse(customer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));

                        decimal availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

                        //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
                        decimal debitPointValue = (availeblePoint - (int)order.OrderTotal);

                        var setting = _settingService.GetSetting("payment.moneypointfactor");
                        decimal debitMoneyValue = ((debitPointValue * -1) / decimal.Parse(setting.Value));


                        _orderService.InsertOrderNote(new OrderNote
                        {
                            Note = debitMoneyValue.ToString() + " TL için alınan hata bilgisi : " + form["errorMessage"].ToString().Replace("\"", "").Replace("[\"", "").Replace("\"]", ""),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow,
                            OrderId = order.Id,
                        });

                        ErrorNotification(JsonConvert.SerializeObject(form["errorMessage"]).ToString().Replace("\"", "").Replace("[\"", "").Replace("\"]", ""));

                        PaymentOptionModel model = new PaymentOptionModel();
                        model.TotalDebit = debitMoneyValue;
                        model.OrderNumber = order.OrderNumber.ToString() + "_" + DateTime.Now.Millisecond.ToString();

                        return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
                    }
                    else if (pageType == 2)
                    {
                        //_orderService.InsertOrderNote(new OrderNote
                        //{
                        //    Note = order.OrderNumber.ToString() + " numaralı sipariş kullanıcı tarafından iptal edildi.",
                        //    DisplayToCustomer = false,
                        //    CreatedOnUtc = DateTime.UtcNow,
                        //    OrderId = order.Id,
                        //});

                        //order.PaymentStatus = PaymentStatus.Voided;
                        //order.OrderStatus = OrderStatus.Cancelled;
                        //order.AuthorizationTransactionId = order.OrderGuid.ToString();
                        //_orderService.UpdateOrder(order);

                        _orderProcessingService.CancelOrder(order, false);

                        returnRouteName = "HomePage";
                        return RedirectToRoute("HomePage");
                    }
                }
            }
            return RedirectToRoute(returnRouteName);
        }


        [CheckAccessPublicStore(true)]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public IActionResult IParaSuccess(IFormCollection form)
        {
            Settings iParaSet = new Settings();
            iParaSet.BaseUrl = "https://api.ipara.com/";
            iParaSet.Mode = "P";
            iParaSet.PrivateKey = _settingService.GetSetting("paymentoption.iparasettings.privatekey").Value;
            iParaSet.PublicKey = _settingService.GetSetting("paymentoption.iparasettings.publickey").Value;
            iParaSet.ThreeDInquiryUrl = "https://www.ipara.com/3dgate";
            iParaSet.HashString = string.Empty;
            iParaSet.Version = "1.0";
            iParaSet.TransactionDate = form["transactionDate"];

            string[] orderNumber= form["orderId"].ToString().Split("_");
            Order order = _orderService.GetOrderByNumber(int.Parse(orderNumber[0]));

            string carInfo = string.Empty;
            decimal selectedPointPaidVal = 0;
            Core.Domain.Customers.Customer customer = _customerService.GetCustomerById(order.CustomerId);
            if (customer != null)
            {
                selectedPointPaidVal = decimal.Parse(customer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));
                carInfo = customer.GenericAttributes.Where(x => x.Key == "cardInfo").FirstOrDefault().Value;
            }

            string[] cardInfoItems = carInfo.Split("_");

            decimal availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

            //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
            decimal debitPointValue = (availeblePoint - (int)order.OrderTotal);

            var setting = _settingService.GetSetting("payment.moneypointfactor");
            decimal debitMoneyValue = ((debitPointValue * -1) / decimal.Parse(setting.Value));

            ThreeDPaymentInitResponse paymentResponse = new ThreeDPaymentInitResponse();
            paymentResponse.OrderId = form["orderId"];
            paymentResponse.Result = form["result"];
            paymentResponse.Amount = form["amount"];
            paymentResponse.Mode = form["mode"];
            if (!string.IsNullOrEmpty(form["errorCode"]))
                paymentResponse.ErrorCode = form["errorCode"];

            if (!string.IsNullOrEmpty(form["errorMessage"]))
                paymentResponse.ErrorMessage = form["errorMessage"];

            if (!string.IsNullOrEmpty(form["transactionDate"]))
                paymentResponse.TransactionDate = form["transactionDate"];

            if (!string.IsNullOrEmpty(form["hash"]))
                paymentResponse.Hash = form["hash"];

            if (Helper.Validate3DReturn(paymentResponse, iParaSet))
            {
                var request = new ThreeDPaymentCompleteRequest();

                #region Request New
                request.OrderId = form["orderId"];
                request.Echo = "Echo";
                request.Mode = "P";
                request.Amount = form["amount"]; // 100 tL
                request.CardOwnerName = cardInfoItems[0];
                request.CardNumber = cardInfoItems[1];
                request.CardExpireMonth = cardInfoItems[2];
                request.CardExpireYear = cardInfoItems[3];
                request.Installment = "1";
                request.Cvc = cardInfoItems[4];
                request.ThreeD = "true";
                request.ThreeDSecureCode = form["threeDSecureCode"];
                #endregion

                #region Sipariş veren bilgileri
                request.Purchaser = new Purchaser();
                request.Purchaser.Name = order.ShippingAddress.FirstName;
                request.Purchaser.SurName = order.ShippingAddress.LastName;
                request.Purchaser.Email = order.CustomerEmail;
                #endregion

                request.Products = new List<Product>();
                foreach (var item in order.OrderItems)
                {
                    var prdk = _productService.GetProductById(item.ProductId);

                    Product p = new Product();
                    p.Title = prdk.Name;
                    p.Code = prdk.Sku;
                    p.Price = item.PriceInclTax.ToString().Replace(",", "").Replace(".", "");
                    p.Quantity = item.Quantity;
                    request.Products.Add(p);
                }

                var response = ThreeDPaymentCompleteRequest.Execute(request, iParaSet);
                if (response.Result == "1")
                {
                    string logMsg = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                    order.PaymentStatus = PaymentStatus.Paid;
                    order.OrderStatus = OrderStatus.Processing;
                    order.AuthorizationTransactionId = form["result"];
                    _orderService.UpdateOrder(order);

                    //YENİ STOK KONTROLÜ
                    foreach (var sc in order.OrderItems)
                    {
                        var product = _productService.GetProductById(sc.ProductId);
                        StockCheckHelper.DecreasePIMStock(product, sc.Quantity, sc.AttributesXml);
                    }

                    _orderService.InsertOrderNote(new OrderNote
                    {
                        Note = debitMoneyValue.ToString() + " TL 3D ile alındı...",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                    });

                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Information, logMsg);

                    //Puanın düşülmesi
                    string orderId = "";
                    Process _proc = new Process()
                    {
                        Description = order.OrderNumber.ToString() + " numaralı sipariş için düşülen puan.",
                        OrderGuid = order.OrderGuid,
                        OrderNumber = order.OrderNumber,
                        CreatedOnUtc = DateTime.Now,
                        Point = selectedPointPaidVal * -1,
                        TypeId = (int)ProcessType.Spend,
                        Username = customer.Username,
                    };
                    orderId = _processService.Insert(_proc).Id;

                    if (!string.IsNullOrEmpty(orderId))
                    {
                        _orderService.InsertOrderNote(new OrderNote
                        {
                            Note = selectedPointPaidVal.ToString() + " puan düşüldü.",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow,
                            OrderId = order.Id,
                        });
                    }
                    else
                    {
                        _orderService.InsertOrderNote(new OrderNote
                        {
                            Note = selectedPointPaidVal.ToString() + " puan düşülürken hata oluştu!",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow,
                            OrderId = order.Id,
                        });
                    }

                    //Karttan çekilen tutarın oran tutarının hesaplaması
                    List<OrderPayments> ordPayList = new List<OrderPayments>();
                    foreach (var item in order.OrderItems)
                    {
                        OrderPayments ordPayment = new OrderPayments();
                        ordPayment.CreatedOnUtc = DateTime.Now;
                        ordPayment.OrderId = order.Id;
                        ordPayment.OrderItemId = item.Id;
                        ordPayment.Amount = (debitMoneyValue * item.PriceInclTax) / (int)order.OrderTotal;
                        ordPayment.TotalAmount = debitMoneyValue;
                        ordPayList.Add(ordPayment);
                    }

                    //Karttan çeliken tutarda kargo tutarının oran hesaplaması
                    OrderPayments ordPaymentKB = new OrderPayments();
                    ordPaymentKB.CreatedOnUtc = DateTime.Now;
                    ordPaymentKB.OrderId = order.Id;
                    ordPaymentKB.OrderItemId = "KB";
                    ordPaymentKB.Amount = (debitMoneyValue * order.OrderShippingInclTax) / (int)order.OrderTotal;
                    ordPaymentKB.TotalAmount = debitMoneyValue;

                    ordPayList.Add(ordPaymentKB);
                    _orderPaymentService.Insert(ordPayList);

                    SendEmailNotification(order);

                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                else
                {
                    ErrorNotification(JsonConvert.SerializeObject(response.ErrorMessage));

                    _orderService.InsertOrderNote(new OrderNote
                    {
                        Note = "Banka ödemesi başarısız döndü : " + response.ResponseMessage,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                    });

                    PaymentOptionModel model = new PaymentOptionModel();
                    model.TotalDebit = debitMoneyValue;
                    model.OrderNumber = order.OrderNumber.ToString() + "_" + DateTime.Now.Millisecond.ToString();

                    return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
                    //return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                }
                //return RedirectToRoute("HomePage");
            }
            else
            {
                ErrorNotification(JsonConvert.SerializeObject(form["errorMessage"]));

                _orderService.InsertOrderNote(new OrderNote
                {
                    Note = order.OrderNumber.ToString() + " numaralı siparişin girilen kart bilgileri hash ile doğurlanamadı : " + JsonConvert.SerializeObject(form["errorMessage"]),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                });

                PaymentOptionModel model = new PaymentOptionModel();
                model.TotalDebit = debitMoneyValue;
                model.OrderNumber = order.OrderNumber.ToString() + "_" + DateTime.Now.Millisecond.ToString();

                return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
            }
        }


        public void SendEmailNotification(Order order)
        {
            //if (order.OrderStatus == OrderStatus.Processing)
            //{
            var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                    _pdfService.PrintOrderToPdf(order, order.CustomerLanguageId) : null;
            var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                "order.pdf" : null;

            //Mesafeli satış sözleşmesi emaili
            int orderPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
            .SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
            if (orderPlacedCustomerNotificationQueuedEmailId > 0)
            {
                _orderService.InsertOrderNote(new OrderNote
                {
                    Note = "\"Mesafeil Satış Sözleşmesi\" mail gönderme kuyruğuna eklenildi.",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,

                });
            }

            //Ön bilgilendirme metni emaili
            int sendContractOnBilgilendirmeFormuNotificationQueuedEmailId = _workflowMessageService
                .SendContractOnBilgilendirmeFormuNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
            if (sendContractOnBilgilendirmeFormuNotificationQueuedEmailId > 0)
            {
                _orderService.InsertOrderNote(new OrderNote
                {
                    Note = "\"Ön Bilgilendirme Formu\" mail gönderme kuyruğuna eklenildi.",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,

                });
            }
            //}
        }

        public bool CancelTransaction(Order order)
        {
            bool isSuccess = false;
            if (order != null)
            {
                try
                {
                    decimal selectedMoneyPaidVal = 0;
                    Core.Domain.Customers.Customer customer = _customerService.GetCustomerById(order.CustomerId);
                    selectedMoneyPaidVal = decimal.Parse(customer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));

                    int result = CancelOrderRefoundPoint(order.OrderGuid.ToString(), selectedMoneyPaidVal);
                    if (result == 0)
                    {
                        isSuccess = true;
                        var pointBalanceTotal = _rewardPointsService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                        if (pointBalanceTotal > 0)
                            _rewardPointsService.RemoveOldPointAndAddNewPoints(_workContext.CurrentCustomer.Id, (pointBalanceTotal + selectedMoneyPaidVal), _storeContext.CurrentStore.Id, string.Empty);

                        _orderService.InsertOrderNote(new OrderNote
                        {
                            Note = selectedMoneyPaidVal.ToString() + " puan iade edildi!",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow,
                            OrderId = order.Id,

                        });
                    }
                }
                catch (Exception ex)
                {
                    _orderService.InsertOrderNote(new OrderNote
                    {
                        Note = "Puan geri yatırılırken bir hata oluştu: " + ex.Message,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,

                    });
                }
            }
            else
            {
                _orderService.InsertOrderNote(new OrderNote
                {
                    Note = "Sipariş bulunamadığından, puan iade edilemedi!",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,

                });
            }
            return isSuccess;
        }


        private int CancelOrderRefoundPoint(string orderGuid, decimal rePoint)
        {
            int result = 0;
            int projectDBNumber = int.Parse(_settingService.GetSetting("rewardoperationssettings.projectdbnumber").Value);
            try
            {
                using (SqlConnection con = new SqlConnection("data source=10.0.0.200;initial catalog=RPPool;persist security info=True;user id=erhasrppool2;password=ERHASrpPool415263;MultipleActiveResultSets=True"))
                {
                    using (SqlCommand cmd = new SqlCommand(string.Format("SP_{0}_CancelFullOrder", projectDBNumber), con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@OrderGuid", SqlDbType.NVarChar).Value = orderGuid;
                        cmd.Parameters.AddWithValue("@OrderPoint", SqlDbType.Decimal).Value = rePoint;

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            result = int.Parse(reader["ErrorCode"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }

            return result;
        }

        #endregion
    }
}