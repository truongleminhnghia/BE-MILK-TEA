using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    //[Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly Source _source;

        public AccountController(IAccountService accountService, IJwtService jwtService, IMapper mapper, Source source, ICustomerService customerService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _source = source;
            _customerService = customerService;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomer();
            var customerResponses = _mapper.Map<IEnumerable<CustomerResponse>>(customers);
            return Ok(customerResponses);
        }

        [HttpPost("customers")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var customer = await _customerService.CreateCustomer(request);
                var customerResponse = _mapper.Map<CustomerResponse>(customer);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpPut("customers/{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            try
            {
                var customer = await _customerService.UpdateCustomer(id, request);
                var customerResponse = _mapper.Map<CustomerResponse>(customer);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpGet("customers/{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customer = await _customerService.GetById(id);
            if (customer == null)
            {
                return NotFound(new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Cannot be found!"));
            }
            var customerResponse = _mapper.Map<CustomerResponse>(customer);
            return Ok(customerResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            var accounts = await _accountService.GetAllAccount();
            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
            return Ok(accountResponses);
        }

        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            var account = await _accountService.GetById(id);
            if (account == null)
            {
                return NotFound(new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Cannot be found!"));
            }
            var accountResponse = _mapper.Map<AccountResponse>(account);
            return Ok(accountResponse);
        }

        [HttpPut("account/{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                var account = await _accountService.UpdateAccount(id, request);
                var accountResponse = _mapper.Map<AccountResponse>(account);
                return Ok(accountResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }



    }
}