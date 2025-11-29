namespace Dsw2025Tpi.Application.Dtos
{
    public class PagedResult<T>
    {
        public int TotalCount { get; set; } // Total de productos en la BD (ej: 50)
        public int Page { get; set; }       // Página actual (ej: 1)
        public int PageSize { get; set; }   // Cuántos por página (ej: 10)
        public IEnumerable<T> Items { get; set; } // La lista de productos
    }
}