using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/admins")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public class AdminController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly Source _source;

        public AdminController(IAccountService accountService, IJwtService jwtService, IMapper mapper, Source source)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _source = source;
        }

        [HttpPost("staffs")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var staff = await _customerService.CreateCustomer(request);
                return Ok(staff);
            }
            catch
            {
                return BadRequest();
            }
            
        }
    }
}
