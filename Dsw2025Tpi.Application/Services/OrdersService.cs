using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
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
            IRepository<Customer> customerRepo)
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
            var order = new Order(DateTime.UtcNow, dto.ShippingAddress, dto.BillingAddress, dto.Notes, dto.CustomerId) 
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
                var orderItem = new OrderItem(itemDto.Quantity, product.CurrentUnitPrice, product.Id, order.Id);
                orderItem.Product = product; 
                order.OrderItems.Add(orderItem);
            }

            // 4. Calcular TotalAmount de la orden
            order.CalcularTotalAmount(); 

            // 5. Añadir la orden a la base de datos
            await _orderRepo.Add(order); // Esto guardará la Order y sus OrderItems relacionados

            return order;
        }
        
        

       
        
    }
}