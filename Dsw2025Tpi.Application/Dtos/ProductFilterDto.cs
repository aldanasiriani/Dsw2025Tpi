using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public class ProductFilterDto : PagingParametersDto
    {
        // Aquí defines cualquier campo de filtrado adicional
      //  public string? SearchTerm { get; set; }

        // Opcional: para filtrar por estado
      //  public bool? IsActive { get; set; }

        public bool? IsActiveFilter { get; set; }   
    }
}
