using Microsoft.AspNetCore.Mvc;
using Grand.Web.Services;
using System.Linq;
using Grand.Web.Models.ShoppingCart;
using Grand.Core.Domain.Orders;
using Grand.Core;
using Grand.Services.Orders;
using System;
using Grand.Services.Points;

namespace Grand.Web.ViewComponents
{
    public class OrderSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartWebService _shoppingCartWebService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProcessService _processService;
        public OrderSummaryViewComponent(IShoppingCartWebService shoppingCartWebService, IWorkContext workContext, IStoreContext storeContext, IProcessService processService)
        {
            this._shoppingCartWebService = shoppingCartWebService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._processService = processService;
        }

        public IViewComponentResult Invoke(bool? prepareAndDisplayOrderReviewData, ShoppingCartModel overriddenModel)
        {
            //use already prepared (shared) model
            //if (overriddenModel != null)
            //    return View(overriddenModel);

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart || sci.ShoppingCartType == ShoppingCartType.Auctions)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartModel();
            model.Point = _processService.GetCustomerActualPoint(_workContext.CurrentCustomer.Username);
            var totals = _shoppingCartWebService.PrepareOrderTotals(cart, true);
            model.CartTotal = string.IsNullOrEmpty(totals.SubTotal) == false ? Convert.ToDecimal(totals.SubTotal.Split(' ').First().Replace(",", ".")) : 0m;
            _shoppingCartWebService.PrepareShoppingCart(model, cart,
                isEditable: true,
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
            return View(model);

        }
    }
}