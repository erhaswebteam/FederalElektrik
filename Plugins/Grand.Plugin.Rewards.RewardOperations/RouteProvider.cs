using Grand.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Rewards.RewardOperations
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.Login",
                 "login/",
                 new { controller = "RewardOperations", action = "Login" }
            );

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.AdminLogin",
                 "system/",
                 new { controller = "RewardOperations", action = "AdminLogin" }
            );

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.CustomerRewardPoints",
                 "rewardpoints/histories",
                 new { controller = "RewardOperations", action = "CustomerRewardPointsHistories" }
                 );

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.ChangeEMailAddress",
               "refresh/password",
               new { controller = "RewardOperations", action = "ChangeEMailAddress" });

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.CustomerChangePassword",
                "customer/changepassword",
                new { controller = "RewardOperations", action = "ChangePassword" });

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.PasswordRecovery",
                "passwordrecovery",
                new { controller = "RewardOperations", action = "PasswordRecovery" });

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.PasswordRecoveryConfirm",
                            "passwordrecovery/confirm",
                            new { controller = "RewardOperations", action = "PasswordRecoveryConfirm" });

            routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.SendPass",
                         "sendpass/",
                         new { controller = "RewardOperations", action = "SendPass" });
        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }
    }
}
