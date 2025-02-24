using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.AutoMappers;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using System.Text.Unicode;
using System.Web;

namespace Business_Logic_Layer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Account?> GetByEmail(string _email)
        {
            try
            {
                if (_email == null)
                {
                    throw new Exception("Id do not null");
                }
                var _account = await _accountRepository.GetByEmail(_email);
                if (_account == null)
                {
                    throw new Exception("Account do not exits");
                }
                return _account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<Account?> GetById(string _id)
        {
            try
            {
                if (_id == null)
                {
                    throw new Exception("Id do not null");
                }
                var _account = await _accountRepository.GetById(_id);
                if (_account == null)
                {
                    throw new Exception("Account do not exits");
                }
                return _account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }
    }
}

// đk -> GG, FB, LOCAL (params) (GG, FB ko sử dụng MK, và mặc định với ROLE_CUSTOMER) 
// khi nó là LOCAL phải check ROLE là ADMIN hay ko phải ADMIN, nếu là ADMIN mở full ROLE nếu ko PHẢI ADMIN thì mặc định nó ra user