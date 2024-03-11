using System;
using System.Collections.Generic;
using System.Linq;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Data;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Security;
using Grand.Core.Domain.Shipping;
using Grand.Services.Customers;
using Grand.Services.Events;
using Grand.Services.Localization;
using Grand.Services.Messages;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Core.Infrastructure;
using Grand.Services.Shipping;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Seo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grand.Services.Orders;
using System.Text.RegularExpressions;
namespace Grand.Services.Catalog
{
    public partial class ProductService
    {
        public virtual IPagedList<Product> SearchProductsTR(
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
            bool? overridePublished = null)
        {
            IList<string> filterableSpecificationAttributeOptionIds;
            return SearchProductsTR(out filterableSpecificationAttributeOptionIds, false,
                pageIndex, pageSize, categoryIds, manufacturerId,
                storeId, vendorId, warehouseId,
                productType, visibleIndividuallyOnly, markedAsNewOnly, featuredProducts,
                priceMin, priceMax, productTagId, keywords, searchDescriptions, searchSku,
                searchProductTags, languageId, filteredSpecs,
                orderBy, showHidden, overridePublished);
        }

        public virtual IPagedList<Product> SearchProductsTR(
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
            bool? overridePublished = null)
        {
            filterableSpecificationAttributeOptionIds = new List<string>();

            //search by keyword
            bool searchLocalizedValue = false;
            if (!String.IsNullOrEmpty(languageId))
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(""))
                categoryIds.Remove("");

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            #region Search products

            //products
            var builder = Builders<Product>.Filter;
            var filter = FilterDefinition<Product>.Empty;

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {

                if (featuredProducts.HasValue)
                {
                    filter = filter & builder.Where(x => x.ProductCategories.Any(y => categoryIds.Contains(y.CategoryId) && y.IsFeaturedProduct == featuredProducts));
                }
                else
                {
                    filter = filter & builder.Where(x => x.ProductCategories.Any(y => categoryIds.Contains(y.CategoryId)));
                }
            }
            //manufacturer filtering
            if (!String.IsNullOrEmpty(manufacturerId))
            {
                if (featuredProducts.HasValue)
                {
                    filter = filter & builder.Where(x => x.ProductManufacturers.Any(y => y.ManufacturerId == manufacturerId && y.IsFeaturedProduct == featuredProducts));
                }
                else
                {
                    filter = filter & builder.Where(x => x.ProductManufacturers.Any(y => y.ManufacturerId == manufacturerId));
                }

            }

            if (!overridePublished.HasValue)
            {
                //process according to "showHidden"
                if (!showHidden)
                {
                    filter = filter & builder.Where(p => p.Published);
                }
            }
            else if (overridePublished.Value)
            {
                //published only
                filter = filter & builder.Where(p => p.Published);
            }
            else if (!overridePublished.Value)
            {
                //unpublished only
                filter = filter & builder.Where(p => !p.Published);
            }
            if (visibleIndividuallyOnly)
            {
                filter = filter & builder.Where(p => p.VisibleIndividually);
            }
            if (productType.HasValue)
            {
                var productTypeId = (int)productType.Value;
                filter = filter & builder.Where(p => p.ProductTypeId == productTypeId);
            }

            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;
            if (priceMin.HasValue)
            {
                filter = filter & builder.Where(p => p.Price >= priceMin.Value);
            }
            if (priceMax.HasValue)
            {
                //max price
                filter = filter & builder.Where(p => p.Price <= priceMax.Value);
            }
            if (!showHidden && !_catalogSettings.IgnoreFilterableAvailableStartEndDateTime)
            {
                filter = filter & builder.Where(p =>
                    (p.AvailableStartDateTimeUtc == null || p.AvailableStartDateTimeUtc < nowUtc) &&
                    (p.AvailableEndDateTimeUtc == null || p.AvailableEndDateTimeUtc > nowUtc));


            }

