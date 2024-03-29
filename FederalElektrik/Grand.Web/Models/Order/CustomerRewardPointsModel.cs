﻿using System;
using System.Collections.Generic;
using Grand.Framework;
using Grand.Framework.Mvc.Models;
using Grand.Framework.Mvc.ModelBinding;

namespace Grand.Web.Models.Order
{
    public partial class CustomerRewardPointsModel : BaseGrandModel
    {
        public CustomerRewardPointsModel()
        {
            RewardPoints = new List<RewardPointsHistoryModel>();
        }

        public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
        public decimal RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public decimal MinimumRewardPointsBalance { get; set; }
        public string MinimumRewardPointsAmount { get; set; }

        #region Nested classes

        public partial class RewardPointsHistoryModel : BaseGrandEntityModel
        {
            [GrandResourceDisplayName("RewardPoints.Fields.Points")]
            public decimal Points { get; set; }

            [GrandResourceDisplayName("RewardPoints.Fields.PointsBalance")]
            public decimal PointsBalance { get; set; }

            [GrandResourceDisplayName("RewardPoints.Fields.Message")]
            public string Message { get; set; }

            [GrandResourceDisplayName("RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}