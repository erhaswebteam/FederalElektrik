using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Messages;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Domain.Shipping;
using Grand.Core.Domain.Tax;
using Grand.Core.Http;
using Grand.Core.Infrastructure;
using Grand.Core.Plugins;
using Grand.Framework.Controllers;
using Grand.Framework.Mvc.Filters;
using Grand.Framework.Security;
using Grand.Framework.Security.Captcha;
using Grand.Plugin.Rewards.RewardOperations.Helper;
using Grand.Plugin.Rewards.RewardOperations.Models;
using Grand.Services.Authentication;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Events;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Messages;
using Grand.Services.Orders;
using Grand.Services.Payments;
using Grand.Services.Points;
using Grand.Services.Shipping;
using Grand.Services.Stores;
using Grand.Services.Tax;
using Grand.Services.Topics;
using Grand.Web.CustomFilters;
using Grand.Web.Extensions;
using Grand.Web.Models.Checkout;
using Grand.Web.Models.Customer;
using Grand.Web.Models.Order;
using Grand.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Grand.Plugin.Rewards.RewardOperations.Controllers
{
    public class RewardOperationsController : BasePaymentController
    {
        #region Const
        private readonly ICustomerWebService _customerWebService;
        private readonly IGrandAuthenticationService _authenticationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ITaxService _taxService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICountryService _countryService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressWebService _addressWebService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ILogger _logger;
        private readonly IRewardPointsService _rewardPointsService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;

        private readonly IShippingService _shippingService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPluginFinder _pluginFinder;

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly RewardPointsSettings _rewardPointsSettings;

        private readonly ICheckoutWebService _checkoutWebService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly OrderSettings _orderSettings;
        private readonly ITopicWebService _topicWebService;
        private readonly ITopicService _topicService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IProcessService _processService;



        public RewardOperationsController
            (
            ICustomerWebService customerWebService,
            IGrandAuthenticationService authenticationService,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService customerService,
            ICustomerAttributeParser customerAttributeParser,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ITaxService taxService,
            CustomerSettings customerSettings,
            ICountryService countryService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IShoppingCartService shoppingCartService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IAddressWebService addressWebService,
            IEventPublisher eventPublisher,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            ILogger logger,
            IRewardPointsService rewardPointsService,
            ISettingService settingService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            RewardPointsSettings rewardPointsSettings,
            ICheckoutWebService checkoutWebService,
            IPaymentService paymentService,
            IOrderService orderService,
            OrderSettings orderSettings,
            IShippingService shippingService,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            IPluginFinder pluginFinder,
            ITopicWebService topicWebService,
            ITopicService topicService,
            IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IProcessService processService
            )
        {
            this._pluginFinder = pluginFinder;

            this._shippingService = shippingService;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;

            this._checkoutWebService = checkoutWebService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderSettings = orderSettings;

            this._customerWebService = customerWebService;
            this._authenticationService = authenticationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._customerAttributeParser = customerAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._taxService = taxService;
            this._customerSettings = customerSettings;
            this._countryService = countryService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._shoppingCartService = shoppingCartService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._addressWebService = addressWebService;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._eventPublisher = eventPublisher;
            this._logger = logger;
            this._rewardPointsService = rewardPointsService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._dateTimeHelper = dateTimeHelper;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._rewardPointsSettings = rewardPointsSettings;
            this._topicWebService = topicWebService;
            this._topicService = topicService;
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._processService = processService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected IShippingRateComputationMethod GetShippingComputation(string input)
        {
            var shippingMethodName = input.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries)[1];
            var shippingMethod = _shippingService.LoadShippingRateComputationMethodBySystemName(shippingMethodName);
            if (shippingMethod == null)
                throw new Exception("Shipping method is not selected");

            return shippingMethod;
        }

        [NonAction]
        protected void ValidateShippingForm(IFormCollection form, out List<string> warnings)
        {
            warnings = GetShippingComputation(form["shippingoption"]).ValidateShippingForm(form).ToList();
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
        }

        #endregion

        #region Methods

        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var model = _customerWebService.PrepareLogin(checkoutAsGuest);
            return View("~/Plugins/Rewards.RewardOperations/Views/Login.cshtml", model);
        }

        [HttpPost]
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        [ValidateCaptcha]
        [PublicAntiForgery]
        public virtual IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            _logger.Information("Login Request Girişi", new GrandException(JsonConvert.SerializeObject(model)));

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    //Kullanıcı zaten içeride var ise...
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

                            decimal actualPoint = _processService.GetCustomerActualPoint(customer.Username);

                            //Kullanıcının mongodaki tüm puan geçmişi siliniyor ve son puanı yükleniyor...
                            _rewardPointsService.RemoveOldPointAndAddNewPoints(customer.Id, actualPoint, _storeContext.CurrentStore.Id, "Güncel Banka Puanı (Giriş)");

                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            _authenticationService.SignIn(customer, model.RememberMe);

                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            _customerActivityService.InsertActivity("PublicStore.Login", "", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            _logger.Information("Login > Login Successful, Redirecting HomePage", new GrandException(JsonConvert.SerializeObject("Başarılı giriş")), customer);
                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View("~/Plugins/Rewards.RewardOperations/Views/Login.cshtml", model);
        }

        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public virtual IActionResult AdminLogin(bool? checkoutAsGuest)
        {
            var model = _customerWebService.PrepareLogin(checkoutAsGuest);
            return View("~/Plugins/Rewards.RewardOperations/Views/AdminLogin.cshtml", model);
        }

        [HttpPost]
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        [ValidateCaptcha]
        [PublicAntiForgery]
        public virtual IActionResult AdminLogin(LoginModel model, string returnUrl, bool captchaValid)
        {
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

                            if (!customer.IsAdmin() && !customer.IsForumModerator())
                            {
                                ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotAdminAccount"));
                                break;
                            }

                            decimal actualPoint = _processService.GetCustomerActualPoint(customer.Username);
                            //Kullanıcının mongodaki tüm puan geçmişi siliniyor ve son puanı yükleniyor...
                            _rewardPointsService.RemoveOldPointAndAddNewPoints(customer.Id, actualPoint, _storeContext.CurrentStore.Id, "Güncel Banka Puanı (Giriş)");

                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            _authenticationService.SignIn(customer, model.RememberMe);

                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            _customerActivityService.InsertActivity("PublicStore.Login", "", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);


                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View("~/Plugins/Rewards.RewardOperations/Views/AdminLogin.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            //var payPalStandardPaymentSettings = _settingService.LoadSetting<RewardOperationsSettings>(storeScope);
            var settings = _settingService.LoadSetting<RewardOperationsSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AddressGetUser = settings.AddressGetUser;
            model.AddressPointSpending = settings.AddressPointSpending;
            model.ApiToken = settings.ApiToken;
            model.ProjectPreChars = settings.ProjectPreChars;

            if (!string.IsNullOrEmpty(storeScope))
            {
                model.AddressGetUser_OverrideForStore = _settingService.SettingExists(settings, setting => setting.AddressGetUser, storeScope);
                model.AddressPointSpending_OverrideForStore = _settingService.SettingExists(settings, setting => setting.AddressPointSpending, storeScope);
                model.ApiToken_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ApiToken, storeScope);
                model.ProjectPreChars_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ProjectPreChars, storeScope);
            }

            return View("~/Plugins/Rewards.RewardOperations/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var _rewardOperationsSettings = _settingService.LoadSetting<RewardOperationsSettings>(storeScope);

            //save settings
            _rewardOperationsSettings.AddressGetUser = model.AddressGetUser;
            _rewardOperationsSettings.AddressPointSpending = model.AddressPointSpending;
            _rewardOperationsSettings.ApiToken = model.ApiToken;
            _rewardOperationsSettings.ProjectPreChars = model.ProjectPreChars;

            if (!String.IsNullOrEmpty(model.AddressGetUser) || String.IsNullOrEmpty(storeScope))
                _settingService.SaveSetting(_rewardOperationsSettings, x => x.AddressGetUser, storeScope, false);
            else if (!String.IsNullOrEmpty(storeScope))
                _settingService.DeleteSetting(_rewardOperationsSettings, x => x.AddressGetUser, storeScope);

            if (!String.IsNullOrEmpty(model.AddressPointSpending) || String.IsNullOrEmpty(storeScope))
                _settingService.SaveSetting(_rewardOperationsSettings, x => x.AddressPointSpending, storeScope, false);
            else if (!String.IsNullOrEmpty(storeScope))
                _settingService.DeleteSetting(_rewardOperationsSettings, x => x.AddressPointSpending, storeScope);

            if (!String.IsNullOrEmpty(model.ApiToken) || String.IsNullOrEmpty(storeScope))
                _settingService.SaveSetting(_rewardOperationsSettings, x => x.ApiToken, storeScope, false);
            else if (!String.IsNullOrEmpty(storeScope))
                _settingService.DeleteSetting(_rewardOperationsSettings, x => x.ApiToken, storeScope);

            if (!String.IsNullOrEmpty(model.ProjectPreChars) || String.IsNullOrEmpty(storeScope))
                _settingService.SaveSetting(_rewardOperationsSettings, x => x.ProjectPreChars, storeScope, false);
            else if (!String.IsNullOrEmpty(storeScope))
                _settingService.DeleteSetting(_rewardOperationsSettings, x => x.ProjectPreChars, storeScope);

            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //My account / Reward points
        public virtual IActionResult CustomerRewardPointsHistories()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            var rewardPointsSettings = EngineContext.Current.Resolve<RewardOperationsSettings>();
            if (!rewardPointsSettings.EnableRewardPointPluginHistory)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;
            var model = PrepareCustomerRewardPoints(customer);
            return View("~/Plugins/Rewards.RewardOperations/Views/CustomerRewardPoints.cshtml", model);
        }

        public virtual CustomerRewardPointsModel PrepareCustomerRewardPoints(Core.Domain.Customers.Customer customer)
        {
            RewardUserOperations rewardUserOperations = new RewardUserOperations(_localizationService, _localizationSettings, _logger, _settingService, _storeService, _workContext);
            GetAllPointResponseModel responseModel = new GetAllPointResponseModel();

            //Kullanıcıyı serviste sorguluyorum...
            responseModel = rewardUserOperations.GetAllPointByCustomer(customer.Username);

            var model = new CustomerRewardPointsModel();
            if (responseModel != null && responseModel.PointDetails != null)
            {
                foreach (var rph in responseModel.PointDetails)
                {
                    string message = rph.Message;
                    string orderGuid = string.Empty;

                    if (!string.IsNullOrWhiteSpace(message) && message.Contains('[') && message.Contains(']'))
                        orderGuid = message.Substring(message.IndexOf('[') + 1, message.IndexOf(']') - 1);

                    Order order = null;
                    string orderNo = string.Empty;

                    if (!string.IsNullOrWhiteSpace(orderGuid))
                    {
                        order = _orderService.GetOrderByGuid(Guid.Parse(orderGuid));
                        if (order != null)
                            orderNo = order.OrderNumber.ToString();
                    }

                    model.RewardPoints.Add(new CustomerRewardPointsModel.RewardPointsHistoryModel
                    {
                        Points = rph.Point,
                        PointsBalance = rph.Point,
                        Message = !string.IsNullOrWhiteSpace(orderNo) ? message.Replace(string.Format("[{0}]", orderGuid), string.Format("[{0}]", orderNo)) : message,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(rph.CreatedDate, DateTimeKind.Utc)
                    });
                }
            }
            //current amount/balance
            decimal rewardPointsBalance = _rewardPointsService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
            model.RewardPointsBalance = rewardPointsBalance;
            model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
            //minimum amount/balance
            int minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
            decimal minimumRewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(minimumRewardPointsBalance);
            decimal minimumRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minimumRewardPointsAmountBase, _workContext.WorkingCurrency);
            model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
            model.MinimumRewardPointsAmount = _priceFormatter.FormatPrice(minimumRewardPointsAmount, true, false);

            return model;
        }



        [NonAction]
        protected JsonResult OpcLoadStepAfterShippingAddress(List<ShoppingCartItem> cart)
        {
            var shippingMethodModel = _checkoutWebService.PrepareShippingMethod(cart, _workContext.CurrentCustomer.ShippingAddress);

            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                shippingMethodModel.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedShippingOption,
                    shippingMethodModel.ShippingMethods.First().ShippingOption,
                    _storeContext.CurrentStore.Id);

                //load next step
                return OpcLoadStepAfterShippingMethod(cart);
            }


            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "shipping-method",
                    html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcShippingMethods.cshtml", shippingMethodModel)
                },
                goto_section = "shipping_method"
            });
        }

        [NonAction]
        protected JsonResult OpcLoadStepAfterShippingMethod(List<ShoppingCartItem> cart)
        {
            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            bool isPaymentWorkflowRequired = _checkoutWebService.IsPaymentWorkflowRequired(cart, false);
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                string filterByCountryId = "";
                if (_addressWebService.AddressSettings().CountryEnabled &&
                    _workContext.CurrentCustomer.BillingAddress != null &&
                    !String.IsNullOrEmpty(_workContext.CurrentCustomer.BillingAddress.CountryId))
                {
                    filterByCountryId = _workContext.CurrentCustomer.BillingAddress.CountryId;
                }

                //payment is required
                var paymentMethodModel = _checkoutWebService.PreparePaymentMethod(cart, filterByCountryId);

                if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                    //so customer doesn't have to choose a payment method

                    var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod,
                        selectedPaymentMethodSystemName, _storeContext.CurrentStore.Id);

                    var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                    if (paymentMethodInst == null ||
                        !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                        !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                        throw new Exception("Selected payment method can't be parsed");

                    return OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                }

                //customer have to choose a payment method
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-method",
                        html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcPaymentMethods.cshtml", paymentMethodModel)
                    },
                    goto_section = "payment_method"
                });
            }

            //payment is not required
            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);

            var confirmOrderModel = _checkoutWebService.PrepareConfirmOrder(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "confirm-order",
                    html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcConfirmOrder.cshtml", confirmOrderModel)
                },
                goto_section = "confirm_order"
            });
        }

        [NonAction]
        protected JsonResult OpcLoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, List<ShoppingCartItem> cart)
        {
            if (paymentMethod.SkipPaymentInfo ||
                    (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection
                    && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();
                //session save
                this.HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                var confirmOrderModel = _checkoutWebService.PrepareConfirmOrder(cart);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcConfirmOrder.cshtml", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }


            //return payment info page
            var paymenInfoModel = _checkoutWebService.PreparePaymentInfo(paymentMethod);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "payment-info",
                    html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcPaymentInfo.cshtml", paymenInfoModel)
                },
                goto_section = "payment_info"
            });
        }

        public virtual IActionResult OnePageCheckout()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return Challenge();

            var model = new OnePageCheckoutModel
            {
                ShippingRequired = cart.RequiresShipping(),
                DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep,
                BillingAddress = _checkoutWebService.PrepareBillingAddress(cart, prePopulateNewAddressWithCustomerFields: true)
            };
            return View("~/Plugins/Rewards.RewardOperations/Views/OnePageCheckout.cshtml", model);
        }

        public virtual IActionResult OpcSaveBilling(IFormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                string billingAddressId = form["billing_address_id"];

                if (!String.IsNullOrEmpty(billingAddressId))
                {
                    //existing address
                    var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == billingAddressId);
                    if (address == null)
                        throw new Exception("Address can't be loaded");

                    _workContext.CurrentCustomer.BillingAddress = address;
                    address.CustomerId = _workContext.CurrentCustomer.Id;
                    _customerService.UpdateBillingAddress(address);
                }
                else
                {
                    //new address
                    var model = new CheckoutBillingAddressModel();
                    TryUpdateModelAsync(model.NewAddress, "BillingNewAddress");

                    //custom address attributes
                    var customAttributes = _addressWebService.ParseCustomAddressAttributes(form);
                    var customAttributeWarnings = _addressWebService.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    TryValidateModel(model.NewAddress);
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var billingAddressModel = _checkoutWebService.PrepareBillingAddress(cart,
                                                    selectedCountryId: model.NewAddress.CountryId,
                                                    overrideAttributesXml: customAttributes);
                        billingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "billing",
                                html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcBillingAddress.cshtml", billingAddressModel)
                            },
                            wrong_billing_address = true,
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                        model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                        model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                        model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                        model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                        model.NewAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = model.NewAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        _workContext.CurrentCustomer.Addresses.Add(address);
                        address.CustomerId = _workContext.CurrentCustomer.Id;
                        _customerService.InsertAddress(address);
                    }
                    _workContext.CurrentCustomer.BillingAddress = address;
                    address.CustomerId = _workContext.CurrentCustomer.Id;
                    _customerService.UpdateBillingAddress(address);
                }

                if (cart.RequiresShipping())
                {
                    //shipping is required

                    var model = new CheckoutBillingAddressModel();
                    TryUpdateModelAsync(model);
                    if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress)
                    {
                        _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                        _customerService.UpdateShippingAddress(_workContext.CurrentCustomer.BillingAddress);
                        _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, "", _storeContext.CurrentStore.Id);
                        return OpcLoadStepAfterShippingAddress(cart);
                    }
                    else
                    {
                        var shippingAddressModel = _checkoutWebService.PrepareShippingAddress(prePopulateNewAddressWithCustomerFields: true);
                        if (_shippingSettings.AllowPickUpInStore && _shippingService.LoadActiveShippingRateComputationMethods(_storeContext.CurrentStore.Id).Count == 0)
                        {
                            shippingAddressModel.PickUpInStoreOnly = true;
                            shippingAddressModel.PickUpInStore = true;
                        }

                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "shipping",
                                html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcShippingAddress.cshtml", shippingAddressModel)
                            },
                            goto_section = "shipping"
                        });
                    }

                }
                //shipping is not required
                _workContext.CurrentCustomer.ShippingAddress = null;
                _customerService.RemoveShippingAddress(_workContext.CurrentCustomer.Id);
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                //load next step
                return OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual IActionResult OpcSaveShipping(CheckoutShippingAddressModel model, IFormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");

                //Pick up in store?
                if (_shippingSettings.AllowPickUpInStore)
                {

                    if (model.PickUpInStore)
                    {
                        //customer decided to pick up in store

                        //no shipping address selected
                        _workContext.CurrentCustomer.ShippingAddress = null;
                        _customerService.RemoveShippingAddress(_workContext.CurrentCustomer.Id);

                        //clear shipping option XML/Description
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.ShippingOptionAttributeXml, "", _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.ShippingOptionAttributeDescription, "", _storeContext.CurrentStore.Id);


                        var pickupPoint = form["pickup-point-id"];
                        var pickupPoints = _shippingService.LoadActivePickupPoints(_storeContext.CurrentStore.Id);
                        var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint));
                        if (selectedPoint == null)
                            throw new Exception("Pickup point is not allowed");

                        //save "pick up in store" shipping method
                        var pickUpInStoreShippingOption = new ShippingOption
                        {
                            Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                            Rate = selectedPoint.PickupFee,
                            Description = selectedPoint.Description,
                            ShippingRateComputationMethodSystemName = string.Format("PickupPoint_{0}", selectedPoint.Id)
                        };

                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedShippingOption,
                        pickUpInStoreShippingOption,
                        _storeContext.CurrentStore.Id);

                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPickupPoint,
                        selectedPoint.Id,
                        _storeContext.CurrentStore.Id);

                        //load next step
                        return OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, "", _storeContext.CurrentStore.Id);

                }

                string shippingAddressId = form["shipping_address_id"];

                if (!String.IsNullOrEmpty(shippingAddressId))
                {
                    //existing address
                    var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == shippingAddressId);
                    if (address == null)
                        throw new Exception("Address can't be loaded");

                    _workContext.CurrentCustomer.ShippingAddress = address;
                    address.CustomerId = _workContext.CurrentCustomer.Id;
                    _customerService.UpdateShippingAddress(address);
                }
                else
                {
                    //new address
                    TryUpdateModelAsync(model.NewAddress, "ShippingNewAddress");

                    //custom address attributes
                    var customAttributes = _addressWebService.ParseCustomAddressAttributes(form);
                    var customAttributeWarnings = _addressWebService.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    TryValidateModel(model.NewAddress);
                    if (!ModelState.IsValid)
                    {
                        var xx = ModelState.Values.SelectMany(v => v.Errors);
                        foreach (var item in xx)
                        {
                            string tt = item.ErrorMessage;
                        }
                        //model is not valid. redisplay the form with errors
                        var shippingAddressModel = _checkoutWebService.PrepareShippingAddress(selectedCountryId: model.NewAddress.CountryId, overrideAttributesXml: customAttributes);
                        shippingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "shipping",
                                html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcShippingAddress.cshtml", shippingAddressModel)
                            }
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                        model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                        model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                        model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                        model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                        model.NewAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        address = model.NewAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;

                        //other null validations
                        _workContext.CurrentCustomer.Addresses.Add(address);
                        address.CustomerId = _workContext.CurrentCustomer.Id;
                        _customerService.InsertAddress(address);
                    }
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    address.CustomerId = _workContext.CurrentCustomer.Id;
                    _customerService.UpdateShippingAddress(address);
                }

                return OpcLoadStepAfterShippingAddress(cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual IActionResult OpcSaveShippingMethod(IFormCollection form)
        {
            try
            {
                //validation
                var customer = _workContext.CurrentCustomer;
                var store = _storeContext.CurrentStore;

                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                    .LimitPerStore(store.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");


                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((customer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");

                //parse selected method 
                string shippingoption = form["shippingoption"];
                if (String.IsNullOrEmpty(shippingoption))
                    throw new Exception("Selected shipping method can't be parsed");
                var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    throw new Exception("Selected shipping method can't be parsed");
                string selectedName = splittedOption[0];
                string shippingRateComputationMethodSystemName = splittedOption[1];

                //clear shipping option XML/Description
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ShippingOptionAttributeXml, "", store.Id);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ShippingOptionAttributeDescription, "", store.Id);

                //validate customer's input
                List<string> warnings;
                ValidateShippingForm(form, out warnings);

                //find it
                //performance optimization. try cache first
                var shippingOptions = customer.GetAttribute<List<ShippingOption>>(SystemCustomerAttributeNames.OfferedShippingOptions, store.Id);
                if (shippingOptions == null || shippingOptions.Count == 0)
                {
                    //not found? let's load them using shipping service
                    shippingOptions = _shippingService
                        .GetShippingOptions(customer, cart, customer.ShippingAddress, shippingRateComputationMethodSystemName, store.Id)
                        .ShippingOptions
                        .ToList();
                }
                else
                {
                    //loaded cached results. let's filter result by a chosen shipping rate computation method
                    shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var shippingOption = shippingOptions
                    .Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase));
                if (shippingOption == null)
                    throw new Exception("Selected shipping method can't be loaded");

                //save
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, store.Id);

                if (ModelState.IsValid)
                {
                    //load next step
                    return OpcLoadStepAfterShippingMethod(cart);
                }

                var message = String.Join(", ", warnings.ToArray());
                return Json(new { error = 1, message = message });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual IActionResult OpcSavePaymentMethod(IFormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                string paymentmethod = form["paymentmethod"];
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var model = new CheckoutPaymentMethodModel();
                TryUpdateModelAsync(model);

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);
                }

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = _checkoutWebService.IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);

                    var confirmOrderModel = _checkoutWebService.PrepareConfirmOrder(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcConfirmOrder.cshtml", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);

                return OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual IActionResult OpcSavePaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    throw new Exception("Payment method is not selected");

                var warnings = paymentMethod.ValidatePaymentForm(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = paymentMethod.GetPaymentInfo(form);
                    //session save
                    this.HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                    var confirmOrderModel = _checkoutWebService.PrepareConfirmOrder(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcConfirmOrder.cshtml", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = _checkoutWebService.PreparePaymentInfo(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcPaymentInfo.cshtml", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual IActionResult OpcConfirmOrder([FromServices] IOrderProcessingService orderProcessingService)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!_checkoutWebService.IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = this.HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (_checkoutWebService.IsPaymentWorkflowRequired(cart))
                    {
                        throw new Exception("Payment information is not entered");
                    }
                    else
                        processPaymentRequest = new ProcessPaymentRequest();
                }

                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _storeContext.CurrentStore.Id);
                var placeOrderResult = orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    this.HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };


                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 });

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        return Json(new
                        {
                            redirect = string.Format("{0}checkout/OpcCompleteRedirectionPayment", _webHelper.GetStoreLocation())
                        });
                    }

                    //_paymentService.PostProcessPayment(postProcessPaymentRequest);
                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    Order _order = _orderService.GetOrderById(postProcessPaymentRequest.Order.Id);
                    if (_order.OrderStatus != OrderStatus.Processing)
                    {
                        placeOrderResult.Errors.Add(_localizationService.GetLocaleStringResourceByName("Payment.Rewards.RewardOperations.NotPointSpendingErrorText", _workContext.WorkingLanguage.Id).ResourceValue);
                    }
                    //success
                    return Json(new { success = 1 });
                }

                //error
                var confirmOrderModel = new CheckoutConfirmModel();
                foreach (var error in placeOrderResult.Errors)
                    confirmOrderModel.Warnings.Add(error);

                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = this.RenderPartialViewToString("~/Plugins/Rewards.RewardOperations/Views/OpcConfirmOrder.cshtml", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual IActionResult OpcCompleteRedirectionPayment()
        {
            try
            {
                //validation
                if (!_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("HomePage");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    return Challenge();

                //get the order
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order == null)
                    return RedirectToRoute("HomePage");


                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("HomePage");
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    return RedirectToRoute("HomePage");

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                    return RedirectToRoute("HomePage");


                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };

                _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content("Redirected");
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Content(exc.Message);
            }
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [PublicAntiForgery]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View("~/Plugins/Rewards.RewardOperations/Views/PasswordRecoveryConfirm.cshtml", model);
            }

            if (ModelState.IsValid)
            {
                string username = customer.Username;

                RewardUserOperations rewardUserOperations = new RewardUserOperations(_localizationService, _localizationSettings, _logger, _settingService, _storeService, _workContext);

                var serviceCustomer = rewardUserOperations.GetCustomerByUsername(customer.Username);
                if (serviceCustomer != null && serviceCustomer.Success)
                {
                    var result = rewardUserOperations.SetCustomerPassword(username, model.NewPassword);
                    if (result != null && result.Success)
                    {
                        var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                            false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
                        if (response != null && response.Success)
                        {
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, "");

                            model.DisablePasswordChanging = true;
                            model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                        }
                        else
                        {
                            model.Result = response.Errors.FirstOrDefault();
                        }
                    }
                    else
                        ModelState.AddModelError("", _localizationService.GetResource("Account.RewardOperations.ChangePassword.NotSuccess"));
                }
                else
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Account.ServiceMessage.UserNotInPointBank"));
                }

                return View("~/Plugins/Rewards.RewardOperations/Views/PasswordRecoveryConfirm.cshtml", model);
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Rewards.RewardOperations/Views/PasswordRecoveryConfirm.cshtml", model);
        }


        [HttpPost]
        public virtual JsonResult SendPass(string m)
        {
            string msg = "";
            if (!string.IsNullOrEmpty(m))
            {
                Core.Domain.Customers.Customer cus = _customerService.GetCustomerByEmail(m);
                if (cus != null && cus.Active && !cus.Deleted)
                {
                    try
                    {
                        var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                        if (emailAccount == null)
                            emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                        if (emailAccount == null)
                            throw new GrandException("Email account can't be loaded");

                        MessageTemplate msgBody = _messageTemplateService.GetMessageTemplateByName("SendPassword", _storeContext.CurrentStore.Id);

                        string body = string.Format(msgBody.Body, cus.Username, cus.Password);

                        var email = new QueuedEmail
                        {
                            Priority = QueuedEmailPriority.High,
                            EmailAccountId = emailAccount.Id,
                            FromName = emailAccount.DisplayName,
                            From = emailAccount.Email,
                            ToName = cus.GetFullName(),
                            To = cus.Email,
                            Subject = msgBody.Subject,
                            Body = body,
                            CreatedOnUtc = DateTime.UtcNow,
                            DontSendBeforeDateUtc = null
                        };
                        _queuedEmailService.InsertQueuedEmail(email);
                        msg = "Giriş bilgileriniz mail adresinize iletildi.";
                    }
                    catch (Exception ex)
                    {
                        msg = "Mail gönderilirken bir hata oluştu!";
                    }
                }
                else
                {
                    msg = "Mail adresi bulunamadı!";
                }
            }
            return Json(msg);
        }


        #endregion



    }
}
