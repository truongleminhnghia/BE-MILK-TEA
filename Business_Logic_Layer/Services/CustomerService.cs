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
                var customer = _mapper.Map<Customer>(createCustomerRequest);
                customer.AccountLevel = AccountLevelEnum.NORMAL;
                customer.Purchased = false;
                customer.CreateAt = DateTime.Now;

                var result = await _customerRepository.Create(customer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public async Task<Customer> UpdateCustomer(Guid id, UpdateCustomerRequest updateCustomerRequest)
        {
            try
            {
                var customer = await _customerRepository.GetByAccountId(id);
                if (customer == null)
                {
                    throw new Exception("Account do not exits");
                }
                customer.UpdateAt = DateTime.Now;
                customer.TaxCode = updateCustomerRequest.TaxCode;
                customer.Address = updateCustomerRequest.Address;
                customer.AccountLevel = AccountLevelEnum.NORMAL;
                var result = await _customerRepository.UpdateCustomer(customer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomer()
        {
            try
            {
                var result = await _customerRepository.GetAllCustomer();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<Customer> GetById(Guid _id)
        {
            try
            {
                var result = await _customerRepository.GetByAccountId(_id);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }
    }
}
