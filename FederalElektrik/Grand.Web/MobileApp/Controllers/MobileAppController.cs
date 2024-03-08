//using Grand.Core;
//using Grand.Core.Caching;
//using Grand.Core.Domain.Catalog;
//using Grand.Core.Domain.Customers;
//using Grand.Framework.Security.Captcha;
//using Grand.Services.Authentication;
//using Grand.Services.Catalog;
//using Grand.Services.Common;
//using Grand.Services.Configuration;
//using Grand.Services.Customers;
//using Grand.Services.Events;
//using Grand.Services.Localization;
//using Grand.Services.Logging;
//using Grand.Services.Media;
//using Grand.Services.Orders;
//using Grand.Web.MobileApp.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.AspNetCore.Hosting;
//using Grand.Services.Security;
//using Grand.Web.Services;
//using Grand.Core.Domain.Orders;
//using Grand.Web.Models.ShoppingCart;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Grand.Web.Models.Customer;
//using Grand.Services.Directory;
//using Grand.Core.Domain.Common;
//using Grand.Services.Shipping;
//using Grand.Services.Payments;
//using Grand.Core.Domain.Shipping;
//using Grand.Core.Domain.Payments;
//using Grand.Services.Tax;
//using Grand.Services.Points;
//using Grand.Core.Domain.Points;
//using Grand.Core.Domain.Directory;
//using Grand.Web.Models.Media;
//using Grand.Core.Domain.Media;
//using Grand.Web.MobileApp.Filters;
//using Grand.Services.Messages;
//using Grand.Core.Domain.Messages;
//using Grand.Services.Stores;
//using Grand.Core.Domain.Stores;
//using Grand.Services.Discounts;
//using Grand.Core.Domain.Localization;
//using Grand.Core.Http;
//using IPara.DeveloperPortal.Core;
//using IPara.DeveloperPortal.Core.Request;
//using Grand.Web.Extensions;
//using Microsoft.AspNetCore.Http;
//using IPara.DeveloperPortal.Core.Response;
//using IPara.DeveloperPortal.Core.Entity;
//using Newtonsoft.Json;

//namespace Grand.Web.Controllers
//{
//    [Route("app")]
//    [AllowAnonymous]
//    [JwtTokenValidateFilter]
//    [CustomExceptionHandler]
//    public class MobileAppController : Controller
//    {
//        #region CTOR
//        private readonly IWebHelper _webHelper;
//        private readonly IRewardPointsService _rewardPointsService;
//        private readonly IStoreContext _storeContext;
//        private readonly IWorkContext _workContext;
//        private readonly IPdfService _pdfService;
//        private readonly IWorkflowMessageService _workflowMessageService;
//        private readonly IStoreService _storeService;
//        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
//        private readonly IMessageTemplateService _messageTemplateService;
//        private readonly IEmailAccountService _emailAccountService;
//        private readonly IQueuedEmailService _queuedEmailService;
//        private readonly IShipmentService _shipmentService;
//        private readonly ITaxService _taxService;
//        private readonly IPriceCalculationService _priceCalculationService;
//        private readonly ICurrencyService _currencyService;
//        private readonly IShoppingCartWebService _shoppingCartWebService;
//        private readonly ICustomerWebService _customerWebService;
//        private readonly IGenericAttributeService _genericAttributeService;
//        private readonly IAddressWebService _addressWebService;
//        private readonly ICountryService _countryService;
//        private readonly IPriceFormatter _priceFormatter;
//        private readonly IStateProvinceService _stateProvinceService;
//        private readonly ICheckoutWebService _checkoutWebService;
//        private readonly IShippingService _shippingService;
//        private readonly IPaymentService _paymentService;
//        private readonly ITaxCategoryService _taxCategoryService;
//        private readonly IProcessService _processService;
//        private readonly ILanguageService _languageService;
//        private readonly IAclService _aclService;
//        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
//        private readonly IProductWebService _productWebService;
//        private readonly IPictureService _pictureService;
//        private readonly ISettingService _settingService;
//        private readonly IProductService _productService;
//        private readonly ICategoryService _categoryService;
//        private readonly IOrderService _orderService;
//        private readonly IAddressService _addressService;
//        private readonly ICustomerRegistrationService _customerRegistrationService;
//        private readonly IShoppingCartService _shoppingCartService;
//        private readonly ICustomerService _customerService;
//        private readonly IGrandAuthenticationService _authenticationService;
//        private readonly IEventPublisher _eventPublisher;
//        private readonly ICustomerActivityService _customerActivityService;
//        private readonly ILocalizationService _localizationService;
//        private readonly ICacheManager _cacheManager;
//        private readonly IConfiguration _configuration;
//        private readonly IHostingEnvironment _hostingEnvironment;
//        private readonly IOrderPaymentService _orderPaymentService;
//        private readonly IOrderProcessingService _orderProcessingService;
//        private readonly MediaSettings _mediaSettings;
//        private readonly CaptchaSettings _captchaSettings;
//        private readonly CustomerSettings _customerSettings;
//        private readonly EmailAccountSettings _emailAccountSettings;
//        private readonly LocalizationSettings _localizationSettings;
//        private readonly OrderSettings _orderSettings;
//        public const string PICTURE_URL_MODEL_KEY = "Grand.plugins.widgets.mobilslider.pictureurl-{0}";
//        private const bool is3DPayActive = true;

//        public MobileAppController(IProductService productService, ICategoryService categoryService, IOrderService orderService, IAddressService addressService, ICustomerRegistrationService customerRegistrationService, IShoppingCartService shoppingCartService, ICustomerService customerService, IGrandAuthenticationService authenticationService, IEventPublisher eventPublisher, ICustomerActivityService customerActivityService, ILocalizationService localizationService, CaptchaSettings captchaSettings, CustomerSettings customerSettings, ISettingService settingService, ICacheManager cacheManager, IPictureService pictureService, IConfiguration configuration, IHostingEnvironment hostingEnvironment, IAclService aclService, IRecentlyViewedProductsService recentlyViewedProductsService, IProductWebService productWebService, IShoppingCartWebService shoppingCartWebService, ICustomerWebService customerWebService, IGenericAttributeService genericAttributeService, IAddressWebService addressWebService, ICountryService countryService, IPriceFormatter priceFormatter, IStateProvinceService stateProvinceService, ICheckoutWebService checkoutWebService, IShippingService shippingService, IPaymentService paymentService, ITaxCategoryService taxCategoryService, IProcessService processService, ILanguageService languageService, ITaxService taxService, IPriceCalculationService priceCalculationService, ICurrencyService currencyService, MediaSettings mediaSettings, IShipmentService shipmentService, IEmailAccountService emailAccountService, IQueuedEmailService queuedEmailService, EmailAccountSettings emailAccountSettings, IMessageTemplateService messageTemplateService, IStoreService storeService, IOrderTotalCalculationService orderTotalCalculationService, IWorkflowMessageService workflowMessageService, LocalizationSettings localizationSettings, IPdfService pdfService, OrderSettings orderSettings, IWorkContext workContext, IStoreContext storeContext, IRewardPointsService rewardPointsService, IWebHelper webHelper, IOrderPaymentService orderPaymentService, IOrderProcessingService orderProcessingService)
//        {
//            _productService = productService;
//            _categoryService = categoryService;
//            _orderService = orderService;
//            _addressService = addressService;
//            _customerRegistrationService = customerRegistrationService;
//            _shoppingCartService = shoppingCartService;
//            _customerService = customerService;
//            _authenticationService = authenticationService;
//            _eventPublisher = eventPublisher;
//            _customerActivityService = customerActivityService;
//            _localizationService = localizationService;
//            _captchaSettings = captchaSettings;
//            _customerSettings = customerSettings;
//            _settingService = settingService;
//            _cacheManager = cacheManager;
//            _pictureService = pictureService;
//            _hostingEnvironment = hostingEnvironment;
//            _aclService = aclService;
//            _recentlyViewedProductsService = recentlyViewedProductsService;
//            _productWebService = productWebService;
//            _shoppingCartWebService = shoppingCartWebService;
//            _customerWebService = customerWebService;
//            _genericAttributeService = genericAttributeService;
//            _addressWebService = addressWebService;
//            _countryService = countryService;
//            _priceFormatter = priceFormatter;
//            _stateProvinceService = stateProvinceService;
//            _checkoutWebService = checkoutWebService;
//            _shippingService = shippingService;
//            _paymentService = paymentService;
//            _taxCategoryService = taxCategoryService;
//            _processService = processService;
//            _languageService = languageService;
//            _taxService = taxService;
//            _priceCalculationService = priceCalculationService;
//            _currencyService = currencyService;
//            _mediaSettings = mediaSettings;
//            _shipmentService = shipmentService;
//            _emailAccountService = emailAccountService;
//            _queuedEmailService = queuedEmailService;
//            _emailAccountSettings = emailAccountSettings;
//            _messageTemplateService = messageTemplateService;
//            _storeService = storeService;
//            _configuration = new ConfigurationBuilder()
//                .SetBasePath(_hostingEnvironment.ContentRootPath)
//                .AddJsonFile("App_Data/appsettings.json", optional: false, reloadOnChange: true)
//                .AddEnvironmentVariables()
//                .Build();
//            _orderTotalCalculationService = orderTotalCalculationService;
//            _workflowMessageService = workflowMessageService;
//            _localizationSettings = localizationSettings;
//            _pdfService = pdfService;
//            _orderSettings = orderSettings;
//            _workContext = workContext;
//            _storeContext = storeContext;
//            _rewardPointsService = rewardPointsService;
//            _webHelper = webHelper;
//            _orderPaymentService = orderPaymentService;
//            _orderProcessingService = orderProcessingService;
//        }

