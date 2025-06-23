using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public class ProductUpdateDto
    {
        [Required]
        public string Sku { get; set; } = string.Empty;

        [Required]
        public string InternalCode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal CurrentUnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }

    }
}
