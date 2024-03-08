using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Tax;
using Grand.Framework.Controllers;
using Grand.Plugin.Rewards.StockControlErhasPIM.Helper;
using Grand.Services.Catalog;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Media;
using Grand.Services.Orders;
using Grand.Services.Security;
using Grand.Services.Seo;
using Grand.Services.Tax;
using Grand.Web.Controllers;
using Grand.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Plugin.Rewards.StockControlErhasPIM.Controllers
{
    public class StockControlErhasPIMController : BasePublicController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductReservationService _productReservationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartWebService _shoppingCartWebService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ICurrencyService _currencyService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICacheManager _cacheManager;
        private readonly ITaxService _taxService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        private readonly ISettingService _settingService;

        #endregion

        #region CTOR

        public StockControlErhasPIMController(IProductService productService,
            IProductReservationService productReservationService,
            IShoppingCartService shoppingCartService,
            IShoppingCartWebService shoppingCartWebService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            ICurrencyService currencyService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            ICacheManager cacheManager,
            ITaxService taxService,
            IProductAttributeParser productAttributeParser,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPictureService pictureService,
            ShoppingCartSettings shoppingCartSettings,
            MediaSettings mediaSettings,
            ISettingService settingService)
        {
            this._productService = productService;
            this._productReservationService = productReservationService;
            this._shoppingCartService = shoppingCartService;
            this._shoppingCartWebService = shoppingCartWebService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._currencyService = currencyService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._cacheManager = cacheManager;
            this._taxService = taxService;
            this._productAttributeParser = productAttributeParser;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._settingService = settingService;
        }

        #endregion

        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public virtual IActionResult AddProductToCart_Catalog(string productId, int shoppingCartTypeId,
            int quantity, bool forceredirection = false)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = _productService.GetProductById(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            StockOperations stockOperations = new StockOperations(_settingService);
            var stockQuantity = stockOperations.GetStockQuantityWithSku(product.Sku);
            if (stockQuantity < quantity)
            {
                //Ürün kapalı (-2) veya (gerçek stok ve adet 0) ise ürünü yayından kaldır...
                if (stockQuantity == -2 || stockQuantity == 0)
                {
                    product.StockQuantity = stockQuantity;
                    product.Published = false;
                    _productService.UpdateProduct(product);
                }

                if (stockQuantity == -999 || stockQuantity == -888)
                {
                    LogException(new Exception(string.Format("{0} kodlu ürün {1} döndüğü için ürün sepete eklenemedi...", product.Sku, stockQuantity)));
                }

                //no product found
                return Json(new
                {
                    success = false,
                    message = _localizationService.GetResource("Grand.Plugin.Rewards.RewardOperations.DeficientStockQuantity")
                });
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }


            var allowedQuantities = product.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.ProductAttributeMappings.Any())
            {
                //product has some attributes. let a customer see them
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            var customer = _workContext.CurrentCustomer;

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == cartType)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product.Id);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(customer, new ShoppingCartItem()
                {
                    ShoppingCartType = cartType,
                    StoreId = _storeContext.CurrentStore.Id,
                    CustomerEnteredPrice = decimal.Zero,
                    Quantity = quantityToValidate
                },
                product, false);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = _shoppingCartService.AddToCart(customer: customer,
                productId: productId,
                shoppingCartType: cartType,
                storeId: _storeContext.CurrentStore.Id,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            var addtoCartModel = _shoppingCartWebService.PrepareAddToCartModel(product, customer, quantity, 0, "", cartType, null, null, "", "", "");

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                        customer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .Sum(x => x.Quantity));

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            html = this.RenderPartialViewToString("_AddToCart", addtoCartModel),
                            updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                        customer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .Sum(x => x.Quantity));

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? this.RenderViewComponentToString("FlyoutShoppingCart")
                            : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            html = this.RenderPartialViewToString("_AddToCart", addtoCartModel),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                        });
                    }
            }
        }
    }
}
