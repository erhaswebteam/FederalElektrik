using System;
using System.Collections.Generic;
using Grand.Core;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Shipping;

namespace Grand.Services.Catalog
{
    public partial interface IProductService
    {
        IPagedList<Product> SearchProductsTR(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<string> categoryIds = null,
            string manufacturerId = "",
            string storeId = "",
            string vendorId = "",
            string warehouseId = "",
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            string productTagId = "",
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            string languageId = "",
            IList<string> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);


        IPagedList<Product> SearchProductsTR(
            out IList<string> filterableSpecificationAttributeOptionIds,
            bool loadFilterableSpecificationAttributeOptionIds = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<string> categoryIds = null,
            string manufacturerId = "",
            string storeId = "",
            string vendorId = "",
            string warehouseId = "",
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            string productTagId = "",
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            string languageId = "",
            IList<string> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);
    }
}
