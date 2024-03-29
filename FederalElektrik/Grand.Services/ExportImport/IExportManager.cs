﻿using System;
using System.Collections.Generic;
using System.IO;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Directory;
using Grand.Core.Domain.Messages;
using Grand.Core.Domain.Orders;

namespace Grand.Services.ExportImport
{
    public partial class ShoppingCartItemModel
    {
       
        public string Store { get; set; }
       
        public string ProductId { get; set; }
       
        public string ProductName { get; set; }
        public string AttributeInfo { get; set; }

      
        public int Quantity { get; set; }
        public string Email { get; set; }
    }
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
        /// <summary>
        /// Export manufacturer list to xml
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Result in XML format</returns>
        string ExportManufacturersToXml(IList<Manufacturer> manufacturers);

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        byte[] ExportManufacturersToXlsx(IEnumerable<Manufacturer> manufacturers);
        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        string ExportCategoriesToXml();


        byte[] ExportCustomerProductsQuotaToXlsx(List<CustomerProductQuotaModel> customersQuota);

        /// <summary>
        /// Export category to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        byte[] ExportCategoriesToXlsx(IEnumerable<Category> categories);
        /// <summary>
        /// Export product list to xml
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>Result in XML format</returns>
        string ExportProductsToXml(IList<Product> products);

        /// <summary>
        /// Export products to XLSX
        /// </summary>
        /// <param name="products">Products</param>
        byte[] ExportProductsToXlsx(IEnumerable<Product> products);

        byte[] ExportProductExportModelToXlsx(IEnumerable<ProductExportModel> products);

        /// <summary>
        /// Export order list to xml
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        string ExportOrdersToXml(IList<Order> orders);

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        byte[] ExportOrdersToXlsx(IList<Order> orders);

        byte[] ExportOrderProductToXlsx(IList<ProductOrderModel> orders);

        byte[] ExportOrderForAdminToXlsx(IList<ProductOrderModel> orders);

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="customers">Customers</param>
        byte[] ExportCustomersToXlsx(IList<Customer> customers);

        byte[] ExportAllCustomersToXlsx(IList<Customer> customers);
        byte[] ExportWishlistToXlsx(List<ShoppingCartItemModel> customers);
        /// <summary>
        /// Export customer - personal info to XLSX
        /// </summary>
        /// <param name="customer">Customer</param>
        byte[] ExportCustomerToXlsx(Customer customer, string stroreId);

        /// <summary>
        /// Export customer list to xml
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        string ExportCustomersToXml(IList<Customer> customers);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportNewsletterSubscribersToTxt(IList<string> subscriptions);
        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportStatesToTxt(IList<StateProvince> states);
    }
}
