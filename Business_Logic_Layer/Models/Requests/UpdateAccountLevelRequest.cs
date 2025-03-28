using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class UpdateAccountLevelRequest
    {
        public AccountLevelEnum AccountLevel { get; set; }
    }
}
