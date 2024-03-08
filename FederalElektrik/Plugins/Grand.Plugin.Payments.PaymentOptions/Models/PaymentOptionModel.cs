using Grand.Framework.Mvc.ModelBinding;
using Grand.Framework.Mvc.Models;

namespace Grand.Plugin.Payments.PaymentOptions.Models
{
    public class PaymentOptionModel : BaseGrandModel
    {
        //Settings Params
        public string BanksInfo { get; set; }
        public decimal HavaleDiscountRate { get; set; }
        public bool IsActiveThreeDSecure { get { return true; } }
        public bool IsActiveWithOutThreeDSecure { get { return false; } }
        public bool IsActiveHavale { get { return true; } }

        public string OrderNumber { get; set; }
        public string OrderGuide { get; set; }
        public decimal TotalDebit { get; set; }
        public string CardNameSurname { get; set; }
        public string CardNumber { get; set; }
        public string CardMonth { get; set; }
        public string CardYear { get; set; }
        public string CardCvv { get; set; }

        public string Msg { get; set; }
        public int PaymentStatus { get; set; }

        public bool IsRoleGenelMudurluk { get; set; }
    }
    public enum PaymentStatusTypeEnum
    {
        Odeme_Basarili = 1,
        Odeme_Basarisiz = -1
    }
}