            if (markedAsNewOnly)
            {
                filter = filter & builder.Where(p => p.MarkAsNew);
                filter = filter & builder.Where(p =>
                    (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                    (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc));
            }

            //to do: türkçe için search düzeltilecek, sku değerinde de arama yapacak

            //searching by keyword
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                keywords = StringReplaceToTurkishWithUnicode(keywords);
                if (_commonSettings.UseFullTextSearch)
                {
                    keywords = "\"" + keywords + "\"";
                    keywords = keywords.Replace("+", "\" \"");
                    keywords = keywords.Replace(" ", "\" \"");
                    filter = filter & builder.Text(keywords);
                }
                else
                {
                    if (!searchDescriptions)
                        filter = filter & builder.Where(p =>
                            p.Name.ToLower().Contains(keywords.ToLower())
                            ||
                            (searchSku && p.Sku.ToLower().Contains(keywords.ToLower()))
                            );
                    else
                    {
                        filter = filter & builder.Where(p =>
                                (p.Name != null && p.Name.ToLower().Contains(keywords.ToLower()))
                                ||
                                (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(keywords.ToLower()))
                                ||
                                (p.FullDescription != null && p.FullDescription.ToLower().Contains(keywords.ToLower()))
                                ||
                                (p.Locales.Any(x => x.LocaleValue != null && x.LocaleValue.ToLower().Contains(keywords.ToLower())))
                                ||
                                (searchSku && p.Sku.ToLower().Contains(keywords.ToLower()))
                                );
                    }
                }

            }

            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                filter = filter & (builder.AnyIn(x => x.CustomerRoles, allowedCustomerRolesIds) | builder.Where(x => !x.SubjectToAcl));
            }

            if (!String.IsNullOrEmpty(storeId) && !_catalogSettings.IgnoreStoreLimitations)
            {
                filter = filter & builder.Where(x => x.Stores.Any(y => y == storeId) || !x.LimitedToStores);

            }

            //search by specs
            if (filteredSpecs != null && filteredSpecs.Any())
            {
                foreach (var item in filteredSpecs)
                {
                    filter = filter & builder.Where(x => x.ProductSpecificationAttributes.Any(y => y.SpecificationAttributeOptionId == item));
                }
            }

            //vendor filtering
            if (!String.IsNullOrEmpty(vendorId))
            {
                filter = filter & builder.Where(x => x.VendorId == vendorId);
            }

            //warehouse filtering
            if (!String.IsNullOrEmpty(warehouseId))
            {
                filter = filter & (builder.Where(x => x.UseMultipleWarehouses && x.ProductWarehouseInventory.Any(y => y.WarehouseId == warehouseId)) |
                    builder.Where(x => !x.UseMultipleWarehouses && x.WarehouseId == warehouseId));

            }

            //tag filtering
            if (!String.IsNullOrEmpty(productTagId))
            {
                filter = filter & builder.Where(x => x.ProductTags.Any(y => y == productTagId));
            }


            var builderSort = Builders<Product>.Sort.Descending(x => x.CreatedOnUtc);

            if (orderBy == ProductSortingEnum.Position && categoryIds != null && categoryIds.Any())
            {
                //category position
                builderSort = Builders<Product>.Sort.Ascending(x => x.DisplayOrderCategory);
            }
            else if (orderBy == ProductSortingEnum.Position && !String.IsNullOrEmpty(manufacturerId))
            {
                //manufacturer position
                builderSort = Builders<Product>.Sort.Ascending(x => x.DisplayOrderManufacturer);
            }
            else if (orderBy == ProductSortingEnum.Position)
            {
                //otherwise sort by name
                builderSort = Builders<Product>.Sort.Ascending(x => x.Name);
            }
            else if (orderBy == ProductSortingEnum.NameAsc)
            {
                //Name: A to Z
                builderSort = Builders<Product>.Sort.Ascending(x => x.Name);
            }
            else if (orderBy == ProductSortingEnum.NameDesc)
            {
                //Name: Z to A
                builderSort = Builders<Product>.Sort.Descending(x => x.Name);
            }
            else if (orderBy == ProductSortingEnum.PriceAsc)
            {
                //Price: Low to High
                builderSort = Builders<Product>.Sort.Ascending(x => x.Price);
            }
            else if (orderBy == ProductSortingEnum.PriceDesc)
            {
                //Price: High to Low
                builderSort = Builders<Product>.Sort.Descending(x => x.Price);
            }
            else if (orderBy == ProductSortingEnum.CreatedOn)
            {
                //creation date
                builderSort = Builders<Product>.Sort.Ascending(x => x.CreatedOnUtc);
            }
            else if (orderBy == ProductSortingEnum.OnSale)
            {
                //on sale
                builderSort = Builders<Product>.Sort.Descending(x => x.OnSale);
            }
            else if (orderBy == ProductSortingEnum.MostViewed)
            {
                //most viewed
                builderSort = Builders<Product>.Sort.Descending(x => x.Viewed);
            }
            else if (orderBy == ProductSortingEnum.BestSellers)
            {
                //best seller
                builderSort = Builders<Product>.Sort.Descending(x => x.Sold);
            }