//        #endregion

//        #region Utilities & Helpers

//        [NonAction]
//        public JsonResult Success() => Json(new SuccessResult());

//        [NonAction]
//        private IActionResult Success(object data) => Json(new SuccessDataResult<object>(data));

//        [NonAction]
//        public JsonResult Error(string message) => Json(new ErrorResult(message));

//        [NonAction]
//        public JsonResult Error(object data) => Json(new ErrorDataResult<object>(data));

//        [Route(nameof(Test))]
//        public IActionResult Test()
//        {
//            return Success("ok");
//        }

//        [Route(nameof(TestException))]
//        public IActionResult TestException()
//        {
//            throw new Exception("Email account can't be loaded");
//        }

//        [Route(nameof(Config))]
//        public IActionResult Config()
//        {
//            var settings = _settingService.GetAllSettings().Where(x => x.Name.ToLowerInvariant().Contains("mobileapp"))
//                .Select(x => new
//                {
//                    Name = x.Name.ToLower().Replace("mobileapp.", ""),
//                    Value = x.Value.ToLower().Replace("mobileapp.", ""),
//                }).ToList();
//            return Success(settings);
//        }

//        [NonAction]
//        private string GetUsername()
//        {
//            var key = _configuration["Jwt:key"];
//            var token = HttpContext.Request.Headers["token"];
//            if (string.IsNullOrEmpty(token))
//                return string.Empty;

//            return JwtParserService.GetUsername(token.ToString(), key);
//        }

//        [NonAction]
//        private Customer GetCustomer()
//        {
//            var customer = _customerService.GetCustomerByUsername(GetUsername());

//            if (customer == null)
//                throw new Exception("Customer not found");

//            return customer;
//        }

//        [NonAction]
//        private Store GetCurrentStore()
//        {
//            return _storeService.GetAllStores().First();
//        }

//        [NonAction]
//        private void SetContext()
//        {
//            _storeContext.CurrentStore = GetCurrentStore();
//            _workContext.CurrentCustomer = GetCustomer();
//        }


//        [NonAction]
//        private string GenerateJwtToken(string userName)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new[] { new Claim("id", userName) }),
//                Expires = DateTime.UtcNow.AddHours(1),
//                Issuer = _configuration["Jwt:Issuer"],
//                Audience = _configuration["Jwt:Audience"],
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }

//        [HttpGet(nameof(GetPictureUrl))]
//        [IngoreAllAttributes]
//        public string GetPictureUrl(string pictureId)
//        {

//            string cacheKey = string.Format(PICTURE_URL_MODEL_KEY, pictureId);
//            return _cacheManager.Get(cacheKey, () =>
//            {
//                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
//                if (url == null)
//                    url = "";

//                return url;
//            });
//        }

//        [NonAction]
//        private static Address GetAddressFromModel(AddressModel x, string customerId)
//        {
//            return new Address()
//            {
//                Address1 = x.Address,
//                CountryId = x.CityId,
//                StateProvinceId = x.DistrictId,
//                Email = x.Email,
//                FirstName = x.FirstName,
//                LastName = x.LastName,
//                PhoneNumber = x.PhoneNumber,
//                VatNumber = x.VatNumber,
//                ZipPostalCode = x.ZipPostalCode,
//                CustomerId = customerId,
//            };
//        }

//        [NonAction]
//        private bool CartAny(Customer customer = null)
//        {
//            if (customer == null)
//                customer = GetCustomer();

//            return customer.ShoppingCartItems.Any(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart);
//        }

//        [NonAction]
//        private string GetPointType(int value)
//        {
//            var pointType = ((ProcessType)value).ToString();

//            switch (pointType)
//            {
//                case "Earn":
//                    return "Puan Yükleme";
//                case "Spend":
//                    return "Puan Harcama";
//                case "Partial_Cancellation":
//                    return "Parçalı İade";
//                case "Cancel":
//                    return "Sipariş İade";
//                case "TransferSpend":
//                    return "Transfer Etme";
//                case "TransferEarn":
//                    return "Transfer Alma";
//                default:
//                    return string.Empty;
//            }
//        }

//        [NonAction]
//        private Currency GetWorkingCurrency()
//        {
//            var allStoreCurrencies = _currencyService.GetAllCurrencies(storeId: GetCurrentStore().Id);

//            return allStoreCurrencies.FirstOrDefault();
//        }

//        [NonAction]
//        private void ClearCart()
//        {
//            _shoppingCartService.ClearShoppingCartItems(GetCustomer(), GetCurrentStore().Id);
//        }

//        [NonAction]
//        private string GetLangId()
//        {
//            return _languageService.GetAllLanguages().FirstOrDefault(x => x.UniqueSeoCode.ToLower() == "tr")?.Id;
//        }

//        [NonAction]
//        private string GetTrackingUrl(string orderId)
//        {
//            var baseUrl = "https://www.yurticikargo.com/tr/online-servisler/gonderi-sorgula?code=";
//            var shipment = _shipmentService.GetShipmentsByOrder(orderId).FirstOrDefault();
//            return shipment != null ? baseUrl + shipment.TrackingNumber : null;
//        }

//        [NonAction]
//        private List<ShoppingCartItem> GetCart(Customer customer = null)
//        {
//            if (customer == null)
//                customer = GetCustomer();

//            return customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
//        }

//        [NonAction]
//        private decimal GetCartTotal()
//        {
//             _orderTotalCalculationService.GetShoppingCartSubTotal(GetCart(), true,
//                    out decimal orderSubTotalDiscountAmountBase, out List<AppliedDiscount> orderSubTotalAppliedDiscounts,
//                    out decimal subTotalWithoutDiscountBase, out decimal subTotalWithDiscountBase);
//            decimal subtotalBase = subTotalWithoutDiscountBase;
//            decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, GetWorkingCurrency());

//            return subtotal;
//        }

//        [NonAction]
//        private bool ValidatePoints()
//        {
//            var cartTotal = GetCartTotal();
//            var pointsTotal = _processService.GetCustomerActualPoint(GetUsername());

//            if (cartTotal == 0m || pointsTotal == 0m)
//                return false;

//            return cartTotal < pointsTotal;
//        }

//        [NonAction]
//        private void SpendPoint(Customer customer, Order order)
//        {
//            _processService.Insert(new Process()
//            {
//                OrderGuid = order.OrderGuid,
//                OrderNumber = order.OrderNumber,
//                Description = $"{order.OrderNumber} No'lu sipariş için düşülen tutar.",
//                Point = -order.OrderTotal,
//                Username = customer.Email,
//                TypeId = (int)ProcessType.Spend,
//                CreatedOnUtc = order.CreatedOnUtc,
//            });
//        }

//        [NonAction]
//        private OrderResponseModel SaveOrder(Customer customer, decimal paymentNeeded)
//        {
//            SetContext();

//            var cart = GetCart();
//            var order = new Order();
//            var orderSubtotalExclTax = decimal.Zero;
//            var orderSubtotalInclTax = decimal.Zero;
//            try
//            {
//                foreach (var item in cart)
//                {
//                    var product = _productService.GetProductById(item.ProductId);
//                    var exclTax = (product.Price * item.Quantity) / 1.18M;
//                    var inclTax = product.Price * item.Quantity;
//                    orderSubtotalExclTax += exclTax;
//                    orderSubtotalInclTax += inclTax;

