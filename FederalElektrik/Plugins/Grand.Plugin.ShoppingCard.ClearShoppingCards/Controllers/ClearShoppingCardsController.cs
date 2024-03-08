using Grand.Core;
using Grand.Framework.Controllers;
using Grand.Framework.Mvc.Filters;
using Grand.Services.Customers;
using Grand.Services.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ShoppingCard.ClearShoppingCards.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class ClearShoppingCardsController:BasePluginController
    {
        private readonly IShoppingCartService _shoppingCardService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        public ClearShoppingCardsController(IShoppingCartService shoppingCardService,ICustomerService customerService,IStoreContext storeContext)
        {
            this._shoppingCardService = shoppingCardService;
            this._customerService = customerService;
            this._storeContext = storeContext;
        }

        public IActionResult Index()
        {

            return View("~/Plugins/ShoppingCard.ClearShoppingCards/Views/Index.cshtml");
        }
        [HttpPost]
        public IActionResult Index(IFormCollection model)
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                foreach (var customer in customers)
                {
                    _shoppingCardService.ClearShoppingCartItems(customer, _storeContext.CurrentStore.Id);
                }
                SuccessNotification("Sistemdeki bütün alışveriş sepetleri temizlenmiştir.");
                return Index();
            }
            catch(Exception ex)
            {
                ErrorNotification("Bir hata meydana geldi !");
                return Index();
            }
        }
    }
}
