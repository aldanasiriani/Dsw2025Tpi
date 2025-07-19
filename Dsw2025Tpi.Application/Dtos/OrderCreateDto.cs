using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
  public class OrderCreateDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        public string BillingAddress { get; set; } = string.Empty;


        [Required]
        public string Notes { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Debe haber al menos un producto en la orden.")]
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }
}
