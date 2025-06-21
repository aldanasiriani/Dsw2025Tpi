using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product
    {
        public string sku { get;  set; }
        public string internalCode { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public decimal currentUnitPrice { get; private set; }
        public int stockQuantity { get; private set; }
        public bool isActive { get; private set; }

        public Product(string sku, string internalCode, string name, string description,
               decimal currentUnitPrice, int stockQuantity, bool isActive)
        {
            this.sku = sku;
            this.internalCode = internalCode;
            this.name = name;
            this.description = description;
            this.currentUnitPrice = currentUnitPrice;
            this.stockQuantity = stockQuantity;
            this.isActive = isActive;
        }


    }
}
