using Grand.Core.Data;
using Grand.Core.Domain.Configuration;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Domain.Shipping;
using Grand.Framework.Controllers;
using Grand.Plugin.Order.SendLogo.Services;
using Grand.Services.Catalog;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Orders;
using Grand.Services.Tax;
using Grand.Web.Extensions;
using Grand.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Plugin.Order.SendLogo.Controllers
{
    public class SendLogoController : BasePluginController
    {

        #region Fields
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ISettingService _settingService;
        private readonly IRepository<OrderPayments> _orderPaymentRepository;
        private readonly IRepository<ShippingByWeightRecord> _sbwRepository;
        #endregion

        #region Constructors

        public SendLogoController(IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            ITaxCategoryService taxCategoryService,
            IProductAttributeParser productAttributeParser,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ISettingService settingService,
            IRepository<OrderPayments> orderPaymentRepository,
            IRepository<ShippingByWeightRecord> sbwRepository
            )
        {
            this._orderService = orderService;
            this._customerService = customerService;
            this._productService = productService;
            this._taxCategoryService = taxCategoryService;
            this._productAttributeParser = productAttributeParser;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._settingService = settingService;
            this._orderPaymentRepository = orderPaymentRepository;
            this._sbwRepository = sbwRepository;
        }

        #endregion

        public virtual JsonResult GetNewOrders(string token)
        {
            List<ProductOrderModel> orderProductList = new List<ProductOrderModel>();
            if (token == "C5c8E3DD36543rE57B5C34C7C65")
            {
                

                //Havale İndirim Oranının Settingsden getirilmesi
                decimal discountRate = 0;
                Setting _havaleDiscountRate = _settingService.GetSetting("paymentoptionsettings.havalediscountrate");
                if (_havaleDiscountRate != null)
                    discountRate = decimal.Parse(_havaleDiscountRate.Value);

                var orderList = _orderService.SearchOrders(os: OrderStatus.Processing, ps: PaymentStatus.Paid, ss: ShippingStatus.NotYetShipped);
                

                foreach (var order in orderList)
                {
                    decimal _orderTotal = decimal.Parse(MoneyToPoint.CalculatePoint(order.OrderTotal));
                    var hasPaymentDetail = _orderPaymentRepository.Table.Where(x => x.OrderId == order.Id).FirstOrDefault();
                    decimal calculateRate = 0;
                    if (hasPaymentDetail != null)
                        calculateRate = Math.Round(_orderTotal / hasPaymentDetail.TotalAmount, 2);
                    decimal unitExclTax = 0;
                    decimal totalExclTax = 0;

                    decimal webshopDesiTotal = decimal.Parse(order.GenericAttributes.Where(x => x.Key == "WebshopDesiTotal").FirstOrDefault()?.Value);
                    decimal sadakatDesiTotal = decimal.Parse(order.GenericAttributes.Where(x => x.Key == "SadakatDesiTotal").FirstOrDefault()?.Value);

                    //string companyOrderNumber = calculateRate == 0 ? order.OrderNumber.ToString() : string.Format("{0}P", order.OrderNumber);
                    //string customerOrderNumber = calculateRate == 0 ? order.OrderNumber.ToString() : string.Format("{0}K", order.OrderNumber);


                    var cus = _customerService.GetCustomerById(order.CustomerId);

                    decimal paidPuan = (hasPaymentDetail != null && _orderTotal!=hasPaymentDetail.TotalAmount) ? _orderTotal / (_orderTotal - hasPaymentDetail.TotalAmount) : 0;

                    // Ürün Gönderimi Bilgisi
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = _productService.GetProductById(orderItem.ProductId);
                        var tax = _taxCategoryService.GetTaxCategoryById(product.TaxCategoryId);

                        string sku = product.FormatSku(orderItem.AttributesXml, _productAttributeParser);

                        //unitExclTax = calculateRate == 0 ? orderItem.UnitPriceExclTax : (orderItem.UnitPriceExclTax - ((orderItem.UnitPriceExclTax / 100) * calculateRate));
                        //totalExclTax = calculateRate == 0 ? orderItem.PriceExclTax : (orderItem.PriceExclTax - ((orderItem.PriceExclTax / 100) * calculateRate));


                        bool isWebshop = false;
                        if (product.ProductCategories.Any(x => x.CategoryId == "63205c270f819e2630cac7c1" || x.CategoryId == "63205c150f819e2630cabb8a"))
                            isWebshop = true;


                        decimal _unitPriceExclTaxTL = decimal.Parse(MoneyToPoint.CalculatePoint(orderItem.UnitPriceExclTax));
                        unitExclTax = (calculateRate == 0 || paidPuan==0) ? _unitPriceExclTaxTL : (_unitPriceExclTaxTL / paidPuan);
                        totalExclTax = 0;

                        ProductOrderModel _orderProduct = new ProductOrderModel();
                        _orderProduct.OrderGuidNo = orderItem.OrderItemGuid.ToString();
                        _orderProduct.TaxRate = tax.Name;
                        _orderProduct.BirimFiyati = unitExclTax;
                        _orderProduct.OrderNo = isWebshop ? order.OrderNumber + "W" : order.OrderNumber + "L";
                        _orderProduct.ProductName = _productService.GetProductById(orderItem.ProductId).Name;
                        _orderProduct.Quantity = orderItem.Quantity;
                        _orderProduct.ShipmentName = order.ShippingAddress.FirstName;
                        _orderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                        _orderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                        _orderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                        _orderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                        _orderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                        _orderProduct.Total = totalExclTax;
                        _orderProduct.Username = cus.Username;
                        _orderProduct.Email = cus.Email;
                        _orderProduct.CariKod = "120.01.3317";
                        //_orderProduct.CariKod = _customerService.GetCustomerById(item.CustomerId).GenericAttributes.Where(x => x.Key == "CariKod").FirstOrDefault().Value;
                        _orderProduct.SKU = sku;
                        _orderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                        _orderProduct.Type = 0;
                        _orderProduct.ProductType = isWebshop ? "W" : "L";

                        // Bill Address 
                        _orderProduct.bill_Address = order.BillingAddress.Address1;
                        _orderProduct.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                        _orderProduct.bill_Company = order.BillingAddress.Company;
                        //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                        _orderProduct.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                        _orderProduct.bill_Phone = order.BillingAddress.PhoneNumber;
                        _orderProduct.bill_TaxId = order.BillingAddress.VatNumber;
                        _orderProduct.bill_TaxOffice = order.BillingAddress.VatName;

                        if (string.IsNullOrEmpty(order.BillingAddress.Company))
                            _orderProduct.bill_TCKNO = order.BillingAddress.VatNumber;
                        else
                            _orderProduct.bill_TCKNO = string.Empty;

                        _orderProduct.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;


                        orderProductList.Add(_orderProduct);

                    }

                    //Kargo Satırı ekleniyor.
                    //unitExclTax = calculateRate == 0 ? order.OrderShippingExclTax : (order.OrderShippingExclTax - ((order.OrderShippingExclTax / 100) * calculateRate));
                    //totalExclTax = calculateRate == 0 ? order.OrderShippingExclTax : (order.OrderShippingExclTax - ((order.OrderShippingExclTax / 100) * calculateRate));


                    if (webshopDesiTotal > 0)
                    {
                        decimal _orderShippingExclTaxTL = Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((webshopDesiTotal))) / 1.2M), 2);
                        unitExclTax = (calculateRate == 0 || paidPuan==0) ? _orderShippingExclTaxTL : (_orderShippingExclTaxTL / paidPuan);
                        totalExclTax = 0;

                        ProductOrderModel _cargoOrderProduct = new ProductOrderModel();
                        _cargoOrderProduct.TaxRate = "20";
                        _cargoOrderProduct.BirimFiyati = unitExclTax;
                        _cargoOrderProduct.OrderNo = order.OrderNumber + "W";
                        _cargoOrderProduct.ProductName = "Kargo Ücreti";
                        _cargoOrderProduct.Quantity = 1;
                        _cargoOrderProduct.ShipmentName = order.ShippingAddress.FirstName;
                        _cargoOrderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                        _cargoOrderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                        _cargoOrderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                        _cargoOrderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                        _cargoOrderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                        _cargoOrderProduct.Total = Math.Round(totalExclTax, 2);
                        _cargoOrderProduct.Username = cus.Username;
                        _cargoOrderProduct.Email = cus.Email;
                        _cargoOrderProduct.CariKod = "120.01.3317";
                        _cargoOrderProduct.SKU = "KB";
                        _cargoOrderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                        _cargoOrderProduct.Type = 0;
                        _cargoOrderProduct.ProductType = "W";

                        // Bill Address 
                        _cargoOrderProduct.bill_Address = order.BillingAddress.Address1;
                        _cargoOrderProduct.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                        _cargoOrderProduct.bill_Company = order.BillingAddress.Company;
                        //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                        _cargoOrderProduct.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                        _cargoOrderProduct.bill_Phone = order.BillingAddress.PhoneNumber;
                        _cargoOrderProduct.bill_TaxId = order.BillingAddress.VatNumber;
                        _cargoOrderProduct.bill_TaxOffice = order.BillingAddress.VatName;

                        if (string.IsNullOrEmpty(order.BillingAddress.Company))
                            _cargoOrderProduct.bill_TCKNO = order.BillingAddress.VatNumber;
                        else
                            _cargoOrderProduct.bill_TCKNO = string.Empty;

                        _cargoOrderProduct.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;


                        orderProductList.Add(_cargoOrderProduct);
                    }

                    if (sadakatDesiTotal > 0)
                    {
                        decimal _orderShippingExclTaxTL = Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((sadakatDesiTotal))) / 1.2M), 2);
                        unitExclTax = (calculateRate == 0 || paidPuan==0) ? _orderShippingExclTaxTL : (_orderShippingExclTaxTL / paidPuan);
                        totalExclTax = 0;

                        ProductOrderModel _cargoOrderProduct = new ProductOrderModel();
                        _cargoOrderProduct.TaxRate = "20";
                        _cargoOrderProduct.BirimFiyati = unitExclTax;
                        _cargoOrderProduct.OrderNo = order.OrderNumber + "L";
                        _cargoOrderProduct.ProductName = "Kargo Ücreti";
                        _cargoOrderProduct.Quantity = 1;
                        _cargoOrderProduct.ShipmentName = order.ShippingAddress.FirstName;
                        _cargoOrderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                        _cargoOrderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                        _cargoOrderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                        _cargoOrderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                        _cargoOrderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                        _cargoOrderProduct.Total = Math.Round(totalExclTax, 2);
                        _cargoOrderProduct.Username = cus.Username;
                        _cargoOrderProduct.Email = cus.Email;
                        _cargoOrderProduct.CariKod = "120.01.3317";
                        _cargoOrderProduct.SKU = "KB";
                        _cargoOrderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                        _cargoOrderProduct.Type = 0;
                        _cargoOrderProduct.ProductType = "L";

                        // Bill Address 
                        _cargoOrderProduct.bill_Address = order.BillingAddress.Address1;
                        _cargoOrderProduct.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                        _cargoOrderProduct.bill_Company = order.BillingAddress.Company;
                        //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                        _cargoOrderProduct.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                        _cargoOrderProduct.bill_Phone = order.BillingAddress.PhoneNumber;
                        _cargoOrderProduct.bill_TaxId = order.BillingAddress.VatNumber;
                        _cargoOrderProduct.bill_TaxOffice = order.BillingAddress.VatName;

                        if (string.IsNullOrEmpty(order.BillingAddress.Company))
                            _cargoOrderProduct.bill_TCKNO = order.BillingAddress.VatNumber;
                        else
                            _cargoOrderProduct.bill_TCKNO = string.Empty;

                        _cargoOrderProduct.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;


                        orderProductList.Add(_cargoOrderProduct);
                    }


                    //Para ile ödeme kurgusu var ise...
                    if (calculateRate > 0)
                    {
                        foreach (var orderItem in order.OrderItems)
                        {
                            var product = _productService.GetProductById(orderItem.ProductId);
                            var _tempTax = _taxCategoryService.GetTaxCategoryById(product.TaxCategoryId);
                            decimal tax = 0;
                            if (_tempTax.Name == "20")
                            {
                                tax = 1.2M;
                            }
                            else if (_tempTax.Name == "10")
                            {
                                tax = 1.1M;
                            }
                            else if (_tempTax.Name == "0")
                            {
                                tax = 1M;
                            }
                            //unitExclTax = calculateRate == 0 ? orderItem.UnitPriceExclTax : (((orderItem.UnitPriceExclTax / 100) * calculateRate));
                            //totalExclTax = calculateRate == 0 ? orderItem.PriceExclTax : (((orderItem.PriceExclTax / 100) * calculateRate));

                            var payProductTL = _orderPaymentRepository.Table.Where(x => x.OrderId == order.Id && x.OrderItemId == orderItem.Id).FirstOrDefault();
                            decimal _unitTaxPayProductTL = Math.Round((payProductTL.Amount / tax), 2);
                            _unitTaxPayProductTL = _unitTaxPayProductTL / orderItem.Quantity;

                            bool isWebshop = false;
                            if (product.ProductCategories.Any(x => x.CategoryId == "63205c270f819e2630cac7c1" || x.CategoryId == "63205c150f819e2630cabb8a"))
                                isWebshop = true;

                            ProductOrderModel _orderProduct = new ProductOrderModel();
                            _orderProduct.OrderGuidNo = orderItem.OrderItemGuid.ToString();
                            _orderProduct.TaxRate = _tempTax.Name;
                            _orderProduct.BirimFiyati = _unitTaxPayProductTL;
                            _orderProduct.OrderNo = isWebshop ? order.OrderNumber + "WK" : order.OrderNumber + "LK";
                            _orderProduct.ProductName = "Prometeon Katılım Bedeli";
                            _orderProduct.Quantity = orderItem.Quantity;
                            _orderProduct.ShipmentName = order.ShippingAddress.FirstName;
                            _orderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                            _orderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                            _orderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                            _orderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                            _orderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                            _orderProduct.Total = totalExclTax;
                            _orderProduct.Username = cus.Username;
                            _orderProduct.Email = cus.Email;
                            _orderProduct.CariKod = "";
                            //_orderProduct.CariKod = _customerService.GetCustomerById(item.CustomerId).GenericAttributes.Where(x => x.Key == "CariKod").FirstOrDefault().Value;
                            _orderProduct.SKU = _tempTax.Name == "10" ? "262" : "261";
                            _orderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                            _orderProduct.Type = 4;
                            _orderProduct.ProductType = isWebshop ? "WK" : "LK";


                            // Bill Address 
                            _orderProduct.bill_Address = order.BillingAddress.Address1;
                            _orderProduct.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                            _orderProduct.bill_Company = order.BillingAddress.Company;
                            //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                            _orderProduct.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                            _orderProduct.bill_Phone = order.BillingAddress.PhoneNumber;
                            _orderProduct.bill_TaxId = order.BillingAddress.VatNumber;
                            _orderProduct.bill_TaxOffice = order.BillingAddress.VatName;

                            if (string.IsNullOrEmpty(order.BillingAddress.Company))
                                _orderProduct.bill_TCKNO = order.BillingAddress.VatNumber;
                            else
                                _orderProduct.bill_TCKNO = string.Empty;

                            _orderProduct.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;


                            orderProductList.Add(_orderProduct);

                        }

                        //Kargo satırı ekleniyor...

                        //unitExclTax = calculateRate == 0 ? order.OrderShippingExclTax : (((order.OrderShippingExclTax / 100) * calculateRate));
                        //totalExclTax = calculateRate == 0 ? order.OrderShippingExclTax : (((order.OrderShippingExclTax / 100) * calculateRate));

                        if (webshopDesiTotal > 0)
                        {
                            var payShippingExclTL = _orderPaymentRepository.Table.Where(x => x.OrderId == order.Id && x.OrderItemId == "KB").FirstOrDefault();
                            decimal _orderShippingExclTaxTL = Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((webshopDesiTotal))) / 1.2M), 2);

                            var _webshopCargoExclTL = (payShippingExclTL.TotalAmount * _orderShippingExclTaxTL) / Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((order.OrderTotal)))), 2);

                            //var payShippingExclTL =  _orderPaymentRepository.Table.Where(x => x.OrderId == order.Id && x.OrderItemId == "KB").FirstOrDefault();
                            //decimal _unitpayShippingExclTLTL = Math.Round((payShippingExclTL.Amount / 1.18M), 2);

                            ProductOrderModel _rateCargoOrderProduct = new ProductOrderModel();
                            _rateCargoOrderProduct.TaxRate = "20";
                            _rateCargoOrderProduct.BirimFiyati = _webshopCargoExclTL;
                            _rateCargoOrderProduct.OrderNo = order.OrderNumber + "WK";
                            _rateCargoOrderProduct.ProductName = "Prometeon Webshop Kargo Katılım Bedeli";
                            _rateCargoOrderProduct.Quantity = 1;
                            _rateCargoOrderProduct.ShipmentName = order.ShippingAddress.FirstName;
                            _rateCargoOrderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                            _rateCargoOrderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                            _rateCargoOrderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                            _rateCargoOrderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                            _rateCargoOrderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                            _rateCargoOrderProduct.Total = Math.Round(totalExclTax, 2);
                            _rateCargoOrderProduct.Username = cus.Username;
                            _rateCargoOrderProduct.Email = cus.Email;
                            _rateCargoOrderProduct.CariKod = "";
                            _rateCargoOrderProduct.SKU = "263";
                            _rateCargoOrderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                            _rateCargoOrderProduct.Type = 4;
                            _rateCargoOrderProduct.ProductType = "WK";

                            // Bill Address 
                            _rateCargoOrderProduct.bill_Address = order.BillingAddress.Address1;
                            _rateCargoOrderProduct.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                            _rateCargoOrderProduct.bill_Company = order.BillingAddress.Company;
                            //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                            _rateCargoOrderProduct.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                            _rateCargoOrderProduct.bill_Phone = order.BillingAddress.PhoneNumber;
                            _rateCargoOrderProduct.bill_TaxId = order.BillingAddress.VatNumber;
                            _rateCargoOrderProduct.bill_TaxOffice = order.BillingAddress.VatName;

                            if (string.IsNullOrEmpty(order.BillingAddress.Company))
                                _rateCargoOrderProduct.bill_TCKNO = order.BillingAddress.VatNumber;
                            else
                                _rateCargoOrderProduct.bill_TCKNO = string.Empty;

                            _rateCargoOrderProduct.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;

                            orderProductList.Add(_rateCargoOrderProduct);
                        }

                        if (sadakatDesiTotal > 0)
                        {
                            var payShippingExclTL = _orderPaymentRepository.Table.Where(x => x.OrderId == order.Id && x.OrderItemId == "KB").FirstOrDefault();
                            decimal _orderShippingExclTaxTL = Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((sadakatDesiTotal))) / 1.2M), 2);

                            var _sadakatCargoExclTL = (payShippingExclTL.TotalAmount * _orderShippingExclTaxTL) / Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((order.OrderTotal)))), 2);

                            //var payShippingExclTL =  _orderPaymentRepository.Table.Where(x => x.OrderId == order.Id && x.OrderItemId == "KB").FirstOrDefault();
                            //decimal _unitpayShippingExclTLTL = Math.Round((payShippingExclTL.Amount / 1.18M), 2);

                            ProductOrderModel _rateCargoOrderProduct = new ProductOrderModel();
                            _rateCargoOrderProduct.TaxRate = "20";
                            _rateCargoOrderProduct.BirimFiyati = _sadakatCargoExclTL;
                            _rateCargoOrderProduct.OrderNo = order.OrderNumber + "LK";
                            _rateCargoOrderProduct.ProductName = "Prometeon Sadakat Kargo Katılım Bedeli";
                            _rateCargoOrderProduct.Quantity = 1;
                            _rateCargoOrderProduct.ShipmentName = order.ShippingAddress.FirstName;
                            _rateCargoOrderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                            _rateCargoOrderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                            _rateCargoOrderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                            _rateCargoOrderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                            _rateCargoOrderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                            _rateCargoOrderProduct.Total = Math.Round(totalExclTax, 2);
                            _rateCargoOrderProduct.Username = cus.Username;
                            _rateCargoOrderProduct.Email = cus.Email;
                            _rateCargoOrderProduct.CariKod = "";
                            _rateCargoOrderProduct.SKU = "263";
                            _rateCargoOrderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                            _rateCargoOrderProduct.Type = 4;
                            _rateCargoOrderProduct.ProductType = "LK";

                            // Bill Address 
                            _rateCargoOrderProduct.bill_Address = order.BillingAddress.Address1;
                            _rateCargoOrderProduct.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                            _rateCargoOrderProduct.bill_Company = order.BillingAddress.Company;
                            //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                            _rateCargoOrderProduct.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                            _rateCargoOrderProduct.bill_Phone = order.BillingAddress.PhoneNumber;
                            _rateCargoOrderProduct.bill_TaxId = order.BillingAddress.VatNumber;
                            _rateCargoOrderProduct.bill_TaxOffice = order.BillingAddress.VatName;

                            if (string.IsNullOrEmpty(order.BillingAddress.Company))
                                _rateCargoOrderProduct.bill_TCKNO = order.BillingAddress.VatNumber;
                            else
                                _rateCargoOrderProduct.bill_TCKNO = string.Empty;

                            _rateCargoOrderProduct.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;

                            orderProductList.Add(_rateCargoOrderProduct);
                        }



                    }
                }
            }

            return Json(orderProductList.OrderBy(x => x.OrderNo));
        }
    }
}
