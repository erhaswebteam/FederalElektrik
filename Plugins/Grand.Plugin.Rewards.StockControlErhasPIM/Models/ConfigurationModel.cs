using Grand.Framework.Mvc.ModelBinding;
using Grand.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Rewards.StockControlErhasPIM.Models
{
    public class ConfigurationModel : BaseGrandModel
    {
        public string ActiveStoreScopeConfiguration { get; set; }

        [GrandResourceDisplayName("Grand.Plugin.Rewards.StockControlErhasPIM.StockControlErhasPIMGetSkuQuantityAddress")]
        public string StockControlErhasPIMGetSkuQuantityAddress { get; set; }
        public bool StockControlErhasPIMGetSkuQuantityAddress_OverrideForStore { get; set; }

        [GrandResourceDisplayName("Grand.Plugin.Rewards.StockControlErhasPIM.StockControlErhasPIMToken")]
        public string StockControlErhasPIMToken { get; set; }
        public bool StockControlErhasPIMToken_OverrideForStore { get; set; }
    }
}
