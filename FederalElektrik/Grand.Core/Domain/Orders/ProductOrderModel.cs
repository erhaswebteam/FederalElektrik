using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Core.Domain.Orders
{
    public class ProductOrderModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string OrderNo { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal BirimFiyati { get; set; }
        public decimal Total { get; set; }
        public decimal PointDebit { get; set; }
        public decimal CardDebit { get; set; }
        public string TaxRate { get; set; }
        public string ShipmentName { get; set; }
        public string ShipmentSurname { get; set; }
        public string ShipmentAddress { get; set; }
        public string ShipmentCity { get; set; }
        public string ShipmentTown { get; set; }
        public string ShippingStatus { get; set; }
        public string Phone { get; set; }
        public string CariKod { get; set; }
        public string CreatedDate { get; set; }

        public string TotalIncPrice { get; set; }
        public string CompanyName { get; set; }
        public string PaymentType { get; set; }
        public string OrderGuidNo { get; set; }
        public string OrderStatus { get; set; }
        public int Type { get; set; }
        public string ProductType { get; set; }

        public string bill_NameSurname { get; set; }
        public string bill_Company { get; set; }
        public string bill_Address { get; set; }
        public string bill_City { get; set; }
        public string bill_Town { get; set; }
        public string bill_TaxId { get; set; }
        public string bill_TaxOffice { get; set; }
        public string bill_TCKNO { get; set; }
        public string bill_Phone { get; set; }
        public string bill_GSM { get; set; }

    }
}
