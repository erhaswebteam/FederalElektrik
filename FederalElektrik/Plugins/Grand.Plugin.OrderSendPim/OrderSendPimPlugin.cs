using Grand.Core.Plugins;
using Grand.Plugin.Order.OrderSendPim.Controllers;
using System;

namespace Grand.Plugin.Order.OrderSendPim
{
    public class OrderSendPimPlugin : BasePlugin
    {
        public OrderSendPimPlugin()
        {
        }

        public Type GetControllerType()
        {
            return typeof(OrderSendController);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
