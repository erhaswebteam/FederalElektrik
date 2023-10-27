using Grand.Core.Domain.Orders;
using System;

namespace Grand.Web.MobileApp.Models
{
    public class OrderResponseModel
    {
        public bool Success { get; set; }

        public Order Order { get; set; }

        public Exception Exception { get; set; }
    }

    public class OrderPaymentRequestModel
    {
        public int OrderNumber { get; set; }
    }
}
