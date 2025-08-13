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
using System.Data.SqlClient;
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
        private dynamic GetPrice(string sku)
        {
            decimal price = 0m;
            using (var conn = new SqlConnection("Data Source=sql.erhas.local;Initial Catalog=EYP;Persist Security Info=True;User ID=sa; Password=pw2019PW; Connection Timeout=900;Pooling='true';Max Pool Size=200;MultipleActiveResultSets=True"))
            {
                var sql = "select p.Price,pu.HizmetBedeli,pu.ProjectPrice from [dbo].[ProjectsProduct] pu inner join [dbo].[Product] p on pu.ProductLogoID=p.LogoID where pu.ProjectID=1039 and p.LogoCode=@logocode";
                var scom = new SqlCommand(sql, conn);
                scom.CommandType = System.Data.CommandType.Text;
                scom.Parameters.AddWithValue("@logocode", sku);
                conn.Open();
                var r = scom.ExecuteReader();
                while (r.Read())
                {
                    var obj = new { AlisFiyati = decimal.Parse(r[0].ToString()), HizmetBedeli = decimal.Parse(r[1].ToString()), SabitFiyat = decimal.Parse(r[2].ToString()) };
                    return obj;
                }
                conn.Close();
            }
            return new { AlisFiyati = 0, HizmetBedeli = 0, SabitFiyat = 0 }; ;
        }
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

                    //decimal webshopDesiTotal = decimal.Parse(order.GenericAttributes.Where(x => x.Key == "WebshopDesiTotal").FirstOrDefault()?.Value);
                    //decimal sadakatDesiTotal = decimal.Parse(order.GenericAttributes.Where(x => x.Key == "SadakatDesiTotal").FirstOrDefault()?.Value);

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
                        var hb = GetPrice(sku);
                        //unitExclTax = calculateRate == 0 ? orderItem.UnitPriceExclTax : (orderItem.UnitPriceExclTax - ((orderItem.UnitPriceExclTax / 100) * calculateRate));
                        //totalExclTax = calculateRate == 0 ? orderItem.PriceExclTax : (orderItem.PriceExclTax - ((orderItem.PriceExclTax / 100) * calculateRate));


                        //bool isWebshop = false;
                        //if (product.ProductCategories.Any(x => x.CategoryId == "63205c270f819e2630cac7c1" || x.CategoryId == "63205c150f819e2630cabb8a"))
                        //    isWebshop = true;


                        decimal _unitPriceExclTaxTL = decimal.Parse(MoneyToPoint.CalculatePoint(orderItem.UnitPriceExclTax));
                        unitExclTax = (calculateRate == 0 || paidPuan==0) ? _unitPriceExclTaxTL : (_unitPriceExclTaxTL / paidPuan);
                        totalExclTax = 0;
                        if(hb!=null && hb.HizmetBedeli == 0 && hb.SabitFiyat>0)
                        {
                            unitExclTax = hb.SabitFiyat;
                        }
                        ProductOrderModel _orderProduct = new ProductOrderModel();
                        _orderProduct.OrderGuidNo = orderItem.OrderItemGuid.ToString();
                        _orderProduct.TaxRate = tax.Name;
                        _orderProduct.BirimFiyati = hb.HizmetBedeli > 0 ? hb.AlisFiyati : unitExclTax;
                        _orderProduct.OrderNo = order.OrderNumber.ToString();
                        _orderProduct.ProductName = _productService.GetProductById(orderItem.ProductId).Name;
                        _orderProduct.Quantity = orderItem.Quantity;
                        _orderProduct.ShipmentName = order.ShippingAddress.FirstName;
                        _orderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : string.Empty;
                        _orderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                        _orderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                        _orderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                        _orderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                        _orderProduct.Total = hb.HizmetBedeli > 0 ? (hb.AlisFiyati * orderItem.Quantity) : totalExclTax;
                        _orderProduct.Username = cus.Username;
                        _orderProduct.Email = cus.Email;
                        _orderProduct.CariKod = "120.01.9021";
                        //_orderProduct.CariKod = _customerService.GetCustomerById(item.CustomerId).GenericAttributes.Where(x => x.Key == "CariKod").FirstOrDefault().Value;
                        _orderProduct.SKU = sku;
                        _orderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                        _orderProduct.Type = 0;
                        _orderProduct.ProductType = "L";

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

                        if (hb.HizmetBedeli > 0)
                        {
                            ProductOrderModel _orderProductHizmet = new ProductOrderModel();
                            _orderProductHizmet.OrderGuidNo = orderItem.OrderItemGuid.ToString();
                            _orderProductHizmet.TaxRate = "20";
                            _orderProductHizmet.BirimFiyati = hb.HizmetBedeli;
                            _orderProductHizmet.OrderNo = order.OrderNumber.ToString();
                            _orderProductHizmet.ProductName = "Hizmet Bedeli";
                            _orderProductHizmet.Quantity = orderItem.Quantity;
                            _orderProductHizmet.ShipmentName = order.ShippingAddress.FirstName;
                            _orderProductHizmet.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : order.LastName;
                            _orderProductHizmet.ShipmentAddress = order.ShippingAddress.Address1;
                            _orderProductHizmet.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                            _orderProductHizmet.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                            _orderProductHizmet.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                            _orderProductHizmet.Total = hb.HizmetBedeli;
                            _orderProductHizmet.Username = cus.Username;
                            _orderProductHizmet.Email = cus.Email;
                            _orderProductHizmet.CariKod = "120.01.9021";
                            //_orderProduct.CariKod = _customerService.GetCustomerById(item.CustomerId).GenericAttributes.Where(x => x.Key == "CariKod").FirstOrDefault().Value;
                            _orderProductHizmet.SKU = "001";
                            _orderProductHizmet.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                            _orderProductHizmet.Type = 0;

                            // Bill Address 
                            _orderProductHizmet.bill_Address = order.BillingAddress.Address1;
                            _orderProductHizmet.bill_City = !string.IsNullOrEmpty(order.BillingAddress.CountryId) ? _countryService.GetCountryById(order.BillingAddress.CountryId).Name : string.Empty;
                            _orderProductHizmet.bill_Company = order.BillingAddress.Company;
                            //_orderProduct.bill_GSM = item.BillingAddress.GSM;
                            _orderProductHizmet.bill_NameSurname = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
                            _orderProductHizmet.bill_Phone = order.BillingAddress.PhoneNumber;
                            _orderProductHizmet.bill_TaxId = order.BillingAddress.VatNumber;
                            _orderProductHizmet.bill_TaxOffice = order.BillingAddress.VatName;

                            if (string.IsNullOrEmpty(order.BillingAddress.Company))
                                _orderProductHizmet.bill_TCKNO = order.BillingAddress.VatNumber;
                            else
                                _orderProductHizmet.bill_TCKNO = string.Empty;

                            _orderProductHizmet.bill_Town = !string.IsNullOrEmpty(order.BillingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.BillingAddress.StateProvinceId).Name : string.Empty;


                            orderProductList.Add(_orderProductHizmet);
                        }
                    }
                    decimal _orderShippingExclTaxTL = Math.Round((decimal.Parse(MoneyToPoint.CalculatePoint((order.OrderShippingInclTax))) / 1.2M), 2);
                    unitExclTax = calculateRate == 0 ? _orderShippingExclTaxTL : (_orderShippingExclTaxTL / paidPuan);
                    totalExclTax = 0;

                    ProductOrderModel _cargoOrderProduct = new ProductOrderModel();
                    _cargoOrderProduct.TaxRate = "20";
                    _cargoOrderProduct.BirimFiyati = unitExclTax;
                    _cargoOrderProduct.OrderNo = order.OrderNumber.ToString();
                    _cargoOrderProduct.ProductName = "Kargo Ücreti";
                    _cargoOrderProduct.Quantity = 1;
                    _cargoOrderProduct.ShipmentName = order.ShippingAddress.FirstName;
                    _cargoOrderProduct.ShipmentSurname = !string.IsNullOrEmpty(order.ShippingAddress.LastName) ? order.ShippingAddress.LastName : order.LastName;
                    _cargoOrderProduct.ShipmentAddress = order.ShippingAddress.Address1;
                    _cargoOrderProduct.ShipmentCity = !string.IsNullOrEmpty(order.ShippingAddress.CountryId) ? _countryService.GetCountryById(order.ShippingAddress.CountryId).Name : string.Empty;
                    _cargoOrderProduct.ShipmentTown = !string.IsNullOrEmpty(order.ShippingAddress.StateProvinceId) ? _stateProvinceService.GetStateProvinceById(order.ShippingAddress.StateProvinceId).Name : string.Empty;
                    _cargoOrderProduct.Phone = !string.IsNullOrEmpty(order.ShippingAddress.PhoneNumber) ? order.ShippingAddress.PhoneNumber : string.Empty;
                    _cargoOrderProduct.Total = Math.Round(totalExclTax, 2);
                    _cargoOrderProduct.Username = cus.Username;
                    _cargoOrderProduct.Email = cus.Email;
                    _cargoOrderProduct.CariKod = "120.01.9021";
                    _cargoOrderProduct.SKU = "KB";
                    _cargoOrderProduct.CreatedDate = order.CreatedOnUtc.ToString(string.Format("dd/MM/yyyy HH:mm:ss"));
                    _cargoOrderProduct.Type = 0;
                    orderProductList.Add(_cargoOrderProduct);




                }
            }

            return Json(orderProductList.OrderBy(x => x.OrderNo));
        }
    }
}
