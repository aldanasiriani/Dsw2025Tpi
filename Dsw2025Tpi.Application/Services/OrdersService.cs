using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System; // Necesario para Guid, DateTime, ArgumentException
using System.Linq; // Necesario para .Sum() en CalculateTotalAmount

namespace Dsw2025Tpi.Application.Services
{
    public class OrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Customer> _customerRepo;
        


        public OrderService(
            IRepository<Order> orderRepo,
            IRepository<Product> productRepo,
            IRepository<Customer> customerRepo
            )
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            
        }
       

        public async Task<Order> CreateOrderAsync(OrderCreateDto dto)
        {
            // 1. Validar que el cliente existe
            var customer = await _customerRepo.GetById(dto.CustomerId);
            if (customer == null)
                throw new EntityNotFoundException("Cliente no encontrado."); 

            // 2. Crear la Orden 
            var order = new Order(DateTime.UtcNow, dto.ShippingAddress, dto.BillingAddress, dto.CustomerId) 
            {
                Customer = customer, 
                Status = OrderStatus.Pending 
            };

            // 3. Procesar los OrderItems
            foreach (var itemDto in dto.OrderItems)
            {
                var product = await _productRepo.GetById(itemDto.ProductId);

                // Validaciones de producto
                if (product == null)
                    throw new EntityNotFoundException($"Producto {itemDto.ProductId} no encontrado.");

                if (!product.IsActive) 
                    throw new EntityNotFoundException($"El producto '{product.Name}' (SKU: {product.Sku}) no esta activo y no puede ser ordenado.");

                if (product.StockQuantity < itemDto.Quantity)
                    throw new BusinessRuleViolationException($"Stock insuficiente para el producto {product.Name}");

                // Descontar stock
                product.StockQuantity -= itemDto.Quantity;
                await _productRepo.Update(product);

                // Crear OrderItem y añadirlo a la colección de la orden
                var orderItem = new OrderItem(itemDto.Quantity, product.Id, order.Id);
                orderItem.Product = product; 
                order.OrderItems.Add(orderItem);
            }

            // 4. Calcular TotalAmount de la orden
            order.CalcularTotalAmount(); 

            // 5. Añadir la orden a la base de datos
            await _orderRepo.Add(order); // Esto guardará la Order y sus OrderItems relacionados

            return order;
        }

       // En OrderService.cs

// En OrderService.cs

// Fíjate que cambiamos el retorno de List<...> a PagedResult<...>
public async Task<PagedResult<OrderResponseDto>> GetOrdersAsync(int page, int pageSize, OrderStatus? status, Guid? customerId)
{
    // 1. Preparamos la consulta base
    var consulta = _orderRepo.Query()
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .AsQueryable();

    // 2. Aplicamos filtros (Aquí es donde funciona tu desplegable)
    if (status.HasValue)
    {
        consulta = consulta.Where(o => o.Status == status.Value);
    }
    if (customerId.HasValue)
    {
        consulta = consulta.Where(o => o.CustomerId == customerId.Value);
    }

    // 3. Contar Total (CRUCIAL para que el Frontend sepa cuántas páginas hay)
    var totalCount = await consulta.CountAsync();

    // 4. Paginación y Ordenamiento
    var items = await consulta
        .OrderByDescending(o => o.Date) // Las más recientes primero
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // 5. Mapeo a DTO (Convertir Entidad a lo que ve el Frontend)
    var itemDtos = items.Select(o => new OrderResponseDto
    {
        Id = o.Id,
        CustomerId = o.CustomerId,
        Date = o.Date,
        ShippingAddress = o.ShippingAddress,
        BillingAddress = o.BillingAddress,
        Status = o.Status.ToString(),
        TotalAmount = o.TotalAmount,
        OrderItems = o.OrderItems.Select(i => new OrderItemResponseDto
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            // Agrega más datos del producto si los necesitas
        }).ToList()
    }).ToList();

    // 6. Retornar el objeto Paginado (Esto es lo que el Frontend espera)
    return new PagedResult<OrderResponseDto>
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Items = itemDtos
    };
}





        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepo.Query()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Date = order.Date,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }


        private bool EsTransicionPermitida(OrderStatus actual, OrderStatus nuevo)
        {
            return actual != nuevo; 
        }


        public async Task<OrderResponseDto?> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
             if (!Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var parsedStatus) ||
        !Enum.IsDefined(typeof(OrderStatus), parsedStatus))
    {
        var validStatuses = string.Join(", ", Enum.GetNames(typeof(OrderStatus)));
       throw new ArgumentException($"El estado '{newStatus}' no es válido. Estados válidos: {validStatuses}.");
   }

            var order = await _orderRepo.Query()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

          

            // Validar si la transicion es permitida 
            if (!EsTransicionPermitida(order.Status, parsedStatus))
                throw new ArgumentException($"No se puede cambiar el estado de {order.Status} a {parsedStatus}.");

            order.Status = parsedStatus;

            await _orderRepo.Update(order);

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Date = order.Date,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                }).ToList()
            };
        }

    }
}