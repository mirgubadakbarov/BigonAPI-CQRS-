
using Bigon.Infrastructure.Exceptions;
using Bigon.Infrastructure.Extensions;
using Bigon.Infrastructure.Services.Abstracts;
using Bigon.Infrastructure.Services.Concrates;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigon.Business.Modules.AccountModule.Commands.RefreshTokenCommand
{
    internal class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
    {
        private readonly IJwtService _jwtService;
        private readonly ICryptoService _cryptoService;
        private readonly IUserManager _userManager;
        private readonly IIdentityService _identityService;
        private readonly IActionContextAccessor _ctx;

        public RefreshTokenRequestHandler(IJwtService jwtService,
            ICryptoService cryptoService,
            IUserManager userManager,
            IIdentityService identityService,
            IActionContextAccessor ctx)
        {
            _jwtService = jwtService;
            _cryptoService = cryptoService;
            _userManager = userManager;
            _identityService = identityService;
            _ctx = ctx;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetPrincipalId();

            if (!userId.HasValue)
            {
                var errors = new Dictionary<string, IEnumerable<string>>
                {
                    ["BAD_ACCESS_TOKEN"] = new[] { "BAD_ACCESS_TOKEN" }
                };

                throw new BadRequestException("BAD_DATA", errors);
            };

            request.Token = _ctx.GetHeaderValue("refresh_token");

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                var errors = new Dictionary<string, IEnumerable<string>>
                {
                    ["BAD_REFRESH_TOKEN"] = new[] { "BAD_REFRESH_TOKEN" }
                };

                throw new BadRequestException("BAD_DATA", errors);
            }

            request.Token = _cryptoService.ToMd5(request.Token);

            var user = await _userManager.GetUserByIdAsync(userId.Value, cancellationToken);

            if (await _userManager.ValidateRefreshTokenAsync(user, request.Token, cancellationToken))
            {

                var errors = new Dictionary<string, IEnumerable<string>>
                {
                    ["BAD_REFRESH_TOKEN"] = new[] { "BAD_REFRESH_TOKEN" }
                };

                throw new BadRequestException("BAD_DATA", errors);
            }

            var response = new RefreshTokenResponse
            {
                AccessToken = _jwtService.GenerateAccesstoken(user)
            };

            response.RefreshToken = await _userManager.GenerateRefreshTokenAsync(user, response.AccessToken, cancellationToken);

            return response;
        }
    }
}
