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
        [Required(ErrorMessage = "Email không được bỏ trống")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ không được bỏ trống")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        public string PhoneNumber { get; set; }

    }

    public class CreateCustomerRequest
    {
        [Required(ErrorMessage = "Account id is required")]
        public Guid AccountId { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }
    }

    public class CreateStaffRequest
    {
        [Required(ErrorMessage = "Account id is required")]
        public Guid AccountId { get; set; }
    }
}