//                    var orderItem = new OrderItem
//                    {
//                        OrderItemGuid = Guid.NewGuid(),
//                        CreatedOnUtc = DateTime.Now,
//                        DownloadCount = 0,
//                        ItemWeight = 0,
//                        PriceExclTax = exclTax,
//                        PriceInclTax = inclTax,
//                        ProductId = item.ProductId,
//                        Quantity = item.Quantity,
//                        UnitPriceExclTax = (product.Price / 1.18M),
//                        UnitPriceInclTax = product.Price
//                    };

//                    order.OrderItems.Add(orderItem);
//                }

//                #region Default values
//                order.OrderGuid = Guid.NewGuid();
//                order.CreatedOnUtc = DateTime.UtcNow;
//                order.OrderShippingExclTax = decimal.Zero;
//                order.OrderShippingInclTax = decimal.Zero;
//                order.OrderSubTotalDiscountExclTax = decimal.Zero;
//                order.OrderSubTotalDiscountInclTax = decimal.Zero;
//                #endregion

//                order.ShippingAddress = customer.ShippingAddress;
//                order.BillingAddress = customer.BillingAddress;
//                order.CustomerEmail = customer.Email;
//                order.CustomerId = customer.Id;
//                order.FirstName = customer.ShippingAddress.FirstName;
//                order.LastName = customer.ShippingAddress.LastName;
//                order.OrderSubtotalExclTax = orderSubtotalExclTax;
//                order.OrderSubtotalInclTax = orderSubtotalInclTax;
//                order.OrderTotal = orderSubtotalInclTax;
//                order.OrderTax = orderSubtotalInclTax - orderSubtotalExclTax;
//                order.TaxRates = "18.00000000:" + (orderSubtotalInclTax - orderSubtotalExclTax).ToString();
//                order.PaymentMethodSystemName = paymentNeeded > 0 ? "Payments.PaymentOptions" : "Rewards.RewardOperations";
//                order.ShippingMethod = "Kargo";
//                order.ShippingStatusId = (int)ShippingStatus.NotYetShipped;
//                order.PaymentStatusId = (int)PaymentStatus.Pending;
//                order.OrderStatusId = (int)OrderStatus.Processing;

//                //Add Cargo
//                var fee = _checkoutWebService.PrepareShippingMethod(cart, customer.ShippingAddress).ShippingMethods.First().Fee;
//                var cargoTotal = Convert.ToDecimal(fee.Split(' ').First());
//                order.OrderTotal+= cargoTotal;

//                _orderService.InsertOrder(order);

//                return new OrderResponseModel()
//                {
//                    Order = order,
//                    Success = true,
//                };
//            }
//            catch (Exception exception)
//            {
//                return new OrderResponseModel()
//                {
//                    Order = order,
//                    Success = false,
//                    Exception = exception
//                };
//            }
//        }

//        [NonAction]
//        private decimal GetPaymentNeeded(Customer customer, List<ShoppingCartItem> cart)
//        {
//            string filterByCountryId = "";
//            if (_addressWebService.AddressSettings().CountryEnabled &&
//                customer.BillingAddress != null &&
//                !String.IsNullOrEmpty(customer.BillingAddress.CountryId))
//            {
//                filterByCountryId = customer.BillingAddress.CountryId;
//            }

//            var orderTotal = _shoppingCartWebService.PrepareOrderTotals(cart, true).SubTotal.Replace(" PortTL", "");
//            var cartTotal = Convert.ToDecimal(orderTotal.Split(' ').First());
//            var currentBalance = _processService.GetCustomerActualPoint(GetCustomer().Username);
//            var pointFactor = Convert.ToDecimal(_settingService.GetSetting("payment.moneypointfactor").Value);

//            if (currentBalance >= cartTotal)
//                return 0;

//            var cardPayment = cartTotal - currentBalance;
//            var paymentNeeded = cardPayment / pointFactor;

//            return paymentNeeded;
//        }

//        [NonAction]
//        private void SetOrderAsPaid(Order order)
//        {
//            order.PaymentStatusId = (int)PaymentStatus.Paid;
//            _orderService.UpdateOrder(order);
//        }

//        [NonAction]
//        public void SendEmailNotification(Order order)
//        {
//            var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
//                    _pdfService.PrintOrderToPdf(order, order.CustomerLanguageId) : null;
//            var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
//                "order.pdf" : null;

//            //Mesafeli satış sözleşmesi emaili
//            int orderPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
//            .SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
//            if (orderPlacedCustomerNotificationQueuedEmailId > 0)
//            {
//                _orderService.InsertOrderNote(new OrderNote
//                {
//                    Note = "\"Mesafeil Satış Sözleşmesi\" mail gönderme kuyruğuna eklenildi.",
//                    DisplayToCustomer = false,
//                    CreatedOnUtc = DateTime.UtcNow,
//                    OrderId = order.Id,
//                });
//            }

//            //Ön bilgilendirme metni emaili
//            int sendContractOnBilgilendirmeFormuNotificationQueuedEmailId = _workflowMessageService
//                .SendContractOnBilgilendirmeFormuNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
//            if (sendContractOnBilgilendirmeFormuNotificationQueuedEmailId > 0)
//            {
//                _orderService.InsertOrderNote(new OrderNote
//                {
//                    Note = "\"Ön Bilgilendirme Formu\" mail gönderme kuyruğuna eklenildi.",
//                    DisplayToCustomer = false,
//                    CreatedOnUtc = DateTime.UtcNow,
//                    OrderId = order.Id,
//                });
//            }
//        }
//        #endregion
       
//        #region Customer
//        [HttpPost(nameof(Login))]
//        [IngoreAllAttributes]
//        public virtual IActionResult Login([FromBody] LoginModel model)
//        {
//            //validate CAPTCHA
//            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage)
//            {
//                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
//            }

//            var errorList = new List<string>();

//            if (ModelState.IsValid)
//            {
//                if (_customerSettings.UsernamesEnabled && model.Username != null)
//                {
//                    model.Username = model.Username.Trim();
//                }
//                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
//                switch (loginResult)
//                {
//                    case CustomerLoginResults.Successful:
//                        {
//                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

//                            //sign in new customer
//                            _authenticationService.SignIn(customer, model.RememberMe);

//                            //raise event       
//                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

//                            //activity log
//                            _customerActivityService.InsertActivity("PublicStore.Login", "", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

//                            var tokenString = GenerateJwtToken(customer.Username);

//                            return Success(new { Token = tokenString, Message = "Success" });
//                        }
//                    case CustomerLoginResults.CustomerNotExist:
//                        errorList.Add(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
//                        break;
//                    case CustomerLoginResults.Deleted:
//                        errorList.Add(_localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
//                        break;
//                    case CustomerLoginResults.NotActive:
//                        errorList.Add(_localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
//                        break;
//                    case CustomerLoginResults.NotRegistered:
//                        errorList.Add(_localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
//                        break;
//                    case CustomerLoginResults.LockedOut:
//                        errorList.Add(_localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
//                        break;
//                    case CustomerLoginResults.WrongPassword:
//                    default:
//                        errorList.Add(_localizationService.GetResource("Account.Login.WrongCredentials"));
//                        break;
//                }
//            }

//            return Error(errorList);
//        }

//        [HttpPost(nameof(SendPassword))]
//        [IngoreAllAttributes]
//        public IActionResult SendPassword([FromBody] PasswordRequestModel model)
//        {
//            if (string.IsNullOrWhiteSpace(model.Email))
//                return Error("Email can't be null");

//            var customer = _customerService.GetCustomerByEmail(model.Email);

//            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId) ?? _emailAccountService.GetAllEmailAccounts().FirstOrDefault();

//            if (emailAccount == null)
//                return Error("Email account can't be loaded");

//            var message = _messageTemplateService.GetMessageTemplateByName("SendPassword", GetCurrentStore().Id);

//            string body = string.Format(message.Body, customer.Username, customer.Password);

//            var mailToSent = new QueuedEmail
//            {
//                Priority = QueuedEmailPriority.High,
//                EmailAccountId = emailAccount.Id,
//                FromName = emailAccount.DisplayName,
//                From = emailAccount.Email,
//                ToName = customer.GetFullName(),
//                To = customer.Email,
//                Subject = message.Subject,
//                Body = body,
//                CreatedOnUtc = DateTime.UtcNow,
//                DontSendBeforeDateUtc = null
//            };
//            _queuedEmailService.InsertQueuedEmail(mailToSent);

