using Grand.Core.Domain.Common;

namespace Grand.Web.MobileApp.Models
{
    public class CustomerInfoRequestModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string VatNumber { get; set; }
    }

    public class UpdateCustomerRequestModel
    {
        public UpdateCustomerRequestModel()
        {
            Address = new Address();
        }

        public string AddressId { get; set; }

        public Address Address { get; set; }
    }

    public class PasswordRequestModel
    {
        public string Email { get; set; }
    }

    public class PasswordChangeRequestModel
    {
        public string Password { get; set; }
    }
}
