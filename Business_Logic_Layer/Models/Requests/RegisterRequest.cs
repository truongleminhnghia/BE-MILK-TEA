using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class RegisterRequest
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
}