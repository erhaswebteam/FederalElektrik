using Grand.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Order.SendLogo
{
   public class SendLogoSettings : ISettings
    {
        public string DescriptionText { get; set; }
    }
}
