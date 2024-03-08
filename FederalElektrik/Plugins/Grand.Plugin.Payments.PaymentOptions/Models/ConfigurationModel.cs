using Grand.Framework.Mvc.ModelBinding;
using Grand.Framework.Mvc.Models;

namespace Grand.Plugin.Payments.PaymentOptions.Models
{
    public class ConfigurationModel : BaseGrandModel
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string BaseUrl = "https://api.ipara.com/";
        public string Version = "1.0";
        public string Mode = "T";
        public string HashString = string.Empty;


    }
}