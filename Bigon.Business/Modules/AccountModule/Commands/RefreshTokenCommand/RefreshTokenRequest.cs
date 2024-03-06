using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigon.Business.Modules.AccountModule.Commands.RefreshTokenCommand
{
    public class RefreshTokenRequest:IRequest<RefreshTokenResponse>
    {
        public string Token { get; set; }
    }
}
