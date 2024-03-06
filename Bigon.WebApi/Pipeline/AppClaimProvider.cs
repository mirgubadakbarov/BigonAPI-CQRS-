using Bigon.Business.Modules.AccountModule.Commands.BindClaimsCommand;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Bigon.WebApi.Pipeline
{
    public class AppClaimProvider : IClaimsTransformation
    {
        private readonly IMediator _mediator;

        public AppClaimProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                await _mediator.Send(new BindClaimsRequest { Identity = identity });
            }
            return principal;
        }
    }
}