//            return Success("Giriş bilgileriniz e-posta adresinize gönderildi.");
//        }

//        [HttpPost(nameof(ChangePassword))]
//        public IActionResult ChangePassword([FromBody] PasswordChangeRequestModel model)
//        {
//            var customer = GetCustomer();

//            var changePassRequest = new ChangePasswordRequest(customer.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);

//            var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
//            return changePassResult.Success
//                ? Success(_localizationService.GetResource("Admin.Customers.Customers.PasswordChanged"))
//                : Error(changePassResult.Errors);
//        }

//        [HttpGet(nameof(CustomerPoints))]
//        public IActionResult CustomerPoints() => Success(_processService.GetCustomerActualPoint(GetUsername()));

//        [HttpGet(nameof(PointHistory))]
//        public IActionResult PointHistory()
//        {
//            var history = _processService.GetAllHistory(GetUsername());

//            var result = history.OrderByDescending(x => x.CreatedOnUtc).Select(x => new
//            {
//                Type = GetPointType(x.TypeId),
//                x.Description,
//                x.OrderNumber,
//                x.Point,
//                CreatedOn = x.CreatedOnUtc.ToString("dd.MM.yyyy HH:mm"),
//            }).ToList();

//            return Success(result);
//        }

//        [HttpGet(nameof(HasEnoughPoints))]
//        public IActionResult HasEnoughPoints()
//        {
//            return ValidatePoints() ? Success() : Error(false);
//        }

//        [HttpGet(nameof(CustomerInfo))]
//        public virtual IActionResult CustomerInfo()
//        {
//            SetContext();

//            var customer = GetCustomer();
//            var store = GetCurrentStore();

//            var firstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName, store.Id);
//            var lastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName, store.Id);
//            var phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone, store.Id);

//            if (string.IsNullOrEmpty(firstName))
//            {
//                firstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName, string.Empty);
//                lastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName, string.Empty);
//            }
//            if (string.IsNullOrEmpty(phone))
//            {
//                phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone, string.Empty);
//            }

//            var result = new
//            {
//                customer.Username,
//                customer.Email,
//                FirstName = firstName,
//                LastName = lastName,
//                Phone = phone,
//            };

//            return Success(result);
//        }

//        [HttpPost(nameof(UpdateCustomerInfo))]
//        public IActionResult UpdateCustomerInfo([FromBody] CustomerInfoRequestModel model)
//        {
//            SetContext();

//            var customer = GetCustomer();
//            var store = GetCurrentStore();

//            if (!customer.Email.Equals(model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
//                _customerRegistrationService.SetEmail(customer, model.Email.Trim());

//            if (_customerSettings.PhoneEnabled)
//                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone, store.Id);

//            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName, store.Id);
//            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName, store.Id);

//            return Success();
//        }

//        [HttpGet(nameof(AddressList))]
//        public virtual IActionResult AddressList()
//        {
//            SetContext();

//            var model = _customerWebService.PrepareAddressList(_customerService.GetCustomerByUsername(GetUsername()));

//            var result = model.Addresses.Select(x => new
//            {
//                x.Id,
//                x.Email,
//                x.FirstName,
//                x.LastName,
//                x.PhoneNumber,
//                x.VatNumber,
//                x.ZipPostalCode,
//                Address = x.Address1,
//                CityId = x.CountryId,
//                DistrictId = x.StateProvinceId,
//            }).ToList();

//            return Success(result);
//        }

//        [HttpGet(nameof(Address))]
//        public virtual IActionResult Address(string addressId)
//        {
//            var customer = GetCustomer();
//            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);

//            if (address == null)
//                return Error("Address not found");

//            var result = new
//            {
//                address.Id,
//                address.Email,
//                address.FirstName,
//                address.LastName,
//                address.PhoneNumber,
//                address.VatNumber,
//                address.ZipPostalCode,
//                Address = address.Address1,
//                CityId = address.CountryId,
//                DistrictId = address.StateProvinceId,
//                CustomerId = customer.Id,
//            };

//            return Success(result);
//        }

//        [HttpPost(nameof(AddAddress))]
//        public virtual IActionResult AddAddress([FromBody] AddressModel address)
//        {
//            if (!ModelState.IsValid && ModelState.ErrorCount > 0)
//                return Error(ModelState);

//            var customer = GetCustomer();
//            var addressToAdd = GetAddressFromModel(address, customer.Id);
//            _customerService.InsertAddress(addressToAdd);

//            return Success();
//        }

//        [HttpPost(nameof(UpdateAddress))]
//        public virtual IActionResult UpdateAddress([FromBody] AddressModel address)
//        {
//            if (!ModelState.IsValid && ModelState.ErrorCount > 0)
//                return Error(ModelState);

//            var customer = GetCustomer();
//            var addressInDb = customer.Addresses.FirstOrDefault(x => x.Id == address.Id);

//            if (addressInDb == null)
//                return Json(new ErrorResult("Address not found"));

//            var addressToUpdate = GetAddressFromModel(address, customer.Id);
//            addressToUpdate.Id = address.Id;
//            _customerService.UpdateAddress(addressToUpdate);

//            if (customer.BillingAddress?.Id == address.Id)
//                _customerService.UpdateBillingAddress(addressToUpdate);
//            if (customer.ShippingAddress?.Id == address.Id)
//                _customerService.UpdateShippingAddress(addressToUpdate);

//            return Success();
//        }

//        [HttpPost(nameof(DeleteAddress))]
//        public virtual IActionResult DeleteAddress([FromBody] DeleteAddressRequestModel model)
//        {
//            if (string.IsNullOrEmpty(model.AddressId))
//                return Json(new ErrorResult("Address ID is null"));

//            var customer = GetCustomer();
//            var address = customer.Addresses.FirstOrDefault(a => a.Id == model.AddressId);

//            if (address == null)
//                return Json(new ErrorResult("Address not found"));

//            customer.RemoveAddress(address);
//            address.CustomerId = customer.Id;
//            _customerService.DeleteAddress(address);

//            return Success();
//        }

//        [HttpGet(nameof(Cities))]
//        public virtual IActionResult Cities()
//        {
//            var cities = _countryService.GetAllCountries();

//            if (!cities.Any())
//                return Error("Cities not found");

//            var result = cities.Select(x => new
//            {
//                x.Id,
//                x.Name,
//                x.NumericIsoCode,
//                x.AllowsShipping,
//            }).ToList();

//            return Success(result);
//        }

//        [HttpGet(nameof(Districts))]
//        public virtual IActionResult Districts(string cityId)
//        {
//            if (string.IsNullOrEmpty(cityId))
//                return Json(new ErrorResult("City ID is null"));

//            var districts = _stateProvinceService.GetStateProvincesByCountryId(cityId);

//            if (!districts.Any())
//                return Error("Districts not found");

//            var result = districts.Select(x => new
//            {
//                x.Id,
//                x.Name,
//                CityId = x.CountryId,
//            }).ToList();

//            return Success(result);
//        }
//        #endregion

//        #region Category
//        [HttpGet(nameof(CategoryList))]
//        public IActionResult CategoryList(string inTopMenu = null, string inHomePage = null, List<string> idList = null)
//        {
//            var categoryList = _categoryService.GetAllCategories().Where(x => x.HideOnCatalog == false).ToList();

//            if (inTopMenu != null)
//                categoryList = categoryList.Where(x => x.IncludeInTopMenu == true).ToList();

//            if (inHomePage != null)
//                categoryList = categoryList.Where(x => x.ShowOnHomePage == true).ToList();

//            if (idList != null)
//                categoryList = categoryList.Where(x => idList.Contains(x.Id)).ToList();

//            var result = categoryList.Select(x => new
//            {
//                x.DisplayOrder,
//                x.HideOnCatalog,
//                x.Id,
//                x.IncludeInTopMenu,
//                x.Name,
//                x.ParentCategoryId,
//                x.Published,
//                x.SeName,
//                x.ShowOnHomePage,
//            });

//            return Success(result);
//        }
//        #endregion

//        #region Slider
//        [HttpGet(nameof(Slider))]
//        public IActionResult Slider()
//        {
//            var s = _settingService.LoadSetting<MobileSliderSettings>(GetCurrentStore().Id);

