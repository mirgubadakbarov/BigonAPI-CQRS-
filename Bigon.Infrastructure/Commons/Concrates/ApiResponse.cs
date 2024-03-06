using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bigon.Infrastructure.Commons.Concrates
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }
        public IDictionary<string, IEnumerable<string>> Errors { get; set; }

        public static ApiResponse Succes(string message = null, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            return new ApiResponse { StatusCode = httpStatusCode, Message = message, Error = false };
        }

        public static ApiResponse<T> Succes<T>(T data, string message = null, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        where T : class

        {
            return new ApiResponse<T> { StatusCode = httpStatusCode, Message = message, Error = false, Data = data };
        }

        public static ApiResponse Fail(string message = null, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            return new ApiResponse { StatusCode = httpStatusCode, Message = message, Error = true };
        }

        public static ApiResponse Fail(IDictionary<string, IEnumerable<string>> errors, string message = null, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            return new ApiResponse { StatusCode = httpStatusCode, Message = message, Error = true, Errors = errors };
        }


    }

    public class ApiResponse<T> : ApiResponse
    where T : class
    {
        public T Data { get; set; }
    }
}
