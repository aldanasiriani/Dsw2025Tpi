using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public class PagingParametersDto
    {
        // Define el tamaño máximo de página por seguridad
        private const int MaxPageSize = 50;

        // Número de página por defecto: 1
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        // Cantidad de elementos por página, limitado a MaxPageSize
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
