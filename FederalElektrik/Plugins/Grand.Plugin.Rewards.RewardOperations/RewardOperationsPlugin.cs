using Grand.Core;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Points;
using Grand.Core.Plugins;
using Grand.Plugin.Rewards.RewardOperations.Controllers;
using Grand.Plugin.Rewards.RewardOperations.Helper;
using Grand.Plugin.Rewards.RewardOperations.Models;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Orders;
using Grand.Services.Payments;
using Grand.Services.Points;
using Grand.Services.Stores;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Plugin.Rewards.RewardOperations
{
    public class RewardOperationsPlugin : BasePlugin, IPaymentMethod
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;

        private readonly IProductService _productService;
        private readonly IRewardPointsService _rewardPointsService;
        private readonly IProcessService _processService;
        private IGenericAttributeService _genericService;

        public RewardOperationsPlugin
            (
            ILocalizationService localizationService,
            IWebHelper webHelper,
            LocalizationSettings localizationSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IOrderService orderService,
            IProductService productService,
            IRewardPointsService rewardPointsService,
            IStoreContext storeContext,
            IProcessService processService,
            IGenericAttributeService genericService
            )
        {
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._localizationSettings = localizationSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._orderService = orderService;

            this._productService = productService;
            this._rewardPointsService = rewardPointsService;
            this._storeContext = storeContext;
            this._processService = processService;
            this._genericService = genericService;
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();

            if (_workContext.CurrentCustomer.HasShoppingCartItems)
            {
                var shoppingCart = _workContext.CurrentCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart);

                decimal pointBalanceTotal = _processService.GetCustomerActualPoint(_workContext.CurrentCustomer.Username);

                if (pointBalanceTotal >= processPaymentRequest.OrderTotal)
                {
                    pointBalanceTotal = pointBalanceTotal - Math.Round(processPaymentRequest.OrderTotal);
                    Process _proc = new Process()
                    {
                        Description = processPaymentRequest.OrderGuid.ToString() + " guidli sipariş için düşülen puan.",
                        OrderGuid = processPaymentRequest.OrderGuid,
                        OrderNumber = 0,
                        CreatedOnUtc = DateTime.UtcNow,
                        Point = (processPaymentRequest.OrderTotal * -1),
                        TypeId = (int)ProcessType.Spend,
                        Username = _workContext.CurrentCustomer.Username,
                    };
                   var procc = _processService.Insert(_proc);

                    _rewardPointsService.RemoveOldPointAndAddNewPoints(_workContext.CurrentCustomer.Id, pointBalanceTotal, _storeContext.CurrentStore.Id, string.Empty);

                    processPaymentResult.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                }
                else
                {
                    processPaymentResult.AddError("Üzgünüz, bakiye işlemi gerçekleştirilemedi! Lütfen bakiyenizi kontrol edin veya daha sonra tekrar deneyin.");
                }

                //List<OrderItemPointModel> orderItemPoint = new List<OrderItemPointModel>();
                ////foreach (var orderItem in shoppingCart)
                ////{
                ////    orderItemPoint.Add(new OrderItemPointModel
                ////    {
                ////        OrderItemPoint = _productService.GetProductById(orderItem.ProductId).Price * orderItem.Quantity,
                ////        SupplierProcessId = orderItem.ProductId,
                ////        SecondaryProcessId = orderItem.Id,
                ////        Message = string.Format("[{0}] nolu sipariş için düşülen puan", processPaymentRequest.OrderGuid)
                ////    });
                ////}

                //orderItemPoint.Add(new OrderItemPointModel
                //{
                //    OrderItemPoint = processPaymentRequest.OrderTotal,
                //    SupplierProcessId = processPaymentRequest.OrderGuid.ToString(),
                //    SecondaryProcessId = processPaymentRequest.OrderGuid.ToString(),
                //    Message = string.Format("[{0}] nolu sipariş için düşülen puan", processPaymentRequest.OrderGuid)
                //});

                //PointSpendingModel pointSpendingModel = new PointSpendingModel
                //{
                //    FirstProcessId = processPaymentRequest.OrderGuid.ToString(),
                //    OrderPoint = processPaymentRequest.OrderTotal,
                //    Username = _workContext.CurrentCustomer.Username,
                //    OrderItems = orderItemPoint
                //};

                //RewardUserOperations rewardUserOperations = new RewardUserOperations(_localizationService, _localizationSettings, _logger, _settingService, _storeService, _workContext);
                //ProcessResponseModel result = rewardUserOperations.SetSpendingPoint(pointSpendingModel);
                //if (!result.Success)
                //{
                //    processPaymentResult.AddError("Üzgünüz, bakiye işlemi gerçekleştirilemedi! Lütfen bakiyenizi kontrol edin veya daha sonra tekrar deneyin.");
                //}
                //else
                //{
                //    var pointBalanceTotal = _rewardPointsService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                //    if (pointBalanceTotal > 0)
                //        _rewardPointsService.RemoveOldPointAndAddNewPoints(_workContext.CurrentCustomer.Id, (pointBalanceTotal - pointSpendingModel.OrderPoint), _storeContext.CurrentStore.Id, string.Empty);

                //    processPaymentResult.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                //}
            }

            return processPaymentResult;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //buradan aldım kodları
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return decimal.Parse("0");
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult();
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {

            return new CancelRecurringPaymentResult();
        }

        public bool CanRePostProcessPayment(Order order)
        {
            return false;
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            IList<string> warnings = new List<string>();
            return warnings;
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentRewardPoints";
        }

        public Type GetControllerType()
        {
            return typeof(RewardOperationsController);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/RewardOperations/Configure";
        }




        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return true;
            }
        }

        public string PaymentMethodDescription
        {
            get
            {
                return _localizationService.GetResource("Plugins.Rewards.RewardOperations.PaymentMethodDescription");
            }
        }

        #endregion
    }
}
