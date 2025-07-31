using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string InternalCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CurrentUnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}
