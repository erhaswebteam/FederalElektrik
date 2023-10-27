using Grand.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Rewards.StockControlErhasPIM
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Rewards.AddProductToCart-Catalog",
                 "addproducttocart/catalog/{productId}/{shoppingCartTypeId}",
                 new { controller = "StockControlErhasPIM", action = "AddProductToCart_Catalog" },
                            new { productId = @"\w+", shoppingCartTypeId = @"\d+" }
            );

            //routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.AdminLogin",
            //     "system/",
            //     new { controller = "RewardOperations", action = "AdminLogin" }
            //);
            //routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.CustomerRewardPoints",
            //                "rewardpoints/history",
            //                new { controller = "RewardOperations", action = "CustomerRewardPoints" }
            //                );

            /*
             
            routeBuilder.MapLocalizedRoute("AddProductToCart-Catalog",
                            "addproducttocart/catalog/{productId}/{shoppingCartTypeId}",
                            new { controller = "AddToCart", action = "AddProductToCart_Catalog" },
                            new { productId = @"\w+", shoppingCartTypeId = @"\d+" },
                            new[] { "Grand.Web.Controllers" });
             
             */
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
