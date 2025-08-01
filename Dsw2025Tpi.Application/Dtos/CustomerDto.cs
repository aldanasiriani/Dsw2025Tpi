using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record CustomerDto
    {
        public Guid CustomerId { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }

        public string PhoneNumber { get; set; }
    }

}

