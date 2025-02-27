using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class UpdateAccountRequest
    {
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }

        public UpdateCustomerRequest? Customer { get; set; }
        public UpdateStaffRequest? Employee { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string TaxCode { get; set; }
        public string Address { get; set; }
    }

    public class UpdateStaffRequest
    {
        public string RefCode { get; set; }
    }
}