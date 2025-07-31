using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductUpdateDto
    {      
           [Required]
           [MaxLength(50,ErrorMessage = "El SKU no puede superar los 50 caracteres.")]
           public string Sku { get; set; } = string.Empty;

           [MaxLength(20, ErrorMessage = "El InternalCode no puede superar los 20 caracteres.")]
           public string InternalCode { get; set; } = string.Empty;
           
           [Required]
           [MaxLength(50, ErrorMessage = "El Nombre no puede superar los 50 caracteres.")]
           public string Name { get; set; } = string.Empty;

           [MaxLength(60, ErrorMessage = "La descripcion no puede superar los 60 caracteres.")]
           public string Description { get; set; } = string.Empty;

           [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
           public decimal CurrentUnitPrice { get; set; }

           [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
           public int StockQuantity { get; set; }

           public bool IsActive { get; set; }
        
    }
}
