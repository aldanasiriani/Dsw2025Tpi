using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product : EntityBase
    {
        [Required]
        public string Sku { get;  set; }

        [Required]
        public string InternalCode { get;  set; }

        [Required]
        public string Name { get;  set; }

        [Required]
        public string Description { get;  set; }

        [Required]
        public decimal CurrentUnitPrice { get; set; }


        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Product(string sku, string internalCode, string name, string description,
               decimal currentUnitPrice, int stockQuantity, bool isActive)
        {
        

            this.Sku = sku;
            this.InternalCode = internalCode;
            this.Name = name;
            this.Description = description;
            this.CurrentUnitPrice = currentUnitPrice;
            this.StockQuantity = stockQuantity;
            this.IsActive = isActive;
        }

        public Product()
        {
           
        }

    }
}
