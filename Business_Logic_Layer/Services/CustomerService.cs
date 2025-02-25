using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<Customer> CreateCustomer(CreateCustomerRequest createCustomerRequest)
        {
            try
            {
                foreach (var item in createCustomerRequest.GetType().GetProperties())
                {
                    if (item.GetValue(createCustomerRequest) == null)
                    {
                        throw new Exception("Request do not null!");
                    }
                }
                var account = await _accountRepository.GetById(createCustomerRequest.AccountId);
                if(account == null)
                {
                    throw new Exception("Account do not exits");
                }
                var customer = _mapper.Map<Customer>(createCustomerRequest);

                var result = await _customerRepository.Create(customer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}
