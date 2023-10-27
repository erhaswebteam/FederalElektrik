using Grand.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Rewards.RewardOperations
{
    public class RewardOperationsSettings : ISettings
    {
        public string DescriptionText { get; set; }

        public string AddressGetUser { get; set; }
        public string AddressPointSpending { get; set; }
        public string ProjectPreChars { get; set; }
        public string ApiToken { get; set; }
        public string AddressAllPointsByUser { get; set; }
        public bool EnableRewardPointPluginHistory { get; set; }
        public string CancelTransactionAddress { get; set; }
        public string AddressSetUserPassword { get; set; }
        public string AddressGetUserByUsername { get; set; }
    }
}