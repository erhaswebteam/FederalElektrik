﻿using System.Collections.Generic;
using Grand.Framework.Mvc.Models;

namespace Grand.Web.Models.Catalog
{
    public partial class ProductsByTagModel : BaseGrandEntityModel
    {
        public ProductsByTagModel()
        {
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }
        
        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }
    }
}