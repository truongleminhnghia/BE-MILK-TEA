using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class OrderRequest
    {
        public string OrderCode { get; set; } = string.Empty;
        public OrderStatus orderStatus { get; set; }

        public string FullNameShipping { get; set; } = string.Empty;
        public string PhoneShipping { get; set; } = string.Empty;
        public string? EmailShipping { get; set; }
        public string? NoteShipping { get; set; }
        public string AddressShipping { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string RefCode { get; set; } = string.Empty;

        public List<CartDetailDTO> cartDetailList;
      
    }
}
