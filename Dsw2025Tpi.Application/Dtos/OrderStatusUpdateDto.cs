using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderStatusUpdateDto
    {
        [Required(ErrorMessage ="El nuevo status debe ser entre 0 y 4")]
        public string NewStatus { get; set; } = string.Empty;
    }
}
