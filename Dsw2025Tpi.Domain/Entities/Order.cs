using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public List<OrderItem> OrderItems { get; set; } = new();

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string BillingAddress { get; set; }

        [Required]
        public string Notes { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        public Guid CustomerId { get; set; } // FK
        public Customer Customer { get; set; } //leer los valores de Customer 

        public Order(DateTime date, string shippingAddress, string billingAddress, string notes, Guid customerId)
        {
            this.Date = date;
            this.ShippingAddress = shippingAddress;
            this.BillingAddress = billingAddress;
            this.Notes = notes;
            this.CustomerId = customerId;
            this.Status = OrderStatus.Pending; 
        }
        
        public Order() { }
        public void CalcularTotalAmount()
        {
            TotalAmount = OrderItems.Sum(item => item.Subtotal);
        }
    }
}
