using Grand.Core.Domain.Orders;
using System;
using System.Collections.Generic;

namespace Grand.Core.Domain.Orders
{
    public partial class OrderPayments : BaseEntity
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderItemId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
