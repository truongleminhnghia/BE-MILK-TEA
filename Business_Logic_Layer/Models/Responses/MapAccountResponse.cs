using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class MapToAccountResponse
    {
        public static AccountResponse ComplexAccountResponse(Account account)
        {
            return new AccountResponse
            {
                Id = account.Id,
                FirstName = account.FirstName,
                Email = account.Email,
                LastName = account.LastName,
                AccountStatus = account.AccountStatus,
                Phone = account.Phone,
                RoleName = account.RoleName,

                // Safe mapping: Only map Employee if it exists
                Employee = account.Employee != null ? new EmployeeResponse
                {
                    Id = account.Employee.Id,
                    RefCode = account.Employee.RefCode
                } : null,  // If Employee is null, set EmployeeResponse to null

                // Safe mapping: Only map Customer if it exists
                Customer = account.Customer != null ? new CustomerResponse
                {
                    Id = account.Customer.Id,
                    TaxCode = account.Customer.TaxCode,
                    Address = account.Customer.Address,
                    AccountLevel = account.Customer.AccountLevel,
                    Purchased = account.Customer.Purchased
                } : null  // If Customer is null, set CustomerResponse to null
            };
        }
    }
    
}
