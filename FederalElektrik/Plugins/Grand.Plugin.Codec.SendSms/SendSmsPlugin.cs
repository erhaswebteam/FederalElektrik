using Grand.Core.Plugins;
using Grand.Plugin.Codec.SendSms.Controllers;
using System;

namespace Grand.Plugin.Codec.SendSms
{
    public class SendSmsPlugin : BasePlugin
    {
        public SendSmsPlugin()
        {

        }

        public Type GetControllerType()
        {
            return typeof(SMSSendController);
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
