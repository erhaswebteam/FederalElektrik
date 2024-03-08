using Microsoft.AspNetCore.Mvc;

namespace Grand.Plugin.Payments.PaymentOptions.Components
{
    public class PaymentOptionsComp : ViewComponent
    {
        public PaymentOptionsComp() { }

        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.PaymentOptions/Views/PaymentInfo.cshtml");
        }
    }
}