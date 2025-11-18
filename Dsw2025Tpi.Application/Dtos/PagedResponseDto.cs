using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public class PagedResponseDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public List<T> Data { get; set; } = new List<T>();

        public PagedResponseDto(List<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            this.TotalRecords = totalRecords;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            // Calcula el número total de páginas
            this.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }
}
