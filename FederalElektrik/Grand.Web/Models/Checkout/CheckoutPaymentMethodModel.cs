﻿using System.Collections.Generic;
using Grand.Framework.Mvc.Models;

namespace Grand.Web.Models.Checkout
{
    public partial class CheckoutPaymentMethodModel : BaseGrandModel
    {
        public CheckoutPaymentMethodModel()
        {
            Warnings = new List<string>();
            PaymentMethods = new List<PaymentMethodModel>();
        }

        public IList<PaymentMethodModel> PaymentMethods { get; set; }

        public bool DisplayRewardPoints { get; set; }
        public decimal RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public bool RewardPointsEnoughToPayForOrder { get; set; }
        public bool UseRewardPoints { get; set; }

        public int MoneyPointFactor { get; set; }
        public int TotalPoint { get; set; }
        public int CustomerMoney { get; set; }

        public IList<string> Warnings { get; set; }

        #region Nested classes

        public partial class PaymentMethodModel : BaseGrandModel
        {
            public string PaymentMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }
            public string LogoUrl { get; set; }
        }
        #endregion
    }
}