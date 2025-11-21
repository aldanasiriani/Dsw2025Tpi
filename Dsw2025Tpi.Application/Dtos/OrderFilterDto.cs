using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public class OrderFilterDto
    {
        public OrderStatus? Status { get; set; }
        public Guid? CustomerId { get; set; }
    }

}
