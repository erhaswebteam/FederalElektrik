using Grand.Framework.Mvc.ModelBinding;
using Grand.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer
{
    public class ConfigurationModel : BaseGrandModel
    {
        public string ActiveStoreScopeConfiguration { get; set; }
    }
}
