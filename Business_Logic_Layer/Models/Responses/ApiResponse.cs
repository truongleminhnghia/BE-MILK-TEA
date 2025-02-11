using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class ApiResponse
    {
        public HttpStatusCode Code { get; set; } // 200 400 401 403
        public string? Message { get; set; } // thường true ko có message, false
        public bool Success { get; set; } // true/false

        public object? Data { get; set; } // // thường true ==> data,

        public ApiResponse(HttpStatusCode code, bool success, string? message = null, object? data = null)
        {
            Code = code;
            Success = success;
            Message = message;
            Data = data;
        }
    }
}