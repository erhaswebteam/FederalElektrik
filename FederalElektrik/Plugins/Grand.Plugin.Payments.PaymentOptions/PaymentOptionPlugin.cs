using Grand.Core;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Plugins;
using Grand.Plugin.Payments.PaymentOptions.Controllers;
using Grand.Services.Localization;
using Grand.Services.Payments;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Grand.Plugin.Payments.PaymentOptions
{
    public class PaymentOptionPlugin : BasePlugin, IPaymentMethod
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentOptionPlugin(
             IWebHelper webHelper,
             ILocalizationService localizationService,
             IHttpContextAccessor httpContextAccessor)
        {
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._httpContextAccessor = httpContextAccessor;
        }


        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var url = _httpContextAccessor.HttpContext.Request.Host + "/payment-options";
            _httpContextAccessor.HttpContext.Response.Redirect("https://" + url);
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentOptionsComp";
        }

        public Type GetControllerType()
        {
            return typeof(PaymentOptionsController);
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
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentOptions/Configure";
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return decimal.Parse("0");
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
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
            return new ProcessPaymentRequest();
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
            get { return PaymentMethodType.Redirection; }
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
                return _localizationService.GetResource("Plugins.Payments.PaymentOptions.PaymentOptionsDescription");
            }
        }

        #endregion
    }
}
