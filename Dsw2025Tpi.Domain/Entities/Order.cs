using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public List<OrderItem> OrderItems { get; set; } = new();

        public DateTime Date { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string Notes { get; set; }
        public decimal TotalAmount => OrderItems.Sum(item => item.Subtotal);

        public OrderStatus Status { get; set; }
        public Customer Customer { get; set; } //leer los valores de Customer 



        public Order(DateTime date, string shippingAddress, string billingAddress, string notes)
        {
            this.Date = date;
            this.ShippingAddress = shippingAddress;
            this.BillingAddress = billingAddress;
            this.Notes = notes;
        }
        
    }
}
