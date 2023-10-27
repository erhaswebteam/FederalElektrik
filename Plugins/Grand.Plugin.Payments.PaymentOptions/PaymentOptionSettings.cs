using Grand.Core.Configuration;

namespace Grand.Plugin.Payments.PaymentOptions
{
    public class PaymentOptionSettings : ISettings
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string BaseUrl = "https://api.ipara.com/";
        public string Version = "1.0";
        public string Mode = "T";
        public string HashString = string.Empty;

        public string AddressGetUser { get; set; }
        public string AddressPointSpending { get; set; }
        public string ProjectPreChars { get; set; }
        public string ApiToken { get; set; }
        public string AddressAllPointsByUser { get; set; }
        public bool EnableRewardPointPluginHistory { get; set; }
        public string CancelOrderTransactionAddress { get; set; }

    }
}