//            var list = new List<MobileSlider>
//            {
//                new MobileSlider()
//                {
//                    PictureUrl = GetPictureUrl(s.Picture1Id),
//                    Text = s.Text1,
//                    Link = s.Link1,
//                },
//                new MobileSlider()
//                {
//                    PictureUrl = GetPictureUrl(s.Picture2Id),
//                    Text = s.Text2,
//                    Link = s.Link2,
//                },
//                new MobileSlider()
//                {
//                    PictureUrl = GetPictureUrl(s.Picture3Id),
//                    Text = s.Text3,
//                    Link = s.Link3,
//                },
//                new MobileSlider()
//                {
//                    PictureUrl = GetPictureUrl(s.Picture4Id),
//                    Text = s.Text4,
//                    Link = s.Link4,
//                },
//                new MobileSlider()
//                {
//                    PictureUrl = GetPictureUrl(s.Picture5Id),
//                    Text = s.Text5,
//                    Link = s.Link5,
//                }
//            };

//            var result = list.Where(x => !string.IsNullOrEmpty(x.PictureUrl)).ToList();

//            return Success(result);
//        }
//        #endregion

//        #region Product
//        [HttpGet(nameof(HomeProducts))]
//        public IActionResult HomeProducts()
//        {
//            var list = _productService.GetAllProductsDisplayedOnHomePage().ToList();

//            var products = list.Select(x => new
//            {
//                x.Id,
//                x.ProductType,
//                x.Name,
//                x.IsFreeShipping,
//                x.StockQuantity,
//                Price = _priceFormatter.FormatPrice(x.Price),
//                Pictures = x.ProductPictures.Select(p => _pictureService.GetPictureUrl(p.PictureId)).ToList(),
//            }).ToList();

//            return Success(products);
//        }

//        [HttpGet(nameof(SearchProducts))]
//        public IActionResult SearchProducts(string keywords, int pageIndex = 0, int pageSize = 10)
//        {
//            if (string.IsNullOrWhiteSpace(keywords) || pageIndex < 0 || pageSize <= 0)
//                return Json(new ErrorResult("keywords/page is invalid"));

//            var productList = _productService.SearchProductsTR(keywords: keywords, pageIndex: pageIndex, pageSize: pageSize).ToList();

//            var products = productList.Select(x => new
//            {
//                x.Id,
//                x.ProductType,
//                x.Name,
//                x.SeName,
//                x.FullDescription,
//                x.ShortDescription,
//                x.IsFreeShipping,
//                x.Sku,
//                x.StockQuantity,
//                Price = _priceFormatter.FormatPrice(x.Price),
//                Pictures = x.ProductPictures.Select(p => _pictureService.GetPictureUrl(p.PictureId)).ToList(),
//            }).ToList();

//            return Success(products);
//        }

//        [HttpGet(nameof(SearchProductsByCategoryId))]
//        public IActionResult SearchProductsByCategoryId(IList<string> searchCategoryIds, int pageIndex = 0, int pageSize = 10)
//        {
//            if (!searchCategoryIds.Any())
//                return Error("Category Ids are null");

//            if (pageIndex < 0 || pageSize <= 0)
//                return Error("page is invalid");

//            var products = _productService.SearchProducts(categoryIds: searchCategoryIds, pageIndex: pageIndex, pageSize: pageSize).ToList();

//            var productList = products.Select(x => new
//            {
//                x.Id,
//                x.ProductType,
//                x.Name,
//                x.SeName,
//                x.FullDescription,
//                x.ShortDescription,
//                x.IsFreeShipping,
//                x.Sku,
//                x.StockQuantity,
//                Price = _priceFormatter.FormatPrice(x.Price),
//                Pictures = x.ProductPictures.Select(p => _pictureService.GetPictureUrl(p.PictureId)).ToList(),
//            }).ToList();

//            return Success(productList);
//        }

//        [HttpGet(nameof(SearchProductsByManufacturerId))]
//        public IActionResult SearchProductsByManufacturerId(string manufacturerId, int pageIndex = 0, int pageSize = 10)
//        {

//            if (string.IsNullOrWhiteSpace(manufacturerId))
//                return Error("manufacturer id is null");

//            if (pageIndex < 0 || pageSize <= 0)
//                return Error("page is invalid");

//            var products = _productService.SearchProducts(manufacturerId: manufacturerId, pageIndex: pageIndex, pageSize: pageSize).ToList();

//            var productList = products.Select(x => new
//            {
//                x.Id,
//                x.ProductType,
//                x.Name,
//                x.SeName,
//                x.FullDescription,
//                x.ShortDescription,
//                x.IsFreeShipping,
//                x.Sku,
//                x.StockQuantity,
//                Price = _priceFormatter.FormatPrice(x.Price),
//                Pictures = x.ProductPictures.Select(p => _pictureService.GetPictureUrl(p.PictureId)).ToList(),
//            }).ToList();

//            return Success(productList);
//        }

//        [HttpGet(nameof(ProductDetails))]
//        public virtual IActionResult ProductDetails(string productId)
//        {
//            if (string.IsNullOrWhiteSpace(productId))
//                return Error("product id is null");

//            var product = _productService.GetProductById(productId);

//            if (product == null)
//                return Json(new ErrorResult("Product not found"));

//            var result = new
//            {
//                product.Id,
//                product.ProductType,
//                product.Name,
//                product.SeName,
//                product.FullDescription,
//                product.ShortDescription,
//                product.IsFreeShipping,
//                product.Sku,
//                product.StockQuantity,
//                Price = _priceFormatter.FormatPrice(product.Price),
//                Pictures = product.ProductPictures.Select(p => _pictureService.GetPictureUrl(p.PictureId)).ToList(),
//            };

//            return Success(result);
//        }
//        #endregion

//        #region Shopping Cart
//        [HttpGet(nameof(Cart))]
//        [AllowAnonymous]
//        public virtual IActionResult Cart(int shoppingCartTypeId = 1)
//        {
//            SetContext();

//            var cartType = (ShoppingCartType)shoppingCartTypeId;
//            var customer = GetCustomer();

//            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == cartType).ToList();
//            var workingCurrency = GetWorkingCurrency();
//            var model = new ShoppingCartModel();

//            //prepare model
//            foreach (var item in cart)
//            {
//                var product = _productService.GetProductById(item.ProductId);
//                var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetUnitPrice(item), out decimal taxRate);
//                var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, workingCurrency);

//                model.Items.Add(new ShoppingCartModel.ShoppingCartItemModel()
//                {
//                    Id = item.Id,
//                    ProductId = item.ProductId,
//                    UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount),
//                    Quantity = item.Quantity,
//                    ProductName = product.Name,
//                    Picture = product.ProductPictures.Select(p => new PictureModel()
//                    {
//                        ImageUrl = _pictureService.GetPictureUrl(p.PictureId, _mediaSettings.CartThumbPictureSize)
//                    }).FirstOrDefault(),
//                });
//            }

//            //prepare result
//            var cartItems = model.Items.Select(x => new
//            {
//                x.Id,
//                x.UnitPrice,
//                x.ProductId,
//                x.Quantity,
//                x.ProductName,
//                Picture = x.Picture?.ImageUrl,
//            }).ToList();

//            var orderTotal = _shoppingCartWebService.PrepareOrderTotals(cart, true);

//            var result = new
//            {
//                CartItems = cartItems,
//                CartTotal = new
//                {
//                    orderTotal.OrderTotal,
//                    orderTotal.OrderTotalDiscount,
//                    orderTotal.SelectedShippingMethod,
//                    orderTotal.Shipping,
//                    orderTotal.SubTotal,
//                    orderTotal.SubTotalDiscount,
//                }
//            };

//            return Success(result);
//        }

//        [HttpPost(nameof(AddToCart))]
//        public virtual IActionResult AddToCart([FromBody] AddToCartRequestModel model)
//        {
//            if (model == null)
//                return Json(new ErrorResult("model is null"));

//            var cartType = (ShoppingCartType)model.ShoppingCartTypeId;
//            var product = _productService.GetProductById(model.ProductId);

//            #region Validation
//            if (product == null)
//                return Error($"No product found with the specified ID ({model.ProductId})");

//            if (product.ProductType != ProductType.SimpleProduct)
//                return Error("Product is not Simple Product");

//            if (product.OrderMinimumQuantity > model.Quantity)
//                return Error("Quantity can't be higher than OrderMinimumQuantity");

//            if (product.ParseAllowedQuantities().Length > 0)
//                return Error("allowedQuantities > 0");

//            if (product.ProductAttributeMappings.Any())
//                return Error("Product is not Simple Product");
//            #endregion

//            var customer = GetCustomer();
//            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == cartType).ToList();
//            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product.Id);

