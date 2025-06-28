using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _clienteRepository;

        public CustomerService(ICustomerRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();

            return clientes.Select(c => new CustomerDto
            {
                CustomerId = c.Id,
                Name = c.Name,
                Email = c.Email
            }).ToList();
        }
    }
}

