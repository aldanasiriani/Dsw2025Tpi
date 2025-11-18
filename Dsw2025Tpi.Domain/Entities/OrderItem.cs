using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dsw2025Tpi.Domain.Entities
{
   public class OrderItem : EntityBase
    {
        [Required]
        public int Quantity { get;  set; }
       
      /*  [Required]
        public decimal UnitPrice { get;  set; }

        [Required]
        public decimal Subtotal => Quantity * UnitPrice;*/

        [Required]
        public Guid ProductId { get; set; }  // FK
        public Product Product { get; set; } 

        [Required]
        public Guid OrderId { get; set; } // FK
        public Order Order { get; set; } 

        public OrderItem(int quantity, /*decimal unitPrice*/Guid productId, Guid orderId)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad no puede ser cero ni menor");
        /*if (unitPrice <= 0)
            throw new ArgumentException("El precio no puede ser cero ni menor", nameof(unitPrice));*/
        this.Quantity = quantity;
       // this.UnitPrice = unitPrice;
            this.ProductId = productId;
            this.OrderId = orderId;
            

        }
        
}
}