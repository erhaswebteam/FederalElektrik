using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Core.Domain.Catalog
{
    public class ProductExportModel
    {
        public string Role { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string Desi { get; set; }
        public int Stock { get; set; }
        public decimal PriceExl { get; set; }
        public decimal PriceInc { get; set; }
        public string TaxRate { get; set; }
        public string IsActive { get; set; }
        public string CreatedDate { get; set; }
        public string ProductCategory { get; set; }

    }
}
