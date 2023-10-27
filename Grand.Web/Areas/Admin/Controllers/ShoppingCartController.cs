using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Grand.Web.Areas.Admin.Models.ShoppingCart;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Orders;
using Grand.Services.Catalog;
using Grand.Services.Customers;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Orders;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Services.Tax;
using Grand.Framework.Kendoui;
using Grand.Core.Infrastructure;
using Grand.Core;
using Grand.Framework.Controllers;
using Grand.Web.Areas.Admin.Models.Customers;
using System.Collections.Generic;
using Grand.Services.ExportImport;

namespace Grand.Web.Areas.Admin.Controllers
{
    public partial class ShoppingCartController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IExportManager _exportManager;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Constructors

        public ShoppingCartController(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IStoreService storeService,
            ITaxService taxService, 
            IPriceCalculationService priceCalculationService,
            IPermissionService permissionService, 
            ILocalizationService localizationService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IExportManager exportManager,
            IStoreContext storeContext)
        {
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._storeService = storeService;
            this._taxService = taxService;
            this._priceCalculationService = priceCalculationService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productService = productService;
            this._exportManager = exportManager;
            this._storeContext = storeContext;
        }

        #endregion
        
        #region Methods

        //shopping carts
        public IActionResult CurrentCarts()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public IActionResult CurrentCarts(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.ShoppingCart,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new ShoppingCartModel
                {
                    CustomerId = x.Id,
                    CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    TotalItems = x.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).Sum(y=>y.Quantity)
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult GetCartDetails(string customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId);
                    var product = EngineContext.Current.Resolve<IProductService>().GetProductById(sci.ProductId);
                    var sciModel = new Models.ShoppingCart.ShoppingCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml, customer),
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(product, _priceCalculationService.GetUnitPrice(sci), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }





        //wishlists
        public IActionResult CurrentWishlists()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public IActionResult CurrentWishlists(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.Wishlist,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new ShoppingCartModel
                {
                    CustomerId = x.Id,
                    CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    TotalItems = x.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).Sum(y=>y.Quantity)
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult GetWishlistDetails(string customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var gridModel = new DataSourceResult
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId);
                    var product = EngineContext.Current.Resolve<IProductService>().GetProductById(sci.ProductId);                    
                    var sciModel = new Models.ShoppingCart.ShoppingCartItemModel
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml, customer),
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(product, _priceCalculationService.GetUnitPrice(sci), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };

            return Json(gridModel);
        }
        [HttpPost, ActionName("CurrentWishlists")]
        [FormValueRequired("exportexcel-all")]
        public IActionResult CurrentWishlists2()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.Wishlist);
            var list = new List<Grand.Services.ExportImport.ShoppingCartItemModel>();
            //for point values
            foreach (var cus in customers)
            {
                var cart = cus.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
                foreach (var sci in cart)
                {
                    var product = _productService.GetProductById(sci.ProductId);
                    var store = _storeContext.CurrentStore;
                    var sciModel = new Grand.Services.ExportImport.ShoppingCartItemModel
                    {
                        Store = store != null ? store.Name : "Unknown",
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        ProductName = product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml, cus),
                        Email = cus.Email
                    };
                    list.Add(sciModel);
                }
            }

            try
            {
                byte[] bytes = _exportManager.ExportWishlistToXlsx(list);
                return File(bytes, "text/xls", "Prometeon_Wishlist_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        #endregion
    }
}
