using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    internal class CartResponse
    {
        public Guid Id { get; set; }  // ID của giỏ hàng
        public required string Account { get; set; } // Thông tin tài khoản (có thể đổi sang AccountResponse nếu có)
        public int Count { get; set; } // Số lượng items trong giỏ

        public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();

        // Constructor
        //public CartResponse(Guid id, string account, int count)
        //{
        //    Id = id;
        //    Account = account;
        //    Count = count;
        //}
    }
}
