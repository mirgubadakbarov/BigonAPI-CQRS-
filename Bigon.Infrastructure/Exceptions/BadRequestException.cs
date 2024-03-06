using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigon.Infrastructure.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message, IDictionary<string, IEnumerable<string>> errors) : base(message)
        {
            Errors = errors;
        }

        public BadRequestException(string message, IDictionary<string, IEnumerable<string>> errors, Exception innerException) : base(message, innerException)
        { Errors = errors; }


        public IDictionary<string, IEnumerable<string>> Errors { get; }
    }
}
