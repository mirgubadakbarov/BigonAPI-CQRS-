using Bigon.Business.Modules.AccountModule.Commands.RefreshTokenCommand;
using Bigon.Business.Modules.AccountModule.Commands.SigninCommand;
using Bigon.Infrastructure.Services.Abstracts;
using Bigon.Infrastructure.Services.Concrates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bigon.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;
        private readonly IUserManager _userManager;

        public AccountController(IMediator mediator, IJwtService jwtService, IUserManager userManager)
        {
            _mediator = mediator;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Signin(SigninRequest request)
        {
            var user = await _mediator.Send(request);

            var token = _jwtService.GenerateAccesstoken(user);
            var refreshToken = await _userManager.GenerateRefreshTokenAsync(null, token);

            return Ok(
                new
                {
                    access_token = token,
                    refresh_token = refreshToken
                });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromHeader] RefreshTokenRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(
                new
                {
                    access_token = response.AccessToken,
                    refresh_token = response.RefreshToken
                });
        }
    }
}
