using Grand.Framework.Mvc.ModelBinding;
using Grand.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Rewards.RewardOperations.Models
{
    public class ConfigurationModel : BaseGrandModel
    {
        public string ActiveStoreScopeConfiguration { get; set; }

        [GrandResourceDisplayName("Project.Api.Address.GetUser")]
        public string AddressGetUser { get; set; }
        public bool AddressGetUser_OverrideForStore { get; set; }

        [GrandResourceDisplayName("Project.Address.PointSpending")]
        public string AddressPointSpending { get; set; }
        public bool AddressPointSpending_OverrideForStore { get; set; }

        [GrandResourceDisplayName("Project.Api.ProjectPreChars")]
        public string ProjectPreChars { get; set; }
        public bool ProjectPreChars_OverrideForStore { get; set; }

        [GrandResourceDisplayName("Project.Api.Token")]
        public string ApiToken { get; set; }
        public bool ApiToken_OverrideForStore { get; set; }
    }
}
