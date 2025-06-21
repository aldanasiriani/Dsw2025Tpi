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
        public string email { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }

        public Customer(string email, string name, string phoneNumber)
        {
            this.email = email;
            this.name = name;
            this.phoneNumber = phoneNumber;
        }
    }
}
