using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
   public class Customer : EntityBase
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public Customer(string email, string name, string phoneNumber)
        {
            this.Email = email;
            this.Name = name;
            this.PhoneNumber = phoneNumber;
        }
    }
}
