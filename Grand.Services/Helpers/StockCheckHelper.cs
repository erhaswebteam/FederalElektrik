using Grand.Core.Domain.Catalog;
using Grand.Core.Infrastructure;
using Grand.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Grand.Services.Helpers
{
    public static class StockCheckHelper
    {
        public static bool CheckStock(Product product, int quantityInCart, string attributesXml)
        {
            if (CheckControlSkip(product))
                return true;

            if (product == null || quantityInCart == 0)
                return false;

            var sku = product.ManageInventoryMethodId == 2 ? product.ProductAttributeCombinations.First(x => attributesXml.Contains(x.AttributesXml)).Sku : product.Sku;
            int pimStock = string.IsNullOrEmpty(product.Gtin) ? GetPIMStockBySku(sku) : GetPIMStockByGtin(product.Gtin);
            var mongoStock = product.ManageInventoryMethodId == 2 ? product.ProductAttributeCombinations.First(x => attributesXml.Contains(x.AttributesXml)).StockQuantity : product.StockQuantity;

            if (mongoStock != pimStock)
                UpdateMongoStock(product, pimStock, attributesXml);

            return quantityInCart <= pimStock;
        }

        public static int GetPIMStockBySku(string sku)
        {
            int pimStock = 0;

            using (SqlConnection con = new SqlConnection("data source=10.0.0.200;initial catalog=PIM;persist security info=True;user id=erhaspim;password=ERHASpim415263;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                using (SqlCommand cmd = new SqlCommand($"select Stock from ER_LogoStock where TypeId = 6 and LogoCode = '{sku}'", con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pimStock = int.Parse(reader["Stock"].ToString());
                    }
                    con.Close();
                }
            }

            return pimStock;
        }

        public static int GetPIMStockByGtin(string gtin)
        {
            int pimStock = 0;

            using (SqlConnection con = new SqlConnection("data source=10.0.0.200;initial catalog=PIM;persist security info=True;user id=erhaspim;password=ERHASpim415263;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                using (SqlCommand cmd = new SqlCommand($"select Stock from ER_LogoStock where TypeId = 0 and LogoCode = '{gtin}'", con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pimStock = int.Parse(reader["Stock"].ToString());
                    }
                    con.Close();
                }
            }

            return pimStock;
        }

        public static void UpdateMongoStock(Product product, int pimStock, string attributesXml = null)
        {
            if (product.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes)
            {
                var combination = product.ProductAttributeCombinations.First(x => attributesXml.Contains(x.AttributesXml));
                combination.StockQuantity = pimStock;
                combination.ProductId = product.Id;
                EngineContext.Current.Resolve<IProductAttributeService>().UpdateProductAttributeCombination(combination);
            }
            else
            {
                product.StockQuantity = pimStock;
                EngineContext.Current.Resolve<IProductService>().UpdateProduct(product);
            }

        }

        public static bool CheckControlSkip(Product product)
        {
            var categories = EngineContext.Current.Resolve<ICategoryService>().GetAllCategories();
            var category = categories.FirstOrDefault(x => x.Name == "LOGOSTOKLU");

            //Sistemde böyle bir kategori yoksa
            if (category == null)
                return true;

            var result = !product.ProductCategories.Any(x => x.CategoryId == category.Id);

            return result;
        }

        public static void DecreasePIMStock(Product product, int quantity, string attributesXml = null)
        {
            if (product == null || quantity <= 0 || CheckControlSkip(product))
                return;

            var logoCode = product.ManageInventoryMethodId == 2 ? product.ProductAttributeCombinations.First(x => attributesXml.Contains(x.AttributesXml)).Sku : product.Sku;
            
            int pimStock = string.IsNullOrEmpty(product.Gtin) ? GetPIMStockBySku(logoCode) : GetPIMStockByGtin(product.Gtin);
            int typeId = string.IsNullOrEmpty(product.Gtin) ? 6 : 0;
            logoCode = typeId == 6 ? logoCode : product.Gtin;

            using (SqlConnection con = new SqlConnection("data source=10.0.0.200;initial catalog=PIM;persist security info=True;user id=erhaspim;password=ERHASpim415263;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                using (SqlCommand cmd = new SqlCommand($"UPDATE ER_LogoStock SET Stock = {pimStock - quantity} WHERE LogoCode = '{logoCode}' and TypeId = {typeId}", con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    cmd.ExecuteReader();
                    con.Close();
                }
            }
        }
        
        //public static void DecreaseStockMongo(Product product, int quantity)
        //{
        //    var inDbQuantity = product.ManageInventoryMethodId == 2 ? product.ProductAttributeCombinations.First(x => x.Sku == product.Sku).StockQuantity : product.StockQuantity;
        //    var updatedQuantity = inDbQuantity - quantity;

        //    if (product.ManageInventoryMethodId == 2)
        //        product.ProductAttributeCombinations.First(x => x.Sku == product.Sku).StockQuantity = updatedQuantity;
        //    else
        //        product.StockQuantity = updatedQuantity;

        //    EngineContext.Current.Resolve<IProductService>().UpdateProduct(product);
        //}
    }
}
