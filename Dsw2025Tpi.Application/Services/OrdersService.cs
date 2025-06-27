using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

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
            var customer = await _customerRepo.GetById(dto.CustomerId);
            if (customer == null)
                throw new InvalidOperationException("Cliente no encontrado");

            var order = new Order(DateTime.UtcNow, dto.ShippingAddress, dto.BillingAddress, dto.Notes)
            {
                Customer = customer,
                Status = OrderStatus.Pending
            };

            foreach (var itemDto in dto.OrderItems)
            {
                var product = await _productRepo.GetById(itemDto.ProductId);
                if (product == null)
                    throw new InvalidOperationException($"Producto {itemDto.ProductId} no encontrado.");

                if (product.StockQuantity < itemDto.Quantity)
                    throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");

                // Descontar stock
                product.StockQuantity -= itemDto.Quantity;
                await _productRepo.Update(product);

                var orderItem = new OrderItem(itemDto.Quantity, product.CurrentUnitPrice);
                order.OrderItems.Add(orderItem);
            }

            await _orderRepo.Add(order);
            return order;
        }
    }
}
