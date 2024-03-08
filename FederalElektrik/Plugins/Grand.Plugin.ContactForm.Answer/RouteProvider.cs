using Grand.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.ContactFormAnswer
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //routeBuilder.MapRoute("Plugin.Rewards.RewardOperations.Login",
            //     "login/",
            //     new { controller = "RewardOperations", action = "Login" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.ContactForm",
                 "Admin/ContactForm/List",
                 new { controller = "ContactFormAnswerAdmin", action = "List", area = "admin" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.ContactFormList",
                 "Admin/ContactForm/ContactFormList",
                 new { controller = "ContactFormAnswerAdmin", action = "ContactFormList", area = "admin" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.Details",
                 "Admin/ContactForm/Details/{id}",
                 new { controller = "ContactFormAnswerAdmin", action = "Details", area = "admin" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.ContactFormAnswerList",
                 "Admin/ContactForm/ContactFormAnswerList",
                 new { controller = "ContactFormAnswerAdmin", action = "ContactFormAnswerList", area = "admin" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.ContactFormAnswerWrite",
                 "Admin/ContactForm/ContactFormAnswerWrite",
                 new { controller = "ContactFormAnswerAdmin", action = "ContactFormAnswerWrite", area = "admin" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.ContactUs",
                 "contactus",
                 new { controller = "ContactFormAnswer", action = "ContactUs" });

            routeBuilder.MapRoute("Plugin.Rewards.ContactFormAnswer.ContactUs.Id",
                 "contactus/{id}",
                 new { controller = "ContactFormAnswer", action = "ContactUs" });
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
