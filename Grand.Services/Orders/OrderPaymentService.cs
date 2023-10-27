using System;
using System.Collections.Generic;
using Grand.Core.Data;
using Grand.Core.Domain.Orders;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Grand.Services.Orders
{
    /// <summary>
    /// Order Payments
    /// </summary>
    public partial class OrderPaymentService : IOrderPaymentService
    {
        #region Fields

        private readonly IRepository<OrderPayments> _orderPaymentRepository;
       

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="orderNoteRepository">Order note repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="returnRequestRepository">Return request repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="productAlsoPurchasedRepository">Product also purchased repository</param>
        public OrderPaymentService(IRepository<OrderPayments> orderPaymentRepository)
        {
            this._orderPaymentRepository = orderPaymentRepository;
        }

        #endregion

        #region Methods

        public virtual OrderPayments GetOrderById(string orderId)
        {
            return _orderPaymentRepository.GetById(orderId);
        }
        public virtual OrderPayments GetOrderByOrderId(string orderId)
        {
            return _orderPaymentRepository.Table.Where(x=>x.OrderId==orderId).FirstOrDefault();
        }

        public virtual void Delete(OrderPayments order)
        {
            if (order == null)
                throw new ArgumentNullException("order_payment");

            _orderPaymentRepository.Delete(order);
        }

        public virtual void Insert(List<OrderPayments> order)
        {
            if (order == null)
                throw new ArgumentNullException("order_payment");

            foreach (var item in order)
            {
                _orderPaymentRepository.Insert(item);
            }
           
        }

        public virtual void Update(OrderPayments order)
        {
            if (order == null)
                throw new ArgumentNullException("order_payment");

            _orderPaymentRepository.Update(order);
        }

        #endregion
    }
}
