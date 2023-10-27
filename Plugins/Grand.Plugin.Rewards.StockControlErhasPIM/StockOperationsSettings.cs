using Grand.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Rewards.StockControlErhasPIM
{
    public class StockOperationsSettings : ISettings
    {
        public string DescriptionText { get; set; }

        public string StockControlErhasPIMGetSkuQuantityAddress { get; set; }
        public string StockControlErhasPIMToken { get; set; }
    }
}
