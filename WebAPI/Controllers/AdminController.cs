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
    [Route("api/admins")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly Source _source;

        public AdminController(IAccountService accountService, IJwtService jwtService, IMapper mapper, Source source, IEmployeeService employeeService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _source = source;
            _employeeService = employeeService;
        }


        [HttpGet("staff/{id}")]
        public async Task<IActionResult> GetStaffById(Guid id)
        {
            var employee = await _employeeService.GetById(id);
            if (employee == null)
                return NotFound(new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Cannot found"));

            var employeeResponse = _mapper.Map<EmployeeResponse>(employee);
            
            return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Successfull", employeeResponse));
        }
    }
}
