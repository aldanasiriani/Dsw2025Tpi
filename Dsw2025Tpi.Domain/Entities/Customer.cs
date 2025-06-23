using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{  
   public class Customer : EntityBase
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }


        public Customer(string email, string name, string phoneNumber)
        {
            this.Email = email;
            this.Name = name;
            this.PhoneNumber = phoneNumber;
        }
    }
}
