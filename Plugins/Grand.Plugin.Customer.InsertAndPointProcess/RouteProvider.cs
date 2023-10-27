using Grand.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Customer.InsertAndPointProcess
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Customer.InsertAndPointProcess.Index",
                 "Admin/Customer/InsertAndPointProcess/Index",
                 new { controller = "InsertAndPoint", action = "Index", area = "admin" });

            ////Success
            //routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.IParaSuccess",
            //     "Plugins/PaymentOptions/IParaSuccess",
            //     new { controller = "PaymentOptions", action = "IParaSuccess" }
            //);
            ////Cancel
            //routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.CancelOrder",
            //     "Plugins/PaymentOptions/CancelOrder",
            //     new { controller = "PaymentOptions", action = "CancelOrder" }
            //);
            ////ThreeDRedirect
            //routeBuilder.MapRoute("Plugin.Payments.PaymentOptions.ThreeDPayment",
            //     "Plugins/PaymentOptions/ThreeDPayment",
            //     new { controller = "PaymentOptions", action = "ThreeDPayment" }
            //);
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
