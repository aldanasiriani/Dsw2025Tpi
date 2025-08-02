using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
  public record OrderCreateDto
    {
        [Required(ErrorMessage = "Debe especificar un Id de cliente.")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Debe especificar un domicilio de envio.")]
        [MinLength(1, ErrorMessage = "El domicilio de envio no puede estar vacio.")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe especificar un domicilio de cobro.")]
        [MinLength(1, ErrorMessage = "El domicilio de cobro no puede estar vacio.")]
        public string BillingAddress { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Debe haber al menos un producto en la orden.")]
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }
}
