using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order
    {
        public List<OrderItem> OrderItems { get; set; } = new();

        public DateTime Date { get; private set; }
        public string ShippingAddress { get; private set; }
        public string BillingAddress { get; private set; }
        public string Notes { get; private set; }
        public decimal TotalAmount => OrderItems.Sum(item => item.Subtotal);

        public OrderStatus Status { get; private set; }
        public Customer Customer { get; private set; } //leer los valores de Customer 



        public Order(DateTime date, string shippingAddress, string billingAddress, string notes)
        {
            this.Date = date;
            this.ShippingAddress = shippingAddress;
            this.BillingAddress = billingAddress;
            this.Notes = notes;
        }
        
    }
}
