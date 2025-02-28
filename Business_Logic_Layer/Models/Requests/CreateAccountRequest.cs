using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }
    }

    public class CreateCustomerRequest
    {
        [Required(ErrorMessage = "Account id is required")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Tax code is required")]
        public string TaxCode { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
    }

    public class CreateStaffRequest
    {
        [Required(ErrorMessage = "Account id is required")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Ref code is required")]
        public string RefCode { get; set; }
    }
}