//            //Update if already exists
//            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + model.Quantity : model.Quantity;
//            var item = new ShoppingCartItem()
//            {
//                ShoppingCartType = cartType,
//                StoreId = GetCurrentStore().Id,
//                CustomerEnteredPrice = decimal.Zero,
//                Quantity = quantityToValidate
//            };
//            var addToCartWarnings = _shoppingCartService.GetShoppingCartItemWarnings(customer, item, product, false);

//            if (addToCartWarnings.Any())
//                return Error(addToCartWarnings);

//            //Add
//            addToCartWarnings = _shoppingCartService.AddToCart(customer: customer,
//                productId: model.ProductId,
//                shoppingCartType: cartType,
//                storeId: GetCurrentStore().Id,
//                quantity: model.Quantity);

//            if (addToCartWarnings.Any())
//                return Error(addToCartWarnings);

//            return Success();
//        }

//        [HttpPost(nameof(UpdateCart))]
//        public IActionResult UpdateCart([FromBody] UpdateCartRequestModel requestModel)
//        {
//            var warnings = _shoppingCartService.UpdateShoppingCartItem(GetCustomer(), requestModel.ItemId, requestModel.AttributesXml, decimal.Zero, quantity: requestModel.NewQuantity);

//            if (warnings.Any())
//                return Error(warnings);

//            return Success();
//        }

//        [HttpPost(nameof(DeleteFromCart))]
//        public IActionResult DeleteFromCart([FromBody] DeleteCartRequestModel requestModel)
//        {
//            var customer = GetCustomer();

//            var cartType = (ShoppingCartType)requestModel.ShoppingCartTypeId;

//            var cart = customer.ShoppingCartItems
//                .Where(sci => sci.ShoppingCartType == cartType)
//                .ToList();

//            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, requestModel.ProductId);

//            _shoppingCartService.DeleteShoppingCartItem(customer.Id, shoppingCartItem, ensureOnlyActiveCheckoutAttributes: true);

//            return Json(new SuccessResult("Item successfully deleted"));
//        }
//        #endregion

//        #region Checkout
//        [HttpGet(nameof(OrderAddress))]
//        public IActionResult OrderAddress(string addressType)
//        {
//            if (!CartAny())
//                return Error("Cart is empty");

//            if (string.IsNullOrEmpty(addressType))
//                return Error("Address Type did not specified");

//            if (addressType.ToLower() != "billing" && addressType.ToLower() != "shipping")
//                return Error("Address Type is wrong");

//            var customer = GetCustomer();
//            var address = addressType.ToLower() == "shipping" ? customer.ShippingAddress : customer.BillingAddress;

//            if (address == null)
//                return Error($"{addressType} address not found");

//            var country = _countryService.GetCountryById(address.CountryId);
//            var state = _stateProvinceService.GetStateProvinceById(address.StateProvinceId);

//            var result = new
//            {
//                address.Id,
//                address.FirstName,
//                address.LastName,
//                Address = address.Address1,
//                City = country.Name,
//                District = state.Name,
//            };

//            return Success(result);
//        }

//        [HttpPost(nameof(SaveOrderAddress))]
//        public IActionResult SaveOrderAddress([FromBody] SaveOrderAddressRequestModel model)
//        {
//            if (!CartAny())
//                return Error("Cart is empty");

//            if (string.IsNullOrEmpty(model.AddressType))
//                return Error("Address Type did not specified");

//            if (model.AddressType.ToLower() != "billing" && model.AddressType.ToLower() != "shipping")
//                return Error("Address Type is wrong");

//            var customer = GetCustomer();
//            var address = customer.Addresses.FirstOrDefault(a => a.Id == model.AddressId);

//            if (address == null)
//                return Error($"{model.AddressId} address not found");

//            address.CustomerId = customer.Id;

//            var isShippingAddress = model.AddressType.ToLower() == "shipping";

//            if (isShippingAddress)
//            {
//                customer.ShippingAddress = address;
//                _customerService.UpdateShippingAddress(address);
//            }

//            if (!isShippingAddress)
//            {
//                customer.BillingAddress = address;
//                _customerService.UpdateBillingAddress(address);
//            }

//            return Success();
//        }

//        [HttpGet(nameof(ShippingMethod))]
//        public IActionResult ShippingMethod()
//        {
//            SetContext();

//            if (!CartAny())
//                return Error("Cart is empty");

//            var customer = GetCustomer();
//            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
//            var shippingMethodModel = _checkoutWebService.PrepareShippingMethod(cart, customer.ShippingAddress);

//            return Success(shippingMethodModel);
//        }

//        [HttpPost(nameof(SaveShippingMethod))]
//        public IActionResult SaveShippingMethod([FromBody] SaveShippingMethodRequestModel model)
//        {
//            if (!CartAny())
//                return Error("Cart is empty");

//            var customer = GetCustomer();

//            if (string.IsNullOrEmpty(model.ShippingOption))
//                throw new Exception("Selected shipping method can't be parsed");

//            var splittedOption = model.ShippingOption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
//            if (splittedOption.Length != 2)
//                throw new Exception("Selected shipping method can't be parsed");

//            string selectedName = splittedOption[0];
//            string shippingRateComputationMethodSystemName = splittedOption[1];

//            //clear shipping option XML/Description
//            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ShippingOptionAttributeXml, "");
//            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ShippingOptionAttributeDescription, "");

//            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

//            var shippingOptions = _shippingService
//                        .GetShippingOptions(customer, cart, customer.ShippingAddress, shippingRateComputationMethodSystemName)
//                        .ShippingOptions
//                        .ToList();

//            var shippingOption = shippingOptions.Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase));
//            if (shippingOption == null)
//                throw new Exception("Selected shipping method can't be loaded");

//            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption);

//            return Success();
//        }

//        [HttpGet(nameof(PaymentAmount))]
//        public IActionResult PaymentAmount()
//        {
//            SetContext();

//            if (!CartAny())
//                return Error("Cart is empty");

//            var customer = GetCustomer();

//            var cart = customer.ShoppingCartItems
//                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
//                .ToList();
//            decimal paymentNeeded = GetPaymentNeeded(customer, cart);

//            return Success(paymentNeeded);
//        }

//        [HttpGet(nameof(ConfirmOrder))]
//        public IActionResult ConfirmOrder()
//        {
//            if (!CartAny())
//                return Error("Cart is empty");

//            if (!ValidatePoints() && !is3DPayActive)
//                return Error("Not enough points");

//            var customer = GetCustomer();
//            var paymentNeeded = GetPaymentNeeded(customer, GetCart());
//            var result = SaveOrder(customer, paymentNeeded);

//            if (result.Success)
//            {
//                ClearCart();

//                // Enough Reward Points
//                if (paymentNeeded == 0m)
//                {
//                    SpendPoint(customer, result.Order);
//                    SetOrderAsPaid(result.Order);
//                    SendEmailNotification(result.Order);
//                    ViewData["PaymentStatus"] = "true";
//                    //return RedirectToAction("Callback", "MobileApp");
//                    var model = Success(result.Order.OrderNumber.ToString());
//                    var json = JsonConvert.SerializeObject(model);
//                    return RedirectToAction("Callback", "MobileApp", new { result = json });
//                }

//                var pageModel = new PaymentOptionModel()
//                {
//                    Customer = customer,
//                    TotalDebit = paymentNeeded,
//                    OrderGuide = result.Order.OrderGuid.ToString(),
//                };

//                return View("PaymentForm", pageModel);
//            }

//            return Error(result.Exception);
//        }
//        #endregion

//        #region Orders

//        [HttpGet(nameof(Orders))]
//        public IActionResult Orders()
//        {
//            var orders = _orderService.SearchOrders(customerId: GetCustomer().Id);

