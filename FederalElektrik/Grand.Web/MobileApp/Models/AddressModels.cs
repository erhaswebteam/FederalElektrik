namespace Grand.Web.MobileApp.Models
{
    public class AddressModel
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string VatNumber { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string Address { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SaveOrderAddressRequestModel
    {
        public string AddressType { get; set; }
        public string AddressId { get; set; }
    }
    public class DeleteAddressRequestModel
    {
        public string AddressId { get; set; }
    }
}
