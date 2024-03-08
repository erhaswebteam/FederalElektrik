using Grand.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Payments.PaymentOptions
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Payments.PaymentOptions",
                 "payment-options",
                 new { controller = "PaymentOptions", action = "PaymentOptions" });

            //Success
            routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.IParaSuccess",
                 "Plugins/PaymentOptions/IParaSuccess",
                 new { controller = "PaymentOptions", action = "IParaSuccess" }
            );
            //Cancel
            routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.CancelOrder",
                 "Plugins/PaymentOptions/CancelOrder",
                 new { controller = "PaymentOptions", action = "CancelOrder" }
            );
            //ThreeDRedirect
            routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.ThreeDPayment",
                 "Plugins/PaymentOptions/ThreeDPayment",
                 new { controller = "PaymentOptions", action = "ThreeDPayment" }
            );
            //Havale
            routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.Havale",
                 "Plugins/PaymentOptions/Havale",
                 new { controller = "PaymentOptions", action = "HavalePayment" }
            );
        }

        public int Priority
        {
            get
            {
                return 100;
            }
        }
    }
}