//            var result = orders.Select(x => new
//            {
//                x.Id,
//                x.OrderNumber,
//                x.OrderTotal,
//                ShippingTotal = x.OrderShippingInclTax,
//                SubTotal = x.OrderSubtotalInclTax,
//                CreatedOn = x.CreatedOnUtc.ToString("dd/MM/yyyy HH:mm"),
//                OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, GetLangId()),
//                ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, GetLangId()),
//                ShippingType = x.ShippingMethod,
//                ShippingAddress = x.ShippingAddress.Address1,
//                ShippingCity = _countryService.GetCountryById(x.ShippingAddress.CountryId).Name,
//                ShippingDistrict = _stateProvinceService.GetStateProvinceById(x.ShippingAddress.StateProvinceId).Name,
//                ShippingName = x.ShippingAddress.FirstName + " " + x.ShippingAddress.LastName,
//                ShippingPhone = x.ShippingAddress.PhoneNumber,
//                BillingAddress = x.BillingAddress.Address1,
//                BillingCity = _countryService.GetCountryById(x.BillingAddress.CountryId).Name,
//                BillingDistrict = _stateProvinceService.GetStateProvinceById(x.BillingAddress.StateProvinceId).Name,
//                BillingName = x.BillingAddress.FirstName + " " + x.BillingAddress.LastName,
//                BillingPhone = x.BillingAddress.PhoneNumber,
//                ShippingUrl = GetTrackingUrl(x.Id),
//                OrderItems = x.OrderItems.Select(i => new
//                {
//                    _productService.GetProductById(i.ProductId)?.Name,
//                    i.Quantity,
//                    UnitPrice = i.UnitPriceInclTax,
//                    Total = i.PriceInclTax,
//                    Picture = _productService.GetProductById(i.ProductId)?.ProductPictures.Select(p => new PictureModel()
//                    {
//                        ImageUrl = _pictureService.GetPictureUrl(p.PictureId, _mediaSettings.CartThumbPictureSize)
//                    }).FirstOrDefault()?.ImageUrl,
//                }).ToList(),
//            });

//            return Success(result);
//        }
//        #endregion

//        #region Payment
//        [HttpPost(nameof(ThreeDPayment))]
//        [IngoreAllAttributes]
//        public virtual IActionResult ThreeDPayment(PaymentOptionModel model)
//        {
//            var order = _orderService.GetOrderByGuid(Guid.Parse(model.OrderGuide));
//            if (order != null)
//            {
//                var customer = _customerService.GetCustomerById(order.CustomerId);
//                var selectedPointPaidVal = decimal.Parse(customer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));
//                var availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

//                //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
//                var debitPointValue = (availeblePoint - decimal.Parse(MoneyToPoint.CalculatePoint(order.OrderTotal)));
//                var setting = _settingService.GetSetting("payment.moneypointfactor");
//                var debitMoneyValue = ((debitPointValue * -1) * decimal.Parse(setting.Value));

//                var iParaSet = new Settings
//                {
//                    BaseUrl = "https://api.ipara.com/",
//                    Mode = "t",
//                    PrivateKey = _settingService.GetSetting("paymentoption.iparasettings.privatekey").Value,
//                    PublicKey = _settingService.GetSetting("paymentoption.iparasettings.publickey").Value,
//                    ThreeDInquiryUrl = "https://www.ipara.com/3dgate",
//                    HashString = string.Empty,
//                    Version = "1.0",
//                    TransactionDate = DateTime.Now.ToString()
//                };

//                var request = new ThreeDPaymentInitRequest
//                {
//                    OrderId = model.OrderGuide,
//                    Echo = "Echo",
//                    Mode = iParaSet.Mode,
//                    Version = iParaSet.Version,
//                    Amount = debitMoneyValue.ToString("F").Replace(",", "").Replace(".", ""),
//                    CardOwnerName = model.CardNameSurname,
//                    CardNumber = model.CardNumber,
//                    CardExpireMonth = model.CardMonth,
//                    CardExpireYear = model.CardYear,
//                    Installment = "1",
//                    Cvc = model.CardCvv,
//                    PurchaserName = order.ShippingAddress.FirstName,
//                    PurchaserSurname = order.ShippingAddress.LastName,
//                    PurchaserEmail = order.CustomerEmail
//                };

//                string cardInfo = model.CardNameSurname + "_" + model.CardNumber + "_" + model.CardMonth + "_" + model.CardYear + "_" + model.CardCvv;
//                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "cardInfo", cardInfo,
//                    _storeContext.CurrentStore.Id);

//                //Ssl olsun mu
//                var storeLocation = _webHelper.GetStoreLocation(false);

//                request.SuccessUrl = $"{storeLocation}app/IParaSuccess";
//                request.FailUrl = $"{storeLocation}app/CancelOrder";
//                //request.SuccessUrl = "http://localhost:16592/Plugins/PaymentOptions/IParaSuccess";
//                //request.FailUrl = "http://localhost:16592/Plugins/PaymentOptions/CancelOrder";

//                var form = ThreeDPaymentInitRequest.Execute(request, iParaSet);
//                ViewBag.Source = form;

//                _orderService.InsertOrderNote(new OrderNote
//                {
//                    Note = order.OrderNumber.ToString() + " numaralı sipariş, " + model.TotalDebit.ToString() + " tutarı için bankaya yönlendirildi.",
//                    DisplayToCustomer = false,
//                    CreatedOnUtc = DateTime.UtcNow,
//                    OrderId = order.Id,
//                });

//                return View("~/Plugins/Payments.PaymentOptions/Views/ThreeDPayment.cshtml");

//            }
//            return NoContent();
//        }

//        [IgnoreAntiforgeryToken]
//        [IngoreAllAttributes]
//        [HttpPost(nameof(IParaSuccess))]
//        public IActionResult IParaSuccess(IFormCollection form)
//        {
//            Settings iParaSet = new Settings
//            {
//                BaseUrl = "https://api.ipara.com/",
//                Mode = "P",
//                PrivateKey = _settingService.GetSetting("paymentoption.iparasettings.privatekey").Value,
//                PublicKey = _settingService.GetSetting("paymentoption.iparasettings.publickey").Value,
//                ThreeDInquiryUrl = "https://www.ipara.com/3dgate",
//                HashString = string.Empty,
//                Version = "1.0",
//                TransactionDate = form["transactionDate"]
//            };

//            var orderGuide = form["orderId"].ToString().Split("_");
//            var order = _orderService.GetOrderByGuid(new Guid(orderGuide[0]));
//            var carInfo = string.Empty;
//            var selectedPointPaidVal = 0m;
//            var customer = _customerService.GetCustomerById(order.CustomerId);
//            if (customer != null)
//            {
//                selectedPointPaidVal = decimal.Parse(customer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));
//                carInfo = customer.GenericAttributes.Where(x => x.Key == "cardInfo").FirstOrDefault().Value;
//            }

//            string[] cardInfoItems = carInfo.Split("_");

//            decimal availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

//            //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
//            decimal debitPointValue = (availeblePoint - decimal.Parse(MoneyToPoint.CalculatePoint(order.OrderTotal)));

//            var setting = _settingService.GetSetting("payment.moneypointfactor");
//            decimal debitMoneyValue = ((debitPointValue * -1) * decimal.Parse(setting.Value));

//            ThreeDPaymentInitResponse paymentResponse = new ThreeDPaymentInitResponse
//            {
//                OrderId = form["orderId"],
//                Result = form["result"],
//                Amount = form["amount"],
//                Mode = form["mode"]
//            };
//            if (!string.IsNullOrEmpty(form["errorCode"]))
//                paymentResponse.ErrorCode = form["errorCode"];

//            if (!string.IsNullOrEmpty(form["errorMessage"]))
//                paymentResponse.ErrorMessage = form["errorMessage"];

//            if (!string.IsNullOrEmpty(form["transactionDate"]))
//                paymentResponse.TransactionDate = form["transactionDate"];

//            if (!string.IsNullOrEmpty(form["hash"]))
//                paymentResponse.Hash = form["hash"];

//            if (Helper.Validate3DReturn(paymentResponse, iParaSet))
//            {
//                var request = new ThreeDPaymentCompleteRequest
//                {
//                    #region Request New
//                    OrderId = form["orderId"],
//                    Echo = "Echo",
//                    Mode = "P",
//                    Amount = form["amount"], // 100 tL
//                    CardOwnerName = cardInfoItems[0],
//                    CardNumber = cardInfoItems[1],
//                    CardExpireMonth = cardInfoItems[2],
//                    CardExpireYear = cardInfoItems[3],
//                    Installment = "1",
//                    Cvc = cardInfoItems[4],
//                    ThreeD = "true",
//                    ThreeDSecureCode = form["threeDSecureCode"],
//                    #endregion
//                    #region Sipariş veren bilgileri
//                    Purchaser = new Purchaser
//                    {
//                        Name = order.ShippingAddress.FirstName,
//                        SurName = order.ShippingAddress.LastName,
//                        Email = order.CustomerEmail
//                    },
//                    #endregion
//                    Products = new List<IPara.DeveloperPortal.Core.Entity.Product>()
//                };
//                foreach (var item in order.OrderItems)
//                {
//                    var product = _productService.GetProductById(item.ProductId);
//                    request.Products.Add(new IPara.DeveloperPortal.Core.Entity.Product
//                    {
//                        Title = product.Name,
//                        Code = product.Sku,
//                        Price = item.PriceInclTax.ToString().Replace(",", "").Replace(".", ""),
//                        Quantity = item.Quantity
//                    });
//                }

