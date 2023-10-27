using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Domain.Tasks;
using Grand.Data;
using Grand.Plugin.Order.OrderSendPim.Models;
using Grand.Services.Catalog;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Orders;
using Grand.Services.Stores;
using Grand.Services.Tasks;
using Grand.Services.Tax;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Grand.Plugin.Order.OrderSendPim.Controllers
{
    public partial class OrderSendController : ScheduleTask, IScheduleTask
    {
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IRewardPointsService _rewardPointsService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IProductService _productService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IRepository<Core.Domain.Orders.Order> _orderRepository;
        private readonly ICountryService _countryService;

        public OrderSendController(
            ILocalizationService localizationService,
            LocalizationSettings localizationSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IRewardPointsService rewardPointsService,
            IStoreContext storeContext,
            IOrderService orderService,
            IStateProvinceService stateProvinceService,
            IProductService productService,
            ITaxCategoryService taxCategoryService,
            IRepository<Core.Domain.Orders.Order> orderRepository,
            ICountryService countryService
            )
        {
            this._localizationService = localizationService;
            this._localizationSettings = localizationSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._customerSettings = customerSettings;
            this._rewardPointsService = rewardPointsService;
            this._storeContext = storeContext;
            this._orderService = orderService;
            this._stateProvinceService = stateProvinceService;
            this._productService = productService;
            this._taxCategoryService = taxCategoryService;
            this._orderRepository = orderRepository;
            this._countryService = countryService;
        }

        public void Execute()
        {
            MongoDBRepository<Core.Domain.Orders.Order> _orderRepository = new MongoDBRepository<Core.Domain.Orders.Order>();
            var orders = _orderRepository.Table.Where(x => x.OrderStatusId == (int)OrderStatus.Processing && x.PaymentStatusId == (int)PaymentStatus.Paid && x.IsSendPim == false
            && x.Deleted == false).ToList().OrderBy(x => x.OrderNumber);
            //query = query.Where(x => x.IsSendPim == false);
            //query = query.Where(x => x.OrderStatusId == 20);
            //query = query.Where(x => x.PaymentStatusId == 30);
            //query = query.OrderBy(x => x.OrderNumber);
            //var orders = new PagedList<Core.Domain.Orders.Order>(query, 0, 1000000);

            //List<Core.Domain.Orders.Order> orders = _orderRepository.Table
            //    .Where(x => x.IsSendPim == false && x.OrderStatus == Core.Domain.Orders.OrderStatus.Processing && x.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
            //    .OrderBy(x => x.CreatedOnUtc).ToList();

            //IPagedList<Core.Domain.Orders.Order> tempOrders = _orderService.SearchOrders(os: Core.Domain.Orders.OrderStatus.Processing, ps: Core.Domain.Payments.PaymentStatus.Paid);
            //List<Core.Domain.Orders.Order> orders = new List<Core.Domain.Orders.Order>();
            //orders.Add(_orderService.GetOrderById("5d0cd5da3996560120bacdac"));
            if (orders != null && orders.Count() > 0)
            {
                foreach (Core.Domain.Orders.Order order in orders)
                {
                    foreach (Core.Domain.Orders.OrderItem orderItem in order.OrderItems)
                    {
                        var product = _productService.GetProductById(orderItem.ProductId);
                        if (product != null)
                        {
                            //Settings deki 1 TL Kaç Puan Değeri
                            string tempMoneyRate = _settingService.GetSetting("payment.moneypointfactor").Value;
                            decimal moneyRate = decimal.Parse(tempMoneyRate);

                            var taxCategory = _taxCategoryService.GetTaxCategoryById(product.TaxCategoryId);
                            if (taxCategory != null)
                            {
                                decimal totalMoney = orderItem.PriceInclTax * moneyRate;
                                decimal productKdv = 1 + (decimal.Parse(taxCategory.Name) / 100);

                                decimal tax = 0;
                                if (decimal.Parse(taxCategory.Name) == 0)
                                    tax = 0;
                                else
                                    tax = totalMoney - (totalMoney / productKdv);

                                decimal _priceInclTax = orderItem.PriceInclTax * moneyRate;
                                decimal _priceExclTax = _priceInclTax / productKdv;

                                decimal _unitPriceInclTax = orderItem.UnitPriceInclTax * moneyRate;
                                decimal _unitPriceExclTax = _unitPriceInclTax / productKdv;
                                decimal _unitPointIncTax = _unitPriceInclTax / moneyRate;
                                decimal _pointIncTax = orderItem.PriceInclTax;

                                OrderListModel model = new OrderListModel()
                                {
                                    CreatedDate = DateTime.Now,
                                    Description1 = order.ShippingAddress.PhoneNumber,
                                    Description2 = "",
                                    Description3 = "",
                                    Inv_Address1 = order.BillingAddress.Address1,
                                    Inv_Address2 = order.BillingAddress.Address2,
                                    Inv_Name = order.BillingAddress.FirstName,
                                    Inv_Surname = order.BillingAddress.LastName,
                                    Inv_Phone = order.BillingAddress.PhoneNumber,
                                    Inv_Country = "Türkiye",
                                    Inv_City = _countryService.GetCountryById(order.BillingAddress.CountryId).Name,
                                    Inv_State = _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name,
                                    Inv_TaxNumber = order.BillingAddress.VatNumber,
                                    Inv_TaxOffice = "",
                                    Inv_TCIdentity = "",
                                    IrsNumber = "",
                                    LogoCode = product.Sku,
                                    OrderDate = order.CreatedOnUtc,
                                    ////Ürünün/Ürünlerin satır toplam PUANI (Item ve adet çarpımın toplamı)
                                    PointIncTax = _pointIncTax,
                                    ////Ürünün/Ürünlerin KDV hariç toplam TL fiyatı
                                    PriceExcTax = _priceExclTax,
                                    ////Ürünün/Ürünlerin herşey dahil toplam TL fiyatı
                                    PriceIncTax = _priceInclTax,
                                    ProductId = 0,
                                    ProductName = product.Name,
                                    ProjectCode = "",
                                    ProjectId = 28,
                                    Quantity = orderItem.Quantity,
                                    RefNumber = "",
                                    Ship_Address1 = order.ShippingAddress.Address1,
                                    Ship_Address2 = order.ShippingAddress.Address2,
                                    Ship_Country = "Türkiye",
                                    Ship_City = _countryService.GetCountryById(order.ShippingAddress.CountryId).Name,
                                    Ship_State = _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name,
                                    Ship_Name = order.ShippingAddress.FirstName,
                                    Ship_Surname = order.ShippingAddress.LastName,
                                    Ship_Phone = order.ShippingAddress.PhoneNumber,
                                    Ship_TaxNumber = order.ShippingAddress.VatNumber,
                                    Ship_TaxOffice = "",
                                    Ship_TCIdentity = "",
                                    SiparisNo = order.OrderNumber.ToString(),
                                    StatusId = order.OrderStatusId,
                                    SupplierId = null,
                                    Tax = tax,
                                    TrackingNumber = "",
                                    TrackingStatus = "",
                                    TransferDate = new DateTime(1970, 01, 01),
                                    TransferedLogo = false,
                                    TransferRefId = "",
                                    UnitPointIncTax = _unitPointIncTax,
                                    //Ürünün tekil KDV hariç TL fiyatı
                                    UnitPriceExcTax = _unitPriceExclTax,
                                    //Ürünün tekil herşey dahil TL fiyatı
                                    UnitPriceIncTax = _unitPriceInclTax,
                                    Username = _customerService.GetCustomerById(order.CustomerId).Username,
                                    VatRate = decimal.Parse(taxCategory.Name),
                                    token = "DAA816F6FADAE171"
                                };

                                string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                                //Check Pim.OrderList IsHave 
                                string pimCheckAdress = _settingService.GetSettingByKey<string>("pim.orderlist.ishave");
                                OrderListResultModel isHaveOrders = SendRequest<OrderListResultModel>(pimCheckAdress, request);
                                if (isHaveOrders.count != 1)
                                {
                                    //Insert PIM
                                    string address = _settingService.GetSettingByKey<string>("ordersendpimaddress");
                                    OrderListResultModel result = SendRequest<OrderListResultModel>(address, request);
                                    if (result.id > 0)
                                    {
                                        order.IsSendPim = true;
                                        _orderService.UpdateOrder(order);
                                    }
                                }
                                else
                                {
                                    order.IsSendPim = true;
                                    _orderService.UpdateOrder(order);
                                }
                            }
                        }
                    }
                }
            }
        }

        public T SendRequest<T>(string address, string dataObject)
        {
            var request = (HttpWebRequest)WebRequest.Create("" + address);
            request.Method = "POST";
            var encoding = new UTF8Encoding();

            var byteArray = encoding.GetBytes(dataObject);
            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            T data;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                string text = sr.ReadToEnd();
                data = JsonConvert.DeserializeObject<T>(text);
            }

            return data;
        }
    }
}
