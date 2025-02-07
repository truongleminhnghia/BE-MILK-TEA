using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class BaseEntity
    {
        private DateTime CreateAt { get; set; } = DateTime.UtcNow;
        private DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}
