using Grand.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Payments.PaymentOptions
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Order.SendLogo",
                 "api/getneworders",
                 new { controller = "SendLogo", action = "GetNewOrders" }
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