//                var response = ThreeDPaymentCompleteRequest.Execute(request, iParaSet);
//                if (response.Result == "1")
//                {
//                    string logMsg = JsonConvert.SerializeObject(response);

//                    order.PaymentStatus = PaymentStatus.Paid;
//                    order.OrderStatus = OrderStatus.Processing;
//                    order.AuthorizationTransactionId = form["result"];
//                    _orderService.UpdateOrder(order);

//                    _orderService.InsertOrderNote(new OrderNote
//                    {
//                        Note = debitMoneyValue.ToString() + " TL 3D ile alındı...",
//                        DisplayToCustomer = false,
//                        CreatedOnUtc = DateTime.UtcNow,
//                        OrderId = order.Id,
//                    });

//                    //Puanın düşülmesi
//                    string orderId = "";
//                    Process _proc = new Process()
//                    {
//                        Description = order.OrderNumber.ToString() + " numaralı sipariş için düşülen puan.",
//                        OrderGuid = order.OrderGuid,
//                        OrderNumber = order.OrderNumber,
//                        CreatedOnUtc = DateTime.Now,
//                        Point = selectedPointPaidVal * -1,
//                        TypeId = (int)ProcessType.Spend,
//                        Username = customer.Username,
//                    };
//                    orderId = _processService.Insert(_proc).Id;

//                    //Set new UserPoint
//                    var pointBalanceTotal = _processService.GetCustomerActualPoint(customer.Username);
//                    if (pointBalanceTotal >= 0)
//                        _rewardPointsService.RemoveOldPointAndAddNewPoints(customer.Id, pointBalanceTotal, _storeContext.CurrentStore.Id, string.Empty);

//                    if (!string.IsNullOrEmpty(orderId))
//                    {
//                        _orderService.InsertOrderNote(new OrderNote
//                        {
//                            Note = selectedPointPaidVal.ToString() + " puan düşüldü.",
//                            DisplayToCustomer = false,
//                            CreatedOnUtc = DateTime.UtcNow,
//                            OrderId = order.Id,
//                        });
//                    }
//                    else
//                    {
//                        _orderService.InsertOrderNote(new OrderNote
//                        {
//                            Note = selectedPointPaidVal.ToString() + " puan düşülürken hata oluştu!",
//                            DisplayToCustomer = false,
//                            CreatedOnUtc = DateTime.UtcNow,
//                            OrderId = order.Id,
//                        });
//                    }

//                    //Karttan çekilen tutarın oran tutarının hesaplaması
//                    List<OrderPayments> ordPayList = new List<OrderPayments>();
//                    foreach (var item in order.OrderItems)
//                    {
//                        OrderPayments ordPayment = new OrderPayments();
//                        ordPayment.CreatedOnUtc = DateTime.Now;
//                        ordPayment.OrderId = order.Id;
//                        ordPayment.OrderItemId = item.Id;
//                        ordPayment.Amount = (debitMoneyValue * item.PriceInclTax) / order.OrderTotal;
//                        ordPayment.TotalAmount = debitMoneyValue;
//                        ordPayList.Add(ordPayment);
//                    }

//                    //Karttan çeliken tutarda kargo tutarının oran hesaplaması
//                    OrderPayments ordPaymentKB = new OrderPayments();
//                    ordPaymentKB.CreatedOnUtc = DateTime.Now;
//                    ordPaymentKB.OrderId = order.Id;
//                    ordPaymentKB.OrderItemId = "KB";
//                    ordPaymentKB.Amount = (debitMoneyValue * order.OrderShippingInclTax) / order.OrderTotal;
//                    ordPaymentKB.TotalAmount = debitMoneyValue;

//                    ordPayList.Add(ordPaymentKB);
//                    _orderPaymentService.Insert(ordPayList);

//                    SendEmailNotification(order);

//                    //return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
//                    return RedirectToAction("Callback", Success(order.Id));

//                }
//                else
//                {
//                    _orderService.InsertOrderNote(new OrderNote
//                    {
//                        Note = "Banka ödemesi başarısız döndü : " + response.ResponseMessage,
//                        DisplayToCustomer = false,
//                        CreatedOnUtc = DateTime.UtcNow,
//                        OrderId = order.Id,
//                    });

//                    PaymentOptionModel model = new PaymentOptionModel();
//                    model.TotalDebit = debitMoneyValue;
//                    model.OrderGuide = order.OrderGuid.ToString() + "_" + DateTime.Now.Minute.ToString();

//                    return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
//                    //return RedirectToRoute("OrderDetails", new { orderId = order.Id });
//                }
//                //return RedirectToRoute("HomePage");
//            }
//            else
//            {
//                _orderService.InsertOrderNote(new OrderNote
//                {
//                    Note = order.OrderNumber.ToString() + " numaralı siparişin girilen kart bilgileri hash ile doğurlanamadı : " + JsonConvert.SerializeObject(form["errorMessage"]),
//                    DisplayToCustomer = false,
//                    CreatedOnUtc = DateTime.UtcNow,
//                    OrderId = order.Id,
//                });

//                PaymentOptionModel model = new PaymentOptionModel();
//                model.TotalDebit = debitMoneyValue;
//                model.OrderGuide = order.OrderGuid.ToString() + "_" + DateTime.Now.Minute.ToString();

//                return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
//            }
//        }

//        [IgnoreAntiforgeryToken]
//        [IngoreAllAttributes]
//        [HttpPost(nameof(CancelOrder))]
//        public IActionResult CancelOrder(IFormCollection form)
//        {
//            string returnRouteName = "";
//            if (form != null)
//            {
//                int pageType = 0;//1 = 3Dden gelen, 2 = Sayfa iptalinden Gelen
//                string[] orderGuide = new string[2];
//                if (!string.IsNullOrEmpty(form["errorMessage"]))
//                {
//                    orderGuide = form["orderId"].ToString().Split("_");
//                    pageType = 1;
//                }
//                else
//                {
//                    string[] tempGuide = form["OrderGuide"].ToString().Split("_");
//                    orderGuide[0] = tempGuide[0];
//                    pageType = 2;
//                }

//                Order order = _orderService.GetOrderByGuid(new Guid(orderGuide[0]));
//                if (order != null)
//                {
//                    if (pageType == 1)
//                    {
//                        var customer = _customerService.GetCustomerById(order.CustomerId);
//                        var selectedPointPaidVal = decimal.Parse(customer.GenericAttributes.Where(x => x.Key == "SelectedPaymentPoint").FirstOrDefault().Value.Replace(".", ","));
//                        var availeblePoint = _processService.GetCustomerActualPoint(customer.Username);

//                        //Sipariş tutarı puana çevirilerek seçilen puan miktarı kontrol ediliyor.
//                        var debitPointValue = (availeblePoint - decimal.Parse(MoneyToPoint.CalculatePoint(order.OrderTotal)));

//                        var setting = _settingService.GetSetting("payment.moneypointfactor");
//                        var debitMoneyValue = ((debitPointValue * -1) * decimal.Parse(setting.Value));
//                        var errorMessage = form["errorMessage"].ToString().Replace("\"", "").Replace("[\"", "").Replace("\"]", "");
//                        _orderService.InsertOrderNote(new OrderNote
//                        {
//                            Note = debitMoneyValue.ToString() + " TL için alınan hata bilgisi : " + errorMessage,
//                            DisplayToCustomer = false,
//                            CreatedOnUtc = DateTime.UtcNow,
//                            OrderId = order.Id,
//                        });

//                        PaymentOptionModel model = new PaymentOptionModel();
//                        model.TotalDebit = debitMoneyValue;
//                        model.OrderGuide = order.OrderGuid.ToString() + "_" + DateTime.Now.Minute.ToString();

//                        //return View("~/Plugins/Payments.PaymentOptions/Views/PaymentOptions.cshtml", model);
                        
//                        return Error(errorMessage);
//                    }
//                    else if (pageType == 2)
//                    {
//                        _orderProcessingService.CancelOrder(order, false);
//                        return Error("Sipariş iptal edildi.");
//                    }
//                }
//            }
//            return RedirectToRoute(returnRouteName);
//        }
//        #endregion
//    }
//}