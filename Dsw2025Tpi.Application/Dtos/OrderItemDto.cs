using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderItemResponseDto
    {
        [Required]
        public Guid ProductId { get; set; }

        //  public string ProductName { get; set; } = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Quantity { get; set; }
      //  public decimal UnitPrice { get; set; }
    }
}
