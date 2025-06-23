using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dsw2025Tpi.Domain.Entities
{
   public class OrderItem : EntityBase
    {
     public int Quantity { get;  set; }
     public decimal UnitPrice { get;  set; }
     public decimal Subtotal => Quantity * UnitPrice;

    public OrderItem(int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad no puede ser cero ni menor", nameof(quantity));
        if (unitPrice <= 0)
            throw new ArgumentException("El precio no puede ser cero ni menor", nameof(unitPrice));
        this.Quantity = quantity;
        this.UnitPrice = unitPrice;

        }
}
}