            var products = new PagedList<Product>(_productRepository.Collection, filter, builderSort, pageIndex, pageSize);

            if (loadFilterableSpecificationAttributeOptionIds && !_catalogSettings.IgnoreFilterableSpecAttributeOption)
            {
                IList<string> specyfication = new List<string>();
                var filterSpecExists = filter &
                    builder.Where(x => x.ProductSpecificationAttributes.Count > 0);
                var productSpec = _productRepository.Collection.Find(filterSpecExists).Limit(1);
                if (productSpec != null)
                {
                    var qspec = _productRepository.Collection
                    .Aggregate()
                    .Match(filter)
                    .Unwind(x => x.ProductSpecificationAttributes)
                    .Project(new BsonDocument
                        {
                        {"AllowFiltering", "$ProductSpecificationAttributes.AllowFiltering"},
                        {"SpecificationAttributeOptionId", "$ProductSpecificationAttributes.SpecificationAttributeOptionId"}
                        })
                    .Match(new BsonDocument("AllowFiltering", true))
                    .Group(new BsonDocument
                            {
                                        {"_id",
                                            new BsonDocument {
                                                { "SpecificationAttributeOptionId", "$SpecificationAttributeOptionId" },
                                            }
                                        },
                                        {"count", new BsonDocument
                                            {
                                                { "$sum" , 1}
                                            }
                                        }
                            })
                    .ToListAsync().Result;
                    foreach (var item in qspec)
                    {
                        var so = item["_id"]["SpecificationAttributeOptionId"].ToString();
                        specyfication.Add(so);
                    }
                }

                filterableSpecificationAttributeOptionIds = specyfication;
            }

            return products;

            #endregion

        }

        private string StringReplace(string text)
        {
            text = text.Replace("İ", "I");
            text = text.Replace("ı", "i");
            text = text.Replace("Ğ", "G");
            text = text.Replace("ğ", "g");
            text = text.Replace("Ö", "O");
            text = text.Replace("ö", "o");
            text = text.Replace("Ü", "U");
            text = text.Replace("ü", "u");
            text = text.Replace("Ş", "S");
            text = text.Replace("ş", "s");
            text = text.Replace("Ç", "C");
            text = text.Replace("ç", "c");
            return text.ToLower();
        }

        string StringReplaceToTurkishWithUnicode(string text)
        {
            // Türkçe karakterlerin Unicode kodları ve karşılık gelen ASCII karakterler
            string[,] turkishCharMap = new string[,] {
                { "\u0131", "i" }, // ı -> i
                { "\u0049", "ı" }, // İ -> ı
                { "\u00E7", "c" }, // ç -> c
                { "\u00C7", "C" }, // Ç -> C
                { "\u015F", "s" }, // ş -> s
                { "\u015E", "S" }, // Ş -> S
                { "\u011F", "g" }, // ğ -> g
                { "\u011E", "G" }, // Ğ -> G
                { "\u00FC", "u" }, // ü -> u
                { "\u00DC", "U" }, // Ü -> U
                { "\u00F6", "o" }, // ö -> o
                { "\u00D6", "O" }  // Ö -> O
            };

            // Türkçe karakterlerin Unicode kodlarını içeren Regex deseni
            string regexPattern = string.Join("|", turkishCharMap);

            // Regex.Replace metodu ile Türkçe karakterleri değiştirme
            return Regex.Replace(text, regexPattern, m =>
            {
                for (int i = 0; i < turkishCharMap.GetLength(0); i++)
                {
                    if (m.Value.Equals(turkishCharMap[i, 0]))
                        return turkishCharMap[i, 1];
                }
                return m.Value; // Eşleşme yoksa orijinal karakteri geri döndür
            });
        }
    }
}
