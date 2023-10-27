using Grand.Core.Domain.Orders;
using System.Collections.Generic;

namespace Grand.Services.Orders
{
    /// <summary>
    /// Order Payment service interface
    /// </summary>
    public partial interface IOrderPaymentService
    {
        OrderPayments GetOrderById(string orderId);
        OrderPayments GetOrderByOrderId(string orderId);
        void Delete(OrderPayments order);
        void Insert(List<OrderPayments> order);
        void Update(OrderPayments order);
    }
}
