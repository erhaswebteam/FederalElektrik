using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Order.OrderSendPim.Models
{
    public class OrderListModel
    {
        public int? Id { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string Username { get; set; }
        public string SiparisNo { get; set; }
        public string LogoCode { get; set; }
        public string ProductName { get; set; }
        public string RefNumber { get; set; }
        public string Inv_Name { get; set; }
        public string Inv_Surname { get; set; }
        public string Inv_Phone { get; set; }
        public string Inv_Address1 { get; set; }
        public string Inv_Address2 { get; set; }
        public string Inv_Country { get; set; }
        public string Inv_City { get; set; }
        public string Inv_State { get; set; }
        public string Inv_TCIdentity { get; set; }
        public string Inv_TaxNumber { get; set; }
        public string Inv_TaxOffice { get; set; }
        public string Ship_Name { get; set; }
        public string Ship_Surname { get; set; }
        public string Ship_Phone { get; set; }
        public string Ship_Address1 { get; set; }
        public string Ship_Address2 { get; set; }
        public string Ship_Country { get; set; }
        public string Ship_City { get; set; }
        public string Ship_State { get; set; }
        public string Ship_TCIdentity { get; set; }
        public string Ship_TaxNumber { get; set; }
        public string Ship_TaxOffice { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public int? StatusId { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? Tax { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPriceIncTax { get; set; }
        public decimal? UnitPriceExcTax { get; set; }
        public decimal? UnitPointIncTax { get; set; }
        public decimal? PriceIncTax { get; set; }
        public decimal? PriceExcTax { get; set; }
        public decimal? PointIncTax { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string TrackingNumber { get; set; }
        public string TrackingStatus { get; set; }
        public string IrsNumber { get; set; }
        public bool? TransferedLogo { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferRefId { get; set; }
        public string token { get; set; }
    }
}
