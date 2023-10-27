using Grand.Core.Data;
using Grand.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Plugin.Order.SendLogo.Services
{
    public partial class PaymentPluginServices 
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
        public PaymentPluginServices(IRepository<OrderPayments> orderPaymentRepository)
        {
            this._orderPaymentRepository = orderPaymentRepository;
        }

        #endregion

        #region Methods

        public virtual OrderPayments GetOrderById(string orderId)
        {
            return _orderPaymentRepository.GetById(orderId);
        }

        public virtual OrderPayments GetOrderPayment(string orderItemId)
        {
            return _orderPaymentRepository.Table.Where(x=>x.OrderItemId == orderItemId).FirstOrDefault();
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
