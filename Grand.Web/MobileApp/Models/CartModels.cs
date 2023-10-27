namespace Grand.Web.MobileApp.Models
{
    public class AddToCartRequestModel
    {
        public string ProductId { get; set; }

        public int ShoppingCartTypeId { get; set; }

        public int Quantity { get; set; }
    }

    public class UpdateCartRequestModel
    {
        public string ItemId { get; set; }

        public int NewQuantity { get; set; }

        public string AttributesXml { get; set; }
    }

    public class DeleteCartRequestModel
    {
        public int ShoppingCartTypeId { get; set; }

        public string ProductId { get; set; }
    }
}
